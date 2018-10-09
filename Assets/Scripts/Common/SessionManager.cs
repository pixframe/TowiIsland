using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Boomlagoon.JSON;
using System.Text;
using System.Xml.Serialization;


public class SessionManager : MonoBehaviour
{

    List<User> users;
    public User activeUser;
    public Kid activeKid;
    public bool main = false;
    string syncProfileURL = "http://187.248.54.146:280/api/sync_profiles/";
    string syncLevelsURL = "http://187.248.54.146:280/api/child_levels/";
    string updateProfileURL = "http://187.248.54.146:280/api/update_profile/";
    public List<Kid> temporalKids;
    public static int numberOfSessionManager = 0;
    MenuManager menuManager;

    void Awake()
    {
        numberOfSessionManager++;
        if (numberOfSessionManager > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
        //if(main)
        //	PlayerPrefs.DeleteAll ();
        //PlayerPrefs.DeleteAll ();
        string version = Application.version;
        Debug.Log("version of app is " + version);
        if (PlayerPrefs.GetString(Keys.Version_Last_Season) != version)
        {
            Debug.Log("New Version");
            PlayerPrefs.SetString("sessions", "");
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
                activeUser.kids.Add(new Kid(0, "local_kid", "_local"));
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
                string userS = PlayerPrefs.GetString("activeUser", "");
                Debug.Log(userS);
                if (userS != "")
                {
                    activeUser = GetUser(userS);
                    int kidS = PlayerPrefs.GetInt("activeKid", -1);
                    Debug.Log(kidS);
                    if (kidS != -1)
                    {
                        activeKid = GetKid(kidS);
                    }
                }
            }
        }
    }

    public int GetNumberOfUsers()
    {
        return users.Count;
    }

