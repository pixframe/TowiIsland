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
    MenuManager menuManager;

    string idStrings;
    int kidsIAP;

    bool downlodingData;

    Firebase.FirebaseApp firebaseApp;

    void Awake()
    {
        /*Analytics.CustomEvent("register");
        for (int i = 1; i < 7; i++)
        {
            Analytics.CustomEvent($"game{i}");
            Debug.Log($"current game is game{i}");
        }
        Analytics.CustomEvent("subscribe");
        Debug.Log("We run the funnel fast");*/
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
                    Debug.Log("This is ok");
                    //GameObject.FindGameObjectWithTag("Coin").GetComponent<UnityEngine.UI.Text>().text = "firebase is set All right";
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
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
                string userS = PlayerPrefs.GetString("activeUser");
                if (userS != "")
                {
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
                if (users[i].kids[c].userkey == key||(key=="_local"&&users[i].userkey==key))
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
        Debug.Log(kids.Length);
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
                            users[u].kids.Add(new Kid(cid, name, key, kidObj.GetBoolean("active"),kidObj.GetBoolean("trial")));
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
                    /*if (kidObj.GetValue("active").Boolean || kidObj.GetValue("trial").Boolean)
                    {

                    }
                    /*for (int k = 0; k < users[u].kids.Count; k++)
                    {
                        if (users[u].kids[k].id == (int)kidObj.GetValue("cid").Number)
                        {
                            if (!kidObj.GetValue("active").Boolean && !kidObj.GetValue("trial").Boolean)
                            {
                                users[u].kids.RemoveAt(k);
                            }
                            else
                            {
                                Debug.Log("We are adding a kid");
                                users[u].kids[k].name = kidObj.GetValue("name").Str + " " + kidObj.GetValue("lastname").Str;
                                users[u].kids[k].userkey = kidObj.GetValue("key").Str;
                                /*users[u].kids[k].dateLastEvaluation = DateTime.Parse(kidObj.GetValue("pruebaDate").Str);
                                Debug.Log(users[u].kids[k].dateLastEvaluation);
                                temporalKids.Add(users[u].kids[k]);
                                Debug.Log("we add 1 kid");
                            }
                            /*users[u].kids[k].name = kidObj.GetValue("name").Str + " " + kidObj.GetValue("lastname").Str;
                            users[u].kids[k].userkey = kidObj.GetValue("key").Str;
                            temporalKids.Add(users[u].kids[k]);
                            missing = false;
                            break;
                        }
                    }
                    if (missing)
                    {
                        //byte[] uni = Encoding.Unicode.GetBytes(kidObj.GetValue("name").Str+" "+kidObj.GetValue("lastname").Str);
                        //string ascii = Encoding.ASCII.GetString(uni);

                    }*/
                }
            }
            /*if (missingUser)
            {
                users.Add(new User(kidObj.GetValue("key").Str, "", "", 0));
                if (bool.Parse(kidObj.GetValue("active").Str) || bool.Parse(kidObj.GetValue("trial").Str))
                {
                    int cid = (int)kidObj.GetValue("cid").Number;
                    string name = kidObj.GetValue("name").Str + " " + kidObj.GetValue("lastname").Str;
                    string key = kidObj.GetValue("key").Str;
                    //DateTime date = DateTime.Parse(kidObj.GetValue("pruebaDate").Str);
                    users[users.Count - 1].kids.Add(new Kid(cid, name, key));
                    temporalKids.Add(users[users.Count - 1].kids[users[users.Count - 1].kids.Count - 1]);
                }
            }*/
        }
        /*for (int k=0; k< activeUser.kids.Count; k++) {
            bool exists=false;
            for(int i=0;i<kids.Length;i++)
            {
                JSONObject kidObj=kids[i].Obj;
                if(activeUser.kids[k].id==int.Parse(kidObj.GetValue("cid").Str))
                {
                    exists=true;
                    break;
                }
            }
            if(!exists)
            {
                activeUser.kids.RemoveAt(k--);
            }
        }*/
        //activeKid = activeUser.kids [0];
        //PlayerPrefs.SetInt("activeKid",activeKid.id);
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
                        s += ","+activeUser.kids[i].id.ToString();
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
        BinaryFormatter b = new BinaryFormatter();
        //Create an in memory stream
        MemoryStream m = new MemoryStream();
        //Save the scores
        b.Serialize(m, users);
        //Add it to player prefs
        PlayerPrefs.SetString("sessions", Convert.ToBase64String(m.GetBuffer()));
    }

    void LoadSession()
    {
        //Get the data
        string data = PlayerPrefs.GetString("sessions");
        Debug.Log(data);
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
            BinaryFormatter b = new BinaryFormatter();
            //Create a memory stream with the data
            MemoryStream m = new MemoryStream(Convert.FromBase64String(data));
            //Load back the scoress
            users = (List<User>)b.Deserialize(m);
        }
    }

    public void SyncProfiles(string key)
    {
        StartCoroutine(PostSyncProfiles(key));
    }

    IEnumerator PostSyncProfiles(string key)
    {
        string post_url = syncProfileURL;
        downlodingData = true;

        WWWForm form = new WWWForm();
        form.AddField("userKey", key);

        Debug.Log("We are syncing the game");

        using (UnityWebRequest request = UnityWebRequest.Post(syncProfileURL, form))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log(request.downloadHandler.text);
            }
            else
            {
                Debug.Log($"this is the sync response: \n{request.downloadHandler.text}");
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
                    activeUser.kids[index].activeMissions.Clear();

                    for (int o = 0; o < activeMissions.Length; o++)
                    {
                        activeUser.kids[index].activeMissions.Add(activeMissions[o].Str);
                    }

                    activeUser.kids[index].activeDay = (int)kidObj.GetNumber("activeDay");
                    activeUser.kids[index].ageSet = true;
                    activeUser.kids[index].birdsFirst = kidObj.GetBoolean("arbolFirstTime");
                    activeUser.kids[index].lavaFirst = kidObj.GetBoolean("sombrasFirstTime");
                    activeUser.kids[index].monkeyFirst = kidObj.GetBoolean("bolitaFirstTime");
                    activeUser.kids[index].riverFirst = kidObj.GetBoolean("rioFirstTime");
                    activeUser.kids[index].sandFirst = kidObj.GetBoolean("arenaFirstTime");
                    activeUser.kids[index].treasureFirst = kidObj.GetBoolean("tesoroFirstTime");
                    activeUser.kids[index].testAvailable = kidObj.GetBoolean("testAvailable");
                    activeUser.kids[index].sandLevelSet = kidObj.GetBoolean("arenaLevelSet");

                    for (int o = 0; o < buyedItems.Length; o++)
                    {
                        activeUser.kids[index].buyedIslandObjects.Add((int)buyedItems[o].Number);
                    }

                    if (activeUser.kids[index].birdsFirst || activeUser.kids[index].lavaFirst || activeUser.kids[index].monkeyFirst || activeUser.kids[index].riverFirst || activeUser.kids[index].sandFirst || activeUser.kids[index].treasureFirst)
                    {
                        activeUser.kids[index].anyFirstTime = true;
                    }
                    else
                    {
                        activeUser.kids[index].anyFirstTime = false;
                    }
                    string tyep = kidObj.GetString("suscriptionType");
                    if (tyep == "monthly" || tyep == "quarterly")
                    {
                        setType = true;
                    }
                    else if (tyep == "monthly_inApp" || tyep == "quarterly_inApp")
                    {
                        activeUser.kids[index].isIAPSubscribed = true;
                    }
                    if (activeKid != null)
                    {
                        if (activeKid.id == cid)
                        {
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
                    menuManager.UpdateIAPSubscription(idStrings, kidsIAP);
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

        activeKid.activeMissions.Clear();
        for (int i = 0; i < activeMissionsActive.Count; i++)
        {
            array.Add(activeMissionsActive[i]);
            activeKid.activeMissions.Add(activeMissionsActive[i]);
        }

        JSONObject data = UpdateJsaonData(array);

        Debug.Log(data.ToString());

        WWWForm form = new WWWForm();
        form.AddField("jsonToDb", data.ToString());

        using (UnityWebRequest request = UnityWebRequest.Post(updateProfileURL, form))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError)
            {
                if (FindObjectOfType<GameCenterManager>())
                {
                    FindObjectOfType<GameCenterManager>().ChangeMenus();
                }
                activeKid.syncProfile = false;
                Debug.Log(request.downloadHandler.text);
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
        Debug.Log("Updating profile");

        for (int i = 0; i < activeKid.activeMissions.Count; i++)
        {
            array.Add(activeKid.activeMissions[i]);
        }

        JSONObject data = UpdateJsaonData(array);

        Debug.Log(data.ToString());

        WWWForm form = new WWWForm();
        form.AddField("jsonToDb", data.ToString());

        using (UnityWebRequest request = UnityWebRequest.Post(updateProfileURL, form))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError)
            {
                if (FindObjectOfType<GameCenterManager>())
                {
                    FindObjectOfType<GameCenterManager>().ChangeMenus();
                }
                activeKid.syncProfile = false;
                Debug.Log(request.downloadHandler.text);
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

    JSONObject UpdateJsaonData(JSONArray array)
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
            { "rioFirstTime", activeKid.riverFirst },
            { "tesoroFirstTime", activeKid.treasureFirst },
            { "arbolFirstTime", activeKid.birdsFirst },
            { "arenaFirstTime", activeKid.sandFirst },
            { "sombrasFirstTime", activeKid.lavaFirst },
            { "bolitaFirstTime", activeKid.monkeyFirst },
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
        string post_url = syncLevelsURL;
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
                JSONObject response = JSONObject.Parse(request.downloadHandler.text);

                //Birds Level Set
                int birdsDifficulty = (int)response["arbolMusicalLevel"].Number;
                int birdsLevel = (int)response["arbolMusicalSublevel"].Number;
                SetTheCorrectLevel(ref activeKid.birdsDifficulty, ref activeKid.birdsLevel, birdsDifficulty, birdsLevel);

                //River Level Set
                int riverDifficulty = (int)response["rioLevel"].Number;
                int riverLevel = (int)response["rioSublevel"].Number;
                SetTheCorrectLevel(ref activeKid.riverDifficulty, ref activeKid.riverLevel, riverDifficulty, riverLevel);

                //Monkey Level Set
                int monekeyDifficulty = (int)response["monkeyLevel"].Number;
                int monekeyLevel = (int)response["monkeySublevel"].Number;
                SetTheCorrectLevel(ref activeKid.monkeyDifficulty, ref activeKid.monkeyLevel, monekeyDifficulty, monekeyLevel);

                //Lava Level Set
                int lavaDifficulty = (int)response["sombrasLevel"].Number;
                int lavaLevel = (int)response["sombrasSublevel"].Number;
                SetTheCorrectLevel(ref activeKid.lavaDifficulty, ref activeKid.laveLevel, lavaDifficulty, lavaLevel);

                //Treasure Level Set
                int treasureDifficulty = (int)response["tesoroLevel"].Number;
                int treasureLevel = (int)response["tesoroSublevel"].Number;
                SetTheCorrectLevel(ref activeKid.treasureDifficulty, ref activeKid.treasureLevel, treasureDifficulty, treasureLevel);

                //Sand Level Set
                int sandDifficulty = (int)response["arenaMagicaLevel"].Number;
                int sandLevelA = (int)response["arenaMagicaSublevel"].Number;
                int sandLevelB = (int)response["arenaMagicaSublevel2"].Number;
                int sandLevelC = (int)response["arenaMagicaSublevel3"].Number;
                SetTheCorrectLevel(ref activeKid.sandDifficulty, sandDifficulty);
                SetTheCorrectLevel(ref activeKid.sandLevel, sandLevelA);
                SetTheCorrectLevel(ref activeKid.sandLevel2, sandLevelB);
                SetTheCorrectLevel(ref activeKid.sandLevel3, sandLevelC);

            }
        }

        downlodingData = false;
        //    WWW hs_post = new WWW(post_url, form);
        //yield return hs_post;

        //if (hs_post.error == null)
        //{
        //    JSONObject response = JSONObject.Parse(hs_post.text);
        //    if (response["code"].Str == "200")
        //    {

        //        activeKid.birdsDifficulty = (int)response["arbolMusicalLevel"].Number;
        //        activeKid.birdsLevel = (int)response["arbolMusicalSublevel"].Number;
        //        activeKid.riverDifficulty = (int)response["rioLevel"].Number;
        //        activeKid.riverLevel = (int)response["rioSublevel"].Number;
        //        activeKid.sandDifficulty = (int)response["arenaMagicaLevel"].Number;
        //        activeKid.sandLevel = (int)response["arenaMagicaSublevel"].Number;
        //        activeKid.sandLevel2 = (int)response["arenaMagicaSublevel2"].Number;
        //        activeKid.sandLevel3 = (int)response["arenaMagicaSublevel3"].Number;
        //        activeKid.monkeyDifficulty = (int)response["monkeyLevel"].Number;
        //        activeKid.monkeyLevel = (int)response["monkeySublevel"].Number;
        //        activeKid.lavaDifficulty = (int)response["sombrasLevel"].Number;
        //        activeKid.laveLevel = (int)response["sombrasSublevel"].Number;
        //        activeKid.treasureDifficulty = (int)response["tesoroLevel"].Number;
        //        activeKid.treasureLevel = (int)response["tesoroSublevel"].Number;
        //    }  
        //}
        //else
        //{
        //    Debug.Log(hs_post.error);
        //    Debug.Log(hs_post.text);
        //}
        SaveSession();
    }

    void SetTheCorrectLevel(ref int difficulty, ref int level, int difficltyFromWeb, int levelFromWeb)
    {
        if (difficulty < difficltyFromWeb)
        {
            difficulty = difficltyFromWeb;
            level = levelFromWeb;
            Debug.Log("We set a web data");
        }
        else if (difficulty == difficltyFromWeb)
        {
            if (level < levelFromWeb)
            {
                level = levelFromWeb;
            }
            Debug.Log("We set a web data");
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
        public List<string> activeMissions;
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
        public int laveLevel;
        public int treasureDifficulty;
        public int treasureLevel;
        public int icecreamDifficulty;
        public int icecreamLevel;

        public int playedBird;
        public int blockedArbolMusical;
        public int playedRiver;
        public int blockedRio;
        public int playedSand;
        public int blockedArenaMagica;
        public int playedLava;
        public int blockedSombras;
        public int playedMonkey;
        public int blockedDondeQuedoLaBolita;
        public int playedTreasure;
        public int blockedTesoro;

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

        public string xmlArbolMusical;
        public string xmlRio;
        public string xmlArenaMagica;
        public string xmlSombras;
        public string xmlDondeQuedoLaBolita;
        public string xmlTesoro;

        public int extraField1;
        public int extraField2;

        public bool syncProfile = true;

        public bool birdsFirst;
        public bool lavaFirst;
        public bool monkeyFirst;
        public bool riverFirst;
        public bool sandFirst;
        public bool treasureFirst;
        public bool icecreamFirst;
        public bool anyFirstTime;

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
            activeMissions = new List<string>();
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
            laveLevel = 0;
            treasureDifficulty = 0;
            treasureLevel = 0;
            tesoroTutorial = 0;

            playedBird = 0;
            blockedArbolMusical = 0;
            playedRiver = 0;
            blockedRio = 0;
            playedSand = 0;
            blockedArenaMagica = 0;
            playedLava = 0;
            blockedSombras = 0;
            playedMonkey = 0;
            blockedDondeQuedoLaBolita = 0;
            playedTreasure = 0;
            blockedTesoro = 0;

            xmlArbolMusical = "";
            xmlRio = "";
            xmlArenaMagica = "";
            xmlSombras = "";
            xmlDondeQuedoLaBolita = "";
            xmlTesoro = "";

            syncProfile = true;

            birdsFirst = true;
            lavaFirst = true;
            monkeyFirst = true;
            riverFirst = true;
            sandFirst = true;
            treasureFirst = true;
            anyFirstTime = true;
            needSync = false;
            testAvailable = true;
            isActive = active;
            isInTrial = trial;
            isIAPSubscribed = false;
            buyedIslandObjects = new List<int>();

        }
    }
}
