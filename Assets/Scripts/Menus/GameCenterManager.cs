using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Monetization;
using TMPro;

public class GameCenterManager : MonoBehaviour
{
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
    public GameObject demoPanelObject;
    DemoPanel demoPanel;

    int index;
    List<bool> unlocks;
    List<int> stations = new List<int>();

    string[] scenes = { "Birds_Singing_Scene", "Magic_Sand_Scene", "Treasure_Hunting_Scene", "Monkey_Hiding_Scene", "Magic_River_Scene", "Lava_Game_Scene", "Icecream_Madness" };

    const string folderPath = "Sprites/GameCenter/";
    const string bannerPath = "Banners/GamePanel_";
    const string iconPath = "Icons/Icon_";
    const string screenPath = "Screens/Capture_";

    readonly string urlBuy = $"{Keys.Api_Web_Key}api/store/purchase/";

    AsyncOperation asyncLoad;
    EventTrigger eventTrigger;
    SessionManager sessionManager;
    AudioPlayerForMenus audioPlayer;
    DemoKey key;

    int tPos;
    float firstXPos;
    float secondXPos;

    enum DirectionOfSwipe { Left, Right, None };
    DirectionOfSwipe directtion = DirectionOfSwipe.None;

    // Use this for initialization
    void Start()
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
        loadingCanvas.GetComponentInChildren<TextMeshProUGUI>().text = stringsToShow[12];

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

        currentPanel.playButton.GetComponentInChildren<TextMeshProUGUI>().text = stringsToShow[7];


        ChildGames();
        ShowLoaderCanvas();

