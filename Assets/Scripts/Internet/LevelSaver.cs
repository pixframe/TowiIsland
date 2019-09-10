using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Boomlagoon.JSON;
using System.Collections.Generic;
using System;
using System.IO;
using System.Globalization;

public class LevelSaver : MonoBehaviour {

    string secretKey = "$k1w1GAMES$";
    string postURL = Keys.Api_Web_Key + "api/v2/levels/create/";
    JSONObject data;
    JSONObject item;
    JSONObject levelsData;
    Header headers;
    GameData gameData;

    bool saving = false;

    string game;
    string key;
    int kidKey;

    SessionManager sessionManager;

    CultureInfo invariantCulture = CultureInfo.InvariantCulture;

    // Use this for initialization
    void Start ()
    {
        data = new JSONObject();
        levelsData = new JSONObject();

        if (FindObjectOfType<SessionManager>())
        {
            sessionManager = FindObjectOfType<SessionManager>();
            key = sessionManager.activeKid.userkey;
            kidKey = sessionManager.activeKid.id;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void AddLevelData(string key, int value)
    {
        if (item == null)
        {
            item = new JSONObject();
        }

        item.Add(key, value);
    }
    public void AddLevelData(string key, float value)
    {
        if (item == null)
        {
            item = new JSONObject();
        }

        int valueInteger = Mathf.RoundToInt(value * 1000);
        float fToSave = ((valueInteger / 1000f));
        item.Add(key, fToSave.ToString(invariantCulture));
    }
    public void AddLevelData(string key, string value)
    {
        if (item == null)
        {
            item = new JSONObject();
        }

        item.Add(key, value);
    }
    public void AddLevelData(string key, bool value)
    {
        if (item == null)
        {
            item = new JSONObject();
        }

        item.Add(key, value);
    }
    public void AddLevelData(string key, string[] value)
    {
        if (item == null)
        {
            item = new JSONObject();
        }

        JSONArray tempJsonArray = new JSONArray();
        for (int i = 0; i < value.Length; i++)
        {
            tempJsonArray.Add(value[i]);
        }
        item.Add(key, tempJsonArray);
    }
    public void AddLevelData(string key, int[] value)
    {
        if (item == null)
        {
            item = new JSONObject();
        }

        JSONArray tempJsonArray = new JSONArray();

        for (int i = 0; i < value.Length; i++)
        {
            tempJsonArray.Add(value[i]);
        }

        item.Add(key, tempJsonArray);
    }
    public void AddLevelData(string key, float[] value)
    {
        if (item == null)
        {
            item = new JSONObject();
        }

        JSONArray tempJsonArray = new JSONArray();
        for (int i = 0; i < value.Length; i++)
        {
            tempJsonArray.Add(value[i].ToString(invariantCulture));
        }
        item.Add(key, tempJsonArray);
    }
    public void AddLevelData(string key, Texture2D photo)
    {
        if (item == null)
        {
            item = new JSONObject();
        }

        JSONArray tempJsonArray = new JSONArray();

        byte[] bytes = photo.EncodeToPNG();
        tempJsonArray.Add(BitConverter.ToString(bytes));

        item.Add(key, tempJsonArray);
    }
    public void AddLevelData(string key, List<string> value)
    {
        if (item == null)
        {
            item = new JSONObject();
        }

        JSONArray tempJsonArray = new JSONArray();

        for (int i = 0; i < value.Count; i++)
        {
            tempJsonArray.Add(value[i]);
        }

        item.Add(key, tempJsonArray);
    }
    public void AddLevelData(string key, List<int> value)
    {
        if (item == null)
        {
            item = new JSONObject();
        }

        JSONArray tempJsonArray = new JSONArray();

        for (int i = 0; i < value.Count; i++)
        {
            tempJsonArray.Add(value[i]);
        }

        item.Add(key, tempJsonArray);
    }
    public void AddLevelData(string key, List<float> value)
    {
        if (item == null)
        {
            item = new JSONObject();
        }
        JSONArray tempJsonArray = new JSONArray();
        for (int i = 0; i < value.Count; i++)
        {
            tempJsonArray.Add(value[i].ToString(invariantCulture));
        }
        item.Add(key, tempJsonArray);
    }
    public void AddLevelData(string key, List<List<int>> value)
    {
        if (item == null)
        {
            item = new JSONObject();
        }

        JSONArray tempJsonArray = new JSONArray();

        for (int i = 0; i < value.Count; i++)
        {
            JSONArray miniArray = new JSONArray();

            for (int j = 0; j < value[i].Count; j++)
            {
                miniArray.Add(value[i][j]);
            }

            tempJsonArray.Add(miniArray);
        }

        item.Add(key, tempJsonArray);
    }
    public void AddLevelData(string key, int[][] value)
    {
        if (item == null)
        {
            item = new JSONObject();
        }

        JSONArray tempJsonArray = new JSONArray();

        for (int i = 0; i < value.Length; i++)
        {
            JSONArray miniArray = new JSONArray();

            for (int j = 0; j < value[i].Length; j++)
            {
                miniArray.Add(value[i][j]);
            }

            tempJsonArray.Add(miniArray);
        }
        item.Add(key, tempJsonArray);
    }
    public void AddLevelData(string key, List<List<string>> value)
    {
        if (item == null)
        {
            item = new JSONObject();
        }
        JSONArray tempJsonArray = new JSONArray();
        for (int i = 0; i < value.Count; i++)
        {
            JSONArray miniArray = new JSONArray();
            for (int j = 0; j < value[i].Count; j++)
            {
                miniArray.Add(value[i][j]);
            }
            tempJsonArray.Add(miniArray);
        }
        item.Add(key, tempJsonArray);
    }
    public void AddLevelDataAsString(string key, int[] listToConvert)
    {
        if (item == null)
        {
            item = new JSONObject();
        }
        string stringToAdd = "";
        for (int i = 0; i < listToConvert.Length; i++)
        {
            if (i > 0)
            {
                stringToAdd += ",";
            }
            stringToAdd += listToConvert[i].ToString();
        }
        item.Add(key, stringToAdd);
    }
    public void AddLevelDataAsString(string key, float[] listToConvert)
    {
        if (item == null)
        {
            item = new JSONObject();
        }
        string stringToAdd = "";
        for (int i = 0; i < listToConvert.Length; i++)
        {
            if (i > 0)
            {
                stringToAdd += ",";
            }
            stringToAdd += listToConvert[i].ToString(invariantCulture);
        }
        item.Add(key, stringToAdd);
    }
    public void AddLevelDataAsString(string key, string[] listToConvert)
    {
        if (item == null)
        {
            item = new JSONObject();
        }
        string stringToAdd = "";
        for (int i = 0; i < listToConvert.Length; i++)
        {
            if (i > 0)
            {
                stringToAdd += ",";
            }
            stringToAdd += listToConvert[i].ToString();
        }
        item.Add(key, stringToAdd);
    }
    public void AddLevelDataAsString(string key, List<int> listToConvert)
    {
        if (item == null)
        {
            item = new JSONObject();
        }
        string stringToAdd = "";
        for (int i = 0; i < listToConvert.Count; i++)
        {
            if (i > 0)
            {
                stringToAdd += ",";
            }
            stringToAdd += listToConvert[i].ToString();
        }
        item.Add(key, stringToAdd);
    }
    public void AddLevelDataAsString(string key, List<float> listToConvert)
    {
        if (item == null)
        {
            item = new JSONObject();
        }
        string stringToAdd = "";
        for (int i = 0; i < listToConvert.Count; i++)
        {
            if (i > 0)
            {
                stringToAdd += ",";
            }
            stringToAdd += listToConvert[i].ToString(invariantCulture);
        }
        item.Add(key, stringToAdd);
    }

    public void SetLevel()
    {
        //SaveFastData();
        item = new JSONObject();
    }

    public void AddLevelsToBlock()
    {
        if (data.ContainsKey("levels"))
        {
            data["levels"] = item;
        }
        else
        {
            data.Add("levels", item);
        }
        levelsData = new JSONObject();
        //EmergencySave();
    }

    public void SetGameData()
    {
        SetIcecreamData(new List<int> { 1, 1 }, new List<int>{ 1,1}, new List<int> { 1, 1 }, 
            new List<int> { 1, 1 }, new List<int> { 1, 1 }, new List<int> { 1, 1 }, new List<int> { 1, 1 },
            new List<float> { 1.1f, 1.1f },20.22222f,2.222222f,2.22222f,20.22222f, 2, 1);
    }

    public void SetIcecreamData(List<int> ordersAsk, List<int> correctOrders, List<int> expiredOrders,
        List<int> deliveredOrders, List<int> madeOrders, List<int> wrongOrders, List<int> trashOrders,
        List<float> latencies, float correctPercentage, float errorPercentage, float expiredPercentage,
        float time, int playedLevels, int initialLevel)
    {
        gameData = new IcecreamData
        {
            total_orders = GetListAsArray(ordersAsk),
            total_corrects = GetListAsArray(correctOrders),
            total_expired = GetListAsArray(expiredOrders),
            total_delivers = GetListAsArray(deliveredOrders),
            total_done = GetListAsArray(madeOrders),
            total_errors = GetListAsArray(wrongOrders),
            total_trash = GetListAsArray(trashOrders),
            latencies = GetListAsArray(latencies),
            session_correct_percentage = correctPercentage,
            session_errors_percentage = errorPercentage,
            session_expired_percentage = expiredPercentage,
            session_time = time,
            played_levels = playedLevels,
            initial_level = initialLevel
        };

        StartCoroutine(FormToReturn());
    }

    string GetListAsArray(List<int> listToTransform)
    {
        string listString = "";
        for (int i = 0; i < listToTransform.Count; i++)
        {
            listString += $"{listToTransform[i]}{(listToTransform.Count - 1 > i ? "," : "")}";
        }
        return listString;
    }
    string GetListAsArray(List<float> listToTransform)
    {
        string listString = "";
        for (int i = 0; i < listToTransform.Count; i++)
        {
            listString += $"{listToTransform[i].ToString(invariantCulture)}{(listToTransform.Count - 1 > i ? "," : "")}";
        }
        return listString;
    }

    public IEnumerator FormToReturn()
    {
        var forms = new JsonIcecream()
        {
            header = headers,
            levels = gameData as IcecreamData
        };

        Debug.Log(JsonUtility.ToJson(forms));

        WWWForm form = new WWWForm();
        //form.AddField("data", data.ToString());
        form.AddField("jsonToDb", JsonUtility.ToJson(forms));
        using (UnityWebRequest request = UnityWebRequest.Post(postURL, form))
        {

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.downloadHandler.text);
            }
            else
            {
                Debug.Log("Done");
            }
        }
    }

