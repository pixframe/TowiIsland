using UnityEngine;
using System.Collections;
using Boomlagoon.JSON;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

public class Login : MonoBehaviour
{
    string username = "";
    string psswd = "";
    public string loginUrl;
    public string activeUrl;
    public string suscriptionUrl;
    public Texture loginBox;
    public Texture kiwiLogo;
    public Texture loadingScreen;
    public Texture es_loadingScreen;
    public Texture en_loadingScreen;
    public Texture loadingColor;
    public Texture towiLogo;
    public Texture towiLogo2;
    public Texture towiLogo3;
    public Sprite kidsBg;
    public ArrowController backArrowController;
    public SpriteRenderer backArrow;
    public Transform profile;
    public GUIStyle inputStyle;
    public GUIStyle labelStyle;
    public GUIStyle loginStyle;
    public GUIStyle errorStyle;
    public GUIStyle errorStyle2;
    public GUIStyle trialButton;
    public Texture2D es_trial;
    public Texture2D es_trialActive;
    public Texture2D en_trial;
    public Texture2D en_trialActive;
    public Texture2D es_buy;
    public Texture2D es_buyActive;
    public Texture2D es_subscribe;
    public Texture2D es_subscribeActive;
    public Texture2D en_buy;
    public Texture2D en_buyActive;
    public Texture2D en_subscribe;
    public Texture2D en_subscribeActive;
    public GUIStyle versionStyle;
    public GUIStyle exitButton;
    public float userLabelOffset = 50;
    public float userInputOffset = 100;
    public float psswdLabelOffset = 150;
    public float psswdInputOffset = 200;
    public float loginOffset = 300;
    public float errorInputOffset = 400;
    public string errorText = "";
    public string version = "1.0.0";
    float opacity = 0;
    bool loading = false;
    [HideInInspector]
    public bool fadeIn = false;
    public bool fadeOut = false;
    float mapScale;
    [HideInInspector]
    public SessionManager sessionMng;
    Transform camRef;
    Camera camRend;
    Transform kidsRef;
    TextMesh textRef;
    SpriteRenderer bgRef;
    SpriteRenderer nextArrowRef;
    SpriteRenderer prefArrowRef;
    [HideInInspector]
    public SpriteRenderer loaderRef;
    public enum Phase { Trial, TrialOptions, Login, Menu, Profiles, About, Loading };
    int state = 0;
    public Phase currentState = Phase.Trial;
    Vector3[] positions;
    List<Transform> profiles;
    float exitOffset = 0;
    float enterOffset = -1;
    [HideInInspector]
    public bool changeProfiles = false;
    int profilePivot = 0;
    public float profileSpeed = 0.5f;
    bool forward = true;
    bool switching = false;
    float switchTime = 0;
    public bool waitingForPurchase = false;
    bool waitingForLogin = false;

    //Trial Options
    public Texture blueBg;
    public Texture greenBg;
    public GUIStyle optionTitle;
    public GUIStyle optionInfo;
    public GUIStyle remainingText;
    public GUIStyle daysText;
    public GUIStyle buyButton;
    public GUIStyle suscriptionButton;
    public GUIStyle gameButton;
    public GUIStyle linkStyle;
    public static bool local = false;
    public static bool alreadyLogged = false;

    //Menu
    public Transform menu;
    GameObject effects;
    MenuController[] menuControllers;
    public GUIStyle aboutTextStyle;
    public GUIStyle closeButton;
    public GUIStyle nameTextStyle;
    public GUIStyle surnameTextStyle;
    public GUIStyle logTextStyle;
    public GUIStyle languageStyle;
    public Texture profilePic;
    public bool showLogin = false;
    public Texture overlay;
    public Texture blueColor;
    TrialManager trial;
    bool avoidSwitch = false;

    //FroemaRegistro
    public GUIStyle inputsNamesStyle;
    public GUIStyle inputsStyle;
    public GUIStyle nextStyle;
    public GUIStyle skipStyle;
    public GUIStyle regInfoStyle;
    public Texture formInfoTexture;
    public Texture platformPreview;
    public Texture platformKey;

    string regParentName = "";
    string regParentSurname = "";
    string regParentMail = "";
    string regParentPsswd = "";
    string regParentPsswdMask = "";
    string regParentPsswdCheck = "";
    string regParentPsswdCheckMask = "";
    string regKidName = "";
    string regKidSurname = "";
    string keySerial = "";
    public bool showRegister;
    bool showRegisterButton;
    bool showEnterKey;
    public int registerStep = 0;
    BuyController buyController;
    [HideInInspector]
    public LanguageLoader language;
    string lang = "es";

	public GameObject pruebaBut;
	public GameObject playBut;

	bool auxisPlay = false;
	bool auxisPrueb = false;

    // Use this for initialization
    void Start()
    {


        sessionMng = GetComponent<SessionManager>();
        language = GetComponent<LanguageLoader>();
        menuControllers = menu.GetComponentsInChildren<MenuController>();
        menu.gameObject.SetActive(false);
        camRef = GameObject.Find("Camera").transform;
        camRend = camRef.GetComponent<Camera>();
        kidsRef = camRef.Find("Kids");
        effects = camRef.Find("Confetti").gameObject;
        backArrowController = camRef.Find("BackArrow").GetComponent<ArrowController>();
        backArrow = camRef.Find("BackArrow").GetComponent<SpriteRenderer>();
        bgRef = camRef.Find("KidBg").GetComponent<SpriteRenderer>();
        textRef = camRef.Find("Text").GetComponent<TextMesh>();
        ChangeLanguage();
        loaderRef = camRef.Find("Loader").GetComponent<SpriteRenderer>();
        nextArrowRef = camRef.Find("NextArrow").GetComponent<SpriteRenderer>();
        prefArrowRef = camRef.Find("PrevArrow").GetComponent<SpriteRenderer>();
        profiles = new List<Transform>();
        mapScale = (float)Screen.height / (float)768;
		nameTextStyle.fontSize = (int)(nameTextStyle.fontSize * mapScale);
		surnameTextStyle.fontSize = (int)(surnameTextStyle.fontSize * mapScale);
        logTextStyle.fontSize = (int)(logTextStyle.fontSize * mapScale);
        inputStyle.fontSize = (int)(inputStyle.fontSize * mapScale);
        labelStyle.fontSize = (int)(labelStyle.fontSize * mapScale);
        errorStyle.fontSize = (int)(errorStyle.fontSize * mapScale);
        errorStyle2.fontSize = (int)(errorStyle2.fontSize * mapScale);
        optionTitle.fontSize = (int)(optionTitle.fontSize * mapScale);
        optionInfo.fontSize = (int)(optionInfo.fontSize * mapScale);
        remainingText.fontSize = (int)(remainingText.fontSize * mapScale);
        daysText.fontSize = (int)(daysText.fontSize * mapScale);
        inputsNamesStyle.fontSize = (int)(inputsNamesStyle.fontSize * mapScale);
        inputsStyle.fontSize = (int)(inputsStyle.fontSize * mapScale);
        nextStyle.fontSize = (int)(nextStyle.fontSize * mapScale);
        skipStyle.fontSize = (int)(skipStyle.fontSize * mapScale);
        regInfoStyle.fontSize = (int)(regInfoStyle.fontSize * mapScale);
        aboutTextStyle.fontSize = (int)(aboutTextStyle.fontSize * mapScale);
        versionStyle.fontSize = (int)(versionStyle.fontSize * mapScale);
        linkStyle.fontSize = (int)(linkStyle.fontSize * mapScale);
        languageStyle.fontSize = (int)(languageStyle.fontSize * mapScale);
        PlayerPrefs.SetInt("PlayIntro", 1);
        PlayerPrefs.SetInt("Map", 0);
        trial = GetComponent<TrialManager>();
        buyController = GetComponent<BuyController>();
        showRegister = false;




        //showRegisterButton = PlayerPrefs.GetInt ("SubscriptionTrial", 0) == 0;
        showRegisterButton = true;
        //remainingDays = trial.GetRemaining();
        if ((PlayerPrefs.GetInt("purchased", 0) == 1 || PlayerPrefs.GetInt("subscriptionPurchased", 0) == 1) && !local)
        {
            //if(alreadyLogged)
            //{
            if (PlayerPrefs.GetString("activeUser", "") != "_local")
            {
                if (PlayerPrefs.GetString("activeUser", "") != "")
                {
                    currentState = Phase.Menu;
                    waitingForLogin = true;
                    IsActive();
                }
                else
                {
                    currentState = Phase.TrialOptions;
                    DisplayLogin();
                }
            }
            else
            {
                if (PlayerPrefs.GetInt("purchased", 0) == 1)
                {
                    currentState = Login.Phase.Menu;
                    menu.gameObject.SetActive(true);
                    HideBG();
                    loaderRef.color = new Color(1, 1, 1, 0);
                }
                else if (PlayerPrefs.HasKey("TrialRemaining"))
                {
                    currentState = Phase.TrialOptions;
                }
            }
            //}
        }
        else
        {
            if (PlayerPrefs.GetString("TrialDay", "") != "")
            {
                currentState = Phase.TrialOptions;
                DisplayLogin();
            }
            //local=PlayerPrefs.GetInt("LocalGame")==1;
            if (local || alreadyLogged)
            {
                if (alreadyLogged)
                {
                    //sessionMng.LoadActiveUser();
                    lang = sessionMng.activeUser.language;
                    ChangeLanguage();
                    CreateProfiles();
                }
                currentState = Phase.Menu;
                menu.gameObject.SetActive(true);
            }
        }
        local = false;
    }