        if (FindObjectOfType<DemoKey>())
        {
            key = FindObjectOfType<DemoKey>();
            key.ResetSpecial();
            demoPanel = new DemoPanel(demoPanelObject.transform, true);
            //difficultySlider.value = key.GetDifficulty();
            //difficultySlider.onValueChanged.AddListener(delegate { key.SetDifficulty((int)difficultySlider.value); });
            //flisActivation.isOn = key.IsFLISOn();
            //flisActivation.onValueChanged.AddListener(delegate { key.ChangeFLIS(); });
            //ageButton.onClick.AddListener(ChangeAge);
            //ageValue.placeholder.GetComponent<Text>().text = "Age is " + sessionManager.activeKid.age.ToString("00");
            ChangeMenus();
        }
        else
        {
            demoPanelObject.SetActive(false);
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

    void ChildGames()
    {
        unlocks = new List<bool>();
        for (int i = 0; i < Keys.Number_Of_Games; i++)
        {
            unlocks.Add(false);
        }

        if (!FindObjectOfType<DemoKey>())
        {
            List<int> missions = new List<int>();

            for (int i = 0; i < Keys.Number_Of_Games; i++)
            {
                if (sessionManager.activeKid.gamesUnlocked[i])
                {
                    Debug.Log($"{i} is unlocked ?{sessionManager.activeKid.gamesUnlocked[i]}");
                    unlocks[i] = true;
                    var x = i;
                    missions.Add(x);
                    stations.Add(x);
                }
            }

            Debug.Log($"missions are {missions.Count}");
        }
        else
        {
            for (int i = 0; i < unlocks.Count; i++)
            {
                unlocks[i] = true;
            }
        }

        for (int i = 0; i < Keys.Number_Of_Games; i++)
        {
            var x = i;
            if (stations.Contains(x))
            {
                continue;
            }
            stations.Add(x);
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
    void Update()
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

            int count = Keys.Number_Of_Games;

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

            SetPanel(currentPanel, stations[index], true);
            SetPanel(nextPanel, stations[next]);
            SetPanel(previosPanel, stations[previous]);
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

        int count = Keys.Number_Of_Games;

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

        if (stations.Count > 0)
        {
            SetPanel(currentPanel, stations[index], true);
            SetPanel(nextPanel, stations[next]);
            SetPanel(previosPanel, stations[previous]);
        }
    }

    void SetPanel(GamePanel panel, int number)
    {
        SetPanel(panel, number, false);
    }

    void SetPanel(GamePanel panel, int number, bool isCenter)
    {
        var banner = Resources.Load<Sprite>($"{folderPath}{bannerPath}{number}");
        panel.backgroundPanel.sprite = banner;

        panel.gameText.text = stringsToShow[number];
        panel.barColor.color = barColors[number];

        var icon = Resources.Load<Sprite>($"{folderPath}{iconPath}{number}");
        panel.iconImage.sprite = icon;

        var capture = Resources.Load<Sprite>($"{folderPath}{screenPath}{number}");
        panel.captureImage.sprite = capture;
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
                panel.blockPanel.onClick.AddListener(ShowBuyOptions);
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
            demoPanelObject.SetActive(true);
            backPanel.SetActive(false);
            centerPanel.SetActive(false);
            fowardPanel.SetActive(false);
            warningPanel.SetActive(false);
            goBackButton.gameObject.SetActive(false);
            leftButton.gameObject.SetActive(false);
            rigthButton.gameObject.SetActive(false);
            demoPanel.ShowDemoPanel(LoadNewScene);
        }
        else
        {
            LoadNewScene(scenes[stations[index]]);
        }
    }

    void LoadNewScene(string sceneName)
    {
        Destroy(audioPlayer.gameObject);
        PrefsKeys.SetNextScene(sceneName);
        asyncLoad.allowSceneActivation = true;
    }

    void LoadNewScene(DemoPanel.Difficulty difficulty)
    {
        Debug.Log("Doing some");
        FindObjectOfType<DemoKey>().SetDifficulty(difficulty);
        LoadNewScene(scenes[stations[index]]);
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
        warningPanel.GetComponentInChildren<TextMeshProUGUI>().text = stringsToShow[8];
    }

    public void ShowBuyOptionError() 
    {
        ShowBuyOptions();
        warningPanel.GetComponentInChildren<TextMeshProUGUI>().text = string.Format(stringsToShow[11], stringsToShow[stations[index]]);
    }

    void ShowBuyOptions()
    {
        DeactivateMainPanel();
        loadingCanvas.SetActive(false);
        warningPanel.SetActive(true);
        var button = warningPanel.GetComponentInChildren<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            //Debug.Log($"this is the station {numberOfGame}");
            //warningPanel.SetActive(false);
            //ChangeMenus();
            BuyItem();

        });
        warningPanel.GetComponentInChildren<TextMeshProUGUI>().text = string.Format(stringsToShow[9], stringsToShow[stations[index]]);
        button.GetComponentInChildren<TextMeshProUGUI>().text = stringsToShow[10];
    }

    void DeactivateMainPanel()
    {
        backPanel.SetActive(false);
        centerPanel.SetActive(false);
        fowardPanel.SetActive(false);
        leftButton.gameObject.SetActive(false);
        rigthButton.gameObject.SetActive(false);
        goBackButton.gameObject.SetActive(true);
        goBackButton.onClick.RemoveAllListeners();
        goBackButton.onClick.AddListener(DeactivateDisclaimer);
    }

    void DeactivateDisclaimer()
    {
        backPanel.SetActive(true);
        centerPanel.SetActive(true);
        fowardPanel.SetActive(true);
        leftButton.gameObject.SetActive(true);
        rigthButton.gameObject.SetActive(true);
        goBackButton.gameObject.SetActive(true);
        warningPanel.SetActive(false);
        goBackButton.onClick.RemoveAllListeners();
        goBackButton.onClick.AddListener(GoBackScene);
    }

    void BuyItem()
    {
#if UNITY_ANDROID
        ShowLoaderCanvas();
        var iapManager = FindObjectOfType<MyIAPManager>();
        iapManager.BuyAGame(stations[index]);
#elif UNITY_IOS
        ShowLoaderCanvas();
        var iapManager = FindObjectOfType<MyIAPManager>();
        iapManager.BuyAGame(stations[index]);
#elif UNITY_EDITOR
        ShowLoaderCanvas();
        ShowBuyOptionError();
#else
        Application.OpenURL($"{Keys.Api_Web_Key}tienda");
#endif
    }

    public void SendBuy()
    {
        SendABuy(stations[index]);
    }

    IEnumerator SendABuy(int numberOfGame)
    {
        WWWForm form = new WWWForm();
        form.AddField("userKey", sessionManager.activeUser.userkey);
        form.AddField("cid", sessionManager.activeKid.id);
        form.AddField("games", Keys.Game_Names[numberOfGame]);
        warningPanel.SetActive(false);
        loadingCanvas.SetActive(true);
        goBackButton.gameObject.SetActive(false);

        using (UnityWebRequest request = UnityWebRequest.Post(urlBuy, form))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log("Error");
                Debug.Log(request.downloadHandler.text);
                DeactivateDisclaimer();
                loadingCanvas.SetActive(false);
                sessionManager.activeKid.gamesUnlocked[numberOfGame] = true;
                unlocks[numberOfGame] = true;
                SetPanel(currentPanel, numberOfGame, true);
                sessionManager.SaveSession();
            }
            else
            {
                DeactivateDisclaimer();
                loadingCanvas.SetActive(false);
                sessionManager.activeKid.gamesUnlocked[numberOfGame] = true;
                unlocks[numberOfGame] = true;
                SetPanel(currentPanel, numberOfGame, true);
                sessionManager.SaveSession();
            }
            if (FindObjectOfType<MyIAPManager>()) 
            {
                FindObjectOfType<MyIAPManager>().ConfirmPurchaseProduct();
            }
        }
    }

}

public struct GamePanel
{
    public GamePanel(GameObject gamePanel)
    {
        gameObject = gamePanel;
        button = gameObject.GetComponent<Button>();
        backgroundPanel = gamePanel.GetComponent<Image>();
        gameText = gamePanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        barColor = gamePanel.transform.GetChild(2).GetComponent<Image>();
        iconImage = gamePanel.transform.GetChild(3).GetComponent<Image>();
        captureImage = gamePanel.transform.GetChild(4).GetComponent<Image>();
        playButton = gamePanel.transform.GetChild(5).GetComponent<Button>();
        blockPanel = gamePanel.transform.Find("Block Panel").GetComponent<Button>();
    }

    public GameObject gameObject;
    public Button button;
    public Image backgroundPanel;
    public TextMeshProUGUI gameText;
    public Image barColor;
    public Image iconImage;
    public Image captureImage;
    public Button playButton;
    public Button blockPanel;

}