    public void CreateSaveBlock()
    {
        DateTime nowT = DateTime.Now;

        headers = new Header
        {
            device = SystemInfo.deviceType.ToString(),
            version = Application.version,
            game_key = "Helados",
            parent_id = 28,
            kid_id = 42,
            passed_levels = 0,
            repeated_levels = 0,
            played_levels = 0,
            session_time = 1.1f,
            game_time = (int)Mathf.Round(1 * 100) / 100,
            session_number = 1,
            date = String.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}", nowT.Year, nowT.Month, nowT.Day, nowT.Hour, nowT.Minute, nowT.Second)
        };
        SetGameData();
    }

    public void CreateSaveBlock(string gameKey, float gameTime, int passedLevels, int repeatedLevels, int playedLevels, int sessionNumber)
    {
        if (sessionManager == null) {
            Debug.Log("We find no session manager");
            sessionManager = FindObjectOfType<SessionManager>();
        }
        game = gameKey;
        DateTime nowT = DateTime.Now;

        headers = new Header()
        {
            device = SystemInfo.deviceType.ToString(),
            version = Application.version,
            game_key = gameKey,
            parent_id = sessionManager.activeUser.id,
            kid_id = kidKey,
            passed_levels = passedLevels,
            repeated_levels = repeatedLevels,
            played_levels = playedLevels,
            session_time = Mathf.Round(gameTime * 100) / 100,
            game_time = (int)Mathf.Round(gameTime * 100) / 100,
            session_number = sessionNumber,
            date = String.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}", nowT.Year, nowT.Month, nowT.Day, nowT.Hour, nowT.Minute, nowT.Second)
        };

        Debug.Log(JsonUtility.ToJson(headers));

        JSONObject headerItem = new JSONObject
        {

            //{ "parentid", sessionManager.activeUser.id },
            //{ "cid", kidKey },
            //{ "gameKey", gameKey },
            //{ "gameTime", Mathf.Round(gameTime * 100) / 100 },
            //{ "passedLevels", passedLevels },
            //{ "repeatedLevels", repeatedLevels },
            //{ "playedLevels", playedLevels },
            { "device", SystemInfo.deviceType.ToString()},
            { "version", Application.version },
            //version 2
            { "game_key", gameKey },
            { "parent_id", sessionManager.activeUser.id },
            { "kid_id", kidKey },
            { "passed_levels", passedLevels},
            { "repeated_levels", repeatedLevels},
            { "played_levels", playedLevels},
            { "session_time",Mathf.Round(gameTime * 100) / 100 },
            { "game_time",(int)Mathf.Round(gameTime * 100) / 100 },
            { "session_number",  sessionNumber}
        };

        headerItem.Add("date", String.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}", nowT.Year, nowT.Month, nowT.Day, nowT.Hour, nowT.Minute, nowT.Second));
        data.Add("header", headerItem);
    }

    public string ArrayToString(dynamic[] arrayToTransform)
    {
        string stringToReturn = "";
        for (int i = 0; i < arrayToTransform.Length; i++)
        {
            if (i != 0)
            {
                stringToReturn += $",{arrayToTransform[i].ToString()}";
            }
            else
            {
                stringToReturn += arrayToTransform[i].ToString();
            }
        }
        return stringToReturn;
    }

    public void PostProgress()
    {
        saving = true;
        StartCoroutine(CheckInternetConecction("www.towi.com.mx"));
    }

    public void SaveDataOffline()
    {
        if (!FindObjectOfType<DemoKey>()) 
        {
            string dataToSave = data.ToString();
            int gameSavedOffline = PlayerPrefs.GetInt(Keys.Games_Saved);

            string path = $"{Application.persistentDataPath}/{gameSavedOffline}{Keys.Game_To_Save}";
            gameSavedOffline++;

            Debug.Log($"We have {gameSavedOffline} jsons to save");

            File.WriteAllText(path, dataToSave);

            PlayerPrefs.SetInt(Keys.Games_Saved, gameSavedOffline);

            sessionManager.SaveSession();
        }
    }

    IEnumerator CheckInternetConecction(string resource)
    {
        WWWForm form = new WWWForm();
        using (UnityWebRequest newRequest = UnityWebRequest.Get(resource))
        {
            yield return newRequest.SendWebRequest();

            if (newRequest.isNetworkError)
            {
                SaveDataOffline();
            }
            else
            {
                StartCoroutine(PostScores());
            }

            FindObjectOfType<PauseTheGame>().DataIsSend();
        }
    }

    // remember to use StartCoroutine when calling this function!
    IEnumerator PostScores()
    {
        //This connects to a server side php script that will add the name and score to a MySQL DB.
        // Supply it with a string representing the players name and the players score.
        //string hash = Md5Sum(data.ToString() + secretKey);
        string post_url = postURL/* + "name=" + WWW.EscapeURL(name) + "&score=" + score + "&hash=" + hash*/;

        // Build form to post in server
        WWWForm form = new WWWForm();
        //form.AddField("data", data.ToString());
        form.AddField("jsonToDb", data.ToString());
        Debug.Log(data.ToString());

        using (UnityWebRequest request = UnityWebRequest.Post(postURL, form))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log($"the error has the next messsage {request.downloadHandler.text}");
                SaveDataOffline();
            }
            else
            {
                JSONObject response = JSONObject.Parse(request.downloadHandler.text);
                Debug.Log(response["code"].Str);
                if (response["code"].Str != "200" && response["code"].Str != "201")
                {
                    //SavePending();
                    Debug.Log($"the error has the next messsage {request.downloadHandler.text}");
                }
                else
                {
                    switch (game)
                    {
                        case "ArbolMusical":
                            sessionManager.activeKid.dontSyncArbolMusical = 0;
                            break;
                        case "Rio":
                            sessionManager.activeKid.dontSyncRio = 0;
                            break;
                        case "ArenaMagica":
                            sessionManager.activeKid.dontSyncArenaMagica = 0;
                            break;
                        case "DondeQuedoLaBolita":
                            sessionManager.activeKid.dontSyncDondeQuedoLaBolita = 0;
                            break;
                        case "JuegoDeSombras":
                            sessionManager.activeKid.dontSyncSombras = 0;
                            break;
                        case "Tesoro":
                            sessionManager.activeKid.dontSyncTesoro = 0;
                            break;
                    }
                    sessionManager.SaveSession();
                }
                Debug.Log($"the response was {request.downloadHandler.text}");
                //print("There was an error posting the high score: " + hs_post.error);
            }
        }
    }
}

[System.Serializable]
class JsonLevelToSend
{
    public Header header;
    public GameData levels;
}

class JsonIcecream : JsonLevelToSend
{
    public new IcecreamData levels;
}

[System.Serializable]
class Header
{
    public string device;
    public string version;
    public string game_key;
    public int parent_id;
    public int kid_id;
    public int passed_levels;
    public int repeated_levels;
    public int played_levels;
    public float session_time;
    public float game_time;
    public int session_number;
    public string date;
}

[System.Serializable]
class GameData
{
    public int current_level;
    public int played_levels;
    public int played_difficulty;
    public float session_time;
}

[System.Serializable]
class IcecreamData : GameData
{
    public int initial_level;
    public string total_orders;
    public string total_corrects;
    public string total_expired;
    public string total_errors;
    public string total_done;
    public string total_delivers;
    public string total_trash;
    public string latencies;
    public float session_correct_percentage;
    public float session_expired_percentage;
    public float session_errors_percentage;
}

class BirdData : GameData
{

}