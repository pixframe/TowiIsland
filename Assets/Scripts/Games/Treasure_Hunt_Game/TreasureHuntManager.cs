using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;
using TMPro;

public class TreasureHuntManager : MonoBehaviour {

    [Header("Objects")]
    public GameObject glow;
    public GameObject bengal;
    public GameObject inputer;
    public GameObject spawnAreasManager;
    public GameObject characterSpawnerPlace;
    public GameObject parentOfTreasures;
    public GameObject storyManager;
    GameObject[] stories;

    [Header("Characters")]
    public GameObject[] characters;

    [Header("Stimulus")]
    public GameObject[] stimulus;
    public Sprite[] stimuluisIcons;
    MiniCanvas[] miniCanvasShower;
    MiniCanvas[] inventoryShower;
    MiniCanvas[] backpackShower;
    Button[] backpackInventoryManager;

    [Header("Cameras")]
    public Camera normalCam;
    public Camera playerCam;

    [Header("UI Elements")]
    public GameObject[] storiesCanvas;
    public GameObject instructionPanel;
    public GameObject specialInstructionPanel;
    public GameObject miniCanvasManager;
    public GameObject inventoryManager;
    public GameObject backpackManager;
    public GameObject cellPhone;
    public GameObject faceTimeApp;
    public GameObject needApp;
    public GameObject backpackApp;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI instrctionSpecialText;
    public Button readyButton;
    public Button cellPhoneButton;
    public Button stopCellPhoneButton;

    [Header("Mini UI elements")]
    public TextMeshProUGUI miniUIInstructionsText;
    public Button yesButton;
    public Button noButton;

    [Header("Text needs")]
    public TextAsset textAsset;
    string[] stringsToShow;

    PauseTheGame pauser;

    GameObject[] miniPanels;
    GameObject character;
    PlayerController playerController;
    SessionManager sessionManager;
    LevelSaver levelSaver;
    AudioManager audioManager;
    DemoKey demokey;

    AudioClip[] instructionsClips;

    List<BoxCollider> spawnAreas = new List<BoxCollider>();
    List<TreasureManager> tempTreasures = new List<TreasureManager>();
    List<GameObject> spawnTreasures = new List<GameObject>();
    List<int[]> packedArrays = new List<int[]>();
    List<int> countsOfArrays = new List<int>();

    int numberOfStimulus;
    int numberOfDistractions;
    int numberOfHides;

    List<int> sizeOfStimulus = new List<int>();
    List<int[]> groupAndTypeOfStimulus = new List<int[]>();
    List<int[]> backpackedObjects = new List<int[]>();

    int difficulty;
    int level;
    int numberOfAssays = 4;
    int numberOfAnswers;
    int trys = 2;
    int storyIndex;
    int errors = 0;
    int goods = 0;
    int extraInventoryShow = 0;
    int firstTime;
    int instructionsIndex;
    int passLevels;
    int repeatedLevels;
    int notSure;
    int levelCategorizer;
    int totalLevels = 36;
    int miniKidLevel = 11;
    int maxiKidLevel = 26;
    int secondChances;
    int diferenceBetweenAskObjects;
    int minimunObjects;
    int maximunObjects;
    int remainder;

    float time;

    enum PackingStatuts { Ok, LessItems, MoreItems, WrongItems};
    PackingStatuts packingStatuts;

    bool reapetAssay = false;
    bool showTutorial = false;
    bool counting = false;

    List<int> playedLevels = new List<int>();
    List<int> playedDifficulties = new List<int>();

