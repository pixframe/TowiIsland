using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Boomlagoon.JSON;
using System.Collections.Generic;
using System;
using System.IO;
using System.Globalization;

public class ProgressHandler : MonoBehaviour {
	
	private string secretKey = "$k1w1GAMES$"; // Edit this value and make sure it's the same as the one stored on the server
	public string postURL = ""; //be sure to add a ? to your url
    string postSuperURL = Keys.Api_Web_Key + "api/v2/levels/assesment/";
    //string header;
    //string []data;
    //List<string>dataDynamic;
    JSONObject data;
	JSONObject levelsData;
	JSONObject item;
    JSONObject rawItem;
	private string key;
	private string game;
	private int kidKey;
	public bool saving=false;
	SessionManager sessionManager;
    LastSceneManager lastSceneManager;
	bool localGame;

    CultureInfo invariantCulture = CultureInfo.InvariantCulture;

	void OnEnable()
	{
        sessionManager = FindObjectOfType<SessionManager>();
        if (sessionManager != null)
        {
            key = sessionManager.activeKid.userkey;
            kidKey = sessionManager.activeKid.id;
        }
        data = new JSONObject();
        levelsData = new JSONObject();
        item = new JSONObject();
        rawItem = new JSONObject();
		//dataDynamic=new List<string>();
		//StartCoroutine(GetScores());
	}

	void Start()
    {

	}

	public void AddLevelData(string key,int value)
    {
		if (item==null)
			item = new JSONObject ();
        item.Add(key, value);
	}
	public void AddLevelData(string key,float value){
        if (item == null)
        {
            item = new JSONObject();
        }
        int valueInteger = Mathf.RoundToInt(value * 1000);
        float fToSave = (( valueInteger/ 1000f));
        item.Add(key, fToSave.ToString(invariantCulture));
	}

	public void AddLevelData(string key,string value){
		if (item==null)
			item = new JSONObject ();
		item.Add(key,value);
	}

	public void AddLevelData(string key,bool value){
		if (item==null)
			item = new JSONObject ();
        item.Add(key, value);
	}

	public void AddLevelData(string key,string[] value){
        if (item == null)
        {
            item = new JSONObject();
        }

        string stringToGo = "";

        for (int i = 0; i < value.Length; i++)
        {
            stringToGo += value[i].ToString() + ",";
        }

        item.Add(key, stringToGo);
    }

    public void AddLevelData(string key, int[] value)
    {
        if (item == null)
        {
            item = new JSONObject();
        }

        string stringToGo = "";

        for (int i = 0; i < value.Length; i++)
        {
            stringToGo += value[i].ToString(invariantCulture) + ",";
        }

        item.Add(key, stringToGo);
    }

    public void AddLevelData(string key, float[] value)
    {
		if (item==null)
        {
            item = new JSONObject();
        }

        string stringToGo = "";

        for (int i = 0; i < value.Length; i++)
        {
            stringToGo += value[i].ToString(invariantCulture) + ",";
        }

        item.Add(key, stringToGo);
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

        string stringToGo = "";

        for (int i = 0; i < value.Count; i++)
        {
            stringToGo += value[i].ToString() + ",";
        }

        item.Add(key, stringToGo);
    }

    public void AddLevelData(string key, List<int> value)
    {
        if (item == null)
        {
            item = new JSONObject();
        }

        string stringToGo = "";

        for (int i = 0; i < value.Count; i++)
        {
            stringToGo += value[i].ToString() + ",";
        }

        item.Add(key, stringToGo);
    }

