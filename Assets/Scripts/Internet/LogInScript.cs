using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Boomlagoon.JSON;

public class LogInScript : MonoBehaviour {
    #region Variables
    //This urls are set in the NewLogin Scene
    string loginUrl = Keys.Api_Web_Key + "api/login/";
    string activeUserUrl = Keys.Api_Web_Key + "api/active_account/";
    string registerUrl = Keys.Api_Web_Key + "api/register_parent_child/";

    string username;
    string password;

    string errorText;

    bool loading;

    SessionManager sessionManager;
    MenuManager menuController;

    #endregion
    // Use this for initialization
    void Awake ()
    {
        sessionManager = FindObjectOfType<SessionManager>();
        menuController = GetComponent<MenuManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public string Md5SUm(string stringToCode) {

        System.Text.UTF8Encoding uTF8 = new System.Text.UTF8Encoding();
        byte[] bytes = uTF8.GetBytes(stringToCode);

        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++) {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }

    public void PostLogin(string parentMail, string currentPasword) {
        username = parentMail;
        password = currentPasword;
        StartCoroutine(PostLoginData());
    }

    IEnumerator PostLoginData() {
        Debug.Log("login in");
        string hash = password;//Md5SUm(password);
        string post_url = loginUrl;

        //Build form to post in server
        WWWForm form = new WWWForm();
        form.AddField("email", username);
        form.AddField("password", password);


        //Post the URL to the site and download a result
        WWW hs_post = new WWW(post_url, form);
        yield return hs_post;

        Debug.Log(hs_post.text);
        //this is what is do if there is no error
        if (hs_post.error == null)
        {
            //we get a JSON object from the server  
            JSONObject jsonObject = JSONObject.Parse(hs_post.text);
            //we check if the accses is allow by a key
            if (jsonObject.GetValue("access").Boolean)
            {
                JSONArray kids = jsonObject.GetValue("children").Array;

                sessionManager.LoadUser(username, hash, jsonObject.GetValue("key").Str, null, (int)jsonObject.GetValue("id").Number);
                sessionManager.AddKids(kids);
                menuController.LoggedNow();
                sessionManager.activeUser.trialAccount = false;
                sessionManager.SaveSession();
                menuController.SetKidsProfiles();
            }
            else
            {
                /*if (sessionManager.FindUser(username))
                {
                    int resultS = sessionManager.TryLogin(username, hash);
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
                }*/
            }
        }
        else
        {
            JSONObject jsonObj = JSONObject.Parse(hs_post.text);
            string error = jsonObj.GetString("status");
            if (error == "USER_NOT_FOUND")
            {
                menuController.ShowWarning(2);
            }
            else
            {
                menuController.ShowWarning(3);
            }
            Debug.Log(error);
        }
    }

    public void IsActive(string user)
    {
        SessionManager.User tempUser = sessionManager.GetUser(user);
        if (tempUser != null)
        {
            StartCoroutine(PostIsActive(tempUser));
        }
        else
        {
            menuController.ShowLogIn();
        }
    }

    IEnumerator PostIsActive(SessionManager.User user)
    {
        string post_url = activeUserUrl;

        // Build form to post in server
        WWWForm form = new WWWForm();
        form.AddField("parent_email", user.username);

        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url, form);
        yield return hs_post; // Wait until the download is done
        if (hs_post.error == null)
        {
            JSONObject jsonObject = JSONObject.Parse(hs_post.text);
            //menuController.ShowGameMenu();
            Debug.Log(hs_post.text);
            //Debug.Log(jsonObject.GetValue("access"));

            if (jsonObject.GetValue("active").Boolean)
            {
                if (sessionManager.LoadActiveUser())
                {
                    sessionManager.SyncProfiles(sessionManager.activeUser.userkey);
                }
                menuController.ShowGameMenu();
            }
            else
            {
                menuController.ShowTrialIsOff();
            }
        }
        else
        {
            int resultS = sessionManager.TryLogin(user.username, user.psswdHash);
            if (resultS == 1)
            {
                if (sessionManager.LoadActiveUser())
                {
                    /*menu.gameObject.SetActive(true);
                    lang = sessionMng.activeUser.language;
                    ChangeLanguage();
                    CreateProfiles();*/
                }
                else
                {
                    /*menu.gameObject.SetActive(false);
                    currentState = Phase.TrialOptions;
                    DisplayLogin();*/
                    menuController.ShowLogIn();
                }
            }
            else
            {
                /*menu.gameObject.SetActive(false);
                currentState = Phase.TrialOptions;
                DisplayLogin();*/
                menuController.ShowLogIn();
            }
        }
    }

    public void RegisterParentAndKid(string email, string password, string kidName, string dateOfBirth)
    {
        StartCoroutine(PostRegisterParentAndKid(email, password, kidName, dateOfBirth));
    }

    IEnumerator PostRegisterParentAndKid(string email, string password, string kidName, string dateOfBirth)
    {
        string psswdHash = password;//Md5SUm(password);
        string post_url = registerUrl;

        JSONObject data = new JSONObject();
        //data.Add("parent_name", name);
        //data.Add("parent_lastname", lastName);
        data.Add("parent_email", email);
        data.Add("parent_password", psswdHash);
        data.Add("child_name", kidName);
        //data.Add("child_lastname", kidLastName);
        data.Add("child_dob", dateOfBirth);
        //data.Add("child_gender", gender);
        data.Add("user_type", "Familiar");
        Debug.Log(data.ToString());
        WWWForm form = new WWWForm();
        form.AddField("jsonToDb", data.ToString());

        WWW hs_post = new WWW(post_url, form);
        yield return hs_post;

        if (hs_post.error == null)
        {
            JSONObject jsonObject = JSONObject.Parse(hs_post.text);
            Debug.Log(jsonObject.GetValue("code").Str);
            if (jsonObject.GetValue("code").Str == "200")
            {
                PostLogin(email, password);
            }
            else
            {
                Debug.Log("Theres an error in the connection");
            }
        }
        else
        {
            menuController.ShowWarning(8);
            Debug.Log("Theres an error " + hs_post.error);
            Debug.Log(hs_post.text);
            //trialMng.loginRef.errorText = trialMng.loginRef.language.levelStrings[39];
        }
    }
}
