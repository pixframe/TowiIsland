using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonkeyHidingManager : MonoBehaviour {

    #region Variables
    [Header("Managers")]
    public GameObject monkeyManager;
    public GameObject stimulusManager;
    public GameObject indicationManager;
    public GameObject positions1;
    public GameObject positions2;

    [Header("UI Elements")]
    public Text instructionText;
    public Button readyButton;
    public GameObject instructionPanel;

    [Header("Oranaments Area")]
    public GameObject[] screen;

    [Header("Text Elements")]
    public TextAsset textAsset;
    public TextAsset objectsText;
    string[] stringsToShow;
    string[] stringOfObjects;

    int numberOfMovements;
    int numberOfObjects;
    int numberOfObjectsToFind;
    int numberOfMonekys;
    int difficulty;
    int level;
    int monkeysInCorrectPlace;
    int monkeysSwitchInCorrectPlaces;
    int straightBadAnswers;
    int levelCategorizer;
    int numberOfAssays = 5;
    int totalNumberOfAssays;
    int goodAnswer;
    int badAnswer;
    int numberOfObjectToFind;
    int firstTime;
    int passLevels;
    int repeatedLevels;
    int totalLevels = 60;
    int miniKidLevel = 15;
    int maxiKidLevel = 45;

    float movementTime;
    float timeForMovement;

    string objectToFind;

    float time;
    float latency;

    bool counting = false;
    bool showTutorial = false;
    bool findingMultipleObjects = false;
    bool latencyTime = false;

    MonkeyController monkey1;
    MonkeyController monkey2;
    PauseTheGame pauser;
    AudioManager audioManager;
    SessionManager sessionManager;
    LevelSaver levelSaver;

    AudioClip[] instructionsClips;
    AudioClip[] stimulusAudioClip;

    List<MonkeyController> monkeys = new List<MonkeyController>();
    List<MonkeyController> monkeysHolders = new List<MonkeyController>();
    List<GameObject> totalStimulus = new List<GameObject>();
    List<GameObject> tempMonkeys = new List<GameObject>();
    List<GameObject> tempStimulus = new List<GameObject>();
    List<GameObject> finalMonkeys = new List<GameObject>();
    List<GameObject> finalStimulus = new List<GameObject>();
    List<GameObject> positions = new List<GameObject>();
    List<GameObject> indications = new List<GameObject>();
    List<int> tempStimulusNums = new List<int>();
    List<int> stimulusNumbers = new List<int>();
    List<int> playedLevels = new List<int>();
    List<int> playedDifficulty = new List<int>();
    List<float> latencies = new List<float>();

    #endregion

    // Use this for initialization
    void Start ()
    {
        if (FindObjectOfType<SessionManager>())
        {
            sessionManager = FindObjectOfType<SessionManager>();
            levelSaver = GetComponent<LevelSaver>();
        }
        audioManager = FindObjectOfType<AudioManager>();
        textAsset = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Games/Monkeys/MonkeyText");
        objectsText = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Games/Monkeys/MonkeyObjects");
        instructionsClips = Resources.LoadAll<AudioClip>($"{LanguagePicker.BasicAudioRoute()}Games/Monkeys");
        GetLevel();
        stringsToShow = TextReader.TextsToShow(textAsset);
        stringOfObjects = TextReader.TextsToShow(objectsText);
        stimulusAudioClip = new AudioClip[stringOfObjects.Length];
        for (int i = 0; i < stringOfObjects.Length; i++)
        {
            stimulusAudioClip[i] = Resources.Load<AudioClip>($"{LanguagePicker.BasicAudioRoute()}Games/Stimulus/g_st_0{i}");
        }
        pauser = FindObjectOfType<PauseTheGame>();
        counting = true;
        if (firstTime == 0)
        {
            SetStory(0);
            pauser.HideTutorialButtons();
        }
        else
        {
            instructionPanel.transform.parent.gameObject.SetActive(false);
            pauser.WantTutorial();
            pauser.howToPlayButton.onClick.AddListener(() => SetStory(0));
            pauser.playButton.onClick.AddListener(SetAGame);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (counting)
        {
            time += Time.deltaTime;
        }
        if (latencyTime)
        {
            latency += Time.deltaTime;
        }
	}


    #region SetUps

    #endregion
    void GetLevel()
    {
        totalNumberOfAssays = numberOfAssays;
        if (!FindObjectOfType<DemoKey>())
        {
            if (sessionManager != null)
            {
                if (sessionManager.activeKid.monkeyFirst)
                {
                    FLISSetup();
                }
                else
                {
                    difficulty = sessionManager.activeKid.monkeyDifficulty;
                    if (difficulty > 1)
                    {
                        difficulty = 1;
                    }
                    level = sessionManager.activeKid.monkeyLevel;
                    firstTime = 1;
                }
            }
            else
            {
                difficulty = PlayerPrefs.GetInt(Keys.Monkey_Difficulty);
                level = PlayerPrefs.GetInt(Keys.Monkey_Level);
                firstTime = PlayerPrefs.GetInt(Keys.Monkey_First);
            }
        }
        else
        {
            var key = FindObjectOfType<DemoKey>();
            if (key.IsFLISOn())
            {
                sessionManager.activeKid.monkeyFirst = true;
                FLISSetup();
            }
            else
            {
                sessionManager.activeKid.monkeyFirst = false;
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
            sessionManager.activeKid.monkeyDifficulty = difficulty;
            sessionManager.activeKid.monkeyLevel = level;

            sessionManager.activeKid.kiwis += passLevels;
            sessionManager.activeKid.playedMonkey = 1;
            sessionManager.activeKid.needSync = true;

            //levelSaver.AddLevelData("level", difficulty);
            //levelSaver.AddLevelData("sublevel", level);
            //levelSaver.AddLevelData("time", time);
            //levelSaver.AddLevelData("numofmonkeys", 5);
            //levelSaver.AddLevelData("numofobjects", 2);
            //levelSaver.AddLevelData("instructions", "find");
            //levelSaver.AddLevelData("numofmovements", 14);
            //levelSaver.AddLevelData("timeofmovements", 10);
            //levelSaver.AddLevelData("correct", goodAnswer);

            //Verison 2
            sessionManager.activeKid.monkeySessions++;

            if (sessionManager.activeKid.monkeyFirst)
            {
                sessionManager.activeKid.monkeyFirst = false;
                levelSaver.AddLevelData("initial_level", level);
                levelSaver.AddLevelData("initial_difficulty", difficulty);
            }
            levelSaver.AddLevelData("current_level", level);
            levelSaver.AddLevelData("current_difficulty", difficulty);
            levelSaver.AddLevelData("correct_monkeys", goodAnswer);
            levelSaver.AddLevelData("errors_monkeys", badAnswer);
            levelSaver.AddLevelData("session_correct_percentage", goodAnswer * 20);
            levelSaver.AddLevelData("session_errors_percentage", badAnswer * 20);
            levelSaver.AddLevelDataAsString("latency", latencies);
            levelSaver.AddLevelDataAsString("played_levels", playedLevels);
            levelSaver.AddLevelDataAsString("played_difficulty", playedDifficulty);
            levelSaver.AddLevelData("session_number", difficulty);
            levelSaver.AddLevelData("session_time", time);

            levelSaver.CreateSaveBlock("DondeQuedoLaBolita", time, passLevels, repeatedLevels, passLevels - repeatedLevels, sessionManager.activeKid.monkeySessions);
            levelSaver.AddLevelsToBlock();
            levelSaver.PostProgress();

        }

        PlayerPrefs.SetInt(Keys.Monkey_Difficulty, difficulty);
        PlayerPrefs.SetInt(Keys.Monkey_Level, level);
        PlayerPrefs.SetInt(Keys.Monkey_First, 1);
    }

    void SetStory(int part)
    {
        if (!showTutorial)
        {
            showTutorial = true;
            instructionPanel.transform.parent.gameObject.SetActive(true);
        }
        readyButton.gameObject.SetActive(false);
        instructionText.text = stringsToShow[part];
        audioManager.PlayClip(instructionsClips[part]);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
        screen[part].SetActive(true);
        readyButton.onClick.RemoveAllListeners();
        if (part > 0)
        {
            screen[part-1].SetActive(false);
        }
        if (part < 2)
        {
            readyButton.onClick.AddListener(() => SetStory(part + 1));
        }
        else
        {
            readyButton.onClick.AddListener(SetAGame);
        }
    }

    void SetAGame()
    {
        if (showTutorial)
        {
            showTutorial = false;
        }
        else
        {
            instructionPanel.transform.parent.gameObject.SetActive(true);
        }
        screen[2].gameObject.SetActive(false);
        readyButton.gameObject.SetActive(false);
        instructionPanel.SetActive(false);
        FillTheData();
        FillTheLists();
        ChooseTheCorrectElemets();
        SendMonkeysToWalk();
    }

    void FillTheData()
    {
        int[] dataGameConfig = GameConfigurator.MonkeyGameConfig(difficulty, level);
        numberOfMonekys = dataGameConfig[0];
        movementTime = dataGameConfig[1];
        numberOfMovements = dataGameConfig[2];
        numberOfObjectsToFind = dataGameConfig[3];
        timeForMovement = movementTime / numberOfMovements;
        numberOfObjects = difficulty + 1;
        playedLevels.Add(level);
        playedDifficulty.Add(difficulty);
    }

    void FillTheLists()
    {
        for (int i = 0; i < indicationManager.transform.childCount; i++)
        {
            indications.Add(indicationManager.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < monkeyManager.transform.childCount; i++)
        {
            monkeys.Add(monkeyManager.transform.GetChild(i).GetComponent<MonkeyController>());
        }

        for (int i = 0; i < stimulusManager.transform.childCount; i++)
        {
            totalStimulus.Add(stimulusManager.transform.GetChild(i).gameObject);
        }

        if (numberOfMonekys == 4)
        {
            for (int i = 0; i < positions2.transform.childCount; i++)
            {
                positions.Add(positions2.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject);
            }
        }
        else
        {
            for (int i = 0; i < positions1.transform.childCount; i++)
            {
                positions.Add(positions1.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject);
            }
        }
    }

    void ChooseTheCorrectElemets()
    {

        for (int i = 0; i < numberOfMonekys; i++) {
            tempMonkeys.Add(monkeys[i].gameObject);
            tempMonkeys[i].GetComponent<MonkeyController>().SetPosition(positions[i].transform.position);
            finalMonkeys.Add(tempMonkeys[i]);
        }

        for (int i = 0; i < totalStimulus.Count; i++)
        {
            tempStimulus.Add(totalStimulus[i]);
            int x = i;
            tempStimulusNums.Add(x);
        }

        for (int i = 0; i < numberOfObjects; i++)
        {
            int randomize = UnityEngine.Random.Range(0, tempStimulus.Count);
            finalStimulus.Add(tempStimulus[randomize]);
            stimulusNumbers.Add(tempStimulusNums[randomize]);
            tempStimulus.Remove(tempStimulus[randomize]);
            tempStimulusNums.Remove(tempStimulusNums[randomize]);
        }

        foreach (GameObject o in finalStimulus)
        {
            Debug.Log($"we have to look for {o.name}");
        }

        foreach (int i in stimulusNumbers)
        {
            Debug.Log($"we have in {numberOfAssays} this stimulus {i}");
        }
    }

    void SendMonkeysToWalk()
    {
        for (int i = 0; i < finalMonkeys.Count; i++) {
            finalMonkeys[i].GetComponent<MonkeyController>().WalkTo(0);
        }
    }

    public void MonkeyInPlace()
    {
        monkeysInCorrectPlace++;
        if (monkeysInCorrectPlace == numberOfMonekys)
        {
            monkeysInCorrectPlace = 0;
            foreach (GameObject mon in finalMonkeys)
            {
                mon.GetComponent<MonkeyController>().MonkeyFloat();
            }
        }
    }

    public void MonkeysFlying()
    {
        monkeysInCorrectPlace++;
        if (monkeysInCorrectPlace == numberOfMonekys)
        {
            monkeysInCorrectPlace = 0;
            SelectMonkeyHolders();
            for (int i = 0; i < finalStimulus.Count; i++) {
                finalStimulus[i].SetActive(true);
                finalStimulus[i].transform.position = monkeysHolders[i].HoldAnObject(finalStimulus[i].name, i);
            }
            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(MonkeysOfPapantla);
            readyButton.gameObject.SetActive(true);
            instructionPanel.gameObject.SetActive(true);
            if (numberOfObjects < 2)
            {
                instructionText.text = stringsToShow[3];
                audioManager.PlayClip(instructionsClips[3]);
                Invoke("ReadyButtonOn", audioManager.ClipDuration());
            }
            else
            {
                instructionText.text = stringsToShow[4];
                audioManager.PlayClip(instructionsClips[4]);
                Invoke("ReadyButtonOn", audioManager.ClipDuration());
            }
        }
    }

    void SelectMonkeyHolders() {
        for (int i = 0; i < finalStimulus.Count; i++)
        {
            int randomize = Random.Range(0, tempMonkeys.Count);
            monkeysHolders.Add(tempMonkeys[randomize].GetComponent<MonkeyController>());
            tempMonkeys.Remove(tempMonkeys[randomize]);
        }
    }

    void MonkeysOfPapantla() {
        foreach (GameObject sti in finalStimulus)
        {
            sti.SetActive(false);
        }
        ChooseMonkeysToMove();
        instructionPanel.gameObject.SetActive(false);
        Cursor.visible = false;
    }

    void ChooseMonkeysToMove()
    {
        tempMonkeys.RemoveRange(0, tempMonkeys.Count);
        for (int i = 0; i < finalMonkeys.Count; i++)
        {
            tempMonkeys.Add(finalMonkeys[i]);
        }
        int randomize = Random.Range(0, tempMonkeys.Count);
        monkey1 = tempMonkeys[randomize].GetComponent<MonkeyController>();
        tempMonkeys.Remove(tempMonkeys[randomize]);
        randomize = Random.Range(0, tempMonkeys.Count);
        monkey2 = tempMonkeys[randomize].GetComponent<MonkeyController>();
        MoveTheChoosenMonkeys();
    }

    void MoveTheChoosenMonkeys()
    {
        Vector3 pos1 = monkey2.transform.position;
        Vector3 pos2 = monkey1.transform.position;
        monkey1.MoveToNextPopsition(0, pos1, timeForMovement);
        monkey2.MoveToNextPopsition(1, pos2, timeForMovement);
    }

    public void NextMovement() {
        monkeysSwitchInCorrectPlaces++;
        if (monkeysSwitchInCorrectPlaces == 2)
        {
            monkeysSwitchInCorrectPlaces = 0;
            numberOfMovements--;
            if (numberOfMovements > 0)
            {
                ChooseMonkeysToMove();
            }
            else
            {
                latencyTime = true;
                SetObjectsToFind();
                Cursor.visible = true;
                foreach (GameObject mon in finalMonkeys)
                {
                    mon.GetComponent<MonkeyController>().SelectableNow();
                }
            }
        }
    }

    public void CorrectAnswer(string name)
    {

        latencyTime = false;
        if (objectToFind == null)
        {
            numberOfObjectsToFind--;
            if (numberOfObjectsToFind == 0)
            {
                GoodAnswer(11);
            }
        }
        else
        {
            Debug.Log($"object to find is {objectToFind} object grabbed is {name}");
            if (objectToFind == name)
            {
                numberOfObjectsToFind--;
                if (numberOfObjectsToFind == 0)
                {
                    if (findingMultipleObjects)
                    {
                        GoodAnswer(14);
                    }
                    else
                    {
                        GoodAnswer(11);
                    }
                }
            }
            else
            {
                BadAnswer();
            }
        }
    }

    public void BadAnswer()
    {
        latencyTime = false;
        badAnswer++;
        straightBadAnswers++;
        numberOfAssays--;

        GetLevelDown();
        instructionText.text = stringsToShow[10];
        audioManager.PlayClip(instructionsClips[10]);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());

        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(ExitMonkeys);
        foreach (GameObject mon in finalMonkeys)
        {
            mon.GetComponent<MonkeyController>().DisableTheMonkey();
        }
    }

    void GoodAnswer(int audiosAndTextToPlay)
    {
        goodAnswer++;
        straightBadAnswers = 0;
        level++;
        passLevels++;
        numberOfAssays--;

        GetLevelUp();
        instructionText.text = stringsToShow[audiosAndTextToPlay];
        audioManager.PlayClip(instructionsClips[audiosAndTextToPlay]);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());

        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(ExitMonkeys);

        foreach (GameObject mon in finalMonkeys)
        {
            mon.GetComponent<MonkeyController>().DisableTheMonkey();
        }
    }

    void GetLevelUp()
    {
        if (!sessionManager.activeKid.monkeyFirst)
        {
            if (level >= 30)
            {
                if (difficulty < 1)
                {
                    difficulty++;
                    level = 0;
                }
                else
                {
                    difficulty = 1;
                    level = 29;
                }
            }
        }
        else
        {
            levelCategorizer += LevelDifficultyChange(totalLevels);
            levelCategorizer = Mathf.Clamp(levelCategorizer, 0, totalLevels - 1);
            GetDataJustForLevel(levelCategorizer);
        }
    }

    void GetLevelDown()
    {
        if (!sessionManager.activeKid.monkeyFirst)
        {
            if (straightBadAnswers >= 3)
            {
                level--;
                straightBadAnswers = 0;
                if (level < 0)
                {
                    if (difficulty <= 1)
                    {
                        difficulty = 0;
                        level = 29;
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
            levelCategorizer -= LevelDifficultyChange(totalLevels);
            levelCategorizer = Mathf.Clamp(levelCategorizer, 0, totalLevels - 1);
            GetDataJustForLevel(levelCategorizer);
        }
    }

    void ExitMonkeys()
    {
        foreach (GameObject g in finalStimulus)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in indications)
        {
            g.SetActive(false);
        }
        foreach (GameObject mon in finalMonkeys)
        {
            mon.GetComponent<MonkeyController>().DownTheMonkey();
        }
        instructionPanel.SetActive(false);
    }

    public void MonkeysReadyToDanceAgain()
    {
        monkeysInCorrectPlace++;
        if(monkeysInCorrectPlace == finalMonkeys.Count)
        {
            monkeysInCorrectPlace = 0;
            latencies.Add(latency);
            if (numberOfAssays > 0)
            {
                ResetGame();
            }
            else
            {
                FinishGame();
            }
        }
    }

    public GameObject ObjectToShow(int i)
    {
        return finalStimulus[i];
    }

    public GameObject BadAnswerShow()
    {
        return indications[0];
    }

    void ResetGame() {
        foreach (GameObject mon in finalMonkeys)
        {
            mon.GetComponent<MonkeyController>().MonkeyReset();
        }
        monkeys.RemoveRange(0, monkeys.Count);
        monkeysHolders.RemoveRange(0, monkeysHolders.Count);

        //Clear Area


        totalStimulus.RemoveRange(0, totalStimulus.Count);
        tempMonkeys.RemoveRange(0, tempMonkeys.Count);
        tempStimulus.RemoveRange(0, tempStimulus.Count);
        finalMonkeys.RemoveRange(0, finalMonkeys.Count);
        finalStimulus.RemoveRange(0, finalStimulus.Count);
        stimulusNumbers.Clear();
        tempStimulusNums.Clear();
        positions.RemoveRange(0, positions.Count);
        latency = 0;
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(SetAGame);
        readyButton.gameObject.SetActive(true);
        instructionPanel.SetActive(true);
        instructionText.text = stringsToShow[12];
        audioManager.PlayClip(instructionsClips[12]);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
    }

    void SetObjectsToFind()
    {
        instructionPanel.SetActive(true);
        readyButton.gameObject.SetActive(false);
        if (numberOfObjects == 1)
        {
            instructionText.text = stringsToShow[7];
            audioManager.PlayClip(instructionsClips[7]);
            objectToFind = null;
            numberOfObjectToFind = stimulusNumbers[0];
            findingMultipleObjects = false;
        }
        else
        {
            if (numberOfObjectsToFind == 1)
            {
                int randomito = Random.Range(0, numberOfObjects);
                instructionText.text = $"{stringsToShow[8]} {stringOfObjects[stimulusNumbers[randomito]]}";
                Debug.Log($"you have to find {finalStimulus[randomito].name}");
                audioManager.PlayClip(instructionsClips[8], stimulusAudioClip[stimulusNumbers[randomito]]);
                objectToFind = finalStimulus[randomito].name;
                numberOfObjectToFind = stimulusNumbers[randomito];
                findingMultipleObjects = false;
            }
            else
            {
                instructionText.text = stringsToShow[9];
                audioManager.PlayClip(instructionsClips[9]);
                objectToFind = null;
                findingMultipleObjects = true;
            }
        }
    }

    void FinishGame()
    {
        SaveLevel();
        readyButton.onClick.RemoveAllListeners();
        readyButton.gameObject.SetActive(false);
        instructionPanel.SetActive(true);
        instructionText.text = stringsToShow[13];
        audioManager.PlayClip(instructionsClips[13]);
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

    void GetDataJustForLevel(int levelInput)
    {
        if (levelInput < 30)
        {
            difficulty = 0;
            level = levelInput;
        }
        else
        {
            difficulty = 1;
            level = levelInput - 30;
        }

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
        Debug.Log(amountOfLevelsToChange);
        return amountOfLevelsToChange;
    }
}
