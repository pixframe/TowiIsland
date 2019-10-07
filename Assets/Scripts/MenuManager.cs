using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{

    //this assets contains all the information that we need to create the text of the first menu
    [Header("Texts To Show")]
    TextAsset loginTextAsset;
    TextAsset textOfAll;
    TextAsset textBefore;
    TextAsset textAddable;
    TextAsset warningAsset;
    string[] lines;

    //this region contains all the ui elements of this menu
    #region UI Elements
    [Header("Game UI")]
    public GameObject gameCanvas;
    public Button escapeButton;
    GameMenu gameMenuObject;

    public GameObject registerCanvas;
    RegisterMenu registerMenu;

    public GameObject addCanvas;
    AddMenu addMenu;

    [Header("Log in UI")]
    public GameObject kidsPanel;
    public GameObject warningPanel;

    [Header("Kid Selector")]
    public GameObject miniKidCanvas;
    public GameObject miniKidContainer;
    public TextMeshProUGUI selectionKidText;
    public Button selectionKidBackButton;
    public Button addKidButton;
    public TMP_InputField kidLooker;

    [Header("Credits")]
    public GameObject creditCanvas;
    public Button exitCredits;
    public TextMeshProUGUI creditText;
    public TextMeshProUGUI creditColumOne;
    public TextMeshProUGUI creditColumTwo;

    [Header("Subscribe")]
    public GameObject subscribeCanvas;
    public Text subscribeText;
    public Button subscribeButton;
    public Button subscribeAnotherCountButton;
    public Button changeProfileButton;
    public Button continueEvaluationButton;
    public Button escapeEvaluationButton;
    public Button subscribeBackButton;
    public Image suscripctionLogo;
    public Image warningLogo;

    [Header("Shop Button")]
    public GameObject shopCanvas;
    ShopMenu shopMenu;

    [Header("Warnings")]
    public Text warningText;
    public Button warningButton;
    string[] warningLines;

    [Header("New Kid")]
    public GameObject newKidPanel;
    public InputField newKidNameInput;
    public InputField newKidDay;
    public Text newKidBirthday;
    public InputField newKidMonth;
    public InputField newKidYear;
    public Button newKidButton;
    public Button newKidBackButton;

    [Header("Loading")]
    public GameObject loadingCanvas;
    public Text loadingText;

    [Header("Config")]
    public GameObject configCanvas;
    ConfigMenu configMenu;
    #endregion

    AudioManager audioManager;
    EvaluationController evaluationController;

    DemoKey key;
    LogInScript logInScript;
    SessionManager sessionManager;
    MyIAPManager myIAPManager;
    SubscriptionsWays subscriptionsManager;

    int[] dobYMD;
    static bool alreadyLogged = false;

    string subscriptionIAPType;
    int numKids;
    int monthsOfSubs;
    int availableKidsInSubscription;
    int logoPushes;

    List<int> ids = new List<int>();
    List<Button> miniCanvitas = new List<Button>();

    bool needInternetConectionNow = false;

    void Awake()
    {
        //here we start the process of initrilization
        Initialization();
        dobYMD = new int[0];
        Analytics.CustomEvent("open");
        //PlayerPrefs.SetInt(Keys.First_Try, 0);
    }

    // Use this for initialization
    void Start()
    {
        ShowLoading();

        if (key == null)
        {
            StartCoroutine(CheckInternetConnection(Keys.Api_Web_Key + Keys.Try_Connection_Key));
        }
        else
        {
            ShowGameMenu();
        }
    }

    IEnumerator CheckInternetConnection(string resource)
    {
        WWWForm newForm = new WWWForm();

        using (UnityWebRequest newRequest = UnityWebRequest.Get(resource))
        {
            yield return newRequest.SendWebRequest();

            if (newRequest.isNetworkError)
            {
                NoInternetAvailableLogin();
            }
            else
            {
                InternetAvailableLogin();
            }
        }
    }

    void InternetAvailableLogin()
    {
        PlayerPrefs.SetString(Keys.Last_Time_Were, DateTime.Today.ToString(DateTimeFormatInfo.InvariantInfo));
        if (PlayerPrefs.GetInt(Keys.Logged_Session) == 0)
        {

            if (PlayerPrefs.GetInt(Keys.Logged_In) == 1)
            {
                string user = PlayerPrefs.GetString(Keys.Active_User_Key);
                if (user != "_local")
                {
                    if (user != "")
                    {
                        logInScript.IsActive(user);
                    }
                    else
                    {
                        ShowFirstMenu();
                    }
                }
                else
                {

                    ShowGameMenu();
                    PlayerPrefs.SetInt(Keys.Logged_Session, 1);
                }
            }
            else
            {
                ShowFirstMenu();
            }
        }
        else
        {
            if (sessionManager.activeKid != null)
            {
                if (sessionManager.activeKid.offlineSubscription < DateTime.Today)
                {
                    string user = PlayerPrefs.GetString(Keys.Active_User_Key);
                    logInScript.IsActive(user);
                }
                else
                {
                    ShowGameMenu();
                }
            }
            else
            {
                SetKidsProfiles();
            }
        }
    }

    void NoInternetAvailableLogin()
    {
        DateTime lastSession;
        if (PlayerPrefs.GetString(Keys.Last_Time_Were) == "")
        {
            lastSession = DateTime.Today.Subtract(TimeSpan.FromDays(1));
        }
        else
        {
            lastSession = DateTime.Parse(PlayerPrefs.GetString(Keys.Last_Time_Were), DateTimeFormatInfo.InvariantInfo);
        }
        Debug.Log($"Last time in internte was {lastSession.ToUniversalTime()} and today is { DateTime.Today.ToUniversalTime()} compare to today is {DateTime.Compare(DateTime.Today, lastSession)}");
        if (DateTime.Compare(DateTime.Today, lastSession) >= 0)
        {
            PlayerPrefs.SetString(Keys.Last_Time_Were, DateTime.Today.ToString(DateTimeFormatInfo.InvariantInfo));
            if (PlayerPrefs.GetInt(Keys.Logged_Session) == 0)
            {
                if (PlayerPrefs.GetInt(Keys.Logged_In) == 1)
                {
                    if (DateTime.Compare(sessionManager.activeKid.offlineSubscription, DateTime.Today) < 0)
                    {
                        Debug.Log("here are less");
                        ShowNeedConectionToPlay();
                    }
                    else
                    {
                        DateTime lastFetchTime = DateTime.Parse(PlayerPrefs.GetString(Keys.Last_Play_Time), DateTimeFormatInfo.InvariantInfo);

                        if (DateTime.Compare(DateTime.Today, lastFetchTime) > 0 && sessionManager.activeKid.missionsToPlay.Count <= 0)
                        {
                            Debug.Log("Here we create a new activities");
                            sessionManager.activeKid.missionsToPlay = OfflineManager.Create_Levels();
                        }
                        ShowGameMenu();
                    }
                }
                else
                {
                    ShowFirstMenu();
                }
            }
            else
            {
                if (PlayerPrefs.GetInt(Keys.Logged_In) == 1)
                {
                    if (DateTime.Compare(sessionManager.activeKid.offlineSubscription, DateTime.Today) < 0)
                    {
                        ShowNeedConectionToPlay();
                    }
                    else
                    {

                        DateTime lastFetchTime = DateTime.Parse(PlayerPrefs.GetString(Keys.Last_Play_Time), DateTimeFormatInfo.InvariantInfo);


                        if (DateTime.Compare(DateTime.Today, lastFetchTime) > 0 && sessionManager.activeKid.missionsToPlay.Count <= 0)
                        {
                            if (DateTime.Compare(DateTime.Today, lastFetchTime) > 0)
                            {
                                sessionManager.activeKid.missionsToPlay = OfflineManager.Create_Levels();
                            }
                        }
                        ShowGameMenu();
                    }
                }
                else
                {
                    ShowFirstMenu();
                }
            }
        }
        else
        {
            Debug.Log("theres no today");
            ShowNeedConectionToPlay();
        }

    }

    #region Functions

    #region Set Up Functions

    // in this process we set all the things that neeed to be set to make this script work 
    void Initialization()
    {
        logInScript = GetComponent<LogInScript>();
        subscriptionsManager = GetComponent<SubscriptionsWays>();
        sessionManager = FindObjectOfType<SessionManager>();
        myIAPManager = FindObjectOfType<MyIAPManager>();
        configMenu = new ConfigMenu(configCanvas);
        UpdateTexts();
        gameMenuObject = new GameMenu(gameCanvas, this);
        shopMenu = new ShopMenu(shopCanvas, this);
        registerMenu = new RegisterMenu(registerCanvas, this);
        addMenu = new AddMenu(addCanvas, this);

        ButtonSetUp();

        if (FindObjectOfType<DemoKey>())
        {
            key = FindObjectOfType<DemoKey>();
        }

        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            escapeButton.gameObject.SetActive(false);
        }
    }

    void UpdateTexts()
    {
        SetLanguageResources();
        WriteTheTexts();
    }

    void UpdateTexts(bool upadateLanguages)
    {
        SetLanguageResources(upadateLanguages);
        WriteTheTexts();
        shopMenu.SetStaticTexts();
    }

    public void ClearInputs()
    {
        registerMenu.ClearInputData();
        newKidNameInput.text = "";
        newKidDay.text = "";
        newKidMonth.text = "";
        newKidYear.text = "";
    }

    void SetLanguageResources()
    {
        loginTextAsset = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Login/Login");
        warningAsset = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Login/Warnings");
        textOfAll = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Login/CommonStrings");
        textAddable = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Login/AddableStrings");
        textBefore = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Login/BeforeStrings");

        TextReader.FillCommon(textOfAll);
        TextReader.FillAddables(textAddable);
        TextReader.FillBefore(textBefore);
        lines = TextReader.TextsToShow(loginTextAsset);
        warningLines = TextReader.TextsToShow(warningAsset);
    }

    void SetLanguageResources(bool updateLanguages)
    {
        loginTextAsset = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Login/Login");
        warningAsset = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Login/Warnings");
        textOfAll = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Login/CommonStrings");
        textAddable = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Login/AddableStrings");
        textBefore = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Login/BeforeStrings");

        TextReader.FillCommon(textOfAll, updateLanguages);
        TextReader.FillAddables(textAddable, updateLanguages);
        TextReader.FillBefore(textBefore, updateLanguages);
        lines = TextReader.TextsToShow(loginTextAsset);
        warningLines = TextReader.TextsToShow(warningAsset);
    }

    //here we set almost every button in the ui with the correspondent function to do
    void ButtonSetUp()
    {
        exitCredits.onClick.AddListener(ShowGameMenu);
        selectionKidBackButton.onClick.AddListener(CloseKids);
        newKidBackButton.onClick.AddListener(CloseKids);
        subscribeAnotherCountButton.onClick.AddListener(CloseSession);
        addKidButton.onClick.AddListener(AddKidShower);
        changeProfileButton.onClick.AddListener(SetKidsProfiles);
        newKidButton.onClick.AddListener(CreateAKid);
        escapeButton.onClick.AddListener(ShowTheEscapeApp);
        configMenu.languageButton.onClick.AddListener(ShowLenguages);
        configMenu.englishLanguageButton.onClick.AddListener(() => SetLanguageOfGame(configMenu.englishLanguageButton.transform.GetSiblingIndex()));
        configMenu.spanishLanguageButton.onClick.AddListener(() => SetLanguageOfGame(configMenu.spanishLanguageButton.transform.GetSiblingIndex()));
        configMenu.automaticButton.onClick.AddListener(SetDeviceLanguage);
        configMenu.logoButton.onClick.AddListener(ShowTextRoute);
        configMenu.updateDataButton.onClick.AddListener(logInScript.UpdateData);
        kidLooker.onValueChanged.AddListener(delegate { UpdateKids(); });
    }

    #endregion

    #region Set Functions

    //we show the kids that are available for the player
    public void SetKidsProfiles()
    {
        WriteTheText(selectionKidText, 23);
        HideAllCanvas();
        kidsPanel.SetActive(true);

        kidLooker.text = "";

        WriteTheText(addKidButton, 30, 0);
        addKidButton.onClick.RemoveAllListeners();
        addKidButton.onClick.AddListener(AddKidShower);
        selectionKidBackButton.gameObject.SetActive(true);

        if (sessionManager.activeUser != null)
        {
            UpdateKids();
        }

        sessionManager.SaveSession();
    }

    void UpdateKids()
    {
        List<KidProfileCanvas> kidos = new List<KidProfileCanvas>();
        float deltaSize = miniKidContainer.GetComponent<RectTransform>().sizeDelta.x;
        miniKidContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(0, miniKidContainer.GetComponent<RectTransform>().sizeDelta.y);
        for (int i = 0; i < miniKidContainer.transform.childCount; i++)
        {
            Destroy(miniKidContainer.transform.GetChild(i).gameObject);
        }
        int kidsNumber = sessionManager.activeUser.kids.Count;
        var kidsToShow = sessionManager.activeUser.kids.FindAll(kid => {
            var kidName = kid.name.ToLowerInvariant();
            var lookingKid = kidLooker.text.ToLowerInvariant();
            return kidName.Contains(lookingKid);
        });

        if (kidsToShow.Count > 0)
        {
            HideWarning();
            for (int i = 0; i < kidsToShow.Count; i++)
            {
                GameObject objectToInstance = Instantiate(miniKidCanvas, miniKidContainer.transform);
                kidos.Add(new KidProfileCanvas(objectToInstance));
                float addableSize = kidos[i].gameObject.GetComponent<RectTransform>().sizeDelta.x * 2f;
                miniKidContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(miniKidContainer.GetComponent<RectTransform>().sizeDelta.x + addableSize, miniKidContainer.GetComponent<RectTransform>().sizeDelta.y);
                //kidos[i].gameObject.GetComponent<RectTransform>().parent = miniKidContainer.GetComponent<RectTransform>();
                kidos[i].gameObject.GetComponent<RectTransform>().SetParent(miniKidContainer.GetComponent<RectTransform>());
                if (i > 0)
                {
                    Vector2 pos = kidos[i - 1].gameObject.GetComponent<RectTransform>().localPosition;
                    float positionOfCanvitas = pos.x + addableSize;
                    kidos[i].gameObject.GetComponent<RectTransform>().localPosition = new Vector2(positionOfCanvitas, pos.y);
                }
                else
                {
                    float positionOfCanvitas = addableSize / 2;
                    kidos[i].gameObject.GetComponent<RectTransform>().localPosition = new Vector2(positionOfCanvitas, kidos[i].gameObject.GetComponent<RectTransform>().localPosition.y);
                }
                kidos[i].SetKidName(kidsToShow[i].name);
                string parentkey = kidsToShow[i].userkey;
                int id = kidsToShow[i].id;
                kidos[i].buttonOfProfile.onClick.AddListener(() => SetKidProfile(parentkey, id));
                kidos[i].ChangeAvatar(kidsToShow[i].avatar);
                if (!kidsToShow[i].isActive)
                {
                    kidos[i].PutInGrey();
                }
            }
        }
        else
        {
            ShowWarning(16);
        }
    }

    //this will set all available kids to show wich of them you will add to your subscription plan
    public void SetKidProfilesToAddASubscription(int numOfKids, string typeOfSubscription)
    {
        WriteTheText(selectionKidText, 24);
        HideAllCanvas();
        kidsPanel.SetActive(true);
        numKids = numOfKids;
        subscriptionIAPType = typeOfSubscription;

        if (sessionManager.activeUser != null)
        {
            WriteTheText(addKidButton, 31, 0);
            addKidButton.onClick.RemoveAllListeners();
            addKidButton.onClick.AddListener(() => ConfirmKidsPurchase());
            selectionKidBackButton.gameObject.SetActive(false);

            float deltaSize = miniKidContainer.GetComponent<RectTransform>().sizeDelta.x;
            miniKidContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(0, miniKidContainer.GetComponent<RectTransform>().sizeDelta.y);
            for (int i = 0; i < miniKidContainer.transform.childCount; i++)
            {
                Destroy(miniKidContainer.transform.GetChild(i).gameObject);
            }

            UpdateKidsToBuy();
        }
        else
        {
            registerMenu.ShowRegisterPanel(true);
        }
        sessionManager.SaveSession();
    }

    void UpdateKidsToBuy()
    {

        List<KidProfileCanvas> kidos = new List<KidProfileCanvas>();

        kidLooker.text = "";

        var kidsToShow = sessionManager.activeUser.kids.FindAll(kid => {
            var kidName = kid.name.ToLowerInvariant();
            var lookingKid = kidLooker.text.ToLowerInvariant();
            return kidName.Contains(lookingKid);
        });

        if (kidsToShow.Count > 0)
        {
            for (int i = 0; i < kidsToShow.Count; i++)
            {
                GameObject theObject = Instantiate(miniKidCanvas, miniKidContainer.transform);
                kidos.Add(new KidProfileCanvas(theObject));
                float addableSize = kidos[i].gameObject.GetComponent<RectTransform>().sizeDelta.x * 2f;
                miniKidContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(miniKidContainer.GetComponent<RectTransform>().sizeDelta.x + addableSize, miniKidContainer.GetComponent<RectTransform>().sizeDelta.y);
                kidos[i].gameObject.GetComponent<RectTransform>().parent = miniKidContainer.GetComponent<RectTransform>();
                if (i > 0)
                {
                    Vector2 pos = kidos[i - 1].gameObject.GetComponent<RectTransform>().localPosition;
                    float positionOfCanvitas = pos.x + addableSize;
                    kidos[i].gameObject.GetComponent<RectTransform>().localPosition = new Vector2(positionOfCanvitas, pos.y);
                }
                else
                {
                    float positionOfCanvitas = addableSize / 2;
                    kidos[i].gameObject.GetComponent<RectTransform>().localPosition = new Vector2(positionOfCanvitas, kidos[i].gameObject.GetComponent<RectTransform>().localPosition.y);
                }
                kidos[i].SetKidName(kidsToShow[i].name);
                string parentkey = kidsToShow[i].userkey;
                int id = kidsToShow[i].id;
                kidos[i].buttonOfProfile.onClick.AddListener(() => SetKidIdToSubscriptionPlan(id, kidos[i]));
                kidos[i].PutInGrey();
                kidos[i].ChangeAvatar(kidsToShow[i].avatar);
            }
        }
        else
        {
            ShowWarning(16);
            Debug.Log("Some");
        }
    }

    //This will set the shop
    public void SetShop(int isAShopForNewKid)
    {
        HideAllCanvas();
        shopMenu.ShowThisMenu();
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            //shopMenu.SetWebShop(isAShopForNewKid);

            //This code is used to try ios shop in the editor but to realease version should be always commented
            shopMenu.oneMonthButton.GetComponentInChildren<Text>().text = $"{lines[33]}\n{myIAPManager.CostInCurrency(1)} {lines[35]}";
            shopMenu.threeMonthButton.GetComponentInChildren<Text>().text = $"{lines[34]}\n{myIAPManager.CostInCurrency(3)} {lines[35]}";
            shopMenu.SetIOSShop(isAShopForNewKid, myIAPManager.IsInitialized());
        }
        else
        {
            shopMenu.oneMonthButton.GetComponentInChildren<Text>().text = $"{lines[33]}\n{myIAPManager.CostInCurrency(1)} {lines[35]}";
            shopMenu.threeMonthButton.GetComponentInChildren<Text>().text = $"{lines[34]}\n{myIAPManager.CostInCurrency(3)} {lines[35]}";
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                shopMenu.SetIOSShop(isAShopForNewKid, myIAPManager.IsInitialized());
            }
            else
            {
                shopMenu.SetAndroidShop(isAShopForNewKid);
            }
            //TODO get exeptions
            /*if (sessionManager.activeUser.isPossibleBuyIAP)
            {

            }
            else
            {
                oneMonthButton.gameObject.SetActive(false);
                threeMonthButton.gameObject.SetActive(false);
                shopInWeb.gameObject.SetActive(true);
            }*/
        }
    }

    #endregion

    #region Show Functions

    public void HideAllCanvas()
    {
        gameMenuObject.HideThisMenu();
        shopMenu.HideThisMenu();
        kidsPanel.SetActive(false);
        creditCanvas.SetActive(false);
        subscribeCanvas.SetActive(false);
        warningPanel.SetActive(false);
        loadingCanvas.SetActive(false);
        newKidPanel.SetActive(false);
        escapeButton.gameObject.SetActive(false);
        configMenu.panel.SetActive(false);
        registerMenu.HideAll();
        addMenu.HidePanel();
    }

    void UpdateKidInMenu()
    {
        gameMenuObject.kidProfile.SetKidName(sessionManager.activeKid.name);
        gameMenuObject.kidProfile.ChangeAvatar(sessionManager.activeKid.avatar);
    }

    public void ShowEscapeButton()
    {
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            escapeButton.gameObject.SetActive(false);
        }
        else
        {
            escapeButton.gameObject.SetActive(true);
        }
    }

    //we show the game menu if the player has acces to it
    public void ShowGameMenu()
    {
        if (!needInternetConectionNow)
        {
            if (sessionManager.activeKid != null)
            {
                StartCoroutine(ShowMenuInCorrectTime());
            }
            else
            {
                ShowFirstMenu();
            }
        }
        else
        {
            ShowNeedConectionToPlay();
        }
    }

    IEnumerator ShowMenuInCorrectTime()
    {
        while (sessionManager.IsDownlodingData())
        {
            yield return null;
        }

        PlayerPrefs.SetString(Keys.Last_Play_Time, DateTime.Today.ToString(DateTimeFormatInfo.InvariantInfo));
        HideAllCanvas();
        if (key)
        {
            gameMenuObject.ShowThisMenu(true, false, IsEvaluationAvilable(), true, 0);
        }
        else
        {
            gameMenuObject.ShowThisMenu(sessionManager.activeKid.isActive, PlayerPrefs.GetInt(Keys.First_Try) == 0, IsEvaluationAvilable(), false, sessionManager.activeUser.suscriptionsLeft);
        }

        UpdateKidInMenu();
        ShowEscapeButton();
    }

    void ShowNeedConectionToPlay()
    {
        HideAllCanvas();
        gameMenuObject.ShowThisMenu();
        UpdateKidInMenu();
        ShowEscapeButton();
        needInternetConectionNow = true;
    }

    //we show the log in menu for if the player has not sing in on log in
    public void ShowFirstMenu()
    {
        HideAllCanvas();
        gameMenuObject.ShowFirstMenu();
        ShowEscapeButton();
    }

    public void ShowTryChances()
    {
        HideAllCanvas();
        registerMenu.ShowTryPanel();
    }

    public void ShowTryChancesRegistered()
    {
        HideAllCanvas();
        registerMenu.ShowTryPanelRegistered();
    }

    public void ShowAdd()
    {
        HideAllCanvas();
        addMenu.ShowAdd();
    }

    public void ShowRegisteredAdd()
    {
        HideAllCanvas();
        addMenu.ShowAddRegister();
    }

    public void ShowLogIn()
    {
        HideAllCanvas();
        registerMenu.ShowLoginPanel();
        gameMenuObject.HideThisMenu();
    }

    //we give the user the create account format
    public void CreateAccount()
    {
        HideAllCanvas();
    }

    public void ShowDisclaimer()
    {
        if (SystemInfo.deviceType == DeviceType.Handheld && !IsTablet())
        {
            ShowBiggerScreenMessage();
        }
        else
        {
            ShowTheDisclaimer();
        }
    }

    void ShowBiggerScreenMessage()
    {
        HideAllCanvas();
        subscribeCanvas.SetActive(true);
        subscribeAnotherCountButton.gameObject.SetActive(false);
        changeProfileButton.gameObject.SetActive(false);
        subscribeButton.gameObject.SetActive(false);
        continueEvaluationButton.gameObject.SetActive(false);
        escapeEvaluationButton.gameObject.SetActive(true);
        WriteTheText(subscribeText, 45);
        warningLogo.gameObject.SetActive(true);
        suscripctionLogo.gameObject.SetActive(false);
        WriteTheText(escapeEvaluationButton, 41);
        WriteTheText(continueEvaluationButton, 42);
        escapeEvaluationButton.onClick.RemoveAllListeners();
        escapeEvaluationButton.onClick.AddListener(ShowGameMenu);
        subscribeBackButton.onClick.RemoveAllListeners();
        subscribeBackButton.onClick.AddListener(ShowGameMenu);
    }

    public void ShowYouHaveASuscription()
    {
        HideAllCanvas();
        subscribeCanvas.SetActive(true);
        subscribeAnotherCountButton.gameObject.SetActive(false);
        changeProfileButton.gameObject.SetActive(false);
        subscribeButton.gameObject.SetActive(false);
        continueEvaluationButton.gameObject.SetActive(false);
        escapeEvaluationButton.gameObject.SetActive(true);
        WriteTheText(subscribeText, 58);
        warningLogo.gameObject.SetActive(true);
        suscripctionLogo.gameObject.SetActive(false);
        WriteTheText(escapeEvaluationButton, 41);
        WriteTheText(continueEvaluationButton, 42);
        escapeEvaluationButton.onClick.RemoveAllListeners();
        escapeEvaluationButton.onClick.AddListener(ShowGameMenu);
        subscribeBackButton.onClick.RemoveAllListeners();
        subscribeBackButton.onClick.AddListener(ShowGameMenu);
    }

    void ShowTheDisclaimer()
    {
        HideAllCanvas();
        subscribeCanvas.SetActive(true);
        subscribeAnotherCountButton.gameObject.SetActive(false);
        changeProfileButton.gameObject.SetActive(false);
        subscribeButton.gameObject.SetActive(false);
        continueEvaluationButton.gameObject.SetActive(true);
        escapeEvaluationButton.gameObject.SetActive(true);
        WriteTheText(subscribeText, 26);
        warningLogo.gameObject.SetActive(true);
        suscripctionLogo.gameObject.SetActive(false);
        WriteTheText(escapeEvaluationButton, 41);
        WriteTheText(continueEvaluationButton, 42);
        continueEvaluationButton.onClick.RemoveAllListeners();
        escapeEvaluationButton.onClick.RemoveAllListeners();
        continueEvaluationButton.onClick.AddListener(LoadEvaluation);
        escapeEvaluationButton.onClick.AddListener(ShowGameMenu);
        subscribeBackButton.onClick.RemoveAllListeners();
        subscribeBackButton.onClick.AddListener(ShowGameMenu);
    }

    public void ShowTheEvaluationNeeds()
    {
        HideAllCanvas();
        subscribeCanvas.SetActive(true);
        subscribeAnotherCountButton.gameObject.SetActive(false);
        changeProfileButton.gameObject.SetActive(false);
        subscribeButton.gameObject.SetActive(false);
        continueEvaluationButton.gameObject.SetActive(false);
        escapeEvaluationButton.gameObject.SetActive(true);
        WriteTheText(subscribeText, 63);
        warningLogo.gameObject.SetActive(true);
        suscripctionLogo.gameObject.SetActive(false);
        WriteTheText(escapeEvaluationButton, 41);
        escapeEvaluationButton.onClick.RemoveAllListeners();
        escapeEvaluationButton.onClick.AddListener(ShowGameMenu);
        subscribeBackButton.onClick.RemoveAllListeners();
        subscribeBackButton.onClick.AddListener(ShowGameMenu);
    }

    public void ShowSingOutWarning()
    {
        HideAllCanvas();
        subscribeCanvas.SetActive(true);
        subscribeAnotherCountButton.gameObject.SetActive(false);
        changeProfileButton.gameObject.SetActive(false);
        subscribeButton.gameObject.SetActive(false);
        continueEvaluationButton.gameObject.SetActive(true);
        escapeEvaluationButton.gameObject.SetActive(true);
        WriteTheText(subscribeText, 53);
        warningLogo.gameObject.SetActive(true);
        suscripctionLogo.gameObject.SetActive(false);
        WriteTheText(escapeEvaluationButton, 54);
        WriteTheText(continueEvaluationButton, 55);
        continueEvaluationButton.onClick.RemoveAllListeners();
        escapeEvaluationButton.onClick.RemoveAllListeners();
        continueEvaluationButton.onClick.AddListener(ShowGameMenu);
        escapeEvaluationButton.onClick.AddListener(CloseSession);
        subscribeBackButton.onClick.RemoveAllListeners();
        subscribeBackButton.onClick.AddListener(ShowGameMenu);
    }

    void ShowTheEscapeApp()
    {
        HideAllCanvas();
        subscribeCanvas.SetActive(true);
        subscribeAnotherCountButton.gameObject.SetActive(false);
        changeProfileButton.gameObject.SetActive(false);
        subscribeButton.gameObject.SetActive(false);
        continueEvaluationButton.gameObject.SetActive(true);
        escapeEvaluationButton.gameObject.SetActive(true);
        WriteTheText(subscribeText, 56);
        warningLogo.gameObject.SetActive(true);
        suscripctionLogo.gameObject.SetActive(false);
        WriteTheText(escapeEvaluationButton, 54);
        WriteTheText(continueEvaluationButton, 55);
        continueEvaluationButton.onClick.RemoveAllListeners();
        escapeEvaluationButton.onClick.RemoveAllListeners();
        subscribeBackButton.onClick.RemoveAllListeners();
        if (PlayerPrefs.GetInt(Keys.Logged_In) == 1)
        {
            continueEvaluationButton.onClick.AddListener(ShowGameMenu);
            subscribeBackButton.onClick.AddListener(ShowGameMenu);
        }
        else
        {
            continueEvaluationButton.onClick.AddListener(ShowFirstMenu);
            subscribeBackButton.onClick.AddListener(ShowFirstMenu);
        }
        escapeEvaluationButton.onClick.AddListener(EscapeApplication);
    }

    //We show the player that his accoount has not that privelage
    public void ShowAccountWarning(int typeOfWarning)
    {
        HideAllCanvas();
        subscribeCanvas.SetActive(true);
        subscribeButton.onClick.RemoveAllListeners();
        subscribeAnotherCountButton.gameObject.SetActive(true);
        changeProfileButton.gameObject.SetActive(true);
        subscribeButton.gameObject.SetActive(true);
        continueEvaluationButton.gameObject.SetActive(false);
        escapeEvaluationButton.gameObject.SetActive(false);
        warningLogo.gameObject.SetActive(false);
        suscripctionLogo.gameObject.SetActive(true);
        subscribeBackButton.onClick.RemoveAllListeners();
        subscribeBackButton.onClick.AddListener(ShowGameMenu);
        WriteTheText(subscribeButton, 29);
        if (sessionManager.activeUser.suscriptionsLeft < 1)
        {
            if (typeOfWarning == 0)
            {
                subscribeButton.onClick.AddListener(ShowRegisteredAdd);
                WriteTheText(subscribeText, 25);
            }
            else if (typeOfWarning == 1)
            {
                subscribeButton.onClick.AddListener(() => ShowShop(1));
                WriteTheText(subscribeText, 27);
            }
        }
        else
        {
            subscribeButton.onClick.AddListener(GiveASuscription);
            WriteTheText(subscribeText, 28);
        }

    }

    public void ShowRegister(bool isIAP)
    {
        registerMenu.ShowRegisterPanel(isIAP);
    }

    //we shoe the genius that make this game
    public void ShowCredits()
    {
        HideAllCanvas();
        creditCanvas.SetActive(true);
    }

    //we show the shop and gives them the option to buy
    public void ShowShop(int shopForNewKid)
    {
        HideAllCanvas();
        shopMenu.ShowThisMenu();
        SetShop(shopForNewKid);
    }

    //we start the proceess of purchasing a suscription with a prepaid code
    public void ShopWithCode(int showShop)
    {
        HideAllCanvas();
        shopMenu.ShowThisMenu();
        shopMenu.SetChangePrepaidCode(showShop);
    }

    //we let the player to shop with the InAppPurchase system this only works up to 5 kids
    public void ShopNumOfKids(int showShop, int months)
    {
        HideAllCanvas();
        shopMenu.ShowThisMenu();
        shopMenu.SetNumberShopKid(showShop);
        monthsOfSubs = months;
    }

    public void ShopNumOfKids()
    {
        HideAllCanvas();
        shopMenu.ShowThisMenu();
    }

    public void ShopIAP()
    {
        ShowLoading();
        int numberOfKids = shopMenu.kidsNumberDropdown.value + 1;
        if (monthsOfSubs == 1)
        {
            myIAPManager.BuySubscriptionOneMonth(numberOfKids);
        }
        else
        {
            myIAPManager.BuySubscriptionThreeMonths(numberOfKids);
        }
    }

    //This one is used to add kids
    public void AddKidShower()
    {
        HideAllCanvas();
        if (sessionManager.activeUser.suscriptionsLeft > 0)
        {
            newKidPanel.SetActive(true);
        }
        else
        {
            ShowAccountWarning(1);
        }
    }

    //this is a image that shows tha the game is donwloading the data for the backend to give an answer
    public void ShowLoading()
    {
        HideAllCanvas();
        loadingCanvas.SetActive(true);
    }

    public void ShowSettings()
    {
        HideAllCanvas();
        configMenu.panel.SetActive(true);
        configMenu.languageButtonHandler.SetActive(false);
        configMenu.logoButton.gameObject.SetActive(true);
        configMenu.languageButton.gameObject.SetActive(true);
        logoPushes = 0;
        configMenu.backButton.onClick.RemoveAllListeners();
        configMenu.backButton.onClick.AddListener(ShowGameMenu);
        configMenu.versionText.text = $"V. {Application.version}";
        Debug.Log(PlayerPrefs.GetInt(Keys.Games_Saved) + " " + PlayerPrefs.GetInt(Keys.Evaluations_Saved));
        if (PlayerPrefs.GetInt(Keys.Games_Saved) != 0 || PlayerPrefs.GetInt(Keys.Evaluations_Saved) != 0)
        {
            configMenu.updateDataButton.gameObject.SetActive(true);
        }
        else
        {
            configMenu.updateDataButton.gameObject.SetActive(false);
        }
    }

    void ShowLenguages()
    {
        configMenu.languageButtonHandler.SetActive(true);
        configMenu.languageButton.gameObject.SetActive(false);
        configMenu.backButton.onClick.RemoveAllListeners();
        configMenu.backButton.onClick.AddListener(ShowSettings);
    }

    public void ShowTextRoute()
    {
        logoPushes++;
        if (logoPushes > 11 && logoPushes < 13)
        {
            TextEditor textEditor = new TextEditor
            {
                text = Application.persistentDataPath
            };
            textEditor.SelectAll();
            textEditor.Copy();
            Application.OpenURL(Application.persistentDataPath);
        }
    }

    #endregion

    #region Button Functions

    //this one is used to load the evaluation
    void LoadEvaluation()
    {
        if (key != null)
        {
            if (key.isActiveAndEnabled)
            {
                PrefsKeys.SetNextScene("DemoEvaluationLoader");
                SceneManager.LoadScene("Loader_Scene");
            }
            else
            {
                PrefsKeys.SetNextScene("Evaluation_Scene0");
                SceneManager.LoadScene("Loader_Scene");
            }
        }
        else
        {
            PrefsKeys.SetNextScene("Evaluation_Scene0");
            SceneManager.LoadScene("Loader_Scene");
        }

    }

    //this one is to go to the menu center
    public void LoadGameMenus()
    {
        PrefsKeys.SetNextScene("GameMenus");
        SceneManager.LoadScene("Loader_Scene");
    }

    public void TryALogIn(string email, string password)
    {
        var verificationUtility = new EmailVerificationUtility();
        bool isAccountReady = verificationUtility.IsValidMail(email);
        bool isPassReady = password != "";
        if (isAccountReady)
        {
            if (isPassReady)
            {
                ShowLoading();
                logInScript.PostLogin(email, password, false);
            }
            else
            {
                ShowWarning(0, registerMenu.ShowLoginPanel);
            }
        }
        else
        {
            ShowWarning(1, registerMenu.ShowLoginPanel);
        }
    }

    public void TrySignIn(string mail, string pass, string passConfirmation, string kidName, bool isNewPaidUser)
    {
        string kidDob = DefineTheDateOfBirth(0);
        EmailVerificationUtility verificationUtility = new EmailVerificationUtility();
        if (mail != "" && pass != "" && passConfirmation != "" && kidName != "" && kidDob != "")
        {

            if (verificationUtility.IsValidMail(mail))
            {
                if (pass.Length >= 8)
                {
                    if (pass == passConfirmation)
                    {
                        if (KidDateIsOK(0))
                        {
                            ShowLoading();
                            logInScript.RegisterParentAndKid(mail, pass, kidName, kidDob, isNewPaidUser);
                        }
                        else
                        {
                            ShowWarning(12);
                        }
                    }
                    else
                    {
                        ShowWarning(7);
                    }
                }
                else
                {
                    ShowWarning(6);
                }
            }
            else
            {
                ShowWarning(5);
            }

        }
        else
        {
            ShowWarning(4);
        }
    }

    //this one tries to change a code for the active kid
    public void ChangeAPrePaidCode(int isNewChild)
    {
        ShowLoading();
        if (isNewChild == 0)
        {
            if (sessionManager.activeUser != null)
            {
                subscriptionsManager.SendACode(sessionManager.activeUser.id, sessionManager.activeKid.id, shopMenu.prepaidInput.text, isNewChild);
            }
            else
            {
                HideAllCanvas();
                registerMenu.ShowRegisterPanel(false);
            }
        }
        else
        {
            subscriptionsManager.SendACode(sessionManager.activeUser.id, shopMenu.prepaidInput.text, isNewChild);
        }
    }

    //this one will set a kid as an active kid if its selected
    void SetKidProfile(string parentKey, int id)
    {
        sessionManager.SetKid(parentKey, id);
        ShowGameMenu();
    }

    void SetKidIdToSubscriptionPlan(int id, KidProfileCanvas profileCanvas)
    {
        if (ids.Contains(id))
        {
            ids.Remove(id);
            profileCanvas.PutInGrey();
            miniCanvitas.Remove(profileCanvas.buttonOfProfile);
        }
        else
        {
            ids.Add(id);
            profileCanvas.avatarImage.color = Color.white;
            //Color farben;
            ColorUtility.TryParseHtmlString("#AB6021", out Color farben);
            profileCanvas.buttonOfProfile.transform.GetChild(2).GetComponent<Image>().color = farben;
            miniCanvitas.Add(profileCanvas.buttonOfProfile);
            if (miniCanvitas.Count > numKids)
            {
                miniCanvitas[0].transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                miniCanvitas[0].transform.GetChild(2).GetComponent<Image>().color = Color.grey;
                miniCanvitas.Remove(miniCanvitas[0]);
                ids.Remove(ids[0]);
            }
        }
    }

    public void ConfirmKidPurchase()
    {
        subscriptionsManager.SendSubscriptionData(numKids, sessionManager.activeKid.id.ToString(), sessionManager.activeUser.id, subscriptionIAPType);
    }

    public void ConfirmKidsPurchase()
    {
        string idsSt = "";

        for (int i = 0; i < ids.Count; i++)
        {
            if (i > 0)
            {
                idsSt += "," + ids[i].ToString();
                Debug.Log(idsSt);
            }
            else
            {
                idsSt += ids[i].ToString();
                Debug.Log(idsSt);
            }
        }
        Debug.Log(idsSt);

        subscriptionsManager.SendSubscriptionData(numKids, idsSt, sessionManager.activeUser.id, subscriptionIAPType);
    }

    //this one goes and reset the password for a player if its needed
    public void ForgotPassword()
    {
        Application.OpenURL(Keys.Api_Web_Key + "recuperar/");
    }

    //This is if a person regrets to start the registration process
    void GoBack()
    {
        ShowFirstMenu();
    }

    //this will send the app to terms and conditions
    public void GoToTermsAndConditions()
    {
        Application.OpenURL("https://towi.com.mx/terminos-y-condiciones/");
    }

    public void GoPrivacyPolicy()
    {
        Application.OpenURL("https://towi.com.mx/aviso-de-privacidad/");
    }

    //This will not set any active kids
    void CloseKids()
    {
        if (sessionManager.activeKid != null)
        {
            ShowGameMenu();
        }
        else
        {
            CloseSession();
        }
    }

    //This will close the session and let the user open a new one
    void CloseSession()
    {
        PlayerPrefs.SetInt(Keys.Logged_In, 0);
        PlayerPrefs.SetString("sessions", "");
        PlayerPrefs.SetInt(Keys.Logged_Session, 0);
        PlayerPrefs.SetString(Keys.Active_User_Key, "");
        ShowFirstMenu();
        sessionManager.StartAgain();
        alreadyLogged = false;
    }

    void CreateAKid()
    {
        if (newKidNameInput.text != "" && newKidDay.text != "" && newKidMonth.text != "" && newKidYear.text != "")
        {
            if (KidDateIsOK(1))
            {
                string dob = DefineTheDateOfBirth(1);
                string nameKid = newKidNameInput.text;
                int id = sessionManager.activeUser.id;
                ShowLoading();
                logInScript.RegisterAKid(dob, nameKid, id);
            }
            else
            {
                ShowWarning(12);
            }
        }
        else
        {
            ShowWarning(4);
        }
        //Debug.Log("We are creating a kid");
    }

    void SetLanguageOfGame(int index)
    {
        PlayerPrefs.SetInt(Keys.DeviceLenguage, 1);
        PlayerPrefs.SetInt(Keys.Selected_Language, index);
        UpdateTexts(true);
    }

    void SetDeviceLanguage()
    {
        PlayerPrefs.SetInt(Keys.DeviceLenguage, 0);
        UpdateTexts(true);
    }

    #endregion

    //This will set all the texts need for the menus
    void WriteTheTexts()
    {
        WriteTheText(subscribeAnotherCountButton, 3);
        WriteTheText(creditColumOne, 6);
        WriteTheText(creditColumTwo, 7);
        WriteTheText(newKidNameInput, 10);
        WriteTheText(selectionKidBackButton, 15, 0);
        WriteTheText(newKidBirthday, 16);
        WriteTheText(newKidDay, 17);
        WriteTheText(newKidMonth, 18);
        WriteTheText(newKidYear, 19);
        WriteTheText(creditText, 40);
        WriteTheText(configMenu.languageButton, 47, 0);
        WriteTheText(configMenu.englishLanguageButton, 48, 0);
        WriteTheText(configMenu.spanishLanguageButton, 49, 0);
        WriteTheText(configMenu.automaticButton, 50, 0);
        WriteTheText(changeProfileButton, 51);
        WriteTheText(loadingText, 52);
        kidLooker.placeholder.GetComponent<TextMeshProUGUI>().text = lines[64];
        warningButton.GetComponentInChildren<Text>().text = TextReader.commonStrings[0];
        newKidButton.GetComponentInChildren<Text>().text = TextReader.commonStrings[0];
    }

    //This metods will set the text accordingly with the type of objects
    //This is for buttons
    public void WriteTheText(Button but, int index)
    {
        but.GetComponentInChildren<Text>().text = lines[index];
    }

    public void WriteTheText(Button but, int index, int a)
    {
        but.GetComponentInChildren<TextMeshProUGUI>().text = lines[index];
    }

    public void WriteTheText(TextMeshProUGUI text, int index)
    {
        text.text = lines[index];
    }

    //This for Text
    public void WriteTheText(Text text, int index)
    {
        text.text = lines[index];
    }

    //This for Dopdowns
    public void WriteTheText(Dropdown drop, int dropIndex, int index)
    {
        drop.options[dropIndex].text = lines[index];
    }

    //This for InputField placeholders
    public void WriteTheText(InputField field, int index)
    {
        field.placeholder.GetComponent<Text>().text = lines[index];
    }

    public void ShowSyncMessage(int number)
    {
        warningPanel.SetActive(true);
        var text = TextReader.TextsToSet("Login/Config")[number];
        warningText.text = text;
        /*if (numberOfWarning == 8)
        {
            ShowGameMenu();
        }*/

        warningButton.onClick.RemoveAllListeners();
        warningButton.onClick.AddListener(HideWarning);
    }

    public void ShowWarning(int numberOfWarning, UnityEngine.Events.UnityAction action)
    {
        warningPanel.SetActive(true);
        warningText.text = warningLines[numberOfWarning];
        /*if (numberOfWarning == 8)
        {
            ShowGameMenu();
        }*/

        warningButton.onClick.RemoveAllListeners();
        warningButton.onClick.AddListener(() =>
        {
            HideWarning();
            action();
        });
    }

    //This will display a warning and show what was the error if that will occure
    public void ShowWarning(int numberOfWarning)
    {
        warningPanel.SetActive(true);
        warningText.text = warningLines[numberOfWarning];
        /*if (numberOfWarning == 8)
        {
            ShowGameMenu();
        }*/

        warningButton.onClick.RemoveAllListeners();
        warningButton.onClick.AddListener(HideWarning);
    }

    public void GoToWebSubscriptions()
    {
        Application.OpenURL(Keys.Api_Web_Key + "subscripciones/");
    }

    void GiveASuscription()
    {
        ShowLoading();
        subscriptionsManager.ActivateASingleKidAvailable(sessionManager.activeKid.id, sessionManager.activeUser.id);
    }

    //this will hide the warning
    void HideWarning()
    {
        warningPanel.SetActive(false);
    }

    //This will set if a player is logged already
    public void LoggedNow()
    {
        alreadyLogged = true;
        PlayerPrefs.SetInt(Keys.Logged_In, 1);
        PlayerPrefs.SetString(Keys.Active_User_Key, sessionManager.activeUser.userkey);
    }

    //This is what happens if you are buyinhg in windows
    void ShopWindows()
    {
        Application.OpenURL(Keys.Api_Web_Key + "subscripciones/");
        Debug.Log("Open shop");
    }

    //This will set the correct date for a child
    bool DatesFromInput(int existDad)
    {
        var day = registerMenu.dayInput.text;
        var month = registerMenu.monthInput.text;
        var year = registerMenu.yearInput.text;

        if (existDad == 0)
        {
            if (day != "" && month != "" && year != "")
            {
                dobYMD = new int[] { int.Parse(year), int.Parse(month), int.Parse(day) };
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (newKidDay.text != "" && newKidMonth.text != "" && newKidYear.text != "")
            {
                dobYMD = new int[] { int.Parse(newKidYear.text), int.Parse(newKidMonth.text), int.Parse(newKidDay.text) };
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    //This one will set a new kid
    string DefineTheDateOfBirth(int newKid)
    {
        if (DatesFromInput(newKid))
        {
            string date = dobYMD[0].ToString("D4") + "-" + dobYMD[1].ToString("D2") + "-" + dobYMD[2].ToString("D2");
            return date;
        }
        else
        {
            return "";
        }
    }

    //this will ook if a date is correct
    bool KidDateIsOK(int typeOfKid)
    {
        int year = 0;
        int month = 0;
        int day = 0;
        if (typeOfKid == 0)
        {
            year = int.Parse(registerMenu.yearInput.text);
            month = int.Parse(registerMenu.monthInput.text);
            day = int.Parse(registerMenu.dayInput.text);
        }
        if (typeOfKid != 0)
        {
            year = int.Parse(newKidYear.text);
            month = int.Parse(newKidMonth.text);
            day = int.Parse(newKidDay.text);
        }

        List<int> months1 = new List<int> { 1, 3, 5, 7, 8, 10, 12 };
        List<int> months2 = new List<int> { 4, 6, 9, 11 };

        if (year > 999 && day > 0 && month > 0 && month < 13)
        {
            if (months1.Contains(month) && day < 32 || months2.Contains(month) && day < 31
                || year % 4 == 0 && month == 2 && day < 30 || year % 4 != 0 && month == 2 && day < 29)
            {
                dobYMD = new int[] { year, month, day };
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public bool IsDemoKeyAvailable()
    {
        return key != null;
    }

    public void UpdateIAPSubscription(string kidsIDs, int kids)
    {
        if (myIAPManager.IsStillSuscribed())
        {
            DateTime nowT = myIAPManager.ExpireDate();
            string date = String.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}", nowT.Year, nowT.Month, nowT.Day, nowT.Hour, nowT.Minute, nowT.Second);
            if (kids == myIAPManager.GetKidData())
            {
                subscriptionsManager.ActivateKidIAP(kidsIDs, date, sessionManager.activeUser.id);
            }
            else
            {
                subscriptionsManager.ActivateKidIAP(kidsIDs, date, sessionManager.activeUser.id);
            }
        }
    }
    #endregion

    //this class will check if a email is well set
    class EmailVerificationUtility
    {
        bool isInvalid = false;

        public bool IsValidMail(string email)
        {
            try
            {
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None);
            }
            catch
            {
                return false;
            }

            if (isInvalid)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        string DomainMapper(Match match)
        {
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                isInvalid = false;
            }
            return match.Groups[1].Value + domainName;
        }
    }

    bool IsTablet()
    {
        float screenWidth = Screen.width / Screen.dpi;
        float screenHeight = Screen.height / Screen.dpi;
        float diagonalInches = Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));

        return diagonalInches > 6.5f;
    }


    bool IsEvaluationAvilable()
    {
        bool returner = false;
        if (IsTablet() && sessionManager.activeKid.testAvailable)
        {
            returner = true;
        }

        return returner;
    }

    void EscapeApplication()
    {
        Application.Quit();
        Debug.Log("shoul exit now");
    }
}

class ConfigMenu
{
    public GameObject panel;
    public Button languageButton;
    public GameObject languageButtonHandler;
    public Button englishLanguageButton;
    public Button spanishLanguageButton;
    public Button automaticButton;
    public Button backButton;
    public Button logoButton;
    public Button updateDataButton;
    public TextMeshProUGUI versionText;

    public ConfigMenu(GameObject mainPanel)
    {
        panel = mainPanel;
        languageButton = panel.transform.GetChild(0).GetComponent<Button>();
        languageButtonHandler = panel.transform.GetChild(1).gameObject;
        englishLanguageButton = languageButtonHandler.transform.GetChild(0).GetComponent<Button>();
        spanishLanguageButton = languageButtonHandler.transform.GetChild(1).GetComponent<Button>();
        automaticButton = languageButtonHandler.transform.GetChild(languageButtonHandler.transform.childCount - 1).GetComponent<Button>();
        backButton = panel.transform.Find("Back Button").GetComponent<Button>();
        logoButton = panel.transform.Find("Logo Button").GetComponent<Button>();
        updateDataButton = panel.transform.Find("Upadte Button").GetComponent<Button>();
        versionText = panel.transform.Find("Version Number Text").GetComponent<TextMeshProUGUI>();
    }
}

class KidProfileCanvas
{
    public GameObject gameObject;
    public Button buttonOfProfile;
    public Image avatarImage;
    public Image frameImage;
    public Image billboardImage;
    public Text nameText;

    public KidProfileCanvas(GameObject canvas)
    {
        gameObject = canvas;
        buttonOfProfile = gameObject.GetComponent<Button>();
        avatarImage = gameObject.transform.GetChild(0).GetComponent<Image>();
        frameImage = gameObject.transform.GetChild(1).GetComponent<Image>();
        billboardImage = gameObject.transform.GetChild(2).GetComponent<Image>();
        nameText = billboardImage.GetComponentInChildren<Text>();
    }

    public void PutInGrey()
    {
        avatarImage.color = Color.grey;
        frameImage.color = Color.grey;
        billboardImage.color = Color.grey;
    }

    public void ChangeAvatar(string avatarData)
    {
        if (avatarData != "")
        {
            avatarImage.sprite = Resources.Load<Sprite>($"Icons/{avatarData}");
        }
        else
        {
            avatarImage.sprite = Resources.Load<Sprite>($"Icons/koala");
        }
    }

    public void SetKidName(string kidName)
    {
        nameText.text = kidName;
    }
}

class RegisterMenu
{
    MenuManager manager;

    public GameObject gameObject;

    public GameObject division;

    public GameObject add0;
    public GameObject add1;

    public GameObject logInMenu;
    public InputPanels inputEmail;
    public InputPanels inputPass;
    public Button forgotPassButton;
    public Button logInButton;
    public Text notRegisterText;
    public Button goToSingInButton;

    public GameObject signInMenu;
    public InputPanels inputEmailDad;
    public InputPanels inputPassDad;
    public InputPanels inputPassAgain;
    public Button termsAndConditionsButton;
    public InputPanels inputName;
    public Text birthdateText;
    public InputField dayInput;
    public InputField monthInput;
    public InputField yearInput;
    public Text addKidLaterText;
    public Button signInButton;

    public GameObject tryPanel;
    public Button[] buttonsOfAges;

    public Button returnButton;

    public Text woodText;

    public RegisterMenu(GameObject mainMenu, MenuManager menu)
    {
        manager = menu;
        gameObject = mainMenu;
        var mainPanel = gameObject.transform.Find("Main Panel");

        division = mainPanel.Find("Division").gameObject;

        add0 = mainPanel.Find("Add 0").gameObject;
        add1 = mainPanel.Find("Add 1").gameObject;

        var logInPanel = mainPanel.Find("Log In Panel");
        logInMenu = logInPanel.gameObject;
        inputEmail = new InputPanels(logInPanel.Find("Input E-mail"));
        inputPass = new InputPanels(logInPanel.Find("Input Password"));
        forgotPassButton = logInPanel.Find("Forgot Pass Button").GetComponent<Button>();
        logInButton = logInPanel.Find("Button Log In").GetComponent<Button>();
        notRegisterText = logInPanel.Find("Not Sign in Text").GetComponent<Text>();
        goToSingInButton = logInPanel.Find("Button Sign In").GetComponent<Button>();

        var signInPanel = mainPanel.Find("Sing In Panel");
        signInMenu = signInPanel.gameObject;
        inputEmailDad = new InputPanels(signInPanel.Find("Input E-mail"));
        inputPassDad = new InputPanels(signInPanel.Find("Input Password"));
        inputPassAgain = new InputPanels(signInPanel.Find("Input Password Confirm"));
        termsAndConditionsButton = signInPanel.Find("Terms And Conditions Button").GetComponent<Button>();
        inputName = new InputPanels(signInPanel.Find("Input Name"));
        var birthPanel = signInPanel.Find("Input BirthDate");
        birthdateText = birthPanel.Find("Birth Date Text").GetComponent<Text>();
        dayInput = birthPanel.Find("Day Input").GetComponent<InputField>();
        monthInput = birthPanel.Find("Month Input").GetComponent<InputField>();
        yearInput = birthPanel.Find("Year Input").GetComponent<InputField>();
        addKidLaterText = signInPanel.Find("Can Add Text").GetComponent<Text>();
        signInButton = signInPanel.Find("Button Sign In").GetComponent<Button>();

        tryPanel = mainPanel.Find("Try Panel").gameObject;
        buttonsOfAges = new Button[tryPanel.transform.childCount];
        for (int i = 0; i < tryPanel.transform.childCount; i++)
        {
            buttonsOfAges[i] = tryPanel.transform.GetChild(i).GetComponent<Button>();
        }

        returnButton = mainPanel.transform.Find("Return Button").GetComponent<Button>();

        var logoPanel = gameObject.transform.Find("Logo Panel");
        woodText = logoPanel.transform.GetComponentInChildren<Text>();

        HideAll();
    }

    public void ClearInputData()
    {
        inputEmail.field.text = "";
        inputEmailDad.field.text = "";
        inputPass.field.text = "";
        inputPassAgain.field.text = "";
        inputPassDad.field.text = "";
        inputName.field.text = "";
        dayInput.text = "";
        monthInput.text = "";
        yearInput.text = "";
    }

    void ShouldShowAdds(bool showAdds)
    {
        add0.gameObject.SetActive(showAdds);
        add1.gameObject.SetActive(showAdds);
        division.SetActive(showAdds);
    }

    public void HideAll()
    {
        gameObject.SetActive(false);
    }

    void HideAllPanels()
    {
        logInMenu.SetActive(false);
        signInMenu.SetActive(false);
        tryPanel.SetActive(false);
    }

    public void ShowLoginPanel()
    {
        gameObject.SetActive(true);
        const string pathOfLoginMenuTetxs = "Login/LoginMenu";
        HideAllPanels();
        ShouldShowAdds(true);
        logInMenu.SetActive(true);
        var texts = TextReader.TextsToSet(pathOfLoginMenuTetxs);

        woodText.text = texts[0];
        add0.GetComponentInChildren<TextMeshProUGUI>().text = texts[1];
        add1.GetComponentInChildren<TextMeshProUGUI>().text = texts[2];

        inputEmail.inputNameText.text = texts[3];
        inputEmail.field.placeholder.GetComponent<TextMeshProUGUI>().text = texts[3];
        inputPass.inputNameText.text = texts[4];
        inputPass.field.placeholder.GetComponent<TextMeshProUGUI>().text = texts[4];
        inputPass.field.inputType = TMP_InputField.InputType.Password;
        forgotPassButton.GetComponent<TextMeshProUGUI>().text = texts[5];
        logInButton.GetComponentInChildren<TextMeshProUGUI>().text = texts[6];
        notRegisterText.text = texts[7];
        goToSingInButton.GetComponentInChildren<TextMeshProUGUI>().text = texts[8];

        forgotPassButton.onClick.RemoveAllListeners();
        forgotPassButton.onClick.AddListener(manager.ForgotPassword);

        logInButton.onClick.RemoveAllListeners();
        logInButton.onClick.AddListener(() => manager.TryALogIn(inputEmail.field.text, inputPass.field.text));

        goToSingInButton.onClick.RemoveAllListeners();
        goToSingInButton.onClick.AddListener(ShowSignInPanel);

        returnButton.onClick.RemoveAllListeners();
        returnButton.onClick.AddListener(() =>
        {
            ClearData();
            manager.ShowFirstMenu();
        });

    }

    public void ShowSignInPanel()
    {
        gameObject.SetActive(true);
        const string pathOfLoginMenuTetxs = "Login/SingInMenu";
        HideAllPanels();
        ShouldShowAdds(true);
        signInMenu.SetActive(true);
        var texts = TextReader.TextsToSet(pathOfLoginMenuTetxs);

        woodText.text = texts[0];
        add0.GetComponentInChildren<TextMeshProUGUI>().text = texts[1];
        add1.GetComponentInChildren<TextMeshProUGUI>().text = texts[2];

        inputEmailDad.inputNameText.text = texts[3];
        inputEmailDad.field.placeholder.GetComponent<TextMeshProUGUI>().text = texts[3];
        inputPassDad.inputNameText.text = texts[4];
        inputPassDad.field.placeholder.GetComponent<TextMeshProUGUI>().text = texts[4];
        inputPassDad.field.inputType = TMP_InputField.InputType.Password;
        inputPassAgain.inputNameText.text = texts[5];
        inputPassAgain.field.placeholder.GetComponent<TextMeshProUGUI>().text = texts[5];
        inputPassAgain.field.inputType = TMP_InputField.InputType.Password;
        termsAndConditionsButton.GetComponent<TextMeshProUGUI>().text = texts[6];
        termsAndConditionsButton.onClick.RemoveAllListeners();
        termsAndConditionsButton.onClick.AddListener(manager.GoToTermsAndConditions);

        inputName.inputNameText.text = texts[7];
        inputName.field.placeholder.GetComponent<TextMeshProUGUI>().text = texts[7];
        birthdateText.text = texts[8];
        dayInput.placeholder.GetComponent<TextMeshProUGUI>().text = texts[9];
        monthInput.placeholder.GetComponent<TextMeshProUGUI>().text = texts[10];
        yearInput.placeholder.GetComponent<TextMeshProUGUI>().text = texts[11];
        addKidLaterText.text = texts[12];
        signInButton.GetComponentInChildren<TextMeshProUGUI>().text = texts[0];

        signInButton.onClick.RemoveAllListeners();
        signInButton.onClick.AddListener(TryASignIn);

        returnButton.onClick.RemoveAllListeners();
        returnButton.onClick.AddListener(() =>
        {
            ClearData();
            ShowLoginPanel();
        });
    }

    public void ShowRegisterPanel(bool isIAP)
    {
        HideAllPanels();
        manager.HideAllCanvas();
        gameObject.SetActive(true);
        const string pathOfLoginMenuTetxs = "Login/SingInMenu";
        ShouldShowAdds(true);
        signInMenu.SetActive(true);
        var texts = TextReader.TextsToSet(pathOfLoginMenuTetxs);

        woodText.text = texts[13];
        add0.GetComponentInChildren<TextMeshProUGUI>().text = texts[1];
        add1.GetComponentInChildren<TextMeshProUGUI>().text = texts[2];

        inputEmailDad.inputNameText.text = texts[3];
        inputEmailDad.field.placeholder.GetComponent<TextMeshProUGUI>().text = texts[3];
        inputPassDad.inputNameText.text = texts[4];
        inputPassDad.field.placeholder.GetComponent<TextMeshProUGUI>().text = texts[4];
        inputPassDad.field.inputType = TMP_InputField.InputType.Password;
        inputPassAgain.inputNameText.text = texts[5];
        inputPassAgain.field.placeholder.GetComponent<TextMeshProUGUI>().text = texts[5];
        inputPassAgain.field.inputType = TMP_InputField.InputType.Password;
        termsAndConditionsButton.GetComponent<TextMeshProUGUI>().text = texts[6];
        termsAndConditionsButton.onClick.RemoveAllListeners();
        termsAndConditionsButton.onClick.AddListener(manager.GoToTermsAndConditions);

        inputName.inputNameText.text = texts[7];
        inputName.field.placeholder.GetComponent<TextMeshProUGUI>().text = texts[7];
        birthdateText.text = texts[8];
        dayInput.placeholder.GetComponent<TextMeshProUGUI>().text = texts[9];
        monthInput.placeholder.GetComponent<TextMeshProUGUI>().text = texts[10];
        yearInput.placeholder.GetComponent<TextMeshProUGUI>().text = texts[11];
        addKidLaterText.text = texts[12];
        signInButton.GetComponentInChildren<TextMeshProUGUI>().text = texts[13];

        signInButton.onClick.RemoveAllListeners();
        signInButton.onClick.AddListener(()=>TrySingInAndPurchase(isIAP));

        returnButton.onClick.RemoveAllListeners();
        returnButton.onClick.AddListener(() =>
        {
            ClearData();
            ShowLoginPanel();
        });
        if (isIAP)
        {
            returnButton.gameObject.SetActive(false);
        }
    }

    public void ShowTryPanel()
    {
        gameObject.SetActive(true);
        const string pathOfLoginMenuTetxs = "Login/TryAges";
        HideAllPanels();
        ShouldShowAdds(false);
        tryPanel.SetActive(true);
        var texts = TextReader.TextsToSet(pathOfLoginMenuTetxs);

        for (int i = 0; i < buttonsOfAges.Length; i++)
        {
            buttonsOfAges[i].GetComponentInChildren<Text>().text = texts[i];
            buttonsOfAges[i].onClick.RemoveAllListeners();

            int x = i;

            buttonsOfAges[i].onClick.AddListener(() =>
            {
                CanSendToTry(x);
            });
        }

        woodText.text = texts[3];

        returnButton.onClick.RemoveAllListeners();
        returnButton.onClick.AddListener(() =>
        {
            ClearData();
            manager.ShowFirstMenu();
        });
    }

    public void ShowTryPanelRegistered()
    {
        ShowTryPanel();
        returnButton.onClick.RemoveAllListeners();
        returnButton.onClick.AddListener(() =>
        {
            manager.ShowGameMenu();
        });
    }

    void CanSendToTry(int difficulty)
    {
        if (PlayerPrefs.GetInt(Keys.First_Try) == 0)
        {
            PlayerPrefs.SetInt(Keys.Level_Of_Try, difficulty);
            PrefsKeys.SetNextScene("Monkey_Hiding_Scene");
            SceneManager.LoadScene("Loader_Scene");
        }
        else
        {
            manager.ShowAdd();
        }
    }

    void ClearData()
    {
        dayInput.text = "";
        monthInput.text = "";
        yearInput.text = "";
        inputEmail.field.text = "";
        inputEmailDad.field.text = "";
        inputPass.field.text = "";
        inputPassDad.field.text = "";
        inputPassAgain.field.text = "";
        inputName.field.text = "";
    }

    void TryASignIn()
    {
        var mail = inputEmailDad.field.text;
        var pass = inputPassDad.field.text;
        var pass2 = inputPassAgain.field.text;
        var kidName = inputName.field.text;

        manager.TrySignIn(mail, pass, pass2, kidName, false);
    }

    void TrySingInAndPurchase(bool isIAP)
    {
        var mail = inputEmailDad.field.text;
        var pass = inputPassDad.field.text;
        var pass2 = inputPassAgain.field.text;
        var kidName = inputName.field.text;
        PlayerPrefs.SetInt(Keys.Buy_IAP, Convert.ToInt32(isIAP));
        manager.TrySignIn(mail, pass, pass2, kidName, true);
    }
}

class InputPanels
{
    public GameObject gameObject;
    public TextMeshProUGUI inputNameText;
    public TMP_InputField field;

    public InputPanels(Transform panel)
    {
        gameObject = panel.gameObject;
        inputNameText = panel.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        field = panel.Find("InputField (TMP)").GetComponent<TMP_InputField>();
    }
}

class GameMenu
{
    GameObject mainCanvas;
    MenuManager manager;
    public GameObject tryLogo;
    public GameObject logoIcon;
    public Button gamesButton;
    public Button evaluationButton;
    public Button buyButton;
    public KidProfileCanvas kidProfile;
    Button singOutButton;
    Button settingsButton;
    Button aboutButton;

    const string pathOfFirstMenu = "Login/FirstMenu";

    public GameMenu(GameObject panel, MenuManager managerToRelay)
    {
        manager = managerToRelay;
        mainCanvas = panel;
        tryLogo = mainCanvas.transform.Find("Try Logo").gameObject;
        logoIcon = mainCanvas.transform.Find("Game Logo").gameObject;
        gamesButton = mainCanvas.transform.Find("Games Button").GetComponent<Button>();
        evaluationButton = mainCanvas.transform.Find("Evaluation Button").GetComponent<Button>();
        buyButton = mainCanvas.transform.Find("Buy Button").GetComponent<Button>();
        kidProfile = new KidProfileCanvas(mainCanvas.transform.Find("Change Kid Profile Button").gameObject);
        singOutButton = mainCanvas.transform.Find("Sing Out Button").GetComponent<Button>();
        settingsButton = mainCanvas.transform.Find("Settings Button").GetComponent<Button>();
        aboutButton = mainCanvas.transform.Find("About Button").GetComponent<Button>();
        SetStaticButtonFuctions();
    }

    public void ShowFirstMenu()
    {
        var textsToSet = TextReader.TextsToSet(pathOfFirstMenu);

        mainCanvas.SetActive(true);

        gamesButton.GetComponentInChildren<TextMeshProUGUI>().text = textsToSet[0];
        evaluationButton.GetComponentInChildren<TextMeshProUGUI>().text = textsToSet[1];
        buyButton.GetComponentInChildren<TextMeshProUGUI>().text = textsToSet[2];
        tryLogo.GetComponentInChildren<TextMeshProUGUI>().text = textsToSet[3];

        tryLogo.SetActive(true);
        logoIcon.SetActive(false);

        gamesButton.gameObject.SetActive(true);
        evaluationButton.gameObject.SetActive(true);
        buyButton.gameObject.SetActive(true);

        singOutButton.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        aboutButton.gameObject.SetActive(false);
        kidProfile.gameObject.SetActive(false);

        SetImageColor(buyButton.GetComponent<Image>(), TowiDictionary.ColorHexs["activeOrange"]);
        SetImageColor(gamesButton.GetComponent<Image>(), TowiDictionary.ColorHexs["activeYellow"]);
        SetImageColor(evaluationButton.GetComponent<Image>(), TowiDictionary.ColorHexs["activeGreen"]);

        gamesButton.onClick.RemoveAllListeners();
        evaluationButton.onClick.RemoveAllListeners();
        buyButton.onClick.RemoveAllListeners();

        gamesButton.onClick.AddListener(manager.ShowLogIn);
        if (PlayerPrefs.GetInt(Keys.First_Try) != 0)
        {
            buyButton.onClick.AddListener(manager.ShowAdd);
        }
        else
        {
            buyButton.onClick.AddListener(manager.ShowTryChances);
        }

        evaluationButton.onClick.AddListener(manager.ShowAdd);
    }

    public void ShowThisMenu()
    {
        mainCanvas.SetActive(true);

        gamesButton.onClick.RemoveAllListeners();
        evaluationButton.onClick.RemoveAllListeners();
        buyButton.onClick.RemoveAllListeners();

        gamesButton.onClick.AddListener(manager.ShowRegisteredAdd);
        evaluationButton.onClick.AddListener(manager.ShowRegisteredAdd);
        buyButton.onClick.AddListener(manager.ShowRegisteredAdd);

        SetImageColor(gamesButton.GetComponent<Image>(), TowiDictionary.ColorHexs["deactivated"]);
        SetImageColor(buyButton.GetComponent<Image>(), TowiDictionary.ColorHexs["deactivated"]);
        SetImageColor(evaluationButton.GetComponent<Image>(), TowiDictionary.ColorHexs["deactivated"]);

        singOutButton.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(true);
        aboutButton.gameObject.SetActive(true);
        kidProfile.gameObject.SetActive(true);

        tryLogo.SetActive(false);
        logoIcon.SetActive(true);
    }

    public void ShowThisMenu(bool isActiveTheCurrentKid, bool isInTrial, bool isEvaluationAvailable, bool isDemo, int licencesToActivate)
    {
        mainCanvas.SetActive(true);
        SetDynamicButtonFunctions(isActiveTheCurrentKid, isInTrial, isEvaluationAvailable, isDemo, licencesToActivate);

        singOutButton.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(true);
        aboutButton.gameObject.SetActive(true);
        kidProfile.gameObject.SetActive(true);

        tryLogo.SetActive(false);
        logoIcon.SetActive(true);
    }

    public void HideThisMenu()
    {
        mainCanvas.SetActive(false);
    }

    void SetStaticButtonFuctions()
    {
        aboutButton.onClick.AddListener(manager.ShowCredits);
        settingsButton.onClick.AddListener(manager.ShowSettings);
        kidProfile.buttonOfProfile.onClick.AddListener(manager.SetKidsProfiles);
        singOutButton.onClick.AddListener(manager.ShowSingOutWarning);
    }

    public void SetDynamicButtonFunctions(bool isActiveTheCurrentKid, bool isInTrail, bool evaluationAvailable, bool isDemo, int licencesToActivate)
    {
        gamesButton.onClick.RemoveAllListeners();
        evaluationButton.onClick.RemoveAllListeners();
        buyButton.onClick.RemoveAllListeners();
        manager.WriteTheText(evaluationButton, 0);

        if (evaluationAvailable)
        {
            evaluationButton.gameObject.SetActive(true);
        }
        else
        {
            evaluationButton.gameObject.SetActive(false);
        }

        if (isActiveTheCurrentKid)
        {
            manager.WriteTheText(gamesButton, 1);
            manager.WriteTheText(buyButton, 57);

            gamesButton.onClick.AddListener(manager.LoadGameMenus);
            evaluationButton.onClick.AddListener(manager.ShowDisclaimer);
            buyButton.onClick.AddListener(manager.ShowYouHaveASuscription);

            SetImageColor(buyButton.GetComponent<Image>(), TowiDictionary.ColorHexs["activeOrange"]);
            SetImageColor(gamesButton.GetComponent<Image>(), TowiDictionary.ColorHexs["activeYellow"]);
            SetImageColor(evaluationButton.GetComponent<Image>(), TowiDictionary.ColorHexs["activeGreen"]);

            PlayerPrefs.SetInt(Keys.First_Try, 1);
        }
        else if (isInTrail)
        {
            manager.WriteTheText(buyButton, 62);


            manager.WriteTheText(gamesButton, 59);
            gamesButton.onClick.AddListener(manager.ShowTryChancesRegistered);
            SetImageColor(buyButton.GetComponent<Image>(), TowiDictionary.ColorHexs["activeOrange"]);
            SetImageColor(gamesButton.GetComponent<Image>(), TowiDictionary.ColorHexs["activeYellow"]);
            SetImageColor(evaluationButton.GetComponent<Image>(), TowiDictionary.ColorHexs["activeGreen"]);
            if (licencesToActivate > 0)
            {
                manager.ShowAccountWarning(1);
            }
            else
            {
                buyButton.onClick.AddListener(manager.ShowRegisteredAdd);
            }
            evaluationButton.onClick.AddListener(manager.ShowRegisteredAdd);
        }
        else
        {
            manager.WriteTheText(gamesButton, 1);
            manager.WriteTheText(buyButton, 62);
            gamesButton.onClick.AddListener(manager.ShowRegisteredAdd);
            evaluationButton.onClick.AddListener(manager.ShowRegisteredAdd);
            if (licencesToActivate > 0)
            {
                manager.ShowAccountWarning(1);
            }
            else
            {
                buyButton.onClick.AddListener(manager.ShowRegisteredAdd);
            }
            SetImageColor(gamesButton.GetComponent<Image>(), TowiDictionary.ColorHexs["deactivated"]);
            SetImageColor(buyButton.GetComponent<Image>(), TowiDictionary.ColorHexs["deactivated"]);
            SetImageColor(evaluationButton.GetComponent<Image>(), TowiDictionary.ColorHexs["deactivated"]);
        }

        if (isDemo)
        {
            evaluationButton.gameObject.SetActive(true);
            evaluationButton.onClick.RemoveAllListeners();
            evaluationButton.onClick.AddListener(manager.ShowDisclaimer);
        }
    }

    void SetImageColor(Image imageToChange, string colorToSet)
    {
        Color colorToPut;
        ColorUtility.TryParseHtmlString(colorToSet, out colorToPut);
        imageToChange.color = colorToPut;
    }
}

class AddMenu
{
    MenuManager manager;
    GameObject gameObject;
    Text woodText;
    Text[] promoTexts = new Text[2];
    Button buyButton;
    Button backButton;

    public AddMenu(GameObject canvas, MenuManager mainManager)
    {
        gameObject = canvas;
        manager = mainManager;
        var main = gameObject.transform.Find("Add Panel");
        for (int i = 0; i < promoTexts.Length; i++)
        {
            promoTexts[i] = main.Find($"Promotional Text {i}").GetComponent<Text>();
        }

        woodText = main.Find("Logo Panel").GetComponentInChildren<Text>();
        buyButton = main.Find("Buy Button").GetComponent<Button>();
        backButton = main.Find("Return Button").GetComponent<Button>();

        HidePanel();
    }

    public void ShowAdd()
    {
        gameObject.SetActive(true);
        Analytics.CustomEvent("add");
        const string pathOfLoginMenuTetxs = "Login/Add";
        var texts = TextReader.TextsToSet(pathOfLoginMenuTetxs);

        for (int i = 0; i < promoTexts.Length; i++)
        {
            promoTexts[i].GetComponentInChildren<Text>().text = texts[i];
        }

        woodText.text = texts[2];
        buyButton.GetComponentInChildren<Text>().text = texts[2];
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() =>
        {
            manager.SetShop(0);
        });

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(manager.ShowFirstMenu);
    }

    public void ShowAddRegister()
    {
        gameObject.SetActive(true);
        Analytics.CustomEvent("add");
        const string pathOfLoginMenuTetxs = "Login/Add";
        var texts = TextReader.TextsToSet(pathOfLoginMenuTetxs);

        for (int i = 0; i < promoTexts.Length; i++)
        {
            promoTexts[i].GetComponentInChildren<Text>().text = texts[i + 3];
        }

        woodText.text = texts[2];
        buyButton.GetComponentInChildren<Text>().text = texts[2];
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() =>
        {
            manager.SetShop(0);
        });

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(manager.ShowGameMenu);
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
}

class ShopMenu
{
    GameObject mainCanvas;
    MenuManager manager;
    GameObject mainPanel;
    Button backButton;

    Text shopText;
    Button shopButton;

    Button shopWebButton;
    public Button oneMonthButton;
    public Button threeMonthButton;
    Button gotCardButton;
    Text legalText;

    public InputField prepaidInput;

    public Dropdown kidsNumberDropdown;
    Button moreKidsButton;

    Button termsAndConditionsButton;
    Button privacyPolicyButton;

    public ShopMenu(GameObject canvas, MenuManager menuManager)
    {
        mainCanvas = canvas;
        manager = menuManager;
        mainPanel = mainCanvas.transform.GetChild(0).gameObject;
        backButton = mainCanvas.transform.GetChild(1).GetComponent<Button>();

        shopText = mainPanel.transform.GetChild(1).GetComponent<Text>();
        shopButton = mainPanel.transform.GetChild(2).GetComponent<Button>();

        shopWebButton = mainPanel.transform.GetChild(3).GetComponent<Button>();
        oneMonthButton = mainPanel.transform.GetChild(4).GetComponent<Button>();
        threeMonthButton = mainPanel.transform.GetChild(5).GetComponent<Button>();
        gotCardButton = mainPanel.transform.GetChild(6).GetComponent<Button>();
        legalText = mainPanel.transform.GetChild(7).GetComponent<Text>();

        prepaidInput = mainPanel.transform.GetChild(8).GetComponent<InputField>();

        kidsNumberDropdown = mainPanel.transform.GetChild(9).GetComponent<Dropdown>();
        moreKidsButton = mainPanel.transform.GetChild(10).GetComponent<Button>();

        termsAndConditionsButton = mainPanel.transform.GetChild(11).GetComponent<Button>();
        privacyPolicyButton = mainPanel.transform.GetChild(12).GetComponent<Button>();

        SetStaticButtonFunctions();
        SetStaticTexts();
    }

    public void ShowThisMenu()
    {
        mainCanvas.SetActive(true);
    }

    public void HideThisMenu()
    {
        mainCanvas.SetActive(false);
    }

    void SetStaticButtonFunctions()
    {
        shopWebButton.onClick.AddListener(manager.GoToWebSubscriptions);
        moreKidsButton.onClick.AddListener(manager.GoToWebSubscriptions);
        termsAndConditionsButton.onClick.AddListener(manager.GoToTermsAndConditions);
        privacyPolicyButton.onClick.AddListener(manager.GoPrivacyPolicy);
    }

    public void SetStaticTexts()
    {
        manager.WriteTheText(shopWebButton, 29);
        manager.WriteTheText(gotCardButton, 36);
        manager.WriteTheText(moreKidsButton, 39);
        manager.WriteTheText(legalText, 44);
        manager.WriteTheText(termsAndConditionsButton, 22);
        manager.WriteTheText(privacyPolicyButton, 61);
    }

    void HideAllComponents()
    {
        for (int i = 0; i < mainPanel.transform.childCount; i++)
        {
            mainPanel.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void SetWebShop(int isAShopForNewKid)
    {
        HideAllComponents();
        shopWebButton.gameObject.SetActive(true);
        gotCardButton.gameObject.SetActive(true);

        SetFunctionalIAPButtons(isAShopForNewKid);
    }

    public void SetIOSShop(int isAShopForNewKid, bool storeIsSet)
    {
        HideAllComponents();
        if (storeIsSet)
        {
            oneMonthButton.gameObject.SetActive(true);
            threeMonthButton.gameObject.SetActive(true);
            legalText.gameObject.SetActive(true);
            termsAndConditionsButton.gameObject.SetActive(true);
            privacyPolicyButton.gameObject.SetActive(true);
        }
        else
        {
            manager.ShowWarning(0);
        }

        SetFunctionalIAPButtons(isAShopForNewKid);
    }

    public void SetAndroidShop(int isAShopForNewKid)
    {
        HideAllComponents();
        oneMonthButton.gameObject.SetActive(true);
        threeMonthButton.gameObject.SetActive(true);
        gotCardButton.gameObject.SetActive(true);

        SetFunctionalIAPButtons(isAShopForNewKid);
    }

    public void SetNumberShopKid(int isAShopForNewKid)
    {
        HideAllComponents();
        shopButton.gameObject.SetActive(true);
        shopText.gameObject.SetActive(true);
        kidsNumberDropdown.gameObject.SetActive(true);
        moreKidsButton.gameObject.SetActive(true);

        manager.WriteTheText(shopText, 38);
        manager.WriteTheText(shopButton, 29);

        shopButton.onClick.RemoveAllListeners();
        shopButton.onClick.AddListener(manager.ShopIAP);

        SetBackButtonSecondStep(isAShopForNewKid);
    }

    public void SetNumberShopKid()
    {
        HideAllComponents();
        shopButton.gameObject.SetActive(true);
        shopText.gameObject.SetActive(true);
        kidsNumberDropdown.gameObject.SetActive(true);
        moreKidsButton.gameObject.SetActive(true);
    }

    public void SetChangePrepaidCode(int isAShopForNewKid)
    {
        HideAllComponents();
        shopButton.gameObject.SetActive(true);
        shopText.gameObject.SetActive(true);
        prepaidInput.gameObject.SetActive(true);

        manager.WriteTheText(shopText, 37);
        manager.WriteTheText(shopButton, 60);

        shopButton.onClick.RemoveAllListeners();
        shopButton.onClick.AddListener(() => manager.ChangeAPrePaidCode(isAShopForNewKid));

        SetBackButtonSecondStep(isAShopForNewKid);
    }

    void SetFunctionalIAPButtons(int isAShopForNewKid)
    {
        oneMonthButton.onClick.RemoveAllListeners();
        threeMonthButton.onClick.RemoveAllListeners();
        gotCardButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();

        backButton.onClick.AddListener(manager.ShowGameMenu);
        gotCardButton.onClick.AddListener(() => manager.ShopWithCode(isAShopForNewKid));
        oneMonthButton.onClick.AddListener(() => manager.ShopNumOfKids(isAShopForNewKid, 1));
        threeMonthButton.onClick.AddListener(() => manager.ShopNumOfKids(isAShopForNewKid, 3));
    }

    void SetBackButtonSecondStep(int isAShopForNewKid)
    {
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() => manager.ShowShop(isAShopForNewKid));
    }

}
