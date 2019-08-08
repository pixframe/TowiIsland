using System.IO;
using System.Collections;
using UnityEngine;
using Boomlagoon.JSON;
using UnityEngine.Analytics;
using UnityEngine.Networking;

public class LogInScript : MonoBehaviour
{
    #region Variables
    //This urls are set in the NewLogin Scene
    string loginUrl = Keys.Api_Web_Key + "api/login/";
    string activeUserUrl = Keys.Api_Web_Key + "api/profile/active/";
    string registerUrl = Keys.Api_Web_Key + "api/register/user/";
    string newKidURL = Keys.Api_Web_Key + "api/register/children/";
    string levelUpdateURL = Keys.Api_Web_Key + "api/v2/levels/create/";

    string username;
    string password;

    string errorText;

    bool loading;

    SessionManager sessionManager;
    MenuManager menuController;

    #endregion
    // Use this for initialization
    void Awake()
    {
        sessionManager = FindObjectOfType<SessionManager>();
        menuController = GetComponent<MenuManager>();
    }

    public string Md5SUm(string stringToCode)
    {

        System.Text.UTF8Encoding uTF8 = new System.Text.UTF8Encoding();
        byte[] bytes = uTF8.GetBytes(stringToCode);

        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }

    public void PostLogin(string parentMail, string currentPasword, bool newPaidUser)
    {
        username = parentMail;
        password = currentPasword;
        StartCoroutine(PostLoginData(newPaidUser));
    }

