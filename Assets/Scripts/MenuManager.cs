using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    [Header("Texts To Show")]
    public TextAsset textOfScene;
    public TextAsset textOfAll;
    public TextAsset textBefore;
    public TextAsset textAddable;
    public TextAsset creditsAsset;
    public TextAsset warningAsset;
    string[] lines;

    #region UI Elements
    [Header("Game UI")]
    public GameObject gameCanvas;
    public Button evaluationButton;
    public Button gamesButton;
    public Button aboutButton;
    public Button kidsButton;
    public Button singOutButton;
    public Text savigDirectionText;

    [Header("Log in UI")]
    public GameObject logInMenu;
    public GameObject logAndSingPanel;
    public GameObject accountCanvas;
    public GameObject logInCanavas;
    public GameObject singInCanvas;
    public GameObject kidsPanel;
    public GameObject warningPanel;
    public Text ornamentText;

    [Header("Account Manager UI")]
    public Button gotAccountButton;
    public Button createAccountButton;


    [Header("Log in canvas UI")]
    public InputField emailLogInInput;
    public InputField passLogInInput;
    public Text emailLogInText;
    public Text passLogInText;
    public Button forgotPassButton;
    public Button logInButton;
    public Button returnLogInButton;

    [Header("Sing In UI")]
    public InputField dadMailInput;
    public Text dadMailText;
    public InputField dadPassInput;
    public Text dadPassText;
    public InputField dadPassRepeatInput;
    public Text dadPassRepeatText;
    public InputField kidNameInput;
    public Text kidNameText;
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

    [Header("Credits")]
    public GameObject creditCanvas;
    public Button exitCredits;
    public Text creditText;

    [Header("Subscribe")]
    public GameObject subscribeCanvas;
    public Text subscribeText;
    public Button subscribeButton;
    public Button subscribeAnotherCountButton;

    [Header("Shop Button")]
    public GameObject shopCanvas;
    public Button oneMonthButton;
    public Button threeMonthButton;
    public Button prepaidButton;
    public Button sendButton;
    public InputField input;

    [Header("Warnings")]
    public Text warningText;
    public Button warningButton;
    string[] warningLines;

    #endregion

    AudioManager audioManager;
    EvaluationController evaluationController;

    DemoKey key;
    LogInScript logInScript;
    SessionManager sessionManager;
    MyIAPManager myIAPManager;

    string gender = "";
    int[] dobYMD;
    static bool alreadyLogged = false;

    private void Awake()
    {
        Initialization();
        dobYMD = new int[0];
    }

    // Use this for initialization
    void Start()
    {
        if (key == null)
        {
            if (PlayerPrefs.GetInt(Keys.Purchase_Key) == 1 || PlayerPrefs.GetInt(Keys.Subscription_Purchased_Key) == 1)
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
                        ShowLogIn();
                    }
                }
                else
                {
                    if (PlayerPrefs.GetInt(Keys.Purchase_Key) == 1)
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
    void Update() {

    }

    #region Functions

    #region Set Up Functions

    void Initialization()
    {
        logInScript = GetComponent<LogInScript>();
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
        SetShop();
    }

    void ButtonSetUp()
    {
        evaluationButton.onClick.AddListener(LoadEvaluation);
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
    }

#endregion

#region Shower Functions

    public void ShowGameMenu()
    {
        gameCanvas.SetActive(true);
        logInMenu.SetActive(false);
        accountCanvas.SetActive(false);
        logInCanavas.SetActive(false);
        singInCanvas.SetActive(false);
        kidsPanel.SetActive(false);
        creditCanvas.SetActive(false);
        subscribeCanvas.SetActive(false);
        warningPanel.SetActive(false);
        shopCanvas.SetActive(false);
        IsTestIsAvailable();
    }

    public void ShowLogIn()
    {
        gameCanvas.SetActive(false);
        logInMenu.SetActive(true);
        accountCanvas.SetActive(true);
        logInCanavas.SetActive(false);
        singInCanvas.SetActive(false);
        kidsPanel.SetActive(false);
        creditCanvas.SetActive(false);
        subscribeCanvas.SetActive(false);
        warningPanel.SetActive(false);
        shopCanvas.SetActive(false);
    }

    void LogInMenuActive()
    {
        gameCanvas.SetActive(false);
        logInMenu.SetActive(true);
        accountCanvas.SetActive(false);
        logInCanavas.SetActive(true);
        singInCanvas.SetActive(false);
        kidsPanel.SetActive(false);
        creditCanvas.SetActive(false);
        subscribeCanvas.SetActive(false);
        warningPanel.SetActive(false);
        shopCanvas.SetActive(false);
    }

    public void ShowTrialIsOff()
    {
        gameCanvas.SetActive(false);
        logInMenu.SetActive(false);
        accountCanvas.SetActive(false);
        logInCanavas.SetActive(false);
        singInCanvas.SetActive(false);
        kidsPanel.SetActive(false);
        creditCanvas.SetActive(false);
        subscribeCanvas.SetActive(true);
        warningPanel.SetActive(false);
        shopCanvas.SetActive(false);
    }

    void CreateAccount()
    {
        gameCanvas.SetActive(false);
        logInMenu.SetActive(true);
        accountCanvas.SetActive(false);
        logInCanavas.SetActive(false);
        singInCanvas.SetActive(true);
        kidsPanel.SetActive(false);
        creditCanvas.SetActive(false);
        subscribeCanvas.SetActive(false);
        warningPanel.SetActive(false);
        shopCanvas.SetActive(false);
    }

    public void SetKidsProfiles()
    {
        gameCanvas.SetActive(false);
        logInMenu.SetActive(false);
        accountCanvas.SetActive(false);
        logInCanavas.SetActive(false);
        singInCanvas.SetActive(false);
        kidsPanel.SetActive(true);
        creditCanvas.SetActive(false);
        subscribeCanvas.SetActive(false);
        warningPanel.SetActive(false);
        shopCanvas.SetActive(false);
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

    void ShowCredits()
    {
        gameCanvas.SetActive(false);
        logInMenu.SetActive(false);
        accountCanvas.SetActive(false);
        logInCanavas.SetActive(false);
        singInCanvas.SetActive(false);
        kidsPanel.SetActive(false);
        creditCanvas.SetActive(true);
        subscribeCanvas.SetActive(false);
        warningPanel.SetActive(false);
        shopCanvas.SetActive(false);
    }

    void ShowShop()
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
        shopCanvas.SetActive(true);
        oneMonthButton.GetComponentInChildren<Text>().text = lines[39] + " " + myIAPManager.CostInCurrency(1);
        threeMonthButton.GetComponentInChildren<Text>().text = lines[40] + " " + myIAPManager.CostInCurrency(3);
    }



    #endregion

    #region Button Functions

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

    void LoadGameMenus()
    {
        PrefsKeys.SetNextScene("GameMenus");
        SceneManager.LoadScene("Loader_Scene");
    }

    void TryToLogIn()
    {
        string email = emailLogInInput.text;
        string password = passLogInInput.text;
        bool isAccountReady = (email != "");
        bool isPassReady = (password != "");
        if (isAccountReady)
        {
            if (isPassReady)
            {
                logInScript.PostLogin(email, password);
            }
            else
            {
                ShowWarning(0);
                Debug.Log("Necesitas ingresar tu contraseña");
                Debug.Log("there should be some password");
            }
        }
        else
        {
            ShowWarning(1);
            Debug.Log("There should be something in the mail");
            Debug.Log("Necesitas ingresar tu correo");
        }
    }

    /*void TryToRegister()
    {
        string kidName = kidNameInput.text;
        if (kidName != "")
        {
            if (KidDateIsOK())
            {
                string email = dadMailInput.text;
                string dadName = dadNameInput.text;
                //string dadLastname = dadLastNameInput.text;
                string pass = dadPassInput.text;
                //string kidLastname = kidLastnameInput.text;
                string kidDate = DefineTheDateOfBirth();
                string kidGender = gender;
                logInScript.RegisterParentAndKid(dadName, email, pass, kidName, kidDate, kidGender);
            }
            else
            {
                Debug.Log("input a valid date");
            }
        }
        else
        {
            Debug.Log("Input a name");
        }


    }*/

    void CreateUser()
    {
        Debug.Log("we try to create user");
        string mail = dadMailInput.text;
        string pass = dadPassInput.text;
        string pass2 = dadPassRepeatInput.text;
        string kidName = kidNameInput.text;
        string kidDob = DefineTheDateOfBirth();
        EmailVerificationUtility verificationUtility = new EmailVerificationUtility();
        if (mail != "" && pass != "" && pass2 != "" && kidName != "" && kidDob != "")
        {

            if (verificationUtility.IsValidMail(mail))
            {
                if (pass.Length >= 8)
                {
                    if (pass == pass2)
                    {
                        Debug.Log("try to log in");
                        logInScript.RegisterParentAndKid(mail, pass, kidName, kidDob);
                    }
                    else
                    {

                        Debug.Log("Passwords doesnt Match");
                        ShowWarning(7);
                    }
                }
                else
                {
                    Debug.Log("pass is not long enough");
                    ShowWarning(6);
                }
            }
            else
            {
                Debug.Log("Input a valid mail");
                ShowWarning(5);
            }

        }
        else
        {
            Debug.Log("All field are needed");
            ShowWarning(4);
        }
    }

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
            ShowTrialIsOff();
        }
    }

    void ForgotPassword()
    {
        Application.OpenURL(Keys.Api_Web_Key + "recuperar/");
    }

    void GoBack()
    {
        dadMailInput.text = null;
        dadPassInput.text = null;
        dadPassRepeatInput.text = null;

        emailLogInInput.text = null;
        passLogInInput.text = null;
        ShowLogIn();
    }

    void GoToTermsAndConditions()
    {
        Application.OpenURL("http://towi.com.mx/index.php/aviso-de-privacidad/");
    }

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

    void CloseSession()
    {
        PlayerPrefs.SetInt(Keys.Purchase_Key, 0);
        PlayerPrefs.SetInt(Keys.Subscription_Purchased_Key, 0);
        PlayerPrefs.SetString("sessions", "");
        ShowLogIn();
        sessionManager.StartAgain();
        alreadyLogged = false;
    }

    #endregion

    void WriteTheTexts()
    {
        WriteTheText(evaluationButton, 0);
        WriteTheText(gamesButton, 1);
        WriteTheText(aboutButton, 2);
        WriteTheText(singOutButton, 3);
        WriteTheText(subscribeAnotherCountButton, 3); 
        WriteTheText(ornamentText, 4);
        WriteTheText(gotAccountButton, 5);
        WriteTheText(createAccountButton, 6);
        WriteTheText(emailLogInText, 8);
        WriteTheText(passLogInText, 9);
        WriteTheText(logInButton, 10);
        //WriteTheText(returnLogInButton, 11);
        //WriteTheText(kidReturnButton, 11);
        WriteTheText(forgotPassButton, 12);
        //WriteTheText(dadRegistryText, 13);
        //WriteTheText(dadNameText, 14);
        WriteTheText(kidNameText, 15);
        //WriteTheText(dadLastNameText, 15);
        WriteTheText(dadMailText, 16);
        WriteTheText(dadPassText, 17);
        WriteTheText(dadPassRepeatText, 18);
        //WriteTheText(dadWarningText, 19);
        WriteTheText(singInButton, 20);
        //WriteTheText(singInDadCancelButton, 21);
        WriteTheText(selectionKidBackButton, 21);
        /*WriteTheText(kidRegistryText, 22);
        WriteTheText(kidLastnameText, 23);*/
        WriteTheText(kidDateText, 24);
        /*WriteTheText(kidDayText, 25);
        WriteTheText(kidMonthText, 26);
        WriteTheText(kidYearText, 27);
        WriteTheText(kidGenderText, 28);
        WriteTheText(kidSexDropdown, 0, 29);
        WriteTheText(kidSexDropdown, 1, 30);
        WriteTheText(kidSexDropdown, 2, 31);*/
        WriteTheText(kidMoreText, 32);
        WriteTheText(acceptTermsAndConditionText, 33);
        WriteTheText(termsAndConditionsButton, 34);
        //WriteTheText(kidSendButton, 35);
        WriteTheText(selectionKidText, 36);
        WriteTheText(subscribeText, 37);
        WriteTheText(subscribeButton, 38);
        //creditText.text = creditsAsset.text;
    }

    void WriteTheText(Button but, int index)
    {
        but.GetComponentInChildren<Text>().text = lines[index];
    }

    void WriteTheText(Text text, int index)
    {
        text.text = lines[index];
    }

    void WriteTheText(Dropdown drop, int dropIndex, int index)
    {
        drop.options[dropIndex].text = lines[index];
    }

    public void ShowWarning(int numberOfWarning)
    {
        warningPanel.SetActive(true);
        warningText.text = warningLines[numberOfWarning];
    }

    void HideWarning()
    {
        warningPanel.SetActive(false);
    }

    public void LoggedNow()
    {
        alreadyLogged = true;
        PlayerPrefs.SetInt(Keys.Purchase_Key, 1);
        PlayerPrefs.SetInt(Keys.Subscription_Purchased_Key, 1);
    }

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

    void SetShop()
    {
        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows)
        {
            subscribeButton.onClick.AddListener(ShopWindows);
        }
        else
        {
            subscribeButton.onClick.AddListener(ShowShop);
            oneMonthButton.onClick.AddListener(myIAPManager.BuySubscriptionOneMonth);
            threeMonthButton.onClick.AddListener(myIAPManager.BuySubscriptionThreeMonths);

        }
    }

    void ShopWindows()
    {
        Application.OpenURL(Keys.Api_Web_Key + "subscripciones/");
        Debug.Log("Open shop");
    }

    bool DatesFromInput()
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

    string DefineTheDateOfBirth()
    {
        if (DatesFromInput())
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

    bool KidDateIsOK()
    {
        int year = int.Parse(kidYearInput.text);
        int month = int.Parse(kidMonthInput.text);
        int day = int.Parse(kidDayInput.text);
        Debug.Log(year + " " + month + " " + day);
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
    #endregion

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
