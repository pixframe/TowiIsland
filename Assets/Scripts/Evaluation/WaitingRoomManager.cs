using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingRoomManager : MonoBehaviour {
    #region Variables

    #region Variables used for the game

    #region Scripts
    //this script is the one that controlls all the evaluation for more info go to that particullar script
    EvaluationController evaluationController;
    //this one is used in the control of all the audio in the evaluation for more info refer to the script
    AudioManager audioManager;
    //this will make a flash probe
    FlashProbes flashProbes;

    public AudioClip[] flightSounds;
    AudioClip[] audioInScene;
    AudioClip tryAudio;
    AudioClip tryAgainAudio;
    AudioClip veryGoodAudio;
    AudioClip cautionAudio;
    AudioClip upsAudio;
    #endregion

    #region Booleans

    //this will determine when its or when its not the tutorial
    bool tutorialMode;
    //this will turn on when the test begin
    bool gameMode;
    //this will strat counting the clicks to aviod extra clicks and badclicks
    bool clickTime;
    //this will see if the tutorial has benn wrong made
    bool failTheTutorial;
    //this will check if we already click
    bool clicked;
    //this will check if its the first click in the audio time
    bool firstClick;
    //this will tell that the feed back should start to hide
    bool hideTheFeed;
    //this will coun any extra click
    bool extraClick;
    //this will change the color of the arrows
    bool changeColorOfArrows;

    #endregion

    #region Numbers
    //single ints
    //this will keep a track of the current audio in the test part
    int flightNumberIndex;
    //this will keep a track of the current audio in the tutorial part
    int tutorialIndex;
    //this is the total amout of audio that are play
    int totalNumberOfFlights;
    //this will switch the arrows colors
    float timeToSwitchArrows = 0.3f;
    //This will set the latencies acording to the stimulus;
    float latencieTime = 0f;


    //these list are needed and cointaind diferent things to develop good performance of the test
    //this list includes the numbers of the audios that are shown in the tutorial
    List<int> tutorialClips = new List<int> {
        30,
        31,
        32
    };
    //this will handle what are the correct answers
    List<int> correctAnswers = new List<int> {
        8,
        14,
        21,
        25,
        31,
        39,
        44,
        48,
        54,
        56,
        58
    };
    //this will keep track of the time of good latencies
    List<float> goodLatencies = new List<float>();
    //this will keep track of the bad latencies
    List<float> badLatencies = new List<float>();
    //this will keep track of every interaction that has been made
    List<int> interactionRecord = new List<int>();
    List<float> interactionLatencies = new List<float>();
    #endregion

    #region Enum

    //This enums will control the posible errors in the tutorial
    enum TutorialErrors { None, Miss, NotYet, NotDef };
    //this is the enum that controls the error
    TutorialErrors flightError;

    #endregion

    #region UI
    //UI elemts needed for this game
    public GameObject instruccionPanel;
    public Button readyButton;
    public Text textInstruccions;
    public Text storyText;
    public GameObject airportView;
    #endregion

    #region layerMasks
    //this one will determine if the click object its or not a screen
    public LayerMask screenLayers;
    #endregion

    #region FeedBack
    //this is the image is show to gives sort of feedback
    public GameObject feedbackImage;
    //this is the sprite renderer of the feedback
    SpriteRenderer rendi;
    //this color is used to change the alpha component of the color in the feedback image
    Color farben;
    #endregion

    #region ScreenShower

    //this will have the arrows
    public GameObject arrowManager;
    //this will be the color one of arrows
    public Color arrowColorA;
    //this will be the second color of the arrows
    public Color arrowColorB;
    //this will be the arrows
    SpriteRenderer[] arrows;

    #endregion

    #region Text
    [Header("Text Elements")]
    // Assset used for the text of the game
    public TextAsset textAssetForGameText;
    //string array that saves the strings of the game
    string[] stringsToShow;
    #endregion

    #region Lists

    List<int> orderOfStimulus = new List<int>
    {
        0,
        45,
        3,
        9,
        7,
        6,
        5,
        44,
        33,
        18,
        17,
        37,
        1,
        20,
        31,
        19,
        23,
        25,
        44,
        22,
        29,
        32,
        4,
        43,
        2,
        30,
        8,
        15,
        10,
        17,
        16,
        34,
        14,
        29,
        42,
        21,
        38,
        13,
        5,
        35,
        42,
        12,
        7,
        9,
        30,
        11,
        39,
        26,
        6,
        25,
        24,
        40,
        27,
        41,
        36,
        28,
        34,
        37,
        36,
        0
    };

    #endregion

    #endregion

    #region Variables used in the data record

    #region Numbers
    //this is used when the right answers are delivered
    int goodAnswer;
    //this are used when the answer is good and is click in the screen
    int goodAnswerInScreen;
    //this is used if its click in a bad moment
    int badAnswer;
    //this is used if its click in a bad moment but in the screen
    int badAnswerInScreen;
    //this is used if theres another click in the right moment
    int extraGoodClick;
    //this is click if its a bad extra click
    int extraBadClick;
    #endregion

    #endregion

    #endregion

    // Use this for initialization
    void Start () {
        evaluationController = FindObjectOfType<EvaluationController>();
        audioManager = FindObjectOfType<AudioManager>();
        flashProbes = FindObjectOfType<FlashProbes>();
        rendi = feedbackImage.GetComponent<SpriteRenderer>();
        textAssetForGameText = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Evaluation/Evaluation_04/Evaluation_Scene4");
        stringsToShow = TextReader.TextsToShow(textAssetForGameText);
        evaluationController.StarCounting();
        flightSounds = Resources.LoadAll<AudioClip>(($"{LanguagePicker.BasicAudioRoute()}Evaluation/Stimulus/Flights"));
        audioInScene = Resources.LoadAll<AudioClip>($"{LanguagePicker.BasicAudioRoute()}Evaluation/Scene_4");
        tryAudio = Resources.Load<AudioClip>($"{LanguagePicker.BasicAudioRoute()}Evaluation/Phrases/phrases_00");
        tryAgainAudio = Resources.Load<AudioClip>($"{LanguagePicker.BasicAudioRoute()}Evaluation/Phrases/phrases_01");
        veryGoodAudio = Resources.Load<AudioClip>($"{LanguagePicker.BasicAudioRoute()}Evaluation/Phrases/phrases_05");
        upsAudio = Resources.Load<AudioClip>($"{LanguagePicker.BasicAudioRoute()}Evaluation/Phrases/phrases_03");
        cautionAudio = Resources.Load<AudioClip>($"{LanguagePicker.BasicAudioRoute()}Evaluation/Phrases/phrases_06");
        Cursor.visible = true;
        failTheTutorial = false;
        firstClick = true;
        changeColorOfArrows = true;
        flightError = TutorialErrors.NotDef;
        totalNumberOfFlights = flightSounds.Length;
        if (flashProbes != null)
        {
            totalNumberOfFlights -= 12;
        }
        Debug.Log(totalNumberOfFlights);
        storyText.text = stringsToShow[0];
        audioManager.PlayClip(audioInScene[0]);
        Invoke("PlayTheTutorial", audioManager.ClipDuration() + 2f);
        extraClick = false;
        arrows = new SpriteRenderer[arrowManager.transform.childCount];
        for (int i = 0; i < arrowManager.transform.childCount; i++)
        {
            arrows[i] = arrowManager.transform.GetChild(i).GetComponent<SpriteRenderer>();
        }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
	
	// Update is called once per frame
	void Update () {
        latencieTime += Time.deltaTime;
        if (Input.GetMouseButtonDown(0)) {
            if (!tutorialMode) {
                interactionRecord.Add(flightNumberIndex);
                interactionLatencies.Add(latencieTime);
            }
            clicked = true;
            if (clickTime)
            {
                if (tutorialMode)
                {
                    RaycastHit2D hit = IsOnTheScreen();
                    if (hit)
                    {
                        flightError = TutorialErrors.None;
                        ShowFeedback();
                    }
                    else
                    {
                        flightError = TutorialErrors.None;
                        ShowFeedback();
                    }
                }
                else
                {
                    if (extraClick)
                    {
                        extraGoodClick++;
                        ShowFeedback();
                    }
                    else {
                        RaycastHit2D hit = IsOnTheScreen();
                        goodLatencies.Add(latencieTime);
                        if (hit)
                        {
                            goodAnswer++;
                            goodAnswerInScreen++;
                            ShowFeedback();
                        }
                        else
                        {
                            goodAnswer++;
                            ShowFeedback();
                        }
                        extraClick = true;
                    }
                }
            }
            else
            {
                if (tutorialMode)
                {
                    if (!firstClick) {
                        clicked = true;
                        ErrorChecker();
                        ErrorHandler();
                    }
                }
                else
                {
                    if (!firstClick) {
                        if (extraClick)
                        {
                            extraBadClick++;
                            ShowFeedback();
                        }
                        else
                        {
                            RaycastHit2D hit = IsOnTheScreen();
                            badLatencies.Add(latencieTime);
                            if (hit)
                            {
                                badAnswer++;
                                badAnswerInScreen++;
                                extraClick = true;
                                ShowFeedback();
                            }
                            else
                            {
                                badAnswer++;
                                extraClick = true;
                                ShowFeedback();
                            }

                        }
                    }
                }

            }
        }
        if (hideTheFeed) {
            HideFeedback();
        }
        if (changeColorOfArrows) {
            timeToSwitchArrows -= Time.deltaTime;
            if (timeToSwitchArrows <= 0) {
                SwitchColorOfArrows();
            }
        }
        
	}

    #region Functions

    #region Game Set Ups

    //This is the tutorial start
    void PlayTheTutorial()
    {
        airportView.SetActive(false);
        storyText.transform.parent.gameObject.SetActive(false);
        tutorialMode = true;
        tutorialIndex = 0;
        textInstruccions.text = TextReader.AddStrings(0, stringsToShow[1]);
        audioManager.PlayClip(audioInScene[1], tryAudio);
        evaluationController.SetButtonText(readyButton, TextReader.commonStrings[0]);
        readyButton.onClick.AddListener(PlayTutorialFlights);
        readyButton.gameObject.SetActive(false);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
    }

    //This is the game start
    void PlayTheGame()
    {
        tutorialMode = false;
        firstClick = true;
        instruccionPanel.SetActive(true);
        readyButton.gameObject.SetActive(false);
        audioManager.PlayClip(veryGoodAudio, audioInScene[4]);
        textInstruccions.text = TextReader.AddBeforeStrings(2, stringsToShow[4]);
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(PlayTheGameFlights);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
    }

    //This Reset the tutorial 
    void RestartTutorial()
    {
        failTheTutorial = false;
        flightError = TutorialErrors.NotDef;
        PlayTutorialFlights();
    }

    //This will play the flight sounds and manage the moment in which the click has to be done
    void PlayTutorialFlights()
    {
        firstClick = false;
        clicked = false;
        extraClick = false;
        changeColorOfArrows = false;
        arrowManager.SetActive(false);
        readyButton.gameObject.SetActive(false);
        if (tutorialIndex >= 2)
        {
            ErrorChecker();
            ErrorHandler();
        }
        if (!failTheTutorial)
        {
            instruccionPanel.SetActive(false);
            audioManager.PlayClip(flightSounds[orderOfStimulus[tutorialClips[tutorialIndex]]]);
            if (tutorialIndex == 1)
            {
                clickTime = true;
            }
            else
            {
                clickTime = false;
            }
            if (tutorialIndex >= 2)
            {
                Invoke("PlayTheGame", audioManager.ClipDuration());
            }
            else
            {
                Invoke("PlayTutorialFlights", audioManager.ClipDuration() + 0.7f);
            }
            tutorialIndex++;
        }
    }

    //This is the game part that play the audios and handle the answers
    void PlayTheGameFlights()
    {
        firstClick = false;
        instruccionPanel.SetActive(false);
        latencieTime = 0;
        audioManager.PlayClip(flightSounds[orderOfStimulus[flightNumberIndex]]);
        readyButton.gameObject.SetActive(false);
        tutorialMode = false;
        clickTime = false;
        extraClick = false;
        if (correctAnswers.Contains(flightNumberIndex))
        {
            clickTime = true;
        }
        flightNumberIndex++;
        if (flightNumberIndex >= totalNumberOfFlights)
        {
            Invoke("FinishTheSecction", audioManager.ClipDuration());
        }
        else
        {
            Invoke("PlayTheGameFlights", audioManager.ClipDuration() + 0.7f);
        }

    }

    //This script is used to show an error in the tutorial
    void ShowError()
    {
        instruccionPanel.SetActive(true);
        CancelInvoke();
        tutorialIndex = 0;
        failTheTutorial = true;
        firstClick = true;
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(RestartTutorial);
    }

    //This will show an error when you click in other time that is not in the right moment
    void ShowNotYetError()
    {
        ShowError();
        string firstString = TextReader.AddStrings(1, stringsToShow[2]);
        textInstruccions.text = TextReader.AddBeforeStrings(0, firstString);
        audioManager.PlayClip(upsAudio, audioInScene[2], tryAgainAudio);
        Invoke("ActivateReadyButton", audioManager.ClipDuration());     
    }

    //This will show an error when you dont click in KW flight
    void ShowMissError()
    {
        ShowError();
        string firstString = TextReader.AddStrings(1, stringsToShow[3]);
        textInstruccions.text = TextReader.AddBeforeStrings(3, firstString);
        audioManager.PlayClip(cautionAudio, audioInScene[3], tryAgainAudio);
        Invoke("ActivateReadyButton", audioManager.ClipDuration());
    }

    //This is the last part of the section
    void FinishTheSecction()
    {
        evaluationController.SaveWaitingRoomProgress(goodAnswer, goodAnswerInScreen, extraGoodClick, goodLatencies.ToArray(),
            badAnswer, badAnswerInScreen, extraBadClick, badLatencies.ToArray(),
            interactionRecord.ToArray(), interactionLatencies.ToArray());
    }

    //This will turn on the ready button on 
    void ReadyButtonOn()
    {
        readyButton.gameObject.SetActive(true);
    }

    //This will switch the color of the arrows
    void SwitchColorOfArrows() {
        timeToSwitchArrows = 0.3f;
        if (arrows[0].color == arrowColorA)
        {
            for (int i = 0; i < arrows.Length; i++)
            {
                arrows[i].color = arrowColorB;
            }
        }
        else
        {
            for (int i = 0; i < arrows.Length; i++)
            {
                arrows[i].color = arrowColorA;
            }
        }
    }
    #endregion

    #region Game interactions
    //This one is the script that check every posible error every time
    void ErrorChecker() {
        if (tutorialIndex >= 2 && flightError == TutorialErrors.NotDef)
        {
            flightError = TutorialErrors.Miss;
        }
        else if (clicked)
        {
            flightError = TutorialErrors.NotYet;
        }
    }

    //This will handle the error
    void ErrorHandler() {
        evaluationController.TutorialPlay();
        switch (flightError) {
            case TutorialErrors.Miss:
                ShowMissError();
                break;
            case TutorialErrors.NotYet:
                ShowNotYetError();
                break;
            default:
                break;
        }
    }

    //This will chechk if the click was on the screens
    RaycastHit2D IsOnTheScreen() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics2D.GetRayIntersection(ray, Mathf.Infinity, screenLayers);
    }

    //this will show the feedback
    void ShowFeedback() {
        feedbackImage.SetActive(true);
        feedbackImage.transform.localScale = Vector3.one;
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        feedbackImage.transform.position = new Vector3(pos.x, pos.y, feedbackImage.transform.position.z);
        farben = rendi.color;
        farben.a = 1;
        rendi.color = farben;
        hideTheFeed = true;
    }

    //This will hide the feedBack
    void HideFeedback() {
        farben.a -= 1.2f * Time.deltaTime;
        feedbackImage.transform.localScale = new Vector3(feedbackImage.transform.localScale.x + 0.2f * Time.deltaTime, feedbackImage.transform.localScale.y + 0.2f * Time.deltaTime, feedbackImage.transform.localScale.z + 0.2f * Time.deltaTime);
        rendi.color = farben;
        if (farben.a <= 0)
        {
            hideTheFeed = false;
            feedbackImage.SetActive(false);
        }
    }


    void ActivateReadyButton()
    {
        readyButton.gameObject.SetActive(true);
    }
    #endregion

    #endregion
}
