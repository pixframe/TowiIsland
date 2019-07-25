using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class EvaluationController : MonoBehaviour
{
    //needed scrpits to handle the proyect
    MetricsProcessing metricsProcessing;
    GameSaveLoad saveLoad;
    ProgressHandler saveHandler;
    DemoKey key;
    SessionManager sessionManager;

    //Need Variables to handle the excercises backend
    float secctionTime;
    float totalTimeTest;
    bool countingSectionTime;
    bool isDataSend;
    int ageOfTheCurrentPlayer;
    int tutorialsPlayed = 1;

    // Use this for initialization
    void Awake()
    {
        sessionManager = FindObjectOfType<SessionManager>();
        ageOfTheCurrentPlayer = sessionManager.activeKid.age;
        EvaluationController[] evas = FindObjectsOfType<EvaluationController>();
        if (evas.Length > 1)
        {
            for (int i = 0; i < evas.Length; i++)
            {
                if (evas[i] != this)
                {
                    Destroy(evas[i].gameObject);
                }
            }
        }
        DontDestroyOnLoad(this.gameObject);
        saveHandler = GetComponent<ProgressHandler>();
        var audioRemember = FindObjectOfType<NeedAudioMenu>();
        audioRemember.SetHideButtonFunction(GoToSceneOne);
    }

    void GoToSceneOne()
    {
        key = FindObjectOfType<DemoKey>();
        if (key == null)
        {
            GoToNextScene("Evaluation_Scene1");
        }
    }

    // Update is called once per frame
    void Update()
    {
        totalTimeTest += Time.deltaTime;
        if (countingSectionTime)
        {
            secctionTime += Time.deltaTime;
        }
    }

    public void SetAge(int age)
    {
        ageOfTheCurrentPlayer = age;
    }

    //Restart the secction counter of the section to 0 to start counting another test;
    void RestartSecctionTime(){
        tutorialsPlayed = 1;
        secctionTime = 0;
    }

    //This region helps to count the time of ech individual secction
    public void StarCounting()
    {
        countingSectionTime = true;
    }

    //this will stop the counting of the test
    public void StopCounting()
    {
        countingSectionTime = false;
    }

    //we used this function to add numbers of play times
    public void TutorialPlay()
    {
        tutorialsPlayed++;
    }

    //Save the data in the first part of the first section
    public void SaveAgeProgress(int ageOfPlayer, string birthOfPlayer, float latencie)
    {
        saveHandler.AddLevelData("boarding_latency", latencie);
        saveHandler.AddLevelData("boarding_age", ageOfPlayer);
        saveHandler.AddLevelData("boarding_birthday", birthOfPlayer);
        saveHandler.AddLevelData("boarding_time1", Mathf.Round(secctionTime));

        StopCounting();
        RestartSecctionTime();
    }

    //Save tHe data in the second part of the first secction
    public void SaveBuyTicketProgress(string playerName, string playerPlace, string currentDate)
    {
        saveHandler.AddLevelData("boarding_name", playerName);
        saveHandler.AddLevelData("boarding_address", playerPlace);
        saveHandler.AddLevelData("boarding_currentdate", currentDate);
        saveHandler.AddLevelData("boarding_time2", Mathf.Round(secctionTime));

        StopCounting();
        RestartSecctionTime();
        GoToNextScene("Evaluation_Scene2");
    }
    
    //This will save all the data recolected in the packing foward part of the second section
    //TODO assay latencies
	public void SavePackProgress(int normalLevel, int errorsFront, List<List<int>> rawSelectedObjects, int intrusion, int badOrdered)
    {
        saveHandler.AddLevelData("packforward_tutorial", tutorialsPlayed);
        saveHandler.AddLevelData("packforward_score", normalLevel + 3);
        saveHandler.AddLevelData("packforward_time", secctionTime);
        saveHandler.AddLevelData("packforward_incorrect_secuence", badOrdered);
        saveHandler.AddLevelData("packforward_instrusions", intrusion);

        /*saveHandler.AddLevelData("packErrors", errorsFront);
        saveHandler.AddLevelData("packRawData", rawSelectedObjects);*/

        //saveHandler.SetLevel();

		StopCounting();
		RestartSecctionTime();
	}

    //This will save the data recolected in the packing backward part of the second part
    //TODO assay latencies
    public void SavePackBackProgress(int backLevel, int errorsBack, List<List<int>> rawSelectedObjects, int intrusion, int badOrdered)
    {
        saveHandler.AddLevelData("packbackward_tutorial", tutorialsPlayed);
        saveHandler.AddLevelData("packbackward_score", backLevel + 2);
        saveHandler.AddLevelData("packbackward_time", secctionTime);
        saveHandler.AddLevelData("packbackward_incorrect_secuence", badOrdered);
        saveHandler.AddLevelData("packbackward_intrusions", intrusion);


        /*saveHandler.AddLevelData("reversePackErrors", errorsBack);

        saveHandler.AddLevelData("packBackRawData", rawSelectedObjects);*/


        StopCounting();
        RestartSecctionTime();
    }

    //this will save the weather part of the second part
    public void SaveTheWeatherProgress(List<string> weatherList, float latency, int score)
    {
        saveHandler.AddLevelData("weather_object_packed", weatherList);
        saveHandler.AddLevelData("weather_latency", latency);
        //saveHandler.AddLevelData("weather_coherence", 9);
        saveHandler.AddLevelData("weather_time", Mathf.Round(secctionTime));

        StopCounting();
        RestartSecctionTime();
        GoToNextScene("Evaluation_Scene3");
    }

    //this will recollect all the data of the third section of the test
    //TODO SinglesData
    public void SaveCarLaberynthProgress(float[] latencies, float[] labTimes, int[] hits, int[] crosses, int[] changeRoute, int[] deadEnds, Texture2D[] photos, int difficulty)
    {
        int laberyntNumber = 3;
        int totalHits = 0;
        int totalCrosses = 0;
        int totalDeadEnds = 0;
        for (int i = 0; i < laberyntNumber; i++)
        {
            totalHits += hits[i];
            totalCrosses += crosses[i];
            totalDeadEnds += deadEnds[i];
            saveHandler.AddLevelData("lab" + (i + 1).ToString() + "_time", labTimes[i]);
            saveHandler.AddLevelData("lab" + (i + 1).ToString() + "_latency", latencies[i]);
            saveHandler.AddLevelData("lab" + (i + 1).ToString() + "_hits", hits[i]);
            saveHandler.AddLevelData("lab" + (i + 1).ToString() + "_crosses", crosses[i]);
            saveHandler.AddLevelData("lab" + (i + 1).ToString() + "_deadends", deadEnds[i]);
            saveHandler.AddLevelData("lab" + (i + 1).ToString() + "_changeofroutes", changeRoute[i]);
        }
        /*saveHandler.AddLevelData("lab1_time", labTimes[0]);
        saveHandler.AddLevelData("lab2_time", labTimes[1]);
        saveHandler.AddLevelData("lab3_time", labTimes[2]);
        saveHandler.AddLevelData("lab1_latency", latencies[0]);
        saveHandler.AddLevelData("lab2_latency", latencies[1]);
        saveHandler.AddLevelData("lab3_latency", latencies[2]);
        saveHandler.AddLevelData("lab1_hits", hits[0]);
        saveHandler.AddLevelData("lab2_hits", hits[1]);
        saveHandler.AddLevelData("lab3_hits", hits[2]);
        saveHandler.AddLevelData("lab1_crosses", crosses[0]);
        saveHandler.AddLevelData("lab2_crosses", crosses[1]);
        saveHandler.AddLevelData("lab3_crosses", crosses[2]);
        saveHandler.AddLevelData("lab1_deadends", deadEnds[0]);
        saveHandler.AddLevelData("lab2_deadends", deadEnds[1]);
        saveHandler.AddLevelData("lab3_deadends", deadEnds[2]);
        saveHandler.AddLevelData("lab1_changeofroutes", changeRoute[0]);
        saveHandler.AddLevelData("lab1_changeofroutes", changeRoute[1]);
        saveHandler.AddLevelData("lab1_changeofroutes", changeRoute[2]);
        for (int i = 0; i < 3; i++)
        {
            totalHits += hits[i];
            totalCrosses += crosses[i];
            totalDeadEnds += deadEnds[i];
        }*/
        saveHandler.AddLevelData("lab_start_level", difficulty);
        saveHandler.AddLevelData("lab_mhits", IntDivider(totalHits, laberyntNumber));
        saveHandler.AddLevelData("lab_mcrosses", IntDivider(totalCrosses, laberyntNumber));
        saveHandler.AddLevelData("lab_mdeadends", IntDivider(totalDeadEnds, laberyntNumber));
        saveHandler.AddLevelData("lab_time", Mathf.Round(secctionTime));

        StopCounting();
        RestartSecctionTime();
        GoToNextScene("Evaluation_Scene4");
    }

    //this will save the data relecolected from the fourth part of the test
    public void SaveWaitingRoomProgress(int good, int goodInScreen, int extraGood, float[] goodLatencies, int bad, int badInScreen, int extraBad, float[] badLatencies, int[] interaction, float[] latencies)
    {
        saveHandler.AddLevelData("waitroom_correct", good);
        saveHandler.AddLevelData("waitroom_incorrect", bad);

        float medium = 0;
        for (int i = 0; i < goodLatencies.Length; i++)
        {
            medium += goodLatencies[i];
        }
        saveHandler.AddLevelData("waitroom_correct_mlatency", FloatDivider(medium, goodLatencies.Length));

        medium = 0;
        for (int i = 0; i < badLatencies.Length; i++)
        {
            medium += badLatencies[i];
        }
        saveHandler.AddLevelData("waitroom_incorrect_mlatency", FloatDivider(medium, badLatencies.Length));

        saveHandler.AddLevelData("waitroom_tutorial", tutorialsPlayed);

        /*saveHandler.AddLevelData("waitRoomCorrectInScreen", goodInScreen);
        saveHandler.AddLevelData("waitRoomExtraCorrect", extraGood);
        saveHandler.AddLevelData("waitRoomIncorrectInScreen", badInScreen);
        saveHandler.AddLevelData("waitRoomExtraIncorrect", extraBad);
        saveHandler.AddLevelData("waitRoomInteractionClick", interaction);
        saveHandler.AddLevelData("waitRoomFullLatencies", latencies);
        saveHandler.AddLevelData("waitRoomTimeOfComp", secctionTime);*/

        StopCounting();
        RestartSecctionTime();
        GoToNextScene("Evaluation_Scene5");
    }

    //This will save the data recolected from the fith part of the test
    public void SaveTheArrowProgress(int good, int gGood, int bad, int gBad, int miss, int gMiss, List<float> goodL, List<float> gGoodL, List<float> badL, List<float> gBadL, List<int> interactions, List<float> latencies, int difficulty)
    {
        saveHandler.AddLevelData("flyplane_tutorial", tutorialsPlayed);
        saveHandler.AddLevelData("flyplane_series", difficulty);
        saveHandler.AddLevelData("flyplane_correct", good);
        saveHandler.AddLevelData("flyplane_incorrect", bad);
        saveHandler.AddLevelData("flyplane_missed", miss);
        saveHandler.AddLevelData("flyplane_greencorrect", gGood);
        saveHandler.AddLevelData("flyplane_greenincorrect", gBad);
        saveHandler.AddLevelData("flyplane_greenmissed", gMiss);

        float lat = 0;
        float prom = 0;
        for (int i = 0; i < goodL.Count; i++)
        {
            lat += goodL[i];
        }
        saveHandler.AddLevelData("flyplane_correct_mlatency", FloatDivider(lat, goodL.Count));

        lat = 0;
        for (int i = 0; i < badL.Count; i++)
        {
            lat += badL[i];
        }
        prom = (lat / badL.Count);
        saveHandler.AddLevelData("flyplane_incorrect_mlatency", FloatDivider(lat, badL.Count));

        lat = 0;
        for (int i = 0; i < gGoodL.Count; i++)
        {
            lat += gGoodL[i];
        }
        prom = (lat / gGoodL.Count);
        saveHandler.AddLevelData("flyplane_greencorrect_mlatency", FloatDivider(lat, gGoodL.Count));

        lat = 0;
        for (int i = 0; i < gBadL.Count; i++)
        {
            lat += gBadL[i];
        }
        prom = (lat / gBadL.Count);
        saveHandler.AddLevelData("flyplane_greenincorrect_mlatency", FloatDivider(lat, gBadL.Count));
        saveHandler.AddLevelData("flyplane_time", Mathf.Round(secctionTime));

        /*saveHandler.AddLevelData("flyPlanesLatencies", goodL);
        saveHandler.AddLevelData("flyPlanesBadLatencies", badL);
        saveHandler.AddLevelData("flyPlanesGreenLatencie", gGoodL);
        saveHandler.AddLevelData("flyPlanesGreenBadLatencies", gBadL);
        saveHandler.AddLevelData("flyPlanesInteractions", interactions);
        saveHandler.AddLevelData("flyPlanesInteractionLatencies", latencies);

        saveHandler.SetLevel();*/

        StopCounting();
        RestartSecctionTime();
        GoToNextScene("Evaluation_Scene6");
    }

    //this will save the progress of the coins game
    public void SaveCoinsProgress(int good, int bad, int gX, int bX, int score, List<int> coins, List<float> latencies,List<float> goodLatencies ,List<float> badLatencies, int coinsLevel, int pauses)
    {
        saveHandler.AddLevelData("coins_level", coinsLevel);
        saveHandler.AddLevelData("coins_min_correct", good);
        saveHandler.AddLevelData("coins_min_incorrect", bad);
        saveHandler.AddLevelData("coins_extra_correct", gX);
        saveHandler.AddLevelData("coins_extra_incorrect", bX);
        saveHandler.AddLevelData("coins_extra_missed", 24 - (good + gX));
        saveHandler.AddLevelData("coins_selected", coins);
        saveHandler.AddLevelData("coins_organization_score", score);
        saveHandler.AddLevelData("coins_clickfinish_before_min", pauses);

        float prom = 0;
        for (int i = 0; i < goodLatencies.Count; i++)
        {
            prom += goodLatencies[i];
        }
        saveHandler.AddLevelData("coins_correct_mlatency", FloatDivider(prom, goodLatencies.Count));

        prom = 0;
        for (int i = 0; i < badLatencies.Count; i++)
        {
            prom += badLatencies[i];
        }
        saveHandler.AddLevelData("coins_incorrect_mlatency", FloatDivider(prom, badLatencies.Count));

        //saveHandler.AddLevelData("coins_pattern_type", 5);
        saveHandler.AddLevelData("coins_time", Mathf.Round(secctionTime));

        //saveHandler.AddLevelData("coinsLatencies", latencies);

        //saveHandler.SetLevel();

        StopCounting();
        RestartSecctionTime();
        GoToNextScene("Evaluation_Scene7");
    }

    //this will save the unpackprogress
    public void SaveUnpackProgress(string firstItem, string firstCorrectItem, string[] unpackObjects, int badrecognition, int scoreOfUnpack, List<string> takenObjects, int reapeted, int fillSpaces)
    {
        saveHandler.AddLevelData("unpack_first_selected", firstItem);
        saveHandler.AddLevelData("unpack_first_correct", firstCorrectItem);
        saveHandler.AddLevelData("unpack_incorrect", fillSpaces - scoreOfUnpack);
        saveHandler.AddLevelData("unpack_perseveration", reapeted);
        saveHandler.AddLevelData("unpack_bad_recognition", badrecognition);
        saveHandler.AddLevelData("unpack_correct", scoreOfUnpack);
        //saveHandler.AddLevelData("unpack_objects_selected", takenObjects);
        saveHandler.AddLevelData("unpack_time", secctionTime);
        //saveHandler.AddLevelData("unPackObjects", unpackObjects);
        //saveHandler.SetLevel();

        StopCounting();
        RestartSecctionTime();
    }

    //this will save the progreess in the last part
    public void SaveOrderProgress(int nOfStimulus, int[] corrects, int[] bads, int[] repeated, int[] firstObjects, int[] lastObjects, int[] positionScore, int[] wellPositioned, List<float> asseyLatencie, List<List<string>> listOfOrdnung, List<List<string>> goodReapeted, List<List<string>> badReapeted)
    {
        int numberOfAssays = 3;
        int totalCorrects = 0;
        float firstPercentage = 0;
        float lastpercentage = 0;
        for (int i = 0; i < numberOfAssays; i++)
        {
            saveHandler.AddLevelData("arrange" + (i + 1).ToString() + "_correct", corrects[i]);
            saveHandler.AddLevelData("arrange" + (i + 1).ToString() + "_incorrect", bads[i]);
            saveHandler.AddLevelData("arrange" + (i + 1).ToString() + "_perseveration", repeated[i]);
            firstPercentage += firstObjects[i];
            lastpercentage += lastObjects[i];
            totalCorrects += corrects[i];
        }
        saveHandler.AddLevelData("arrange_learningcurve", corrects[2] - corrects[0]);
        saveHandler.AddLevelData("arrange_amplitude", nOfStimulus);
        /*saveHandler.AddLevelData("arrange_primacy", firstObjects);
        saveHandler.AddLevelData("arrange_recence", lastObjects);*/
        saveHandler.AddLevelData("arrange_spacialprecision_score", positionScore);
        saveHandler.AddLevelData("arrange_spacialprecision_sample", wellPositioned);
        saveHandler.AddLevelData("arrange_time", Mathf.Round(secctionTime));
        saveHandler.AddLevelData("arrange_primacy", (firstPercentage / 3));
        saveHandler.AddLevelData("arrange_recence", (lastpercentage / 3));
        saveHandler.AddLevelData("arrange_perc_correct", GetPercentage(totalCorrects, (numberOfAssays * nOfStimulus)));
        for (int i = 0; i < 2; i++)
        {
            saveHandler.AddLevelData("arrange_storage_efficency" + (i+1).ToString(), goodReapeted[i].Count);
        }

        List<string> incorrectReapeted = new List<string>();
        for (int i = 0; i < badReapeted[0].Count; i++)
        {
            for (int j = 0; j < badReapeted[1].Count; j++)
            {
                if (badReapeted[0][i] == badReapeted[1][j])
                {
                    incorrectReapeted.Add(badReapeted[0][i]);
                }
            }
        }
        saveHandler.AddLevelData("arrange_incorrect_repeated", incorrectReapeted);
        saveHandler.AddLevelData("total_time", totalTimeTest);

        /*saveHandler.AddLevelData("orderReapeted", repeated);
        saveHandler.AddLevelData("orderLatencies", asseyLatencie);
        saveHandler.AddLevelData("orderGoodRepeated", goodReapeted);
        saveHandler.AddLevelData("orderBadRepeated", badReapeted);
        saveHandler.AddLevelData("orderFullData", listOfOrdnung);*/
        //saveHandler.SetLevel();

        StopCounting();
        RestartSecctionTime();
        saveHandler.CreateSaveBlock("PruebaEcologica", totalTimeTest);
        saveHandler.AddLevelsToBlock();
        GoToNextScene("Evaluation_Scene8");
    }

    public void FinishEvaluation()
    {
        sessionManager.activeKid.needSync = true;
        sessionManager.activeKid.testAvailable = false;
        StartCoroutine(FinishTheGame());
    }

    IEnumerator FinishTheGame()
    {
        yield return new WaitUntil(() => isDataSend);
        GoToNextScene("NewLogin");
    }

    public float GetPercentage(int playerValue, int maxValue)
    {
        float division = (float)playerValue / (float)maxValue;
        return division *= 100f;
    }

    public void DataIsSend()
    {
        isDataSend = true;
    }

    public void SaveFlashCarData(string name, int age, int[] hits, float[] latencies, float[] labtimes)
    {
        saveHandler.AddLevelData("name", name);
        saveHandler.AddLevelData("age", age);
        //saveHandler.SetLevel();
        saveHandler.AddLevelData("latenciesInLabs", latencies);
        saveHandler.AddLevelData("hitsInLabs", hits);
        saveHandler.AddLevelData("labtimes", labtimes);
        //saveHandler.SetLevel();
        StopCounting();
        RestartSecctionTime();
        GoToNextScene("Evaluation_Scene5");
    }

    public void SaveArrows1(int good, int gGood, int bad, int gBad, int miss, int gMiss, List<float> goodL, List<float> gGoodL, List<float> badL, List<float> gBadL, List<int> interactions, List<float> latencies)
    {
        saveHandler.AddLevelData("flyPlaneCorrect", good);
        saveHandler.AddLevelData("flyPlaneInorrect", bad);
        saveHandler.AddLevelData("flyPlaneMiss", miss);
        saveHandler.AddLevelData("flyPlaneGreenCorrect", gGood);
        saveHandler.AddLevelData("flyPlaneGreenInorrect", gBad);
        saveHandler.AddLevelData("flyPlaneGreenMiss", gMiss);
        saveHandler.AddLevelData("flyPlanesLatencies", goodL);
        saveHandler.AddLevelData("flyPlanesBadLatencies", badL);
        saveHandler.AddLevelData("flyPlanesGreenLatencie", gGoodL);
        saveHandler.AddLevelData("flyPlanesGreenBadLatencies", gBadL);
        saveHandler.AddLevelData("flyPlanesInteractions", interactions);
        saveHandler.AddLevelData("flyPlanesInteractionLatencies", latencies);
        saveHandler.AddLevelData("flyPlaneTimeOfComp", secctionTime);
        saveHandler.AddLevelData("flyPlaneTutorials", tutorialsPlayed);
        //saveHandler.SetLevel();
        GoToNextScene("Evaluation_Scene5");
    }

    public void SaveArrows2(int good, int gGood, int bad, int gBad, int miss, int gMiss, List<float> goodL, List<float> gGoodL, List<float> badL, List<float> gBadL, List<int> interactions, List<float> latencies)
    {
        saveHandler.AddLevelData("flyPlaneCorrect", good);
        saveHandler.AddLevelData("flyPlaneInorrect", bad);
        saveHandler.AddLevelData("flyPlaneMiss", miss);
        saveHandler.AddLevelData("flyPlaneGreenCorrect", gGood);
        saveHandler.AddLevelData("flyPlaneGreenInorrect", gBad);
        saveHandler.AddLevelData("flyPlaneGreenMiss", gMiss);
        saveHandler.AddLevelData("flyPlanesLatencies", goodL);
        saveHandler.AddLevelData("flyPlanesBadLatencies", badL);
        saveHandler.AddLevelData("flyPlanesGreenLatencie", gGoodL);
        saveHandler.AddLevelData("flyPlanesGreenBadLatencies", gBadL);
        saveHandler.AddLevelData("flyPlanesInteractions", interactions);
        saveHandler.AddLevelData("flyPlanesInteractionLatencies", latencies);
        saveHandler.AddLevelData("flyPlaneTimeOfComp", secctionTime);
        saveHandler.AddLevelData("flyPlaneTutorials", tutorialsPlayed);
        //saveHandler.SetLevel();
        saveHandler.SaveFlashProbes();
        GoToNextScene("DemoEvaluationLoader");
    }

    //This function check the level of dificulty of the test according to the input of the age
    public int DifficultyLevel()
    {    
        if (ageOfTheCurrentPlayer < 7)
        {
            return 0;
        }
        else if (ageOfTheCurrentPlayer >= 7 && ageOfTheCurrentPlayer < 10) 
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }

    //this will return the age of the kid
    public int GetTheAgeOfPlayer()
    {
        return ageOfTheCurrentPlayer;
    }

    public void SetButtonText(Button button, string stringToText)
    {
        button.GetComponentInChildren<Text>().text = stringToText;
    }

    //this will send to the next need scene
    void GoToNextScene(string scene)
    {
        PrefsKeys.SetNextScene(scene);
        SceneManager.LoadScene("Loader_Scene");
    }

    int IntDivider(int num1, int num2)
    {
        if (num2 != 0)
        {
            return (num1 / num2);
        }
        else
        {
            return 0;
        }
    }

    float FloatDivider(float num1, float num2)
    {
        if (num2 != 0)
        {
            return (num1 / num2);
        }
        else
        {
            return 0;
        }
    }

    float FloatDivider(float num1, int num2)
    {
        if (num2 != 0)
        {
            return (num1 / num2);
        }
        else
        {
            return 0;
        }
    }
}
