using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinGameManager : MonoBehaviour {

    #region Varaibles

    #region Variables used for the game

    #region Scripts
    //this script is the one that controlls all the evaluation for more info go to that particullar script
    EvaluationController evaluationController;
    //this one is used in the control of all the audio in the evaluation for more info refer to the script
    AudioManager audioManager;
    AudioClip[] audioInScene;
    #endregion

    #region Numbers
    //this one is the central time of the test
    float oneMinute = 60f;
    //this is the total time of the test
    float timeToEnd = 150f;
    //this is the time after pausing the game the menu comeback to the test
    float timeToReturnToCoins = 3.5f;
    //this will keep track of how much times it tooks to take another coin
    float latencieOfGrabing = 0f;
    #endregion

    #region Boolean
    //this will determine when is time to grab the coins
    bool playTime;
    //this is used to determine if it has benn a minute or more
    bool afterTime;
    //this is used to pause the time
    bool pauseTime;
    #endregion

    #region Layermasks
    public LayerMask mask;
    #endregion

    #region UI Elements
    public GameObject instructionPanel;
    public GameObject coinsSample;
    public GameObject coinsSample2;
    public Text instructionText;
    public Button readyButton;
    #endregion

    #region Game Objects
    //this is the object that will be selected and process
    GameObject selectedObject;
    //this is the manager of all coins it will appear and disapern in pause time
    public GameObject coinsManager;
    public GameObject coinsManagerEasy;
    //this is the cam used to tell the story
    public GameObject storyCam;
    //this is the cam used to tell the story
    public GameObject coinCam;
    public GameObject storyManager;
    #endregion

    #region List
    //this will keep track of every good coin selected to punctuate a score
    List<CoinOrder> coins = new List<CoinOrder>();
    #endregion


    #region Text
    [Header("Text Element")]
    // Assset used for the text
    public TextAsset textAsset;
    //string array that saves the strings of the game
    string[] stringsToShow;
    #endregion

    #endregion

    #region Variables used for data recolection

    #region Numbers
    int difficulty;
    int coinsInTime;
    int coinsInTimeBad;
    int coinsTotal;
    int coinsExtra;
    int coinsExtraBad;
    int pausedTimes;
    int goodCoins;

    #endregion

    #region List
    //this will save all the coins that are being selected and in wich order and in wich time is selected
    List<int> coinsSelected = new List<int>();
    List<float> timeToGrabCoin = new List<float>();
    List<float> goodGrabbingLatencie = new List<float>();
    List<float> badGrabbingLatencie = new List<float>();
    #endregion

    #endregion

    #endregion

    // Use this for initialization
    void Start() {
        evaluationController = FindObjectOfType<EvaluationController>();
        audioManager = FindObjectOfType<AudioManager>();
        textAsset = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Evaluation/Evaluation_06/Evaluation_Scene6");
        audioInScene = Resources.LoadAll<AudioClip>($"{LanguagePicker.BasicAudioRoute()}Evaluation/Scene_6");
        audioManager.PlayClip(audioInScene[0]);
        stringsToShow = TextReader.TextsToShow(textAsset);
        storyCam.SetActive(true);
        coinCam.SetActive(false);
        evaluationController.SetButtonText(readyButton, TextReader.commonStrings[0]);
        instructionText.text = stringsToShow[1];
        instructionPanel.SetActive(false);
        playTime = false;
        int age = evaluationController.GetTheAgeOfPlayer();
        if (age < 7)
        {
            difficulty = 0;
            goodCoins = 7;
        }
        else
        {
            difficulty = 1;
            goodCoins = 24;
        }
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
        readyButton.onClick.AddListener(PlayTheInstrcution);
        readyButton.gameObject.SetActive(false);
    }


    void PlayTheInstrcution()
    {
        storyCam.SetActive(false);
        coinCam.SetActive(true);
        instructionPanel.SetActive(true);
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(PlayTheSecondInstruction);
        readyButton.gameObject.SetActive(false);
        evaluationController.StarCounting();
        coinsManager.SetActive(false);
        coinsManagerEasy.SetActive(false);
        audioManager.PlayClip(audioInScene[1]);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
    }


    void PlayTheSecondInstruction()
    {
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(StartTheTest);
        readyButton.gameObject.SetActive(false);
        audioManager.PlayClip(audioInScene[2]);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
        instructionText.text = stringsToShow[2];
    }
	// Update is called once per frame
	void Update () {
        if (playTime)
        {
            latencieOfGrabing += Time.deltaTime;
            if (oneMinute > 0)
            {
                oneMinute -= Time.deltaTime;
            }
            else
            {
                if (!afterTime)
                {
                    afterTime = true;
                    SaveInTimeValues();
                }
            }
            if (timeToEnd > 0)
            {
                timeToEnd -= Time.deltaTime;
            }
            else
            {
                NextScene();
            }
            SelectCoins();
        }
        else if(!playTime && pauseTime)
        {
            timeToReturnToCoins -= Time.deltaTime;
            if (timeToReturnToCoins < 0) {
                PauseOrUnpause();
            }
        }

	}

    #region Functions

    //this will start the test of picking coins
    void StartTheTest()
    {
        playTime = true;
        if (difficulty == 0)
        {
            coinsManagerEasy.SetActive(true);
        }
        else
        {
            coinsManager.SetActive(true);
        }
        coinsSample.SetActive(true);
        coinsSample2.SetActive(false);
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(PauseOrUnpause);
        instructionPanel.SetActive(false);
        evaluationController.SetButtonText(readyButton, TextReader.commonStrings[2]);
    }

    //thi will save the coins while there are in time
    void SaveInTimeValues()
    {
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(NextScene);
    }

    //this will select the coins
    void SelectCoins()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            {
                selectedObject = hit.collider.gameObject;
            }
            if (selectedObject != null)
            {
                int coinNumber = int.Parse(selectedObject.name.Remove(0, 4));
                coinsSelected.Add(coinNumber);
                timeToGrabCoin.Add(latencieOfGrabing);
                latencieOfGrabing = 0;
                HandleCoins();
            }

        }
    }

    //this will handle what are the data with the selected coin
    void HandleCoins() {
        if (!afterTime)
        {
            if (selectedObject.tag == "Finish")
            {
                goodGrabbingLatencie.Add(latencieOfGrabing);
                coinsInTime++;
                coins.Add(selectedObject.GetComponent<CoinOrder>());
            }
            else
            {
                badGrabbingLatencie.Add(latencieOfGrabing);
                coinsInTimeBad++;
            }
        }
        else
        {
            if (selectedObject.tag == "Finish")
            {
                goodGrabbingLatencie.Add(latencieOfGrabing);
                coinsExtra++;
            }
            else
            {
                badGrabbingLatencie.Add(latencieOfGrabing);
                coinsExtraBad++;
            }
        }
        selectedObject.SetActive(false);
        selectedObject = null;
    }

    //this will pause the game if it hasnt been a minute and going back if its click again
    void PauseOrUnpause() {
        if (!instructionPanel.activeInHierarchy)
        {
            if (coinsInTime == goodCoins)
            {
                SaveInTimeValues();
            }
            else
            {
                instructionPanel.SetActive(true);
                playTime = false;
                pauseTime = true;
                pausedTimes++;
                audioManager.PlayClip(audioInScene[3]);
                instructionText.text = stringsToShow[3];
                coinsManager.SetActive(false);
                coinsManagerEasy.SetActive(false);
                evaluationController.SetButtonText(readyButton, TextReader.commonStrings[1]);
            }
        }
        else
        {
            instructionPanel.SetActive(false);
            playTime = true;
            pauseTime = false;
            if (difficulty == 0)
            {
                coinsManagerEasy.SetActive(true);
            }
            else
            {
                coinsManager.SetActive(true);
            }
            evaluationController.SetButtonText(readyButton, TextReader.commonStrings[2]);
        }

    }

    //this will send to the next scene
    void NextScene()
    {
        int score = CalculateAScore(coins);
        evaluationController.SaveCoinsProgress(coinsInTime, coinsInTimeBad, coinsExtra, coinsExtraBad,
            score, coinsSelected, timeToGrabCoin, goodGrabbingLatencie, badGrabbingLatencie, difficulty, pausedTimes);
    }

    //Thiw will activate the sound when its needed
    void ReadyButtonOn()
    {
        readyButton.gameObject.SetActive(true);
    }

    //this will calculate the score index
    public int CalculateAScore(List<CoinOrder> coins)
    {
        int score = coins.Count;
        bool conditionA = false;
        bool conditionB = false;
        bool conditionC = false;
        for (int i = 0; i < coins.Count - 1; i++)
        {
            conditionA = false;
            conditionB = false;
            conditionC = false;
            if (coins[i].order[0] + 1 == coins[i + 1].order[0])
            {
                score++;
                conditionA = true;
            }
            else if (coins[i].order[1] + 1 == coins[i + 1].order[1] && !conditionA)
            {
                score++;
                conditionB = true;
            }
            else if (coins[i].order[2] + 1 == coins[i + 1].order[2] && !conditionB && !conditionA)
            {
                score++;
                conditionC = true;
            }
            else if (coins[i].order[3] + 1 == coins[i + 1].order[3] && !conditionC && !conditionB && !conditionA)
            {
                score++;
            }
        }

        for (int i = 0; i < coins.Count - 2; i++)
        {
            conditionA = false;
            conditionB = false;
            conditionC = false;
            if (coins[i].order[0] + 2 == coins[i + 2].order[0])
            {
                score++;
                conditionA = true;
            }
            else if (coins[i].order[1] + 2 == coins[i + 2].order[1] && !conditionA)
            {
                score++;
                conditionB = true;
            }
            else if (coins[i].order[2] + 2 == coins[i + 2].order[2] && !conditionB && !conditionA)
            {
                score++;
                conditionC = true;
            }
            else if (coins[i].order[3] + 2 == coins[i + 2].order[3] && !conditionC && !conditionB && !conditionA)
            {
                score++;
            }
        }

        return score;
    }

    #endregion
}
