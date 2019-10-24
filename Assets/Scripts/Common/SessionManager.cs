using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Boomlagoon.JSON;
using UnityEngine.Analytics;
using UnityEngine.Networking;

public class SessionManager : MonoBehaviour
{

    List<User> users;
    public User activeUser;
    public Kid activeKid;
    public bool main = false;
    string syncProfileURL = Keys.Api_Web_Key + "api/profile/sync/";
    string syncLevelsURL = Keys.Api_Web_Key + "api/v2/levels/children/";
    string updateProfileURL = Keys.Api_Web_Key + "api/profile/update/";
    public List<Kid> temporalKids;

    string idStrings;
    int kidsIAP;

    bool downlodingData;

    Firebase.FirebaseApp firebaseApp;

    void Awake()
    {
        if (FindObjectsOfType<SessionManager>().Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp, i.e.
                    //   app = Firebase.FirebaseApp.DefaultInstance;
                    // where app is a Firebase.FirebaseApp property of your application class.
                    firebaseApp = Firebase.FirebaseApp.DefaultInstance;
                    // Set a flag here indicating that Firebase is ready to use by your
                    // application.
                    //GameObject.FindGameObjectWithTag("Coin").GetComponent<UnityEngine.UI.Text>().text = "firebase is set All right";
                }
                else
                {
                    // Firebase Unity SDK is not safe to use here
                    //GameObject.FindGameObjectWithTag("Coin").GetComponent<UnityEngine.UI.Text>().text = $"firebase is not correctly set: {dependencyStatus}";
                }
            });
        }
        //if(main)
        //PlayerPrefs.DeleteAll ();
        string version = Application.version;
        if (PlayerPrefs.GetString(Keys.Version_Last_Season) != version)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString(Keys.Version_Last_Season, version);
        }
        StartAgain();
    }

    public void StartAgain()
    {
        activeUser = null;
        activeKid = null;
        users = new List<User>();
        temporalKids = new List<Kid>();
        LoadSession();

        if (users.Count == 0)
        {
            AddUser("_local", "", "_local", null, 0);
            if (activeUser != null)
            {
                activeUser.kids.Add(new Kid(0, "local_kid", "_local", false, false));
            }
            activeUser = null;
            SaveSession();
        }
        if (main)
        {
            /*if(PlayerPrefs.GetInt("SubscriptionTrial",0)==0)
            {
                PlayerPrefs.SetString("activeUser","");	
            }
            else
            {
                LoadActiveUser();
            }
            PlayerPrefs.SetInt("activeKid",-1);*/
        }
        else
        {
            if (FindObjectOfType<DemoKey>())
            {
                activeUser = GetUser("_local");
                activeKid = GetKid(0);
            }
            else
            {
                string userS = PlayerPrefs.GetString(Keys.Active_User_Key);
                if (userS != "")
                {
                    Debug.Log(userS);
                    activeUser = GetUser(userS);
                    int kidS = PlayerPrefs.GetInt("activeKid", -1);
                    if (kidS != -1)
                    {
                        activeKid = GetKid(kidS);
                    }
                }
            }
        }
    }

    public bool IsDownlodingData()
    {
        return downlodingData;
    }

    public int GetNumberOfUsers()
    {
        return users.Count;
    }

    public User GetUser(string key)
    {
        for (int i = 0; i < users.Count; i++)
        {
            if (users[i].userkey == key)
            {
                return users[i];
            }
            for (int c = 0; c < users[i].kids.Count; c++)
            {
                if (users[i].kids[c].userkey == key || (key == "_local" && users[i].userkey == key))
                {
                    return users[i];
                }
            }
        }
        return null;
    }

    public Kid GetKid(int id)
    {
        for (int i = 0; i < activeUser.kids.Count; i++)
        {
            if (activeUser.kids[i].id == id)
            {
                return activeUser.kids[i];
            }
        }
        return null;
    }

    public bool FindUser(string username)
    {
        for (int i = 0; i < users.Count; i++)
        {
            if (users[i].username == username)
            {
                return true;
            }
        }
        return false;
    }

    public int TryLogin(string username, string psswd)
    {
        for (int i = 0; i < users.Count; i++)
        {
            if (users[i].username == username && users[i].psswdHash == psswd)
            {
                if (DateTime.Now <= users[i].suscriptionDate)
                {
                    activeUser = users[i];
                    activeKid = activeUser.kids[0];
                    PlayerPrefs.SetString("activeUser", activeKid.userkey);
                    PlayerPrefs.SetInt("activeKid", 1);
                    SyncProfiles(activeKid.userkey);
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
        }
        return -1;
    }

    public void Logout()
    {
        PlayerPrefs.SetString("activeUser", "");
        PlayerPrefs.SetInt("activeKid", -1);
        activeKid = null;
        activeUser = null;
    }

    public void LoadLocal()
    {
        activeUser = users[0];
        activeKid = activeUser.kids[0];
        PlayerPrefs.SetString("activeUser", "_local");
        PlayerPrefs.SetInt("activeKid", activeKid.id);
    }

    public void LoadActiveUser(string key)
    {
        activeUser = GetUser(key);
        /*string userS = PlayerPrefs.GetString("activeUser", "");
        if (userS != "")
        {
            activeUser = GetUser(userS);
            return true;
        }
        return false;*/
    }

    public void LoadUser(string username, string psswd, string key, string[] kids, int id)
    {
        bool missing = true;
        if (users.Count > 0)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].userkey == key)
                {
                    missing = false;
                    users[i].username = username;
                    users[i].psswdHash = psswd;
                    users[i].id = id;
                    SaveSession();
                    activeUser = users[i];
                    if (activeUser.kids.Count > 0)
                    {
                        activeKid = activeUser.kids[0];
                        PlayerPrefs.SetInt("activeKid", activeKid.id);
                    }
                    else
                    {
                        activeKid = null;
                        PlayerPrefs.SetInt("activeKid", -1);
                    }
                    PlayerPrefs.SetString("activeUser", activeUser.userkey);
                    //SyncProfiles(key);
                    break;
                }
            }
        }
        if (missing)
        {
            users.Add(new User(key, username, psswd, id));

            activeUser = users[users.Count - 1];
            if (activeUser.kids.Count > 0)
            {
                activeKid = activeUser.kids[0];
                PlayerPrefs.SetInt("activeKid", activeKid.id);
            }
            else
            {
                activeKid = null;
                PlayerPrefs.SetInt("activeKid", -1);
            }
            if (activeKid != null)
            {
                PlayerPrefs.SetString("activeUser", activeKid.userkey);
            }
            SaveSession();
        }
    }

    public void AddUser(string username, string psswd, string key, string[] kids, int id)
    {
        if (users.Count > 0)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].username == username)
                {
                    return;
                }
            }
        }
        users.Add(new User(key, username, psswd, id));
        activeUser = users[users.Count - 1];
        SaveSession();
    }

    public void AddKid(int kidID, string name, string key, bool actiive, bool trial)
    {
        //TODO Add Age
        activeUser.kids.Add(new Kid(kidID, name, key, actiive, trial));
        SaveSession();
    }

    public void AddKids(JSONArray kids)
    {
        temporalKids.Clear();
        for (int i = 0; i < kids.Length; i++)
        {
            JSONObject kidObj = kids[i].Obj;
            //bool missingUser = true;
            //bool missing = true;
            for (int u = 0; u < users.Count; u++)
            {
                if (users[u].userkey == kidObj.GetValue("key").Str)
                {
                    int cid = (int)kidObj.GetValue("cid").Number;
                    if (users[u].kids.Count >= kids.Length)
                    {
                        if (cid != users[u].kids[i].id)
                        {
                            string name = kidObj.GetValue("name").Str + " " + kidObj.GetValue("lastname").Str;
                            string key = kidObj.GetValue("key").Str;
                            users[u].kids.Add(new Kid(cid, name, key, kidObj.GetBoolean("active"), kidObj.GetBoolean("trial")));
                            temporalKids.Add(users[u].kids[users[u].kids.Count - 1]);
                        }
                    }
                    else
                    {
                        string name = kidObj.GetValue("name").Str + " " + kidObj.GetValue("lastname").Str;
                        string key = kidObj.GetValue("key").Str;
                        users[u].kids.Add(new Kid(cid, name, key, kidObj.GetBoolean("active"), kidObj.GetBoolean("trial")));
                        temporalKids.Add(users[u].kids[users[u].kids.Count - 1]);
                    }

                }
            }
        }
        SaveSession();
    }

    public string GetKidsIds()
    {
        string s = "";
        for (int i = 0; i < activeUser.kids.Count; i++)
        {
            if (!activeUser.kids[i].isActive)
            {
                if (activeUser.kids[i].isIAPSubscribed)
                {
                    kidsIAP++;
                    if (s == "")
                    {
                        s += activeUser.kids[i].id.ToString();
                    }
                    else
                    {
                        s += "," + activeUser.kids[i].id.ToString();
                    }
                }
            }
        }
        return s;
    }

    public void SetKid(string parentkey, int id)
    {
        for (int u = 0; u < users.Count; u++)
        {
            if (users[u].userkey == parentkey)
            {
                for (int i = 0; i < users[u].kids.Count; i++)
                {
                    if (users[u].kids[i].id == id)
                    {
                        activeKid = users[u].kids[i];
                    }
                }
            }
        }
        PlayerPrefs.SetInt("activeKid", id);
        //PlayerPrefs.SetString("activeUser", activeKid.userkey);
        SaveSession();
        SyncChildLevels();
    }

    public void SaveSession()
    {
        Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");

        /*
        XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
        MemoryStream stream = new MemoryStream();
        serializer.Serialize(stream, users);
        PlayerPrefs.SetString("sessions", Convert.ToBase64String(stream.GetBuffer()));
        */
        BinaryFormatter binaryFormater = new BinaryFormatter();

        //Create an in memory stream
        using (MemoryStream memoryStream = new MemoryStream())
        {
            binaryFormater.Serialize(memoryStream, users);
            //Add it to player prefs
            PlayerPrefs.SetString("sessions", Convert.ToBase64String(memoryStream.GetBuffer()));
        }

    }

    void LoadSession()
    {
        //Get the data
        string data = PlayerPrefs.GetString("sessions");
        //If not blank then load it
        if (!string.IsNullOrEmpty(data))
        {
            Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
            /*
            XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
            MemoryStream stream = new MemoryStream(Convert.FromBase64String(data));
            users = (List<User>)serializer.Deserialize(stream);
            */

            //Binary formatter for loading back
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(data)))
            {
                users = (List<User>)binaryFormatter.Deserialize(memoryStream);
            }
        }
    }

    public void SyncProfiles(string key)
    {
        StartCoroutine(PostSyncProfiles(key));
    }

    IEnumerator PostSyncProfiles(string key)
    {
        Debug.Log("Syncing now ");
        downlodingData = true;

        WWWForm form = new WWWForm();
        form.AddField("userKey", key);

        using (UnityWebRequest request = UnityWebRequest.Post(syncProfileURL, form))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                downlodingData = false;
                Debug.Log(request.downloadHandler.text);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                PlayerPrefs.SetString(Keys.Last_Play_Time, DateTime.Today.ToString(System.Globalization.DateTimeFormatInfo.InvariantInfo));

                JSONArray kids = JSONArray.Parse(request.downloadHandler.text);

                bool setType = false;
                bool needStoreSync = false;

                var cidsOfActiveKids = new List<int>();

                foreach (Kid k in activeUser.kids)
                {
                    cidsOfActiveKids.Add(k.id);
                }

                for (int i = 0; i < kids.Length; i++)
                {
                    JSONObject kidObj = kids[i].Obj;
                    JSONArray activeMissions = kidObj.GetArray("activeMissions");
                    JSONArray buyedItems = kidObj.GetArray("islandShoppingList");

                    int cid = (int)kidObj.GetNumber("cid");
                    string kidName = kidObj.GetString("name");
                    int index;

                    if (!cidsOfActiveKids.Contains(cid))
                    {
                        AddKid(cid, kidName, activeUser.userkey, kidObj.GetBoolean("active"), kidObj.GetBoolean("trial"));
                        index = activeUser.kids.Count - 1;
                    }
                    else
                    {
                        index = cidsOfActiveKids.IndexOf(cid);
                    }

                    activeUser.kids[index].isActive = kidObj.GetBoolean("active");
                    activeUser.kids[index].kiwis = (int)kidObj.GetNumber("kiwis");
                    if (kidObj.GetString("avatar") != null)
                    {
                        activeUser.kids[index].avatar = kidObj.GetString("avatar").ToLower();
                    }
                    else
                    {
                        activeUser.kids[index].avatar = "koala";
                    }
                    activeUser.kids[index].avatarClothes = kidObj.GetString("avatarClothes");
                    activeUser.kids[index].ownedItems = kidObj.GetString("ownedItems");
                    activeUser.kids[index].age = (int)kidObj.GetNumber("age");

                    activeUser.kids[index].ResetPlayedGames();

                    activeUser.kids[index].missionsToPlay = new List<int>();
                    for (int o = 0; o < activeMissions.Length; o++)
                    {
                        string gameKey = activeMissions[o].Str;
                        for (int gameName = 0; gameName < Keys.Number_Of_Games; gameName++)
                        {
                            if (gameKey == Keys.Game_Names[gameName])
                            {
                                activeUser.kids[index].missionsToPlay.Add(gameName);
                                break;
                            }
                        }
                    }

                    activeUser.kids[index].activeDay = (int)kidObj.GetNumber("activeDay");
                    activeUser.kids[index].ageSet = true;
                    activeUser.kids[index].firstsGames[0] = kidObj.GetBoolean("arbolFirstTime");
                    activeUser.kids[index].firstsGames[1] = kidObj.GetBoolean("arenaFirstTime");
                    activeUser.kids[index].firstsGames[2] = kidObj.GetBoolean("tesoroFirstTime");
                    activeUser.kids[index].firstsGames[3] = kidObj.GetBoolean("bolitaFirstTime");
                    activeUser.kids[index].firstsGames[4] = kidObj.GetBoolean("rioFirstTime");
                    activeUser.kids[index].firstsGames[5] = kidObj.GetBoolean("sombrasFirstTime");
                    activeUser.kids[index].firstsGames[6] = kidObj.GetBoolean("heladosFirstTime");

                    activeUser.kids[index].testAvailable = kidObj.GetBoolean("testAvailable");
                    activeUser.kids[index].sandLevelSet = kidObj.GetBoolean("arenaLevelSet");

                    activeUser.kids[index].buyedIslandObjects.Clear();
                    for (int o = 0; o < buyedItems.Length; o++)
                    {
                        activeUser.kids[index].buyedIslandObjects.Add((int)buyedItems[o].Number);
                    }

                    string type = kidObj.GetString("suscriptionType");
                    if (type == "monthly" || type == "quarterly")
                    {
                        setType = true;
                        PlayerPrefs.SetInt(Keys.First_Try, 1);
                    }
                    else if (type == "monthly_inApp" || type == "quarterly_inApp")
                    {
                        activeUser.kids[index].isIAPSubscribed = true;
                        PlayerPrefs.SetInt(Keys.First_Try, 1);
                    }

                    if (activeKid != null)
                    {
                        if (activeKid.id == cid)
                        {
                            activeKid = activeUser.kids[index];
                            SyncChildLevels();
                        }
                    }
                }
                if (!setType)
                {
                    activeUser.isPossibleBuyIAP = true;
                }
                if (needStoreSync)
                {
                    idStrings = GetKidsIds();
                    FindObjectOfType<MenuManager>().UpdateIAPSubscription(idStrings, kidsIAP);
                }
                SaveSession();
            }
        }
    }

    public void UpdateProfile()
    {
        StartCoroutine(PostUpdateProfile());
    }

    public void UpdateProfile(List<string> activeMissionsActive)
    {
        StartCoroutine(PostUpdateProfile(activeMissionsActive));
    }

    IEnumerator PostUpdateProfile(List<string> activeMissionsActive)
    {
        JSONArray array = new JSONArray();

        for (int i = 0; i < activeMissionsActive.Count; i++)
        {
            array.Add(activeMissionsActive[i]);
        }

        JSONObject data = UpdateJsonData(array);

        WWWForm form = new WWWForm();
        form.AddField("jsonToDb", data.ToString());

        using (UnityWebRequest request = UnityWebRequest.Post(updateProfileURL, form))
        {
            yield return request.SendWebRequest();

            Debug.Log(request.downloadHandler.text);
            if (request.isNetworkError || request.isHttpError)
            {
                if (FindObjectOfType<GameCenterManager>())
                {
                    FindObjectOfType<GameCenterManager>().ChangeMenus();
                }
                activeKid.syncProfile = false;
            }
            else
            {
                if (FindObjectOfType<GameCenterManager>())
                {
                    FindObjectOfType<GameCenterManager>().ChangeMenus();
                }
                activeKid.needSync = false;
            }
        }
        SaveSession();
    }

    IEnumerator PostUpdateProfile()
    {
        JSONArray array = new JSONArray();

        for (int i = 0; i < activeKid.missionsToPlay.Count; i++)
        {
            array.Add(Keys.Game_Names[activeKid.missionsToPlay[i]]);
        }

        JSONObject data = UpdateJsonData(array);

        WWWForm form = new WWWForm();
        form.AddField("jsonToDb", data.ToString());

        using (UnityWebRequest request = UnityWebRequest.Post(updateProfileURL, form))
        {
            yield return request.SendWebRequest();

            Debug.Log(request.downloadHandler.text);

            if (request.isNetworkError)
            {
                if (FindObjectOfType<GameCenterManager>())
                {
                    FindObjectOfType<GameCenterManager>().ChangeMenus();
                }
                activeKid.syncProfile = false;
            }
            else
            {
                if (FindObjectOfType<GameCenterManager>())
                {
                    FindObjectOfType<GameCenterManager>().ChangeMenus();
                }
                activeKid.needSync = false;
            }
        }
        SaveSession();
    }

    JSONObject UpdateJsonData(JSONArray array)
    {
        string shopingList = "";
        for (int i = 0; i < activeKid.buyedIslandObjects.Count; i++)
        {
            if (i == 0)
            {
                shopingList += activeKid.buyedIslandObjects[i].ToString();
            }
            else
            {
                shopingList += "," + activeKid.buyedIslandObjects[i].ToString();
            }
        }

        JSONObject data = new JSONObject
        {
            { "cid", activeKid.id },
            { "kiwis", activeKid.kiwis },
            { "avatar", activeKid.avatar },
            { "avatarclothes", activeKid.avatarClothes },
            { "activeDay", activeKid.activeDay },
            { "owneditems", activeKid.ownedItems },
            { "activemissions", array },
            { "activeday", activeKid.activeDay },
            { "rioFirstTime", activeKid.firstsGames[4] },
            { "tesoroFirstTime", activeKid.firstsGames[2] },
            { "arbolFirstTime", activeKid.firstsGames[0] },
            { "arenaFirstTime", activeKid.firstsGames[1] },
            { "sombrasFirstTime", activeKid.firstsGames[5]},
            { "bolitaFirstTime", activeKid.firstsGames[3] },    
            { "heladosFirstTime", activeKid.firstsGames[6]},
            { "tesoroLevelSet", true },
            { "arenaLevelSet", activeKid.sandLevelSet },
            { "arbolLevelSet", true },
            { "arenaLevelSet2", true },
            { "sombrasLevelSet", true },
            { "bolitaLevelSet", true },
            { "rioLevelSet", true },
            { "islandShoppingList", shopingList },
            { "userKey", activeKid.userkey }
        };

        return data;
    }

    public void SyncChildLevels()
    {
        StartCoroutine(PostChildSyncLevels());
    }

    IEnumerator PostChildSyncLevels()
    {
        downlodingData = true;
        DateTime today = DateTime.Now;

        WWWForm form = new WWWForm();
        form.AddField("userKey", activeKid.userkey);
        form.AddField("cid", activeKid.id);
        form.AddField("date", String.Format("{0:0000}-{1:00}-{2:00}", today.Year, today.Month, today.Day));
        //WWW hs_post = new WWW(post_url);

        using (UnityWebRequest request = UnityWebRequest.Post(syncLevelsURL, form))
        {
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {

            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                var json = JsonUtility.FromJson(request.downloadHandler.text, typeof(LevlSyncJson)) as LevlSyncJson;

                //Birds Level Set
                SetTheCorrectLevel(ref activeKid.birdsDifficulty, ref activeKid.birdsLevel, json.arbolMusicalLevel, json.arbolMusicalSublevel);

                //River Level Set
                SetTheCorrectLevel(ref activeKid.riverDifficulty, ref activeKid.riverLevel, json.rioLevel, json.rioSublevel);

                //Monkey Level Set
                SetTheCorrectLevel(ref activeKid.monkeyDifficulty, ref activeKid.monkeyLevel, json.monkeyLevel, json.monkeySublevel);

                //Lava Level Set
                SetTheCorrectLevel(ref activeKid.lavaDifficulty, ref activeKid.lavaLevel, json.sombrasLevel, json.sombrasSublevel);

                //Treasure Level Set
                SetTheCorrectLevel(ref activeKid.treasureDifficulty, ref activeKid.treasureLevel, json.tesoroLevel, json.tesoroSublevel);

                //Sand Level Set
                SetTheCorrectLevel(ref activeKid.sandDifficulty, json.arenaMagicaLevel);
                SetTheCorrectLevel(ref activeKid.sandLevel, json.arenaMagicaSublevel);
                SetTheCorrectLevel(ref activeKid.sandLevel2, json.arenaMagicaSublevel2);
                SetTheCorrectLevel(ref activeKid.sandLevel3, json.arenaMagicaSublevel3);

                //Icecream level set
                SetTheCorrectLevel(ref activeKid.icecreamLevel, json.heladosLevel);
            }
        }

        downlodingData = false;
        SaveSession();
    }

    void SetTheCorrectLevel(ref int difficulty, ref int level, int difficltyFromWeb, int levelFromWeb)
    {
        if (difficulty < difficltyFromWeb)
        {
            difficulty = difficltyFromWeb;
            level = levelFromWeb;
        }
        else if (difficulty == difficltyFromWeb)
        {
            if (level < levelFromWeb)
            {
                level = levelFromWeb;
            }
        }
        else
        {
            Debug.Log("We stick to the local data");
        }

    }

    void SetTheCorrectLevel(ref int level, int levelFromWeb)
    {
        if (levelFromWeb > level)
        {
            level = levelFromWeb;
        }
    }

    //
    [System.Serializable]
    public class User
    {
        public string userkey;
        public string username;
        public string psswdHash;
        public List<Kid> kids;
        public int id;
        public int suscriptionsLeft;
        public string language;
        public bool trialAccount;
        public DateTime suscriptionDate;
        public bool isPossibleBuyIAP;
        public User(string key, string user, string psswd, int ide)
        {
            language = "es";
            userkey = key;
            username = user;
            psswdHash = psswd;
            id = ide;
            kids = new List<Kid>();
            trialAccount = true;
            suscriptionDate = DateTime.Now;
            suscriptionDate.AddDays(7);
            suscriptionsLeft = 0;
            isPossibleBuyIAP = false;
        }
    }

    [System.Serializable]
    public class Kid
    {
        public string userkey;
        public int id;
        public int age;
        public string name;
        public int kiwis;
        public int dontSyncArbolMusical;
        public int dontSyncRio;
        public int dontSyncArenaMagica;
        public int dontSyncDondeQuedoLaBolita;
        public int dontSyncSombras;
        public int dontSyncTesoro;

        public DateTime offlineSubscription;

        public string avatar;
        public string avatarClothes;

        public string offlineData;
        public List<int> missionsToPlay;
        public bool[] playedGames;
        public List<bool> firstsGames;
        public string ownedItems;
        public int activeDay;
        public bool ageSet;

        public int birdsDifficulty;
        public int birdsLevel;
        public int riverDifficulty;
        public int riverLevel;
        public int sandDifficulty;
        public int sandLevel;
        public int sandLevel2;
        public int sandLevel3;
        public int monkeyDifficulty;
        public int monkeyLevel;
        public int lavaDifficulty;
        public int lavaLevel;
        public int treasureDifficulty;
        public int treasureLevel;
        public int icecreamDifficulty;
        public int icecreamLevel;

        //These integers are used to keep track of the amout of sessions played by the kid
        public int birdsSessions;
        public int lavaSessions;
        public int monkeySessions;
        public int riverSessions;
        public int sandSessions;
        public int treasureSessions;
        public int icecreamSessions;

        public int rioTutorial;
        public int tesoroTutorial;
        public int arbolMusicalTutorial;

        public bool isTimeLimited;
        public float timeLimit;

        public string xmlArbolMusical;
        public string xmlRio;
        public string xmlArenaMagica;
        public string xmlSombras;
        public string xmlDondeQuedoLaBolita;
        public string xmlTesoro;

        public int extraField1;
        public int extraField2;

        public bool syncProfile = true;

        public bool sandLevelSet;

        public bool needSync;
        public bool testAvailable;
        public bool isActive;
        public bool isInTrial;
        public bool isIAPSubscribed;

        public List<int> buyedIslandObjects;

        public Kid(int id, string name, string key, bool active, bool trial)
        {
            this.id = id;
            this.name = name;
            userkey = key;
            kiwis = 0;
            age = 0;
            dontSyncArbolMusical = 0;
            dontSyncRio = 0;
            dontSyncArenaMagica = 0;
            dontSyncDondeQuedoLaBolita = 0;
            dontSyncSombras = 0;
            dontSyncTesoro = 0;

            if (active)
            {
                offlineSubscription = DateTime.Today.AddDays(7);
            }
            else
            {
                offlineSubscription = DateTime.Today.AddDays(-365);
            }

            avatar = "";
            avatarClothes = "";

            offlineData = "";
            missionsToPlay = new List<int>();
            playedGames = new bool[Keys.Number_Of_Games];
            firstsGames = new List<bool>();
            for (int i = 0; i < Keys.Number_Of_Games; i++)
            {
                firstsGames.Add(true);
            }
            ownedItems = "";
            activeDay = -1;
            ageSet = false;

            birdsDifficulty = 0;
            birdsLevel = 0;
            arbolMusicalTutorial = 1;
            riverDifficulty = 0;
            riverLevel = 0;
            rioTutorial = 0xfffffff;
            sandDifficulty = 0;
            sandLevel = 0;
            sandLevel2 = 0;
            sandLevel3 = 0;
            monkeyDifficulty = 0;
            monkeyLevel = 0;
            lavaDifficulty = 0;
            lavaLevel = 0;
            treasureDifficulty = 0;
            treasureLevel = 0;
            tesoroTutorial = 0;

            isTimeLimited = false;
            timeLimit = 15;

            xmlArbolMusical = "";
            xmlRio = "";
            xmlArenaMagica = "";
            xmlSombras = "";
            xmlDondeQuedoLaBolita = "";
            xmlTesoro = "";

            syncProfile = true;

            needSync = false;
            testAvailable = true;
            isActive = active;
            isInTrial = trial;
            isIAPSubscribed = false;
            buyedIslandObjects = new List<int>();

        }

        public void ResetPlayedGames()
        {
            playedGames = new bool[Keys.Number_Of_Games];
        }
    }

    [System.Serializable]
    public class LevlSyncJson
    {
        public int arenaMagicaSublevel2 = 0;
        public int rioSublevel = 0;
        public int rioLevel = 0;
        public int sombrasLevel = 0;
        public int sombrasToday = 0;
        public int monkeyToday = 0;
        public int rioToday = 0;
        public int sombrasSublevel = 0;
        public int tesoroLevel = 0;
        public int tesoroSublevel = 0;
        public int arenaMagicaSublevel = 0;
        public int arbolMusicalLevel = 0;
        public int code = 0;
        public int arenaToday = 0;
        public int arenaMagicaSublevel3 = 0;
        public int arenaMagicaLevel = 0;
        public int arbolMusicalSublevel = 0;
        public int tesoroToday = 0;
        public int monkeySublevel = 0;
        public int monkeyLevel = 0;
        public int arbolToday = 0;
        public int heladosToday = 0;
        public int heladosLevel = 0;
    }


}
