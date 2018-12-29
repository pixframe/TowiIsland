using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicRiverManager : MonoBehaviour {

    [Header("UI Elements")]
    public GameObject instructionPanel;
    public Text instructionText;
    public Button readyButton;
    public GameObject[] instructionScreens;

    [Header("Parameters")]
    public LayerMask classifierMask;

    [Header("Stimulus")]
    public GameObject[] forestObjets;
    public GameObject[] beachObjects;

    [Header("Text Assets")]
    TextAsset textAsset;
    TextAsset beachTextAsset;
    TextAsset forestTextAsset;
    string[] stringsToShow;
    string[] beachStrings;
    string[] forestStrings;

    Transform startArea;
    Transform showerSpot;
    BoxCollider positionToDrop;
    Transform collector;
    PauseTheGame pauser;
    SessionManager sessionManager;
    AudioManager audioManager;
    LevelSaver levelSaver;

    AudioClip[] instructionsClips;
    AudioClip[] stimulusForestClips;
    AudioClip[] stimulusBeachClips;

    GameObject objectToDrop;
    GameObject selectedObject;

    enum Actions {LetItGo, Reverse };
    enum TypeOfSpecial {SpecificObject,AfterObject, IfObject };

    List<Actions> specialAction = new List<Actions>();
    List<TypeOfSpecial> specialType = new List<TypeOfSpecial>();
    List<GameObject> finalForestStimulus = new List<GameObject>();
    List<GameObject> finalBeachStimulus = new List<GameObject>();
    List<GameObject> specialStimulus = new List<GameObject>();
    List<GameObject> specialStuff = new List<GameObject>();
    List<string> forestFinalStrings = new List<string>();
    List<string> beachFinalStrings = new List<string>();
    List<string> finalStimulusStrings = new List<string>();
    List<int> numbersForest = new List<int>();
    List<int> numbersBeach = new List<int>();
    List<int> playedLevels = new List<int>();
    List<int> playedDifficulty = new List<int>();
    List<AudioClip> audiosOfStimulus = new List<AudioClip>();

    float speedOfSpawning;
    float speedOfSwiming = 8f;
    float lastKnowPosition;
    float timeToDrop = 3f;
    float dropSpeed;
    float timeToSwim = 8f;
    float time;

    int level;
    int difficulty;

    int badAnswer;
    int specificBadAnswer;
    int correctAnswer;
    int specificGoodAnswer;
    int missAnswer;
    int specificMissAnswer;

    int instructionIndex;
    int objectsToDrop = 15;
    const int numberOfObjectToDrop = 15;
    int specialObjects;
    int maxTimeToShowSpecial;
    int timeToShowSpecial;
    int direction;
    int direction1;
    int direction2;
    int numberOfAssays = 5;
    const int totalNumberOfAssays = 5;
    int handleStimulus;
    int specialInstructionIndex;
    int whenToDrop1;
    int whenToDrop2;
    int dropperTime = 0;
    int firstTime;
    int totalSpecials;
    int totalReverse;
    int totalCorrects;
    int totalIncorrects;
    int totalCorrectTargets;
    int totalIncorrectTargets;
    int passLevels;
    int repeatedLevels;
    //Those will set the type of special object 0 SwitchPlaces, object 1 LetItGo
    int typeOfSpecials;
    int totalLevels = 36;
    int levelCategorizer;
    int miniKidLevel = 9;
    int maxiKidLevel = 27;
    int totalTargets;
    static int startingStimulustListPoint = 5;

    bool reverseMode;
    bool letItGoMode;
    bool dragMode;
    bool dropingTime;
    bool showTutorial;
    bool counting;

	// Use this for initialization
	void Start ()
    {
        if (FindObjectOfType<SessionManager>())
        {
            sessionManager = FindObjectOfType<SessionManager>();
            levelSaver = GetComponent<LevelSaver>();
        }
        audioManager = FindObjectOfType<AudioManager>();
        textAsset = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Games/River/RiverText");
        beachTextAsset = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Games/River/BeachObjectsTextAsset");
        forestTextAsset = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Games/River/ForestObjectsTextAsset");
        instructionsClips = Resources.LoadAll<AudioClip>($"{LanguagePicker.BasicAudioRoute()}Games/River");
        GetLevel();
        stringsToShow = TextReader.TextsToShow(textAsset);
        beachStrings = TextReader.TextsToShow(beachTextAsset);
        forestStrings = TextReader.TextsToShow(forestTextAsset);

        stimulusBeachClips = new AudioClip[beachStrings.Length];
        stimulusForestClips = new AudioClip[forestStrings.Length];
        for (int i = 0; i < stimulusBeachClips.Length; i++)
        {
            stimulusBeachClips[i] = Resources.Load<AudioClip>($"{LanguagePicker.BasicAudioRoute()}Games/Stimulus/g_st_{(i + startingStimulustListPoint).ToString("00")}");
        }
        for (int i = 0; i < stimulusForestClips.Length; i++)
        {
            stimulusForestClips[i] = Resources.Load<AudioClip>($"{LanguagePicker.BasicAudioRoute()}Games/Stimulus/g_st_{(i + startingStimulustListPoint + stimulusBeachClips.Length).ToString("00")}");
        }

        pauser = FindObjectOfType<PauseTheGame>();
        startArea = transform.GetChild(0).transform;
        showerSpot = transform.GetChild(1).transform;
        positionToDrop = transform.GetChild(2).GetComponent<BoxCollider>();
        collector = transform.GetChild(3).transform;
        if (firstTime == 0)
        {
            SetTutorial();
            pauser.HideTutorialButtons();
        }
        else
        {
            instructionPanel.transform.parent.gameObject.SetActive(false);
            pauser.WantTutorial();
            pauser.howToPlayButton.onClick.AddListener(SetTutorial);
            pauser.playButton.onClick.AddListener(SetTheInstruction);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (counting)
        {
            time += Time.deltaTime;
        }
        if (dropingTime)
        {
            timeToDrop -= Time.deltaTime;

            if (timeToDrop <= 0)
            {
                DropTheStimulus();
            }
        }

        if (dragMode)
        {
            DragAnObject();
            DropAThing();
        }
	}

    void SetTutorial()
    {
        showTutorial = true;
        instructionPanel.transform.parent.gameObject.SetActive(true);
        readyButton.onClick.AddListener(TellStory);
        instructionScreens[instructionIndex].SetActive(true);
        instructionText.text = stringsToShow[instructionIndex];
        readyButton.gameObject.SetActive(false);
        audioManager.PlayClip(instructionsClips[instructionIndex]);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
    }

    void GetLevel()
    {
        if (!FindObjectOfType<DemoKey>())
        {
            if (sessionManager != null)
            {
                if (sessionManager.activeKid.riverFirst)
                {
                    FLISSetup();
                }
                else
                {
                    difficulty = sessionManager.activeKid.riverDifficulty;
                    level = sessionManager.activeKid.riverLevel;
                    firstTime = 1;
                }
            }
            else
            {
                difficulty = PlayerPrefs.GetInt(Keys.River_Difficulty);
                level = PlayerPrefs.GetInt(Keys.River_Level);
                firstTime = PlayerPrefs.GetInt(Keys.River_First);
            }
        }
        else
        {
            var key = FindObjectOfType<DemoKey>();
            if (key.IsFLISOn())
            {
                sessionManager.activeKid.riverFirst = true;
                FLISSetup();
            }
            else
            {
                sessionManager.activeKid.riverFirst = false;
                firstTime = 1;
                switch (key.GetDifficulty())
                {
                    case 0:
                        levelCategorizer = 0;
                        break;
                    case 1:
                        levelCategorizer = totalLevels / 2;
                        break;
                    case 2:
                        levelCategorizer = totalLevels - 5;
                        break;
                }
                GetDataJustForLevel(levelCategorizer);
            }
        }

    }

    void FLISSetup()
    {
        if (sessionManager.activeKid.age < 7)
        {
            levelCategorizer = miniKidLevel;
        }
        else if (sessionManager.activeKid.age > 9)
        {
            levelCategorizer = maxiKidLevel;
        }
        else
        {
            levelCategorizer = LevelDifficultyChange(totalLevels);
        }
        GetDataJustForLevel(levelCategorizer);
        firstTime = 0;
    }

    void SaveLevel()
    {
        counting = false;
        if (sessionManager != null)
        {
            sessionManager.activeKid.riverDifficulty = difficulty;
            sessionManager.activeKid.riverLevel = level;
            sessionManager.activeKid.kiwis += passLevels;
            sessionManager.activeKid.playedRiver = 1;
            sessionManager.activeKid.needSync = true;


            levelSaver.AddLevelData("level", difficulty);
            levelSaver.AddLevelData("sublevel", level);
            levelSaver.AddLevelData("time", time);
            levelSaver.AddLevelData("tutorial", 1);
            levelSaver.AddLevelData("reverse", 2);
            levelSaver.AddLevelData("speed", 1);
            levelSaver.AddLevelData("correctobjects", totalCorrects);
            levelSaver.AddLevelData("incorrectobjects", totalIncorrects);
            levelSaver.AddLevelData("levelobjects", "15");
            levelSaver.AddLevelData("availableobjects", "15");
            levelSaver.AddLevelData("neutralobjects", "find");
            levelSaver.AddLevelData("forceforestobjects", "14");
            levelSaver.AddLevelData("forcebeachforest", "10");
            levelSaver.AddLevelData("specialreverseobjects", "totalReverse");
            levelSaver.AddLevelData("specialleaveobjects", "totalSpecials");

            //Version 2
            sessionManager.activeKid.riverSessions++;
            if (sessionManager.activeKid.riverFirst)
            {
                sessionManager.activeKid.riverFirst = false;
                levelSaver.AddLevelData("initial_level", level);
                levelSaver.AddLevelData("initial_difficulty", difficulty);
            }
            levelSaver.AddLevelData("current_level", level);
            levelSaver.AddLevelData("current_difficulty", difficulty);
            levelSaver.AddLevelData("session_correct_total", (totalCorrects * 100) / (numberOfObjectToDrop * totalNumberOfAssays));
            levelSaver.AddLevelData("session_errors_total", (totalIncorrects* 100) / (numberOfObjectToDrop * totalNumberOfAssays));
            levelSaver.AddLevelData("played_levels", playedLevels);
            levelSaver.AddLevelData("played_difficulty", playedDifficulty);
            levelSaver.AddLevelData("target_total", totalTargets);
            levelSaver.AddLevelData("target_correct", totalCorrectTargets);
            levelSaver.AddLevelData("target_errors", totalIncorrectTargets);

            levelSaver.SetLevel();
            levelSaver.CreateSaveBlock("Rio", time, passLevels, repeatedLevels, 5, sessionManager.activeKid.riverSessions);
            levelSaver.AddLevelsToBlock();
            levelSaver.PostProgress();
        }

        PlayerPrefs.SetInt(Keys.River_Difficulty, difficulty);
        PlayerPrefs.SetInt(Keys.River_Level, level);
        PlayerPrefs.SetInt(Keys.River_First, 1);
    }

    void TellStory()
    {
        showTutorial = true;
        instructionIndex++;
        instructionScreens[instructionIndex - 1].SetActive(false);
        instructionScreens[instructionIndex].SetActive(true);
        instructionText.text = stringsToShow[instructionIndex];
        readyButton.gameObject.SetActive(false);
        audioManager.PlayClip(instructionsClips[instructionIndex]);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
        if (instructionIndex >= instructionScreens.Length - 1)
        {
            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(SetTheInstruction);
        }
    }

    void SetTheInstruction()
    {
        counting = true;
        if (showTutorial)
        {
            instructionScreens[instructionIndex].SetActive(false);
        }
        else
        {
            instructionPanel.transform.parent.gameObject.SetActive(true);
        }

        GetTheData();

        if (reverseMode)
        {
            instructionText.text = stringsToShow[3];
            readyButton.gameObject.SetActive(false);
            audioManager.PlayClip(instructionsClips[3]);
            Invoke("ReadyButtonOn", audioManager.ClipDuration());
        }
        else
        {
            instructionText.text = stringsToShow[4];
            readyButton.gameObject.SetActive(false);
            audioManager.PlayClip(instructionsClips[4]);
            Invoke("ReadyButtonOn", audioManager.ClipDuration());
        }
        readyButton.onClick.RemoveAllListeners();
        if (specialObjects > 0)
        {
            readyButton.onClick.AddListener(SetTheSpecialInstruction);
        }
        else
        {
            readyButton.onClick.AddListener(SetANewGame);
        }
    }

    void SetTheSpecialInstruction()
    {

        readyButton.onClick.RemoveAllListeners();
        switch (specialType[specialInstructionIndex])
        {
            case TypeOfSpecial.SpecificObject:
                if (specialAction[specialInstructionIndex] == Actions.LetItGo)
                {
                    instructionText.text = stringsToShow[7] + " " +finalStimulusStrings[specialInstructionIndex];
                    audioManager.PlayClip(instructionsClips[7], audiosOfStimulus[specialInstructionIndex]);

                }
                else if(specialAction[specialInstructionIndex] == Actions.Reverse)
                {
                    if (reverseMode)
                    {
                        instructionText.text = stringsToShow[8] + " " + finalStimulusStrings[specialInstructionIndex] + " " + stringsToShow[10];
                        audioManager.PlayClip(instructionsClips[8], audiosOfStimulus[specialInstructionIndex] , instructionsClips[10]);
                    }
                    else
                    {
                        instructionText.text = stringsToShow[8] + " " + finalStimulusStrings[specialInstructionIndex] + " " + stringsToShow[9];
                        audioManager.PlayClip(instructionsClips[8], audiosOfStimulus[specialInstructionIndex], instructionsClips[9]);
                    }
                }
                break;
            case TypeOfSpecial.AfterObject:
                if (specialAction[specialInstructionIndex] == Actions.LetItGo)
                {
                    instructionText.text = stringsToShow[11] + " " + finalStimulusStrings[specialInstructionIndex];
                    audioManager.PlayClip(instructionsClips[11], audiosOfStimulus[specialInstructionIndex]);
                }
                else if (specialAction[specialInstructionIndex] == Actions.Reverse)
                {
                    if (reverseMode)
                    {
                        instructionText.text = stringsToShow[12] + " " + finalStimulusStrings[specialInstructionIndex] + " " + stringsToShow[10];
                        audioManager.PlayClip(instructionsClips[12], audiosOfStimulus[specialInstructionIndex], instructionsClips[10]);
                    }
                    else
                    {
                        instructionText.text = stringsToShow[12] + " " + finalStimulusStrings[specialInstructionIndex] + " " + stringsToShow[9];
                        audioManager.PlayClip(instructionsClips[12], audiosOfStimulus[specialInstructionIndex], instructionsClips[9]);
                    }
                }
                break;
            case TypeOfSpecial.IfObject:
                if (specialAction[specialInstructionIndex] == Actions.LetItGo)
                {
                    if (specialInstructionIndex == 0)
                    {
                        instructionText.text = stringsToShow[5] + " " + stringsToShow[13];
                        audioManager.PlayClip(instructionsClips[5], instructionsClips[13]);
                    }
                    else
                    {
                        instructionText.text = stringsToShow[6] + " " + stringsToShow[13];
                        audioManager.PlayClip(instructionsClips[6], instructionsClips[13]);
                    }
                }
                else if (specialAction[specialInstructionIndex] == Actions.Reverse)
                {
                    if (specialInstructionIndex == 0)
                    {
                        if (reverseMode)
                        {
                            instructionText.text = stringsToShow[5] + " " + stringsToShow[10];
                            audioManager.PlayClip(instructionsClips[5], instructionsClips[10]);
                        }
                        else
                        {
                            instructionText.text = stringsToShow[5] + " " + stringsToShow[9];
                            audioManager.PlayClip(instructionsClips[5], instructionsClips[9]);
                        }
                    }
                    else
                    {
                        if (reverseMode)
                        {
                            instructionText.text = stringsToShow[6] + " " + stringsToShow[10];
                            audioManager.PlayClip(instructionsClips[6], instructionsClips[10]);
                        }
                        else
                        {
                            instructionText.text = stringsToShow[6] + " " + stringsToShow[9];
                            audioManager.PlayClip(instructionsClips[6], instructionsClips[9]);
                        }
                    }
                }
                break;
        }
        specialInstructionIndex++;
        readyButton.onClick.RemoveAllListeners();
        readyButton.gameObject.SetActive(false);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
        if (specialInstructionIndex < specialObjects)
        {
            readyButton.onClick.AddListener(SetTheSpecialInstruction);
        }
        else
        {
            readyButton.onClick.AddListener(SetANewGame);
        }
    }

    void SetANewGame()
    {
        instructionPanel.SetActive(false);
        SetParametersForDropping();
        DropTheStimulus();
    }

    void SetParametersForDropping()
    {
        if (specialObjects > 0)
        {
            maxTimeToShowSpecial = numberOfObjectToDrop / specialObjects;
        }
        objectsToDrop = numberOfObjectToDrop;
        speedOfSwiming = Vector3.Distance(showerSpot.transform.position, collector.transform.position);
    }

    void DropTheStimulus()
    {
        objectsToDrop--;
        timeToDrop = dropSpeed;
        if (objectsToDrop >= 0)
        {
            dropingTime = true;
            SelectAThing();
        }
        else
        {
            dropingTime = false;
        }

    }

    void SelectAThing()
    {
        //categorie 0 = forest Object, categorie 1 == beach Object
        bool isTheObjectDrop = false;
        if(specialObjects > 0)
        {
            if (specialType[0] == TypeOfSpecial.AfterObject)
            {
                if (whenToDrop1 == dropperTime)
                {
                    direction = direction1;
                    DropTheObject(specialStimulus[0]);

                    DropATarget(specialStimulus[0], specialType[0], 0);
                    isTheObjectDrop = true;
                }
                else if (whenToDrop1 + 1 == dropperTime)
                {
                    int categorie = Random.Range(0, 2);
                    direction = categorie;
                    SelectATargetToDrop(categorie);
                }
            }
            else
            {
                if (whenToDrop1 == dropperTime)
                {
                    direction = direction1;
                    DropATarget(specialStimulus[0], specialType[0], 0);
                    isTheObjectDrop = true;
                }
            }
            if (specialObjects > 1)
            {
                if (specialType[1] == TypeOfSpecial.AfterObject)
                {
                    if (whenToDrop2 == dropperTime)
                    {
                        direction = direction2;
                        DropTheObject(specialStimulus[1]);

                        DropATarget(specialStimulus[1], specialType[1], 1);
                        isTheObjectDrop = true;
                    }
                    else if (whenToDrop2 + 1 == dropperTime)
                    {
                        int categorie = Random.Range(0, 2);
                        direction = categorie;
                        SelectATargetToDrop(categorie);
                    }
                }
                else
                {
                    if (whenToDrop2 == dropperTime)
                    {
                        direction = direction2;
                        DropATarget(specialStimulus[1], specialType[1], 1);
                        isTheObjectDrop = true;
                    }
                }
            }
        }
        if (!isTheObjectDrop)
        {
            int categorie = Random.Range(0, 2);
            direction = categorie;
            SelectetTheObjectToDrop(categorie);
        }
        dropperTime++;
    }

    void SelectetTheObjectToDrop(int category)
    {
        int randomObject;
        if (specialObjects > 0)
        {
            switch (category)
            {
                case 0:
                    randomObject = Random.Range(0, finalForestStimulus.Count);
                    DropTheObject(finalForestStimulus[randomObject]);
                    break;
                case 1:
                    randomObject = Random.Range(0, finalBeachStimulus.Count);
                    DropTheObject(finalBeachStimulus[randomObject]);
                    break;
            }
        }
        else
        {
            switch (category)
            {
                case 0:
                    randomObject = Random.Range(0, forestObjets.Length);
                    DropTheObject(forestObjets[randomObject]);
                    break;
                case 1:
                    randomObject = Random.Range(0, beachObjects.Length);
                    DropTheObject(beachObjects[randomObject]);
                    break;
            }
        }
    }

    void SelectATargetToDrop(int category)
    {
        int randomObject;
        switch (category)
        {
            case 0:
                randomObject = Random.Range(0, finalForestStimulus.Count);
                DropTheObject(finalForestStimulus[randomObject]);
                break;
            case 1:
                randomObject = Random.Range(0, finalBeachStimulus.Count);
                DropTheObject(finalBeachStimulus[randomObject]);
                break;
        }
    }

    void DropATarget(GameObject theObject, TypeOfSpecial typer, int stimulNumber)
    {
        objectToDrop = Instantiate(theObject, positionToDrop.transform);
        float randyXPos = Random.Range(positionToDrop.center.x - (positionToDrop.size.x / 2), positionToDrop.center.x + (positionToDrop.size.x / 2));
        float randyZPos = Random.Range(positionToDrop.center.z - (positionToDrop.size.z / 2), positionToDrop.center.z + (positionToDrop.size.z / 2));
        Vector3 pos = new Vector3(randyXPos, 0, randyZPos);
        objectToDrop.transform.localPosition = pos;
        objectToDrop.GetComponent<Rigidbody>().useGravity = true;
        int typeOfSpecialToSave;
        switch (typer)
        {
            case TypeOfSpecial.AfterObject:
                typeOfSpecialToSave = 1;
                break;
            case TypeOfSpecial.IfObject:
                if (specialObjects < 2)
                {
                    typeOfSpecialToSave = 2;
                }
                else
                {
                    if (whenToDrop2 == dropperTime)
                    {
                        typeOfSpecialToSave = 3;
                    }
                    else
                    {
                        typeOfSpecialToSave = 2;
                    }
                }
                break;
            case TypeOfSpecial.SpecificObject:
                typeOfSpecialToSave = 0;
                break;
            default:
                typeOfSpecialToSave = 0;
                break;
        }
        objectToDrop.GetComponent<FloatingObject>().SetTheDirection(direction);
        objectToDrop.GetComponent<FloatingObject>().SetThisAsTarget(typeOfSpecialToSave, stimulNumber);
    }

    void DropTheObject(GameObject theObject)
    {
        objectToDrop = Instantiate(theObject, positionToDrop.transform);
        float randyXPos = Random.Range(positionToDrop.center.x - (positionToDrop.size.x / 2), positionToDrop.center.x + (positionToDrop.size.x / 2));
        float randyZPos = Random.Range(positionToDrop.center.z - (positionToDrop.size.z / 2), positionToDrop.center.z + (positionToDrop.size.z / 2));
        Vector3 pos = new Vector3(randyXPos, 0, randyZPos);
        objectToDrop.transform.localPosition = pos;
        objectToDrop.GetComponent<Rigidbody>().useGravity = true;
        objectToDrop.GetComponent<FloatingObject>().SetTheDirection(direction);
    }

    void OnTriggerEnter(Collider target)
    {
        if (target.tag == "NPC")
        {
            objectToDrop = Instantiate(target.gameObject, startArea.position, startArea.rotation);
            objectToDrop.transform.parent = startArea;
            BoxCollider boxi = startArea.GetComponent<BoxCollider>();
            float posX = Random.Range(boxi.center.x - (boxi.size.x / 2), boxi.center.x + (boxi.size.x / 2));
            Vector3 posi = new Vector3(posX, objectToDrop.transform.localPosition.y, objectToDrop.transform.localPosition.z);
            objectToDrop.transform.localPosition = posi;
            objectToDrop.tag = "Player";
            FloatingObject oldFloatingObject = target.GetComponent<FloatingObject>();
            FloatingObject newFloatingObject = objectToDrop.GetComponent<FloatingObject>();
            newFloatingObject.SetTheDirection(oldFloatingObject.GetTheData());
            if (oldFloatingObject.IsThisATarget())
            {
                newFloatingObject.SetThisAsTarget(oldFloatingObject.WhatTargetIs(), oldFloatingObject.SpecialNumber());
            }
            newFloatingObject.SetToSwim(speedOfSwiming);
            Destroy(target.gameObject);
        }
    }

    public void GrabAThing(GameObject theSelectedObject)
    {
        selectedObject = theSelectedObject;
        dragMode = true;
        selectedObject.transform.position = showerSpot.transform.position;
        lastKnowPosition = Vector3.Distance(selectedObject.transform.position, Camera.main.transform.position);
    }

    void DropAThing()
    {
        if (Input.GetMouseButtonUp(0)) {
            FloatingObject floatingObject = selectedObject.GetComponent<FloatingObject>();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            dragMode = false;
            int numToCompare = floatingObject.GetTheData();
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, classifierMask))
            {
                if (floatingObject.IsThisATarget())
                {
                    if (specialAction[floatingObject.SpecialNumber()] == Actions.Reverse)
                    {
                        if (reverseMode)
                        {
                            if (numToCompare == hit.transform.GetSiblingIndex())
                            {
                                specificGoodAnswer++;
                                correctAnswer++;
                                hit.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                            }
                            else
                            {
                                specificBadAnswer++;
                                badAnswer++;
                                hit.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                            }
                        }
                        else
                        {
                            if (numToCompare != hit.transform.GetSiblingIndex())
                            {
                                correctAnswer++;
                                hit.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                            }
                            else
                            {
                                specificBadAnswer++;
                                badAnswer++;
                                hit.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                            }
                        }
                        if (IsLastStimulus())
                        {
                            HandleLevelData();
                        }
                    }
                    else
                    {
                        specificBadAnswer++;
                        badAnswer++;
                        hit.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                    }

                    GameObject temp = selectedObject;
                    selectedObject = null;
                    Destroy(temp);
                    handleStimulus++;

                    if (IsLastStimulus())
                    {
                        HandleLevelData();
                    }

                }
                else
                {
                    if (reverseMode)
                    {
                        if (numToCompare != hit.transform.GetSiblingIndex())
                        {
                            correctAnswer++;
                            hit.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                        }
                        else
                        {
                            badAnswer++;
                            hit.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                        }
                    }
                    else
                    {
                        if (numToCompare == hit.transform.GetSiblingIndex())
                        {
                            correctAnswer++;
                            hit.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                        }
                        else
                        {
                            badAnswer++;
                            hit.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                        }
                    }

                    GameObject temp = selectedObject;
                    selectedObject = null;
                    Destroy(temp);
                    handleStimulus++;

                    if (IsLastStimulus())
                    {
                        HandleLevelData();
                    }
                }

            }
            else
            {
                floatingObject.ActAfterAnswer();
                selectedObject = null;
            }
        }

    }

    void DragAnObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 pos = ray.GetPoint(lastKnowPosition);
        selectedObject.transform.position = pos;
    }

    bool IsLastStimulus()
    {
        return handleStimulus == numberOfObjectToDrop;
    }

    public void MissAnAnswer(FloatingObject floating)
    {
        GameObject collector = transform.GetChild(3).gameObject;
        collector.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        if (floating.IsThisATarget())
        {
            specificMissAnswer++;
        }
        missAnswer++;
        handleStimulus++;

        if (IsLastStimulus())
        {
            HandleLevelData();
        }
    }

    public void SpecialMiss(int special)
    {
        GameObject collector = transform.GetChild(3).gameObject;
        if (specialAction[special] == Actions.LetItGo)
        {
            collector.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            correctAnswer++;
        }
        else
        {
            collector.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
            specificBadAnswer++;
            badAnswer++;
        }

        handleStimulus++;
        if (IsLastStimulus())
        {
            HandleLevelData();
        }
    }

    void HandleLevelData()
    {
        int totalErrors = missAnswer + badAnswer;
        totalCorrects += (numberOfObjectToDrop - totalErrors);
        totalIncorrects += totalErrors;
        totalCorrectTargets += specificGoodAnswer;
        totalIncorrectTargets += specificBadAnswer + specificMissAnswer;
        numberOfAssays--;
        if (!sessionManager.activeKid.riverFirst)
        {
            if (totalErrors < 2)
            {
                if (specificBadAnswer < 1)
                {
                    level++;
                    passLevels++;
                    if (level == 18)
                    {
                        difficulty++;
                        if (difficulty >= 2)
                        {
                            difficulty = 1;
                            level = 17;
                        }
                        else
                        {
                            level = 0;
                        }
                    }
                }
            }
            else
            {
                repeatedLevels++;
                level--;
            }
        }
        else
        {
            if (totalErrors < 2)
            {
                passLevels++;
                levelCategorizer += LevelDifficultyChange(totalLevels);
            }
            else
            {
                levelCategorizer -= LevelDifficultyChange(totalLevels);
            }
            levelCategorizer = Mathf.Clamp(levelCategorizer, 0, totalLevels - 1);
            GetDataJustForLevel(levelCategorizer);
        }
        PrepareAnotherGame();
    }

    void PrepareAnotherGame()
    {
        if (numberOfAssays <= 0)
        {
            FinishTheGame();
        }
        else
        {
            CreateNewLevel();
        }
    }

    void CreateNewLevel()
    {
        instructionIndex = 0;
        missAnswer = 0;
        badAnswer = 0;
        specificBadAnswer = 0;
        specificGoodAnswer = 0;
        specificMissAnswer = 0;
        correctAnswer = 0;
        handleStimulus = 0;
        specialInstructionIndex = 0;
        dropperTime = 0;
        direction = 0;
        direction1 = 0;
        direction2 = 0;
        instructionPanel.SetActive(true);
        specialAction.Clear();
        finalStimulusStrings.Clear();
        audiosOfStimulus.Clear();
        specialType.Clear();
        finalBeachStimulus.Clear();
        finalForestStimulus.Clear();
        specialStimulus.Clear();
        SetTheInstruction();
    }

    void FinishTheGame()
    {
        instructionPanel.SetActive(true);
        SaveLevel();
        readyButton.onClick.RemoveAllListeners();
        instructionText.text = stringsToShow[14];
        readyButton.gameObject.SetActive(false);
        audioManager.PlayClip(instructionsClips[14]);
        Invoke("ShowEarnings", audioManager.ClipDuration());
    }

    void ShowEarnings()
    {
        instructionPanel.gameObject.SetActive(false);
        pauser.ShowKiwiEarnings(passLevels);
    }

    void ReadyButtonOn()
    {
        readyButton.gameObject.SetActive(true);
    }

    void GetTheData()
    {
        int[] data = GameConfigurator.RiverConfig(difficulty, level);

        playedLevels.Add(level);
        playedDifficulty.Add(difficulty);

        if (data[0] == 0)
        {
            reverseMode = false;
        }
        else
        {
            reverseMode = true;
        }

        dropSpeed = data[1];

        specialObjects = (data.Length / 2) - 1;
        totalTargets += specialObjects;

        for (int i = 0; i < data.Length - 2; i += 2)
        {
            switch (data[i + 2])
            {
                case 0:
                    specialType.Add(TypeOfSpecial.SpecificObject);
                    break;
                case 1:
                    specialType.Add(TypeOfSpecial.AfterObject);
                    break;
                case 2:
                    specialType.Add(TypeOfSpecial.IfObject);
                    break;
            }

            if (data[i + 3] == 0)
            {
                specialAction.Add(Actions.LetItGo);
                totalSpecials++;
            }
            else if (data[i + 3] == 1)
            {
                specialAction.Add(Actions.Reverse);
                totalReverse++;
            }
        }

        if (specialObjects > 0)
        {
            int randy;
            int randomCategoryChoice;
            List<int> whenIDrop = new List<int>();
            numbersForest.Clear();
            numbersBeach.Clear();
            for (int i = 0; i < numberOfObjectToDrop - 1; i++)
            {
                int z = i;
                whenIDrop.Add(z);
            }
            for (int i = 0; i < forestObjets.Length;i++)
            {
                finalForestStimulus.Add(forestObjets[i]);
                forestFinalStrings.Add(forestStrings[i]);
                int x = i;
                numbersForest.Add(x);
            }

            for (int i = 0; i < beachObjects.Length; i++)
            {
                finalBeachStimulus.Add(beachObjects[i]);
                beachFinalStrings.Add(beachStrings[i]);
                int x = i;
                numbersBeach.Add(x);
            }

            randomCategoryChoice = Random.Range(0, 2);

            if (randomCategoryChoice == 0)
            {
                randy = Random.Range(0, finalForestStimulus.Count);
                specialStimulus.Add(finalForestStimulus[randy]);
                finalStimulusStrings.Add(forestFinalStrings[randy]);
                audiosOfStimulus.Add(stimulusForestClips[numbersForest[randy]]);
                finalForestStimulus.Remove(finalForestStimulus[randy]);
                forestFinalStrings.Remove(forestFinalStrings[randy]);
                numbersForest.Remove(numbersForest[randy]);
            }
            else
            {
                randy = Random.Range(0, finalBeachStimulus.Count);
                specialStimulus.Add(finalBeachStimulus[randy]);
                finalStimulusStrings.Add(beachFinalStrings[randy]);
                audiosOfStimulus.Add(stimulusBeachClips[numbersBeach[randy]]);
                finalBeachStimulus.Remove(finalBeachStimulus[randy]);
                beachFinalStrings.Remove(beachFinalStrings[randy]);
                numbersBeach.Remove(numbersBeach[randy]);
            }
            direction1 = randomCategoryChoice;

            if (specialObjects > 1)
            {
                randomCategoryChoice = Random.Range(0, 2);
                if (randomCategoryChoice == 0)
                {
                    randy = Random.Range(0, finalForestStimulus.Count);
                    specialStimulus.Add(finalForestStimulus[randy]);
                    finalStimulusStrings.Add(forestFinalStrings[randy]);
                    audiosOfStimulus.Add(stimulusForestClips[numbersForest[randy]]);
                    finalForestStimulus.Remove(finalForestStimulus[randy]);
                    forestFinalStrings.Remove(forestFinalStrings[randy]);
                    numbersForest.Remove(numbersForest[randy]);
                }
                else
                {
                    randy = Random.Range(0, finalBeachStimulus.Count);
                    specialStimulus.Add(finalBeachStimulus[randy]);
                    finalStimulusStrings.Add(beachFinalStrings[randy]);
                    audiosOfStimulus.Add(stimulusBeachClips[numbersBeach[randy]]);
                    finalBeachStimulus.Remove(finalBeachStimulus[randy]);
                    beachFinalStrings.Remove(beachFinalStrings[randy]);
                    numbersBeach.Remove(numbersBeach[randy]);
                }
                direction2 = randomCategoryChoice;
            }

            randy = Random.Range(0, whenIDrop.Count);
            whenToDrop1 = whenIDrop[randy];
            whenIDrop.Remove(whenToDrop1 - 1);
            whenIDrop.Remove(whenToDrop1);
            if (specialType[0] == TypeOfSpecial.AfterObject)
            {
                if (whenToDrop1 != numberOfObjectToDrop-1)
                {
                    whenIDrop.Remove(whenToDrop1 + 1);
                }
            }

            if (specialObjects > 1)
            {
                randy = Random.Range(0, whenIDrop.Count);
                whenToDrop2 = whenIDrop[randy];

                if (specialType[1] == TypeOfSpecial.AfterObject)
                {
                    if (whenToDrop2 != numberOfObjectToDrop - 1)
                    {
                        whenIDrop.Remove(whenToDrop2 + 1);
                    }
                }
            }
        }
    }

    void GetDataJustForLevel(int levelInput)
    {
        int amountOfDifficulties = 4;
        int baseLevelDifficulty = 16;

        for (int i = 0; i < amountOfDifficulties; i++)
        {

            if (levelInput < baseLevelDifficulty * (i + 1))
            {
                int x = i;
                difficulty = x;
                break;
            }
        }

        level = levelInput - (difficulty * baseLevelDifficulty);

        Debug.Log("Level is " + level + " Difficulty is " + difficulty);
    }

    /// <summary>
    /// Function to determine how many levels a player change with the FLIS
    /// </summary>
    /// <param name="totalNumberOfLevelsToAdapt"></param>
    /// <returns></returns>
    int LevelDifficultyChange(int totalNumberOfLevelsToAdapt)
    {
        //To determine how many levels go up or down we use the function
        // y = z/(2^(x+1))
        //where x = current assay, y = the amount of levels to change, z = the total levels of the game
        //x starts in 0
        //we return a integer value, so could be some differences with an actual graphic of the function
        int currentAssay = 5 - numberOfAssays;
        int amountOfLevelsToChange = Mathf.RoundToInt(totalNumberOfLevelsToAdapt / Mathf.Pow(2, (currentAssay + 1)));
        return amountOfLevelsToChange;
    }
}