    // Use this for initialization
    void Start()
    {
        if (FindObjectOfType<SessionManager>())
        {
            sessionManager = FindObjectOfType<SessionManager>();
            levelSaver = GetComponent<LevelSaver>();
        }
        if (FindObjectOfType<DemoKey>())
        {
            demokey = FindObjectOfType<DemoKey>();
        }
        audioManager = FindObjectOfType<AudioManager>();
        textAsset = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Games/Treasure/TreasureText");
        instructionsClips = Resources.LoadAll<AudioClip>($"{LanguagePicker.BasicAudioRoute()}Games/Treasure");
        cellPhone.SetActive(false);
        GetLevel();
        FillLists();
        stringsToShow = TextReader.TextsToShow(textAsset);
        pauser = GameObject.FindObjectOfType<PauseTheGame>();
        FillAreas();
        InstanciateCharacter();
        ShowTheHunterManager();
        ResetAGame();
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
            pauser.playButton.onClick.AddListener(TalkInstruction);
        }
    }

	// Update is called once per frame
	void Update ()
    {
        sessionManager.UpdateTime();
        if (counting)
        {
            time += Time.deltaTime;
        }
	}

    public void CheckForAnswer()
    {
        switch (instructionsIndex)
        {
            case 0:
                TellInstructionsToPlay();
                StartCoroutine(WaitForInstruction());
                break;
            case 1:
                TellInstructionsToPlay();
                playerController.gameObject.GetComponent<PlayerGrabbing>().AskForRetro();
                break;
            case 2:
                TellInstructionsToPlay();
                playerController.gameObject.GetComponent<PlayerGrabbing>().AskForRetro();
                break;
            case 3:
                TellInstructionsToPlay();
                playerController.gameObject.GetComponent<PlayerGrabbing>().AskForRetro();
                break;
            case 4:
                TellInstructionsToPlay();
                //playerController.gameObject.GetComponent<PlayerGrabbing>().AskForRetro();
                break;
        }
    }

    IEnumerator WaitForInstruction()
    {
        playerController.DontMove();
        yield return new WaitForSeconds(0.3f);
        playerController.AskForRetro();
        playerController.TimeToMove();
    }

    //This will set a new game no matter what condition
    void ResetAGame()
    {
        if (parentOfTreasures.transform.childCount > 0)
        {
            for (int i = parentOfTreasures.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(parentOfTreasures.transform.GetChild(i).gameObject);
            }
        }
        foreach (MiniCanvas m in miniCanvasShower)
        {
            m.theObject.SetActive(false);
        }
        foreach (MiniCanvas m in inventoryShower)
        {
            m.theObject.SetActive(false);
        }
        foreach (MiniCanvas m in inventoryShower)
        {
            m.theObject.SetActive(false);
        }
        instructionPanel.SetActive(true);
        sizeOfStimulus.Clear();
        spawnTreasures.Clear();
        tempTreasures.Clear();
        groupAndTypeOfStimulus.Clear();
        countsOfArrays.Clear();
        packedArrays.Clear();
        errors = 0;
        trys = 2;
        extraInventoryShow = 0;
        reapetAssay = false;
        GetTheData();
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(TalkInstruction);
        readyButton.gameObject.SetActive(true);
        counting = true;
    }

    void ResetAGame(bool isOk)
    {
        if (parentOfTreasures.transform.childCount > 0)
        {
            for (int i = parentOfTreasures.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(parentOfTreasures.transform.GetChild(i).gameObject);
            }
        }
        foreach (MiniCanvas m in miniCanvasShower)
        {
            m.theObject.SetActive(false);
        }
        foreach (MiniCanvas m in inventoryShower)
        {
            m.theObject.SetActive(false);
        }
        foreach (MiniCanvas m in inventoryShower)
        {
            m.theObject.SetActive(false);
        }
        instructionPanel.SetActive(true);
        sizeOfStimulus.Clear();
        spawnTreasures.Clear();
        tempTreasures.Clear();
        groupAndTypeOfStimulus.Clear();
        countsOfArrays.Clear();
        packedArrays.Clear();
        errors = 0;
        trys = 2;
        extraInventoryShow = 0;
        reapetAssay = false;
        SetTheTutorial();
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(TalkInstruction);
    }

    //This script is the one used to tell the story of the game
    void TellAStory()
    {
        ResetAGame(true);

        showTutorial = true;

        instructionPanel.transform.parent.gameObject.SetActive(true);
        instructionPanel.SetActive(true);
        cellPhoneButton.gameObject.SetActive(false);

        stopCellPhoneButton.gameObject.SetActive(false);
        stories[storyIndex].SetActive(true);
        instructionText.text = stringsToShow[storyIndex];
        readyButton.gameObject.SetActive(false);
        audioManager.PlayClip(instructionsClips[storyIndex]);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
        if (storyIndex > 0)
        {
            stories[storyIndex - 1].SetActive(false);
        }

        readyButton.onClick.RemoveAllListeners();
        storyIndex++;

        if (storyIndex < stories.Length)
        {
            readyButton.onClick.AddListener(TellAStory);
        }
        else
        {
            readyButton.onClick.AddListener(TalkInstruction);
        }
    }

    //This will show the objects that will be look for
    void TalkInstruction()
    {
        instructionPanel.transform.parent.gameObject.SetActive(true);

        if (showTutorial)
        {
            stories[storyIndex - 1].SetActive(false);
        }

        readyButton.onClick.RemoveAllListeners();

        cellPhoneButton.gameObject.SetActive(false);
        stopCellPhoneButton.gameObject.SetActive(false);
        inventoryManager.SetActive(false);
        backpackApp.SetActive(false);
        faceTimeApp.SetActive(false);
        instructionPanel.SetActive(true);
        cellPhone.SetActive(true);
        needApp.gameObject.SetActive(true);

        readyButton.onClick.AddListener(HuntTheTreasures);
        cellPhoneButton.onClick.AddListener(CallByFaceTime);
        stopCellPhoneButton.onClick.AddListener(StopCall);
        yesButton.onClick.AddListener(CheckTheBackpack);
        instructionText.text = stringsToShow[4];
        readyButton.gameObject.SetActive(false);
        audioManager.PlayClip(instructionsClips[4]);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
    }

    //This will let the game to start
    void ShowGame()
    {
        cellPhoneButton.gameObject.SetActive(false);
        stopCellPhoneButton.gameObject.SetActive(false);
        inventoryManager.SetActive(false);
        faceTimeApp.SetActive(false);
        instructionPanel.SetActive(true);
        needApp.gameObject.SetActive(true);
    }

    //This will get the information of the game to let the kid play
    void GetLevel()
    {
        if (sessionManager != null)
        {
            if (!FindObjectOfType<DemoKey>())
            {
                if (sessionManager.activeKid.firstsGames[(int)GameConfigurator.KindOfGame.Treasure])
                {
                    FLISSetup();
                }
                else
                {
                    difficulty = sessionManager.activeKid.treasureDifficulty;
                    level = sessionManager.activeKid.treasureLevel;
                    firstTime = 1;
                }
            }
            else
            {
                var key = FindObjectOfType<DemoKey>();
                if (key.IsFLISOn())
                {
                    sessionManager.activeKid.firstsGames[(int)GameConfigurator.KindOfGame.Treasure] = true;
                    FLISSetup();
                }
                else
                {
                    sessionManager.activeKid.firstsGames[(int)GameConfigurator.KindOfGame.Treasure] = false;
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

        }
        else
        {
            difficulty = PlayerPrefs.GetInt(Keys.Treasure_Difficulty);
            level = PlayerPrefs.GetInt(Keys.Treasure_Level);
            firstTime = PlayerPrefs.GetInt(Keys.Treasure_First);
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

    //This will sav ethe progress that the kid have achived in the session
    void SaveLevel()
    {
        counting = false;
        if (sessionManager != null)
        {
            var startLevel = sessionManager.activeKid.treasureLevel;
            var startDifficulty = sessionManager.activeKid.treasureDifficulty;
            sessionManager.activeKid.treasureDifficulty = difficulty;
            sessionManager.activeKid.treasureLevel= level;
            if (sessionManager.activeKid.firstsGames[(int)GameConfigurator.KindOfGame.Treasure])
            {
                sessionManager.activeKid.firstsGames[(int)GameConfigurator.KindOfGame.Treasure] = false;
            }
            sessionManager.activeKid.playedGames[(int)GameConfigurator.KindOfGame.Treasure] = true;
            sessionManager.activeKid.needSync = true;
            sessionManager.activeKid.kiwis += passLevels;

            //levelSaver.AddLevelData("level", difficulty);
            //levelSaver.AddLevelData("sublevel", level);
            //levelSaver.AddLevelData("time", time);
            //levelSaver.AddLevelData("tutorial", 1);
            //levelSaver.AddLevelData("passed", passLevels);
            //levelSaver.AddLevelData("playerobjects", "player objects");
            //levelSaver.AddLevelData("playerobjectsquantity", " quantity of objects");
            //levelSaver.AddLevelData("correctobjects", "objects corrects");
            //levelSaver.AddLevelData("correctobjectsquantity", "quantity of correct objects");
            //levelSaver.AddLevelData("spawnedobjects", "quantity of spawn objects");
            //levelSaver.AddLevelData("spawneddistractors", "quantity of spawned distractors");
            //levelSaver.AddLevelData("notsurecorrect", notSure);
            //levelSaver.AddLevelData("notsureincorrect", 0);
            //levelSaver.AddLevelData("minobjects", 3);
            //levelSaver.AddLevelData("maxobjects", 15);
            //levelSaver.AddLevelData("availableobjects", "15");
            //levelSaver.AddLevelData("availablecategories", "5");
            //levelSaver.AddLevelData("searchorders", "look for these objects");
            //levelSaver.AddLevelData("availabledistractors", "15");

            //Version 2
            sessionManager.activeKid.treasureSessions++;
            if (sessionManager.activeKid.firstsGames[(int)GameConfigurator.KindOfGame.Treasure])
            {
                sessionManager.activeKid.firstsGames[(int)GameConfigurator.KindOfGame.Treasure] = false;
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
            levelSaver.AddLevelData("session_correct_total", (goods * 100)/numberOfAnswers);
            levelSaver.AddLevelData("session_errors_total", (errors * 100) / numberOfAnswers);
            levelSaver.AddLevelData("total_second_chances", secondChances);
            levelSaver.AddLevelData("diffrence_in_quantity", diferenceBetweenAskObjects);
            levelSaver.AddLevelDataAsString("played_levels", playedLevels);
            levelSaver.AddLevelDataAsString("played_difficulty", playedDifficulties);
            levelSaver.AddLevelData("session_minobjects", minimunObjects);
            levelSaver.AddLevelData("session_maxobjects", maximunObjects);
            levelSaver.AddLevelData("requestreminder", remainder);
            levelSaver.AddLevelData("session_time", (int)time);

            levelSaver.CreateSaveBlock("Tesoro", time, passLevels, repeatedLevels, 5, sessionManager.activeKid.treasureSessions);
            levelSaver.AddLevelsToBlock();
            levelSaver.PostProgress();
        }

        PlayerPrefs.SetInt(Keys.Treasure_Difficulty, difficulty);
        PlayerPrefs.SetInt(Keys.Treasure_Level, level);
        PlayerPrefs.SetInt(Keys.Treasure_First, 1);
    }

    //This is used to fill all the arrays that will have to be filled in the game in order to work properly
    void FillLists()
    {
        miniCanvasShower = new MiniCanvas[miniCanvasManager.transform.childCount];
        inventoryShower = new MiniCanvas[inventoryManager.transform.childCount];
        backpackShower = new MiniCanvas[backpackManager.transform.childCount];
        backpackInventoryManager = new Button[backpackManager.transform.childCount];

        for (int i = 0; i < miniCanvasManager.transform.childCount; i++)
        {
            GameObject putter = miniCanvasManager.transform.GetChild(i).gameObject;

            miniCanvasShower[i] = new MiniCanvas(putter);
        }

        for (int i = 0; i < inventoryManager.transform.childCount; i++)
        {
            GameObject putter = inventoryManager.transform.GetChild(i).gameObject;

            inventoryShower[i] = new MiniCanvas(putter);
        }

        for (int i = 0; i < backpackManager.transform.childCount; i++)
        {
            GameObject putter = backpackManager.transform.GetChild(i).gameObject;

            backpackShower[i] = new MiniCanvas(putter);
            backpackInventoryManager[i] = putter.GetComponent<Button>();
        }

        stories = new GameObject[storyManager.transform.childCount];

        for (int i = 0; i < storyManager.transform.childCount; i++)
        {
            stories[i] = storyManager.transform.GetChild(i).gameObject;
        }

    }

    //This one is created to fill th eareas in wich the objects can be spawned
    void FillAreas()
    {
        for (int i = 0; i < spawnAreasManager.transform.childCount; i++)
        {
            spawnAreas.Add(spawnAreasManager.transform.GetChild(i).GetComponent<BoxCollider>());
        }
    }

    //This will create the character that the child previosly has chose
    void InstanciateCharacter()
    {
        string avatarName = sessionManager.activeKid.avatar;
        if (avatarName != "")
        {
            avatarName.ToLower();
        }
        character = Instantiate(characters[TowiDictionary.AvatarNames[avatarName]], characterSpawnerPlace.transform.position, characterSpawnerPlace.transform.rotation);
        character.AddComponent<PlayerGrabbing>();
        playerController = character.GetComponent<PlayerController>();
    }


    void ShowTheHunterManager()
    {
        inputer.SetActive(false);
        playerCam.gameObject.SetActive(true);
        normalCam.gameObject.SetActive(true);
    }

    void SetTheTutorial()
    {
        numberOfStimulus = 1;
        numberOfDistractions = 0;
        numberOfHides = 0;

        sizeOfStimulus.Add(0);

        spawnTreasures.Add(SpawnTheObjects(stimulus[0].GetComponent<TreasureManager>(), 0));

        int[] data = new int[] { spawnTreasures[spawnTreasures.Count - 1].GetComponent<TreasureManager>().GetTheTypeOfThe(), 0};
        sizeOfStimulus[0]++;
        groupAndTypeOfStimulus.Add(data);

        for (int i = 0; i < groupAndTypeOfStimulus.Count; i++)
        {
            miniCanvasShower[i].image.sprite = stimuluisIcons[(groupAndTypeOfStimulus[i][0] * 3) + groupAndTypeOfStimulus[i][1]];
            miniCanvasShower[i].numberText.text = "x " + sizeOfStimulus[i].ToString();
            miniCanvasShower[i].theObject.SetActive(true);
        }
    }

    //This will get all the information needed for the game that is hod in other script
    void GetTheData()
    {
        List<int> individuals = new List<int>();
        List<List<int>> lists = new List<List<int>>();
        individuals = GameConfigurator.TreasureGameConfigSimple(difficulty, level);
        lists = GameConfigurator.TreasureGameConfigDouble(difficulty, level);
        numberOfStimulus = individuals[0];
        numberOfDistractions = individuals[1];
        numberOfHides = individuals[2];

        if (minimunObjects == 0 || minimunObjects > numberOfStimulus)
        {
            minimunObjects = numberOfStimulus;
        }

        if (maximunObjects == 0 || maximunObjects < numberOfStimulus)
        {
            maximunObjects = numberOfStimulus;
        }

        for (int i = 0; i < stimulus.Length; i++)
        {
            tempTreasures.Add(stimulus[i].GetComponent<TreasureManager>());
        }

        int index = 0;
        bool addData = false;

        for (int i = 0; i < lists.Count; i++)
        {
            int randomize = Random.Range(0, tempTreasures.Count);
            int current = lists[i][0];
            sizeOfStimulus.Add(0);
            addData = true;
            for (int j = 0; j < lists[i].Count; j++) {
                spawnTreasures.Add(SpawnTheObjects(tempTreasures[randomize], lists[i][j]));
                int[] data = new int[]{spawnTreasures[spawnTreasures.Count-1].GetComponent<TreasureManager>().GetTheTypeOfThe(),lists[i][j]};
                if (current < lists[i][j]) {
                    current++;
                    index++;
                    sizeOfStimulus.Add(0);
                    addData = true;
                }
                sizeOfStimulus[index]++;
                if (addData) {
                    groupAndTypeOfStimulus.Add(data);
                    addData = false;
                }

            }
            index ++;
            tempTreasures.Remove(tempTreasures[randomize]);
        }

        for (int i = 0; i < groupAndTypeOfStimulus.Count; i++)
        {
            miniCanvasShower[i].image.sprite = stimuluisIcons[(groupAndTypeOfStimulus[i][0] * 3) + groupAndTypeOfStimulus[i][1]];
            miniCanvasShower[i].numberText.text = "x " + sizeOfStimulus[i].ToString();
            miniCanvasShower[i].theObject.SetActive(true);
        }
        CreateTheDistractors();
    }

    //This will spawn an object
    GameObject SpawnTheObjects(TreasureManager treasure, int qualifire)
    {
        int randy = Random.Range(0, spawnAreas.Count);
        float xPos = Random.Range((spawnAreas[randy].center.x - (spawnAreas[randy].size.x / 2)), (spawnAreas[randy].center.x + (spawnAreas[randy].size.x / 2)));
        float zPos = Random.Range((spawnAreas[randy].center.z - (spawnAreas[randy].size.x / 2)), (spawnAreas[randy].center.z + (spawnAreas[randy].size.x / 2)));
        GameObject theObject = Instantiate(treasure.gameObject, transform.position, transform.rotation);
        theObject.transform.SetParent(spawnAreas[randy].transform);
        theObject.transform.localPosition = new Vector3(xPos, 2f, zPos);
        theObject.transform.parent = parentOfTreasures.transform;
        theObject.transform.GetComponent<TreasureManager>().Activater(qualifire);
        return theObject;
    }

    //This script is made to spaw the distractors if there are needed
    void CreateTheDistractors()
    {
        for (int i = 0; i < numberOfDistractions; i++) {
            int typeOfStimulus = Random.Range(0, stimulus.Length);
            int typeRandomize = Random.Range(0, 3);
            int randy = Random.Range(0, spawnAreas.Count);
            float xPos = Random.Range((spawnAreas[randy].center.x - (spawnAreas[randy].size.x / 2)), (spawnAreas[randy].center.x + (spawnAreas[randy].size.x / 2)));
            float zPos = Random.Range((spawnAreas[randy].center.z - (spawnAreas[randy].size.x / 2)), (spawnAreas[randy].center.z + (spawnAreas[randy].size.x / 2)));
            GameObject theObject = Instantiate(stimulus[typeOfStimulus], transform);
            theObject.transform.SetParent(spawnAreas[randy].transform);
            theObject.transform.localPosition = new Vector3(xPos, 2f, zPos);
            theObject.transform.parent = parentOfTreasures.transform;
            theObject.transform.GetComponent<TreasureManager>().Activater(typeRandomize);
        }
    }
    
    //Here we will let the player start with the game and enable all the things that the kid needs
    void HuntTheTreasures()
    {
        if (showTutorial)
        {
            instructionText.text = stringsToShow[instructionsIndex + 5];
            readyButton.gameObject.SetActive(false);
            audioManager.PlayClip(instructionsClips[instructionsIndex + 5]);
            readyButton.gameObject.SetActive(false);
            StartCoroutine(WaitForInstruction());
            cellPhoneButton.gameObject.SetActive(false);
        }
        else
        {
            cellPhoneButton.gameObject.SetActive(true);
            instructionPanel.SetActive(false);
            playedLevels.Add(level);
            playedDifficulties.Add(difficulty);
        }
        Transform t = GameObject.FindGameObjectWithTag("CamaraLayer").transform;
        normalCam.gameObject.SetActive(false);
        playerCam.gameObject.SetActive(true);
        inventoryManager.SetActive(true);
        backpackApp.SetActive(false);
        stopCellPhoneButton.gameObject.SetActive(false);
        cellPhone.SetActive(false);
        inputer.SetActive(true);
        playerCam.GetComponent<ThirdPersonCamera>().SetFollowToTransfrom(t);
        character.GetComponent<PlayerGrabbing>().SetGrabTime(glow.GetComponent<ParticleSystem>());
        character.GetComponent<PlayerController>().TimeToMove();
    }

    void TellInstructionsToPlay()
    {
        instructionsIndex++;
        int ins = instructionsIndex + 5;
        if (ins == 8)
        {
            specialInstructionPanel.SetActive(true);
            instructionPanel.SetActive(false);
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                instructionsIndex++;
                ins++;
            }
            instrctionSpecialText.text = instructionText.text = stringsToShow[ins];
            audioManager.PlayClip(instructionsClips[ins]);
            if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                instructionsIndex++;
            }
        }
        else
        {
            if (ins == 10)
            {
                cellPhoneButton.gameObject.SetActive(true);
            }
            specialInstructionPanel.SetActive(false);
            instructionPanel.SetActive(true);
            instructionText.text = stringsToShow[ins];
            audioManager.PlayClip(instructionsClips[ins]);
        }
    }

    //This one is the script used to storage the object in the backpack
    public void PutObjectInBackpack(GameObject packable)
    {

        int[] arr = packable.GetComponent<TreasureManager>().GetTheInfo();
        if (packedArrays.Count < 1)
        {
            packedArrays.Add(arr);
            countsOfArrays.Add(1);
        }
        else if (packedArrays.Count > 0)
        {
            bool isReapeat = false;
            for (int i = 0; i < packedArrays.Count; i++)
            {
                if (packedArrays[i][0] == arr[0] && packedArrays[i][1] == arr[1])
                {
                    isReapeat = true;
                    countsOfArrays[i]++;
                    break;
                }
            }
            if (!isReapeat)
            {
                packedArrays.Add(arr);
                countsOfArrays.Add(1);
            }
        }
        InventoryUpdate();
        packable.SetActive(false);
        glow.transform.position = new Vector3(50f, 50f, 50f);
        Destroy(packable);
    }

    //This will update the inventory of the player that is showed in the up part of the screen
    void InventoryUpdate()
    {
        for (int i = 0; i < packedArrays.Count; i++)
        {
            inventoryShower[i].image.sprite = stimuluisIcons[(packedArrays[i][0] * 3) + packedArrays[i][1]];
            inventoryShower[i].numberText.text = "x " + countsOfArrays[i].ToString();
            inventoryShower[i].theObject.SetActive(true);
        }

        for (int i = packedArrays.Count; i < inventoryShower.Length; i++)
        {
            inventoryShower[i].theObject.SetActive(false);
        }
    }

    //Here we will schek if everything is ok or not
    void SeeIfAllAreRight()
    {
        packingStatuts = CompareListAndBackpack();
        HandlePackingResult();
    }

    //Here will see what is in the backpack
    void CheckTheBackpack()
    {
        packingStatuts = CompareListAndBackpack();
        HandlePackingResult();
    }

    //Here we compare the storage items with the intems previosly showed
    PackingStatuts CompareListAndBackpack()
    {

        int totalBackpack = 0;
        for (int i = 0; i < countsOfArrays.Count; i++)
        {
            totalBackpack += countsOfArrays[i];
        }
        diferenceBetweenAskObjects += (totalBackpack - numberOfStimulus);
        if (totalBackpack < numberOfStimulus)
        {
            return PackingStatuts.LessItems;
        }
        else if (totalBackpack > numberOfStimulus)
        {
            return PackingStatuts.MoreItems;
        }
        else
        {
            List<int[]> tempRightItems = new List<int[]>();
            for (int i = 0; i < groupAndTypeOfStimulus.Count; i++)
            {
                tempRightItems.Add(groupAndTypeOfStimulus[i]);

            }

            List<bool> bools = new List<bool>();

            for (int i = 0; i < packedArrays.Count; i++)
            {
                bool foundInTheList = false;
                for (int j = 0; j < groupAndTypeOfStimulus.Count; j++)
                {
                    if (!foundInTheList)
                    {
                        if (((packedArrays[i][0] * 3) + packedArrays[i][1]) == ((groupAndTypeOfStimulus[j][0] * 3) + groupAndTypeOfStimulus[j][1]) && countsOfArrays[i] == sizeOfStimulus[j])
                        {
                            foundInTheList = true;
                        }
                    }
                }
                if (foundInTheList)
                {
                    bools.Add(true);
                }
                else
                {
                    bools.Add(false);
                }
            }

            for (int i = 0; i < bools.Count; i++)
            {
                if (!bools[i])
                {
                    return PackingStatuts.WrongItems;
                }
            }
            return PackingStatuts.Ok;
        }
    }

    //Here we make an actions accordingly to the answer of the player
    void HandlePackingResult()
    {
        noButton.gameObject.SetActive(false);
        yesButton.GetComponentInChildren<TextMeshProUGUI>().text = stringsToShow[23];
        numberOfAnswers++;
        
        switch (packingStatuts)
        {
            case PackingStatuts.MoreItems:
                yesButton.onClick.RemoveAllListeners();
                trys--;
                if (trys > 0)
                {
                    UniversalError();
                }
                else
                {
                    errors++;
                    miniUIInstructionsText.text = stringsToShow[18];
                    audioManager.PlayClip(instructionsClips[18]);
                    yesButton.onClick.AddListener(SetNewAssay);
                }
                break;
            case PackingStatuts.LessItems:
                yesButton.onClick.RemoveAllListeners();
                trys--;
                if (trys > 0)
                {
                    UniversalError();
                }
                else
                {
                    errors++;
                    miniUIInstructionsText.text = stringsToShow[19];
                    audioManager.PlayClip(instructionsClips[19]);
                    yesButton.onClick.AddListener(SetNewAssay);
                }
                break;
            case PackingStatuts.Ok:
                goods++;
                if (numberOfAssays <= 1)
                {
                    miniUIInstructionsText.text = stringsToShow[16];
                    audioManager.PlayClip(instructionsClips[16]);
                    FinishGame();
                }
                else
                {
                    miniUIInstructionsText.text = stringsToShow[15];
                    audioManager.PlayClip(instructionsClips[15]);
                    yesButton.onClick.AddListener(SetNewAssay);
                }
                noButton.gameObject.SetActive(false);
                break;
            case PackingStatuts.WrongItems:
                yesButton.onClick.RemoveAllListeners();
                trys--;
                if (trys > 0)
                {
                    UniversalError();
                }
                else
                {
                    errors++;
                    miniUIInstructionsText.text = stringsToShow[18];
                    audioManager.PlayClip(instructionsClips[18]);
                    yesButton.onClick.AddListener(SetNewAssay);
                }
                break;
        }
    }

    void UniversalError()
    {
        miniUIInstructionsText.text = stringsToShow[14];
        audioManager.PlayClip(instructionsClips[14]);
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(ShowInventorySecondTime);
        stopCellPhoneButton.gameObject.SetActive(false);
        errors++;
        secondChances++;
    }

    //This will handle the level up or down accordingly to the game
    void HandleNewLevel()
    {
        if (!sessionManager.activeKid.firstsGames[(int)GameConfigurator.KindOfGame.Treasure])
        {
            if (errors < 2)
            {
                level++;
                passLevels++;
                if (level > 5)
                {
                    level = 0;
                    difficulty++;
                    if (difficulty > 5)
                    {
                        level = 5;
                        difficulty = 5;
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
            FastLevelIdentificationSystem();
        }
    }

    void FastLevelIdentificationSystem()
    {
        if (errors < 2)
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

    //This will clear the game and create a new assay ready for play 
    void SetNewAssay()
    {
        numberOfAssays--;
        HandleNewLevel();
        if (numberOfAssays < 1)
        {
            miniUIInstructionsText.text = stringsToShow[16];
            audioManager.PlayClip(instructionsClips[16]);
            FinishGame();
        }
        else
        {
            ResetAGame();
            TalkInstruction();
        }
    }

    //This will iniciate the calling app wich is a small menu to talk with the turtle and see if the answers are right
    void CallByFaceTime()
    {
        //Here will set all the needs to show the correct app
        cellPhone.SetActive(true);
        normalCam.gameObject.SetActive(true);
        faceTimeApp.SetActive(true);
        yesButton.GetComponentInChildren<TextMeshProUGUI>().text = stringsToShow[22];

        //Here willl be set some condition accordingly to the try number or other stuff we consider
        if (trys > 1)
        {
            miniUIInstructionsText.text = stringsToShow[12];
            audioManager.PlayClip(instructionsClips[12]);
            stopCellPhoneButton.gameObject.SetActive(true);
            yesButton.gameObject.SetActive(true);
            noButton.gameObject.SetActive(true);
            stopCellPhoneButton.onClick.RemoveAllListeners();
            yesButton.onClick.RemoveAllListeners();
            noButton.onClick.RemoveAllListeners();
            stopCellPhoneButton.onClick.AddListener(StopCall);
            yesButton.onClick.AddListener(CheckTheBackpack);
            noButton.onClick.AddListener(AskForAClue);
            if (showTutorial)
            {
                stopCellPhoneButton.gameObject.SetActive(false);
                noButton.gameObject.SetActive(false);
                specialInstructionPanel.SetActive(false);
                instructionPanel.SetActive(false);
                showTutorial = false;
            }
        }
        else
        {
            CheckTheBackpack();
        }

        //Here will stop showing what is not needed
        cellPhoneButton.gameObject.SetActive(false);
        needApp.SetActive(false);
        inventoryManager.SetActive(false);
        backpackApp.SetActive(false);
        inputer.SetActive(false);
        playerController.DontMove();
    }

    void AskForAClue()
    {
        miniUIInstructionsText.text = stringsToShow[13];
        audioManager.PlayClip(instructionsClips[13]);
        if (packedArrays.Count > 0)
        {
            trys--;
        }
        else
        {
        }

        yesButton.gameObject.SetActive(true);
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(ShowInventorySecondTime);
        noButton.gameObject.SetActive(true);
        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(StopCall);
    }

    //This actions is call if the player ask for a remainder of the correct objects
    void ShowInventorySecondTime()
    {
        extraInventoryShow++;
        remainder++;
        needApp.SetActive(true);
        inventoryManager.SetActive(true);

        stopCellPhoneButton.gameObject.SetActive(true);
        stopCellPhoneButton.onClick.RemoveAllListeners();
        stopCellPhoneButton.onClick.AddListener(AskForDropAction);

        normalCam.gameObject.SetActive(false);
        backpackApp.SetActive(false);
        faceTimeApp.SetActive(false);
    }

    //Here we ask if the child wants to delete an object
    void AskForDropAction()
    {
        StopAllCoroutines();
        miniUIInstructionsText.text = stringsToShow[20];
        audioManager.PlayClip(instructionsClips[20]);
        yesButton.GetComponentInChildren<TextMeshProUGUI>().text = stringsToShow[22];
        faceTimeApp.SetActive(true);
        normalCam.gameObject.SetActive(true);

        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(CheckTheBackpackInventory);
        noButton.onClick.AddListener(StopCall);

        needApp.SetActive(false);
        backpackApp.SetActive(false);
        stopCellPhoneButton.gameObject.SetActive(false);
        inventoryManager.SetActive(false);
    }

    void CheckTheBackpackInventory()
    {
        cellPhone.SetActive(true);
        backpackApp.SetActive(true);

        stopCellPhoneButton.gameObject.SetActive(true);
        stopCellPhoneButton.onClick.RemoveAllListeners();
        stopCellPhoneButton.onClick.AddListener(AskForMoreAction);

        needApp.SetActive(false);
        faceTimeApp.SetActive(false);
        inventoryManager.SetActive(false);
        normalCam.gameObject.SetActive(false);
        cellPhoneButton.gameObject.SetActive(false);

        ShowTheBackPackStuff();
    }

    void AskForMoreAction()
    {
        miniUIInstructionsText.text = stringsToShow[21];
        audioManager.PlayClip(instructionsClips[21]);
        faceTimeApp.SetActive(true);
        normalCam.gameObject.SetActive(true);

        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(StopCall);
        noButton.onClick.AddListener(CheckTheBackpack);

        stopCellPhoneButton.gameObject.SetActive(false);
    }

    //this will stop the use of the cellphone and let the game continue without a troble
    void StopCall()
    {
        inventoryManager.SetActive(true);
        cellPhoneButton.gameObject.SetActive(true);
        inputer.SetActive(true);

        needApp.SetActive(false);
        cellPhone.SetActive(false);
        backpackApp.SetActive(false);
        normalCam.gameObject.SetActive(false);
        stopCellPhoneButton.gameObject.SetActive(false);

        playerController.TimeToMove();
        InventoryUpdate();
    }

    //This is used when a player got extra objects and ask for drop them
    void ShowTheBackPackStuff()
    {
        foreach (MiniCanvas m in backpackShower)
        {
            m.theObject.SetActive(false);
        }

        for (int i = 0; i < packedArrays.Count; i++)
        {
            backpackShower[i].image.sprite = stimuluisIcons[(packedArrays[i][0] * 3) + packedArrays[i][1]];
            backpackShower[i].numberText.text = "x " + countsOfArrays[i];
            backpackShower[i].theObject.SetActive(true);
            backpackInventoryManager[i].onClick.RemoveAllListeners();
            int x = i;
            backpackInventoryManager[i].onClick.AddListener(() => RemoveBackpackObjects(x));
        }
    }

    //This script is attached to the button in the backpack app and let the player remove an object
    void RemoveBackpackObjects(int indi)
    {
        countsOfArrays[indi]--;
        if (countsOfArrays[indi] < 1)
        {
            packedArrays.Remove(packedArrays[indi]);
            countsOfArrays.Remove(countsOfArrays[indi]);
            ShowTheBackPackStuff();
        }
        else
        {
            backpackShower[indi].numberText.text = "x" + countsOfArrays[indi];
        }
    }

    //this will set the finish of the game
    void FinishGame()
    {
        HandleNewLevel();
        SaveLevel();
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(ShowEarnings);
    }

    void ShowEarnings()
    {
        specialInstructionPanel.SetActive(false);
        instructionPanel.gameObject.SetActive(false);
        pauser.ShowKiwiEarnings(passLevels);
    }

    IEnumerator TimeOfTheList()
    {

        yield return new WaitForSeconds(15f);
        AskForDropAction();
    }

    void ReadyButtonOn()
    {
        readyButton.gameObject.SetActive(true);
    }

    void GetDataJustForLevel(int levelInput)
    {
        int baseDificulty = 5;
        int amountOfDifficulties = 6;

        for (int i = 0; i < amountOfDifficulties; i++)
        {

            if (levelInput < baseDificulty * (i + 1))
            {
                int x = i;
                difficulty = x;
                break;
            }
        }

        level = levelInput - (difficulty * baseDificulty);

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
        int currentAssay = 4 - numberOfAssays;
        int amountOfLevelsToChange = Mathf.RoundToInt(totalNumberOfLevelsToAdapt / Mathf.Pow(2, (currentAssay + 1)));
        Debug.Log(amountOfLevelsToChange);
        return amountOfLevelsToChange;
    }
}

//This struc here help us to show the objects that the kid will need or the ones that already grabbed
public struct MiniCanvas
{
    public Image image;
    public TextMeshProUGUI numberText;
    public GameObject theObject;

    public MiniCanvas(GameObject obj)
    {
        theObject = obj;
        image = obj.transform.GetChild(0).GetComponent<Image>();
        numberText = obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }
}
