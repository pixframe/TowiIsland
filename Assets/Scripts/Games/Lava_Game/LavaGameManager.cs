using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LavaGameManager : MonoBehaviour {
    #region Variables

    #region Scripts

    AudioManager audioManager;
    LevelSaver levelSaver;
    SessionManager sessionManager;

    #endregion

    #region Posibilities

    //This is used to determine if a shadow has to move or not
    enum HideMovement { None, Switch, Rotation};
    HideMovement hideMovement;
    
    //This is used in the order od the game to develop
    enum GamePhase { ShowObject, ShowVapor, ShowShadows, EraserVapors, ShowResults, AnswerVapor};
    GamePhase phase = GamePhase.ShowObject;

    #endregion

    #region UI Elements
    [Header("UI Elments")]
    public GameObject instructionPanel;
    public TextMeshProUGUI instructionText;
    public Button readyButton;

    [Header("UI World")]
    public GameObject buttonCanvas;
    public GameObject imageOfResultsManager;
    public Image imageToIdentify;
    List<Button> buttonsOfObjects = new List<Button>();
    List<Image> imagesOfResults = new List<Image>();

    #endregion

    #region Particles Systems 

    //Those particle systems are used to create the animation sentation of the game
    [Header("Ornaments")]
    public GameObject particleManager;
    List<ParticleSystem> vapors = new List<ParticleSystem>();

    #endregion

    #region Sprites

    //Here will save the instructions Vectors
    public GameObject[] sceneInstructions;

    //Those are the possible categories to be Used in the game 
    [Header("Categories")]
    public Sprite[] category0 = new Sprite[21];
    public Sprite[] category1 = new Sprite[21];
    public Sprite[] category2 = new Sprite[21];
    public Sprite[] category3 = new Sprite[21];
    public Sprite[] category4 = new Sprite[21];
    public Sprite[] category5 = new Sprite[21];
    public Sprite[] category6 = new Sprite[21];

    //This will set the categories into a big list for esay accessibility
    List<Sprite[]> categories;

    #endregion

    #region Audios

    AudioClip[] instructionsClips;

    #endregion

    #region Integers
    int instructionIndex;
    //Here we will store the number of the current object
    int objectToFind;
    //Here we will get the posibles categories of the sprites
    int numberOfCategories;
    //This is the one that get the variables
    int level = 0;
    //This is the difficulty
    int difficulty = 0;
    //This is the amount of the games play by session
    int numberOfAssays = 5;
    //This is the category that was selected
    int categorySelected = 0;
    //This is the amount of items Inside Every Category
    int objectsPerCategory = 3;
    int firstTime;
    int goodAnswer;
    int badAnswer;
    int missedAnswer;
    int passLevels;
    int repeatedLevels;
    int totalLevels = 54;
    int levelCategorizer;
    int miniKidLevel = 9;
    int maxiKidLevel = 27;
    int totalAnswers;//This is used to store the max amount of answers a player can input
    int initialLevel;
    int initialDifficulty;

    //Here we Store Every Object to be show in a particular time in the game
    int[] objectsToShow = new int[3];
    //Here we get the order in which the objects will be placed
    int[] orderOfObjects = new int[3];
    //This will keep track of wich categories have been used and avoid to used the same category in the same session
    List<int> categoriesToSelected = new List<int>();

    #endregion

    #region Text
    //here will save all the game strings
    TextAsset textAsset;
    //here will b ethe string procesed and ready to placed in the game
    string[] stringsToShow;

    #endregion

    #region floats

    //This is the time the object to identify is shown
    float timeToShow = 3.0f;
    //This is the time the objects to match are shown
    float timeToFind = 5.0f;
    //This is the time of a vapor
    float vaporTime = 1.0f;
    //This is the time the 
    float showResultTime = 3.0f;

    float latency; //Here we store temporaly th elatency of the answer later this will be store in the list named "latencies"

    float timer = 0.0f;
    float time;

    float rotationSpeed = 30.5f;

    Quaternion firstRotation;

    #endregion

    bool rotate = false;
    bool gameTime = false;
    bool winTheGame = false;
    bool answer = false;
    bool showTutorial = false;
    bool missAnswer = false;
    bool counting = false;

    PauseTheGame pauser;

    List<float> latencies = new List<float>();
    List<int> levelsPlayed = new List<int>();
    List<int> difficultiesPlayed = new List<int>();
    #endregion

    // Use this for initialization
    void Start ()
    {
        //We get need componets to make the game work
        sessionManager = FindObjectOfType<SessionManager>();
        levelSaver = GetComponent<LevelSaver>();
        audioManager = FindObjectOfType<AudioManager>();
        textAsset = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Games/LavaGame/LavaText");
        instructionsClips = Resources.LoadAll<AudioClip>($"{LanguagePicker.BasicAudioRoute()}Games/Lava");
        //Here we declare that the max amout of possible answers equals the numbers of assays
        totalAnswers = numberOfAssays;

        GetLevels();
        FillTheLists();

        firstRotation = buttonsOfObjects[0].transform.rotation;
        pauser = FindObjectOfType<PauseTheGame>();
        if (firstTime == 0)
        {
            ShowFirstTutorial();
            pauser.HideTutorialButtons();
        }
        else
        {
            instructionPanel.transform.parent.gameObject.SetActive(false);
            pauser.WantTutorial();
            pauser.howToPlayButton.onClick.AddListener(ShowFirstTutorial);
            pauser.playButton.onClick.AddListener(StartTheGame);
        }
	}

    void ShowFirstTutorial()
    {
        instructionPanel.transform.parent.gameObject.SetActive(true);
        showTutorial = true;
        sceneInstructions[instructionIndex].SetActive(true);
        instructionText.text = stringsToShow[instructionIndex];
        readyButton.gameObject.SetActive(false);
        audioManager.PlayClip(instructionsClips[instructionIndex]);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
        readyButton.onClick.AddListener(InstructionSet);
    }

	// Update is called once per frame
	void Update ()
    {
        sessionManager.UpdateTime();
        if (counting)
        {
            time += Time.deltaTime;
        }
        if (gameTime) {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                ToDo();
            }
            else
            {
                if (phase == GamePhase.EraserVapors)
                {
                    //Its in the phase of eraser vapors because the variable phase tells wich its the next phase and not the current one
                    latency += Time.deltaTime;
                }
                if (rotate)
                {
                    RotateTheShadows();
                }
            }
        }
	}

    void InstructionSet()
    {
        instructionIndex++;
        sceneInstructions[instructionIndex - 1].SetActive(false);
        sceneInstructions[instructionIndex].SetActive(true);
        instructionText.text = stringsToShow[instructionIndex];
        readyButton.gameObject.SetActive(false);
        audioManager.PlayClip(instructionsClips[instructionIndex]);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
        if (instructionIndex == sceneInstructions.Length - 1)
        {
            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(StartTheGame);
        }
    }

    #region Functions

    #region Set Up Functions

    void GetLevels()
    {
        if (sessionManager != null)
        {
            if (!FindObjectOfType<DemoKey>())
            {
                if (sessionManager.activeKid.firstsGames[(int)GameConfigurator.KindOfGame.Lava])
                {
                    FLISSetup();
                }
                else
                {
                    difficulty = sessionManager.activeKid.lavaDifficulty;
                    level = sessionManager.activeKid.lavaLevel;
                    levelsPlayed.Add(level);
                    difficultiesPlayed.Add(difficulty);
                    firstTime = 1;
                }
            }
            else
            {
                var key = FindObjectOfType<DemoKey>();
                if (key.IsFLISOn())
                {
                    sessionManager.activeKid.firstsGames[(int)GameConfigurator.KindOfGame.Lava] = true;
                    FLISSetup();
                }
                else
                {
                    sessionManager.activeKid.firstsGames[(int)GameConfigurator.KindOfGame.Lava] = false;
                    firstTime = 1;
                    if (key.IsLevelSetSpecially())
                    {
                        level = key.GetLevelA();
                        difficulty = key.GetLevelB();
                    }
                    else
                    {
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

            initialLevel = level;
            initialDifficulty = difficulty;
        }
        else
        {
            level = PlayerPrefs.GetInt(Keys.Lava_Level);
            difficulty = PlayerPrefs.GetInt(Keys.Lava_Difficulty);
            firstTime = PlayerPrefs.GetInt(Keys.Lava_First);
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

    void SaveLevels()
    {
        counting = false;
        if (sessionManager != null)
        {
            var startLevel = sessionManager.activeKid.lavaLevel;
            var startDifficulty = sessionManager.activeKid.lavaDifficulty;
            sessionManager.activeKid.lavaDifficulty = difficulty;
            sessionManager.activeKid.lavaLevel = level;
            sessionManager.activeKid.playedGames[(int)GameConfigurator.KindOfGame.Lava] = true;
            sessionManager.activeKid.needSync = true;
            sessionManager.activeKid.kiwis += passLevels;

            levelSaver.AddLevelData("level", difficulty);
            levelSaver.AddLevelData("sublevel", level);
            levelSaver.AddLevelData("shadow", "names");
            levelSaver.AddLevelData("shadowtime", "find");
            levelSaver.AddLevelData("numofoptions", 3);
            levelSaver.AddLevelData("options", "options that were show");
            levelSaver.AddLevelData("correct", goodAnswer);
            levelSaver.AddLevelData("time", (int)time);
            levelSaver.AddLevelData("latencies", latencies);
            //Version 2 
            sessionManager.activeKid.lavaSessions++;
            if (sessionManager.activeKid.firstsGames[(int)GameConfigurator.KindOfGame.Lava])
            {
                sessionManager.activeKid.firstsGames[(int)GameConfigurator.KindOfGame.Lava] = false;
                levelSaver.AddLevelData("initial_level", level);
                levelSaver.AddLevelData("initial_difficulty", difficulty);
            }
            else
            {
                levelSaver.AddLevelData("initial_level", startLevel);
                levelSaver.AddLevelData("initial_difficulty", startDifficulty);
            }
            levelSaver.AddLevelData("current_level", level);
            levelSaver.AddLevelData("current_difficulty", difficulty);
            levelSaver.AddLevelData("session_correct_percentage", goodAnswer * 20);
            levelSaver.AddLevelData("session_errors_percentage", badAnswer * 20);
            levelSaver.AddLevelData("session_miss_percentage", missedAnswer * 20);
            float totalLatencies = 0;
            foreach (float f in latencies)
            {
                totalLatencies += f;
            }
            totalLatencies /= totalAnswers;
            levelSaver.AddLevelData("latencies_percentage", totalLatencies);
            levelSaver.AddLevelData("correct_answers", goodAnswer);
            levelSaver.AddLevelData("bad_answers", badAnswer);
            levelSaver.AddLevelData("miss_answers", missedAnswer);
            levelSaver.AddLevelDataAsString("latencies", latencies);
            levelSaver.AddLevelDataAsString("played_levels", levelsPlayed);
            levelSaver.AddLevelDataAsString("played_difficulty", difficultiesPlayed);
            levelSaver.AddLevelData("session_time", (int)time);

            levelSaver.CreateSaveBlock("JuegoDeSombras", time, passLevels, repeatedLevels, passLevels+repeatedLevels, sessionManager.activeKid.lavaSessions);
            levelSaver.AddLevelsToBlock();
            levelSaver.PostProgress();
        }

        PlayerPrefs.SetInt(Keys.Lava_Level, level);
        PlayerPrefs.SetInt(Keys.Lava_Difficulty, difficulty);
        PlayerPrefs.SetInt(Keys.Lava_First, 1);
    }

    //This fill the require things to game be able to work properly
    void FillTheLists()
    {
        categories = new List<Sprite[]>() { category0, category1, category2, category3, category4, category5, category6 };

        numberOfCategories = (categories[categorySelected].Length / objectsPerCategory);

        for (int i = 0; i < particleManager.transform.childCount; i++)
        {
            vapors.Add(particleManager.transform.GetChild(i).GetComponent<ParticleSystem>());
        }

        for (int i = 0; i < numberOfCategories; i++)
        {
            categoriesToSelected.Add(i);
        }

        for (int i = 0; i < buttonCanvas.transform.childCount; i++)
        {
            buttonsOfObjects.Add(buttonCanvas.transform.GetChild(i).GetComponent<Button>());
        }

        for (int i = 0; i < imageOfResultsManager.transform.childCount; i++)
        {
            imagesOfResults.Add(imageOfResultsManager.transform.GetChild(i).GetComponent<Image>());
        }

        vaporTime = vapors[0].time;

        stringsToShow = TextReader.TextsToShow(textAsset);
    }

    //This will get the data to know if its a movement in the shodow part
    void GetData()
    {
        int movement = GameConfigurator.LavaGameConfig(level);
        switch (movement)
        {
            case 0:
                hideMovement = HideMovement.None;
                break;
            case 1:
                hideMovement = HideMovement.Switch;
                break;
            case 2:
                hideMovement = HideMovement.Rotation;
                break;
        }
    }

    //This will get the data needed for the game
    void SetTheGameParameters()
    {
        SetACategory();
        SetTheNumbersOfSprites();
        SelectTheObejectToFind();
        DisorderTheObjects();
        SetTheImagesAndButtons();
    }

    //Here we select a category and let it out for futere references in this session
    void SetACategory() {
        int randonCategory = Random.Range(0, categoriesToSelected.Count);
        categorySelected = categoriesToSelected[randonCategory];
        categoriesToSelected.Remove(categoriesToSelected[randonCategory]);
    }

    //Here will select the sprites to develop the game
    //we multiple the category slected for the objects per category to get a start point which will be very helpful in set of sprites
    void SetTheNumbersOfSprites()
    {
        int startPoint = categorySelected * objectsPerCategory;
        for (int i = 0; i < objectsToShow.Length; i++)
        {
            objectsToShow[i] = startPoint + i;
        }
    }

    //Here we select a random object of the category to match later in the shadows
    void SelectTheObejectToFind()
    {
        int randomObjectIndex = Random.Range(0, objectsPerCategory);
        objectToFind = objectsToShow[randomObjectIndex];
    }

    //Here we put a randomnes in the order of the object to show
    void DisorderTheObjects()
    {
        List<int> randomOrder = new List<int> { 0, 1, 2 };
        for (int i = 0; i < orderOfObjects.Length; i++)
        {
            int randomize = Random.Range(0, randomOrder.Count);
            orderOfObjects[randomOrder[randomize]] = objectsToShow[i];
            randomOrder.Remove(randomOrder[randomize]);
        }
    }

    //Here we put the image and the interactions in the correspondant objects
    void SetTheImagesAndButtons()
    {
        imageToIdentify.sprite = categories[difficulty][objectToFind];
        imagesOfResults[0].sprite = categories[difficulty][objectToFind];
        for (int i = 0; i < buttonsOfObjects.Count; i++)
        {
            buttonsOfObjects[i].onClick.RemoveAllListeners();
            int index = buttonsOfObjects[i].transform.GetSiblingIndex();
            buttonsOfObjects[i].onClick.AddListener(()=> CompareTheNumber(index));
            buttonsOfObjects[i].GetComponent<Image>().sprite = categories[difficulty][orderOfObjects[i]];
        }
    }

    #endregion

    #region Game Order Functions
    //This is the start set up of the game
    void StartTheGame()
    {
        if (!showTutorial)
        {
            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(StartTheGame);
        }
        SetTheCorrectScale();
        GetData();
        SetTheGameParameters();
        instructionPanel.SetActive(false);
        gameTime = true;
        answer = false;
        missAnswer = false;
        counting = true;
        phase = GamePhase.ShowObject;
        timer = vaporTime;
        PlayTheVapors(0);
        if (showTutorial)
        {
            sceneInstructions[sceneInstructions.Length - 1].SetActive(false);
        }
        else
        {
            instructionPanel.transform.parent.gameObject.SetActive(true);
        }
    }

    //This is the thing that you should do
    void ToDo() {
        switch (phase) {
            case GamePhase.ShowObject:
                if (!vapors[0].isPlaying) {
                    ShowTheObject();
                    timer = timeToShow;
                    phase = GamePhase.ShowVapor;
                }
                break;
            case GamePhase.ShowVapor:
                PlayTheVapors(0, 3);
                HideAll();
                timer = vaporTime;
                phase = GamePhase.ShowShadows;
                break;
            case GamePhase.ShowShadows:
                if (!vapors[0].isPlaying)
                {
                    ShowTheShadows();
                    timer = timeToFind;
                    phase = GamePhase.EraserVapors;
                }
                break;
            case GamePhase.EraserVapors:
                if (!answer)
                {
                    imagesOfResults[1].sprite = categories[difficulty][objectToFind];
                    winTheGame = false;
                    missAnswer = true;
                    latencies.Add(3f);
                    missedAnswer++;
                }
                else
                {
                    latencies.Add(latency);
                }
                latency = 0;
                rotate = false;
                PlayTheVapors(0, 3);
                HideAll();
                timer = vaporTime;
                phase = GamePhase.ShowResults;
                break;
            case GamePhase.ShowResults:
                if (!vapors[0].isPlaying) {
                    ShowTheResults();
                    timer = showResultTime;
                    phase = GamePhase.AnswerVapor;
                }
                break;
            case GamePhase.AnswerVapor:
                HideAll();
                PlayTheVapors();
                timer = vaporTime;
                gameTime = false;
                Invoke("HandleTheAnswer", 2f);
                break;

        }
    }

    #endregion

    #region Game Mechanic Functions

    //This show the object to show
    void ShowTheObject()
    {
        imageToIdentify.gameObject.SetActive(true);
    }

    //This is the one that shows everything
    void ShowTheShadows()
    {
        buttonCanvas.SetActive(true);
        ShadowsMovement();
    }

    //This is the One that shows the results
    void ShowTheResults()
    {
        imageOfResultsManager.SetActive(true);
    }

    //This will determine if a shadow shoud move or not?
    void ShadowsMovement()
    {
        if (hideMovement == HideMovement.Rotation)
        {
            rotate = true;
        }
        else if (hideMovement == HideMovement.Switch)
        {
            for (int i = 0; i < buttonsOfObjects.Count; i++)
            {
                buttonsOfObjects[i].GetComponent<RectTransform>().localScale = new Vector3(-1f, 1f, 1f);
            }
        }
    }

    //This script is the one thata will change the rotation
    void RotateTheShadows()
    {
        for (int i = 0; i < buttonsOfObjects.Count; i++)
        {
            buttonsOfObjects[i].GetComponent<RectTransform>().Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
    }

    //This will hide everything
    void HideAll()
    {
        imageToIdentify.gameObject.SetActive(false);
        imageOfResultsManager.SetActive(false);
        buttonCanvas.SetActive(false);
    }

    //This will play the vapor o the color according to the answer
    void PlayTheVapors()
    {
        if (winTheGame)
        {
            PlayTheVapors(3);
        }
        else
        {
            PlayTheVapors(4);
        }
    }

    //This will play an especific selected vapor
    void PlayTheVapors(int vaporNumber)
    {
        vapors[vaporNumber].Play();
    }

    //This will play a range of vapors
    void PlayTheVapors(int firtstVapor, int numberOfVapors)
    {
        for (int i = firtstVapor; i < numberOfVapors; i++)
        {
            vapors[i].Play();
        }
    }

    //This is the function that will handle what happed if the answer is right or worng
    void HandleTheAnswer()
    {
        if (winTheGame)
        {
            LevelUp();
        }
        else
        {
            LevelDown();
        }
        StartNewGame();
    }

    //This one will put a level up
    void LevelUp()
    {
        level++;
        passLevels++;
        if (level >= 9)
        {
            level = 0;
            difficulty++;
            if (difficulty == categories.Count)
            {
                level = 8;
                difficulty = categories.Count - 1;
            }
        }
    }

    //This one will drop a level up
    void LevelDown()
    {
        level--;
        repeatedLevels++;
        if (level < 0)
        {
            level = 9;
            difficulty--;
            if (difficulty < 0)
            {
                difficulty = 0;
                level = 0;
            }
        }
    }

    //This will start the new game
    void StartNewGame()
    {
        numberOfAssays--;
        if (sessionManager.activeKid.firstsGames[(int)GameConfigurator.KindOfGame.Lava])
        {
            if (winTheGame)
            {
                levelCategorizer += LevelDifficultyChange(totalLevels);
                levelCategorizer = Mathf.Clamp(levelCategorizer, 0, totalLevels - 1);
                GetDataJustForLevel(levelCategorizer);
            }
            else
            {
                levelCategorizer -= LevelDifficultyChange(totalLevels);
                levelCategorizer = Mathf.Clamp(levelCategorizer, 0, totalLevels - 1);
                GetDataJustForLevel(levelCategorizer);
            }
        }
        string stringChoose;
        if (winTheGame)
        {
            if (numberOfAssays > 0)
            {
                stringChoose = stringsToShow[7] + "\n" + stringsToShow[6];
                audioManager.PlayClip(instructionsClips[7], instructionsClips[6]);
            }
            else
            {
                stringChoose = stringsToShow[7];
                audioManager.PlayClip(instructionsClips[7]);
            }

            readyButton.gameObject.SetActive(false);
            Invoke("ReadyButtonOn", audioManager.ClipDuration());
        }
        else
        {
            if (missAnswer)
            {
                if (numberOfAssays > 0)
                {
                    stringChoose = stringsToShow[9] + "\n" + stringsToShow[6];
                    audioManager.PlayClip(instructionsClips[9], instructionsClips[6]);
                }
                else
                {
                    stringChoose = stringsToShow[9];
                    audioManager.PlayClip(instructionsClips[9]);
                }
            }
            else
            {
                if (numberOfAssays > 0)
                {
                    stringChoose = stringsToShow[8] + "\n" + stringsToShow[6];
                    audioManager.PlayClip(instructionsClips[8], instructionsClips[6]);
                }
                else
                {
                    stringChoose = stringsToShow[8];
                    audioManager.PlayClip(instructionsClips[8]);
                }
            }
        }

        readyButton.gameObject.SetActive(false);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
        instructionText.text = stringChoose;
        instructionPanel.SetActive(true);

        if (numberOfAssays <= 0)
        {
            Debug.Log(levelsPlayed.Count);
            levelsPlayed.Add(level);
            difficultiesPlayed.Add(difficulty);
            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(FinishTheGame);
        }
        else
        {
            Debug.Log(levelsPlayed.Count);
            levelsPlayed.Add(level);
            difficultiesPlayed.Add(difficulty);
        }
    }

    void FinishTheGame()
    {
        SaveLevels();
        instructionText.text = stringsToShow[5];
        readyButton.gameObject.SetActive(false);
        audioManager.PlayClip(instructionsClips[5]);
        readyButton.onClick.RemoveAllListeners();
        instructionPanel.SetActive(true);
        Invoke("ShowEarnings", audioManager.ClipDuration());
    }

    void ShowEarnings()
    {   
        instructionPanel.gameObject.SetActive(false);
        pauser.ShowKiwiEarnings(passLevels);
    }

    //This will put the correct scale of the object
    void SetTheCorrectScale()
    {
        for (int i = 0; i < buttonsOfObjects.Count; i++)
        {
            buttonsOfObjects[i].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            buttonsOfObjects[i].transform.rotation = firstRotation;
        }
    }

    void GoBack()
    {
        SceneManager.LoadScene("GameCenter");
    }

    //This will compare the data stored to see if its right or wrong
    void CompareTheNumber(int index)
    {
        answer = true;
        missAnswer = false;
        PlayTheVapors(0, 3);
        if (orderOfObjects[index] == objectToFind)
        {
            winTheGame = true;
            goodAnswer++;
        }
        else
        {
            winTheGame = false;
            badAnswer++;
        }
        imagesOfResults[1].sprite = categories[difficulty][orderOfObjects[index]];
        ToDo();
    }

    void ReadyButtonOn()
    {
        readyButton.gameObject.SetActive(true);
    }

    #endregion

    void GetDataJustForLevel(int levelInput)
    {
        int amountOfDifficulties = 6;
        int baseLevelDifficulty = 9;

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

    #endregion
}