    IEnumerator PostLoginData(bool newPaidUser)
    {
        string hash = password;//Md5SUm(password);
        string post_url = loginUrl;

        //Build form to post in server
        WWWForm form = new WWWForm();
        form.AddField("email", username);
        form.AddField("password", password);


        //Post the URL to the site and download a result
        using (UnityWebRequest request = UnityWebRequest.Post(post_url, form))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError)
            {
                menuController.ShowWarning(9, menuController.ShowLogIn);
            }
            else if (request.isHttpError)
            {
                Debug.Log("this is a error");
                JSONObject jsonObj = JSONObject.Parse(request.downloadHandler.text);
                Debug.Log(request.error);
                Debug.Log(jsonObj);
                string error = jsonObj.GetString("status");
                if (error == "USER_NOT_FOUND")
                {
                    menuController.ShowWarning(2, menuController.ShowLogIn);
                }
                else
                {
                    menuController.ShowWarning(3, menuController.ShowLogIn);
                }
                Debug.Log(error);
            }
            else
            {
                PlayerPrefs.SetInt(Keys.Logged_Session, 1);
                Debug.Log(request.ToString());
                //we get a JSON object from the server  
                JSONObject jsonObject = JSONObject.Parse(request.downloadHandler.text);
                JSONArray kids = jsonObject.GetValue("children").Array;

                sessionManager.LoadUser(username, hash, jsonObject.GetValue("key").Str, null, (int)jsonObject.GetValue("id").Number);
                sessionManager.AddKids(kids);
                sessionManager.SyncProfiles(sessionManager.activeUser.userkey);
                menuController.LoggedNow();
                sessionManager.activeUser.trialAccount = false;
                sessionManager.activeUser.suscriptionsLeft = (int)jsonObject.GetNumber("suscriptionsAvailables");
                sessionManager.SaveSession();
                Debug.Log("We finish the login succesfully");
                if (newPaidUser)
                {
                    Debug.Log("Try to log in");
                    string parentkey = sessionManager.activeUser.kids[0].userkey;
                    int id = sessionManager.activeUser.kids[0].id;
                    sessionManager.SetKid(parentkey, id);
                    if (System.Convert.ToBoolean(PlayerPrefs.GetInt(Keys.Buy_IAP)))
                    {
                        menuController.ConfirmKidPurchase();
                    }
                    else
                    {
                        menuController.ChangeAPrePaidCode(0);
                    }
                }
                else
                {
                    menuController.SetKidsProfiles();
                }
            }
        }
    }

    public void IsActive(string user)
    {
        SessionManager.User tempUser = sessionManager.GetUser(user);

        if (tempUser != null)
        {
            if (PlayerPrefs.GetInt(Keys.Games_Saved) > 0 || PlayerPrefs.GetInt(Keys.Evaluations_Saved) > 0)
            {
                Debug.Log("We try to update the data");
                StartCoroutine(UpdateDataToSend(tempUser));
            }
            else
            {
                Debug.Log("nothing to updtae here");
                StartCoroutine(PostIsActive(tempUser));
            }
        }
        else
        {
            menuController.ShowFirstMenu();
        }
    }

    public void UpdateData()
    {
        StartCoroutine(UpdateDataRoutine());
    }

    IEnumerator PostIsActive(SessionManager.User user)
    {
        Debug.Log("Now we start to chechk if the player its active");
        string post_url = activeUserUrl;

        // Build form to post in server
        WWWForm form = new WWWForm();
        form.AddField("parent_email", user.username);

        using (UnityWebRequest request = UnityWebRequest.Post(activeUserUrl, form))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                menuController.HideAllCanvas();
                menuController.ShowWarning(8);
            }
            else
            {
                PlayerPrefs.SetInt(Keys.Logged_Session, 1);
                JSONObject jsonObject = JSONObject.Parse(request.downloadHandler.text);

                sessionManager.LoadActiveUser(user.userkey);
                sessionManager.SyncProfiles(sessionManager.activeUser.userkey);
                sessionManager.activeUser.suscriptionsLeft = (int)jsonObject.GetNumber("suscriptionsAvailables");

                while (sessionManager.IsDownlodingData())
                {
                    yield return null;
                }

                if (jsonObject.GetValue("active").Boolean)
                {
                    if (sessionManager.activeKid != null)
                    {
                        menuController.ShowGameMenu();
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
        }
    }

    IEnumerator UpdateDataRoutine()
    {
        Debug.Log($"Evaluations to be saved {PlayerPrefs.GetInt(Keys.Evaluations_Saved)}, Games to be saved {PlayerPrefs.GetInt(Keys.Games_Saved)}");
        var user = sessionManager.GetUser(PlayerPrefs.GetString(Keys.Active_User_Key));
        int gamesSaved = PlayerPrefs.GetInt(Keys.Games_Saved);
        if (gamesSaved > 0)
        {
            int dataNotSavedByProblems = 0;
            for (int i = 0; i < gamesSaved; i++)
            {
                int x = i;
                string pathOfFile = $"{Application.persistentDataPath}/{x}{Keys.Game_To_Save}";

                if (File.Exists(pathOfFile))
                {
                    string jsonString = File.ReadAllText(pathOfFile);

                    var jsonObj = JSONObject.Parse(jsonString);

                    var form = new WWWForm();
                    form.AddField("jsonToDb", jsonObj.ToString());

                    using (UnityWebRequest request = UnityWebRequest.Post(levelUpdateURL, form))
                    {
                        int dataRemaining = i;
                        yield return request.SendWebRequest();
                        if (request.isNetworkError || request.isHttpError)
                        {
                            PlayerPrefs.SetInt(Keys.Games_Saved, dataRemaining);
                            Debug.Log($"We have trouble uploading the data this is the error :\n{request.downloadHandler.text}");
                            string newPathOfFile = $"{Application.persistentDataPath}/{dataNotSavedByProblems}{Keys.Game_To_Save}";
                            dataNotSavedByProblems++;
                            if (pathOfFile != newPathOfFile)
                            {
                                File.Delete(newPathOfFile);
                                File.Move(pathOfFile, newPathOfFile);
                            }
                        }
                        else
                        {
                            JSONObject response = JSONObject.Parse(request.downloadHandler.text);
                            Debug.Log(response["code"].Str);
                            if (response["code"].Str != "200" && response["code"].Str != "201")
                            {
                                Debug.Log("The game data has been upload");
                                File.Delete(pathOfFile);
                            }
                        }
                    }
                }
            }
            PlayerPrefs.SetInt(Keys.Games_Saved, dataNotSavedByProblems);
        }

        int evaluationSaved = PlayerPrefs.GetInt(Keys.Evaluations_Saved);
        if (evaluationSaved > 0)
        {
            int dataNotSavedByProblems = 0;
            for (int i = 0; i < evaluationSaved; i++)
            {
                int x = i;
                string pathOfFile = $"{Application.persistentDataPath}/{x}{Keys.Evaluation_To_Save}";

                if (File.Exists(pathOfFile))
                {
                    string jsonString = File.ReadAllText(pathOfFile);

                    var jsonObj = JSONObject.Parse(jsonString);

                    var form = new WWWForm();
                    form.AddField("jsonToDb", jsonObj.ToString());

                    using (UnityWebRequest request = UnityWebRequest.Post(levelUpdateURL, form))
                    {
                        int dataRemaining = i;
                        yield return request.SendWebRequest();
                        if (request.isNetworkError || request.isHttpError)
                        {
                            PlayerPrefs.SetInt(Keys.Games_Saved, dataRemaining);
                            Debug.Log($"We have trouble uploading the data this is the error :\n{request.downloadHandler.text}");
                            string newPathOfFile = $"{Application.persistentDataPath}/{dataNotSavedByProblems}{Keys.Evaluation_To_Save}";
                            dataNotSavedByProblems++;
                            if (pathOfFile != newPathOfFile)
                            {
                                File.Delete(newPathOfFile);
                                File.Move(pathOfFile, newPathOfFile);
                            }
                        }
                        else
                        {
                            JSONObject response = JSONObject.Parse(request.downloadHandler.text);
                            Debug.Log(response["code"].Str);
                            if (response["code"].Str != "200" && response["code"].Str != "201")
                            {
                                Debug.Log("The game data has been upload");
                                File.Delete(pathOfFile);
                            }
                        }
                    }
                }

            }
            PlayerPrefs.SetInt(Keys.Evaluations_Saved, dataNotSavedByProblems);
        }

        if (PlayerPrefs.GetInt(Keys.Evaluations_Saved) == 0 || PlayerPrefs.GetInt(Keys.Games_Saved) == 0)
        {
            Debug.Log("Ok");
            menuController.ShowSyncMessage(1);
        }
        else
        {
            Debug.Log("Not OK");
            menuController.ShowSyncMessage(2);
        }
    }

    IEnumerator UpdateDataToSend(SessionManager.User user)
    {
        Debug.Log("We are updating data");

        int gamesSaved = PlayerPrefs.GetInt(Keys.Games_Saved);
        if (gamesSaved > 0)
        {
            int dataNotSavedByProblems = 0;
            for (int i = 0; i < gamesSaved; i++)
            {
                int x = i;
                string pathOfFile = $"{Application.persistentDataPath}/{x}{Keys.Game_To_Save}";

                if (File.Exists(pathOfFile))
                {
                    string jsonString = File.ReadAllText(pathOfFile);

                    var jsonObj = JSONObject.Parse(jsonString);

                    var form = new WWWForm();
                    form.AddField("jsonToDb", jsonObj.ToString());

                    using (UnityWebRequest request = UnityWebRequest.Post(levelUpdateURL, form))
                    {
                        int dataRemaining = i;
                        yield return request.SendWebRequest();
                        if (request.isNetworkError || request.isHttpError)
                        {
                            PlayerPrefs.SetInt(Keys.Games_Saved, dataRemaining);
                            Debug.Log($"We have trouble uploading the data this is the error :\n{request.downloadHandler.text}");
                            string newPathOfFile = $"{Application.persistentDataPath}/{dataNotSavedByProblems}{Keys.Game_To_Save}";
                            dataNotSavedByProblems++;
                            if (pathOfFile != newPathOfFile)
                            {
                                File.Delete(newPathOfFile);
                                File.Move(pathOfFile, newPathOfFile);
                            }
                        }
                        else
                        {
                            JSONObject response = JSONObject.Parse(request.downloadHandler.text);
                            Debug.Log(response["code"].Str);
                            if (response["code"].Str != "200" && response["code"].Str != "201")
                            {
                                Debug.Log("The game data has been upload");
                                File.Delete(pathOfFile);
                            }
                        }
                    }
                }
            }
            PlayerPrefs.SetInt(Keys.Games_Saved, dataNotSavedByProblems);
        }

        int evaluationSaved = PlayerPrefs.GetInt(Keys.Evaluations_Saved);
        if (evaluationSaved > 0)
        {
            int dataNotSavedByProblems = 0;
            for (int i = 0; i < evaluationSaved; i++)
            {
                int x = i;
                string pathOfFile = $"{Application.persistentDataPath}/{x}{Keys.Evaluation_To_Save}";

                if (File.Exists(pathOfFile))
                {
                    string jsonString = File.ReadAllText(pathOfFile);

                    var jsonObj = JSONObject.Parse(jsonString);

                    var form = new WWWForm();
                    form.AddField("jsonToDb", jsonObj.ToString());

                    using (UnityWebRequest request = UnityWebRequest.Post(levelUpdateURL, form))
                    {
                        int dataRemaining = i;
                        yield return request.SendWebRequest();
                        if (request.isNetworkError || request.isHttpError)
                        {
                            PlayerPrefs.SetInt(Keys.Games_Saved, dataRemaining);
                            Debug.Log($"We have trouble uploading the data this is the error :\n{request.downloadHandler.text}");
                            string newPathOfFile = $"{Application.persistentDataPath}/{dataNotSavedByProblems}{Keys.Evaluation_To_Save}";
                            dataNotSavedByProblems++;
                            if (pathOfFile != newPathOfFile)
                            {
                                File.Delete(newPathOfFile);
                                File.Move(pathOfFile, newPathOfFile);
                            }
                        }
                        else
                        {
                            JSONObject response = JSONObject.Parse(request.downloadHandler.text);
                            Debug.Log(response["code"].Str);
                            if (response["code"].Str != "200" && response["code"].Str != "201")
                            {
                                Debug.Log("The game data has been upload");
                                File.Delete(pathOfFile);
                            }
                        }
                    }
                }

            }
            PlayerPrefs.SetInt(Keys.Evaluations_Saved, dataNotSavedByProblems);
        }

        StartCoroutine(PostIsActive(user));
    }

    public void RegisterParentAndKid(string email, string password, string kidName, string dateOfBirth, bool newPaidUser)
    {
        StartCoroutine(PostRegisterParentAndKid(email, password, kidName, dateOfBirth, newPaidUser));
    }

    IEnumerator PostRegisterParentAndKid(string email, string password, string kidName, string dateOfBirth, bool newPaidUser)
    {
        string psswdHash = password;//Md5SUm(password);

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

        WWWForm form = new WWWForm();
        form.AddField("jsonToDb", data.ToString());

        using (UnityWebRequest request = UnityWebRequest.Post(registerUrl, form))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError)
            {
                menuController.ShowWarning(8);
                Debug.Log($"Theres an error {request.error}");
                Debug.Log(request.downloadHandler);
            }
            else if (request.isHttpError)
            {
                menuController.CreateAccount();
                menuController.ShowWarning(13);
                Debug.Log($"Theres an error {request.error}");
            }
            else
            {
                PostLogin(email, password, newPaidUser);
                Analytics.CustomEvent("register");
            }
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
