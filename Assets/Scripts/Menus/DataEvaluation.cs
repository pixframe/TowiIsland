using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON;
using System;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class DataEvaluation : MonoBehaviour
{
    [Header("Game UI")]
    public GameObject EvaluationCanvas;
    public Button returnButton;
    public TextMeshProUGUI score;
    public TextMeshProUGUI users;
    public TextMeshProUGUI quartile;
    public TextMeshProUGUI averageQ;

    private void Start()
    {
        GetEvaluation();
    }

    public void GetEvaluation()
    {
        string path = $"{Application.persistentDataPath}/1 - 24 - 35{Keys.Evaluation_To_Save}";
        var json = File.ReadAllText(path);
        SetJson(json);
        Debug.Log(path);
        Debug.Log(json);
       // JSONObject eval = JSONObject.Parse(json);
       // Debug.Log(eval);
    }

    public void SetJson(string json)
    {
        JSONObject myJson = JSONObject.Parse(json);
        Debug.Log(myJson);
        var texto = myJson.GetValue("coins_organization_score") + "\n" + myJson.GetValue("coins_clickfinish_before_min") + "\n" + myJson.GetValue("arrange_perc_correct") + "\n" + myJson.GetValue("arrange_time") + "\n" + myJson.GetValue("unpack_correct") + "\n" + myJson.GetValue("packforward_score") + "\n" + myJson.GetValue("packbackward_score") + "\n" + myJson.GetValue("waitroom_correct") + "\n" + myJson.GetValue("flyplane_time") + "\n" + myJson.GetValue("flyplane_correct") + "\n" + myJson.GetValue("flyplane_incorrect") + "\n" + myJson.GetValue("flyplane_greencorrect") + myJson.GetValue("flyplane_greenincorrect") + "\n" + myJson.GetValue("lab1_hits") + "\n" + myJson.GetValue("lab1_crosses") + "\n" + myJson.GetValue("lab1_deadends") + "\n" + myJson.GetValue("lab2_time") + "\n" + myJson.GetValue("lab2_latency");
        score.text = texto;
        Debug.Log("Aqui esta el texto" + texto);
    }
}
