using System.Collections;
using UnityEngine;
using Boomlagoon.JSON;
using UnityEngine.Analytics;

public class LogInScript : MonoBehaviour {
    #region Variables
    //This urls are set in the NewLogin Scene
    string loginUrl = Keys.Api_Web_Key + "api/login/";
    string activeUserUrl = Keys.Api_Web_Key + "api/profile/active/";
    string registerUrl = Keys.Api_Web_Key + "api/register/user/";
    string newKidURL = Keys.Api_Web_Key + "api/register/children/";

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

        //this is what is do if there is no error
        if (hs_post.error == null)
        {
            //we get a JSON object from the server  
            JSONObject jsonObject = JSONObject.Parse(hs_post.text);
            JSONArray kids = jsonObject.GetValue("children").Array;

            sessionManager.LoadUser(username, hash, jsonObject.GetValue("key").Str, null, (int)jsonObject.GetValue("id").Number);
            sessionManager.AddKids(kids);
            sessionManager.SyncProfiles(sessionManager.activeUser.userkey);
            menuController.LoggedNow();
            sessionManager.activeUser.trialAccount = false;
            sessionManager.activeUser.suscriptionsLeft = (int)jsonObject.GetNumber("suscriptionsAvailables");
            sessionManager.SaveSession();
            menuController.SetKidsProfiles();
            //we check if the accses is allow by a key
        }
        else if (hs_post.text == "")
        {
            menuController.LogInMenuActive();
            menuController.ShowWarning(9);
        }
        else
        {
            menuController.LogInMenuActive();
            JSONObject jsonObj = JSONObject.Parse(hs_post.text);
            Debug.Log(jsonObj.ToString());
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

        Debug.Log(hs_post.text);

        if (hs_post.error == null)
        {
            JSONObject jsonObject = JSONObject.Parse(hs_post.text);
            //menuController.ShowGameMenu();

            //Debug.Log(jsonObject.GetValue("access"));

            sessionManager.LoadActiveUser(user.userkey);
            sessionManager.SyncProfiles(sessionManager.activeUser.userkey);
            sessionManager.activeUser.suscriptionsLeft = (int)jsonObject.GetNumber("suscriptionsAvailables");

            if (jsonObject.GetValue("active").Boolean)
            {
                if (sessionManager.activeKid != null)
                {
                    if (sessionManager.activeKid.isActive)
                    {
                        menuController.ShowGameMenu();
                    }
                    else
                    {
                        menuController.ShowAccountWarning(0);
                    }
                }
                else
                {
                    menuController.SetKidsProfiles();
                }
            }
            else
            {
                menuController.ShowAccountWarning(0);
            }
        }
        else
        {
            menuController.HideAllCanvas();
            menuController.ShowWarning(8);
            /*int resultS = sessionManager.TryLogin(user.username, user.psswdHash);
            if (resultS == 1)
            {
                sessionManager.LoadActiveUser(user.userkey);
                menuController.ShowLogIn();
                /*if ()
                {
                    /*menu.gameObject.SetActive(true);
                    lang = sessionMng.activeUser.language;
                    ChangeLanguage();
                    CreateProfiles();
                }
                else
                {
                    /*menu.gameObject.SetActive(false);
                    currentState = Phase.TrialOptions;
                    DisplayLogin();

                }*/
            /*}
            else
            {
                /*menu.gameObject.SetActive(false);
                currentState = Phase.TrialOptions;
                DisplayLogin();
                menuController.ShowLogIn();
            }*/
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
                Analytics.CustomEvent("register");
            }
            else
            {
                menuController.CreateAccount();
                menuController.ShowWarning(13);
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

    public void RegisterAKid(string dobKid, string nameKid, int parentId)
    {
        StartCoroutine(RegisterKid(dobKid, nameKid, parentId));
    }

    IEnumerator RegisterKid(string dobKid, string nameKid, int parentId)
    {
        JSONObject jsonObj = new JSONObject
        {
            { "child_dob", dobKid},
            { "child_name", nameKid},
            { "parent_id", parentId}
        };

        WWWForm form = new WWWForm();
        form.AddField("jsonToDb", jsonObj.ToString());

        WWW post = new WWW(newKidURL, form);
        yield return post;
        Debug.Log(post.text);
        JSONObject jsonObt = JSONObject.Parse(post.text);
        if (post.text == "")
        {
            menuController.ShowWarning(9);
        }
        else
        {
            if (jsonObt.ContainsKey("status"))
            {
                menuController.ShowWarning(9);
                menuController.AddKidShower();
            }
            else
            {
                sessionManager.activeUser.suscriptionsLeft = (int)jsonObt.GetNumber("suscriptionsAvailables");
                sessionManager.SyncProfiles(sessionManager.activeUser.userkey);
                sessionManager.activeKid = sessionManager.activeUser.kids[sessionManager.activeUser.kids.Count - 1];
                sessionManager.SaveSession();
                menuController.ShowGameMenu();
            }
        }
    }
}
