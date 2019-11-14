using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BirdsSingingManager : MonoBehaviour {
    //These Objects are Needed and are made available ina previous scene
    AudioManager audioManager;
    SessionManager sessionManager;
    DemoKey key;
    PauseTheGame pauser;
    LevelSaver levelSaver;
    NeedAudioMenu needAudioMenu;

    //These are the public objects needed to play the scripts
    [Header("Game Elements")]
    public GameObject birdManager;
    public GameObject nestManager;
    public GameObject positionManager;
    public GameObject storyScreensManager;
    public LayerMask birdLayer;
    public LayerMask nestLayer;

    //These objects are the UI that will change the stuff
    [Header("UI Elements")]
    public TextMeshProUGUI instructionText;
    public GameObject instructionPanel;
    public Button readyButton;

    GameObject[] storyScreens;

    [Header("Text Elements")]
    public TextAsset textAsset;
    string[] stringsToShow;

    //These private variables are used to set up the game
    GameObject selectedBird;
    Vector3 currentPosition;

    List<NestController> tempoNest = new List<NestController>();
    List<NestController> currentNest = new List<NestController>();
    List<NestController> fullNest = new List<NestController>();
    List<BirdsController> tempoBirds = new List<BirdsController>();
    List<BirdsController> currentBirds = new List<BirdsController>();
    List<BirdsController> fullBirds = new List<BirdsController>();
    List<Transform> tempoBirdPos = new List<Transform>();
    List<Transform> currentBirdPos = new List<Transform>();
    List<Transform> fullBirdPos = new List<Transform>();

    [Header("Sound Categories")]
    public AudioClip[] categories0;
    public AudioClip[] categories1;
    public AudioClip[] categories2;
    public AudioClip[] categories3;
    public AudioClip[] categories4;
    public AudioClip[] categories5;
    public AudioClip wellDoneAudio;
    public AudioClip badDoneAudio;

    AudioClip[] instructionsClips;
    List<AudioClip[]> audioBank = new List<AudioClip[]>();
    List<int> data = new List<int>();
    List<int> miniCategoryBank = new List<int>(){ 0, 1, 2, 3, 4 };
    //this is the level of difficulty inside the game
    int difficulty;
    //this is the sub level of difficulty
    int level;
    int levelCategorizer;
    int numberOfCategories;
    int pool;
    int categorie;
    int numberOfAssays = 5;
    int birdsNumber;
    int nestsNumber;
    int occupiedNest;
    int bankNumber;
    int storyIndex;
    int maxLevelPerDifficulty;
    int singingBirds;
    int nestPlayIndex;
    int reapetedClue;
    int reapetedClueAssays;
    int repeatedOneBad;
    int sizeOfCategories;
    int firstTime;
    int birdsWellOrdered;
    int birdsBadOrdered;
    int totalLevels = 54;
    int miniKidLevel = 8;
    int maxiKidLevel = 36;
    public enum GamePhase {Tutorial, Listen , Listening, Lounging , Game, Transition };

    int errors;
    int goods;
    int goodsAfterClue;
    int errorsAfterClue;
    int passLevels;
    int repeatedLevels;
    int firstDifficulty;
    int clues;
    int tutorialLook;
    int beforePair;
    int afterPair;


    float time;
    float assayTime;

    [System.NonSerialized]
    public GamePhase phase;
    bool playTime = false;
    bool listenTime = false;
    bool objectIsSelected = false;
    bool isFirstBird = true;
    bool showTutorial;
    bool countTime = false;
    bool pairing = false;
    bool afterHear;

    List<int> cluesToSend = new List<int>();
    List<int> goodAnswers = new List<int>();
    List<int> badAnswers = new List<int>();
    List<int> goodAnswersAfterClue = new List<int>();
    List<int> badAnswersAfterClue = new List<int>();
    List<int> levelsPLayed = new List<int>();
    List<int> difficultyPlayed = new List<int>();
    List<int> birdsBeforePair = new List<int>();
    List<int> birdsAfterPair = new List<int>();
    List<float> assaysTimes = new List<float>();


    // Use this for initialization
    void Start()
    {
        if (FindObjectOfType<SessionManager>())
        {
            sessionManager = FindObjectOfType<SessionManager>();
            levelSaver = GetComponent<LevelSaver>();
        }
        GetLevel();
        textAsset = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Games/Birds/BirdsText");
        instructionsClips = Resources.LoadAll<AudioClip>($"{LanguagePicker.BasicAudioRoute()}Games/Birds");
        maxLevelPerDifficulty = 9 + (difficulty * 3);
        stringsToShow = TextReader.TextsToShow(textAsset);
        pauser = FindObjectOfType<PauseTheGame>();
        audioBank = new List<AudioClip[]> { categories0, categories1, categories2, categories3, categories4, categories5 };
        audioManager = FindObjectOfType<AudioManager>();
        readyButton.gameObject.SetActive(true);
        FillList();
        instructionPanel.SetActive(false);
        needAudioMenu = FindObjectOfType<NeedAudioMenu>();
        needAudioMenu.SetHideButtonFunction(Init);
    }

    void Init()
    {
        if (firstTime == 0)
        {
            TellTheStory();
            pauser.HideTutorialButtons();
        }
        else
        {
            instructionPanel.transform.parent.gameObject.SetActive(false);
            pauser.WantTutorial();
            pauser.howToPlayButton.onClick.AddListener(TellTheStory);
            pauser.playButton.onClick.AddListener(TellToHearTheNests);
        }
        needAudioMenu.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        sessionManager.UpdateTime();

        if (countTime)
        {
            time += Time.deltaTime;
        }

        if (afterHear)
        {
            assayTime += Time.deltaTime;
        }

        if (phase == GamePhase.Game)
        {
            if (objectIsSelected)
            {
                DragABird();
                DropTheBird();
            }
        }
    }

    #region Game dynamics


    #endregion

    #region GameSetUp 
    void GetLevel()
    {
        if (sessionManager != null)
        {
            if (!FindObjectOfType<DemoKey>())
            {
                if (sessionManager.activeKid.firstsGames[(int)GameConfigurator.KindOfGame.Birds])
                {
                    FLISSetup();
                }
                else
                {
                    difficulty = sessionManager.activeKid.birdsDifficulty;
                    level = sessionManager.activeKid.birdsLevel;
                    firstTime = 1;
                }
            }
            else
            {
                var key = FindObjectOfType<DemoKey>();
                var isFLISOn = false;
                sessionManager.activeKid.firstsGames[(int)GameConfigurator.KindOfGame.Birds] = false;
                firstTime = 1;
                switch (key.GetDifficulty())
                {
                    case DemoPanel.Difficulty.Easy:
                        levelCategorizer = 0;
                        break;
                    case DemoPanel.Difficulty.Normal:
                        levelCategorizer = totalLevels / 2;
                        break;
                    case DemoPanel.Difficulty.Hard:
                        levelCategorizer = totalLevels - 5;
                        break;
                    case DemoPanel.Difficulty.Flis:
                        isFLISOn = true;
                        sessionManager.activeKid.firstsGames[(int)GameConfigurator.KindOfGame.Birds] = true;
                        FLISSetup();
                        break;
                }
                if (!isFLISOn) 
                {
                    GetDataJustForLevel(levelCategorizer);
                }
            }
        }
        else
        {
            difficulty = PlayerPrefs.GetInt(Keys.Bird_Difficulty);
            level = PlayerPrefs.GetInt(Keys.Bird_Level);
            firstTime = PlayerPrefs.GetInt(Keys.Bird_First);
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
        countTime = false;

        if (sessionManager != null)
        {
            var startLevel = sessionManager.activeKid.birdsLevel;
            var startDifficulty = sessionManager.activeKid.birdsDifficulty;
            sessionManager.activeKid.birdsDifficulty = difficulty;
            sessionManager.activeKid.birdsLevel = level;

            sessionManager.activeKid.playedGames[(int)GameConfigurator.KindOfGame.Birds] = true;
            sessionManager.activeKid.needSync = true;
            sessionManager.activeKid.kiwis += passLevels;

            //levelSaver.AddLevelData("sessioncorrect_percentage")
            //levelSaver.AddLevelData("sessioncorrect_percentage")
            //levelSaver.AddLevelData("birds", 5);
            //levelSaver.AddLevelData("nests", 5);
            //levelSaver.AddLevelData("level", difficulty);
            //levelSaver.AddLevelData("sublevel", level);
            //levelSaver.AddLevelData("tutorial", tutorialLook);
            //levelSaver.AddLevelData("sound", "null");
            //levelSaver.AddLevelData("birdsound", "null");
            //levelSaver.AddLevelData("time", time);
            //levelSaver.AddLevelData("birdlistenedpre", 0);
            //levelSaver.AddLevelData("birdlistened", 0);
            //levelSaver.AddLevelData("errors", 0);
            //levelSaver.AddLevelData("correct", 0);

            //Version 2
            sessionManager.activeKid.birdsSessions++;
            if (sessionManager.activeKid.firstsGames[(int)GameConfigurator.KindOfGame.Birds])
            {
                sessionManager.activeKid.firstsGames[(int)GameConfigurator.KindOfGame.Birds] = false;
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
            var goodBefore = new List<int>();
            var badBefore = new List<int>();
            int interactions = 0;


            for (int i = 0; i < goodAnswers.Count; i++)
            {
                goodBefore.Add(goodAnswers[i] - goodAnswersAfterClue[i]);
                badBefore.Add(badAnswers[i] - badAnswersAfterClue[i]);
                interactions += goodAnswers[i];
                interactions += badAnswers[i];
            }
            levelSaver.AddLevelData("session_correct_percentage", (goods * 100)/interactions);
            levelSaver.AddLevelData("session_errors_percentage",(errors * 100)/interactions);
            levelSaver.AddLevelDataAsString("correct_before_clue", goodBefore);
            levelSaver.AddLevelDataAsString("errors_before_clue", badBefore);
            levelSaver.AddLevelDataAsString("correct_after_clue", goodAnswersAfterClue);
            levelSaver.AddLevelDataAsString("errors_after_clue", badAnswersAfterClue);
            levelSaver.AddLevelDataAsString("dad_bird_listened_pre", birdsBeforePair);
            levelSaver.AddLevelDataAsString("dad_bird_listened_post", birdsAfterPair);
            levelSaver.AddLevelData("total_clues", cluesToSend.Sum());
            levelSaver.AddLevelData("total_correct", goodAnswers.Sum());
            levelSaver.AddLevelData("total_errors", badAnswers.Sum());
            levelSaver.AddLevelDataAsString("played_levels", levelsPLayed);
            levelSaver.AddLevelDataAsString("played_difficulty", difficultyPlayed);
            levelSaver.AddLevelDataAsString("essay_time", assaysTimes);

            levelSaver.CreateSaveBlock("ArbolMusical", time, passLevels, repeatedLevels, 5, sessionManager.activeKid.birdsSessions);
            levelSaver.AddLevelsToBlock();
            levelSaver.PostProgress();
        }

        PlayerPrefs.SetInt(Keys.Bird_Difficulty, difficulty);
        PlayerPrefs.SetInt(Keys.Bird_Level, level);
        PlayerPrefs.SetInt(Keys.Bird_First, 1);
    }

    public void SetNewGame()
    {
        GetTheDataInfo();
        SetNewLists();
        SetTheNests();
        SetTheBirds();
        if (numberOfAssays < 5)
        {
            instructionPanel.SetActive(true);
            instructionText.text = stringsToShow[3];
            audioManager.PlayClip(instructionsClips[3]);
            Invoke("ReadyButtonOn", audioManager.ClipDuration());
        }
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(PlayTheNestSongs);
    }

    void TellTheStory()
    {
        instructionPanel.SetActive(true);
        instructionPanel.transform.parent.gameObject.SetActive(true);
        showTutorial = true;
        readyButton.gameObject.SetActive(false);
        if (storyIndex > 0)
        {
            storyScreens[storyIndex - 1].SetActive(false);
        }
        instructionText.text = stringsToShow[storyIndex];
        audioManager.PlayClip(instructionsClips[storyIndex]);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
        storyScreens[storyIndex].gameObject.SetActive(true);
        readyButton.onClick.RemoveAllListeners();
        storyIndex++;
        if (storyIndex < storyScreens.Length)
        {
            readyButton.onClick.AddListener(TellTheStory);
        }
        else
        {
            readyButton.onClick.AddListener(TellToHearTheNests);
        }
    }

    void TellToHearTheNests()
    {
        countTime = true;
        instructionPanel.transform.parent.gameObject.SetActive(true);
        if (showTutorial)
        {
            storyScreens[storyIndex - 1].SetActive(false);
            instructionText.text = stringsToShow[3];
            readyButton.gameObject.SetActive(false);
            audioManager.PlayClip(instructionsClips[3]);
            Invoke("ReadyButtonOn", audioManager.ClipDuration());
            instructionPanel.SetActive(true);
            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(PlayTheNestSongs);
        }
        else
        {
            instructionPanel.SetActive(false);
            Invoke("PlayTheNestSongs", 1.5f);
        }

        GetTheDataInfo();
        SetNewLists();
        SetTheNests();
        SetTheBirds();

    }

    void FillList()
    {
        for (int i = 0; i < birdManager.transform.childCount; i++)
        {
            fullBirds.Add(birdManager.transform.GetChild(i).GetComponent<BirdsController>());

        }

        for (int i = 0; i < nestManager.transform.childCount; i++)
        {
            fullNest.Add(nestManager.transform.GetChild(i).GetComponent<NestController>());
        }

        for (int i = 0; i < positionManager.transform.childCount; i++)
        {
            fullBirdPos.Add(positionManager.transform.GetChild(i).transform);
        }

        storyScreens = new GameObject[storyScreensManager.transform.childCount];

        for (int i = 0; i < storyScreensManager.transform.childCount; i++)
        {
            storyScreens[i] = storyScreensManager.transform.GetChild(i).gameObject;
        }
    }

    void SetNewLists()
    {
        for (int i = 0; i < fullBirds.Count; i++)
        {
            tempoBirds.Add(fullBirds[i]);
        }

        for (int i = 0; i < fullNest.Count; i++)
        {
            tempoNest.Add(fullNest[i]);
        }
        for (int i = 0; i < fullBirdPos.Count; i++)
        {
            tempoBirdPos.Add(fullBirdPos[i]);
        }

        for (int i = 0; i < birdsNumber; i++)
        {
            int randy = UnityEngine.Random.Range(0, tempoBirds.Count);
            currentBirds.Add(tempoBirds[randy]);
            tempoBirds.Remove(tempoBirds[randy]);
            currentBirdPos.Add(tempoBirdPos[randy]);
            tempoBirdPos.Remove(tempoBirdPos[randy]);
        }

        foreach (BirdsController bird in tempoBirds)
        {
            bird.gameObject.SetActive(true);
            bird.TeleportTheBird();
            bird.gameObject.SetActive(false);
        }

        for (int i = 0; i < nestsNumber; i++)
        {
            int randy = UnityEngine.Random.Range(0, tempoNest.Count);
            currentNest.Add(tempoNest[randy]);
            tempoNest.Remove(tempoNest[randy]);
        }
    }

    void SetTheBirds()
    {
        int lenght = tempoBirds.Count;

        for (int i = 0; i < birdsNumber; i++)
        {
            currentBirds[i].gameObject.SetActive(true);
            currentBirds[i].SetANewDirection(currentBirdPos[i].position);
        }
        TeachTheSongsToTheBirds();
    }

    void SetTheNests()
    {
        int selectWhichBank = UnityEngine.Random.Range(0, miniCategoryBank.Count);
        categorie = (miniCategoryBank[selectWhichBank] * 5);
        miniCategoryBank.Remove(miniCategoryBank[selectWhichBank]);


        for (int i = 0; i < currentNest.Count; i++)
        {
            currentNest[i].gameObject.SetActive(true);
            currentNest[i].SetANestSong(categorie + i);
        }

    }

    void GetTheDataInfo()
    {
        data = GameConfigurator.SoundThreeConfig(difficulty, level);
        nestsNumber = data[0];
        birdsNumber = data[1];
        bankNumber = data[2];
        difficultyPlayed.Add(difficulty);
        levelsPLayed.Add(level);
    }

    void ChangeThePhase(GamePhase newPhase)
    {
        phase = newPhase;
    }

    void PlayTheNestSongs() {
        ChangeThePhase(GamePhase.Listen);
        instructionPanel.SetActive(false);
        audioManager.PlayClip(audioBank[bankNumber][currentNest[nestPlayIndex].GetTheNestSong()]);
        currentNest[nestPlayIndex].PlayTheNotes();
        nestPlayIndex++;
        Debug.Log($"current amount of nest should be {nestsNumber - occupiedNest} but its {currentNest.Count}");
        if (nestPlayIndex < (nestsNumber - occupiedNest))
        {
            Invoke("PlayTheNestSongs", audioManager.ClipDuration() + 1.5f);
        }
        else
        {
            Invoke("PlaySecondInstruction", audioManager.ClipDuration() + 1.5f);
        }
    }

    void ReapeatTheFreeNest()
    {
        clues++;
        nestPlayIndex = 0;
        instructionPanel.SetActive(false);
        audioManager.PlayClip(audioBank[bankNumber][currentNest[nestPlayIndex].GetTheNestSong()]);
        currentNest[nestPlayIndex].PlayTheNotes();
        nestPlayIndex++;
        if (nestPlayIndex < currentNest.Count)
        {
            Invoke("PlayTheNestSongs", audioManager.ClipDuration() + 1.5f);
        }
        else
        {
            Invoke("ReturnToGamePhase", audioManager.ClipDuration() + 1.5f);
            Debug.Log("we return");
        }
    }

    public void PlayANestSong(int soundNumber)
    {
        audioManager.PlayClip(audioBank[bankNumber][soundNumber]);
    }

    public void ReturnToGamePhase()
    {
        ChangeThePhase(GamePhase.Game);
    }

    void PlaySecondInstruction()
    {
        ChangeThePhase(GamePhase.Listen);
        if (showTutorial)
        {
            readyButton.gameObject.SetActive(false);
            instructionPanel.SetActive(true);
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                instructionText.text = stringsToShow[4];
                audioManager.PlayClip(instructionsClips[4]);
            }
            else
            {
                instructionText.text = stringsToShow[5];
                audioManager.PlayClip(instructionsClips[5]);
            }
            Invoke("ReadyButtonOn", audioManager.ClipDuration());
            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(HearTheBirds);
        }
        else
        {
            HearTheBirds();
        }
        //audioManager.PlayBirdInstructions(4);
    }

    void HearTheBirds()
    {
        instructionPanel.SetActive(false);
        ChangeThePhase(GamePhase.Game);
        afterHear = true;
    }

    void PlayLastInstruction()
    {
        if (showTutorial)
        {
            ChangeThePhase(GamePhase.Listen);
            instructionPanel.SetActive(true);
            instructionText.text = stringsToShow[6];
            readyButton.gameObject.SetActive(false);
            audioManager.PlayClip(instructionsClips[6]);
            Invoke("ReadyButtonOn", audioManager.ClipDuration());
            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(QuitInstruction);
        }
        else
        {
            QuitInstruction();
        }
    }

    void QuitInstruction()
    {
        ChangeThePhase(GamePhase.Game);
        instructionPanel.SetActive(false);
    }

    #endregion

    #region Bird Actions

    void TeachTheSongsToTheBirds()
    {
        if (birdsNumber >= nestsNumber)
        {
            for (int i = 0; i < birdsNumber; i++)
            {
                currentBirds[i].LearnASongNumber(categorie + i);
            }
        }
        else
        {
            List<int> nestSounds = new List<int>();
            for (int i = 0; i < currentNest.Count; i++)
            {
                nestSounds.Add(currentNest[i].GetTheNestSong());
            }

            for (int i = 0; i < birdsNumber; i++)
            {
                int random = UnityEngine.Random.Range(0, nestSounds.Count);
                currentBirds[i].LearnASongNumber(nestSounds[random]);
                nestSounds.Remove(nestSounds[random]);
            }
        }

    }

    public void BirdSing(int category)
    {
        audioManager.PlayClip(audioBank[bankNumber][category]);
        ChangeThePhase(GamePhase.Listening);
        if (isFirstBird)
        {
            Invoke("PlayLastInstruction", audioManager.ClipDuration());
            isFirstBird = false;
        }
        else
        {
            if (selectedBird != null)
            {
                selectedBird.GetComponent<BirdsController>().TeleportToStandingPos();
                selectedBird = null;
                objectIsSelected = false;
            }
            Invoke("BirdStopSinging", audioManager.ClipDuration());
        }
    }

    public void BirdStopSinging()
    {
        ChangeThePhase(GamePhase.Game);
    }

    public void SelectTheBird(GameObject birdToSelect)
    {
        selectedBird = birdToSelect;
        objectIsSelected = true;
    }

    //this will move the selected bird
    void DragABird()
    {
        currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentPosition.z = selectedBird.transform.position.z;
        selectedBird.transform.position = currentPosition;
    }

    //this will leave a bird in a pplpace or will make it return to its place
    void DropTheBird()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit,Mathf.Infinity, nestLayer))
            {
                NestController nestSelected = hit.transform.GetComponent<NestController>();
                BirdsController dadaBird = selectedBird.GetComponent<BirdsController>();
                if (CheckIfBirdEqualsNest(nestSelected, dadaBird))
                {
                    audioManager.PlayClip(wellDoneAudio);
                    objectIsSelected = false;
                    selectedBird.transform.position = nestSelected.DadPosition();
                    selectedBird = null;
                    birdsWellOrdered++;
                    if (clues > 0)
                    {
                        goodsAfterClue++;
                    }
                    nestSelected.PlayTheNotes(true);
                    dadaBird.BirdIsWellSet();
                    currentNest.Remove(nestSelected);
                    occupiedNest++;
                    if (AreAllNestFill())
                    {
                        HandleFinishActivity();
                    }
                }
                else
                {
                    birdsBadOrdered++;
                    if (clues > 0)
                    {
                        errorsAfterClue++;
                    }
                    nestSelected.PlayTheNotes(false);
                    //PlayANestSong(nestSelected.GetTheNestSong());
                    if (birdsBadOrdered % 3 == 0)
                    {
                        audioManager.PlayClip(badDoneAudio);
                        ChangeThePhase(GamePhase.Listen);
                        Invoke("ReapeatTheFreeNest", audioManager.ClipDuration() + 1.5f);
                    }
                    else
                    {
                        audioManager.PlayClip(badDoneAudio);
                    }
                    selectedBird = null;
                    objectIsSelected = false;
                    dadaBird.BirdMissTheNest();
                }
            }
            else
            {
                selectedBird.GetComponent<BirdsController>().BirdMissTheNest();
                selectedBird = null;
                objectIsSelected = false;
            }
        }
    }


    bool CheckIfBirdEqualsNest(NestController nestToCheck, BirdsController birdToCheck)
    {
        return nestToCheck.GetTheNestSong() == birdToCheck.SingASongNumber();
    }

    bool AreAllNestFill()
    {
        if (nestsNumber < birdsNumber)
        {
            if (nestsNumber == birdsWellOrdered)
            {
                return true;
            }
        }
        else if (nestsNumber > birdsNumber)
        {
            if (birdsWellOrdered == birdsNumber)
            {
                return true;
            }
        }
        else
        {
            if (birdsWellOrdered == nestsNumber)
            {
                return true;
            }
        }
        return false;
    }

    void HandleFinishActivity()
    {
        instructionPanel.SetActive(true);
        readyButton.onClick.RemoveAllListeners();
        instructionText.text = stringsToShow[7];
        readyButton.gameObject.SetActive(false);
        audioManager.PlayClip(instructionsClips[7]);
        Invoke("ReadyButtonOn", audioManager.ClipDuration());
        if (sessionManager.activeKid.firstsGames[(int)GameConfigurator.KindOfGame.Birds])
        {
            numberOfAssays--;
            FastLevelIdentificationSystem();
        }
        else
        {
            NewLevelArrange();
            numberOfAssays--;
        }

        goodAnswers.Add(birdsWellOrdered);
        goodAnswersAfterClue.Add(goodsAfterClue);
        badAnswers.Add(birdsBadOrdered);
        badAnswersAfterClue.Add(errorsAfterClue);
        cluesToSend.Add(clues);
        birdsAfterPair.Add(afterPair);
        birdsBeforePair.Add(beforePair);
        assaysTimes.Add(assayTime);

        if (numberOfAssays > 0)
        {
            readyButton.onClick.AddListener(ResetTheGame);
        }
        else
        {
            FinishTheGame();
        }
    }

    void ResetTheGame()
    {
        occupiedNest = 0;
        instructionPanel.SetActive(false);
        errors += birdsBadOrdered;
        goods += birdsWellOrdered;
        birdsWellOrdered = 0;
        birdsBadOrdered = 0;
        goodsAfterClue = 0;
        errorsAfterClue = 0;
        clues = 0;
        afterPair = 0;
        beforePair = 0;
        assayTime = 0;
        pairing = false;
        afterHear = false;
        DesactivateAllBirds();
        Camera.main.transform.GetComponent<BirdCamera>().StartMoving();
        tempoBirdPos.Clear();
        tempoBirds.Clear();
        tempoNest.Clear();
        currentNest.Clear();
        currentBirds.Clear();
        currentBirdPos.Clear();
        ChangeThePhase(GamePhase.Transition);
        nestPlayIndex = 0;
        showTutorial = false;
    }

    void DesactivateAllBirds()
    {
        for (int i = 0; i < tempoBirds.Count; i++)
        {
            tempoBirds[i].transform.gameObject.SetActive(false);
        }
    }

    public void GoToNewPos()
    {
        foreach (NestController nest in fullNest)
        {
            nest.gameObject.SetActive(false);
        }
        foreach (BirdsController bird in fullBirds)
        {
            bird.gameObject.SetActive(true);
            bird.TeleportTheBird();
            bird.gameObject.SetActive(false);
        }
    }

    void FinishTheGame()
    {
        SaveLevel();
        instructionText.text = stringsToShow[8];
        readyButton.gameObject.SetActive(false);
        audioManager.PlayClip(instructionsClips[8]);
        Invoke("ShowEarnings", audioManager.ClipDuration());
    }

    void ShowEarnings()
    {
        instructionPanel.gameObject.SetActive(false);
        pauser.ShowKiwiEarnings(passLevels);
    }

    void NewLevelArrange()
    {
        if (birdsBadOrdered < 1)
        {
            level++;
            passLevels++;
            ChangeLevel();
        }
        if (birdsBadOrdered == 1)
        {
            repeatedOneBad++;
            repeatedLevels++;
            if (repeatedOneBad >= 5)
            {
                level--;
                ChangeLevel();
            }
        }
        if (reapetedClue == 1)
        {
            reapetedClueAssays++;
            if (reapetedClueAssays == 3)
            {
                level--;
                ChangeLevel();
            }
        }
        if (reapetedClue > 1)
        {
            level--;
            ChangeLevel();
        }
    }

    void FastLevelIdentificationSystem()
    {
        if (birdsBadOrdered < 1)
        {
            passLevels++;
            levelCategorizer += LevelDifficultyChange(totalLevels);
        }
        else
        {
            levelCategorizer -= LevelDifficultyChange(totalLevels);
        }
        Debug.Log("New Level categorizer is " + levelCategorizer);
        levelCategorizer = Mathf.Clamp(levelCategorizer, 0, totalLevels - 1);
        GetDataJustForLevel(levelCategorizer);
    }

    void ChangeLevel()
    {
        repeatedOneBad = 0;
        reapetedClueAssays = 0;
    }

    void ReadyButtonOn()
    {
        readyButton.gameObject.SetActive(true);
    }

    public void PairOrNot()
    {
        if (pairing)
        {
            afterPair++;
        }
        else
        {
            beforePair++;
        }
    }

    void GetDataJustForLevel(int levelInput)
    {
        int totalDifficulty = 4;
        int baseLevelDifficulty = 9;
        int[] levelBreakers = new int[totalDifficulty];

        for (int i = 0; i < levelBreakers.Length; i++)
        {
            if (i == 0)
            {
                levelBreakers[i] = baseLevelDifficulty;
            }
            else
            {
                levelBreakers[i] = levelBreakers[i - 1] + (baseLevelDifficulty + ((baseLevelDifficulty / 3) * i));
            }

            if (levelInput < levelBreakers[i])
            {
                int getDifficulty = i;
                difficulty = getDifficulty;
                if (difficulty == 0)
                {
                    level = levelInput;
                }
                else
                {
                    level = levelInput - levelBreakers[i - 1];
                }
                break;
            }
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
        return amountOfLevelsToChange;
    }

    #endregion
}
