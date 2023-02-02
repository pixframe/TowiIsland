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
    public TextMeshProUGUI quartileText;
    public TextMeshProUGUI AverageQText;

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
        string texto = myJson.GetValue("coins_min_correct").ToString() + "\n" + myJson.GetValue("coins_extra_missed").ToString() + "\n" + myJson.GetValue("arrange_perc_correct").ToString() + "\n" + myJson.GetValue("arrange_time").ToString() + "\n" + myJson.GetValue("unpack_correct").ToString() + "\n" + myJson.GetValue("packforward_score").ToString() + "\n" + myJson.GetValue("packbackward_score").ToString() + "\n" + myJson.GetValue("waitroom_correct").ToString() + "\n" + myJson.GetValue("flyplane_time").ToString() + "\n" + myJson.GetValue("flyplane_greencorrect").ToString() + "\n" + myJson.GetValue("flyplane_greenincorrect").ToString() + "\n" + myJson.GetValue("flyplane_correct").ToString() + "\n" + myJson.GetValue("flyplane_incorrect").ToString() + "\n" + myJson.GetValue("lab1_hits").ToString() + "\n" + myJson.GetValue("lab1_crosses").ToString() + "\n" + myJson.GetValue("lab1_deadends").ToString() + "\n" + myJson.GetValue("lab2_time").ToString() + "\n" + myJson.GetValue("lab2_latency").ToString();
        texto.Replace("'"," " );
        score.text = texto;
        Debug.Log("Aqui esta el texto" + texto);

        var genre = PlayerPrefs.GetInt($"Genre-{PlayerPrefs.GetInt("activesKid")}");
        if(genre == 1)
        {
            SetGirlQuartile(json);
        }
        else
        {
            SetBoyQuartile(json);
        }
    }

    public void SetBoyQuartile(string json)
    {
        var age = PlayerPrefs.GetInt($"Age-{PlayerPrefs.GetInt("activesKid")}");
        JSONObject myJson = JSONObject.Parse(json);
        string values = myJson.GetValue("levels").ToString();
        myJson = new JSONObject();
        myJson = JSONObject.Parse(values);
        

        if(age <= 6)
        {
            var texto = myJson.GetValue("coins_organization_score").ToString();
            var num = float.Parse(texto);
            var average = 0f;
            if (num <= 4.74f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 4.75f && num <= 7.99f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 8f && num <= 10.24f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 10.25f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("coins_extra_missed").ToString();
            num = float.Parse(texto);
            if (num > 13.25f)
            {
                quartileText.text = "4" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 13.25f && num >= 7.6f)
            {
                quartileText.text = "4" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 7.5f && num >= 3.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_perc_correct").ToString();
            num = float.Parse(texto);
            if (num <= 71.2f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 71.3f && num <= 87.03f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 87.04f && num <= 96.2f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 96.3f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_time").ToString();
            num = float.Parse(texto);
            if (num <= 271.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 272f && num <= 360.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");

            }
            else if (num >= 361f && num <= 369.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 400f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("unpack_correct").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= .9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 1f && num <= 1.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 2f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packforward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 3f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 4f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num >= 5f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packbackward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 1f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 2f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num == 3f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 4f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("waitroom_correct").ToString();
            num = float.Parse(texto);
            if (num <= 4.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 5f && num <= 7.4f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 7.5f && num <= 9.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 10f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_time").ToString();
            num = float.Parse(texto);
            if (num > 165.5f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 165.5f && num >= 120.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 120f && num >= 86.76f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 86.75f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greencorrect").ToString();
            num = float.Parse(texto);
            if (num <= 3.24f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 3.25f && num <= 4.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 5f && num <= 6.74f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 6.75f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greenincorrect").ToString();
            num = float.Parse(texto);
            if (num > 5f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 5f && num >= 2.6f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 2.5f && num >= 1.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 1f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_correct").ToString();
            num = float.Parse(texto);
            if (num <= 14.25f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 15.25f && num <= 18.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 19f && num <= 22.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 23f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_incorrect").ToString();
            num = float.Parse(texto);
            if (num > 12f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 12f && num >= 8.6f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 8.5f && num >= 4.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 4f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_hits").ToString();
            num = float.Parse(texto);
            if (num > 79f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 79f && num >= 60.1f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 60f && num >= 45.1f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 45f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_crosses").ToString();
            num = float.Parse(texto);
            if (num > 38.25f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 38.25f && num >= 26.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 26f && num >= 18.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 18f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_deadends").ToString();
            num = float.Parse(texto);
            if (num > 12.25f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 12.25f && num >= 8.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 8f && num >= 5.76f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num == 5.75f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_time").ToString();
            num = float.Parse(texto);
            if (num > 238.77f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 238.77f && num >= 160.13f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 160.12f && num >= 120.47f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 120.46f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_latency").ToString();
            num = float.Parse(texto);
            if (num > 28.8f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 28.8f && num >= 19.76f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 19.75f && num >= 16.36f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 16.35f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString();
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }
        }
        if(age == 7)
        {
            var texto = myJson.GetValue("coins_organization_score").ToString();
            var num = float.Parse(texto);
            var average = 0f;
            if (num <= 4.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 5f && num <= 7.99f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 8f && num <= 10.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 11f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("coins_extra_missed").ToString();
            num = float.Parse(texto);
            if (num > 15f)
            {
                quartileText.text = "4" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 15f && num >= 9.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 9f && num >= 4.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 4f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_perc_correct").ToString();
            num = float.Parse(texto);
            if (num <= 77.77f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 77.78f && num <= 88.88f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 88.89f && num <= 96.2f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 96.3f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_time").ToString();
            num = float.Parse(texto);
            if (num <= 250.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 251f && num <= 309.4f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");

            }
            else if (num >= 309.5f && num <= 360.24f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 360.25f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("unpack_correct").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= .9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 1f && num <= 1.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            if (num >= 2f && num <= 2.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packforward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 3f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 4f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num >= 5f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packbackward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 2f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 3f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num == 4f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("waitroom_correct").ToString();
            num = float.Parse(texto);
            if (num <= 5.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 6f && num <= 7.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 8f && num <= 9.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 10f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_time").ToString();
            num = float.Parse(texto);
            if (num > 144f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 144f && num >= 101.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 101f && num >= 76.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 76f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greencorrect").ToString();
            num = float.Parse(texto);
            if (num <= 3.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 4f && num <= 5.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 6f && num <= 6.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 7f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greenincorrect").ToString();
            num = float.Parse(texto);
            if (num > 4f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 4f && num >= 2.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 2f && num >= 1.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 1f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_correct").ToString();
            num = float.Parse(texto);
            if (num <= 16.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 17f && num <= 20.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 21f && num <= 23.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 24f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_incorrect").ToString();
            num = float.Parse(texto);
            if (num > 10f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 10f && num >= 6.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 6f && num >= 3.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_hits").ToString();
            num = float.Parse(texto);
            if (num > 79f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 79f && num >= 56.1f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 56f && num >= 38.1f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 38f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_crosses").ToString();
            num = float.Parse(texto);
            if (num > 31f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 31f && num >= 22.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 22f && num >= 16.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 16f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_deadends").ToString();
            num = float.Parse(texto);
            if (num > 10f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 10f && num >= 7.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 7f && num >= 5.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num == 5f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_time").ToString();
            num = float.Parse(texto);
            if (num > 253.54f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 253.54f && num >= 185.78f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 185.78f && num >= 128.75f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 128.74f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_latency").ToString();
            num = float.Parse(texto);
            if (num > 30.7f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 30.7f && num >= 20.2f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 20.1f && num >= 13.69f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 13.68f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString();
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }
        }
        if(age == 8)
        {
            var texto = myJson.GetValue("coins_organization_score").ToString();
            var num = float.Parse(texto);
            var average = 0f;
            if (num <= 5.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 6f && num <= 9.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 10f && num <= 12.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 13f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("coins_extra_missed").ToString();
            num = float.Parse(texto);
            if (num > 13f)
            {
                quartileText.text = "4" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 13f && num >= 7.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 7f && num >= 3.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_perc_correct").ToString();
            num = float.Parse(texto);
            if (num <= 82.40f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 82.41f && num <= 92.58f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 92.59f && num <= 96.2f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 96.3f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_time").ToString();
            num = float.Parse(texto);
            if (num <= 242.74f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 242.75f && num <= 293.4f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");

            }
            else if (num >= 293.5f && num <= 343.24f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 343.25f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("unpack_correct").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 1f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            if (num >= 2f && num <= 2.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packforward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 3f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 4f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 5f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num >= 6f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packbackward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 2f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 3f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num == 4f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 5f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("waitroom_correct").ToString();
            num = float.Parse(texto);
            if (num <= 6.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 7f && num <= 8.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 9f && num <= 9.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 10f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_time").ToString();
            num = float.Parse(texto);
            if (num > 107f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 107f && num >= 84.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 84f && num >= 67.26f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 67.25f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greencorrect").ToString();
            num = float.Parse(texto);
            if (num <= 4.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 5f && num <= 6.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 7f && num <= 7.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 8f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greenincorrect").ToString();
            num = float.Parse(texto);
            if (num > 4f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 4f && num >= 2.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 2f && num >= 1.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 1f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_correct").ToString();
            num = float.Parse(texto);
            if (num <= 18.24f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 18.25f && num <= 22.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 23f && num <= 25.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 26f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_incorrect").ToString();
            num = float.Parse(texto);
            if (num > 8f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 8f && num >= 5.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 5f && num >= 3.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_hits").ToString();
            num = float.Parse(texto);
            if (num > 63f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 63f && num >= 43.6f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 43.5f && num >= 30.1f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 30f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_crosses").ToString();
            num = float.Parse(texto);
            if (num > 27.75f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 27.75f && num >= 19.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 19f && num >= 13.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 13f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_deadends").ToString();
            num = float.Parse(texto);
            if (num > 10f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 10f && num >= 6.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 6f && num >= 4.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num == 4f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_time").ToString();
            num = float.Parse(texto);
            if (num > 223.72f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 223.72f && num >= 156.03f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 156.02f && num >= 97.97f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 97.96f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_latency").ToString();
            num = float.Parse(texto);
            if (num > 22.74f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 22.74f && num >= 16.02f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 16.01f && num >= 11.46f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 11.45f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString();
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }
        }
        if(age == 9)
        {
            var texto = myJson.GetValue("coins_organization_score").ToString();
            var num = float.Parse(texto);
            var average = 0f;
            if (num <= 7.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 8f && num <= 10.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 11f && num <= 13.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 14f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("coins_extra_missed").ToString();
            num = float.Parse(texto);
            if (num > 13f)
            {
                quartileText.text = "4" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 13f && num >= 8.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 8f && num >= 3.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_perc_correct").ToString();
            num = float.Parse(texto);
            if (num <= 81.47f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 81.48f && num <= 92.58f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 92.59f && num <= 96.2f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 96.3f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_time").ToString();
            num = float.Parse(texto);
            if (num <= 222.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 223f && num <= 255.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");

            }
            else if (num >= 256f && num <= 301.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 302f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("unpack_correct").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 1f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            if (num >= 2f && num <= 2.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packforward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 3f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 4f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 5f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 6f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packbackward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 2f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 3.5f && num <= 4.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num == 5f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 6f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("waitroom_correct").ToString();
            num = float.Parse(texto);
            if (num <= 7.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 8f && num <= 9.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 10f && num <= 10.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 11f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_time").ToString();
            num = float.Parse(texto);
            if (num > 90f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 90f && num >= 71.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 71f && num >= 60.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 60f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greencorrect").ToString();
            num = float.Parse(texto);
            if (num <= 5.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 6f && num <= 6.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 7f && num <= 7.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 8f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greenincorrect").ToString();
            num = float.Parse(texto);
            if (num > 3f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 3f && num >= 2.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 2f && num >= 1.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 1f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_correct").ToString();
            num = float.Parse(texto);
            if (num <= 21.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 22f && num <= 23.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 24f && num <= 26.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 27f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_incorrect").ToString();
            num = float.Parse(texto);
            if (num > 7f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 7f && num >= 4.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 4f && num >= 2.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 2f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_hits").ToString();
            num = float.Parse(texto);
            if (num > 62f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 62f && num >= 41.1f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 41f && num >= 26.1f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 26f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_crosses").ToString();
            num = float.Parse(texto);
            if (num > 25f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 25f && num >= 18.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 18f && num >= 11.6f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 11.5f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_deadends").ToString();
            num = float.Parse(texto);
            if (num > 8f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 8f && num >= 5.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 5f && num >= 3.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_time").ToString();
            num = float.Parse(texto);
            if (num > 207.76f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 207.76f && num >= 152.72f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 152.71f && num >= 102.99f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 102.98f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_latency").ToString();
            num = float.Parse(texto);
            if (num > 22.55f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 22.55f && num >= 13.72f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 13.71f && num >= 9.48f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 9.47f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString();
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }
        }
        if(age == 10)
        {
            var texto = myJson.GetValue("coins_organization_score").ToString();
            var num = float.Parse(texto);
            var average = 0f;
            if (num <= 8.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 9f && num <= 11.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 12f && num <= 16.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 17f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("coins_extra_missed").ToString();
            num = float.Parse(texto);
            if (num > 11f)
            {
                quartileText.text = "4" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 11f && num >= 6.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 6f && num >= 2.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 2f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_perc_correct").ToString();
            num = float.Parse(texto);
            if (num <= 88.88f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 88.89f && num <= 94.43f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 94.44f && num <= 99.98f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 99.99f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_time").ToString();
            num = float.Parse(texto);
            if (num <= 256.74f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 256.75f && num <= 287.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");

            }
            else if (num >= 288f && num <= 327.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 328f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("unpack_correct").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 1f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            if (num >= 2f && num <= 2.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packforward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 4f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 5f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 6f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num >= 7f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packbackward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 2f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 4f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num == 5f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 6f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("waitroom_correct").ToString();
            num = float.Parse(texto);
            if (num <= 7.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 8f && num <= 9.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 10f && num <= 10.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 11f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_time").ToString();
            num = float.Parse(texto);
            if (num > 78f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 78f && num >= 66.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 66f && num >= 55.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 55f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greencorrect").ToString();
            num = float.Parse(texto);
            if (num <= 5.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 6f && num <= 6.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 7f && num <= 7.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 8f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greenincorrect").ToString();
            num = float.Parse(texto);
            if (num > 3f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 3f && num >= 1.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 1f && num >= .1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 0f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_correct").ToString();
            num = float.Parse(texto);
            if (num <= 23.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 24f && num <= 25.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 26f && num <= 27.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 28f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_incorrect").ToString();
            num = float.Parse(texto);
            if (num > 5f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 5f && num >= 3.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 3f && num >= 2.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 2f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_hits").ToString();
            num = float.Parse(texto);
            if (num > 49f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 49f && num >= 32.1f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 32f && num >= 19.1f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 19f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_crosses").ToString();
            num = float.Parse(texto);
            if (num > 20f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 20f && num >= 14.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 14f && num >= 6.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 6f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_deadends").ToString();
            num = float.Parse(texto);
            if (num > 7f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 7f && num >= 4.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 4f && num >= 1.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num == 1f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_time").ToString();
            num = float.Parse(texto);
            if (num > 205.82f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 205.82f && num >= 149.14f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 149.13f && num >= 88.66f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 88.65f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_latency").ToString();
            num = float.Parse(texto);
            if (num > 21.64f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 21.64f && num >= 13.56f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 13.55f && num >= 8.89f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 8.88f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString();
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }
        }
        if(age == 11)
        {
            var texto = myJson.GetValue("coins_organization_score").ToString();
            var num = float.Parse(texto);
            var average = 0f;
            if (num <= 10.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 11f && num <= 13.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 14f && num <= 15.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 16f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("coins_extra_missed").ToString();
            num = float.Parse(texto);
            if (num > 9f)
            {
                quartileText.text = "4" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 9f && num >= 5.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 5f && num >= 2.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 2f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_perc_correct").ToString();
            num = float.Parse(texto);
            if (num <= 93.05f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 93.06f && num <= 97.21f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 97.22f && num <= 99.98f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 99.99f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_time").ToString();
            num = float.Parse(texto);
            if (num <= 246.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 247f && num <= 282.4f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");

            }
            else if (num >= 282.5f && num <= 319.24f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 319.25f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("unpack_correct").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 1f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            if (num >= 2f && num <= 2.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packforward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 4f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 5f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 6f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num >= 7f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packbackward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 2f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 4f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num == 5f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 6f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("waitroom_correct").ToString();
            num = float.Parse(texto);
            if (num <= 8.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 9f && num <= 9.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 10f && num <= 10.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 11f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_time").ToString();
            num = float.Parse(texto);
            if (num > 72f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 72f && num >= 60.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 60f && num >= 52.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 52f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greencorrect").ToString();
            num = float.Parse(texto);
            if (num <= 6.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 7f && num <= 7.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 8f && num <= 8.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 9f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greenincorrect").ToString();
            num = float.Parse(texto);
            if (num > 2f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 2f && num >= 1.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 1f && num >= .1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 0f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_correct").ToString();
            num = float.Parse(texto);
            if (num <= 23.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 24f && num <= 26.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 27f && num <= 28.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 29f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_incorrect").ToString();
            num = float.Parse(texto);
            if (num > 5f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 5f && num >= 3.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 3f && num >= 1.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 1f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_hits").ToString();
            num = float.Parse(texto);
            if (num > 37.25f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 37.25f && num >= 23.6f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 23.5f && num >= 13.1f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 13f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_crosses").ToString();
            num = float.Parse(texto);
            if (num > 18f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 18f && num >= 10.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 10f && num >= 4.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 4f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_deadends").ToString();
            num = float.Parse(texto);
            if (num > 6f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 6f && num >= 3.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 3f && num >= 1.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num == 1f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_time").ToString();
            num = float.Parse(texto);
            if (num > 189.45f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 189.45f && num >= 143.52f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 143.51f && num >= 107.44f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 107.43f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_latency").ToString();
            num = float.Parse(texto);
            if (num > 21.05f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 21.05f && num >= 11.98f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 11.97f && num >= 8.5f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 8.4f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString();
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }
        }
        if(age == 12)
        {
            var texto = myJson.GetValue("coins_organization_score").ToString();
            var num = float.Parse(texto);
            var average = 0f;
            if (num <= 10.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 11f && num <= 14.4f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 14.5f && num <= 17.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 18f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("coins_extra_missed").ToString();
            num = float.Parse(texto);
            if (num > 8f)
            {
                quartileText.text = "4" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 8f && num >= 4.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 4f && num >= 1.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 1f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_perc_correct").ToString();
            num = float.Parse(texto);
            if (num <= 91.66f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 91.67f && num <= 97.21f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 97.22f && num <= 99.98f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 99.99f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_time").ToString();
            num = float.Parse(texto);
            if (num <= 242.24f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 242.25f && num <= 280.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");

            }
            else if (num >= 280f && num <= 315.74f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 315.75f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("unpack_correct").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 1f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            if (num >= 2f && num <= 2.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packforward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 4f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 5f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 6f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 7f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packbackward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 2f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 4f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num == 5f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 6f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("waitroom_correct").ToString();
            num = float.Parse(texto);
            if (num <= 9.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 10f && num <= 9.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 10f && num <= 10.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 11f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_time").ToString();
            num = float.Parse(texto);
            if (num > 65f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 65f && num >= 57.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 57f && num >= 51.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 51f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greencorrect").ToString();
            num = float.Parse(texto);
            if (num <= 6.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 7f && num <= 7.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 8f && num <= 8.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 9f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greenincorrect").ToString();
            num = float.Parse(texto);
            if (num > 2f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 2f && num >= 1.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 1f && num >= .1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 0f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_correct").ToString();
            num = float.Parse(texto);
            if (num <= 23.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 24f && num <= 26.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 27f && num <= 28.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 29f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_incorrect").ToString();
            num = float.Parse(texto);
            if (num > 5f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 5f && num >= 2.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 2f && num >= 1.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 1f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_hits").ToString();
            num = float.Parse(texto);
            if (num > 38f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 38f && num >= 21.6f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 21.5f && num >= 10.1f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 10f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_crosses").ToString();
            num = float.Parse(texto);
            if (num > 17f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 17f && num >= 9.6f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 9.5f && num >= 3.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_deadends").ToString();
            num = float.Parse(texto);
            if (num > 5f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 5f && num >= 2.6f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 2.5f && num >= 1.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num == 1f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_time").ToString();
            num = float.Parse(texto);
            if (num > 183.28f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 183.28f && num >= 148.24f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 148.23f && num >= 103.75f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 1033.74f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_latency").ToString();
            num = float.Parse(texto);
            if (num > 25.16f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 25.16f && num >= 14.52f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 14.51f && num >= 9.33f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 9.23f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString();
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }
        }
    }

    public void SetGirlQuartile(string json)
    {
        var age = PlayerPrefs.GetInt($"Age-{PlayerPrefs.GetInt("activesKid")}");
        JSONObject myJson = JSONObject.Parse(json);
        string values = myJson.GetValue("levels").ToString();
        myJson = new JSONObject();
        myJson = JSONObject.Parse(values);
        //quartileText
        // AverageQText


        if (age <= 6)
        {
            var texto = myJson.GetValue("coins_organization_score").ToString();
            var num = float.Parse(texto);
            var average = 0f;
            if (num <= 4.74f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 4.75f && num <= 7.99f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 8f && num <= 10.24f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 10.25f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("coins_extra_missed").ToString();
            num = float.Parse(texto);
            if (num > 13.25f)
            {
                quartileText.text = "4" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 13.25f && num >= 7.6f)
            {
                quartileText.text = "4" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 7.5f && num >= 3.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_perc_correct").ToString();
            num = float.Parse(texto);
            if (num <= 77.77f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 77.78f && num <= 87.03f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 87.04f && num <= 92.58f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 92.59f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_time").ToString();
            num = float.Parse(texto);
            if (num <= 271.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 272f && num <= 360.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");

            }
            else if (num >= 361f && num <= 369.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 400f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("unpack_correct").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= .9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 1f && num <= 1.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 2f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packforward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 2f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 3f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num == 4f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num >= 4.75f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packbackward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 1f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 2f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num == 3f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 4f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("waitroom_correct").ToString();
            num = float.Parse(texto);
            if (num <= 4.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 5f && num <= 7.4f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 7.5f && num <= 9.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 10f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_time").ToString();
            num = float.Parse(texto);
            if (num > 165.5f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 165.5f && num >= 120.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 120f && num >= 86.76f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 86.75f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greencorrect").ToString();
            num = float.Parse(texto);
            if (num <= 1.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 2f && num <= 3.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 4f && num <= 5.74f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 5.75f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greenincorrect").ToString();
            num = float.Parse(texto);
            if (num >= 6.76f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 6.75f && num >= 4.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 4f && num >= 1.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 1f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_correct").ToString();
            num = float.Parse(texto);
            if (num <= 15.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 16f && num <= 17.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 18f && num <= 19.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 20f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_incorrect").ToString();
            num = float.Parse(texto);
            if (num > 12f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 12f && num >= 8.6f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 8.5f && num >= 4.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 4f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_hits").ToString();
            num = float.Parse(texto);
            if (num > 79f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 79f && num >= 60.1f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 60f && num >= 45.1f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 45f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_crosses").ToString();
            num = float.Parse(texto);
            if (num > 38.25f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 38.25f && num >= 26.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 26f && num >= 18.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 18f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_deadends").ToString();
            num = float.Parse(texto);
            if (num > 12.25f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 12.25f && num >= 8.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 8f && num >= 5.76f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num == 5.75f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_time").ToString();
            num = float.Parse(texto);
            if (num > 238.77f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 238.77f && num >= 160.13f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 160.12f && num >= 120.47f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 120.46f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_latency").ToString();
            num = float.Parse(texto);
            if (num > 28.8f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 28.8f && num >= 19.76f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 19.75f && num >= 16.36f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 16.35f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString();
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }
        }
        if (age == 7)
        {
            var texto = myJson.GetValue("coins_organization_score").ToString();
            var num = float.Parse(texto);
            var average = 0f;
            if (num <= 4.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 5f && num <= 7.99f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 8f && num <= 10.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 11f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("coins_extra_missed").ToString();
            num = float.Parse(texto);
            if (num > 15f)
            {
                quartileText.text = "4" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 15f && num >= 9.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 9f && num >= 4.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 4f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_perc_correct").ToString();
            num = float.Parse(texto);
            if (num <= 79.62f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 79.63f && num <= 88.88f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 88.89f && num <= 96.2f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 96.3f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_time").ToString();
            num = float.Parse(texto);
            if (num <= 250.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 251f && num <= 309.4f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");

            }
            else if (num >= 309.5f && num <= 360.24f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 360.25f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("unpack_correct").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= .9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 1f && num <= 1.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            if (num >= 2f && num <= 2.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packforward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 3f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 4f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num >= 5f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packbackward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 2f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 3f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num == 4f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("waitroom_correct").ToString();
            num = float.Parse(texto);
            if (num <= 5.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 6f && num <= 7.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 8f && num <= 9.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 10f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_time").ToString();
            num = float.Parse(texto);
            if (num > 144f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 144f && num >= 101.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 101f && num >= 76.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 76f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greencorrect").ToString();
            num = float.Parse(texto);
            if (num <= 32.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 3f && num <= 4.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 5f && num <= 6.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 7f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greenincorrect").ToString();
            num = float.Parse(texto);
            if (num > 5f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 5f && num >= 3.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 3f && num >= 1.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 1f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_correct").ToString();
            num = float.Parse(texto);
            if (num <= 16.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 17f && num <= 19.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 20f && num <= 22.4f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 22.5f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_incorrect").ToString();
            num = float.Parse(texto);
            if (num > 10f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 10f && num >= 6.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 6f && num >= 3.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_hits").ToString();
            num = float.Parse(texto);
            if (num > 79f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 79f && num >= 56.1f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 56f && num >= 38.1f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 38f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_crosses").ToString();
            num = float.Parse(texto);
            if (num > 31f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 31f && num >= 22.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 22f && num >= 16.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 16f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_deadends").ToString();
            num = float.Parse(texto);
            if (num > 10f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 10f && num >= 7.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 7f && num >= 5.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num == 5f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_time").ToString();
            num = float.Parse(texto);
            if (num > 253.54f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 253.54f && num >= 185.78f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 185.78f && num >= 128.75f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 128.74f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_latency").ToString();
            num = float.Parse(texto);
            if (num > 30.7f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 30.7f && num >= 20.2f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 20.1f && num >= 13.69f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 13.68f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString();
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }
        }
        if (age == 8)
        {
            var texto = myJson.GetValue("coins_organization_score").ToString();
            var num = float.Parse(texto);
            var average = 0f;
            if (num <= 5.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 6f && num <= 9.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 10f && num <= 12.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 13f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("coins_extra_missed").ToString();
            num = float.Parse(texto);
            if (num > 13f)
            {
                quartileText.text = "4" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 13f && num >= 7.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 7f && num >= 3.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_perc_correct").ToString();
            num = float.Parse(texto);
            if (num <= 77.77f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 77.78f && num <= 94.43f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 94.44f && num <= 97.21f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 97.22f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_time").ToString();
            num = float.Parse(texto);
            if (num <= 242.74f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 242.75f && num <= 293.4f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");

            }
            else if (num >= 293.5f && num <= 343.24f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 343.25f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("unpack_correct").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 1f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            if (num >= 2f && num <= 2.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packforward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 3f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 4f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 5f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num >= 6f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packbackward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 2f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 3f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num == 4f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 5f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("waitroom_correct").ToString();
            num = float.Parse(texto);
            if (num <= 6.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 7f && num <= 8.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 9f && num <= 9.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 10f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_time").ToString();
            num = float.Parse(texto);
            if (num > 107f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 107f && num >= 84.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 84f && num >= 67.26f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 67.25f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greencorrect").ToString();
            num = float.Parse(texto);
            if (num <= 4.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 5f && num <= 5.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 6f && num <= 7.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 8f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greenincorrect").ToString();
            num = float.Parse(texto);
            if (num > 3f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 3f && num >= 2.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 2f && num >= 1.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 1f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_correct").ToString();
            num = float.Parse(texto);
            if (num <= 18.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 19f && num <= 20.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 21f && num <= 23.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 24f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_incorrect").ToString();
            num = float.Parse(texto);
            if (num > 8f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 8f && num >= 5.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 5f && num >= 3.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_hits").ToString();
            num = float.Parse(texto);
            if (num > 63f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 63f && num >= 43.6f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 43.5f && num >= 30.1f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 30f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_crosses").ToString();
            num = float.Parse(texto);
            if (num > 27.75f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 27.75f && num >= 19.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 19f && num >= 13.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 13f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_deadends").ToString();
            num = float.Parse(texto);
            if (num > 10f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 10f && num >= 6.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 6f && num >= 4.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num == 4f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_time").ToString();
            num = float.Parse(texto);
            if (num > 223.72f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 223.72f && num >= 156.03f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 156.02f && num >= 97.97f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 97.96f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_latency").ToString();
            num = float.Parse(texto);
            if (num > 22.74f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 22.74f && num >= 16.02f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 16.01f && num >= 11.46f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 11.45f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString();
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }
        }
        if (age == 9)
        {
            var texto = myJson.GetValue("coins_organization_score").ToString();
            var num = float.Parse(texto);
            var average = 0f;
            if (num <= 7.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 8f && num <= 10.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 11f && num <= 13.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 14f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("coins_extra_missed").ToString();
            num = float.Parse(texto);
            if (num > 13f)
            {
                quartileText.text = "4" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 13f && num >= 8.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 8f && num >= 3.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_perc_correct").ToString();
            num = float.Parse(texto);
            if (num <= 86.10f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 86.11f && num <= 92.58f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 92.59f && num <= 99.98f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 99.99f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_time").ToString();
            num = float.Parse(texto);
            if (num <= 222.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 223f && num <= 255.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");

            }
            else if (num >= 256f && num <= 301.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 302f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("unpack_correct").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 1f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            if (num >= 2f && num <= 2.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packforward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 3f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 4f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 5f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num >= 5.75f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packbackward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 2f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 3.5f && num <= 4.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num == 5f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 6f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("waitroom_correct").ToString();
            num = float.Parse(texto);
            if (num <= 7.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 8f && num <= 9.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 10f && num <= 10.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 11f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_time").ToString();
            num = float.Parse(texto);
            if (num > 90f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 90f && num >= 71.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 71f && num >= 60.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 60f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greencorrect").ToString();
            num = float.Parse(texto);
            if (num <= 4.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 5f && num <= 5.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 6f && num <= 7.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 8f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greenincorrect").ToString();
            num = float.Parse(texto);
            if (num > 3f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 3f && num >= 2.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 2f && num >= 1.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 1f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_correct").ToString();
            num = float.Parse(texto);
            if (num <= 19.24f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 19.25f && num <= 22.4f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 22.5f && num <= 26.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 27f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_incorrect").ToString();
            num = float.Parse(texto);
            if (num > 7f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 7f && num >= 4.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 4f && num >= 2.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 2f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_hits").ToString();
            num = float.Parse(texto);
            if (num > 62f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 62f && num >= 41.1f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 41f && num >= 26.1f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 26f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_crosses").ToString();
            num = float.Parse(texto);
            if (num > 25f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 25f && num >= 18.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 18f && num >= 11.6f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 11.5f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_deadends").ToString();
            num = float.Parse(texto);
            if (num > 8f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 8f && num >= 5.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 5f && num >= 3.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_time").ToString();
            num = float.Parse(texto);
            if (num > 207.76f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 207.76f && num >= 152.72f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 152.71f && num >= 102.99f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 102.98f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_latency").ToString();
            num = float.Parse(texto);
            if (num > 22.55f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 22.55f && num >= 13.72f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 13.71f && num >= 9.48f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 9.47f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString();
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }
        }
        if (age == 10)
        {
            var texto = myJson.GetValue("coins_organization_score").ToString();
            var num = float.Parse(texto);
            var average = 0f;
            if (num <= 8.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 9f && num <= 11.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 12f && num <= 16.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 17f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("coins_extra_missed").ToString();
            num = float.Parse(texto);
            if (num > 11f)
            {
                quartileText.text = "4" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 11f && num >= 6.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 6f && num >= 2.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 2f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_perc_correct").ToString();
            num = float.Parse(texto);
            if (num <= 94.43f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 94.44f && num <= 97.21f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 97.22f && num <= 99.98f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 99.99f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_time").ToString();
            num = float.Parse(texto);
            if (num <= 256.74f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 256.75f && num <= 287.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");

            }
            else if (num >= 288f && num <= 327.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 328f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("unpack_correct").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 1f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            if (num >= 2f && num <= 2.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packforward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 3f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 4f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 5f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 6f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packbackward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 2f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 4f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num == 5f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 6f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("waitroom_correct").ToString();
            num = float.Parse(texto);
            if (num <= 7.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 8f && num <= 9.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 10f && num <= 10.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 11f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_time").ToString();
            num = float.Parse(texto);
            if (num > 78f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 78f && num >= 66.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 66f && num >= 55.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 55f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greencorrect").ToString();
            num = float.Parse(texto);
            if (num <= 4.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 5f && num <= 6.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 7f && num <= 7.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 8f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greenincorrect").ToString();
            num = float.Parse(texto);
            if (num > 3f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 3f && num >= 1.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 1f && num >= .76f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 0.75f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_correct").ToString();
            num = float.Parse(texto);
            if (num <= 21.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 22f && num <= 24.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 25f && num <= 27.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 28f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_incorrect").ToString();
            num = float.Parse(texto);
            if (num > 5f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 5f && num >= 3.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 3f && num >= 2.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 2f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_hits").ToString();
            num = float.Parse(texto);
            if (num > 49f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 49f && num >= 32.1f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 32f && num >= 19.1f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 19f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_crosses").ToString();
            num = float.Parse(texto);
            if (num > 20f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 20f && num >= 14.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 14f && num >= 6.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 6f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_deadends").ToString();
            num = float.Parse(texto);
            if (num > 7f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 7f && num >= 4.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 4f && num >= 1.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num == 1f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_time").ToString();
            num = float.Parse(texto);
            if (num > 205.82f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 205.82f && num >= 149.14f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 149.13f && num >= 88.66f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 88.65f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_latency").ToString();
            num = float.Parse(texto);
            if (num > 21.64f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 21.64f && num >= 13.56f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 13.55f && num >= 8.89f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 8.88f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString();
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }
        }
        if (age == 11)
        {
            var texto = myJson.GetValue("coins_organization_score").ToString();
            var num = float.Parse(texto);
            var average = 0f;
            if (num <= 10.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 11f && num <= 13.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 14f && num <= 15.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 16f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("coins_extra_missed").ToString();
            num = float.Parse(texto);
            if (num > 9f)
            {
                quartileText.text = "4" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 9f && num >= 5.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 5f && num >= 2.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 2f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_perc_correct").ToString();
            num = float.Parse(texto);
            if (num <= 94.43f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 94.44f && num <= 97.21f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 97.22f && num <= 99.98f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 99.99f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_time").ToString();
            num = float.Parse(texto);
            if (num <= 246.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 247f && num <= 282.4f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");

            }
            else if (num >= 282.5f && num <= 319.24f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 319.25f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("unpack_correct").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 1f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            if (num >= 2f && num <= 2.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packforward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 3f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 4f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 5f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 6f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packbackward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 2f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 4f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num == 5f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 6f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("waitroom_correct").ToString();
            num = float.Parse(texto);
            if (num <= 8.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 9f && num <= 9.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 10f && num <= 10.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 11f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_time").ToString();
            num = float.Parse(texto);
            if (num > 72f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 72f && num >= 60.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 60f && num >= 52.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 52f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greencorrect").ToString();
            num = float.Parse(texto);
            if (num <= 6.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 7f && num <= 7.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num == 8f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greenincorrect").ToString();
            num = float.Parse(texto);
            if (num > 2f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 2f && num >= 1.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 1f && num >= .1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 0f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_correct").ToString();
            num = float.Parse(texto);
            if (num <= 23.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 24f && num <= 25.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 26f && num <= 27.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 28f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_incorrect").ToString();
            num = float.Parse(texto);
            if (num > 5f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 5f && num >= 3.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 3f && num >= 1.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 1f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_hits").ToString();
            num = float.Parse(texto);
            if (num > 37.25f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 37.25f && num >= 23.6f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 23.5f && num >= 13.1f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 13f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_crosses").ToString();
            num = float.Parse(texto);
            if (num > 18f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 18f && num >= 10.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 10f && num >= 4.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 4f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_deadends").ToString();
            num = float.Parse(texto);
            if (num > 6f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 6f && num >= 3.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 3f && num >= 1.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num == 1f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_time").ToString();
            num = float.Parse(texto);
            if (num > 189.45f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 189.45f && num >= 143.52f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 143.51f && num >= 107.44f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 107.43f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_latency").ToString();
            num = float.Parse(texto);
            if (num > 21.05f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 21.05f && num >= 11.98f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 11.97f && num >= 8.5f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 8.4f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString();
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }
        }
        if (age == 12)
        {
            var texto = myJson.GetValue("coins_organization_score").ToString();
            var num = float.Parse(texto);
            var average = 0f;
            if (num <= 10.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 11f && num <= 14.4f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 14.5f && num <= 17.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 18f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("coins_extra_missed").ToString();
            num = float.Parse(texto);
            if (num > 8f)
            {
                quartileText.text = "4" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 8f && num >= 4.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 4f && num >= 1.1f)
            {
                quartileText.text = "4" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 1f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_perc_correct").ToString();
            num = float.Parse(texto);
            if (num <= 9.96f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 90.97f && num <= 97.21f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 97.22f && num <= 99.98f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num >= 99.99f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("arrange_time").ToString();
            num = float.Parse(texto);
            if (num <= 242.24f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 242.25f && num <= 280.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");

            }
            else if (num >= 280f && num <= 315.74f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else if (num >= 315.75f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("unpack_correct").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 1f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            if (num >= 2f && num <= 2.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packforward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 3f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 4f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 5f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num >= 6.25f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("packbackward_score").ToString();
            num = float.Parse(texto);
            if (num >= 0f && num <= 2f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num == 4f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num == 5f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 6f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("waitroom_correct").ToString();
            num = float.Parse(texto);
            if (num <= 9.9f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 10f && num <= 9.9f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 10f && num <= 10.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 11f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_time").ToString();
            num = float.Parse(texto);
            if (num > 65f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 65f && num >= 57.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 57f && num >= 51.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 51f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greencorrect").ToString();
            num = float.Parse(texto);
            if (num <= 6.9f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num >= 7f && num <= 7.9f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num >= 8f && num <= 8.9f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 9f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_greenincorrect").ToString();
            num = float.Parse(texto);
            if (num > 2f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 2f && num >= 1.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 1f && num >= .1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 0f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_correct").ToString();
            num = float.Parse(texto);
            if (num <= 24.74f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num >= 24.75f && num <= 27.4f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num >= 27.5f && num <= 28.9f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 29f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("flyplane_incorrect").ToString();
            num = float.Parse(texto);
            if (num > 5f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 5f && num >= 2.1f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num <= 2f && num >= 1.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else if (num == 1f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n") + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_hits").ToString();
            num = float.Parse(texto);
            if (num > 38f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 38f && num >= 21.6f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 21.5f && num >= 10.1f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 10f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_crosses").ToString();
            num = float.Parse(texto);
            if (num > 17f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
            }
            else if (num <= 17f && num >= 9.6f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
            }
            else if (num <= 9.5f && num >= 3.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
            }
            else if (num == 3f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab1_deadends").ToString();
            num = float.Parse(texto);
            if (num > 5f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 5f && num >= 2.6f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num <= 2.5f && num >= 1.1f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else if (num == 1f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString() + ("\n");
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_time").ToString();
            num = float.Parse(texto);
            if (num > 183.28f)
            {
                quartileText.text = "1" + ("\n");
                average = 1;
            }
            else if (num <= 183.28f && num >= 148.24f)
            {
                quartileText.text = "2" + ("\n");
                average = 2;
            }
            else if (num <= 148.23f && num >= 103.75f)
            {
                quartileText.text = "3" + ("\n");
                average = 3;
            }
            else if (num == 1033.74f)
            {
                quartileText.text = "4" + ("\n");
                average = 4;
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }

            texto = myJson.GetValue("lab2_latency").ToString();
            num = float.Parse(texto);
            if (num > 25.16f)
            {
                quartileText.text = "1" + ("\n");
                average += 1;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 25.16f && num >= 14.52f)
            {
                quartileText.text = "2" + ("\n");
                average += 2;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num <= 14.51f && num >= 9.33f)
            {
                quartileText.text = "3" + ("\n");
                average += 3;
                AverageQText.text = (average / 2).ToString();
            }
            else if (num == 9.23f)
            {
                quartileText.text = "4" + ("\n");
                average += 4;
                AverageQText.text = (average / 2).ToString();
            }
            else
            {
                quartileText.text = "0" + ("\n");
            }
        }
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
