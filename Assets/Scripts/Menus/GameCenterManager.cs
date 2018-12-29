using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    int index;
    List<int> stations = new List<int> { 0, 1, 2, 3, 4, 5 };
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
        if (sessionManager.activeKid.needSync)
        {
            Debug.Log("We need to update a profile");
            sessionManager.UpdateProfile(activeMissions);
        }
        if (FindObjectOfType<DemoKey>())
        {
            key = FindObjectOfType<DemoKey>();
            demoPanel.SetActive(true);
            difficultySlider.value = key.GetDifficulty();
            difficultySlider.onValueChanged.AddListener(delegate { key.SetDifficulty((int)difficultySlider.value); });
            flisActivation.isOn = key.IsFLISOn();
            flisActivation.onValueChanged.AddListener(delegate { key.ChangeFLIS(); });
            ageButton.onClick.AddListener(ChangeAge);
            ageValue.placeholder.GetComponent<Text>().text = "Age is " + sessionManager.activeKid.age.ToString("00");
        }
        ChildGames();
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

        ChangeMenus();

        backPanel.gameObject.GetComponent<Button>().onClick.AddListener(() => ChangeMenus(DirectionOfSwipe.Left));
        fowardPanel.gameObject.GetComponent<Button>().onClick.AddListener(() => ChangeMenus(DirectionOfSwipe.Right));

        currentPanel.playButton.GetComponentInChildren<Text>().text = stringsToShow[6];
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
                stations.Clear();
                if (sessionManager.activeKid.birdsFirst)
                {
                    stations.Add(0);
                }
                if (sessionManager.activeKid.sandFirst)
                {
                    stations.Add(1);
                }
                if (sessionManager.activeKid.treasureFirst)
                {
                    stations.Add(2);
                }
                if (sessionManager.activeKid.monkeyFirst)
                {
                    stations.Add(3);
                }
                if (sessionManager.activeKid.riverFirst)
                {
                    stations.Add(4);
                }
                if (sessionManager.activeKid.lavaFirst)
                {
                    stations.Add(5);
                }
            }
            else
            {
                stations.Clear();
                for (int i = 0; i < sessionManager.activeKid.activeMissions.Count; i++)
                {
                    switch (sessionManager.activeKid.activeMissions[i])
                    {
                        case "ArbolMusical":
                            if (sessionManager.activeKid.playedBird == 0)
                            {
                                stations.Add(0);
                                activeMissions.Add("ArbolMusical");
                            }
                            break;
                        case "ArenaMagica":
                            if (sessionManager.activeKid.playedSand == 0)
                            {
                                stations.Add(1);
                                activeMissions.Add("ArenaMagica");
                            }
                            break;
                        case "Tesoro":
                            if (sessionManager.activeKid.playedTreasure == 0)
                            {
                                stations.Add(2);
                                activeMissions.Add("Tesoro");
                            }
                            break;
                        case "DondeQuedoLaBolita":
                            if (sessionManager.activeKid.playedMonkey == 0)
                            {
                                stations.Add(3);
                                activeMissions.Add("DondeQuedoLaBolita");
                            }
                            break;
                        case "Rio":
                            if (sessionManager.activeKid.playedRiver == 0)
                            {
                                stations.Add(4);
                                activeMissions.Add("Rio");
                            }
                            break;
                        case "JuegoDeSombras":
                            if (sessionManager.activeKid.playedLava == 0)
                            {
                                stations.Add(5);
                                activeMissions.Add("JuegoDeSombras");
                            }
                            break;
                        default:
                            if (sessionManager.activeKid.playedBird == 0)
                            {
                                stations.Add(0);
                            }
                            Debug.LogError("Theres no game in that format");
                            break;
                    }
                }
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

    void ChangeMenus()
    {
        if (stations.Count < 1)
        {
            if (sessionManager.activeKid.anyFirstTime)
            {
                LoadNewLogin();
            }
            else
            {
                warningPanel.SetActive(true);
                backPanel.SetActive(false);
                centerPanel.SetActive(false);
                fowardPanel.SetActive(false);
            }
        }
        else
        {
            if (directtion == DirectionOfSwipe.Right)
            {
                index++;
            }
            else if (directtion == DirectionOfSwipe.Left)
            {
                index--;
            }

            int count = stations.Count;

            if (count < 2)
            {
                index = 0;
                previosPanel.theObject.SetActive(false);
                nextPanel.theObject.SetActive(false);
                SetPanel(currentPanel, stations[index], true);
            }
            else if (count < 3)
            {
                if (index == count)
                {
                    index = count - 1;
                }

                if (index == -1)
                {
                    index = 0;
                }

                if (index == 0)
                {
                    previosPanel.theObject.SetActive(false);
                    nextPanel.theObject.SetActive(true);
                    SetPanel(nextPanel, stations[index + 1], false);
                }
                if (index == 1)
                {
                    previosPanel.theObject.SetActive(true);
                    nextPanel.theObject.SetActive(false);
                    SetPanel(previosPanel, stations[index - 1], false);
                }
                SetPanel(currentPanel, stations[index], true);
            }
            else
            {
                if (index == count)
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

                SetPanel(currentPanel, stations[index], true);
                SetPanel(nextPanel, stations[next], false);
                SetPanel(previosPanel, stations[previous], false);
            }
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

        if (count < 2)
        {
            index = 0;
            previosPanel.theObject.SetActive(false);
            nextPanel.theObject.SetActive(false);
            SetPanel(currentPanel, stations[index], true);
        }
        else if (count < 3)
        {
            if (index == count)
            {
                index = count - 1;
            }

            if (index == -1)
            {
                index = 0;
            }

            if (index == 0)
            {
                previosPanel.theObject.SetActive(false);
                nextPanel.theObject.SetActive(true);
                SetPanel(nextPanel, stations[index + 1], false);
            }
            if (index == 1)
            {
                previosPanel.theObject.SetActive(true);
                nextPanel.theObject.SetActive(false);
                SetPanel(previosPanel, stations[index - 1], false);
            }
            SetPanel(currentPanel, stations[index], true);
        }
        else
        {
            if (index == count)
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

            SetPanel(currentPanel, stations[index], true);
            SetPanel(nextPanel, stations[next], false);
            SetPanel(previosPanel, stations[previous], false);
        }
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
    }

    void GoBackScene()
    {
        SceneManager.LoadScene("GameMenus");
        DontDestroyOnLoad(audioPlayer);
    }

    void LoadNewScene()
    {
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
}

public struct GamePanel
{
    public GamePanel(GameObject gamePanel)
    {
        theObject = gamePanel;
        backgroundPanel = gamePanel.GetComponent<Image>();
        gameText = gamePanel.transform.GetChild(1).GetComponent<Text>();
        barColor = gamePanel.transform.GetChild(2).GetComponent<Image>();
        iconImage = gamePanel.transform.GetChild(3).GetComponent<Image>();
        captureImage = gamePanel.transform.GetChild(4).GetComponent<Image>();
        playButton = gamePanel.transform.GetChild(5).GetComponent<Button>();
    }

    public GameObject theObject;
    public Image backgroundPanel;
    public Text gameText;
    public Image barColor;
    public Image iconImage;
    public Image captureImage;
    public Button playButton;
}
