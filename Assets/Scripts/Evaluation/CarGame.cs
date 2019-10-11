using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarGame : MonoBehaviour {

    #region Varaiables

    #region Variables used in the game

    #region Scripts
    //this script is the one that controlls all the evaluation for more info go to that particullar script
    EvaluationController evaluationController;
    //this one is used in the control of all the audio in the evaluation for more info refer to the script
    AudioManager audioManager;
    AudioClip[] audioInScene;
    FlashProbes prober;
    #endregion

    #region Game Objects
    //Single Game Objects
    //this is the manager of the laberynths
    public GameObject laberynthManager;
    //this is the manager of the cars
    public GameObject carManager;
    //this is the manager of thjje exits
    public GameObject exitManager;

    //Arrays of Game Objects
    //in this will store all the available laberynts
    GameObject[] laberynths;
    //in this will store all the available cars
    GameObject[] cars;
    //in this will be store all the available exits
    GameObject[] exits;
    //this will transform the arrows to show where the exit is
    SpriteRenderer[] arrows;
    #endregion

    #region UI 
    [Header("UI Elements")]
    public GameObject instruccionPanel;
    public Button readyButton;
    public TextMeshProUGUI instructionText;
    #endregion

    #region Ornaments Data
    [Header("Colors of Arrow")]
    public Color blue1;
    public Color blue2;
    #endregion

    #region Numbers
    //this handle the difficulty for the laberynts
    int difficulty;
    //this index will keep track of what laberynt we are in
    int laberynthNumber;
    //this number is the one that determines the numbers of laberynts to been play
    int numberOfLaberyntsToPlay = 3;
    //this is the index of the laberynts
    int laberyntIndex;
    //This are the laberynts numbers accordinf to each kid
    List<int> easyLaberynts = new List<int>() { 1, 3, 2 };
    List<int> difficultLaberynts = new List<int>() { 3, 4, 5 };

    float arrowTime = 0.3f;
    float arrowTimer = 0.3f;
    #endregion

    #region Booleans
    //this will determine when we count latencies
    bool latencyMode;
    //this will say when its time to solving the problems
    bool solvingMode;
    //this will set when the game strats
    bool gameStart;
    #endregion

    #region Text
    [Header("Text Element")]
    // Assset used for the text
    public TextAsset textAsset;
    //string array that saves the strings of the game
    string[] stringsToShow;
    #endregion

    #endregion

    #region Variables used in the data record

    #region Numbers
    //Arrays of data, this lenght is the same that the amount of laberynths to play
    //this will keep track of the time spend in the resolution of the laberynth
    float[] times;
    //this will keep track of the latencie to strat the laberynth
    float[] latencies;
    //this will keep count of the hits by laberynt
    int[] hits;
    //this will keep count of the crosses by laberynt
    int[] crosses;
    //this will keep count of the change of routes by laberynth
    int[] changeOfRoutes; //this is comment by the moment because we havent define yet the functionallity of this particullary stuff
    //this will keep count of the deadEnds by laberynt
    int[] deadEnds;
    //those are the textures send to the dataBase
    Texture2D[] texturesToSend;
    #endregion

    #endregion

    #endregion

    // Use this for initialization
    void Start ()
    {
        evaluationController = FindObjectOfType<EvaluationController>();
        audioManager = FindObjectOfType<AudioManager>();
        audioInScene = Resources.LoadAll<AudioClip>($"{LanguagePicker.BasicAudioRoute()}Evaluation/Scene_3");
        textAsset = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Evaluation/Evaluation_03/Evaluation_Scene3");
        stringsToShow = TextReader.TextsToShow(textAsset);
        evaluationController.StarCounting();
        Initialization();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStart)
        {
            if (latencyMode)
            {
                latencies[laberyntIndex] += Time.deltaTime;
                SwitchArrowColor();
            }
            if (solvingMode)
            {
                times[laberyntIndex] += Time.deltaTime;
            }
        }
    }

    #region Functions

    #region Set Ups
    //this scripts will change the values of the game objects to play the game
    //This Method Provides all the needed things we have to initialize for the sript to work properly
    void Initialization()
    {
        prober = FindObjectOfType<FlashProbes>();
        if (prober != null)
        {
            difficulty = 2;
            numberOfLaberyntsToPlay = 6;
            laberynthNumber = 0;
        }
        else
        {
            int age = evaluationController.GetTheAgeOfPlayer();
            if (age < 7)
            {
                difficulty = 0;
            }
            else
            {
                difficulty = 1;
            }
            if (difficulty == 0)
            {
                laberynthNumber = easyLaberynts[laberyntIndex];
            }
            else
            {
                laberynthNumber = difficultLaberynts[laberyntIndex];
            }
        }

        laberynths = new GameObject[laberynthManager.transform.childCount];
        cars = new GameObject[carManager.transform.childCount];
        exits = new GameObject[exitManager.transform.childCount];
        arrows = new SpriteRenderer[exitManager.transform.childCount];
        texturesToSend = new Texture2D[numberOfLaberyntsToPlay];
        for (int i = 0; i < laberynthManager.transform.childCount; i++)
        {
            laberynths[i] = laberynthManager.transform.GetChild(i).gameObject;
        }
        for (int i = 0; i < carManager.transform.childCount; i++)
        {
            cars[i] = carManager.transform.GetChild(i).gameObject;
        }
        for (int i = 0; i < exitManager.transform.childCount; i++)
        {
            exits[i] = exitManager.transform.GetChild(i).gameObject;
            arrows[i] = exits[i].transform.GetChild(0).GetComponent<SpriteRenderer>();
        }
        times = new float[numberOfLaberyntsToPlay];
        latencies = new float[numberOfLaberyntsToPlay];
        hits = new int[numberOfLaberyntsToPlay];
        crosses = new int[numberOfLaberyntsToPlay];
        changeOfRoutes = new int[numberOfLaberyntsToPlay];
        deadEnds = new int[numberOfLaberyntsToPlay];
        latencyMode = false;
        solvingMode = false;
        readyButton.gameObject.SetActive(false);

        string s = stringsToShow[0];
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            s = stringsToShow[0] + stringsToShow[2] + stringsToShow[3];
            audioManager.PlayClip(audioInScene[0] ,audioInScene[2] , audioInScene[3]);
        }
        else if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            s = stringsToShow[0] + stringsToShow[1] + stringsToShow[3];
            audioManager.PlayClip(audioInScene[0], audioInScene[2], audioInScene[3]);
        }
        instructionText.text = s;

        evaluationController.SetButtonText(readyButton, TextReader.commonStrings[0]);
        readyButton.onClick.AddListener(StartNewLaberynth);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
    }

    //this method is call every time a new laberynt is going to be play
    void StartNewLaberynth (){
        instruccionPanel.SetActive(false);
        solvingMode = false;
        latencyMode = true;
        gameStart = true;

        laberynths[laberynthNumber].SetActive(true);
        cars[laberynthNumber].SetActive(true);
        exits[laberynthNumber].SetActive(true);
        arrows[laberynthNumber].gameObject.SetActive(true);
        Cursor.visible = true;

        if (laberyntIndex >= 1)
        {
            int lastLaberyntNumber;
            if (difficulty == 2)
            {
                lastLaberyntNumber = laberyntIndex - 1;
            }
            else if (difficulty == 0)
            {
                lastLaberyntNumber = easyLaberynts[laberyntIndex - 1];
            }
            else
            {
                lastLaberyntNumber = difficultLaberynts[laberyntIndex - 1];
            }
            laberynths[lastLaberyntNumber].SetActive(false);
            cars[lastLaberyntNumber].SetActive(false);
            exits[lastLaberyntNumber].SetActive(false);
        }
    }

    //this methos is call only one time after all the laberynths are solved
    void FinishTheScene()
    {
        if (difficulty == 2)
        {
            evaluationController.SaveFlashCarData(prober.GetName(), prober.GetAge(), hits, latencies, times);
        }
        else
        {
            evaluationController.SaveCarLaberynthProgress(latencies, times, hits, crosses, changeOfRoutes, deadEnds, texturesToSend, difficulty);
        }
    }

    //this will show when the instruction is over
    void ReadyButtonOn() {
        readyButton.gameObject.SetActive(true);
    }
    #endregion

    #region Needed by the car 
    //this scripts are used by the CarController to perform certain data collection to have more info look that script
    //This methos is calleverytime the car is moved for the first time
    public void StartSolving()
    {
        solvingMode = true;
        latencyMode = false;
        arrows[laberynthNumber].gameObject.SetActive(false);
    }

    //this method is call every time th car get into the finish line
    public void FinishLaberynth()
    {
        solvingMode = false;
        latencyMode = false;
        CarController carContrller = cars[laberynthNumber].GetComponent<CarController>();
        hits[laberyntIndex] = carContrller.hits;
        crosses[laberyntIndex] = carContrller.crosses;
        changeOfRoutes[laberyntIndex] = carContrller.changesOfRoutes;
        deadEnds[laberyntIndex] = carContrller.deadEnds;
        if (difficulty == 2)
        {
            ScreenCapture.CaptureScreenshot("lab_" + laberynthNumber.ToString() + "_try_" + Keys.Flash_Probe_Num);
        }
        texturesToSend[laberyntIndex] = ScreenCapture.CaptureScreenshotAsTexture();
        laberyntIndex++;
        if (laberyntIndex < numberOfLaberyntsToPlay)
        {
            if (difficulty == 2)
            {
                laberynthNumber = laberyntIndex;
            }
            else if (difficulty == 0)
            {
                laberynthNumber = easyLaberynts[laberyntIndex];
            }
            else
            {
                laberynthNumber = difficultLaberynts[laberyntIndex];
            }
            StartNewLaberynth();
        }
        else
        {
            FinishTheScene();
        }
    }

    void SwitchArrowColor()
    {
        arrowTimer -= Time.deltaTime;
        if (arrowTimer <= 0) {
            if (arrows[laberynthNumber].color == blue1)
            {
                arrows[laberynthNumber].color = blue2;
            }
            else
            {
                arrows[laberynthNumber].color = blue1;
            }
            arrowTimer = arrowTime;
        }


    }
    #endregion

    #endregion
}
