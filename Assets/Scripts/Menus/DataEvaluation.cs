using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON;
using System;
using System.IO;
using UnityEngine.UI;

public class DataEvaluation : MonoBehaviour
{
    [Header("Game UI")]
    public GameObject EvaluationCanvas;
    public Button returnButton;


    private void Start()
    {
        GetEvaluation();
    }

    public void GetEvaluation()
    {
        string path = $"{Application.persistentDataPath}/0{Keys.Evaluation_To_Save}";
        var json = File.ReadAllText(path);
        Debug.Log(path);
        Debug.Log(json);
       // JSONObject eval = JSONObject.Parse(json);
       // Debug.Log(eval);
    }
}
