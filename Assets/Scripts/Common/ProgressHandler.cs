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
    string syncURL = "http://www.towi.com.mx/api/sync.php";
    string rankURL = "http://www.towi.com.mx/api/towi_index.php";
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
        localGame = Login.local;
		//dataDynamic=new List<string>();
		//StartCoroutine(GetScores());
	}

	void Start()
    {
		if(!localGame)
			Sync ();
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

	/*public void SetLevel()
    {
		levelsData.Add("", item);
        SaveFastData();
        item = new JSONObject();
	}*/

    public void SetRawData()
    {

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

    public IEnumerator PostProgressConos(System.Action<bool> result)
    {
        bool success = false;
        saving = true;
        yield return StartCoroutine(PostScoresConos(value => success = value));
        result(success);
    }

    IEnumerator PostScoresConos(System.Action<bool> result)
    {
        string hash = Md5Sum(data.ToString() + secretKey);
        string post_url = postURL;

        // Build form to post in server
        WWWForm form = new WWWForm();

        form.AddField("jsonToDb", data.ToString());

        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url, form);
        yield return hs_post; // Wait until the download is done

        if (hs_post.error == null)
        {
            JSONObject response = JSONObject.Parse(hs_post.text);
            Debug.Log(response["code"].Str);
            if (response["code"].Str != "200")
            {
                result(false);
            }
            else
            {
                result(true);
            }
            Debug.Log(hs_post.text);
        }
        else
        {
            Debug.Log(hs_post.error);
            result(false);
        }
        saving = false;
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

    public void PostProgress(bool rank){
		if(!localGame)
		{
			saving = true;
			StartCoroutine(PostScores(rank));
		}
	}
	// remember to use StartCoroutine when calling this function!
	IEnumerator PostScores(bool rank)
	{
		//This connects to a server side php script that will add the name and score to a MySQL DB.
		// Supply it with a string representing the players name and the players score.
		string hash = Md5Sum(data.ToString() + secretKey);
		string post_url = postURL/* + "name=" + WWW.EscapeURL(name) + "&score=" + score + "&hash=" + hash*/;
		
		// Build form to post in server
		WWWForm form = new WWWForm();
		//form.AddField("data", data.ToString());
		form.AddField("jsonToDb", data.ToString());
		
		// Post the URL to the site and create a download object to get the result.
		WWW hs_post = new WWW(post_url,form);
		yield return hs_post; // Wait until the download is done

		if (hs_post.error == null) {
			JSONObject response= JSONObject.Parse(hs_post.text);
			Debug.Log(response["code"].Str);
			if(response["code"].Str!="200"){
				SavePending();
			}else
			{
				switch(game)
				{
					case "ArbolMusical":
						sessionManager.activeKid.dontSyncArbolMusical=0;
					break;
					case "Rio":
						sessionManager.activeKid.dontSyncRio=0;
					break;
					case "ArenaMagica":
						sessionManager.activeKid.dontSyncArenaMagica=0;
					break;
					case "DondeQuedoLaBolita":
						sessionManager.activeKid.dontSyncDondeQuedoLaBolita=0;
					break;
					case "JuegoDeSombras":
						sessionManager.activeKid.dontSyncSombras=0;
					break;
					case "Tesoro":
						sessionManager.activeKid.dontSyncTesoro=0;
					break;
				}
				sessionManager.SaveSession();

				post_url = rankURL;

				form = new WWWForm();
				form.AddField("userKey", key);
				form.AddField("cid", kidKey);
				form.AddField("gameKey", game);
				DateTime tempDate=DateTime.Today;
				string dateString=String.Format("{0:0000}-{1:00}-{2:00}",(int)tempDate.Year,tempDate.Month,tempDate.Day);
				Debug.Log(dateString);
				form.AddField("date", dateString);
				hs_post = new WWW(post_url,form);
				yield return hs_post; 
				
				if (hs_post.error == null) {
					response= JSONObject.Parse(hs_post.text);
					Debug.Log(response["code"].Str);
					if(response["code"].Str!="200"){

					}
				}
			}
			Debug.Log (hs_post.text);
			//print("There was an error posting the high score: " + hs_post.error);
		} else {
			Debug.Log(hs_post.error);
			SavePending();
		}
		saving = false;
	}

	
	IEnumerator PostSync(JSONObject syncData)
	{
		// Build form to post in server
		WWWForm form = new WWWForm();
		//form.AddField("data", data.ToString());
		form.AddField("syncjsonToDB",syncData.ToString());
		
		// Post the URL to the site and create a download object to get the result.
		WWW hs_post = new WWW(syncURL,form);
		yield return hs_post; // Wait until the download is done
		
		if (hs_post.error == null)
		{
			JSONArray response= JSONArray.Parse(hs_post.text);
			JSONArray jsonItems=syncData["pending"].Array;
			int offsetIdx=0;
			bool remaining=false;
			for(int i=0;i<response.Length;i++){
				if(response[i].Obj["code"].Str=="200")
				{
					jsonItems.Remove(i-offsetIdx);
					offsetIdx++;
				}
				else{
					remaining=true;
				}
			}
			if(remaining)
			{
				Debug.Log("Sync Incomplete");
				Debug.Log(syncData.ToString());
				sessionManager.activeKid.offlineData=syncData.ToString();
				sessionManager.SaveSession();
			}else{
				Debug.Log("Sync Complete");
				Debug.Log(syncData.ToString());
				sessionManager.activeKid.offlineData="";
				sessionManager.SaveSession();
			}
		} else 
		{
			Debug.Log("Sync Error");
			Debug.Log(hs_post.error);
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

	void Sync(){
		if(!localGame && sessionManager)
		{
			string pendingData = sessionManager.activeKid.offlineData;
			if(pendingData!="")
			{
				JSONObject jsonData=JSONObject.Parse(pendingData);
				JSONArray jsonItems=jsonData["pending"].Array;
				for(int i=0;i<jsonItems.Length;i++){
					jsonItems[i].Obj["header"].Obj["userKey"]=key;
				}
				//PlayerPrefs.SetString("offlineData",jsonData.ToString());
				//Debug.Log(jsonData.ToString());
				StartCoroutine(PostSync(jsonData));
			}
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

        string path = $"{Application.persistentDataPath}/{evaluationSavedOffline}_{Keys.Evaluation_To_Save}";
        evaluationSavedOffline++;

        Debug.Log($"We have {evaluationSavedOffline} jsons to save");

        File.WriteAllText(path, dataToSave);

        PlayerPrefs.SetInt(Keys.Games_Saved, evaluationSavedOffline);

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
