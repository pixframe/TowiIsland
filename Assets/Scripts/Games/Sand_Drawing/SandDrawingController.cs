using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SandDrawingController : MonoBehaviour {

    DrawCanvas centerCanvas;
    DrawCanvas[] multiples;
    SandDrawer drawer;
    SwipeTrail trailer;
    SessionManager sessionManager;
    LevelSaver levelSaver;

    enum Answer { Good, Reapetable, Bad, NotFilledEnough };
    Answer currentAnswer;
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

	Sprite[] probableSprites;
	Sprite[] keySprites;

    int insideDots;
    int outsideDots;
    int totalOfPoints;
    int indexStory;
    int assayIndex;
    int levelGame = 14;
    int levelFill = 20;
    int levelCompletion = 11;
    int levelIdentyfy = 11;
    int maxNumberOfAssays = 5;
    int totalLevelsNormal = 30;
    int totalSpecialLevels = 15;
    int miniKidLevel = 10;
    int maxiKidLevel = 20;
    int miniKidLevelSpecials = 5;
    int maxiKidLevelSpecials = 10;
    int blackToFill = 0;
    int drawFull = 0;
    int w = Screen.width;
    int h = Screen.width;
    int firstTime;
    int passLevels;
    int repeatedLevels;
    List<int> choosen = new List<int>();

    int[] typeOfGamesIndex;

    float time;
    float accuracyPercentage;

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

	RenderTexture tex;
	RenderTexture tex2;

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
            levelSaver = GetComponent<LevelSaver>();
        }
        audioManager = FindObjectOfType<AudioManager>();
        instructionsClips = Resources.LoadAll<AudioClip>("Audios/Games/Sand");
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
        }
        else
        {
            instructionPanel.transform.parent.gameObject.SetActive(false);
            pauser.WantTutorial();
            pauser.howToPlayButton.onClick.AddListener(TellAStory);
            pauser.playButton.onClick.AddListener(TellInstruction);
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
        if (sessionManager != null)
        {
            if (sessionManager.activeKid.sandFirst)
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
            else
            {
                if (!sessionManager.activeKid.sandLevelSet)
                {
                    Debug.Log("Set second stuff");
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
                    Debug.Log("Set no stuff");
                    levelGame = sessionManager.activeKid.sandDifficulty;
                    levelFill = sessionManager.activeKid.sandLevel;
                    levelIdentyfy = sessionManager.activeKid.sandLevel2;
                    levelCompletion = sessionManager.activeKid.sandLevel3;
                }
                firstTime = 1;
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

    void SaveLevels()
    {
        countTime = true;
        if (sessionManager != null)
        {
            string a = "1" + levelFill.ToString("00") + levelIdentyfy.ToString("00") + levelCompletion.ToString("00");
            int sublevel = int.Parse(a);
            accuracyPercentage = (accuracyPercentage * 100) / 500;
            if (sessionManager.activeKid.sandFirst)
            {
                sessionManager.activeKid.sandFirst = false;
            }
            else
            {
                if (!sessionManager.activeKid.sandLevelSet)
                {
                    sessionManager.activeKid.sandLevelSet = true;
                }
            }
            sessionManager.activeKid.sandDifficulty = levelGame;
            sessionManager.activeKid.sandLevel = levelFill;
            sessionManager.activeKid.sandLevel2 = levelIdentyfy;
            sessionManager.activeKid.sandLevel3 = levelCompletion;
            sessionManager.activeKid.kiwis += passLevels;
            sessionManager.activeKid.playedSand = 1;
            sessionManager.activeKid.needSync = true;

            levelSaver.AddLevelData("level", levelGame);
            levelSaver.AddLevelData("sublevel", levelFill);
            levelSaver.AddLevelData("sublevel", levelIdentyfy);
            levelSaver.AddLevelData("sublevel", levelCompletion);
            levelSaver.AddLevelData("time", time);
            levelSaver.AddLevelData("passed", passLevels);
            levelSaver.AddLevelData("repeated", repeatedLevels);
            levelSaver.AddLevelData("accuracy", accuracyPercentage);

            levelSaver.SetLevel();
            levelSaver.CreateSaveBlock("ArenaMagica", time, passLevels, repeatedLevels, 5);
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

    void TellInstruction()
    {
        countTime = true;
        if (showTutorial)
        {
            stories[indexStory - 1].SetActive(false);
        }
        else
        {
            instructionPanel.transform.parent.gameObject.SetActive(true);
        }
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
        tex = new RenderTexture(w, h, 24);

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
                int[] deficit = new int[] { 0, 2 };
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

    void SetGameStuff()
    {
        if (reapeatExercise && secondTry)
        {
            secondTry = false;
            newStimulus = true;
        }
        else
        {
            Sprite shower;
            Sprite bases;
			Object[] datas;
			Object[] data2;
            int randy = 0;
            List<int> temporals = new List<int>();
            switch (typeOfGameToPlay)
            {
                case TypeOfGame.Fill:
                    if (levelFill < 10)
                    {
						datas = Resources.LoadAll("Sand/EasyBank" , typeof(Sprite));
						probableSprites = new Sprite[datas.Length];
						for (int i = 0; i < datas.Length;i++)
						{
							probableSprites[i] = (Sprite)datas[i];
                            bool hasBeenAdded = false;
                            for (int j = 0; j < choosen.Count; j++)
                            {
                                if (choosen[j] == i)
                                {
                                    hasBeenAdded = true;
                                }
                            }
                            if (!hasBeenAdded)
                            {
                                temporals.Add(i);
                            }
						}
                    }
                    else if (levelFill >= 10 && levelFill < 20)
                    {
						datas = Resources.LoadAll("Sand/MiddleBank", typeof(Sprite));
                        probableSprites = new Sprite[datas.Length];
                        for (int i = 0; i < datas.Length; i++)
                        {
                            probableSprites[i] = (Sprite)datas[i];
                            bool hasBeenAdded = false;
                            for (int j = 0; j < choosen.Count; j++)
                            {
                                if (choosen[j] == i)
                                {
                                    hasBeenAdded = true;
                                }
                            }
                            if (!hasBeenAdded)
                            {
                                temporals.Add(i);
                            }
                        }
                    }
                    else
                    {
						datas = Resources.LoadAll("Sand/HardBank", typeof(Sprite));
                        probableSprites = new Sprite[datas.Length];
                        for (int i = 0; i < datas.Length; i++)
                        {
                            probableSprites[i] = (Sprite)datas[i];
                            bool hasBeenAdded = false;
                            for (int j = 0; j < choosen.Count; j++)
                            {
                                if (choosen[j] == i)
                                {
                                    hasBeenAdded = true;
                                }
                            }
                            if (!hasBeenAdded)
                            {
                                temporals.Add(i);
                            }
                        }
                    }

					randy = Random.Range(0, temporals.Count);
                    choosen.Add(temporals[randy]);
                    shower = probableSprites[temporals[randy]];
                    bases = probableSprites[temporals[randy]];

                    break;
                case TypeOfGame.Completion:
                    if (levelCompletion < 3)
                    {
						datas = Resources.LoadAll("Sand/EasyComplete", typeof(Sprite));
                        probableSprites = new Sprite[datas.Length];
                        for (int i = 0; i < datas.Length; i++)
                        {
                            probableSprites[i] = (Sprite)datas[i];
                            bool hasBeenAdded = false;
                            for (int j = 0; j < choosen.Count; j++)
                            {
                                if (choosen[j] == i)
                                {
                                    hasBeenAdded = true;
                                }
                            }
                            if (!hasBeenAdded)
                            {
                                temporals.Add(i);
                            }
                        }
						data2 = Resources.LoadAll("Sand/EasyBank", typeof(Sprite));
						keySprites = new Sprite[data2.Length];
                        for (int i = 0; i < data2.Length; i++)
                        {
							keySprites[i] = (Sprite)datas[i];
                        }

						randy = Random.Range(0, temporals.Count);
                        choosen.Add(temporals[randy]);
                        shower = probableSprites[temporals[randy]];
						bases = keySprites[temporals[randy]];
                    }
                    else if (levelCompletion >= 3 && levelCompletion < 5)
                    {
						datas = Resources.LoadAll("Sand/MiddleComplete", typeof(Sprite));
                        probableSprites = new Sprite[datas.Length];
                        for (int i = 0; i < datas.Length; i++)
                        {
                            probableSprites[i] = (Sprite)datas[i];
                            bool hasBeenAdded = false;
                            for (int j = 0; j < choosen.Count; j++)
                            {
                                if (choosen[j] == i)
                                {
                                    hasBeenAdded = true;
                                }
                            }
                            if (!hasBeenAdded)
                            {
                                temporals.Add(i);
                            }
                        }
                        data2 = Resources.LoadAll("Sand/MiddleBank", typeof(Sprite));
                        keySprites = new Sprite[data2.Length];
                        for (int i = 0; i < data2.Length; i++)
                        {
                            keySprites[i] = (Sprite)datas[i];
                        }

                        randy = Random.Range(0, temporals.Count);
                        choosen.Add(temporals[randy]);
                        shower = probableSprites[temporals[randy]];
                        bases = keySprites[temporals[randy]];
                    }
                    else if (levelCompletion >= 5 && levelCompletion < 8)
                    {
						datas = Resources.LoadAll("Sand/HardComplete", typeof(Sprite));
                        probableSprites = new Sprite[datas.Length];
                        for (int i = 0; i < datas.Length; i++)
                        {
                            probableSprites[i] = (Sprite)datas[i];
                            bool hasBeenAdded = false;
                            for (int j = 0; j < choosen.Count; j++)
                            {
                                if (choosen[j] == i)
                                {
                                    hasBeenAdded = true;
                                }
                            }
                            if (!hasBeenAdded)
                            {
                                temporals.Add(i);
                            }
                        }
                        data2 = Resources.LoadAll("Sand/HardBank", typeof(Sprite));
                        keySprites = new Sprite[data2.Length];
                        for (int i = 0; i < data2.Length; i++)
                        {
                            keySprites[i] = (Sprite)datas[i];
                        }

                        randy = Random.Range(0, temporals.Count);
                        choosen.Add(temporals[randy]);
                        shower = probableSprites[temporals[randy]];
                        bases = keySprites[temporals[randy]];
                    }
                    else if (levelCompletion >= 8 && levelCompletion < 10)
                    {
						datas = Resources.LoadAll("Sand/EasyHalf", typeof(Sprite));
                        probableSprites = new Sprite[datas.Length];
                        for (int i = 0; i < datas.Length; i++)
                        {
                            probableSprites[i] = (Sprite)datas[i];
                            bool hasBeenAdded = false;
                            for (int j = 0; j < choosen.Count; j++)
                            {
                                if (choosen[j] == i)
                                {
                                    hasBeenAdded = true;
                                }
                            }
                            if (!hasBeenAdded)
                            {
                                temporals.Add(i);
                            }
                        }
						randy = Random.Range(0, temporals.Count);
                        choosen.Add(temporals[randy]);
						string path = datas[temporals[randy]].name.Substring(0, datas[temporals[randy]].name.Length - 3) + "_01";
						Sprite spi = Resources.Load<Sprite>("Sand/EasyBank/" + path);
						shower = probableSprites[temporals[randy]];
						bases = spi;
                    }
                    else if (levelCompletion >= 10 && levelCompletion < 12)
                    {
						datas = Resources.LoadAll("Sand/MiddleHalf", typeof(Sprite));
                        probableSprites = new Sprite[datas.Length];
                        for (int i = 0; i < datas.Length; i++)
                        {
                            probableSprites[i] = (Sprite)datas[i];
                            bool hasBeenAdded = false;
                            for (int j = 0; j < choosen.Count; j++)
                            {
                                if (choosen[j] == i)
                                {
                                    hasBeenAdded = true;
                                }
                            }
                            if (!hasBeenAdded)
                            {
                                temporals.Add(i);
                            }
                        }
                        randy = Random.Range(0, temporals.Count);
                        choosen.Add(temporals[randy]);
                        string path = datas[temporals[randy]].name.Substring(0, datas[temporals[randy]].name.Length - 3) + "_01";
                        Sprite spi = Resources.Load<Sprite>("Sand/MiddleBank/" + path);
                        shower = probableSprites[temporals[randy]];
                        bases = spi;
                    }
                    else
                    {
						datas = Resources.LoadAll("Sand/HardHalf", typeof(Sprite));
                        probableSprites = new Sprite[datas.Length];
                        for (int i = 0; i < datas.Length; i++)
                        {
                            probableSprites[i] = (Sprite)datas[i];
                            bool hasBeenAdded = false;
                            for (int j = 0; j < choosen.Count; j++)
                            {
                                if (choosen[j] == i)
                                {
                                    hasBeenAdded = true;
                                }
                            }
                            if (!hasBeenAdded)
                            {
                                temporals.Add(i);
                            }
                        }
                        randy = Random.Range(0, temporals.Count);
                        choosen.Add(temporals[randy]);
                        string path = datas[temporals[randy]].name.Substring(0, datas[temporals[randy]].name.Length - 3) + "_01";
                        Sprite spi = Resources.Load<Sprite>("Sand/HardBank/" + path);
                        shower = probableSprites[temporals[randy]];
                        bases = spi;
                    }
                    break;
                case TypeOfGame.Identify:
                    int[] spis = new int[multiples.Length];
                    List<int> temp = new List<int>();

                    if (levelIdentyfy < 5)
                    {
						datas = Resources.LoadAll("Sand/EasyBank", typeof(Sprite));
                        probableSprites = new Sprite[datas.Length];
                        for (int i = 0; i < datas.Length; i++)
                        {
                            probableSprites[i] = (Sprite)datas[i];
                        }              
                    }
                    else if (levelIdentyfy >= 5 && levelIdentyfy < 10)
                    {
						datas = Resources.LoadAll("Sand/MiddleBank", typeof(Sprite));
                        probableSprites = new Sprite[datas.Length];
                        for (int i = 0; i < datas.Length; i++)
                        {
                            probableSprites[i] = (Sprite)datas[i];
                        }       
                    }
                    else
                    {
						datas = Resources.LoadAll("Sand/HardBank", typeof(Sprite));
                        probableSprites = new Sprite[datas.Length];
                        for (int i = 0; i < datas.Length; i++)
                        {
                            probableSprites[i] = (Sprite)datas[i];
                        }    
                    }

					for (int i = 0; i < probableSprites.Length; i++)
                    {
                            temp.Add(i);
                    }    
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
        tex2 = new RenderTexture(w, h, 24);

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

        blackOverFilled = drawFull - blackFilled;
        float percentageOff = (blackOverFilled * 100) / drawFull;
        float fillPercentage = (blackFilled * 100) / blackToFill;
        accuracyPercentage += fillPercentage;
        Debug.Log("percentage off equals " + percentageOff + " fill percentage " + fillPercentage);
        bool tooMuchOut = false;
        bool passable = false;

        if (fillPercentage > 64)
        {
            passable = true;
            showStar = true;
            reapeatExercise = false;
        }
        else
        {
            reapeatExercise = true;
            showStar = false;
        }

        if (percentageOff <= 55)
        {
            tooMuchOut = false;
        }
        else if (percentageOff <= 90 && percentageOff > 70)
        {
            tooMuchOut = true;
            passable = false;
            showStar = false;
            reapeatExercise = true;
        }
        else
        {
            tooMuchOut = true;
            passable = false;
            showStar = false;
            reapeatExercise = true;
        }

        ocean.GetComponent<Animator>().enabled = true;
        LevelUp(passable, tooMuchOut);
        assayIndex++;

    }

    void LevelUp(bool isPassable, bool isWellLimited)
    {
        switch (typeOfGameToPlay)
        {
            case TypeOfGame.Fill:
                if (!sessionManager.activeKid.sandFirst && sessionManager.activeKid.sandLevelSet)
                {
                    if (isPassable && !isWellLimited)
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
                    if (isPassable && !isWellLimited)
                    {
                        passLevels++;
                        levelFill += LevelDifficultyChange(totalLevelsNormal, assayIndex + 1);
                    }
                    else
                    {
                        levelFill -= LevelDifficultyChange(totalLevelsNormal, assayIndex + 1);
                    }
                    Mathf.Clamp(levelFill, 0, maxiKidLevel - 1);
                    Debug.Log("The next level in fill is " + levelFill);
                }
                break;
            case TypeOfGame.Completion:
                if (!sessionManager.activeKid.sandFirst && sessionManager.activeKid.sandLevelSet)
                {
                    if (isPassable && !isWellLimited)
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
                    if (isPassable && !isWellLimited)
                    {
                        passLevels++;
                        levelCompletion += LevelDifficultyChange(totalSpecialLevels, AssaysOfHabilityToEvaluate(assayIndex + 2));
                    }
                    else
                    {
                        levelCompletion -= LevelDifficultyChange(totalSpecialLevels, AssaysOfHabilityToEvaluate(assayIndex + 2));
                    }
                    Mathf.Clamp(levelCompletion, 0, maxiKidLevelSpecials - 1);
                    Debug.Log("The next level in completion is " + levelCompletion);
                }
                break;
            case TypeOfGame.Identify:
                if (!sessionManager.activeKid.sandFirst && sessionManager.activeKid.sandLevelSet)
                {
                    if (isPassable && !isWellLimited)
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
                    if (isPassable && !isWellLimited)
                    {
                        passLevels++;
                        levelIdentyfy += LevelDifficultyChange(totalSpecialLevels, AssaysOfHabilityToEvaluate(assayIndex + 2));
                    }
                    else
                    {
                        levelIdentyfy -= LevelDifficultyChange(totalSpecialLevels, AssaysOfHabilityToEvaluate(assayIndex + 2));
                    }
                    Mathf.Clamp(levelIdentyfy, 0, maxiKidLevelSpecials - 1);
                    Debug.Log("The next level in completion is" + levelIdentyfy);
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

        Debug.Log("determinatios is " + determination);
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

    public GameObject canvas;
    public SpriteRenderer visibleRenderer;
    public SpriteRenderer nonVisibleRenderer;

}