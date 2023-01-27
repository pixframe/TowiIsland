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
    string dataPath;

    string scoreText;
    string winsText;
    string lostsText;
    string levelText;
    string dateText;
    string timeText;
    private void Start()
    {
        GetEvaluation();
        //GetGamesData();
        EvalPanel.SetActive(change);
        GamesPanel.SetActive(!change);
        returnButton.onClick.AddListener(ReturnToMenu);
        changeButton.onClick.AddListener(ChangeData);
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
    }

    //public void GetGamesData()
    //{
    //    dataPath = PlayerPrefs.GetString("gamesDatas");
    //    BirdsData();
    //}

    //void BirdsData()
    //{
    //    var gameCode = dataPath + "ArbolMusical";
    //    string path = $"{Application.persistentDataPath}/{gameCode}{Keys.Game_To_Save}";
    //    if (System.IO.File.Exists(path))
    //    {
    //        var json = File.ReadAllText(path);

    //        JSONObject myJson = JSONObject.Parse(json);
    //        string values = myJson.GetValue("header").ToString();
    //        myJson = new JSONObject();
    //        myJson = JSONObject.Parse(values);
    //        scoreText = myJson.GetValue("session_number").ToString();
    //    }
    //}
}
