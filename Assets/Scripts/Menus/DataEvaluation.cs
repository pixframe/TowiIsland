using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON;
using System;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DataEvaluation : MonoBehaviour
{
    [Header("Game UI")]
    public GameObject EvaluationCanvas;
    public Button returnButton;
    public Button changeButton;
    public GameObject EvalPanel;
    public GameObject GamesPanel;
    public TextMeshProUGUI score;
    public TextMeshProUGUI scoreT;
    public TextMeshProUGUI winsT;
    public TextMeshProUGUI lostsT;
    public TextMeshProUGUI levelT;
    public TextMeshProUGUI dateT;
    public TextMeshProUGUI timeT;

    bool change = true;
    bool change2 = false;
    string dataPath;

    string scoreText;
    string winsText;
    string lostsText;
    string levelText;
    string dateText;
    string timeText;
    private void Start()
    {
        EvalPanel.SetActive(change);
        GamesPanel.SetActive(change2);
        returnButton.onClick.AddListener(ReturnToMenu);
        changeButton.onClick.AddListener(ChangeData);
        GetEvaluation();
        GetGamesData();
    }

    public void GetEvaluation()
    {
        string kidData = $"{PlayerPrefs.GetInt("activesKid")}";
        Debug.Log(kidData);
        string kidOnDate = PlayerPrefs.GetString(kidData);
        string path = $"{Application.persistentDataPath}/{kidOnDate}{Keys.Evaluation_To_Save}";
        var json = File.ReadAllText(path);
        SetJson(json);
       // Debug.Log(path);
       // Debug.Log(json);
       // JSONObject eval = JSONObject.Parse(json);
       // Debug.Log(eval);
    }

    public void SetJson(string json)
    {
        JSONObject myJson = JSONObject.Parse(json);
        string values = myJson.GetValue("levels").ToString();
        myJson = new JSONObject();
        myJson = JSONObject.Parse(values);
        Debug.Log(myJson);
        string texto = myJson.GetValue("coins_organization_score").ToString() + "\n" + myJson.GetValue("coins_clickfinish_before_min").ToString() + "\n" + myJson.GetValue("arrange_perc_correct").ToString() + "\n" + myJson.GetValue("arrange_time").ToString() + "\n" + myJson.GetValue("unpack_correct").ToString() + "\n" + myJson.GetValue("packforward_score").ToString() + "\n" + myJson.GetValue("packbackward_score").ToString() + "\n" + myJson.GetValue("waitroom_correct").ToString() + "\n" + myJson.GetValue("flyplane_time").ToString() + "\n" + myJson.GetValue("flyplane_correct").ToString() + "\n" + myJson.GetValue("flyplane_incorrect").ToString() + "\n" + myJson.GetValue("flyplane_greencorrect").ToString() + "\n" + myJson.GetValue("flyplane_greenincorrect").ToString() + "\n" + myJson.GetValue("lab1_hits").ToString() + "\n" + myJson.GetValue("lab1_crosses").ToString() + "\n" + myJson.GetValue("lab1_deadends").ToString() + "\n" + myJson.GetValue("lab2_time").ToString() + "\n" + myJson.GetValue("lab2_latency").ToString();
        texto.Replace("'"," " );
        score.text = texto;
        Debug.Log("Aqui esta el texto" + texto);
    }

    public void ReturnToMenu()
    {
        PrefsKeys.SetNextScene("NewLogin");
        change = true;
        SceneManager.LoadScene("Loader_Scene");
    }

    public void ChangeData()
    {
        change = !change;
        change2 = !change2;
        EvalPanel.SetActive(change);
        GamesPanel.SetActive(change2);
    }

    public void GetGamesData()
    {
        BirdsData();
        scoreT.text = scoreText;
        winsT.text = winsText;
        lostsT.text = lostsText;
        levelT.text = levelText;
        dateT.text = dateText;
        timeT.text = timeText;
    }

    void BirdsData()
    {
        dataPath = $"{PlayerPrefs.GetInt("BirdSession")}-{PlayerPrefs.GetInt("activesKid")}";
        var gameCode = $"{ dataPath}-ArbolMusical";
        string path = $"{Application.persistentDataPath}/{gameCode}{Keys.Game_To_Save}";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);

            JSONObject myJason = JSONObject.Parse(json);
            string values = myJason.GetValue("header").ToString();
            var myJeison = JSONObject.Parse(values);
            values = " ";
            values = myJason.GetValue("levels").ToString();
            myJason = new JSONObject();
            myJason = JSONObject.Parse(values);

            string numbe = myJeison.GetValue("session_number").ToString();
            int myNumbe = int.Parse(numbe);

            if (PlayerPrefs.HasKey("BirdsSession"))
            {
                var ok = PlayerPrefs.GetInt("BirdsSession");
                if (ok != myNumbe)
                {
                    ok += myNumbe;
                    PlayerPrefs.SetInt("BirdsSession", ok);
                    scoreText += ok.ToString() + ("\n");
                }
                else
                {
                    scoreText += ok.ToString() + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetInt("BirdsSession", myNumbe);
            }
            
            numbe = "";
            numbe = myJason.GetValue("session_correct_percentage").ToString();
            float myTime = float.Parse(numbe);

            if (PlayerPrefs.HasKey("BirdsWins"))
            {
                var ok = PlayerPrefs.GetFloat("BirdsWins");
                if (ok != myTime)
                {
                    ok += myTime;
                    PlayerPrefs.SetFloat("BirdsWin", ok);
                    winsText += ok.ToString() +"%" + ("\n");
                }
                else
                {
                    winsText += ok.ToString() + "%" + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetFloat("BirdsWins", myNumbe);
                winsText += $"{myTime}%" + ("\n");
            }

            numbe = "";
            numbe = myJason.GetValue("session_errors_percentage").ToString();
            myTime = float.Parse(numbe);

            if (PlayerPrefs.HasKey("BirdsErrors"))
            {
                var ok = PlayerPrefs.GetFloat("BirdsErrors");
                if (ok != myTime)
                {
                    ok += myTime;
                    PlayerPrefs.SetFloat("BirdsErrors", ok);
                    lostsText += ok.ToString() + "%" + ("\n");
                }
                else
                {
                    lostsText += ok.ToString() + "%" + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetFloat("BirdsErrors", myTime);
                lostsText += $"{myTime}%" + ("\n");
            }

            numbe = "";
            numbe = myJason.GetValue("current_level").ToString();
            myNumbe = int.Parse(numbe);

            levelText += myNumbe + ("\n");

            numbe = "";
            numbe = myJeison.GetValue("date").ToString();

            dateText += numbe + ("\n");

            numbe = "";
            numbe = myJeison.GetValue("session_time").ToString();
            myTime = float.Parse(numbe);

            if (PlayerPrefs.HasKey("BirdsTime"))
            {
                var ok = PlayerPrefs.GetFloat("BirdsTime");
                if (ok != myTime)
                {
                    ok += myTime;
                    PlayerPrefs.SetFloat("BirdsTime", ok);
                    timeText += ok.ToString() + ("\n");
                }
                else
                {
                    timeText += ok.ToString() + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetFloat("BirdsTime", myTime);
                timeText += $"{myTime}" + ("\n");
            }
        }
        else
        {
            scoreText += "0" + "\n";
            winsText += "0%" + "\n";
            lostsText += "0%" + "\n";
            levelText += "0" + "\n";
            dateText += "0" + "\n";
            timeText += "0" + "\n";
        }

        SandData();
    }

    void SandData()
    {
        dataPath = $"{PlayerPrefs.GetInt("SandSession")}-{PlayerPrefs.GetInt("activesKid")}";
        var gameCode = $"{ dataPath}-ArenaMagica";
        string path = $"{Application.persistentDataPath}/{gameCode}{Keys.Game_To_Save}";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);

            JSONObject myJason = JSONObject.Parse(json);
            string values = myJason.GetValue("header").ToString();
            var myJeison = JSONObject.Parse(values);
            values = " ";
            values = myJason.GetValue("levels").ToString();
            myJason = new JSONObject();
            myJason = JSONObject.Parse(values);

            string numbe = myJeison.GetValue("session_number").ToString();
            int myNumbe = int.Parse(numbe);

            if (PlayerPrefs.HasKey("SandsSession"))
            {
                var ok = PlayerPrefs.GetInt("SandsSession");
                if (ok != myNumbe)
                {
                    ok += myNumbe;
                    PlayerPrefs.SetInt("SandsSession", ok);
                    scoreText += ok.ToString() + ("\n");
                }
                else
                {
                    scoreText += ok.ToString() + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetInt("SandsSession", myNumbe);
            }
            
            numbe = "";
            numbe = myJason.GetValue("session_accuracy_percentage").ToString();
            float myTime = float.Parse(numbe);

            if (PlayerPrefs.HasKey("SandsWins"))
            {
                var ok = PlayerPrefs.GetFloat("SandsWins");
                if (ok != myTime)
                {
                    ok += myTime;
                    PlayerPrefs.SetFloat("SandsWin", ok);
                    winsText += ok.ToString() +"%" + ("\n");
                }
                else
                {
                    winsText += ok.ToString() + "%" + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetFloat("SandsWins", myNumbe);
                winsText += $"{myTime}%" + ("\n");
            }

            numbe = "";
            numbe = myJason.GetValue("session_overdraw_percentage").ToString();
            myTime = float.Parse(numbe);

            if (PlayerPrefs.HasKey("SandsErrors"))
            {
                var ok = PlayerPrefs.GetFloat("SandsErrors");
                if (ok != myTime)
                {
                    ok += myTime;
                    PlayerPrefs.SetFloat("SandsErrors", ok);
                    lostsText += ok.ToString() + "%" + ("\n");
                }
                else
                {
                    lostsText += ok.ToString() + "%" + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetFloat("SandErrors", myTime);
                lostsText += $"{myTime}%" + ("\n");
            }

            numbe = "";
            numbe = myJason.GetValue("current_level").ToString();
            myNumbe = int.Parse(numbe);

            levelText += myNumbe + ("\n");

            numbe = "";
            numbe = myJeison.GetValue("date").ToString();

            dateText += numbe + ("\n");

            numbe = "";
            numbe = myJeison.GetValue("session_time").ToString();
            myTime = float.Parse(numbe);

            if (PlayerPrefs.HasKey("SandsTime"))
            {
                var ok = PlayerPrefs.GetFloat("SandsTime");
                if (ok != myTime)
                {
                    ok += myTime;
                    PlayerPrefs.SetFloat("SandsTime", ok);
                    timeText += ok.ToString() + ("\n");
                }
                else
                {
                    timeText += ok.ToString() + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetFloat("SandsTime", myTime);
                timeText += $"{myTime}" + ("\n");
            }
        }
        else
        {
            scoreText += "0" + "\n";
            winsText += "0%" + "\n";
            lostsText += "0%" + "\n";
            levelText += "0" + "\n";
            dateText += "0" + "\n";
            timeText += "0" + "\n";
        }
        MonkeysData();
    }

    void MonkeysData()
    {
        dataPath = $"{PlayerPrefs.GetInt("MonkeySession")}-{PlayerPrefs.GetInt("activesKid")}";
        var gameCode = $"{ dataPath}-DondeQuedoLaBolita";
        string path = $"{Application.persistentDataPath}/{gameCode}{Keys.Game_To_Save}";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);

            JSONObject myJason = JSONObject.Parse(json);
            string values = myJason.GetValue("header").ToString();
            var myJeison = JSONObject.Parse(values);
            values = " ";
            values = myJason.GetValue("levels").ToString();
            myJason = new JSONObject();
            myJason = JSONObject.Parse(values);

            string numbe = myJeison.GetValue("session_number").ToString();
            int myNumbe = int.Parse(numbe);

            if (PlayerPrefs.HasKey("MonkeysSession"))
            {
                var ok = PlayerPrefs.GetInt("MonkeysSession");
                if (ok != myNumbe)
                {
                    ok += myNumbe;
                    PlayerPrefs.SetInt("MonkeysSession", ok);
                    scoreText += ok.ToString() + ("\n");
                }
                else
                {
                    scoreText += ok.ToString() + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetInt("MonkeysSession", myNumbe);
            }

            numbe = "";
            numbe = myJason.GetValue("session_correct_percentage").ToString();
            float myTime = float.Parse(numbe);

            if (PlayerPrefs.HasKey("MonkeysWins"))
            {
                var ok = PlayerPrefs.GetFloat("MonkeysWins");
                if (ok != myTime)
                {
                    ok += myTime;
                    PlayerPrefs.SetFloat("MonkeysWin", ok);
                    winsText += ok.ToString() + "%" + ("\n");
                }
                else
                {
                    winsText += ok.ToString() + "%" + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetFloat("MonkeysWins", myNumbe);
                winsText += $"{myTime}%" + ("\n");
            }

            numbe = "";
            numbe = myJason.GetValue("session_errors_percentage").ToString();
            myTime = float.Parse(numbe);

            if (PlayerPrefs.HasKey("MonkeysErrors"))
            {
                var ok = PlayerPrefs.GetFloat("MonkeysErrors");
                if (ok != myTime)
                {
                    ok += myTime;
                    PlayerPrefs.SetFloat("MonkeysErrors", ok);
                    lostsText += ok.ToString() + "%" + ("\n");
                }
                else
                {
                    lostsText += ok.ToString() + "%" + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetFloat("MonkeysErrors", myTime);
                lostsText += $"{myTime}%" + ("\n");
            }

            numbe = "";
            numbe = myJason.GetValue("current_level").ToString();
            myNumbe = int.Parse(numbe);

            levelText += myNumbe + ("\n");

            numbe = "";
            numbe = myJeison.GetValue("date").ToString();

            dateText += numbe + ("\n");

            numbe = "";
            numbe = myJeison.GetValue("session_time").ToString();
            myTime = float.Parse(numbe);

            if (PlayerPrefs.HasKey("MonkeysTime"))
            {
                var ok = PlayerPrefs.GetFloat("MonkeysTime");
                if (ok != myTime)
                {
                    ok += myTime;
                    PlayerPrefs.SetFloat("MonkeysTime", ok);
                    timeText += ok.ToString() + ("\n");
                }
                else
                {
                    timeText += ok.ToString() + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetFloat("MonkeysTime", myTime);
                timeText += $"{myTime}" + ("\n");
            }
        }
        else
        {
            scoreText += "0" + "\n";
            winsText += "0%" + "\n";
            lostsText += "0%" + "\n";
            levelText += "0" + "\n";
            dateText += "0" + "\n";
            timeText += "0" + "\n";
        }
        HuntData();
    }

    void HuntData()
    {
        dataPath = $"{PlayerPrefs.GetInt("HuntSession")}-{PlayerPrefs.GetInt("activesKid")}";
        var gameCode = $"{ dataPath}-Tesoro";
        string path = $"{Application.persistentDataPath}/{gameCode}{Keys.Game_To_Save}";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);

            JSONObject myJason = JSONObject.Parse(json);
            string values = myJason.GetValue("header").ToString();
            var myJeison = JSONObject.Parse(values);
            values = " ";
            values = myJason.GetValue("levels").ToString();
            myJason = new JSONObject();
            myJason = JSONObject.Parse(values);

            string numbe = myJeison.GetValue("session_number").ToString();
            int myNumbe = int.Parse(numbe);

            if (PlayerPrefs.HasKey("HuntsSession"))
            {
                var ok = PlayerPrefs.GetInt("HuntsSession");
                if (ok != myNumbe)
                {
                    ok += myNumbe;
                    PlayerPrefs.SetInt("HuntsSession", ok);
                    scoreText += ok.ToString() + ("\n");
                }
                else
                {
                    scoreText += ok.ToString() + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetInt("HuntsSession", myNumbe);
            }

            numbe = "";
            numbe = myJason.GetValue("session_correct_percentage").ToString();
            float myTime = float.Parse(numbe);

            if (PlayerPrefs.HasKey("HuntsWins"))
            {
                var ok = PlayerPrefs.GetFloat("HuntsWins");
                if (ok != myTime)
                {
                    ok += myTime;
                    PlayerPrefs.SetFloat("HuntsWin", ok);
                    winsText += ok.ToString() + "%" + ("\n");
                }
                else
                {
                    winsText += ok.ToString() + "%" + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetFloat("HuntsWins", myNumbe);
                winsText += $"{myTime}%" + ("\n");
            }

            numbe = "";
            numbe = myJason.GetValue("session_errors_percentage").ToString();
            myTime = float.Parse(numbe);

            if (PlayerPrefs.HasKey("HuntsErrors"))
            {
                var ok = PlayerPrefs.GetFloat("HuntsErrors");
                if (ok != myTime)
                {
                    ok += myTime;
                    PlayerPrefs.SetFloat("HuntsErrors", ok);
                    lostsText += ok.ToString() + "%" + ("\n");
                }
                else
                {
                    lostsText += ok.ToString() + "%" + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetFloat("HuntsErrors", myTime);
                lostsText += $"{myTime}%" + ("\n");
            }

            numbe = "";
            numbe = myJason.GetValue("current_level").ToString();
            myNumbe = int.Parse(numbe);

            levelText += myNumbe + ("\n");

            numbe = "";
            numbe = myJeison.GetValue("date").ToString();

            dateText += numbe + ("\n");

            numbe = "";
            numbe = myJeison.GetValue("session_time").ToString();
            myTime = float.Parse(numbe);

            if (PlayerPrefs.HasKey("HuntsTime"))
            {
                var ok = PlayerPrefs.GetFloat("HuntsTime");
                if (ok != myTime)
                {
                    ok += myTime;
                    PlayerPrefs.SetFloat("HuntsTime", ok);
                    timeText += ok.ToString() + ("\n");
                }
                else
                {
                    timeText += ok.ToString() + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetFloat("HuntsTime", myTime);
                timeText += $"{myTime}" + ("\n");
            }
        }
        else
        {
            scoreText += "0" + "\n";
            winsText += "0%" + "\n";
            lostsText += "0%" + "\n";
            levelText += "0" + "\n";
            dateText += "0" + "\n";
            timeText += "0" + "\n";
        }
        RiverData();
    }

    void RiverData()
    {
        dataPath = $"{PlayerPrefs.GetInt("RiverSession")}-{PlayerPrefs.GetInt("activesKid")}";
        var gameCode = $"{ dataPath}-Rio";
        string path = $"{Application.persistentDataPath}/{gameCode}{Keys.Game_To_Save}";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);

            JSONObject myJason = JSONObject.Parse(json);
            string values = myJason.GetValue("header").ToString();
            var myJeison = JSONObject.Parse(values);
            values = " ";
            values = myJason.GetValue("levels").ToString();
            myJason = new JSONObject();
            myJason = JSONObject.Parse(values);

            string numbe = myJeison.GetValue("session_number").ToString();
            int myNumbe = int.Parse(numbe);

            if (PlayerPrefs.HasKey("BirdsSession"))
            {
                var ok = PlayerPrefs.GetInt("RiversSession");
                if (ok != myNumbe)
                {
                    ok += myNumbe;
                    PlayerPrefs.SetInt("RiversSession", ok);
                    scoreText += ok.ToString() + ("\n");
                }
                else
                {
                    scoreText += ok.ToString() + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetInt("RiversSession", myNumbe);
            }

            numbe = "";
            numbe = myJason.GetValue("session_correct_percentage").ToString();
            float myTime = float.Parse(numbe);

            if (PlayerPrefs.HasKey("RiversWins"))
            {
                var ok = PlayerPrefs.GetFloat("RiversWins");
                if (ok != myTime)
                {
                    ok += myTime;
                    PlayerPrefs.SetFloat("RiversWin", ok);
                    winsText += ok.ToString() + "%" + ("\n");
                }
                else
                {
                    winsText += ok.ToString() + "%" + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetFloat("RiversWins", myNumbe);
                winsText += $"{myTime}%" + ("\n");
            }

            numbe = "";
            numbe = myJason.GetValue("session_errors_percentage").ToString();
            myTime = float.Parse(numbe);

            if (PlayerPrefs.HasKey("RiversErrors"))
            {
                var ok = PlayerPrefs.GetFloat("RiversErrors");
                if (ok != myTime)
                {
                    ok += myTime;
                    PlayerPrefs.SetFloat("RiversErrors", ok);
                    lostsText += ok.ToString() + "%" + ("\n");
                }
                else
                {
                    lostsText += ok.ToString() + "%" + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetFloat("RiversErrors", myTime);
                lostsText += $"{myTime}%" + ("\n");
            }

            numbe = "";
            numbe = myJason.GetValue("current_level").ToString();
            myNumbe = int.Parse(numbe);

            levelText += myNumbe + ("\n");

            numbe = "";
            numbe = myJeison.GetValue("date").ToString();

            dateText += numbe + ("\n");

            numbe = "";
            numbe = myJeison.GetValue("session_time").ToString();
            myTime = float.Parse(numbe);

            if (PlayerPrefs.HasKey("RiversTime"))
            {
                var ok = PlayerPrefs.GetFloat("RiversTime");
                if (ok != myTime)
                {
                    ok += myTime;
                    PlayerPrefs.SetFloat("RiversTime", ok);
                    timeText += ok.ToString() + ("\n");
                }
                else
                {
                    timeText += ok.ToString() + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetFloat("RiversTime", myTime);
                timeText += $"{myTime}" + ("\n");
            }
        }
        else
        {
            scoreText += "0" + "\n";
            winsText += "0%" + "\n";
            lostsText += "0%" + "\n";
            levelText += "0" + "\n";
            dateText += "0" + "\n";
            timeText += "0" + "\n";
        }
        ShadowData();
    }

    void ShadowData()
    {
        dataPath = $"{PlayerPrefs.GetInt("ShadowSession")}-{PlayerPrefs.GetInt("activesKid")}";
        var gameCode = $"{ dataPath}-JuegoDeSombras";
        string path = $"{Application.persistentDataPath}/{gameCode}{Keys.Game_To_Save}";
        Debug.Log("Es el pat:" + path);
        Debug.Log("Es el session:" + PlayerPrefs.GetInt("ShadowSession"));
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);

            JSONObject myJason = JSONObject.Parse(json);
            string values = myJason.GetValue("header").ToString();
            var myJeison = new JSONObject();
            myJeison = JSONObject.Parse(values);
            values = " ";
            values = myJason.GetValue("levels").ToString();
            Debug.Log(values);
            myJason = new JSONObject();
            myJason = JSONObject.Parse(values);

            string numbe = myJeison.GetValue("session_number").ToString();
            int myNumbe = int.Parse(numbe);

            if (PlayerPrefs.HasKey("ShadowsSession"))
            {
                var ok = PlayerPrefs.GetInt("ShadowsSession");
                if(ok != myNumbe)
                {
                    ok += myNumbe;
                    PlayerPrefs.SetInt("ShadowsSession", ok);
                    scoreText += ok.ToString() + ("\n");
                }
                else
                {
                    scoreText += ok.ToString() + ("\n");
                }
                
            }
            else
            {
                PlayerPrefs.SetInt("ShadowsSession", myNumbe);
            }

            numbe = "";
            numbe = myJason.GetValue("session_correct_percentage").ToString();
            float myTime = float.Parse(numbe);
            Debug.Log(myTime);
            if (PlayerPrefs.HasKey("ShadowWins"))
            {
                
                var ok = PlayerPrefs.GetFloat("ShadowWins");
                Debug.Log(ok);
                if (ok != myTime)
                {
                    ok += myTime;
                    Debug.Log(ok);
                    PlayerPrefs.SetFloat("ShadowWins", ok);
                    winsText += ok.ToString() + ("\n");
                }
                else
                {
                    winsText += ok.ToString() + ("\n");
                }
            }
            else
            {
                Debug.Log("xd");
                PlayerPrefs.SetFloat("ShadowWins", myTime);
                winsText += $"{myTime}%"+ ("\n");
            }

            numbe = "";
            numbe = myJason.GetValue("session_errors_percentage").ToString();
            myTime = float.Parse(numbe);
            Debug.Log("errores my time" + myTime);
            if (PlayerPrefs.HasKey("ShadowErrors"))
            {
                Debug.Log("xd fail");
                var ok = PlayerPrefs.GetFloat("ShadowErrors");
                Debug.Log(ok);
                if (ok != myTime)
                {
                    ok += myTime;
                    PlayerPrefs.SetFloat("ShadowErrors", ok);
                    lostsText += ok.ToString() + ("\n");
                }
                else
                {
                    lostsText += ok.ToString()+ "%" + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetFloat("ShadowErrors", myTime);
                lostsText += $"{myTime}%" + ("\n");
                
            }

            numbe = "";
            numbe = myJason.GetValue("current_level").ToString();
            myNumbe = int.Parse(numbe);
            Debug.Log("my level is:" + myNumbe);
            levelText += myNumbe + ("\n");

            numbe = "";
            numbe = myJeison.GetValue("date").ToString();

            dateText += numbe + ("\n");

            numbe = "";
            numbe = myJeison.GetValue("session_time").ToString();
            myTime = float.Parse(numbe);


            if (PlayerPrefs.HasKey("ShadowTime"))
            {
                var ok = PlayerPrefs.GetFloat("ShadowTime");
                if (ok != myTime)
                {
                    ok += myTime;
                    PlayerPrefs.SetFloat("ShadowTime", ok);
                    timeText += ok.ToString() + ("\n");
                }
                else
                {
                    timeText += ok.ToString() + ("\n");
                }
            }
            else
            {
                PlayerPrefs.SetFloat("ShadowTime", myTime);
                timeText += $"{myTime}" + ("\n"); ;
            }
        }
        else
        {
            scoreText += "0";
            winsText += "0%";
            lostsText += "0%";
            levelText += "0";
            dateText += "0";
            timeText += "0";
        }
    }
}