    // Update is called once per frame
    void Update()
    {
		
        if (fadeIn)
        {
            opacity += 1 * Time.deltaTime;
            if (currentState == Phase.Menu)
            {

                for (int i = 0; i < profiles.Count; i++)
                {
                    profiles[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, opacity);
                    profiles[i].Find("Text").GetComponent<TextMesh>().color = new Color(1, 1, 1, opacity);
					//profiles[i].FindChild("Text").GetComponent<TextMesh>().fontSize = 35;
                }
                bgRef.color = new Color(0, 0, 0, 1);
                backArrow.color = new Color(1, 1, 1, opacity);
                textRef.color = new Color(1, 1, 1, opacity);
                prefArrowRef.color = new Color(1, 1, 1, opacity);
                nextArrowRef.color = new Color(1, 1, 1, opacity);
            }
            if (opacity >= 1)
            {
                fadeIn = false;
                if (currentState == Phase.Menu && !local)
                {
                    opacity = 0;
                    currentState = Phase.Profiles;
                }
                else
                {
                    loading = true;
                }
            }
            GUI.color = new Color(1, 1, 1, opacity);
        }
        if (fadeOut)
        {
            opacity -= 1 * Time.deltaTime;
            switch (currentState)
            {
                case Phase.Profiles:
                    for (int i = 0; i < profiles.Count; i++)
                    {
                        profiles[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, opacity);
                        profiles[i].Find("Text").GetComponent<TextMesh>().color = new Color(1, 1, 1, opacity);
						//profiles[i].FindChild("Text").GetComponent<TextMesh>().fontSize = 35;
                    }
                    bgRef.color = new Color(0, 0, 0, opacity);
                    backArrow.color = new Color(1, 1, 1, opacity);
                    textRef.color = new Color(1, 1, 1, opacity);
                    prefArrowRef.color = new Color(1, 1, 1, opacity);
                    nextArrowRef.color = new Color(1, 1, 1, opacity);
                    break;
                case Phase.About:
                    HideBG();
                    backArrow.color = new Color(1, 1, 1, 0);
                    ShowMenu();
                    currentState = Login.Phase.Menu;
                    break;
            }
            if (opacity <= 0)
            {
                opacity = 0;
                fadeOut = false;
                if (currentState == Phase.Profiles)
                {
                    showLogin = false;
                    menu.gameObject.SetActive(true);
                    currentState = Phase.Menu;
                }
            }
            GUI.color = new Color(1, 1, 1, opacity);
        }
        RaycastHit hit;
        Ray collRay;
        int insID = -1;
        switch (currentState)
        {
            case Phase.Trial:
                if (switching)
                {
                    switchTime -= Time.deltaTime;
                    if (switchTime <= 0)
                    {
                        switching = false;
                        currentState = Phase.TrialOptions;
                        DisplayLogin();
                        DisplayLogin();
                    }
                }
                break;
            case Phase.Menu:
                if (!showRegister)
                {
                    collRay = camRef.GetComponent<Camera>().ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20));
                    insID = -1;
                    if (Physics.Raycast(collRay, out hit))
                    {
                        if (hit.collider.tag == "Menu")
                        {
                            insID = hit.collider.GetComponent<MenuController>().GetInstanceID();
                            hit.collider.GetComponent<MenuController>().SetHover();
                        }
                    }
                    for (int i = 0; i < menuControllers.Length; i++)
                    {
                        if (menuControllers[i].GetInstanceID() != insID)
                        {
                            menuControllers[i].SetNormal();
                        }
                    }
                }
                break;
            case Phase.Profiles:
                collRay = camRef.GetComponent<Camera>().ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20));
                insID = -1;
                if (Physics.Raycast(collRay, out hit))
                {
                    if (hit.collider.tag == "Profile")
                    {
                        Debug.Log("Enter");
                        insID = hit.collider.transform.GetInstanceID();
                        hit.collider.GetComponent<KidProfile>().SetHover();
                        Debug.Log(insID);
                    }
                    if (hit.collider.tag == "Arrow")
                    {
                        hit.collider.GetComponent<ArrowController>().SetHover();
                    }
                }
                else
                {
                    backArrowController.SetNormal();
                }
                foreach (Transform child in kidsRef)
                {
                    if (child.tag == "Profile")
                    {
                        if (child.GetInstanceID() != insID)
                        {
                            child.GetComponent<KidProfile>().SetNormal();
                        }
                    }
                }
                if (changeProfiles)
                {
                    float refExit = 0;
                    float refEnter = 0;
                    if (forward)
                    {
                        for (int i = profilePivot - 4; i < profilePivot; i++)
                        {
                            profiles[i].localPosition = new Vector3(profiles[i].localPosition.x, profiles[i].localPosition.y + Time.deltaTime * profileSpeed, profiles[i].localPosition.z);
                            refExit = profiles[i].localPosition.y;
                        }
                        for (int i = profilePivot; i < profilePivot + 4; i++)
                        {
                            if (i < profiles.Count)
                            {
                                profiles[i].localPosition = new Vector3(profiles[i].localPosition.x, profiles[i].localPosition.y + Time.deltaTime * profileSpeed, profiles[i].localPosition.z);
                                refEnter = profiles[i].localPosition.y;
                            }
                        }
                        if (refExit >= 1 && refEnter >= 0)
                        {
                            changeProfiles = false;
                        }
                    }
                    else
                    {
                        for (int i = profilePivot + 4; i < profilePivot + 8; i++)
                        {
                            if (i < profiles.Count)
                            {
                                profiles[i].localPosition = new Vector3(profiles[i].localPosition.x, profiles[i].localPosition.y - Time.deltaTime * profileSpeed, profiles[i].localPosition.z);
                                refExit = profiles[i].localPosition.y;
                            }
                        }
                        for (int i = profilePivot; i < profilePivot + 4; i++)
                        {
                            if (i < profiles.Count)
                            {
                                profiles[i].localPosition = new Vector3(profiles[i].localPosition.x, profiles[i].localPosition.y - Time.deltaTime * profileSpeed, profiles[i].localPosition.z);
                                refEnter = profiles[i].localPosition.y;
                            }
                        }
                        if (refExit <= -1 && refEnter <= 0)
                        {
                            changeProfiles = false;
                        }
                    }
                }
                break;
            case Phase.About:
                collRay = camRef.GetComponent<Camera>().ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20));
                if (Physics.Raycast(collRay, out hit))
                {
                    if (hit.collider.tag == "Arrow")
                    {
                        hit.collider.GetComponent<ArrowController>().SetHover();
                    }
                }
                else
                {
                    backArrowController.SetNormal();
                }
                break;
        }

		if (loading) {
			if (sessionMng.activeUser != null) 
			{
				sessionMng.activeUser.language = lang;
				sessionMng.SaveSession ();
			}
			//Descomentar/comentar la siguiente linea para build de Prueba o Build de juego

			if (pruebaBut.GetComponent<MenuController>().isPrueba && !playBut.GetComponent<MenuController>().isPlay) 
			{
				Application.LoadLevel ("PruebaEcologica");
				pruebaBut.GetComponent<MenuController> ().isPrueba = false;
			}
			else if(playBut.GetComponent<MenuController>().isPlay && !pruebaBut.GetComponent<MenuController>().isPrueba)
			{ 
				if (!sessionMng.activeKid.ageSet)
						Application.LoadLevel ("AgeSelection");
				else if (sessionMng.activeKid.avatar != "")
				{ 
				Debug.Log ("No voy a prueba  " );
				Application.LoadLevel ("Archipielago");				
				} else
				{
					Application.LoadLevel ("Selection");
				}
			}

            /*
            if(PlayerPrefs.HasKey("Avatar")&&PlayerPrefs.HasKey("Tests")){
                Application.LoadLevel ("Archipielago");
            }else{
                if(PlayerPrefs.HasKey("Avatar"))
                    Application.LoadLevel ("PruebaEcologica");
                else
                    Application.LoadLevel ("Selection");
            }*/
        }
    }

    void ChangeLanguage()
    {
        language.LoadGameLanguage(lang);
        textRef.text = language.levelStrings[35];
        Transform tempButtonMenu = camRef.Find("Menu").Find("Main");
        tempButtonMenu.Find("PlayButton").Find("Text").GetComponent<TextMesh>().text = language.levelStrings[31];
        tempButtonMenu.Find("AboutButton").Find("Text").GetComponent<TextMesh>().text = language.levelStrings[32];
        tempButtonMenu.Find("ExitButton").Find("Text").GetComponent<TextMesh>().text = language.levelStrings[34];
        switch (lang)
        {
            case "es":
                trialButton.normal.background = es_trial;
                trialButton.active.background = es_trialActive;
                buyButton.normal.background = es_buy;
                buyButton.active.background = es_buyActive;
                suscriptionButton.normal.background = es_subscribe;
                suscriptionButton.active.background = es_subscribeActive;
                loadingScreen = es_loadingScreen;
                break;
            case "en":
                trialButton.normal.background = en_trial;
                trialButton.active.background = en_trialActive;
                buyButton.normal.background = en_buy;
                buyButton.active.background = en_buyActive;
                suscriptionButton.normal.background = en_subscribe;
                suscriptionButton.active.background = en_subscribeActive;
                loadingScreen = en_loadingScreen;
                break;
        }
        if (sessionMng.activeUser != null)
        {
            sessionMng.activeUser.language = lang;
            sessionMng.SaveSession();
        }
    }

    public void CreateProfiles()
    {
        if (sessionMng.activeUser != null)
        {
            if (sessionMng.temporalKids.Count > 0)
            {
                if (sessionMng.temporalKids.Count > 4)
                {
                    nextArrowRef.gameObject.SetActive(true);
                }
                else
                {
                    nextArrowRef.gameObject.SetActive(false);
                }
                if (sessionMng.temporalKids.Count >= 4)
                {
                    positions = new Vector3[4];
                    positions[0] = new Vector3(-0.27f, 0, 0);
                    positions[1] = new Vector3(-0.09f, 0, 0);
                    positions[2] = new Vector3(0.09f, 0, 0);
                    positions[3] = new Vector3(0.27f, 0, 0);
                }
                else
                {
                    positions = new Vector3[sessionMng.temporalKids.Count];
                    switch (sessionMng.temporalKids.Count)
                    {
                        case 1:
                            positions[0] = new Vector3(0, 0, 0);
                            break;
                        case 2:
                            positions[0] = new Vector3(-0.09f, 0, 0);
                            positions[1] = new Vector3(0.09f, 0, 0);
                            break;
                        case 3:
                            positions[0] = new Vector3(-0.18f, 0, 0);
                            positions[1] = new Vector3(0, 0, 0);
                            positions[2] = new Vector3(0.18f, 0, 0);
                            break;
                    }
                }
                for (int i = 0; i < profiles.Count; i++)
                {
                    Destroy(profiles[i].gameObject);
                }
                profiles.Clear();
                for (int i = 0; i < sessionMng.temporalKids.Count; i++)
                {
                    profiles.Add(Instantiate(profile) as Transform);
                    profiles[profiles.Count - 1].GetComponent<KidProfile>().id = sessionMng.temporalKids[i].id;
                    profiles[profiles.Count - 1].GetComponent<KidProfile>().parentkey = sessionMng.temporalKids[i].userkey;
                    profiles[profiles.Count - 1].parent = kidsRef;
                    if (i < 4)
                        profiles[profiles.Count - 1].localPosition = positions[i];
                    else
                        profiles[profiles.Count - 1].localPosition = new Vector3(positions[i % 4].x, -1, positions[i % 4].z);

                    profiles[profiles.Count - 1].localRotation = kidsRef.localRotation;
                    profiles[profiles.Count - 1].Find("Text").GetComponent<TextWrapper>().Wrap(sessionMng.temporalKids[i].name);
                }
            }
            else
            {
                if (sessionMng.activeUser.kids.Count > 4)
                {
                    nextArrowRef.gameObject.SetActive(true);
                }
                else
                {
                    nextArrowRef.gameObject.SetActive(false);
                }
                if (sessionMng.activeUser.kids.Count >= 4)
                {
                    positions = new Vector3[4];
                    positions[0] = new Vector3(-0.27f, 0, 0);
                    positions[1] = new Vector3(-0.09f, 0, 0);
                    positions[2] = new Vector3(0.09f, 0, 0);
                    positions[3] = new Vector3(0.27f, 0, 0);
                }
                else
                {
                    positions = new Vector3[sessionMng.activeUser.kids.Count];
                    switch (sessionMng.activeUser.kids.Count)
                    {
                        case 1:
                            positions[0] = new Vector3(0, 0, 0);
                            break;
                        case 2:
                            positions[0] = new Vector3(-0.09f, 0, 0);
                            positions[1] = new Vector3(0.09f, 0, 0);
                            break;
                        case 3:
                            positions[0] = new Vector3(-0.18f, 0, 0);
                            positions[1] = new Vector3(0, 0, 0);
                            positions[2] = new Vector3(0.18f, 0, 0);
                            break;
                    }
                }
                for (int i = 0; i < profiles.Count; i++)
                {
                    Destroy(profiles[i].gameObject);
                }
                profiles.Clear();
                for (int i = 0; i < sessionMng.activeUser.kids.Count; i++)
                {
                    profiles.Add(Instantiate(profile) as Transform);
                    profiles[profiles.Count - 1].GetComponent<KidProfile>().id = sessionMng.activeUser.kids[i].id;
                    profiles[profiles.Count - 1].GetComponent<KidProfile>().parentkey = sessionMng.activeUser.userkey;
                    profiles[profiles.Count - 1].parent = kidsRef;
                    if (i < 4)
                        profiles[profiles.Count - 1].localPosition = positions[i];
                    else
                        profiles[profiles.Count - 1].localPosition = new Vector3(positions[i % 4].x, -1, positions[i % 4].z);
                    profiles[profiles.Count - 1].localRotation = kidsRef.localRotation;
                    profiles[profiles.Count - 1].Find("Text").GetComponent<TextWrapper>().Wrap(sessionMng.activeUser.kids[i].name);
                }
            }
        }
        for (int i = 0; i < profiles.Count; i++)
        {
            profiles[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            profiles[i].Find("Text").GetComponent<TextMesh>().color = new Color(1, 1, 1, 0);
			//profiles[i].FindChild("Text").GetComponent<TextMesh>().fontSize = 35;

        }
    }

    public void SetKid(string parentkey, int id)
    {
        sessionMng.SetKid(parentkey, id);
        currentState = Phase.Loading;
        //state = 2;
        fadeIn = true;
    }

    public void NextProfiles()
    {
        if (!changeProfiles)
        {
            prefArrowRef.gameObject.SetActive(true);
            profilePivot += 4;
            changeProfiles = true;
            forward = true;
            if (profilePivot + 4 >= sessionMng.temporalKids.Count)
            {
                nextArrowRef.gameObject.SetActive(false);
            }
        }
    }

    public void PrevProfiles()
    {
        if (profilePivot > 0 && !changeProfiles)
        {
            nextArrowRef.gameObject.SetActive(true);
            profilePivot -= 4;
            changeProfiles = true;
            forward = false;
            if (profilePivot == 0)
            {
                prefArrowRef.gameObject.SetActive(false);
            }
        }
    }

    public void IsActive()
    {
        SessionManager.User tempUser = sessionMng.GetUser(PlayerPrefs.GetString("activeUser", ""));
        if (tempUser != null)
        {
            StartCoroutine(PostIsActive(tempUser));
        }
        else
        {
            waitingForLogin = false;
            bgRef.color = new Color(0, 0, 0, 0);
            loaderRef.color = new Color(1, 1, 1, 0);
            menu.gameObject.SetActive(false);
            currentState = Phase.TrialOptions;
            DisplayLogin();
        }
    }

    IEnumerator PostIsActive(SessionManager.User user)
    {
        string post_url = activeUrl/* + "name=" + WWW.EscapeURL(name) + "&score=" + score + "&hash=" + hash*/;

        // Build form to post in server
        WWWForm form = new WWWForm();
        form.AddField("parentemail", user.username);

        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url, form);
        yield return hs_post; // Wait until the download is done

        if (hs_post.error == null)
        {
            waitingForLogin = false;
            bgRef.color = new Color(0, 0, 0, 0);
            loaderRef.color = new Color(1, 1, 1, 0);
            menu.gameObject.SetActive(true);
            JSONObject jsonObject = JSONObject.Parse(hs_post.text);
            //Debug.Log(jsonObject.GetValue("access"));

            if (jsonObject.GetValue("active").Str == "true")
            {
                /*if (sessionMng.LoadActiveUser())
                {
                    lang = sessionMng.activeUser.language;
                    ChangeLanguage();
                    CreateProfiles();
                }*/
            }
            else
            {
                menu.gameObject.SetActive(false);
                currentState = Phase.TrialOptions;
                DisplayLogin();
            }
        }
        else
        {
            waitingForLogin = false;
            bgRef.color = new Color(0, 0, 0, 0);
            loaderRef.color = new Color(1, 1, 1, 0);
            int resultS = sessionMng.TryLogin(user.username, user.psswdHash);
            if (resultS == 1)
            {
                /*if (sessionMng.LoadActiveUser())
                {
                    menu.gameObject.SetActive(true);
                    lang = sessionMng.activeUser.language;
                    ChangeLanguage();
                    CreateProfiles();
                }else
                {
                    menu.gameObject.SetActive(false);
                    currentState = Phase.TrialOptions;
                    DisplayLogin();
                }*/
            }
            else
            {
                menu.gameObject.SetActive(false);
                currentState = Phase.TrialOptions;
                DisplayLogin();
            }
        }
    }

    public void PostLogin()
    {
        StartCoroutine(PostLoginData());
    }

    IEnumerator PostLoginData()
    {
        //This connects to a server side php script that will add the name and score to a MySQL DB.
        // Supply it with a string representing the players name and the players score.
        string hash = Md5Sum(psswd);
        string post_url = loginUrl/* + "name=" + WWW.EscapeURL(name) + "&score=" + score + "&hash=" + hash*/;

        // Build form to post in server
        WWWForm form = new WWWForm();
        form.AddField("parentemail", username);
        form.AddField("parentpassword", hash);

        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url, form);
        yield return hs_post; // Wait until the download is done
        //Debug.Log (hs_post.text);
        if (hs_post.error == null)
        {
            waitingForLogin = false;
            loaderRef.color = new Color(1, 1, 1, 0);
            JSONObject jsonObject = JSONObject.Parse(hs_post.text);
            Debug.Log(jsonObject.GetValue("access"));
            if (jsonObject.GetValue("access").Str == "true"/* && (jsonObject.GetValue("active").Str == "true"
                                                             || jsonObject.GetValue("trial").Str == "true")*/)
            {
                JSONArray kids = jsonObject.GetValue("children").Array;
                bool childSuscription = false;
                bool childTrial = false;
                for (int i = 0; i < kids.Length; i++)
                {
                    JSONObject kidObj = kids[i].Obj;
                    bool tempA = bool.Parse(kidObj.GetValue("active").Str);
                    bool tempT = bool.Parse(kidObj.GetValue("trial").Str);
                    if (tempA && !tempT)
                    {
                        childSuscription = true;
                        break;
                    }
                    else if (tempT)
                    {
                        childTrial = true;
                    }
                }
                if (childSuscription/*jsonObject.GetValue("active").Str == "true" && jsonObject.GetValue("trial").Str == "false"*/)
                {
                    PlayerPrefs.SetInt("subscriptionPurchased", 1);
                    post_url = suscriptionUrl;
                    form = new WWWForm();
                    form.AddField("userkey", jsonObject.GetValue("key").Str);

                    hs_post = new WWW(post_url, form);
                    yield return hs_post; // Wait until the download is done
                    //Debug.Log (hs_post.text);
                    if (hs_post.error == null)
                    {
                        JSONObject jsonObjectSus = JSONObject.Parse(hs_post.text);
                        //JSONArray kids = jsonObject.GetValue("children").Array;
                        sessionMng.LoadUser(username, hash, jsonObject.GetValue("key").Str, null, (int)jsonObject.GetValue("id").Number);
                        sessionMng.AddKids(kids);
                        List<string> tempSyncKeys = new List<string>();
                        for (int k = 0; k < kids.Length; k++)
                        {
                            JSONObject kidObj = kids[k].Obj;
                            bool rep = false;
                            for (int t = 0; t < tempSyncKeys.Count; t++)
                            {
                                if (tempSyncKeys[t] == kidObj.GetValue("key").Str)
                                {
                                    rep = true;
                                }
                            }
                            if (!rep)
                            {
                                tempSyncKeys.Add(kidObj.GetValue("key").Str);
                            }
                        }
                        for (int t = 0; t < tempSyncKeys.Count; t++)
                        {
                            sessionMng.SyncProfiles(tempSyncKeys[t]);
                        }
                        sessionMng.activeUser.trialAccount = false;
                        if (jsonObjectSus.GetValue("code").Str == "200")
                        {
                            sessionMng.activeUser.suscriptionDate = DateTime.Parse(jsonObjectSus.GetValue("day_suscription").Str);
                        }
                        sessionMng.SaveSession();
                        CreateProfiles();
                        backArrow.color = new Color(1, 1, 1, 0);
                        textRef.color = new Color(1, 1, 1, 0);
                        prefArrowRef.color = new Color(1, 1, 1, 0);
                        nextArrowRef.color = new Color(1, 1, 1, 0);
                        alreadyLogged = true;
                        if (!avoidSwitch)
                        {
                            if (currentState == Phase.TrialOptions)
                            {
                                if (PlayerPrefs.GetInt("purchased", 0) == 1 || PlayerPrefs.GetInt("subscriptionPurchased", 0) == 1)
                                {
                                    currentState = Phase.Menu;
                                    menu.gameObject.SetActive(true);
                                }
                                HideBG();
                                showRegister = false;
                            }
                            else
                            {
                                fadeIn = true;
                                menu.gameObject.SetActive(false);
                            }
                        }
                        else
                        {
                            HideBG();
                        }
                        avoidSwitch = false;
                        showRegister = false;
                        showLogin = false;
                    }
                    else
                    {
                        Debug.Log(hs_post.error);
                    }
                }
                else if (childTrial)
                {
                    //JSONArray kids = jsonObject.GetValue("children").Array;
                    sessionMng.LoadUser(username, hash, jsonObject.GetValue("key").Str, null, (int)jsonObject.GetValue("id").Number);
                    sessionMng.AddKids(kids);
                    sessionMng.SyncProfiles(jsonObject.GetValue("key").Str);
                    sessionMng.activeUser.trialAccount = /*jsonObject.GetValue("trial").Str == "true"*/childTrial;
                    CreateProfiles();
                    backArrow.color = new Color(1, 1, 1, 0);
                    textRef.color = new Color(1, 1, 1, 0);
                    prefArrowRef.color = new Color(1, 1, 1, 0);
                    nextArrowRef.color = new Color(1, 1, 1, 0);
                    alreadyLogged = true;
                    if (!avoidSwitch)
                    {
                        if (currentState == Phase.TrialOptions)
                        {
                            if (PlayerPrefs.GetInt("purchased", 0) == 1 || PlayerPrefs.GetInt("subscriptionPurchased", 0) == 1)
                            {
                                currentState = Phase.Menu;
                                menu.gameObject.SetActive(true);
                            }
                            HideBG();
                            showRegister = false;
                        }
                        else
                        {
                            fadeIn = true;
                            menu.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        HideBG();
                    }
                    avoidSwitch = false;
                    showRegister = false;
                    showLogin = false;
                }
                else
                {
                    errorText = language.levelStrings[37];
                }
            }
            else
            {
                if (jsonObject.GetValue("userExists").Str == "false")
                    errorText = language.levelStrings[36];
                else
                    errorText = language.levelStrings[38];
            }
            Debug.Log(jsonObject.GetValue("key"));
        }
        else
        {
            waitingForLogin = false;
            loaderRef.color = new Color(1, 1, 1, 0);
            if (sessionMng.FindUser(username))
            {
                int resultS = sessionMng.TryLogin(username, hash);
                if (resultS == 1)
                {
                    alreadyLogged = true;
                    CreateProfiles();
                    if (!avoidSwitch)
                    {
                        if (currentState == Phase.TrialOptions)
                        {
                            if (PlayerPrefs.GetInt("purchased", 0) == 1 || PlayerPrefs.GetInt("subscriptionPurchased", 0) == 1)
                            {
                                currentState = Phase.Menu;
                                menu.gameObject.SetActive(true);
                            }
                            HideBG();
                            showRegister = false;
                        }
                        else
                        {
                            fadeIn = true;
                            menu.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        HideBG();
                    }
                    avoidSwitch = false;
                    showRegister = false;
                    showLogin = false;
                }
                else if (resultS == -1)
                {
                    errorText = "Hubo un problema de conexion, intenta con el último nombre y contraseña usado en este equipo";
                }
                else
                {
                    errorText = "Parece que esta cuenta no está activa, intenta iniciar sesión conectado a internet.";
                }
            }
            else
            {
                errorText = "Hubo un problema al intentar conectarse, revise su conexión a internet.";
            }
        }
    }

    public void DisplayLogin()
    {
        //showLogin=true;
        showRegister = true;
        registerStep = 0;
        bgRef.color = new Color(0, 0, 0, 1);
        regParentName = "";
        regParentSurname = "";
        regParentMail = "";
        regParentPsswd = "";
        regParentPsswdCheck = "";
        regKidName = "";
        regKidSurname = "";
        errorText = "";
    }

    public void DisplayProfiles()
    {
        fadeIn = true;
        menu.gameObject.SetActive(false);
    }

    public void DisplayOptions()
    {
        menu.Find("Main").gameObject.SetActive(false);
        menu.Find("Options").gameObject.SetActive(true);
    }

    public void DisplayCredits()
    {
        bgRef.color = new Color(0, 0, 0, 1);
        backArrow.color = new Color(1, 1, 1, 1);
        currentState = Login.Phase.About;
        HideMenu();
    }

    public void HideMenu()
    {
        menu.gameObject.SetActive(false);
    }

    public void ShowMenu()
    {
        menu.gameObject.SetActive(true);
    }

    public void ReturnToMenu()
    {
		pruebaBut.GetComponent<MenuController> ().isPrueba = false;
		playBut.GetComponent<MenuController> ().isPlay = false;
		avoidSwitch = false;
        fadeOut = true;
    }

    public void CloseApplication()
    {
        Application.Quit();
    }

    void OnGUI()
    {
        camRend.Render();
        GUI.Label(new Rect(Screen.width - 70 * mapScale, Screen.height - 25 * mapScale, 60 * mapScale, 20 * mapScale), version, versionStyle);
        /*if(GUI.Button (new Rect (10 * mapScale, Screen.height - 60 * mapScale, 100 * mapScale, 50 * mapScale), "Español", languageStyle))
        {
            lang="es";
            ChangeLanguage();
        }
        if(GUI.Button (new Rect (120 * mapScale, Screen.height - 60 * mapScale, 100 * mapScale, 50 * mapScale), "English", languageStyle))
        {
            lang="en";
            ChangeLanguage();
        }*/
        switch (currentState)
        {
            case Phase.Trial:
                GUI.DrawTexture(new Rect(Screen.width * 0.5f - 334f * mapScale, Screen.height * 0.2f, 668 * mapScale, 237 * mapScale), towiLogo);
                if (GUI.Button(new Rect(Screen.width * 0.5f - 200f * mapScale, Screen.height * 0.6f, 400 * mapScale, 150 * mapScale), "", trialButton))
                {
                    if (!switching)
                    {
                        switching = true;
                        trial.ActivateTrial();
                        //ParticleEmitter[] emitters = effects.GetComponentsInChildren<ParticleEmitter>();
                        /*for (int i = 0; i < emitters.Length; i++)
                        {
                            emitters[i].Emit();
                        }*/
                        switchTime = 2;
                    }
                }
                break;
            case Phase.TrialOptions:
                if (waitingForLogin)
                {
                    bgRef.color = new Color(0, 0, 0, 1);
                    loaderRef.color = new Color(1, 1, 1, 1);
                    remainingText.normal.textColor = new Color(0.71f, 0.71f, 0.71f, 1);
                    //GUI.Label(new Rect(Screen.width*0.5f-297*mapScale,Screen.height*0.1f+3*mapScale,600*mapScale,50*mapScale),"Comprando...",remainingText);
                    remainingText.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1);
                    GUI.DrawTexture(new Rect(Screen.width * 0.5f - 75f * mapScale, 270 * mapScale, 150 * mapScale, 138 * mapScale), towiLogo3);
                    GUI.Label(new Rect(Screen.width * 0.5f - 300 * mapScale, Screen.height * 0.75f, 600 * mapScale, 50 * mapScale), "Iniciando sesión", remainingText);
                }
                else
                {
                    if (showRegister)
                    {
                        if (showLogin)
                        {
                            GUI.skin.settings.cursorColor = new Color(0.5f, 0.5f, 0.5f);
                            //GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),overlay);
                            GUI.color = new Color(1, 1, 1, 1 - opacity);
                            Rect boxPosition = new Rect(Screen.width * 0.5f - 235 * mapScale, Screen.height * 0.5f - 130 * mapScale, 470 * mapScale, 363 * mapScale);
                            GUI.DrawTexture(boxPosition, blueColor);
                            GUI.Label(new Rect(boxPosition.x + 50 * mapScale, boxPosition.y + userLabelOffset * mapScale, 368 * mapScale, 20 * mapScale), "Email", labelStyle);
                            username = GUI.TextField(new Rect(boxPosition.x + 50 * mapScale, boxPosition.y + userInputOffset * mapScale, 368 * mapScale, 50 * mapScale), username, inputStyle);
                            GUI.Label(new Rect(boxPosition.x + 50 * mapScale, boxPosition.y + psswdLabelOffset * mapScale, 368 * mapScale, 20 * mapScale), language.levelStrings[12], labelStyle);
                            psswd = GUI.PasswordField(new Rect(boxPosition.x + 50 * mapScale, boxPosition.y + psswdInputOffset * mapScale, 368 * mapScale, 50 * mapScale), psswd, '*', inputStyle);
                            GUI.Label(new Rect(boxPosition.x + 200 * mapScale, boxPosition.y + 30 * mapScale + errorInputOffset * mapScale, 250 * mapScale, 50 * mapScale), errorText, errorStyle);

                            if (GUI.Button(new Rect(boxPosition.x + 200 * mapScale, boxPosition.y - 5 * mapScale + loginOffset * mapScale, 250 * mapScale, 33 * mapScale), "Olvidé mi contraseña", linkStyle))
                            {
                                Application.OpenURL("http://towi.com.mx/platform/forgot_password.php");
                            }

                            if (GUI.Button(new Rect(Screen.width * 0.5f + 205 * mapScale, Screen.height * 0.5f - 140 * mapScale, 40 * mapScale, 40 * mapScale), "", closeButton))
                            {
                                errorText = "";
                                showLogin = false;
                                //bgRef.color=new Color(0,0,0,0);
                            }
                            if (!fadeIn && GUI.Button(new Rect(boxPosition.x + 50 * mapScale, boxPosition.y + loginOffset * mapScale, 137 * mapScale, 61 * mapScale), "", loginStyle))
                            {
                                waitingForLogin = true;
                                PostLogin();
                                //sessionMng.LoadUser("Hector","a","a",null);
                                //CreateProfiles();
                                //fadeIn = true;
                                //fadeIn=true;
                            }
                        }
                        else
                        {
                            switch (registerStep)
                            {
                                case 0:
                                    GUI.BeginGroup(new Rect(Screen.width * 0.5f - 500 * mapScale, Screen.height * 0.5f - 250 * mapScale, 400 * mapScale, 500 * mapScale));
                                    GUI.DrawTexture(new Rect(0, 0, 400 * mapScale, 600 * mapScale), formInfoTexture);
                                    GUI.DrawTexture(new Rect(25 * mapScale, 250 * mapScale, 350 * mapScale, 235 * mapScale), platformPreview);
                                    GUI.Label(new Rect(20 * mapScale, 130 * mapScale, 360 * mapScale, 100 * mapScale), language.levelStrings[0], regInfoStyle);
                                    GUI.DrawTexture(new Rect(50 * mapScale, 20 * mapScale, 300 * mapScale, 106 * mapScale), towiLogo2);
                                    GUI.EndGroup();

                                    GUI.BeginGroup(new Rect(Screen.width * 0.5f - 100 * mapScale, Screen.height * 0.5f - 250 * mapScale, 600 * mapScale, 500 * mapScale));
                                    GUI.skin.settings.cursorColor = new Color(0.5f, 0.5f, 0.5f);
                                    //GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),overlay);
                                    //GUI.color=new Color(1,1,1,1-opacity);
                                    GUI.DrawTexture(new Rect(0, 0, 600 * mapScale, 500 * mapScale), loadingColor);
                                    //GUI.Box(new Rect(0,0,600*mapScale,600*mapScale),"");
                                    if (GUI.Button(new Rect(100 * mapScale, 100 * mapScale, 400 * mapScale, 80 * mapScale), language.levelStrings[13], nextStyle))
                                    {
                                        bgRef.color = new Color(0, 0, 0, 1);
                                        showLogin = true;
                                    }
                                    if (showRegisterButton)
                                    {
                                        if (GUI.Button(new Rect(100 * mapScale, 200 * mapScale, 400 * mapScale, 80 * mapScale), language.levelStrings[14], nextStyle))
                                        {
                                            registerStep = 1;
                                        }
                                    }
                                    if (GUI.Button(new Rect(100 * mapScale, 400 * mapScale, 400 * mapScale, 80 * mapScale), language.levelStrings[15], skipStyle))
                                    {
                                        showRegister = false;
                                    }
                                    GUI.EndGroup();
                                    break;
                                case 1:
                                    //bgRef.color=new Color(0,0,0,1);
                                    GUI.BeginGroup(new Rect(Screen.width * 0.5f - 500 * mapScale, Screen.height * 0.5f - 250 * mapScale, 400 * mapScale, 500 * mapScale));
                                    GUI.DrawTexture(new Rect(0, 0, 400 * mapScale, 600 * mapScale), formInfoTexture);
                                    GUI.DrawTexture(new Rect(25 * mapScale, 250 * mapScale, 350 * mapScale, 235 * mapScale), platformPreview);
                                    GUI.Label(new Rect(20 * mapScale, 130 * mapScale, 360 * mapScale, 100 * mapScale), language.levelStrings[0], regInfoStyle);
                                    GUI.DrawTexture(new Rect(50 * mapScale, 20 * mapScale, 300 * mapScale, 106 * mapScale), towiLogo2);
                                    GUI.EndGroup();

                                    GUI.BeginGroup(new Rect(Screen.width * 0.5f - 100 * mapScale, Screen.height * 0.5f - 250 * mapScale, 600 * mapScale, 500 * mapScale));
                                    GUI.skin.settings.cursorColor = new Color(0.5f, 0.5f, 0.5f);
                                    //GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),overlay);
                                    //GUI.color=new Color(1,1,1,1-opacity);
                                    GUI.DrawTexture(new Rect(0, 0, 600 * mapScale, 500 * mapScale), loadingColor);
                                    //GUI.Box(new Rect(0,0,600*mapScale,600*mapScale),"");
                                    GUI.Label(new Rect(50 * mapScale, 20 * mapScale, 200 * mapScale, 40 * mapScale), language.levelStrings[16], inputsNamesStyle);
                                    GUI.Label(new Rect(310 * mapScale, 20 * mapScale, 300 * mapScale, 40 * mapScale), language.levelStrings[17], inputsNamesStyle);
                                    regParentName = GUI.TextField(new Rect(50 * mapScale, 50 * mapScale, 240 * mapScale, 40 * mapScale), regParentName, inputStyle);
                                    regParentSurname = GUI.TextField(new Rect(310 * mapScale, 50 * mapScale, 240 * mapScale, 40 * mapScale), regParentSurname, inputStyle);
                                    GUI.Label(new Rect(50 * mapScale, 110 * mapScale, 160 * mapScale, 40 * mapScale), "Email", inputsNamesStyle);
                                    regParentMail = GUI.TextField(new Rect(50 * mapScale, 140 * mapScale, 260 * mapScale, 40 * mapScale), regParentMail, inputStyle);
                                    GUI.Label(new Rect(50 * mapScale, 200 * mapScale, 160 * mapScale, 40 * mapScale), language.levelStrings[12], inputsNamesStyle);
                                    regParentPsswd = GUI.PasswordField(new Rect(50 * mapScale, 230 * mapScale, 260 * mapScale, 40 * mapScale), regParentPsswd, '*', inputStyle);
                                    GUI.Label(new Rect(50 * mapScale, 290 * mapScale, 160 * mapScale, 40 * mapScale), language.levelStrings[18], inputsNamesStyle);
                                    regParentPsswdCheck = GUI.PasswordField(new Rect(50 * mapScale, 320 * mapScale, 260 * mapScale, 40 * mapScale), regParentPsswdCheck, '*', inputStyle);
                                    GUI.Label(new Rect(50 * mapScale, 365 * mapScale, 400 * mapScale, 40 * mapScale), errorText, errorStyle2);
                                    if (GUI.Button(new Rect(50 * mapScale, 410 * mapScale, 200 * mapScale, 70 * mapScale), language.levelStrings[2], skipStyle))
                                    {
                                        registerStep = 0;
                                        errorText = "";
                                    }
                                    if (GUI.Button(new Rect(350 * mapScale, 410 * mapScale, 200 * mapScale, 70 * mapScale), language.levelStrings[19], nextStyle))
                                    {
                                        if (regParentName.Trim() != "" && regParentSurname.Trim() != "" && regParentMail.Trim() != "" && regParentPsswd.Trim() != "" && regParentPsswdCheck.Trim() != "")
                                        {
                                            errorText = "";
                                            Regex regex = new Regex(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                                                    + "@"
                                                                    + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$");
                                            Match match = regex.Match(regParentMail);
                                            if (match.Success)
                                            {
                                                errorText = "";
                                                if (regParentPsswd.Length < 8)
                                                {
                                                    errorText = language.levelStrings[20];
                                                }
                                                else
                                                {
                                                    if (regParentPsswd == regParentPsswdCheck)
                                                    {
                                                        registerStep = 2;
                                                        //trial.registerParent(regParentName.Trim(),regParentSurname.Trim(),regParentMail.Trim(),regParentPsswd.Trim());
                                                    }
                                                    else
                                                    {
                                                        errorText = language.levelStrings[21];
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                errorText = language.levelStrings[22];
                                            }
                                        }
                                        else
                                        {
                                            errorText = language.levelStrings[23];
                                        }
                                    }
                                    GUI.EndGroup();
                                    break;
                                case 2:
                                    GUI.BeginGroup(new Rect(Screen.width * 0.5f - 500 * mapScale, Screen.height * 0.5f - 250 * mapScale, 400 * mapScale, 500 * mapScale));
                                    GUI.DrawTexture(new Rect(0, 0, 400 * mapScale, 600 * mapScale), formInfoTexture);
                                    GUI.DrawTexture(new Rect(25 * mapScale, 250 * mapScale, 350 * mapScale, 235 * mapScale), platformPreview);
                                    GUI.Label(new Rect(20 * mapScale, 130 * mapScale, 360 * mapScale, 100 * mapScale), language.levelStrings[24], regInfoStyle);
                                    GUI.DrawTexture(new Rect(50 * mapScale, 20 * mapScale, 300 * mapScale, 106 * mapScale), towiLogo2);
                                    GUI.EndGroup();

                                    GUI.BeginGroup(new Rect(Screen.width * 0.5f - 100 * mapScale, Screen.height * 0.5f - 250 * mapScale, 600 * mapScale, 500 * mapScale));
                                    GUI.skin.settings.cursorColor = new Color(0.5f, 0.5f, 0.5f);
                                    //GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),overlay);
                                    //GUI.color=new Color(1,1,1,1-opacity);
                                    GUI.DrawTexture(new Rect(0, 0, 600 * mapScale, 500 * mapScale), loadingColor);
                                    //GUI.Box(new Rect(0,0,600*mapScale,600*mapScale),"");
                                    GUI.Label(new Rect(50 * mapScale, 20 * mapScale, 200 * mapScale, 40 * mapScale), language.levelStrings[16], inputsNamesStyle);
                                    GUI.Label(new Rect(50 * mapScale, 110 * mapScale, 160 * mapScale, 40 * mapScale), language.levelStrings[17], inputsNamesStyle);
                                    regKidName = GUI.TextField(new Rect(50 * mapScale, 50 * mapScale, 240 * mapScale, 40 * mapScale), regKidName, inputStyle);
                                    regKidSurname = GUI.TextField(new Rect(50 * mapScale, 140 * mapScale, 260 * mapScale, 40 * mapScale), regKidSurname, inputStyle);
                                    GUI.Label(new Rect(50 * mapScale, 300 * mapScale, 420 * mapScale, 40 * mapScale), errorText, errorStyle2);
                                    errorStyle2.normal.textColor = new Color(0.2f, 0.2f, 0.2f, 1);
                                    GUI.Label(new Rect(50 * mapScale, 180 * mapScale, 400 * mapScale, 40 * mapScale), language.levelStrings[25], errorStyle2);
                                    Rect labelRect = new Rect(50 * mapScale, 260 * mapScale, 270 * mapScale, 40 * mapScale);
                                    errorStyle2.normal.textColor = new Color(0.26f, 0.26f, 1, 1);
                                    if (GUI.Button(labelRect, language.levelStrings[26], errorStyle2))
                                    {
                                        Application.OpenURL("http://www.towi.com.mx/privacy.php");
                                    }
                                    //if (Event.current.type == EventType.MouseUp && labelRect.Contains(Event.current.mousePosition))
                                    //	Application.OpenURL("http://www.towi.com.mx/privacy.php");

                                    //GUI.Label(labelRect, "Terminos y Condiciones",errorStyle2);
                                    errorStyle2.normal.textColor = new Color(1, 0.26f, 0.26f, 1);
                                    if (GUI.Button(new Rect(50 * mapScale, 410 * mapScale, 200 * mapScale, 70 * mapScale), language.levelStrings[2], skipStyle))
                                    {
                                        registerStep = 0;
                                        errorText = "";
                                    }
                                    if (GUI.Button(new Rect(350 * mapScale, 410 * mapScale, 200 * mapScale, 70 * mapScale), language.levelStrings[27], nextStyle))
                                    {
                                        if (regKidName.Trim() != "" && regKidSurname.Trim() != "")
                                        {
                                            errorText = "";
                                            trial.registerParentAndKid(regParentName.Trim(), regParentSurname.Trim(), regParentMail.Trim(), regParentPsswd.Trim(), regKidName.Trim(), regKidSurname.Trim());
                                        }
                                        else
                                        {
                                            errorText = language.levelStrings[23];
                                        }
                                    }
                                    GUI.EndGroup();
                                    break;
                            }
                            if (GUI.Button(new Rect(Screen.width * 0.5f + 468 * mapScale, Screen.height * 0.5f - 260 * mapScale, 40 * mapScale, 40 * mapScale), "", closeButton))
                            {
                                showRegister = false;
                                registerStep = 0;
                                HideBG();
                            }
                        }
                        if (GUI.Button(new Rect(0, 0, 172 * mapScale, 68 * mapScale), "", exitButton))
                        {
                            CloseApplication();
                        }
                    }
                    else
                    {
                        if (showEnterKey)
                        {
                            GUI.BeginGroup(new Rect(Screen.width * 0.5f - 500 * mapScale, Screen.height * 0.5f - 250 * mapScale, 400 * mapScale, 500 * mapScale));
                            GUI.DrawTexture(new Rect(0, 0, 400 * mapScale, 600 * mapScale), formInfoTexture);
                            GUI.DrawTexture(new Rect(25 * mapScale, 250 * mapScale, 350 * mapScale, 235 * mapScale), platformKey);
                            regInfoStyle.alignment = TextAnchor.UpperCenter;
                            GUI.Label(new Rect(0, 200 * mapScale, 400 * mapScale, 100 * mapScale), language.levelStrings[1], regInfoStyle);
                            regInfoStyle.alignment = TextAnchor.UpperLeft;
                            GUI.DrawTexture(new Rect(50 * mapScale, 20 * mapScale, 300 * mapScale, 106 * mapScale), towiLogo2);
                            GUI.EndGroup();

                            GUI.BeginGroup(new Rect(Screen.width * 0.5f - 100 * mapScale, Screen.height * 0.5f - 250 * mapScale, 600 * mapScale, 500 * mapScale));
                            GUI.skin.settings.cursorColor = new Color(0.5f, 0.5f, 0.5f);
                            GUI.DrawTexture(new Rect(0, 0, 600 * mapScale, 500 * mapScale), loadingColor);
                            GUI.Label(new Rect(50 * mapScale, 170 * mapScale, 200 * mapScale, 40 * mapScale), "Serial", inputsNamesStyle);
                            keySerial = GUI.TextField(new Rect(50 * mapScale, 200 * mapScale, 500 * mapScale, 40 * mapScale), keySerial, inputStyle);
                            GUI.Label(new Rect(50 * mapScale, 300 * mapScale, 500 * mapScale, 40 * mapScale), errorText, errorStyle2);
                            if (GUI.Button(new Rect(50 * mapScale, 410 * mapScale, 200 * mapScale, 70 * mapScale), language.levelStrings[2], skipStyle))
                            {
                                showEnterKey = false;
                                errorText = "";
                            }
                            if (GUI.Button(new Rect(350 * mapScale, 410 * mapScale, 200 * mapScale, 70 * mapScale), language.levelStrings[3], nextStyle) && keySerial.Trim() != "")
                            {
                                buyController.TryActivate(keySerial.Trim());
                            }
                            if (GUI.Button(new Rect(50 * mapScale, 270 * mapScale, 480 * mapScale, 40 * mapScale), language.levelStrings[4], linkStyle))
                            {
                                Application.OpenURL("http://www.towi.com.mx/platform/buy.php");
                            }
                            GUI.EndGroup();
                        }
                        else
                        {
                            if (waitingForPurchase)
                            {
                                bgRef.color = new Color(0, 0, 0, 1);
                                loaderRef.color = new Color(1, 1, 1, 1);
                                remainingText.normal.textColor = new Color(0.71f, 0.71f, 0.71f, 1);
                                //GUI.Label(new Rect(Screen.width*0.5f-297*mapScale,Screen.height*0.1f+3*mapScale,600*mapScale,50*mapScale),"Comprando...",remainingText);
                                remainingText.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1);
                                GUI.Label(new Rect(Screen.width * 0.5f - 300 * mapScale, Screen.height * 0.75f, 600 * mapScale, 50 * mapScale), language.levelStrings[5], remainingText);
                            }
                            else
                            {//270 Pantalla de compra o suscripcion con dias de prueba faltantes
                                HideBG();
                                loaderRef.color = new Color(1, 1, 1, 0);
                                GUI.DrawTexture(new Rect(Screen.width * 0.5f - 520 * mapScale, Screen.height * 0.5f - 200 * mapScale, 500 * mapScale, 412 * mapScale), blueBg);
                                GUI.Label(new Rect(Screen.width * 0.5f - 470 * mapScale, Screen.height * 0.5f - 80 * mapScale, 440 * mapScale, 412 * mapScale), language.levelStrings[6], optionInfo);
								remainingText.normal.textColor = new Color(0.71f, 0.71f, 0.71f, 1);
								GUI.Label(new Rect(Screen.width * 0.5f - 297 * mapScale, Screen.height * 0.1f + 3 * mapScale, 600 * mapScale, 50 * mapScale), language.levelStrings[7] + " <color=#93c255><b>" + Math.Max(0, trial.remainingDays) + "</b></color> " + language.levelStrings[8], remainingText);
								remainingText.normal.textColor = new Color(1, 1, 1, 1);
								GUI.Label(new Rect(Screen.width * 0.5f - 300 * mapScale, Screen.height * 0.1f, 600 * mapScale, 50 * mapScale), language.levelStrings[7] + " <color=#c1ff70><b>" + Math.Max(0, trial.remainingDays) + "</b></color> " + language.levelStrings[8], remainingText);
								GUI.Label(new Rect(Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 180 * mapScale, 100 * mapScale, 50 * mapScale), language.levelStrings[9], optionTitle);
								
								//Se quita la opcion de suscribirse en iOS ya que no permiten que en la app se suscriba directamente.
								if(Application.platform != RuntimePlatform.IPhonePlayer){
									GUI.DrawTexture(new Rect(Screen.width * 0.5f + 20 * mapScale, Screen.height * 0.5f - 200 * mapScale, 500 * mapScale, 412 * mapScale), greenBg);
									GUI.Label(new Rect(Screen.width * 0.5f + 50 * mapScale, Screen.height * 0.5f - 100 * mapScale, 440 * mapScale, 412 * mapScale), "-Test Towi de cognición\n-Acceso ilimitado a Towi Island\n-Acceso ilimitado a Towi Platform\n-Multiusuario\n-Multidispositivo\n-Almacenamiento de métricas", optionInfo);
									GUI.Label(new Rect(Screen.width * 0.5f + 150 * mapScale, Screen.height * 0.5f - 180 * mapScale, 100 * mapScale, 50 * mapScale), "SUSCRIPCIÓN", optionTitle);
								}

                                if (GUI.Button(new Rect(Screen.width * 0.5f - 388 * mapScale, Screen.height * 0.5f + 70 * mapScale, 233 * mapScale, 90 * mapScale), "", buyButton))
                                {
                                    if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                                    {
                                        if (!waitingForPurchase)
                                        {
                                            waitingForPurchase = true;
                                        }
                                    }
                                    else
                                    {

                                        //currentState=Phase.Menu;
                                        //menu.gameObject.SetActive (true);
                                        showEnterKey = true;
                                    }
                                }

							//Si es iOS no desplegar el boton de suscribirse.
								if (Application.platform != RuntimePlatform.IPhonePlayer && GUI.Button(new Rect(Screen.width * 0.5f + 155 * mapScale, Screen.height * 0.5f + 70 * mapScale, 233 * mapScale, 90 * mapScale), "", suscriptionButton))
                                {
                                    if (!waitingForPurchase)
                                    {
                                        Application.OpenURL("http://www.towi.com.mx/platform/login.php");
                                        //string tempKey = "";
                                        //if (sessionMng.activeUser != null)
                                        //    tempKey = sessionMng.activeUser.userkey;
                                        //string urlKey = tempKey != "" ? "?key=" + tempKey : tempKey;
                                        //Application.OpenURL(trial.controller.subscribeURL + urlKey);
                                        //Application.OpenURL(trial.controller.subscribeURL);
                                        DisplayLogin();
                                    }
                                }
                                if (trial.remainingDays > 0)
                                {
                                    if (GUI.Button(new Rect(Screen.width * 0.5f + 220 * mapScale, Screen.height * 0.5f + 240 * mapScale, 287 * mapScale, 105 * mapScale), "", gameButton))
                                    {
                                        if (!waitingForPurchase)
                                        {
                                            currentState = Phase.Menu;
                                            menu.gameObject.SetActive(true);
                                        }

                                    }
                                }
                            }
                        }
                        if (GUI.Button(new Rect(0, 0, 172 * mapScale, 68 * mapScale), "Volver", skipStyle))
                        {
                            bgRef.color = new Color(0, 0, 0, 1);
                            showRegister = true;
                        }
                    }
                }
                break;
            case Phase.Login:
                //				GUI.color=new Color(1,1,1,1-opacity);
                //				Rect boxPosition = new Rect (Screen.width * 0.5f - 235*mapScale, Screen.height * 0.5f - 130*mapScale, 470*mapScale, 363*mapScale);
                //				GUI.DrawTexture (new Rect (Screen.width * 0.5f - 200 * mapScale, boxPosition.y - 220 * mapScale, 400 * mapScale, 203 * mapScale), kiwiLogo);
                //				GUI.DrawTexture (boxPosition, loginBox);
                //				GUI.Label (new Rect (boxPosition.x + 50 * mapScale, boxPosition.y + userLabelOffset * mapScale, 368 * mapScale, 20 * mapScale), "Email", labelStyle);
                //				username=GUI.TextField (new Rect (boxPosition.x+50*mapScale,boxPosition.y+userInputOffset*mapScale, 368*mapScale, 50*mapScale), username,inputStyle);
                //				GUI.Label (new Rect (boxPosition.x + 50 * mapScale, boxPosition.y + psswdLabelOffset * mapScale, 368 * mapScale, 20 * mapScale), "Contraseña", labelStyle);
                //				psswd=GUI.PasswordField (new Rect (boxPosition.x+50*mapScale,boxPosition.y+psswdInputOffset*mapScale, 368*mapScale, 50*mapScale), psswd,'*',inputStyle);
                //				GUI.Label (new Rect (boxPosition.x + 200 * mapScale, boxPosition.y + errorInputOffset * mapScale, 250 * mapScale, 50 * mapScale), errorText, errorStyle);
                //				if (!fadeIn&&GUI.Button (new Rect (boxPosition.x + 50 * mapScale, boxPosition.y + loginOffset * mapScale, 137 * mapScale, 61 * mapScale), "", loginStyle)) {
                //					//PostLogin();
                //					sessionMng.LoadUser("Hector","a","a",null);
                //					CreateProfiles();
                //					fadeIn = true;
                //					//fadeIn=true;
                //				}
                break;
            case Phase.Loading:
                if (fadeIn)
                {
                    GUI.color = new Color(1, 1, 1, opacity);
                }
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), loadingColor);
                GUI.DrawTexture(new Rect(Screen.width / 2 - Screen.height / 2, 0, Screen.height, Screen.height), loadingScreen);
                break;
            case Phase.Menu:
                if (waitingForLogin)
                {
                    bgRef.color = new Color(0, 0, 0, 1);
                    loaderRef.color = new Color(1, 1, 1, 1);
                    remainingText.normal.textColor = new Color(0.71f, 0.71f, 0.71f, 1);
                    //GUI.Label(new Rect(Screen.width*0.5f-297*mapScale,Screen.height*0.1f+3*mapScale,600*mapScale,50*mapScale),"Comprando...",remainingText);
                    remainingText.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1);
                    GUI.DrawTexture(new Rect(Screen.width * 0.5f - 75f * mapScale, 270 * mapScale, 150 * mapScale, 138 * mapScale), towiLogo3);
                    GUI.Label(new Rect(Screen.width * 0.5f - 300 * mapScale, Screen.height * 0.75f, 600 * mapScale, 50 * mapScale), "Iniciando sesión", remainingText);
                }
                else
                {
                    if (!local)
                    {
                        GUI.DrawTexture(new Rect(Screen.width - 130 * mapScale, 30 * mapScale, 100 * mapScale, 100 * mapScale), profilePic);
                        if (sessionMng.activeUser != null)
                        {
                            GUI.Label(new Rect(Screen.width - 250 * mapScale, 35 * mapScale, 100 * mapScale, 10 * mapScale), sessionMng.activeUser.username, nameTextStyle);
                            if (GUI.Button(new Rect(Screen.width - 250 * mapScale, 100 * mapScale, 100 * mapScale, 20 * mapScale), language.levelStrings[10], logTextStyle))
                            {
                                alreadyLogged = false;
                                sessionMng.Logout();
                            }
                        }
                        else
                        {
                            GUI.Label(new Rect(Screen.width - 250 * mapScale, 35 * mapScale, 100 * mapScale, 10 * mapScale), "?", nameTextStyle);
                            if (GUI.Button(new Rect(Screen.width - 250 * mapScale, 100 * mapScale, 100 * mapScale, 20 * mapScale), language.levelStrings[11], logTextStyle))
                            {
                                avoidSwitch = true;
                                DisplayLogin();
                            }
                        }
                    }
                    //GUI.Label(new Rect(Screen.width-260*mapScale,70*mapScale,100,20),"Manuel",surnameTextStyle);

                    if (!fadeIn)
                    {
                        /*
                        GUI.skin.settings.cursorColor=new Color(0.5f,0.5f,0.5f);
                        //GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),overlay);
                        GUI.color=new Color(1,1,1,1-opacity);
                        Rect boxPosition = new Rect (Screen.width * 0.5f - 235*mapScale, Screen.height * 0.5f - 130*mapScale, 470*mapScale, 363*mapScale);
                        GUI.DrawTexture (boxPosition, blueColor);
                        GUI.Label (new Rect (boxPosition.x + 50 * mapScale, boxPosition.y + userLabelOffset * mapScale, 368 * mapScale, 20 * mapScale), "Email", labelStyle);
                        username=GUI.TextField (new Rect (boxPosition.x+50*mapScale,boxPosition.y+userInputOffset*mapScale, 368*mapScale, 50*mapScale), username,inputStyle);
                        GUI.Label (new Rect (boxPosition.x + 50 * mapScale, boxPosition.y + psswdLabelOffset * mapScale, 368 * mapScale, 20 * mapScale), "Contraseña", labelStyle);
                        psswd=GUI.PasswordField (new Rect (boxPosition.x+50*mapScale,boxPosition.y+psswdInputOffset*mapScale, 368*mapScale, 50*mapScale), psswd,'*',inputStyle);
                        GUI.Label (new Rect (boxPosition.x + 200 * mapScale, boxPosition.y + errorInputOffset * mapScale, 250 * mapScale, 50 * mapScale), errorText, errorStyle);
				
                        if(GUI.Button(new Rect(Screen.width*0.5f+205*mapScale,Screen.height*0.5f-140*mapScale,40*mapScale,40*mapScale),"",closeButton))
                        {
                            showLogin=false;
                            bgRef.color=new Color(0,0,0,0);
                        }
                        if (!fadeIn&&GUI.Button (new Rect (boxPosition.x + 50 * mapScale, boxPosition.y + loginOffset * mapScale, 137 * mapScale, 61 * mapScale), "", loginStyle)) {
                            PostLogin();
                            //sessionMng.LoadUser("Hector","a","a",null);
                            //CreateProfiles();
                            //fadeIn = true;
                            //fadeIn=true;
                        }*/

                        if (showRegister)
                        {
                            if (showLogin)
                            {
                                GUI.skin.settings.cursorColor = new Color(0.5f, 0.5f, 0.5f);
                                //GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),overlay);
                                GUI.color = new Color(1, 1, 1, 1 - opacity);
                                Rect boxPosition = new Rect(Screen.width * 0.5f - 235 * mapScale, Screen.height * 0.5f - 130 * mapScale, 470 * mapScale, 363 * mapScale);
                                GUI.DrawTexture(boxPosition, blueColor);
                                GUI.Label(new Rect(boxPosition.x + 50 * mapScale, boxPosition.y + userLabelOffset * mapScale, 368 * mapScale, 20 * mapScale), "Email", labelStyle);
                                username = GUI.TextField(new Rect(boxPosition.x + 50 * mapScale, boxPosition.y + userInputOffset * mapScale, 368 * mapScale, 50 * mapScale), username, inputStyle);
                                GUI.Label(new Rect(boxPosition.x + 50 * mapScale, boxPosition.y + psswdLabelOffset * mapScale, 368 * mapScale, 20 * mapScale), language.levelStrings[12], labelStyle);
                                psswd = GUI.PasswordField(new Rect(boxPosition.x + 50 * mapScale, boxPosition.y + psswdInputOffset * mapScale, 368 * mapScale, 50 * mapScale), psswd, '*', inputStyle);
                                //GUI.Label(new Rect(boxPosition.x + 200 * mapScale, boxPosition.y + errorInputOffset * mapScale, 250 * mapScale, 50 * mapScale), errorText, errorStyle);

                                GUI.Label(new Rect(boxPosition.x + 200 * mapScale, boxPosition.y + 30 * mapScale + errorInputOffset * mapScale, 250 * mapScale, 50 * mapScale), errorText, errorStyle);

                                if (GUI.Button(new Rect(boxPosition.x + 200 * mapScale, boxPosition.y - 5 * mapScale + loginOffset * mapScale, 250 * mapScale, 33 * mapScale), "Olvidé mi contraseña", linkStyle))
                                {
                                    Application.OpenURL("http://towi.com.mx/platform/forgot_password.php");
                                }

                                if (GUI.Button(new Rect(Screen.width * 0.5f + 205 * mapScale, Screen.height * 0.5f - 140 * mapScale, 40 * mapScale, 40 * mapScale), "", closeButton))
                                {
                                    errorText = "";
                                    showLogin = false;
                                    //bgRef.color=new Color(0,0,0,0);
                                }
                                if (!fadeIn && GUI.Button(new Rect(boxPosition.x + 50 * mapScale, boxPosition.y + loginOffset * mapScale, 137 * mapScale, 61 * mapScale), "", loginStyle))
                                {
                                    waitingForLogin = true;
                                    PostLogin();
                                    //sessionMng.LoadUser("Hector","a","a",null);
                                    //CreateProfiles();
                                    //fadeIn = true;
                                    //fadeIn=true;
                                }
                            }
                            else
                            {
                                switch (registerStep)
                                {
                                    case 0:
                                        GUI.BeginGroup(new Rect(Screen.width * 0.5f - 500 * mapScale, Screen.height * 0.5f - 250 * mapScale, 400 * mapScale, 500 * mapScale));
                                        GUI.DrawTexture(new Rect(0, 0, 400 * mapScale, 600 * mapScale), formInfoTexture);
                                        GUI.DrawTexture(new Rect(25 * mapScale, 250 * mapScale, 350 * mapScale, 235 * mapScale), platformPreview);
                                        GUI.Label(new Rect(20 * mapScale, 130 * mapScale, 360 * mapScale, 100 * mapScale), language.levelStrings[0], regInfoStyle);
                                        GUI.DrawTexture(new Rect(50 * mapScale, 20 * mapScale, 300 * mapScale, 106 * mapScale), towiLogo2);
                                        GUI.EndGroup();

                                        GUI.BeginGroup(new Rect(Screen.width * 0.5f - 100 * mapScale, Screen.height * 0.5f - 250 * mapScale, 600 * mapScale, 500 * mapScale));
                                        GUI.skin.settings.cursorColor = new Color(0.5f, 0.5f, 0.5f);
                                        //GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),overlay);
                                        //GUI.color=new Color(1,1,1,1-opacity);
                                        GUI.DrawTexture(new Rect(0, 0, 600 * mapScale, 500 * mapScale), loadingColor);
                                        //GUI.Box(new Rect(0,0,600*mapScale,600*mapScale),"");
                                        if (GUI.Button(new Rect(100 * mapScale, 100 * mapScale, 400 * mapScale, 80 * mapScale), language.levelStrings[13], nextStyle))
                                        {
                                            bgRef.color = new Color(0, 0, 0, 1);
                                            showLogin = true;
                                        }
                                        if (showRegisterButton)
                                        {
                                            if (GUI.Button(new Rect(100 * mapScale, 200 * mapScale, 400 * mapScale, 80 * mapScale), language.levelStrings[14], nextStyle))
                                            {
                                                registerStep = 1;
                                            }
                                        }
                                        if (GUI.Button(new Rect(100 * mapScale, 400 * mapScale, 400 * mapScale, 80 * mapScale), language.levelStrings[15], skipStyle))
                                        {
                                            local = true;
                                            sessionMng.LoadLocal();
                                            currentState = Login.Phase.Loading;
                                            fadeIn = true;
                                        }
                                        GUI.EndGroup();
                                        break;
                                    case 1:
                                        //bgRef.color=new Color(0,0,0,1);
                                        GUI.BeginGroup(new Rect(Screen.width * 0.5f - 500 * mapScale, Screen.height * 0.5f - 250 * mapScale, 400 * mapScale, 500 * mapScale));
                                        GUI.DrawTexture(new Rect(0, 0, 400 * mapScale, 600 * mapScale), formInfoTexture);
                                        GUI.DrawTexture(new Rect(25 * mapScale, 250 * mapScale, 350 * mapScale, 235 * mapScale), platformPreview);
                                        GUI.Label(new Rect(20 * mapScale, 130 * mapScale, 360 * mapScale, 100 * mapScale), language.levelStrings[0], regInfoStyle);
                                        GUI.DrawTexture(new Rect(50 * mapScale, 20 * mapScale, 300 * mapScale, 106 * mapScale), towiLogo2);
                                        GUI.EndGroup();

                                        GUI.BeginGroup(new Rect(Screen.width * 0.5f - 100 * mapScale, Screen.height * 0.5f - 250 * mapScale, 600 * mapScale, 500 * mapScale));
                                        GUI.skin.settings.cursorColor = new Color(0.5f, 0.5f, 0.5f);
                                        //GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),overlay);
                                        //GUI.color=new Color(1,1,1,1-opacity);
                                        GUI.DrawTexture(new Rect(0, 0, 600 * mapScale, 500 * mapScale), loadingColor);
                                        //GUI.Box(new Rect(0,0,600*mapScale,600*mapScale),"");
                                        GUI.Label(new Rect(50 * mapScale, 20 * mapScale, 200 * mapScale, 40 * mapScale), language.levelStrings[16], inputsNamesStyle);
                                        GUI.Label(new Rect(310 * mapScale, 20 * mapScale, 300 * mapScale, 40 * mapScale), language.levelStrings[17], inputsNamesStyle);
                                        regParentName = GUI.TextField(new Rect(50 * mapScale, 50 * mapScale, 240 * mapScale, 40 * mapScale), regParentName, inputStyle);
                                        regParentSurname = GUI.TextField(new Rect(310 * mapScale, 50 * mapScale, 240 * mapScale, 40 * mapScale), regParentSurname, inputStyle);
                                        GUI.Label(new Rect(50 * mapScale, 110 * mapScale, 160 * mapScale, 40 * mapScale), "Email", inputsNamesStyle);
                                        regParentMail = GUI.TextField(new Rect(50 * mapScale, 140 * mapScale, 260 * mapScale, 40 * mapScale), regParentMail, inputStyle);
                                        GUI.Label(new Rect(50 * mapScale, 200 * mapScale, 160 * mapScale, 40 * mapScale), language.levelStrings[12], inputsNamesStyle);
                                        regParentPsswd = GUI.PasswordField(new Rect(50 * mapScale, 230 * mapScale, 260 * mapScale, 40 * mapScale), regParentPsswd, '*', inputStyle);
                                        GUI.Label(new Rect(50 * mapScale, 290 * mapScale, 160 * mapScale, 40 * mapScale), language.levelStrings[18], inputsNamesStyle);
                                        regParentPsswdCheck = GUI.PasswordField(new Rect(50 * mapScale, 320 * mapScale, 260 * mapScale, 40 * mapScale), regParentPsswdCheck, '*', inputStyle);
                                        GUI.Label(new Rect(50 * mapScale, 365 * mapScale, 400 * mapScale, 40 * mapScale), errorText, errorStyle2);
                                        if (GUI.Button(new Rect(50 * mapScale, 410 * mapScale, 200 * mapScale, 70 * mapScale), language.levelStrings[2], skipStyle))
                                        {
                                            registerStep = 0;
                                            errorText = "";
                                        }
                                        if (GUI.Button(new Rect(350 * mapScale, 410 * mapScale, 200 * mapScale, 70 * mapScale), language.levelStrings[19], nextStyle))
                                        {
                                            if (regParentName.Trim() != "" && regParentSurname.Trim() != "" && regParentMail.Trim() != "" && regParentPsswd.Trim() != "" && regParentPsswdCheck.Trim() != "")
                                            {
                                                errorText = "";
                                                Regex regex = new Regex(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                                                        + "@"
                                                                        + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$");
                                                Match match = regex.Match(regParentMail);
                                                if (match.Success)
                                                {
                                                    errorText = "";
                                                    if (regParentPsswd.Length < 8)
                                                    {
                                                        errorText = language.levelStrings[20];
                                                    }
                                                    else
                                                    {
                                                        if (regParentPsswd == regParentPsswdCheck)
                                                        {
                                                            registerStep = 2;
                                                            //trial.registerParent(regParentName.Trim(),regParentSurname.Trim(),regParentMail.Trim(),regParentPsswd.Trim());
                                                        }
                                                        else
                                                        {
                                                            errorText = language.levelStrings[21];
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    errorText = language.levelStrings[22];
                                                }
                                            }
                                            else
                                            {
                                                errorText = language.levelStrings[23];
                                            }
                                        }
                                        GUI.EndGroup();
                                        break;
                                    case 2:
                                        GUI.BeginGroup(new Rect(Screen.width * 0.5f - 500 * mapScale, Screen.height * 0.5f - 250 * mapScale, 400 * mapScale, 500 * mapScale));
                                        GUI.DrawTexture(new Rect(0, 0, 400 * mapScale, 600 * mapScale), formInfoTexture);
                                        GUI.DrawTexture(new Rect(25 * mapScale, 250 * mapScale, 350 * mapScale, 235 * mapScale), platformPreview);
                                        GUI.Label(new Rect(20 * mapScale, 130 * mapScale, 360 * mapScale, 100 * mapScale), language.levelStrings[24], regInfoStyle);
                                        GUI.DrawTexture(new Rect(50 * mapScale, 20 * mapScale, 300 * mapScale, 106 * mapScale), towiLogo2);
                                        GUI.EndGroup();

                                        GUI.BeginGroup(new Rect(Screen.width * 0.5f - 100 * mapScale, Screen.height * 0.5f - 250 * mapScale, 600 * mapScale, 500 * mapScale));
                                        GUI.skin.settings.cursorColor = new Color(0.5f, 0.5f, 0.5f);
                                        //GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),overlay);
                                        //GUI.color=new Color(1,1,1,1-opacity);
                                        GUI.DrawTexture(new Rect(0, 0, 600 * mapScale, 500 * mapScale), loadingColor);
                                        //GUI.Box(new Rect(0,0,600*mapScale,600*mapScale),"");
                                        GUI.Label(new Rect(50 * mapScale, 20 * mapScale, 200 * mapScale, 40 * mapScale), language.levelStrings[16], inputsNamesStyle);
                                        GUI.Label(new Rect(50 * mapScale, 110 * mapScale, 160 * mapScale, 40 * mapScale), language.levelStrings[17], inputsNamesStyle);
                                        regKidName = GUI.TextField(new Rect(50 * mapScale, 50 * mapScale, 240 * mapScale, 40 * mapScale), regKidName, inputStyle);
                                        regKidSurname = GUI.TextField(new Rect(50 * mapScale, 140 * mapScale, 260 * mapScale, 40 * mapScale), regKidSurname, inputStyle);
                                        GUI.Label(new Rect(50 * mapScale, 345 * mapScale, 400 * mapScale, 40 * mapScale), errorText, errorStyle2);
                                        errorStyle2.normal.textColor = new Color(0.2f, 0.2f, 0.2f, 1);
                                        GUI.Label(new Rect(50 * mapScale, 180 * mapScale, 400 * mapScale, 40 * mapScale), language.levelStrings[25], errorStyle2);
                                        Rect labelRect = new Rect(50 * mapScale, 260 * mapScale, 270 * mapScale, 40 * mapScale);
                                        errorStyle2.normal.textColor = new Color(0.26f, 0.26f, 1, 1);
                                        if (GUI.Button(labelRect, language.levelStrings[26], errorStyle2))
                                        {
                                            Application.OpenURL("http://www.towi.com.mx/privacy.php");
                                        }
                                        //if (Event.current.type == EventType.MouseUp && labelRect.Contains(Event.current.mousePosition))
                                        //	Application.OpenURL("http://www.towi.com.mx/privacy.php");

                                        //GUI.Label(labelRect, "Terminos y Condiciones",errorStyle2);
                                        errorStyle2.normal.textColor = new Color(1, 0.26f, 0.26f, 1);
                                        if (GUI.Button(new Rect(50 * mapScale, 410 * mapScale, 200 * mapScale, 70 * mapScale), language.levelStrings[2], skipStyle))
                                        {
                                            registerStep = 0;
                                            errorText = "";
                                        }
                                        if (GUI.Button(new Rect(350 * mapScale, 410 * mapScale, 200 * mapScale, 70 * mapScale), language.levelStrings[27], nextStyle))
                                        {
                                            if (regKidName.Trim() != "" && regKidSurname.Trim() != "")
                                            {
                                                errorText = "";
                                                trial.registerParentAndKid(regParentName.Trim(), regParentSurname.Trim(), regParentMail.Trim(), regParentPsswd.Trim(), regKidName.Trim(), regKidSurname.Trim());
                                            }
                                            else
                                            {
                                                errorText = language.levelStrings[23];
                                            }
                                        }
                                        GUI.EndGroup();
                                        break;
                                }
                                if (GUI.Button(new Rect(Screen.width * 0.5f + 468 * mapScale, Screen.height * 0.5f - 260 * mapScale, 40 * mapScale, 40 * mapScale), "", closeButton))
                                {
                                    showRegister = false;
                                    registerStep = 0;
                                    HideBG();
                                }
                            }
                        }
                    }
                }
                break;
            case Phase.Profiles:
                GUI.DrawTexture(new Rect(Screen.width - 130 * mapScale, 30 * mapScale, 100 * mapScale, 100 * mapScale), profilePic);
                //GUI.Label(new Rect(Screen.width-260*mapScale,35*mapScale,100,10),"Héctor",nameTextStyle);
                if (sessionMng.activeUser != null)
                    GUI.Label(new Rect(Screen.width - 250 * mapScale, 35 * mapScale, 100 * mapScale, 10 * mapScale), sessionMng.activeUser.username, nameTextStyle);
                else
                    GUI.Label(new Rect(Screen.width - 250 * mapScale, 35 * mapScale, 100 * mapScale, 10 * mapScale), "?", nameTextStyle);
                if (GUI.Button(new Rect(Screen.width - 250 * mapScale, 100 * mapScale, 100 * mapScale, 20 * mapScale), language.levelStrings[10], logTextStyle))
                {
                    ReturnToMenu();
                    alreadyLogged = false;
                    sessionMng.Logout();
                }
                //GUI.Label(new Rect(Screen.width-260*mapScale,70*mapScale,100,20),"Manuel",surnameTextStyle);
                break;
            case Phase.About:
                aboutTextStyle.alignment = TextAnchor.UpperLeft;
                GUI.Label(new Rect(Screen.width * 0.5f - 320 * mapScale, Screen.height * 0.1f, 400 * mapScale, Screen.height * 0.8f), language.levelStrings[28], aboutTextStyle);
                GUI.Label(new Rect(Screen.width * 0.5f + 100 * mapScale, Screen.height * 0.1f, 400 * mapScale, Screen.height * 0.8f), language.levelStrings[29], aboutTextStyle);
                aboutTextStyle.alignment = TextAnchor.UpperCenter;
                GUI.Label(new Rect(Screen.width * 0.5f - 300 * mapScale, Screen.height - 50 * mapScale, 600 * mapScale, Screen.height * 0.8f),
                          "<size=" + aboutTextStyle.fontSize * 0.7f + "><b>© " + language.levelStrings[30] + "</b></size>"
                          , aboutTextStyle);
                break;
        }
    }

    public void HideBG()
    {
        bgRef.color = new Color(0, 0, 0, 0);
    }

    public string Md5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }
}