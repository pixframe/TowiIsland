using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SandDrawingController : MonoBehaviour {

    DrawCanvas centerCanvas;
    DrawCanvas[] multiples;
    SandDrawer drawer;
    SwipeTrail trailer;
    SessionManager sessionManager;

    enum TypeOfGame { Fill, Completion, Identify};
    TypeOfGame typeOfGameToPlay;

    public ParticleSystem stars;
    public GameObject drawerController;
    public GameObject dotCollector;
    public GameObject brush;
    public GameObject ocean;
    public GameObject dot;
    public Color sandColor;
    public Camera answerCam;
    public Camera drawCam;
    GameObject brushTrail;

    [Header("UI Elements")]
    public GameObject storyManager;
    public GameObject instructionPanel;
    public Text instructionText;
    public Button readyButton;
    public Button endButton;

    [Header("Text Elements")]
    public TextAsset textAsset;
    string[] stringsToShow;
    
    GameObject[] stories;

    int indexStory;
    int assayIndex;
    int levelGame = 14;
    int levelFill = 20;
    int levelCompletion = 11;
    int levelIdentyfy = 11;
    int initialLevelFill;
    int initialLevelCompletion;
    int initialLevelIdentify;
    int maxNumberOfAssays = 5;
    int totalAssaysInTheGame;
    int blackToFill = 0;
    int drawFull = 0;
    int w = Screen.width;
    int h = Screen.width;
    int firstTime;
    int passLevels;
    int repeatedLevels;

    const int totalLevelsNormal = 30;
    const int totalSpecialLevels = 15;
    const int miniKidLevel = 10;
    const int maxiKidLevel = 20;
    const int miniKidLevelSpecials = 5;
    const int maxiKidLevelSpecials = 10;

    List<int> choosen = new List<int>();
    List<int> levelsPlayed = new List<int>();
    List<float> accuracies = new List<float>();
    List<float> overdraws = new List<float>();
    List<float> drawingTimes = new List<float>();

    int[] typeOfGamesIndex;

    float time;
    float accuracyPercentage;
    float drawTime;

    bool dragTime;
    bool calificationTime;
    bool playGame;
    bool createTrail;
    bool showStar;
    bool reapeatExercise;
    bool secondTry;
    bool newStimulus;
    bool showTutorial;
    bool countTime = false;

    Color[] aColors;
    Color[] bColors;

    PauseTheGame pauser;

    AudioManager audioManager;
    AudioClip[] instructionsClips;

    // Use this for initialization
    void Start ()
    {
        if (FindObjectOfType<SessionManager>())
        {
            sessionManager = FindObjectOfType<SessionManager>();
        }
        audioManager = FindObjectOfType<AudioManager>();
        textAsset = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Games/Sand/SandText");
        instructionsClips = Resources.LoadAll<AudioClip>($"{LanguagePicker.BasicAudioRoute()}Games/Sand");
        GetLevels();
        GetTheData();
		w = Screen.width;
        h = Screen.height;
        drawer = drawerController.GetComponent<SandDrawer>();
        pauser = FindObjectOfType<PauseTheGame>();
        centerCanvas = new DrawCanvas(transform.GetChild(0).gameObject);
        Transform t = transform.GetChild(1);
        multiples = new DrawCanvas[t.childCount];
        for (int i = 0; i < multiples.Length; i++)
        {
            multiples[i] = new DrawCanvas(t.GetChild(i).gameObject);
        }
        endButton.gameObject.SetActive(false);
        trailer = FindObjectOfType<SwipeTrail>();
        stringsToShow = TextReader.TextsToShow(textAsset);
        playGame = false;
        stories = new GameObject[storyManager.transform.childCount];
        for (int i = 0; i < storyManager.transform.childCount; i++)
        {
            stories[i] = storyManager.transform.GetChild(i).gameObject;
        }
        createTrail = true;
        SetTheNextGame();
        if (firstTime == 0)
        {
            TellAStory();
            pauser.HideTutorialButtons();
        }
        else
        {
            instructionPanel.transform.parent.gameObject.SetActive(false);
            pauser.WantTutorial();
            pauser.howToPlayButton.onClick.AddListener(TellAStory);
            pauser.playButton.onClick.AddListener(TellInstruction);
            pauser.playButton.onClick.AddListener(DestroyTheStory);
        }
        endButton.onClick.AddListener(CalificateTheDraw);

        endButton.GetComponentInChildren<Text>().text = TextReader.commonStrings[2];

        EventTrigger trigger = endButton.GetComponent<EventTrigger>();
        EventTrigger.Entry en = new EventTrigger.Entry();
        en.eventID = EventTriggerType.PointerDown;
        en.callback.AddListener((data) => { DontDraw((PointerEventData)data); });
        trigger.triggers.Add(en);
	}

    void GetLevels()
    {
        totalAssaysInTheGame = maxNumberOfAssays;
        if (sessionManager != null)
        {
            if (!FindObjectOfType<DemoKey>())
            {
                if (sessionManager.activeKid.sandFirst)
                {
                    FLISSetup();
                }
                else
                {
                    if (!sessionManager.activeKid.sandLevelSet)
                    {
                        if (sessionManager.activeKid.age < 7)
                        {
                            levelIdentyfy = miniKidLevelSpecials;
                            levelCompletion = miniKidLevelSpecials;
                        }
                        else if (sessionManager.activeKid.age > 9)
                        {
                            levelIdentyfy = maxiKidLevelSpecials;
                            levelCompletion = maxiKidLevelSpecials;
                        }
                        else
                        {
                            levelIdentyfy = LevelDifficultyChange(totalSpecialLevels, AssaysOfHabilityToEvaluate(assayIndex));
                            levelCompletion = LevelDifficultyChange(totalSpecialLevels, AssaysOfHabilityToEvaluate(assayIndex));
                        }
                        maxNumberOfAssays = 6;
                    }
                    else
                    {
                        levelGame = sessionManager.activeKid.sandDifficulty;
                        levelFill = sessionManager.activeKid.sandLevel;
                        levelIdentyfy = sessionManager.activeKid.sandLevel2;
                        levelCompletion = sessionManager.activeKid.sandLevel3;
                        initialLevelFill = levelFill;
                        initialLevelIdentify = levelIdentyfy;
                        initialLevelCompletion = levelCompletion;
                    }
                    firstTime = 1;
                }
            }
            else
            {
                var key = FindObjectOfType<DemoKey>();
                if (key.IsFLISOn())
                {
                    sessionManager.activeKid.sandFirst = true;
                    sessionManager.activeKid.sandLevelSet = false;
                    FLISSetup();
                }
                else
                {
                    sessionManager.activeKid.sandFirst = false;
                    sessionManager.activeKid.sandLevelSet = true;
                    firstTime = 1;
                    if (key.IsLevelSetSpecially())
                    {
                        levelGame = 9;
                        levelFill = key.GetLevelA();
                        levelIdentyfy = key.GetLevelB();
                        levelCompletion = key.GetLevelC();
                    }
                    else
                    {
                        switch (key.GetDifficulty())
                        {
                            case 0:
                                levelGame = 9;
                                levelFill = 0;
                                levelIdentyfy = 0;
                                levelCompletion = 0;
                                break;
                            case 1:
                                levelGame = 9;
                                levelFill = totalLevelsNormal / 2;
                                levelIdentyfy = totalSpecialLevels / 2;
                                levelCompletion = totalSpecialLevels / 2;
                                break;
                            case 2:
                                levelGame = 10;
                                levelFill = totalLevelsNormal - 5;
                                levelIdentyfy = totalSpecialLevels - 3;
                                levelCompletion = totalSpecialLevels - 3;
                                break;
                        }
                    }
                }
            }
        }
        else
        {
            levelGame = PlayerPrefs.GetInt(Keys.Sand_General_Level_Int);
            levelFill = PlayerPrefs.GetInt(Keys.Sand_Fill_Level_Int);
            levelIdentyfy = PlayerPrefs.GetInt(Keys.Sand_Identify_Level_Int);
            levelCompletion = PlayerPrefs.GetInt(Keys.Sand_Complete_Level_Int);
            firstTime = PlayerPrefs.GetInt(Keys.Sand_First);
        }

    }

    void FLISSetup()
    {
        if (sessionManager.activeKid.age < 7)
        {
            levelFill = miniKidLevel;
        }
        else if (sessionManager.activeKid.age > 9)
        {
            levelFill = maxiKidLevel;
        }
        else
        {
            levelFill = LevelDifficultyChange(totalLevelsNormal, assayIndex);
        }
        firstTime = 0;
    }

    void SaveLevels()
    {
        var levelSaver = FindObjectOfType<LevelSaver>();
        countTime = true;
        if (sessionManager != null)
        {
            accuracyPercentage = (accuracyPercentage * 100) / 500;

            var level1 = sessionManager.activeKid.sandLevel;
            var level2 = sessionManager.activeKid.sandLevel2;
            var level3 = sessionManager.activeKid.sandLevel3;
            sessionManager.activeKid.sandDifficulty = levelGame;
            sessionManager.activeKid.sandLevel = levelFill;
            sessionManager.activeKid.sandLevel2 = levelIdentyfy;
            sessionManager.activeKid.sandLevel3 = levelCompletion;
            sessionManager.activeKid.kiwis += passLevels;
            sessionManager.activeKid.playedGames[(int)GameConfigurator.KindOfGame.Sand] = true;
            sessionManager.activeKid.needSync = true;

            //levelSaver.AddLevelData("level", levelGame);
            //levelSaver.AddLevelData("sublevel", levelFill);
            //levelSaver.AddLevelData("sublevel", levelIdentyfy);
            //levelSaver.AddLevelData("sublevel", levelCompletion);
            //levelSaver.AddLevelData("time", time);
            //levelSaver.AddLevelData("passed", passLevels);
            //levelSaver.AddLevelData("repeated", repeatedLevels);
            //levelSaver.AddLevelData("accuracy", accuracyPercentage);

            //Version 2
            sessionManager.activeKid.sandSessions++;
            float overTotals = 0;
            foreach(float f in overdraws)
            {
                overTotals += f;
            }
            float acuTotals = 0;
            foreach (float f in accuracies)
            {
                acuTotals += f;
            }
            levelSaver.AddLevelData("session_overdraw_percentage", overTotals / totalAssaysInTheGame);
            levelSaver.AddLevelData("session_accuracy_percentage", acuTotals / totalAssaysInTheGame);
            if (sessionManager.activeKid.sandFirst)
            {
                sessionManager.activeKid.sandFirst = false;
                levelSaver.AddLevelData("initial_level_motor", levelFill);
            }
            else
            {
                if (!sessionManager.activeKid.sandLevelSet)
                {
                    sessionManager.activeKid.sandLevelSet = true;
                    levelSaver.AddLevelData("initial_level_overlapping", levelIdentyfy);
                    levelSaver.AddLevelData("initial_level_clousre", levelCompletion);
                }
                else
                {
                    levelSaver.AddLevelData("initial_level_motor", level1);
                    levelSaver.AddLevelData("initial_level_overlapping", level2);
                    levelSaver.AddLevelData("initial_level_clousre", level3);
                }
            }
            levelSaver.AddLevelDataAsString("time_percentage", drawingTimes);
            levelSaver.AddLevelDataAsString("types_levels", typeOfGamesIndex);
            levelSaver.AddLevelDataAsString("played_levels", levelsPlayed);
            levelSaver.AddLevelData("current_level", levelGame);
            levelSaver.AddLevelData("current_level_motor", levelFill);
            levelSaver.AddLevelData("current_level_overlapping", levelIdentyfy);
            levelSaver.AddLevelData("current_level_clousre", levelCompletion);
            levelSaver.AddLevelData("change_level_motor", levelFill - initialLevelFill);
            levelSaver.AddLevelData("change_level_overlapping", levelIdentyfy - initialLevelIdentify);
            levelSaver.AddLevelData("change_level_clousure", levelCompletion - initialLevelCompletion);
            levelSaver.AddLevelData("session_number", levelGame);
            levelSaver.AddLevelDataAsString("accuracy", accuracies);
            levelSaver.AddLevelDataAsString("overdraw", overdraws);
            levelSaver.CreateSaveBlock("ArenaMagica", time, passLevels, repeatedLevels, totalAssaysInTheGame, sessionManager.activeKid.sandSessions);
            levelSaver.AddLevelsToBlock();
            levelSaver.PostProgress();
        }
        else
        {
            PlayerPrefs.SetInt(Keys.Sand_General_Level_Int, levelGame);
            PlayerPrefs.SetInt(Keys.Sand_Fill_Level_Int, levelFill);
            PlayerPrefs.SetInt(Keys.Sand_Identify_Level_Int, levelIdentyfy);
            PlayerPrefs.SetInt(Keys.Sand_Complete_Level_Int, levelCompletion);
            PlayerPrefs.SetInt(Keys.Sand_First, 1);
        }
    }

    void DontDraw(PointerEventData data)
    {
        playGame = false;
    }

    void TellAStory()
    {
        instructionPanel.transform.parent.gameObject.SetActive(true);
        showTutorial = true;
        stories[indexStory].gameObject.SetActive(true);
        instructionText.text = stringsToShow[indexStory];
        readyButton.gameObject.SetActive(false);
        audioManager.PlayClip(instructionsClips[indexStory]);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
        readyButton.onClick.RemoveAllListeners();
        if (indexStory > 0)
        {
            stories[indexStory - 1].SetActive(false);
        }
        indexStory++;
        if (indexStory < stories.Length)
        {
            readyButton.onClick.AddListener(TellAStory);
        }
        else
        {
            readyButton.onClick.AddListener(TellInstruction);
        }
    }

    void DestroyTheStory() 
    {
        if (showTutorial)
        {
            stories[indexStory - 1].SetActive(false);
        }
        else
        {
            instructionPanel.transform.parent.gameObject.SetActive(true);
        }
        stories = null;
    }

    void TellInstruction()
    {
        countTime = true;

        switch (typeOfGameToPlay)
        {
            case TypeOfGame.Fill:
                instructionText.text = stringsToShow[3];
                readyButton.gameObject.SetActive(false);
                audioManager.PlayClip(instructionsClips[3]);
                Invoke("ReadyButtonOn", audioManager.ClipDuration());
                break;
            case TypeOfGame.Identify:
                instructionText.text = stringsToShow[4];
                readyButton.gameObject.SetActive(false);
                audioManager.PlayClip(instructionsClips[4]);
                Invoke("ReadyButtonOn", audioManager.ClipDuration());
                centerCanvas.visibleRenderer.gameObject.SetActive(true);
                break;
            case TypeOfGame.Completion:
                if (levelCompletion < 8)
                {
                    instructionText.text = stringsToShow[5];
                    readyButton.gameObject.SetActive(false);
                    audioManager.PlayClip(instructionsClips[5]);
                    Invoke("ReadyButtonOn", audioManager.ClipDuration());
                }
                else
                {
                    instructionText.text = stringsToShow[6];
                    readyButton.gameObject.SetActive(false);
                    audioManager.PlayClip(instructionsClips[6]);
                    Invoke("ReadyButtonOn", audioManager.ClipDuration());
                }
                break;
        }
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(PlayTheGame);
    }

	// Update is called once per frame
	void Update ()
    {
        if (countTime)
        {
            time += Time.deltaTime;
        }
        if (playGame)
        {
            drawTime += Time.deltaTime;
            if (Input.GetMouseButtonDown(0))
            {
                if (createTrail)
                {
                    brushTrail = Instantiate(brush, drawer.transform);
                    createTrail = false;
                }
                DragTheDrawer();
                drawer.StartDrawing();
                dragTime = true;
            }
            if (dragTime)
            {
                DragTheDrawer();
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (!createTrail)
                {
                    brushTrail.GetComponent<SwipeTrail>().StopMoving();
                    brushTrail.transform.SetParent(dotCollector.transform);
                }
                drawer.StopDrawing();
                createTrail = true;
            }
        }
	}

    void PlayTheGame()
    {
        playGame = true;
        instructionPanel.SetActive(false);
        endButton.gameObject.SetActive(true);
        if (typeOfGameToPlay == TypeOfGame.Identify)
        {
            foreach (DrawCanvas c in multiples)
            {
                c.canvas.SetActive(true);
                c.visibleRenderer.gameObject.SetActive(true);
            }
            centerCanvas.visibleRenderer.gameObject.SetActive(false);
            centerCanvas.nonVisibleRenderer.gameObject.SetActive(false);
        }
        else
        {
            foreach (DrawCanvas c in multiples)
            {
                c.canvas.SetActive(false);
            }
            centerCanvas.visibleRenderer.gameObject.SetActive(true);
            centerCanvas.nonVisibleRenderer.gameObject.SetActive(true);
        }

        ReadAPixels();
    }

    void ReadAPixels()
    {
        var tex = new RenderTexture(w, h, 24);

        answerCam.targetTexture = tex;
        answerCam.Render();
        RenderTexture.active = tex;
        Texture2D a = new Texture2D(w, h);
        a.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        a.Apply();

        aColors = a.GetPixels();

        RenderTexture.active = null;
        tex.Release();

        for (int i = 0; i < aColors.Length; i++)
        {
            if (aColors[i].a > 0)
            {
                aColors[i] = Color.black;
                blackToFill++;
            }
        }
    }

    void GetTheData()
    {
        if (!sessionManager.activeKid.sandFirst && sessionManager.activeKid.sandLevelSet)
        {
            typeOfGamesIndex = GameConfigurator.SandConfig(levelGame);
        }
        else
        {
            if (!sessionManager.activeKid.sandFirst)
            {
                typeOfGamesIndex = new int[] { 1, 2, 1, 2, 1, 2 };
            }
            else
            {
                typeOfGamesIndex = new int[] { 0, 0, 0, 0, 0 };
            }
        }
    }

    void SetTheNextGame()
    {
        int gameIndex = typeOfGamesIndex[assayIndex];

        if (gameIndex == 4)
        {
            gameIndex = LookTheDeficit();
        }

        SetGameType(gameIndex);

        if (assayIndex > 0 && reapeatExercise)
        {
            if (gameIndex == typeOfGamesIndex[assayIndex - 1])
            {
                if (newStimulus)
                {
                    secondTry = false;
                    newStimulus = false;
                }
                else
                {
                    secondTry = true;
                }
            }
            else
            {
                secondTry = false;
                newStimulus = false;
            }
        }

        SetGameStuff();
    }

    int LookTheDeficit()
    {
        int fillLevelAdjusted = 0;
        if (levelFill % 2 == 0)
        {
            fillLevelAdjusted = levelFill / 2;
        }
        else
        {
            fillLevelAdjusted = (levelFill - 1) / 2;
        }

        if (fillLevelAdjusted == levelIdentyfy)
        {
            if (fillLevelAdjusted == levelCompletion)
            {
                return 3;
            }
            else if (levelCompletion > fillLevelAdjusted)
            {
                return Random.Range(0, 2);
            }
            else
            {
                return 2;
            }
        }
        else if (fillLevelAdjusted < levelIdentyfy)
        {
            if (fillLevelAdjusted == levelCompletion)
            {
                int selectDeficitRandom = Random.Range(0, 2);
                int[] deficit = { 0, 2 };
                return deficit[selectDeficitRandom];
            }
            else if (levelCompletion > fillLevelAdjusted)
            {
                return 0;
            }
            else
            {
                return 2;
            }
        }
        else
        {
            if (levelIdentyfy == levelCompletion)
            {
                int selectDeficitRandom = Random.Range(1, 3);
                return selectDeficitRandom;
            }
            else if (levelCompletion > levelIdentyfy)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
    }

    void SetGameType(int gameData)
    {
        if (gameData == 3)
        {
            gameData = Random.Range(0, 3);
        }

        switch (gameData)
        {
            case 0:
                typeOfGameToPlay = TypeOfGame.Fill;
                break;
            case 1:
                typeOfGameToPlay = TypeOfGame.Identify;
                break;
            case 2:
                typeOfGameToPlay = TypeOfGame.Completion;
                break;
        }
        
    }

    Sprite[] GetProbableSprite(string path) 
    {
        return Resources.LoadAll<Sprite>($"Sand/{path}");
    }

    List<int> ProbableIndex(int lenghtOfArray) 
    {
        var listToReturn = Enumerable.Range(0, lenghtOfArray).ToList();
        listToReturn.RemoveAll(a => choosen.Contains(a));
        return listToReturn;
    }

    void SetGameStuff()
    {
        if (reapeatExercise && secondTry)
        {
            secondTry = false;
            newStimulus = true;
        }
        else
        {
            Sprite bases;
            Sprite shower;

            Sprite[] probableSprites;

            int randy = 0;
            List<int> temporals = new List<int>();

            switch (typeOfGameToPlay)
            {
                case TypeOfGame.Fill:
                    levelsPlayed.Add(levelFill);
                    if (levelFill < 10)
                    {
                        probableSprites = GetProbableSprite("EasyBank");
                    }
                    else if (levelFill >= 10 && levelFill < 20)
                    {
                        probableSprites = GetProbableSprite("MiddleBank");
                    }
                    else
                    {
                        probableSprites = GetProbableSprite("HardBank");
                    }

                    temporals = ProbableIndex(probableSprites.Length);
                    randy = Random.Range(0, temporals.Count);
                    choosen.Add(temporals[randy]);
                    shower = probableSprites[temporals[randy]];
                    bases = probableSprites[temporals[randy]];

                    break;
                case TypeOfGame.Completion:
                    Sprite[] keySprites;
                    levelsPlayed.Add(levelCompletion);
                    if (levelCompletion < 3)
                    {
                        probableSprites = GetProbableSprite("EasyComplete");
                        keySprites = GetProbableSprite("EasyBank");
                        temporals = ProbableIndex(probableSprites.Length);

                        randy = Random.Range(0, temporals.Count);
                        choosen.Add(temporals[randy]);
                        shower = probableSprites[temporals[randy]];
                        bases = keySprites[temporals[randy]];
                    }
                    else if (levelCompletion >= 3 && levelCompletion < 5)
                    {
                        probableSprites = GetProbableSprite("MiddleComplete");
                        keySprites = GetProbableSprite("MiddleBank");
                        temporals = ProbableIndex(probableSprites.Length);

                        randy = Random.Range(0, temporals.Count);
                        choosen.Add(temporals[randy]);
                        shower = probableSprites[temporals[randy]];
                        bases = keySprites[temporals[randy]];
                    }
                    else if (levelCompletion >= 5 && levelCompletion < 8)
                    {
                        probableSprites = GetProbableSprite("HardComplete");
                        keySprites = GetProbableSprite("HardBank");
                        temporals = ProbableIndex(probableSprites.Length);

                        randy = Random.Range(0, temporals.Count);
                        choosen.Add(temporals[randy]);
                        shower = probableSprites[temporals[randy]];
                        bases = keySprites[temporals[randy]];
                    }
                    else if (levelCompletion >= 8 && levelCompletion < 10)
                    {
                        probableSprites = GetProbableSprite("EasyHalf");
                        temporals = ProbableIndex(probableSprites.Length);
                        randy = Random.Range(0, temporals.Count);
                        choosen.Add(temporals[randy]);
                        string path = probableSprites[temporals[randy]].name.Substring(0, probableSprites[temporals[randy]].name.Length - 3) + "_01";
                        Sprite spi = Resources.Load<Sprite>("Sand/EasyBank/" + path);
                        shower = probableSprites[temporals[randy]];
                        bases = spi;
                    }
                    else if (levelCompletion >= 10 && levelCompletion < 12)
                    {
                        probableSprites = GetProbableSprite("MiddleHalf");
                        temporals = ProbableIndex(probableSprites.Length);
                        randy = Random.Range(0, temporals.Count);
                        choosen.Add(temporals[randy]);
                        string path = probableSprites[temporals[randy]].name.Substring(0, probableSprites[temporals[randy]].name.Length - 3) + "_01";
                        Sprite spi = Resources.Load<Sprite>("Sand/MiddleBank/" + path);
                        shower = probableSprites[temporals[randy]];
                        bases = spi;
                    }
                    else
                    {
                        probableSprites = GetProbableSprite("HardHalf");
                        temporals = ProbableIndex(probableSprites.Length);
                        randy = Random.Range(0, temporals.Count);
                        choosen.Add(temporals[randy]);
                        string path = probableSprites[temporals[randy]].name.Substring(0, probableSprites[temporals[randy]].name.Length - 3) + "_01";
                        Sprite spi = Resources.Load<Sprite>("Sand/HardBank/" + path);
                        shower = probableSprites[temporals[randy]];
                        bases = spi;
                    }
                    break;
                case TypeOfGame.Identify:
                    levelsPlayed.Add(levelIdentyfy);
                    int[] spis = new int[multiples.Length];
                    List<int> temp = new List<int>();

                    if (levelIdentyfy < 5)
                    {
                        probableSprites = GetProbableSprite("EasyBank");
                    }
                    else if (levelIdentyfy >= 5 && levelIdentyfy < 10)
                    {
                        probableSprites = GetProbableSprite("MiddleBank");
                    }
                    else
                    {
                        probableSprites = GetProbableSprite("HardBank");
                    }
                    temporals = ProbableIndex(probableSprites.Length);
                    temp.AddRange(Enumerable.Range(0, probableSprites.Length));
                    for (int i = 0; i < spis.Length; i++)
                    {
                        int ran = Random.Range(0, temp.Count);
                        spis[i] = temp[ran];
                        temp.Remove(temp[ran]);
                        multiples[i].visibleRenderer.sprite = probableSprites[spis[i]];
                        multiples[i].nonVisibleRenderer.sprite = probableSprites[spis[i]];
                        multiples[i].nonVisibleRenderer.gameObject.SetActive(false);
                    }

                    randy = Random.Range(0, spis.Length);
                    choosen.Add(randy);
                    shower = probableSprites[spis[randy]];
                    bases = probableSprites[spis[randy]];
                    multiples[randy].nonVisibleRenderer.gameObject.SetActive(true);

                    break;
				default:
					shower = centerCanvas.visibleRenderer.sprite;
					bases = centerCanvas.visibleRenderer.sprite;
					break;
            }
            centerCanvas.visibleRenderer.sprite = shower;
            centerCanvas.nonVisibleRenderer.sprite = bases;
            probableSprites = null;
        }
        showStar = false;
    }

    void DragTheDrawer() {
        Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newPos.z = drawer.transform.position.z;
        drawer.transform.position = newPos;
    }

    void ReadBPixels()
    {
        var tex2 = new RenderTexture(w, h, 24);

        drawCam.targetTexture = tex2;
        drawCam.Render();
        RenderTexture.active = tex2;
        Texture2D b = new Texture2D(w, h);
        b.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        b.Apply();


        bColors = b.GetPixels();

        RenderTexture.active = null;
        tex2.Release();

        drawFull = 1;

        for (int i = 0; i < bColors.Length; i++)
        {
            if (bColors[i].a > 0)
            {
                bColors[i] = Color.black;
                drawFull++;
            }
        }
    }

    public void CalificateTheDraw()
    {
        endButton.gameObject.SetActive(false);

        playGame = false;
        drawingTimes.Add(drawTime);

        drawTime = 0;
        if (!createTrail)
        {
            brushTrail.GetComponent<SwipeTrail>().StopMoving();
            brushTrail.transform.SetParent(dotCollector.transform);
        }
        drawer.StopDrawing();
        createTrail = true;

        int blackFilled = 0;
        int blackOverFilled = 0;

        ReadBPixels();

        for (int i = 0; i < aColors.Length; i++)
        {
            if (aColors[i] == Color.black && bColors[i] == Color.black)
            {
                blackFilled++;
            }
        }

        aColors = null;
        bColors = null;

        blackOverFilled = drawFull - blackFilled;
        float percentageOff = (blackOverFilled * 100) / drawFull;
        float fillPercentage = (blackFilled * 100) / blackToFill;
        accuracyPercentage += fillPercentage;
        accuracies.Add(fillPercentage);
        overdraws.Add(percentageOff);

        bool passable = IsWellMade(fillPercentage, percentageOff);

        ocean.GetComponent<Animator>().enabled = true;
        LevelUp(passable);
        assayIndex++;

    }

    public bool IsWellMade(float fillPercentage, float outPercentage)
    {
        if (fillPercentage >= 60)
        {
            if (outPercentage <= 70)
            {
                showStar = true;
                reapeatExercise = false;
                return true;
            }
        }
        reapeatExercise = true;
        showStar = false;
        return false;
    }

    void LevelUp(bool isPassable)
    {
        switch (typeOfGameToPlay)
        {
            case TypeOfGame.Fill:
                if (!sessionManager.activeKid.sandFirst && sessionManager.activeKid.sandLevelSet)
                {
                    if (isPassable)
                    {
                        levelFill++;
                        passLevels++;
                    }
                    else
                    {
                        repeatedLevels++;
                        levelFill--;
                    }
                }
                else
                {
                    reapeatExercise = false;
                    if (isPassable)
                    {
                        passLevels++;
                        levelFill += LevelDifficultyChange(totalLevelsNormal, assayIndex + 1);
                    }
                    else
                    {
                        levelFill -= LevelDifficultyChange(totalLevelsNormal, assayIndex + 1);
                    }
                    levelFill = Mathf.Clamp(levelFill, 0, maxiKidLevel - 1);
                }
                break;
            case TypeOfGame.Completion:
                if (!sessionManager.activeKid.sandFirst && sessionManager.activeKid.sandLevelSet)
                {
                    if (isPassable)
                    {
                        levelCompletion++;
                        passLevels++;
                    }
                    else
                    {
                        repeatedLevels++;
                    }
                }
                else
                {
                    reapeatExercise = false;
                    if (isPassable)
                    {
                        passLevels++;
                        levelCompletion += LevelDifficultyChange(totalSpecialLevels, AssaysOfHabilityToEvaluate(assayIndex + 2));
                    }
                    else
                    {
                        levelCompletion -= LevelDifficultyChange(totalSpecialLevels, AssaysOfHabilityToEvaluate(assayIndex + 2));
                    }
                    levelCompletion = Mathf.Clamp(levelCompletion, 0, maxiKidLevelSpecials - 1);
                }
                break;
            case TypeOfGame.Identify:
                if (!sessionManager.activeKid.sandFirst && sessionManager.activeKid.sandLevelSet)
                {
                    if (isPassable)
                    {
                        levelIdentyfy++;
                        passLevels++;
                    }
                    else
                    {
                        repeatedLevels++;
                    }
                }
                else
                {
                    reapeatExercise = false;
                    if (isPassable)
                    {
                        passLevels++;
                        levelIdentyfy += LevelDifficultyChange(totalSpecialLevels, AssaysOfHabilityToEvaluate(assayIndex + 2));
                    }
                    else
                    {
                        levelIdentyfy -= LevelDifficultyChange(totalSpecialLevels, AssaysOfHabilityToEvaluate(assayIndex + 2));
                    }
                    levelIdentyfy = Mathf.Clamp(levelIdentyfy, 0, maxiKidLevelSpecials - 1);
                }
                break;
        }
    }

    public void CleanTheSand()
    {
        if (showStar)
        {
            stars.Play();
        }

        for (int i = dotCollector.transform.childCount-1; i >= 0; i--)
        {
            Destroy(dotCollector.transform.GetChild(i).gameObject);
        }

        foreach (DrawCanvas c in multiples)
        {
            c.canvas.SetActive(false);
            c.ClearData();
        }

        centerCanvas.visibleRenderer.gameObject.SetActive(false);

        blackToFill = 0;
        drawFull = 0;
    }

    public void AfterTheClean()
    {
        instructionPanel.SetActive(true);
        readyButton.onClick.RemoveAllListeners();

        if (showStar)
        {
            instructionText.text = stringsToShow[8];
            readyButton.gameObject.SetActive(false);
            audioManager.PlayClip(instructionsClips[8]);
            Invoke("ReadyButtonOn", audioManager.ClipDuration());
        }
        else
        {
            instructionText.text = stringsToShow[7];
            readyButton.gameObject.SetActive(false);
            audioManager.PlayClip(instructionsClips[7]);
            Invoke("ReadyButtonOn", audioManager.ClipDuration());
        }
        if (assayIndex < maxNumberOfAssays)
        {
            readyButton.onClick.AddListener(TellInstruction);
            SetTheNextGame();
        }
        else
        {
            FinishGame();
        }
    }

    void FinishGame()
    {
        levelGame++;
        if (levelGame >= 16)
        {
            levelGame = 0;
        }
        SaveLevels();
        instructionText.text = stringsToShow[9];
        readyButton.gameObject.SetActive(false);
        audioManager.PlayClip(instructionsClips[9]);
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

    int AssaysOfHabilityToEvaluate(int assayToDetermine)
    {
        int baseOfAssyas = 2;
        int determination = 0;

        for (int i = 0; i < (maxNumberOfAssays / 2); i++)
        {
            if (assayToDetermine < (baseOfAssyas * (1 + i)))
            {
                determination = i;
                break;
            }
        }

        return determination;
    }

    /// <summary>
    /// Function to determine how many levels a player change with the FLIS
    /// </summary>
    /// <param name="totalNumberOfLevelsToAdapt"></param>
    /// <returns></returns>
    int LevelDifficultyChange(int totalNumberOfLevelsToAdapt, int assay)
    {
        //To determine how many levels go up or down we use the function
        // y = z/(2^(x+1))
        //where x = current assay, y = the amount of levels to change, z = the total levels of the game
        //x starts in 0
        //we return a integer value, so could be some differences with an actual graphic of the function
        int currentAssay = assay;
        int amountOfLevelsToChange = Mathf.RoundToInt(totalNumberOfLevelsToAdapt / Mathf.Pow(2, (currentAssay + 1)));
        return amountOfLevelsToChange;
    }
}

public struct DrawCanvas
{
    public DrawCanvas(GameObject obj)
    {
        canvas = obj;
        visibleRenderer = obj.transform.GetChild(0).GetComponent<SpriteRenderer>();
        nonVisibleRenderer = obj.transform.GetChild(1).GetComponent<SpriteRenderer>();
    }

    public void ClearData() 
    {
        visibleRenderer.sprite = null;
        nonVisibleRenderer.sprite = null;
    }

    public GameObject canvas;
    public SpriteRenderer visibleRenderer;
    public SpriteRenderer nonVisibleRenderer;

}