    public void AddLevelData(string key, List<float> value)
    {
        if (item == null)
        {
            item = new JSONObject();
        }

        string stringToGo = "";

        for (int i = 0; i < value.Count; i++)
        {
            stringToGo += value[i].ToString(invariantCulture) + ",";
        }

        item.Add(key, stringToGo);
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

    public void AddRawData(string key, List<string> value)
    {
        if (rawItem == null)
        {
            rawItem = new JSONObject();
        }

        JSONArray tempJsonArray = new JSONArray();

        for (int i = 0; i < value.Count; i++)
        {
            tempJsonArray.Add(value[i]);
        }

        rawItem.Add(key, tempJsonArray);
    }

    public void AddRawData(string key, List<int> value)
    {
        if (rawItem == null)
        {
            rawItem = new JSONObject();
        }

        JSONArray tempJsonArray = new JSONArray();

        for (int i = 0; i < value.Count; i++)
        {
            tempJsonArray.Add(value[i]);
        }

        rawItem.Add(key, tempJsonArray);
    }

    public void AddRawData(string key, List<float> value)
    {
        if (rawItem == null)
        {
            rawItem = new JSONObject();
        }

        JSONArray tempJsonArray = new JSONArray();

        for (int i = 0; i < value.Count; i++)
        {
            tempJsonArray.Add(value[i]);
        }

        rawItem.Add(key, tempJsonArray);
    }

    public void AddRawData(string key, List<List<string>> value)
    {
        if (rawItem == null)
        {
            rawItem = new JSONObject();
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

        rawItem.Add(key, tempJsonArray);
    }

    public void SaveFlashProbes()
    {
        if (data.ContainsKey("levels"))
        {
            data["levels"] = levelsData;
        }
        else
        {
            data.Add("levels", levelsData);
        }
        levelsData = new JSONObject();
        SaveTheFlashProbe();
    }

    public void SaveFastData()
    {
        if (data.ContainsKey("levels"))
        {
            data["levels"] = levelsData;
        }
        else
        {
            data.Add("levels", levelsData);
        }
        levelsData = new JSONObject();
        SaveTheDataForNow();
    }

	public void AddLevelsToBlock()
    {
		if (data.ContainsKey("levels"))
        {
            data["levels"] = levelsData;
		}
        else
        {
            data.Add("levels", item);
		}
        levelsData = new JSONObject();
	}

	public void CreateSaveBlock(string gameKey,float gameTime,int passedLevels,int repeatedLevels,int playedLevels){
		game = gameKey;
        JSONObject headerItem = new JSONObject();
		headerItem.Add("userKey",key);
		headerItem.Add("cid",kidKey);
		headerItem.Add("gameKey",gameKey);
		headerItem.Add("gameTime", Mathf.Round(gameTime*100)/100);
		headerItem.Add("passedLevels", passedLevels);
		headerItem.Add("repeatedLevels", repeatedLevels);
		headerItem.Add("playedLevels", playedLevels);
		DateTime nowT = DateTime.Now;
		headerItem.Add ("date", String.Format ("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}",nowT.Year, nowT.Month, nowT.Day,nowT.Hour, nowT.Minute, nowT.Second));
		data.Add("header",headerItem);
	}

    public void CreateSaveBlock(string gameKey, float gameTime)
    {
        game = gameKey;
        JSONObject headerItem = new JSONObject
        {
            { "parent_id", sessionManager.activeUser.id},
            { "kid_id", kidKey },
            { "game_key", gameKey },
            { "game_time", (int)Mathf.Round(gameTime * 100) / 100 },
            { "device", SystemInfo.deviceType.ToString() },
            { "version", "5.0" },
            { "passed_levels", 0},
            { "repeated_levels", 0},
            { "played_levels", 0},
        };
        DateTime nowT = DateTime.Now;
        headerItem.Add("date", String.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}", nowT.Year, nowT.Month, nowT.Day, nowT.Hour, nowT.Minute, nowT.Second));
        data.Add("header", headerItem);
    }

    public void createSaveBlockConos(string gameKey, string name, string age, string sex, int routeIdx, DateTime date)
    {
        game = gameKey;
        JSONObject headerItem = new JSONObject();
        headerItem.Add("gameKey", gameKey);
        headerItem.Add("name", name);
        headerItem.Add("age", age);
        headerItem.Add("sex", sex);
        headerItem.Add("routeId", routeIdx);
        headerItem.Add("testDate", String.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second));
        data.Add("header", headerItem);
    }

    public void PostEvaluationData(LastSceneManager last)
    {
        lastSceneManager = last;
        StartCoroutine(PostEvaluation());
    }

    IEnumerator PostEvaluation()
    {
        WWWForm form = new WWWForm();
        form.AddField("jsonToDb", data.ToString());
        Debug.Log($"the form is \n{form.ToString()}");
        using (UnityWebRequest request = UnityWebRequest.Post(postSuperURL, form))
        {
            Debug.Log(request.ToString());
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                EmergencySave();
            }
            else
            {
                Debug.Log("This is done");
            }
            FindObjectOfType<EvaluationController>().DataIsSend();
            lastSceneManager.MoveToMenu();
        }
    }
	 