    public User GetUser(string key)
    {
        for (int i = 0; i < users.Count; i++)
        {
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

    Kid GetKid(int id)
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

    public bool LoadActiveUser()
    {
        string userS = PlayerPrefs.GetString("activeUser", "");
        if (userS != "")
        {
            activeUser = GetUser(userS);
            return true;
        }
        return false;
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
                    //PlayerPrefs.SetString("activeUser", activeKid.userkey);
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
                PlayerPrefs.SetString("activeUser", activeKid.userkey);
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

    public void AddKid(int kidID, string name, string key)
    {
        //TODO Add Age
        activeUser.kids.Add(new Kid(kidID, name, key));
        SaveSession();
    }

    public void AddKids(JSONArray kids)
    {
        temporalKids.Clear();
        for (int i = 0; i < kids.Length; i++)
        {
            JSONObject kidObj = kids[i].Obj;
            bool missingUser = true;
            bool missing = true;
            for (int u = 0; u < users.Count; u++)
            {
                if (users[u].userkey == kidObj.GetValue("key").Str)
                {
                    missingUser = false;
                    for (int k = 0; k < users[u].kids.Count; k++)
                    {
                        if (users[u].kids[k].id == Convert.ToInt32(kidObj.GetValue("cid").Number))
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
                                Debug.Log(users[u].kids[k].dateLastEvaluation);*/
                                temporalKids.Add(users[u].kids[k]);
                                Debug.Log("we add 1 kid");
                            }
                            /*users[u].kids[k].name = kidObj.GetValue("name").Str + " " + kidObj.GetValue("lastname").Str;
                            users[u].kids[k].userkey = kidObj.GetValue("key").Str;
                            temporalKids.Add(users[u].kids[k]);
                            missing = false;*/
                            break;
                        }
                    }
                    if (missing)
                    {
                        //byte[] uni = Encoding.Unicode.GetBytes(kidObj.GetValue("name").Str+" "+kidObj.GetValue("lastname").Str);
                        //string ascii = Encoding.ASCII.GetString(uni);
                        if (kidObj.GetValue("active").Boolean || kidObj.GetValue("trial").Boolean)
                        {
                            int cid = (int)kidObj.GetValue("cid").Number;
                            string name = kidObj.GetValue("name").Str + " " + kidObj.GetValue("lastname").Str;
                            string key = kidObj.GetValue("key").Str;
                            //DateTime date = DateTime.Parse(kidObj.GetValue("pruebaDate").Str);
                            //Debug.Log(date);
                            users[u].kids.Add(new Kid(cid, name, key));
                            temporalKids.Add(users[u].kids[users[u].kids.Count - 1]);
                        }
                    }
                }
            }
            if (missingUser)
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
            }
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
        PlayerPrefs.SetString("activeUser", activeKid.userkey);
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

        WWWForm form = new WWWForm();
        form.AddField("userKey", key);

        WWW hs_post = new WWW(post_url, form);
        yield return hs_post;
        if (hs_post.error == null)
        {
            JSONArray kids = JSONArray.Parse(hs_post.text);
            Debug.Log(hs_post.text);
            activeUser.kids.Clear();
            for (int i = 0; i < kids.Length; i++)
            {
                JSONObject kidObj = kids[i].Obj;
                JSONArray activeMissions = kidObj.GetArray("activeMissions");

                if (kidObj.GetBoolean("active") || kidObj.GetBoolean("trial"))
                {
                    int cid = (int)kidObj.GetNumber("cid");
                    string kidName = kidObj.GetString("name");

                    AddKid(cid, kidName, activeUser.userkey);

                    int index = activeUser.kids.Count - 1;

                    activeUser.kids[index].kiwis = (int)kidObj.GetNumber("kiwis");
                    activeUser.kids[index].avatar = kidObj.GetString("avatar");
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
                    if (activeUser.kids[index].birdsFirst || activeUser.kids[index].lavaFirst || activeUser.kids[index].monkeyFirst || activeUser.kids[index].riverFirst || activeUser.kids[index].sandFirst || activeUser.kids[index].treasureFirst)
                    {
                        activeUser.kids[index].anyFirstTime = true;
                    }
                    else
                    {
                        activeUser.kids[index].anyFirstTime = false;
                    }
                    if (activeKid != null)
                    {
                        if (activeKid.id == cid)
                        {
                            SyncChildLevels();
                        }
                    }
                }
                else
                {
                    if (activeKid != null)
                    {
                        if (activeKid.id == (int)kidObj.GetNumber("cid"))
                        {
                            if (kids.Length > 1)
                            {
                                bool anotherActiveKid = false;
                                for (int j = 0; j < kids.Length; j++)
                                {
                                    JSONObject kidObjt = kids[j].Obj;
                                    if (kidObjt.GetBoolean("active") || kidObjt.GetBoolean("trial"))
                                    {
                                        Debug.Log("Change kid");
                                        anotherActiveKid = true;
                                        break;
                                    }
                                }
                                if (anotherActiveKid)
                                {

                                }
                                else
                                {
                                    FindObjectOfType<MenuManager>().ShowTrialIsOff();
                                }
                            }
                            else
                            {
                                FindObjectOfType<MenuManager>().ShowTrialIsOff();
                            }
                        }
                    }
                    else
                    {
                        FindObjectOfType<MenuManager>().SetKidsProfiles();
                    }
                }
                //Debug.Log(activeMissions);
                /*for (int u = 0; u < users.Count; u++)
                {
                    if (users[u].userkey == key)
                    {
                        users[u].kids.Clear();
                        if (kidObj.GetBoolean("active") || kidObj.GetBoolean("trial"))
                        {
                            int cid = (int)kidObj.GetNumber("cid");
                            string kidName = kidObj.GetString("name");

                            AddKid(cid, kidName, users[u].userkey);

                            int index = users[u].kids.Count - 1;
                        }

                        for (int k = 0; k < users[u].kids.Count; k++)
                        {
                            if (users[u].kids[k].id == (int)kidObj.GetValue("cid").Number && users[u].kids[k].syncProfile)
                            {
                                users[u].kids[k].kiwis = (int)kidObj.GetValue("kiwis").Number;
                                users[u].kids[k].avatar = kidObj.GetValue("avatar").Str;
                                users[u].kids[k].avatarClothes = kidObj.GetValue("avatarClothes").Str;
                                users[u].kids[k].ownedItems = kidObj.GetValue("ownedItems").Str;
                                users[u].kids[k].age = (int)kidObj.GetValue("age").Number;
                                users[u].kids[k].activeMissions.Clear();
                                for (int o = 0; o < activeMissions.Length; o++)
                                {
                                    users[u].kids[k].activeMissions.Add(activeMissions[o].Str);
                                }
                                users[u].kids[k].activeDay = (int)kidObj.GetValue("activeDay").Number;
                                users[u].kids[k].ageSet = true;
                                users[u].kids[k].birdsFirst = kidObj.GetValue("arbolFirstTime").Boolean;
                                users[u].kids[k].lavaFirst = kidObj.GetValue("sombrasFirstTime").Boolean;
                                users[u].kids[k].monkeyFirst = kidObj.GetValue("bolitaFirstTime").Boolean;
                                users[u].kids[k].riverFirst = kidObj.GetValue("rioFirstTime").Boolean;
                                users[u].kids[k].sandFirst = kidObj.GetValue("arenaFirstTime").Boolean;
                                users[u].kids[k].treasureFirst = kidObj.GetValue("tesoroFirstTime").Boolean;
                                users[u].kids[k].testAvailable = kidObj.GetValue("testAvailable").Boolean;
                                if (users[u].kids[k].birdsFirst || users[u].kids[k].lavaFirst || users[u].kids[k].monkeyFirst || users[u].kids[k].riverFirst || users[u].kids[k].sandFirst || users[u].kids[k].treasureFirst)
                                {
                                    users[u].kids[k].anyFirstTime = true;
                                }
                                else
                                {
                                    users[u].kids[k].anyFirstTime = false;
                                }
                                break;
                            }
                        }
                    }
                }*/
                SaveSession();
            }
            /*if (jsonObject.GetValue("code").Str == "200")
            {
                JSONArray kids = jsonObject.GetValue("profiles").Array;

            }*/
        }
        else
        {
            Debug.Log(hs_post.error);
            Debug.Log(hs_post.text);
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
        string post_url = updateProfileURL;
        JSONObject data = new JSONObject();
        JSONArray array = new JSONArray();

        for (int i = 0; i < activeMissionsActive.Count; i++)
        {
            array.Add(activeMissionsActive[i]);
        }

        data.Add("cid", activeKid.id);
        data.Add("kiwis", activeKid.kiwis);
        data.Add("avatar", activeKid.avatar);
        data.Add("avatarclothes", activeKid.avatarClothes);
        data.Add("activeDay", activeKid.activeDay);
        data.Add("owneditems", activeKid.ownedItems);
        data.Add("activemissions", array);
        data.Add("activeday", activeKid.activeDay);
        data.Add("rioFirstTime", activeKid.riverFirst);
        data.Add("tesoroFirstTime", activeKid.treasureFirst);
        data.Add("arbolFirstTime", activeKid.birdsFirst);
        data.Add("arenaFirstTime", activeKid.sandFirst);
        data.Add("sombrasFirstTime", activeKid.lavaFirst);
        data.Add("bolitaFirstTime", activeKid.monkeyFirst);
        data.Add("userKey", activeKid.userkey);
        Debug.Log(data.ToString());
        WWWForm form = new WWWForm();
        form.AddField("jsonToDb", data.ToString());

        WWW hs_post = new WWW(post_url, form);
        yield return hs_post;

        Debug.Log(hs_post.text);

        if (hs_post.error == null)
        {
            activeKid.needSync = false;
        }
        else
        {
            Debug.Log(hs_post.error);
            activeKid.syncProfile = false;
        }
        SaveSession();
    }

    IEnumerator PostUpdateProfile()
    {
        string post_url = updateProfileURL;
        JSONObject data = new JSONObject();
        JSONArray array = new JSONArray();

        for (int i = 0; i < activeKid.activeMissions.Count; i++)
        {
            array.Add(activeKid.activeMissions[i]);
        }

        data.Add("cid", activeKid.id);
        data.Add("kiwis", activeKid.kiwis);
        data.Add("avatar", activeKid.avatar);
        data.Add("avatarclothes", activeKid.avatarClothes);
        data.Add("activeDay", activeKid.activeDay);
        data.Add("owneditems", activeKid.ownedItems);
        data.Add("activemissions", array);
        data.Add("activeday", activeKid.activeDay);
        data.Add("rioFirstTime", activeKid.riverFirst);
        data.Add("tesoroFirstTime", activeKid.treasureFirst);
        data.Add("arbolFirstTime", activeKid.birdsFirst);
        data.Add("arenaFirstTime", activeKid.sandFirst);
        data.Add("sombrasFirstTime", activeKid.lavaFirst);
        data.Add("bolitaFirstTime", activeKid.monkeyFirst);
        data.Add("userKey", activeKid.userkey);
        Debug.Log(data.ToString());
        WWWForm form = new WWWForm();
        form.AddField("jsonToDb", data.ToString());

        WWW hs_post = new WWW(post_url, form);
        yield return hs_post;

        Debug.Log(hs_post.text);

        if (hs_post.error == null)
        {
            activeKid.needSync = false;
            JSONObject jsonObj = JSONObject.Parse(hs_post.text);
        }
        else
        {
            Debug.Log(hs_post.error);
            activeKid.syncProfile = false;
        }
        SaveSession();
    }

    public void SyncChildLevels()
    {
        StartCoroutine(PostChildSyncLevels());
    }

    IEnumerator PostChildSyncLevels()
    {
        string post_url = syncLevelsURL;
        DateTime today = DateTime.Now;

        WWWForm form = new WWWForm();
        form.AddField("userKey", activeKid.userkey);
        form.AddField("cid", activeKid.id);
        form.AddField("date", String.Format("{0:0000}-{1:00}-{2:00}", today.Year, today.Month, today.Day));
        //WWW hs_post = new WWW(post_url);
        WWW hs_post = new WWW(post_url, form);
        yield return hs_post;

        if (hs_post.error == null)
        {
            JSONObject response = JSONObject.Parse(hs_post.text);
            Debug.Log(response["code"].Str);
            if (response["code"].Str == "200")
            {

                activeKid.birdsDifficulty = (int)response["arbolMusicalLevel"].Number;
                activeKid.birdsLevel = (int)response["arbolMusicalSublevel"].Number;
                activeKid.riverDifficulty = (int)response["rioLevel"].Number;
                activeKid.riverLevel = (int)response["rioSublevel"].Number;
                activeKid.sandDifficulty = (int)response["arenaMagicaLevel"].Number;
                activeKid.sandLevel = (int)response["arenaMagicaSublevel"].Number;
                activeKid.sandLevel2 = (int)response["arenaMagicaSublevel2"].Number;
                activeKid.sandLevel3 = (int)response["arenaMagicaSublevel3"].Number;
                activeKid.monkeyDifficulty = (int)response["monkeyLevel"].Number;
                activeKid.monkeyLevel = (int)response["monkeySublevel"].Number;
                activeKid.lavaDifficulty = (int)response["sombrasLevel"].Number;
                activeKid.laveLevel = (int)response["sombrasSublevel"].Number;
                activeKid.treasureDifficulty = (int)response["tesoroLevel"].Number;
                activeKid.treasureLevel = (int)response["tesoroSublevel"].Number;



                /*if (activeKid.dontSyncArbolMusical == 0)
                {
                    activeKid.arbolMusicalLevel = (int)response["arbolMusicalLevel"].Number;
                    activeKid.arbolMusicalSublevel = (int)response["arbolMusicalSublevel"].Number;
                }
                if (activeKid.dontSyncRio == 0)
                {
                    activeKid.rioLevel = int.Parse(response["rioLevel"].Str);
                    activeKid.rioSublevel = int.Parse(response["rioSublevel"].Str);
                }
                if (activeKid.dontSyncArenaMagica == 0)
                {
                    activeKid.arenaMagicaLevel = int.Parse(response["arenaMagicaLevel"].Str);
                    activeKid.arenaMagicaSublevel = int.Parse(response["arenaMagicaSublevel"].Str);
                }
                if (activeKid.dontSyncDondeQuedoLaBolita == 0)
                {
                    activeKid.monkeyLevel = int.Parse(response["monkeyLevel"].Str);
                    activeKid.monkeySublevel = int.Parse(response["monkeySublevel"].Str);
                }
                if (activeKid.dontSyncSombras == 0)
                {
                    activeKid.sombrasLevel = int.Parse(response["sombrasLevel"].Str);
                    activeKid.sombrasSublevel = int.Parse(response["sombrasSublevel"].Str);
                }
                if (activeKid.dontSyncTesoro == 0)
                {
                    activeKid.tesoroLevel = int.Parse(response["tesoroLevel"].Str);
                    activeKid.tesoroSublevel = int.Parse(response["tesoroSublevel"].Str);
                }*/

                /*activeKid.playedArbolMusical = int.Parse(response["arbolToday"].Str);
                activeKid.playedRio = int.Parse(response["rioToday"].Str);
                activeKid.playedArenaMagica = int.Parse(response["arenaToday"].Str);
                activeKid.playedDondeQuedoLaBolita = int.Parse(response["monkeyToday"].Str);
                activeKid.playedSombras = int.Parse(response["sombrasToday"].Str);
                activeKid.playedTesoro = int.Parse(response["tesoroToday"].Str);*/

                /*if (sessionMng.activeKid.playedArbolMusical >= TopArbolMusical)
                    sessionMng.activeKid.blockedArbolMusical = 1;
                if (sessionMng.activeKid.playedRio >= TopRio)
                    sessionMng.activeKid.blockedRio = 1;
                if (sessionMng.activeKid.playedArenaMagica >= TopArenaMagica)
                    sessionMng.activeKid.blockedArenaMagica = 1;
                if (sessionMng.activeKid.playedDondeQuedoLaBolita >= TopMonkey)
                    sessionMng.activeKid.blockedDondeQuedoLaBolita = 1;
                if (sessionMng.activeKid.playedSombras >= TopSombras)
                    sessionMng.activeKid.blockedSombras = 1;
                if (sessionMng.activeKid.playedTesoro >= TopTesoro)
                    sessionMng.activeKid.blockedTesoro = 1;

                sessionMng.SaveSession();

                main.UpdateBlocked();*/
            }
            Debug.Log(hs_post.text);
        }
        else
        {
            Debug.Log(hs_post.error);
            Debug.Log(hs_post.text);
        }
        SaveSession();
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
        public string language;
        public bool trialAccount;
        public DateTime suscriptionDate;
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
        public bool anyFirstTime;
        public bool needSync;
        public bool testAvailable;

        //TODO Add age
        public Kid(int id, string name, string key)
        {
            this.id = id;
            this.name = name;
            userkey = key;
            kiwis = 0;
            dontSyncArbolMusical = 0;
            dontSyncRio = 0;
            dontSyncArenaMagica = 0;
            dontSyncDondeQuedoLaBolita = 0;
            dontSyncSombras = 0;
            dontSyncTesoro = 0;

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

            birdsFirst = false;
            lavaFirst = false;
            monkeyFirst = false;
            riverFirst = false;
            sandFirst = false;
            treasureFirst = false;
            anyFirstTime = false;
            needSync = false;
            testAvailable = true;
        }
    }
}
