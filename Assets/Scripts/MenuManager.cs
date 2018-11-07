using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    //this assets contains all the information that we need to create the text of the first menu
    [Header("Texts To Show")]
    public TextAsset textOfScene;
    public TextAsset textOfAll;
    public TextAsset textBefore;
    public TextAsset textAddable;
    public TextAsset creditsAsset;
    public TextAsset warningAsset;
    string[] lines;

    //this region contains all the ui elements of this menu
    #region UI Elements
    [Header("Game UI")]
    public GameObject gameCanvas;
    public Button evaluationButton;
    public Button gamesButton;
    public Button aboutButton;
    public Button kidsButton;
    public Button singOutButton;
	public Text kidNameText;
    public Text savigDirectionText;

    [Header("Log in UI")]
    public GameObject logInMenu;
    public GameObject logAndSingPanel;
    public GameObject accountCanvas;
    public GameObject logInCanavas;
    public GameObject singInCanvas;
    public GameObject kidsPanel;
    public GameObject warningPanel;

    [Header("Account Manager UI")]
    public Button gotAccountButton;
    public Button createAccountButton;


    [Header("Log in canvas UI")]
    public InputField emailLogInInput;
    public InputField passLogInInput;
    public Button forgotPassButton;
    public Button logInButton;
    public Button returnLogInButton;

    [Header("Sing In UI")]
    public InputField dadMailInput;
    public InputField dadPassInput;
    public InputField dadPassRepeatInput;
    public InputField kidNameInput;
    public Text kidDateText;
    public InputField kidDayInput;
    public InputField kidMonthInput;
    public InputField kidYearInput;
    public Text kidMoreText;
    public Text acceptTermsAndConditionText;
    public Button termsAndConditionsButton;
    public Button singInButton;
    public Button singInBackButton;

    [Header("Kid Selector")]
    public GameObject miniKidCanvas;
    public GameObject miniKidContainer;
    public Text selectionKidText;
    public Button selectionKidBackButton;
    public Button addKidButton;

    [Header("Credits")]
    public GameObject creditCanvas;
    public Button exitCredits;
    public Text creditText;
    public Text creditColumOne;
    public Text creditColumTwo;

    [Header("Subscribe")]
    public GameObject subscribeCanvas;
    public Text subscribeText;
    public Button subscribeButton;
    public Button subscribeAnotherCountButton;
    public Button changeProfileButton;
    public Button continueEvaluationButton;
    public Button escapeEvaluationButton;
    public Image suscripctionLogo;
    public Image warningLogo;

    [Header("Shop Button")]
    public GameObject shopCanvas;
    public Button oneMonthButton;
    public Button threeMonthButton;
    public Button prepaidButton;
    public Button sendCardButton;
    public InputField inputPrepaidCode;
    public Text codeText;
    public Button shopBackButton;
    public Button shopIAPButton;
    public Button moreAccountsNeedsButton;
    public Dropdown quantityKidsDrop;
    public Button shopInWeb;

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

    [Header("Loading")]
    public GameObject loadingCanvas;
    public Text loadingText;

    #endregion

    AudioManager audioManager;
    EvaluationController evaluationController;

    DemoKey key;
    LogInScript logInScript;
    SessionManager sessionManager;
    MyIAPManager myIAPManager;
    SubscriptionsWays subscriptionsManager;

    string gender = "";
    int[] dobYMD;
    static bool alreadyLogged = false;

    int numKids;
    int monthsOfSubs;
    int availableKidsInSubscription;

    List<int> ids = new List<int>();
    List<Button> miniCanvitas = new List<Button>();

    void Awake()
    {
        //here we start the process of initrilization
        Initialization();
        dobYMD = new int[0];
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log(PlayerPrefs.GetInt(Keys.Logged_In) + " logged in");
        if (key == null)
        {
            if (PlayerPrefs.GetInt(Keys.Logged_In) == 1)
            {

                string user = PlayerPrefs.GetString(Keys.Active_User_Key);
                if (user != "_local")
                {
                    if (user != "")
                    {
                        ShowLoading();
                        logInScript.IsActive(user);
                    }
                    else
                    {
                        ShowLogIn();
                    }
                }
                else
                {
                    if (PlayerPrefs.GetInt(Keys.Logged_In) == 1)
                    {
                        ShowGameMenu();
                    }
                    else
                    {
                        ShowLogIn();
                    }
                }
            }
            else
            {
                if (alreadyLogged)
                {
                    ShowGameMenu();
                }
                else
                {
                    ShowLogIn();
                }
            }

            if (FindObjectOfType<EvaluationController>())
            {
                Destroy(FindObjectOfType<EvaluationController>().gameObject);
                //Destroy(FindObjectOfType<AudioManager>().gameObject);
            }
            key = FindObjectOfType<DemoKey>();
            savigDirectionText.text = Application.persistentDataPath + "/emergencysave.txt";
        }
        else
        {
            ShowGameMenu();
        }
    }

    // Update is called once per frame
    void Update()
    {

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
        TextReader.FillCommon(textOfAll);
        TextReader.FillAddables(textAddable);
        TextReader.FillBefore(textBefore);
        lines = TextReader.TextsToShow(textOfScene);
        warningLines = TextReader.TextsToShow(warningAsset);
        WriteTheTexts();
        ButtonSetUp();
        if (FindObjectOfType<DemoKey>())
        {
            key = FindObjectOfType<DemoKey>();
        }
    }

    //here we set almost every button in the ui with the correspondent function to do
    void ButtonSetUp()
    {
        evaluationButton.onClick.AddListener(ShowDisclaimer);
        gamesButton.onClick.AddListener(LoadGameMenus);
        gotAccountButton.onClick.AddListener(LogInMenuActive);
        createAccountButton.onClick.AddListener(CreateAccount);
        singInBackButton.onClick.AddListener(ShowLogIn);
        logInButton.onClick.AddListener(TryToLogIn);
        forgotPassButton.onClick.AddListener(ForgotPassword);
        returnLogInButton.onClick.AddListener(GoBack);
        singInButton.onClick.AddListener(CreateUser);
        termsAndConditionsButton.onClick.AddListener(GoToTermsAndConditions);
        singOutButton.onClick.AddListener(CloseSession);
        kidsButton.onClick.AddListener(SetKidsProfiles);
        aboutButton.onClick.AddListener(ShowCredits);
        exitCredits.onClick.AddListener(ShowGameMenu);
        selectionKidBackButton.onClick.AddListener(CloseKids);
        subscribeAnotherCountButton.onClick.AddListener(CloseSession);
        warningButton.onClick.AddListener(HideWarning);
        addKidButton.onClick.AddListener(AddKidShower);
        changeProfileButton.onClick.AddListener(SetKidsProfiles);
        newKidButton.onClick.AddListener(CreateAKid);
        moreAccountsNeedsButton.onClick.AddListener(MoreSubscriptions);
        shopInWeb.onClick.AddListener(MoreSubscriptions);
    }

    #endregion

    #region Shower Functions

    public void HideAllCanvas()
    {
        gameCanvas.SetActive(false);
        logInMenu.SetActive(false);
        accountCanvas.SetActive(false);
        logInCanavas.SetActive(false);
        singInCanvas.SetActive(false);
        kidsPanel.SetActive(false);
        creditCanvas.SetActive(false);
        subscribeCanvas.SetActive(false);
        warningPanel.SetActive(false);
        shopCanvas.SetActive(false);
        loadingCanvas.SetActive(false);
        newKidPanel.SetActive(false);
    }

    //we show the game menu if the player has acces to it
    public void ShowGameMenu()
    {
        HideAllCanvas();
        gameCanvas.SetActive(true);
		kidNameText.text = sessionManager.activeKid.name;
        IsTestIsAvailable();
    }

    //we show the log in menu for if the player has not sing in on log in
    public void ShowLogIn()
    {
        HideAllCanvas();
        logInMenu.SetActive(true);
        accountCanvas.SetActive(true);
    }

    //we give the player the log in format
    public void LogInMenuActive()
    {
        HideAllCanvas();
        logInMenu.SetActive(true);
        logInCanavas.SetActive(true);
    }

    //we give the user the create account format
    public void CreateAccount()
    {
        HideAllCanvas();
        logInMenu.SetActive(true);
        singInCanvas.SetActive(true);
    }

    public void ShowDisclaimer()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (!UnityEngine.iOS.Device.generation.ToString().Contains("iPad"))
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
            }
            else
            {
                ShowTheDisclaimer();
            }
        }
        else
        {
            ShowTheDisclaimer();
        }

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
    }

    public void ShowIOSDisclaimer()
    {
        HideAllCanvas();
        subscribeCanvas.SetActive(true);
        subscribeAnotherCountButton.gameObject.SetActive(false);
        changeProfileButton.gameObject.SetActive(false);
        subscribeButton.gameObject.SetActive(false);
        continueEvaluationButton.gameObject.SetActive(true);
        escapeEvaluationButton.gameObject.SetActive(true);
        WriteTheText(subscribeText, 44);
        warningLogo.gameObject.SetActive(false);
        suscripctionLogo.gameObject.SetActive(true);
        WriteTheText(escapeEvaluationButton, 42);
        WriteTheText(continueEvaluationButton, 41);
        continueEvaluationButton.onClick.RemoveAllListeners();
        escapeEvaluationButton.onClick.RemoveAllListeners();
        continueEvaluationButton.onClick.AddListener(() => ShowShop(0));
        escapeEvaluationButton.onClick.AddListener(ShopIAP);
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
        WriteTheText(subscribeButton, 29);
        if (sessionManager.activeUser.suscriptionsLeft < 1)
        {
            if (typeOfWarning == 0)
            {
                subscribeButton.onClick.AddListener(() => ShowShop(0));
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

    //we show the kids that are available for the player
    public void SetKidsProfiles()
    {
        WriteTheText(selectionKidText, 23);
        HideAllCanvas();
        kidsPanel.SetActive(true);

        WriteTheText(addKidButton, 30);
        addKidButton.onClick.RemoveAllListeners();
        addKidButton.onClick.AddListener(AddKidShower);
        selectionKidBackButton.gameObject.SetActive(true);

        if (sessionManager.activeUser != null)
        {
            List<GameObject> kidos = new List<GameObject>();
            float deltaSize = miniKidContainer.GetComponent<RectTransform>().sizeDelta.x;
            miniKidContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(0, miniKidContainer.GetComponent<RectTransform>().sizeDelta.y);
            for (int i = 0; i < miniKidContainer.transform.childCount; i++)
            {
                Destroy(miniKidContainer.transform.GetChild(i).gameObject);
            }
            int kidsNumber = sessionManager.activeUser.kids.Count;
            Debug.Log(kidsNumber + " this are kids numebrs");
            for (int i = 0; i < kidsNumber; i++)
            {
                kidos.Add(Instantiate(miniKidCanvas, miniKidContainer.transform));
                float addableSize = kidos[i].GetComponent<RectTransform>().sizeDelta.x * 2f;
                miniKidContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(miniKidContainer.GetComponent<RectTransform>().sizeDelta.x + addableSize, miniKidContainer.GetComponent<RectTransform>().sizeDelta.y);
                kidos[i].GetComponent<RectTransform>().parent = miniKidContainer.GetComponent<RectTransform>();
                if (i > 0)
                {
                    Vector2 pos = kidos[i - 1].GetComponent<RectTransform>().localPosition;
                    float positionOfCanvitas = pos.x + addableSize;
                    kidos[i].GetComponent<RectTransform>().localPosition = new Vector2(positionOfCanvitas, pos.y);
                }
                else
                {
                    float positionOfCanvitas = addableSize / 2;
                    kidos[i].GetComponent<RectTransform>().localPosition = new Vector2(positionOfCanvitas, kidos[i].GetComponent<RectTransform>().localPosition.y);
                }
                kidos[i].GetComponentInChildren<Text>().text = sessionManager.activeUser.kids[i].name;
                string parentkey = sessionManager.activeUser.kids[i].userkey;
                int id = sessionManager.activeUser.kids[i].id;
                kidos[i].GetComponent<Button>().onClick.AddListener(() => SetKidProfile(parentkey, id));
                if (!sessionManager.activeUser.kids[i].isActive)
                {
                    kidos[i].transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                    kidos[i].transform.GetChild(2).GetComponent<Image>().color = Color.grey;
                }
            }
        }

        sessionManager.SaveSession();
    }

    //we shoe the genius that make this game
    void ShowCredits()
    {
        HideAllCanvas();
        creditCanvas.SetActive(true);
    }

    //we show the shop and gives them the option to buy
    void ShowShop(int shopForNewKid)
    {

        HideAllCanvas();
        shopCanvas.SetActive(true);
        prepaidButton.gameObject.SetActive(true);
        inputPrepaidCode.gameObject.SetActive(false);
        sendCardButton.gameObject.SetActive(false);
        codeText.gameObject.SetActive(false);
        quantityKidsDrop.gameObject.SetActive(false);
        moreAccountsNeedsButton.gameObject.SetActive(false);
        shopIAPButton.gameObject.SetActive(false);
        SetShop();

        sendCardButton.onClick.RemoveAllListeners();
        prepaidButton.onClick.RemoveAllListeners();
        oneMonthButton.onClick.RemoveAllListeners();
        threeMonthButton.onClick.RemoveAllListeners();
        shopBackButton.onClick.RemoveAllListeners();
        sendCardButton.onClick.AddListener(() => ChangeAPrePaidCode(shopForNewKid));
        prepaidButton.onClick.AddListener(() => ShopWithCode(shopForNewKid));
        oneMonthButton.onClick.AddListener(() => ShopNumOfKids(shopForNewKid, 1));
        threeMonthButton.onClick.AddListener(() => ShopNumOfKids(shopForNewKid, 3));
        shopBackButton.onClick.AddListener(() => ShowAccountWarning(shopForNewKid));
    }

    //we start the proceess of purchasing a suscription with a prepaid code
    public void ShopWithCode(int showShop)
    {
        HideAllCanvas();
        shopCanvas.SetActive(true);
        oneMonthButton.gameObject.SetActive(false);
        threeMonthButton.gameObject.SetActive(false);
        prepaidButton.gameObject.SetActive(false);
        shopInWeb.gameObject.SetActive(false);
        inputPrepaidCode.gameObject.SetActive(true);
        sendCardButton.gameObject.SetActive(true);
        codeText.gameObject.SetActive(true);
        WriteTheText(codeText, 37);
        quantityKidsDrop.gameObject.SetActive(false);
        moreAccountsNeedsButton.gameObject.SetActive(false);
        shopIAPButton.gameObject.SetActive(false);

        shopBackButton.onClick.RemoveAllListeners();
        shopBackButton.onClick.AddListener(() => ShowShop(showShop));
    }

    //we let the player to shop with the InAppPurchase system this only works up to 5 kids
    void ShopNumOfKids(int showShop, int months)
    {
        HideAllCanvas();
        shopCanvas.SetActive(true);
        oneMonthButton.gameObject.SetActive(false);
        threeMonthButton.gameObject.SetActive(false);
        prepaidButton.gameObject.SetActive(false);
        shopInWeb.gameObject.SetActive(false);
        inputPrepaidCode.gameObject.SetActive(false);
        sendCardButton.gameObject.SetActive(false);
        codeText.gameObject.SetActive(true);
        WriteTheText(codeText, 38);
        quantityKidsDrop.gameObject.SetActive(true);
        moreAccountsNeedsButton.gameObject.SetActive(true);
        shopIAPButton.gameObject.SetActive(true);

        monthsOfSubs = months;
        shopBackButton.onClick.RemoveAllListeners();
        shopIAPButton.onClick.RemoveAllListeners();
        shopBackButton.onClick.AddListener(()=>ShowShop(showShop));
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            shopIAPButton.onClick.AddListener(IOSExtraStep);
        }
        else
        {
            shopIAPButton.onClick.AddListener(ShopIAP);
        }
    }

    void ShopIAP()
    {
        numKids = quantityKidsDrop.value + 1;
        if (monthsOfSubs == 1)
        {
            myIAPManager.BuySubscriptionOneMonth(numKids);
        }
        else
        {
            myIAPManager.BuySubscriptionThreeMonths(numKids);
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
    void ShowLoading()
    {
        HideAllCanvas();
        loadingCanvas.SetActive(true);
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
    void LoadGameMenus()
    {
        PrefsKeys.SetNextScene("GameMenus");
        SceneManager.LoadScene("Loader_Scene");
    }

    //this method tries to get us login 
    void TryToLogIn()
    {
        string email = emailLogInInput.text;
        string password = passLogInInput.text;
        EmailVerificationUtility verificationUtility = new EmailVerificationUtility();
        bool isAccountReady = verificationUtility.IsValidMail(email);
        bool isPassReady = (password != "");
        if (isAccountReady)
        {
            if (isPassReady)
            {
                ShowLoading();
                logInScript.PostLogin(email, password);
            }
            else
            {
                ShowWarning(0);
            }
        }
        else
        {
            ShowWarning(1);
        }
    }

    //this method creates a user and a kid
    void CreateUser()
    {
        Debug.Log("we try to create user");
        string mail = dadMailInput.text;
        string pass = dadPassInput.text;
        string pass2 = dadPassRepeatInput.text;
        string kidName = kidNameInput.text;
        string kidDob = DefineTheDateOfBirth(0);
        EmailVerificationUtility verificationUtility = new EmailVerificationUtility();
        if (mail != "" && pass != "" && pass2 != "" && kidName != "" && kidDob != "")
        {

            if (verificationUtility.IsValidMail(mail))
            {
                if (pass.Length >= 8)
                {
                    if (pass == pass2)
                    {
                        if (KidDateIsOK(0))
                        {
                            ShowLoading();
                            logInScript.RegisterParentAndKid(mail, pass, kidName, kidDob);
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
    void ChangeAPrePaidCode(int isNewChild)
    {
        ShowLoading();
        if (isNewChild == 0)
        {
            subscriptionsManager.SendACode(sessionManager.activeUser.id, sessionManager.activeKid.id, inputPrepaidCode.text, isNewChild);
        }
        else
        {
            subscriptionsManager.SendACode(sessionManager.activeUser.id, inputPrepaidCode.text,isNewChild);
        }
    }

    //this one will set a kid as an active kid if its selected
    void SetKidProfile(string parentKey, int id)
    {
        sessionManager.SetKid(parentKey, id);
        if (sessionManager.activeKid.isActive)
        {
            IsTestIsAvailable();
            ShowGameMenu();
        }
        else
        {
            ShowAccountWarning(0);
        }
    }

    //this will set all available kids to show wich of them you will add to your subscription plan
    public void SetKidProfilesToAddASubscription(int numOfKids, string typeOfSubscription, UnityEngine.Purchasing.PurchaseEventArgs args)
    {
        WriteTheText(selectionKidText, 24);
        HideAllCanvas();
        kidsPanel.SetActive(true);
        numKids = numOfKids;

        WriteTheText(addKidButton, 31);
        addKidButton.onClick.RemoveAllListeners();
        addKidButton.onClick.AddListener(() => ConfirmKidsPurchase(args, numOfKids, typeOfSubscription));
        selectionKidBackButton.gameObject.SetActive(false);

        if (sessionManager.activeUser != null)
        {
            
            List<GameObject> kidos = new List<GameObject>();
            float deltaSize = miniKidContainer.GetComponent<RectTransform>().sizeDelta.x;
            miniKidContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(0, miniKidContainer.GetComponent<RectTransform>().sizeDelta.y);
            for (int i = 0; i < miniKidContainer.transform.childCount; i++)
            {
                Destroy(miniKidContainer.transform.GetChild(i).gameObject);
            }
            int kidsNumber = sessionManager.activeUser.kids.Count;
            Debug.Log(kidsNumber + " this are kids numebrs");
            for (int i = 0; i < kidsNumber; i++)
            {
                kidos.Add(Instantiate(miniKidCanvas, miniKidContainer.transform));
                float addableSize = kidos[i].GetComponent<RectTransform>().sizeDelta.x * 2f;
                miniKidContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(miniKidContainer.GetComponent<RectTransform>().sizeDelta.x + addableSize, miniKidContainer.GetComponent<RectTransform>().sizeDelta.y);
                kidos[i].GetComponent<RectTransform>().parent = miniKidContainer.GetComponent<RectTransform>();
                if (i > 0)
                {
                    Vector2 pos = kidos[i - 1].GetComponent<RectTransform>().localPosition;
                    float positionOfCanvitas = pos.x + addableSize;
                    kidos[i].GetComponent<RectTransform>().localPosition = new Vector2(positionOfCanvitas, pos.y);
                }
                else
                {
                    float positionOfCanvitas = addableSize / 2;
                    kidos[i].GetComponent<RectTransform>().localPosition = new Vector2(positionOfCanvitas, kidos[i].GetComponent<RectTransform>().localPosition.y);
                }
                kidos[i].GetComponentInChildren<Text>().text = sessionManager.activeUser.kids[i].name;
                string parentkey = sessionManager.activeUser.kids[i].userkey;
                int id = sessionManager.activeUser.kids[i].id;
                Button kidoButton = kidos[i].GetComponent<Button>();
                kidoButton.onClick.AddListener(() => SetKidIdToSubscriptionPlan(id, kidoButton));
                kidos[i].transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                kidos[i].transform.GetChild(2).GetComponent<Image>().color = Color.grey;
            }
        }
        sessionManager.SaveSession();
    }

    void SetKidIdToSubscriptionPlan(int id, Button button)
    {
        if (ids.Contains(id))
        {
            ids.Remove(id);
            button.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
            button.transform.GetChild(2).GetComponent<Image>().color = Color.grey;
            miniCanvitas.Remove(button);
        }
        else
        {
            ids.Add(id);
            button.transform.GetChild(0).GetComponent<Image>().color = Color.white;
            Color farben;
            ColorUtility.TryParseHtmlString("#AB6021", out farben);
            button.transform.GetChild(2).GetComponent<Image>().color = farben;
            miniCanvitas.Add(button);
            if (miniCanvitas.Count > numKids)
            {
                miniCanvitas[0].transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                miniCanvitas[0].transform.GetChild(2).GetComponent<Image>().color = Color.grey;
                miniCanvitas.Remove(miniCanvitas[0]);
                ids.Remove(ids[0]);
            }
        }
    }

    void ConfirmKidsPurchase(UnityEngine.Purchasing.PurchaseEventArgs args, int kids , string typeOfSubscription)
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

        subscriptionsManager.SendSubscriptionData(kids, idsSt, sessionManager.activeUser.id, typeOfSubscription, args);
    }

    //this one goes and reset the password for a player if its needed
    void ForgotPassword()
    {
        Application.OpenURL(Keys.Api_Web_Key + "recuperar/");
    }

    //This is if a person regrets to start the registration process
    void GoBack()
    {
        dadMailInput.text = null;
        dadPassInput.text = null;
        dadPassRepeatInput.text = null;

        emailLogInInput.text = null;
        passLogInInput.text = null;
        ShowLogIn();
    }

    //this will send the app to terms and conditions
    void GoToTermsAndConditions()
    {
        Application.OpenURL("http://towi.com.mx/index.php/aviso-de-privacidad/");
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
        ShowLogIn();
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

    #endregion

    //This will set all the texts need for the menus
    void WriteTheTexts()
    {
        WriteTheText(evaluationButton, 0);
        WriteTheText(gamesButton, 1);
        WriteTheText(aboutButton, 2);
        WriteTheText(singOutButton, 3);
        WriteTheText(subscribeAnotherCountButton, 3);
        WriteTheText(gotAccountButton, 4);
        WriteTheText(createAccountButton, 5);
        WriteTheText(creditColumOne, 6);
        WriteTheText(creditColumTwo, 7);
        WriteTheText(logInButton, 8);
        WriteTheText(forgotPassButton, 9);
        WriteTheText(kidNameInput, 10);
        WriteTheText(dadMailInput, 11);
        WriteTheText(emailLogInInput, 11);
        WriteTheText(dadPassInput, 12);
        WriteTheText(passLogInInput, 12);
        WriteTheText(dadPassRepeatInput, 13);
        WriteTheText(singInButton, 14);
        WriteTheText(selectionKidBackButton, 15);
        WriteTheText(kidDateText, 16);
        WriteTheText(newKidBirthday, 16);
        WriteTheText(newKidDay, 17);
        WriteTheText(kidDayInput, 17);
        WriteTheText(newKidMonth, 18);
        WriteTheText(kidMonthInput, 18);
        WriteTheText(newKidYear, 19);
        WriteTheText(kidYearInput, 19);
        WriteTheText(kidMoreText, 20);
        WriteTheText(acceptTermsAndConditionText, 21);
        WriteTheText(termsAndConditionsButton, 22);
        WriteTheText(shopInWeb, 29);
        WriteTheText(prepaidButton, 36);
        WriteTheText(moreAccountsNeedsButton, 39);
        warningButton.GetComponentInChildren<Text>().text = TextReader.commonStrings[0];
        newKidButton.GetComponentInChildren<Text>().text = TextReader.commonStrings[0];
    }

    //This metods will set the text accordingly with the type of objects
    //This is for buttons
    void WriteTheText(Button but, int index)
    {
        but.GetComponentInChildren<Text>().text = lines[index];
    }

    //This for Text
    void WriteTheText(Text text, int index)
    {
        text.text = lines[index];
    }

    //This for Dopdowns
    void WriteTheText(Dropdown drop, int dropIndex, int index)
    {
        drop.options[dropIndex].text = lines[index];
    }

    //This for InputField placeholders
    void WriteTheText(InputField field, int index)
    {
        field.placeholder.GetComponent<Text>().text = lines[index];
    }

    //This will display a warning and show what was the error if that will occure
    public void ShowWarning(int numberOfWarning)
    {
        warningPanel.SetActive(true);
        warningText.text = warningLines[numberOfWarning];
        if(numberOfWarning == 11)
        {

        }
    }

    public void MoreSubscriptions()
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
        PlayerPrefs.GetString(Keys.Active_User_Key, sessionManager.activeUser.userkey);
        Debug.Log("its logged");
    }

    //this method shows if a kid has the evaluation available for playing
    void IsTestIsAvailable()
    {
        if (sessionManager.activeKid != null)
        {
            if (sessionManager.activeKid.testAvailable)
            {
                evaluationButton.gameObject.SetActive(true);
            }
            else
            {
                evaluationButton.gameObject.SetActive(false);
            }
        }
    }

    void IOSExtraStep()
    {

    }

    //This will set the shop
    void SetShop()
    {
        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows)
        {
            oneMonthButton.gameObject.SetActive(false);
            threeMonthButton.gameObject.SetActive(false);
            shopInWeb.gameObject.SetActive(true);
        }
        else
        {
            if (sessionManager.activeUser.isPossibleBuyIAP)
            {
                oneMonthButton.gameObject.SetActive(true);
                threeMonthButton.gameObject.SetActive(true);
                shopInWeb.gameObject.SetActive(false);
                oneMonthButton.GetComponentInChildren<Text>().text = lines[33] + " " + myIAPManager.CostInCurrency(1) + lines[35];
                threeMonthButton.GetComponentInChildren<Text>().text = lines[34] + " " + myIAPManager.CostInCurrency(3) + lines[35];
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    prepaidButton.gameObject.SetActive(false);
                }
            }
            else
            {
                oneMonthButton.gameObject.SetActive(false);
                threeMonthButton.gameObject.SetActive(false);
                shopInWeb.gameObject.SetActive(true);
            }
        }
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
        if (existDad == 0)
        {
            if (kidDayInput.text != "" && kidMonthInput.text != "" && kidYearInput.text != "")
            {
                dobYMD = new int[] { int.Parse(kidYearInput.text), int.Parse(kidMonthInput.text), int.Parse(kidDayInput.text) };
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
            Debug.Log("dob is " + date);
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
            year = int.Parse(kidYearInput.text);
            month = int.Parse(kidMonthInput.text);
            day = int.Parse(kidDayInput.text);
        }
        else
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

}