	void SavePending(){
        if (sessionManager)
        {
            string offlineData = sessionManager.activeKid.offlineData;
            if (offlineData != "")
            {
                JSONObject jsonOffline = JSONObject.Parse(offlineData);
                JSONArray jsonOfflineArray = jsonOffline["pending"].Array;
                jsonOffline.GetArray("pending");
                jsonOfflineArray.Add(data);
                jsonOffline["pending"] = jsonOfflineArray;
                sessionManager.activeKid.offlineData = jsonOffline.ToString();
                sessionManager.SaveSession();
            }
            else
            {
                JSONObject jsonOffline = new JSONObject();
                JSONArray jsonOfflineArray = new JSONArray();
                jsonOfflineArray.Add(data);
                jsonOffline.Add("pending", jsonOfflineArray);
                sessionManager.activeKid.offlineData = jsonOffline.ToString();
                sessionManager.SaveSession();
            }
            JSONObject jsontemp = JSONObject.Parse(sessionManager.activeKid.offlineData);
            Debug.Log(jsontemp.ToString());
            Debug.Log(jsontemp["pending"].ToString());
        }
	}

	// Get the scores from the MySQL DB to display in a GUIText.
	// remember to use StartCoroutine when calling this function!
	/*IEnumerator GetScores()
	{
		gameObject.guiText.text = "Loading Scores";
		WWW hs_get = new WWW(highscoreURL);
		yield return hs_get;
		
		if (hs_get.error != null)
		{
			print("There was an error getting the high score: " + hs_get.error);
		}
		else
		{
			gameObject.guiText.text = hs_get.text; // this is a GUIText that will display the scores in game.
		}
	}*/
	
	public  string Md5Sum(string strToEncrypt)
	{
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
		byte[] bytes = ue.GetBytes(strToEncrypt);
		
		// encrypt bytes
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);
		
		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";
		
		for (int i = 0; i < hashBytes.Length; i++)
		{
			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}
		
		return hashString.PadLeft(32, '0');
	}


    public void EmergencySave()
    {

        string dataToSave = data.ToString();
        int evaluationSavedOffline = PlayerPrefs.GetInt(Keys.Evaluations_Saved);

        PlayerPrefs.SetString($"{sessionManager.activeKid.name}-{evaluationSavedOffline}", $"{DateTime.Now.Month} - {DateTime.Now.Day} - {sessionManager.activeKid.id}");

        string kidData = PlayerPrefs.GetString($"{sessionManager.activeKid.name}-{evaluationSavedOffline}");

        string path = $"{Application.persistentDataPath}/{kidData}{Keys.Evaluation_To_Save}";
        evaluationSavedOffline++;
        Debug.Log($"El dia de hoy es: {kidData}");
        Debug.Log($"Y el id es: {sessionManager.activeKid.id}");
        Debug.Log($"We have {evaluationSavedOffline} jsons to save");
        Debug.Log(path);
        File.WriteAllText(path, dataToSave);
        PlayerPrefs.SetString("evaluationPath", path);
        PlayerPrefs.SetInt(Keys.Evaluations_Saved, evaluationSavedOffline);

        sessionManager.SaveSession();

    }

    void SaveTheFlashProbe()
    {
        PlayerPrefs.SetInt(Keys.Flash_Probe_Num, (PlayerPrefs.GetInt(Keys.Flash_Probe_Num) + 1));
        string content = data.ToString();
        string path = Application.persistentDataPath + "/FlashProbe"+ "PlayerPrefs.GetInt(Keys.Flash_Probe_Num)" + ".txt";
        File.WriteAllText(path, content);
    }

    void SaveTheDataForNow()
    {
        string path = Application.persistentDataPath + "/SuperQuick" + PlayerPrefs.GetInt(Keys.Easy_Evaluation) + ".txt";
        PlayerPrefs.SetInt(Keys.Easy_Evaluation, (PlayerPrefs.GetInt(Keys.Easy_Evaluation) + 1));
        string content = data.ToString();
        File.WriteAllText(path, content);
        Debug.Log(content);
    }
}
