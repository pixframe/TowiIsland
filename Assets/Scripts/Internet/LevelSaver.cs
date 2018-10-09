using UnityEngine;
using System.Collections;
using Boomlagoon.JSON;
using System.Collections.Generic;
using System;
using System.IO;

public class LevelSaver : MonoBehaviour {

    string secretKey = "$k1w1GAMES$";
    string postURL = "http://187.248.54.146:280/api/levels/";
    JSONObject data;
    JSONObject item;
    JSONObject jsonToSend;
    JSONArray levelsData;

    bool saving = false;

    string game;
    string key;
    int kidKey;

    SessionManager sessionManager;

    // Use this for initialization
    void Start ()
    {
        data = new JSONObject();
        levelsData = new JSONArray();
        jsonToSend = new JSONObject();

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

        item.Add(key, value);
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
            tempJsonArray.Add(value[i]);
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
            tempJsonArray.Add(value[i]);
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

    public void SetLevel()
    {
        levelsData.Add(item);
        //SaveFastData();
        item = new JSONObject();
    }

    public void AddLevelsToBlock()
    {
        if (data.ContainsKey("levels"))
        {
            data["levels"] = levelsData;
        }
        else
        {
            data.Add("levels", levelsData);
        }
        levelsData = new JSONArray();
        //EmergencySave();
    }

    public void CreateSaveBlock(string gameKey, float gameTime, int passedLevels, int repeatedLevels, int playedLevels)
    {
        game = gameKey;
        JSONObject headerItem = new JSONObject
        {

            { "parentid", sessionManager.activeUser.id },
            { "cid", kidKey },
            { "gameKey", gameKey },
            { "gameTime", Mathf.Round(gameTime * 100) / 100 },
            { "passedLevels", passedLevels },
            { "repeatedLevels", repeatedLevels },
            { "playedLevels", playedLevels },
            { "device", SystemInfo.deviceType.ToString()},
            { "version", "5.0" }
        };
        DateTime nowT = DateTime.Now;
        headerItem.Add("date", String.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}", nowT.Year, nowT.Month, nowT.Day, nowT.Hour, nowT.Minute, nowT.Second));
        data.Add("header", headerItem);
    }

    public void PostProgress()
    {
        saving = true;
        jsonToSend.Add("jsonToDb", data);
        StartCoroutine(PostScores());
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
        Debug.Log(form.data.ToString());
        Debug.Log(data.ToString());

        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url, form);
        yield return hs_post; // Wait until the download is done
        Debug.Log(hs_post.text);
        if (hs_post.error == null)
        {
            JSONObject response = JSONObject.Parse(hs_post.text);
            Debug.Log(response["code"].Str);
            if (response["code"].Str != "200")
            {
                //SavePending();
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

                //post_url = rankURL;

                /*form = new WWWForm();
                form.AddField("userKey", key);
                form.AddField("cid", kidKey);
                form.AddField("gameKey", game);
                DateTime tempDate = DateTime.Today;
                string dateString = String.Format("{0:0000}-{1:00}-{2:00}", (int)tempDate.Year, tempDate.Month, tempDate.Day);
                Debug.Log(dateString);
                form.AddField("date", dateString);
                hs_post = new WWW(post_url, form);
                yield return hs_post;

                if (hs_post.error == null)
                {
                    response = JSONObject.Parse(hs_post.text);
                    Debug.Log(response["code"].Str);
                    if (response["code"].Str != "200")
                    {

                    }
                }*/
            }
            Debug.Log(hs_post.text);
            //print("There was an error posting the high score: " + hs_post.error);
        }
        else
        {
            Debug.Log(hs_post.error);
            //SavePending();
        }
        saving = false;
    }
}
