﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameCenterManager : MonoBehaviour {

    [Header("Sprites")]
    public Sprite[] icons;
    public Sprite[] captures;
    public Sprite[] backgrounds;

    [Header("Colors")]
    public Color[] barColors;

    [Header("Panels")]
    public GameObject backPanel;
    public GameObject fowardPanel;
    public GameObject centerPanel;
    public GameObject warningPanel;
    public GameObject loadingCanvas;
    public Button leftButton;
    public Button rigthButton;
    public Button panelLeftButton;
    public Button panelRightButton;

    GamePanel previosPanel;
    GamePanel nextPanel;
    GamePanel currentPanel;

    [Header("Text Needs")]
    public TextAsset textAsset;
    string[] stringsToShow;

    [Header("Buttons")]
    public Button goBackButton;

    [Header("Demo Version")]
    public GameObject demoPanel;
    public Slider difficultySlider;
    public Toggle flisActivation;
    public Button ageButton;
    public InputField ageValue;
    public InputField levelInput;
    public InputField difficultyInput;
    public InputField sandOnly;


    int index;
    List<int> stations = new List<int> { 0, 1, 2, 3, 4, 5 };
    List<bool> unlocks = new List<bool> { false, false, false, false, false, false };
    //int[] stations = new int[] { 2, 3, 5 };
    //int[] stations = new int[] { 0, 1 };
    //int[] stations = new int[] { 4 };

    string[] scenes = new string[] { "Birds_Singing_Scene", "Magic_Sand_Scene", "Treasure_Hunting_Scene", "Monkey_Hiding_Scene", "Magic_River_Scene", "Lava_Game_Scene" };
    List<string> activeMissions = new List<string>();

    AsyncOperation asyncLoad;
    EventTrigger eventTrigger;
    SessionManager sessionManager;
    AudioPlayerForMenus audioPlayer;
    DemoKey key;

    int tPos;
    float firstXPos;
    float secondXPos;

    enum DirectionOfSwipe { Left, Right, None};
    DirectionOfSwipe directtion = DirectionOfSwipe.None;

    // Use this for initialization
    void Start ()
    {
        StartCoroutine(LoadLoader());
        sessionManager = FindObjectOfType<SessionManager>();
        audioPlayer = FindObjectOfType<AudioPlayerForMenus>();
        textAsset = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Menus/GameSelectionIsland");
        stringsToShow = TextReader.TextsToShow(textAsset);
        previosPanel = new GamePanel(backPanel);
        nextPanel = new GamePanel(fowardPanel);
        currentPanel = new GamePanel(centerPanel);
        currentPanel.playButton.onClick.AddListener(LoadNewScene);
        eventTrigger = FindObjectOfType<EventTrigger>();
        tPos = eventTrigger.transform.GetSiblingIndex();

        EventTrigger.Entry t1 = new EventTrigger.Entry();
        t1.eventID = EventTriggerType.BeginDrag;
        t1.callback.AddListener((data) => { FisrtTouch((PointerEventData)data); });
        eventTrigger.triggers.Add(t1);
        EventTrigger.Entry t2 = new EventTrigger.Entry();
        t2.eventID = EventTriggerType.EndDrag;
        t2.callback.AddListener((data) => { SecondTouch((PointerEventData)data); });
        eventTrigger.triggers.Add(t2);

        goBackButton.onClick.AddListener(GoBackScene);


        previosPanel.button.onClick.AddListener(() => ChangeMenus(DirectionOfSwipe.Left));
        nextPanel.button.onClick.AddListener(() => ChangeMenus(DirectionOfSwipe.Right));

        previosPanel.blockPanel.onClick.AddListener(() => ChangeMenus(DirectionOfSwipe.Left));
        nextPanel.blockPanel.onClick.AddListener(() => ChangeMenus(DirectionOfSwipe.Right));

        leftButton.onClick.AddListener(() => ChangeMenus(DirectionOfSwipe.Left));
        rigthButton.onClick.AddListener(() => ChangeMenus(DirectionOfSwipe.Right));

        currentPanel.playButton.GetComponentInChildren<Text>().text = stringsToShow[6];


        ChildGames();
        ShowLoaderCanvas();

        if (FindObjectOfType<DemoKey>())
        {
            key = FindObjectOfType<DemoKey>();
            key.ResetSpecial();
            demoPanel.SetActive(true);
            difficultySlider.value = key.GetDifficulty();
            difficultySlider.onValueChanged.AddListener(delegate { key.SetDifficulty((int)difficultySlider.value); });
            flisActivation.isOn = key.IsFLISOn();
            flisActivation.onValueChanged.AddListener(delegate { key.ChangeFLIS(); });
            ageButton.onClick.AddListener(ChangeAge);
            ageValue.placeholder.GetComponent<Text>().text = "Age is " + sessionManager.activeKid.age.ToString("00");
            ChangeMenus();
        }
        else
        {
            demoPanel.SetActive(false);
            if (sessionManager.activeKid.needSync)
            {
                StartCoroutine(CheckInternetConnection(Keys.Api_Web_Key + Keys.Try_Connection_Key));
            }
            else
            {
                ChangeMenus();
            }
        }

    }

    IEnumerator CheckInternetConnection(string resource)
    {
        WWWForm newForm = new WWWForm();
        using (UnityWebRequest newRequest = UnityWebRequest.Get(resource))
        {
            yield return newRequest.SendWebRequest();

            if (newRequest.isNetworkError || newRequest.isHttpError)
            {
                NotAvailableUpdate();
            }
            else
            {
                InternetAvailableUpdate();
            }
        }
    }

    void InternetAvailableUpdate()
    {
        sessionManager.UpdateProfile(activeMissions);
        int funelGame = PlayerPrefs.GetInt(Keys.Funnel_Games, 1);
        if (funelGame < 7)
        {
            UnityEngine.Analytics.Analytics.CustomEvent($"game{funelGame}");
            PlayerPrefs.SetInt(Keys.Funnel_Games, funelGame + 1);
        }
    }

    void NotAvailableUpdate()
    {
        ChangeMenus();
    }

    void ChangeAge()
    {
        sessionManager.activeKid.age = int.Parse(ageValue.text);
        ageValue.text = "";
        ageValue.placeholder.GetComponent<Text>().text = "Age is " + sessionManager.activeKid.age.ToString("00");
    }

    void ChildGames()
    {
        if (!FindObjectOfType<DemoKey>())
        {
            if (sessionManager.activeKid.anyFirstTime)
            {
                if (sessionManager.activeKid.birdsFirst)
                {
                    unlocks[0] = true;
                }
                if (sessionManager.activeKid.sandFirst)
                {
                    unlocks[1] = true;
                }
                if (sessionManager.activeKid.treasureFirst)
                {
                    unlocks[2] = true;
                }
                if (sessionManager.activeKid.monkeyFirst)
                {
                    unlocks[3] = true;
                }
                if (sessionManager.activeKid.riverFirst)
                {
                    unlocks[4] = true;
                }
                if (sessionManager.activeKid.lavaFirst)
                {
                    unlocks[5] = true;
                }
            }
            else
            {
                for (int i = 0; i < sessionManager.activeKid.activeMissions.Count; i++)
                {
                    switch (sessionManager.activeKid.activeMissions[i])
                    {
                        case Keys.Bird_Game_Name:
                            if (sessionManager.activeKid.playedBird == 0)
                            {
                                activeMissions.Add(Keys.Bird_Game_Name);
                                unlocks[0] = true;
                            }
                            break;
                        case Keys.Sand_Game_Name:
                            if (sessionManager.activeKid.playedSand == 0)
                            {
                                activeMissions.Add(Keys.Sand_Game_Name);
                                unlocks[1] = true;
                            }
                            break;
                        case Keys.Treasure_Game_Name:
                            if (sessionManager.activeKid.playedTreasure == 0)
                            {
                                activeMissions.Add(Keys.Treasure_Game_Name);
                                unlocks[2] = true;
                            }
                            break;
                        case Keys.Monkey_Game_Name:
                            if (sessionManager.activeKid.playedMonkey == 0)
                            {
                                activeMissions.Add(Keys.Monkey_Game_Name);
                                unlocks[3] = true;
                            }
                            break;
                        case Keys.River_Game_Name:
                            if (sessionManager.activeKid.playedRiver == 0)
                            {
                                activeMissions.Add(Keys.River_Game_Name);
                                unlocks[4] = true;
                            }
                            break;
                        case Keys.Lava_Game_Name:
                            if (sessionManager.activeKid.playedLava == 0)
                            {
                                activeMissions.Add(Keys.Lava_Game_Name);
                                unlocks[5] = true;
                            }
                            break;
                        default:
                            if (sessionManager.activeKid.playedBird == 0)
                            {

                            }
                            break;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < unlocks.Count; i++)
            {
                unlocks[i] = true;
            }
        }
    }

    void FisrtTouch(PointerEventData data)
    {   
        firstXPos = Input.mousePosition.x;
    }

    void SecondTouch(PointerEventData data)
    {
        secondXPos = Input.mousePosition.x;
        if (secondXPos < firstXPos)
        {
            directtion = DirectionOfSwipe.Right;
        }
        else if (secondXPos > firstXPos)
        {
            directtion = DirectionOfSwipe.Left;
        }
        else
        {
            directtion = DirectionOfSwipe.None;
        }
        ChangeMenus();
    }

	// Update is called once per frame
	void Update ()
    {

	}

    public void ChangeMenus()
    {
        if (unlocks.Contains(true))
        {
            goBackButton.gameObject.SetActive(true);
            loadingCanvas.SetActive(false);
            backPanel.SetActive(true);
            centerPanel.SetActive(true);
            fowardPanel.SetActive(true);

            if (directtion == DirectionOfSwipe.Right)
            {
                index++;
            }
            else if (directtion == DirectionOfSwipe.Left)
            {
                index--;
            }

            int count = stations.Count;

            if (index >= count)
            {
                index = 0;
            }

            if (index == -1)
            {
                index += count;
            }

            int previous = index - 1;

            if (previous < 0)
            {
                previous += count;
            }

            int next = index + 1;
            if (next >= count)
            {
                next = 0;
            }

            SetPanel(currentPanel, index, true);
            SetPanel(nextPanel, next);
            SetPanel(previosPanel, previous);
        }
        else
        {
            ShowWarning();
        }

    }

    void ChangeMenus(DirectionOfSwipe dir)
    {
        if (dir == DirectionOfSwipe.Right)
        {
            index++;
        }
        else if (dir == DirectionOfSwipe.Left)
        {
            index--;
        }

        int count = stations.Count;

        if (index >= count)
        {
            index = 0;
        }

        if (index == -1)
        {
            index += count;
        }

        int previous = index - 1;

        if (previous == -1)
        {
            previous += count;
        }

        int next = index + 1;
        if (next >= count)
        {
            next = 0;
        }

        SetPanel(currentPanel, index, true);
        SetPanel(nextPanel, next);
        SetPanel(previosPanel, previous);
    }

    void SetPanel(GamePanel panel, int number)
    {
        SetPanel(panel, number, false);
    }

    void SetPanel(GamePanel panel, int number, bool isCenter)
    {
        panel.backgroundPanel.sprite = backgrounds[number];
        panel.gameText.text = stringsToShow[number];
        panel.barColor.color = barColors[number];
        panel.iconImage.sprite = icons[number];
        panel.captureImage.sprite = captures[number];
        if (isCenter)
        {
            panel.playButton.gameObject.SetActive(true);
        }
        else
        {
            panel.playButton.gameObject.SetActive(false);

        }

        if (unlocks[number])
        {
            panel.blockPanel.gameObject.SetActive(false);
        }
        else
        {
            panel.blockPanel.gameObject.SetActive(true);
            panel.playButton.gameObject.SetActive(false);
            if (isCenter)
            {
                panel.blockPanel.onClick.RemoveAllListeners();
                panel.blockPanel.onClick.AddListener(ShowDisclaimer);
            }
        }
    }

    void GoBackScene()
    {
        SceneManager.LoadScene("GameMenus");
        DontDestroyOnLoad(audioPlayer);
    }

    void LoadNewScene()
    {
        if (FindObjectOfType<DemoKey>())
        {
            int change = 0;

            int level = 0;
            int difficulty = 0;
            int specialLevel = 0;

            if (levelInput.text != "")
            {
                level = int.Parse(levelInput.text);
                change++;
            }

            if (difficultyInput.text != "")
            {
                difficulty = int.Parse(difficultyInput.text);
                change++;
            }

            if (stations[index] == 1)
            {
                if (sandOnly.text != "")
                {
                    specialLevel = int.Parse(sandOnly.text);
                    change++;
                }
            }
            if (change > 0)
            {
                FindObjectOfType<DemoKey>().SetSpecialLevels(level, difficulty, specialLevel);
            }
        }

        Destroy(audioPlayer.gameObject);
        PrefsKeys.SetNextScene(scenes[stations[index]]);
        asyncLoad.allowSceneActivation = true;
    }

    void LoadNewLogin()
    {
        Destroy(audioPlayer.gameObject);
        PrefsKeys.SetNextScene("NewLogin");
        asyncLoad.allowSceneActivation = true;
    }

    IEnumerator LoadLoader()
    {
        asyncLoad = SceneManager.LoadSceneAsync("Loader_Scene");
        asyncLoad.allowSceneActivation = false;
        yield return asyncLoad;
    }

    void ShowLoaderCanvas()
    {
        backPanel.SetActive(false);
        centerPanel.SetActive(false);
        fowardPanel.SetActive(false);
        warningPanel.SetActive(false);
        goBackButton.gameObject.SetActive(false);
        loadingCanvas.SetActive(true);
    }

    void ShowWarning()
    {
        loadingCanvas.SetActive(false);
        warningPanel.SetActive(true);
        warningPanel.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        warningPanel.GetComponentInChildren<Button>().onClick.AddListener(GoBackScene);
        warningPanel.GetComponentInChildren<Text>().text = stringsToShow[7];
    }

    void ShowDisclaimer()
    {
        loadingCanvas.SetActive(false);
        warningPanel.SetActive(true);
        warningPanel.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        warningPanel.GetComponentInChildren<Button>().onClick.AddListener(()=>
        {
            warningPanel.SetActive(false);
            ChangeMenus();

        });
        warningPanel.GetComponentInChildren<Text>().text = stringsToShow[8];
    }
}

public struct GamePanel
{
    public GamePanel(GameObject gamePanel)
    {
        gameObject = gamePanel;
        button = gameObject.GetComponent<Button>();
        backgroundPanel = gamePanel.GetComponent<Image>();
        gameText = gamePanel.transform.GetChild(1).GetComponent<Text>();
        barColor = gamePanel.transform.GetChild(2).GetComponent<Image>();
        iconImage = gamePanel.transform.GetChild(3).GetComponent<Image>();
        captureImage = gamePanel.transform.GetChild(4).GetComponent<Image>();
        playButton = gamePanel.transform.GetChild(5).GetComponent<Button>();
        blockPanel = gamePanel.transform.Find("Block Panel").GetComponent<Button>();
    }

    public GameObject gameObject;
    public Button button;
    public Image backgroundPanel;
    public Text gameText;
    public Image barColor;
    public Image iconImage;
    public Image captureImage;
    public Button playButton;
    public Button blockPanel;

}
