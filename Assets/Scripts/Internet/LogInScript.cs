﻿using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON;
using UnityEngine.Analytics;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Linq;

public class LogInScript : MonoBehaviour
{
    #region Offline Variables
    string serialTemp;
    string userTemp;
    List<string> fileLines;
    List<string> userLines;


    #endregion

    #region Variables
    //This urls are set in the NewLogin Scene
    string loginUrl = Keys.Api_Web_Key + "api/login/";
    string activeUserUrl = Keys.Api_Web_Key + "api/profile/active/";
    string registerUrl = Keys.Api_Web_Key + "api/register/user/";
    string newKidURL = Keys.Api_Web_Key + "api/register/children/";
    string levelUpdateURL = Keys.Api_Web_Key + "api/v2/levels/create/";

    string username;
    string password;

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
        Debug.Log("PostLogin");
        username = parentMail.TrimEnd('\n');
        password = currentPasword;
        StartCoroutine(PostLoginData(newPaidUser));
    }

    private void Start() {
        
    // var downloadhandler = "[{'id':9464,'access':true,'key':'d61f3e54e37ece1d2ee4231a3d9c2110a731a292e1ffc4babbd8bf0205d6dc2b','userExist':true'children':['cid':9788,'name'ANDRES Prueba','lastname':'','active':true,'trial':false,'key':'d61f3e54e37ece1d2ee4231a3d9c2110a731a292e1ffc4babbd8bf0205d6dc2b''picture2:'https://storage.googleapis.com/storage-towi//avatars/default_user.png','age':6,'pruebaDate':'}],'suscriptionsAvailables':0}']".Replace("'", "\"");
    // Debug.Log(downloadhandler);
    }

    IEnumerator PostLoginData(bool newPaidUser)
    {
        string hash = password;//Md5SUm(password);
        string post_url = loginUrl;

        //Build form to post in server
        WWWForm form = new WWWForm();
        form.AddField("email", username);
        form.AddField("password", password);

        PlayerPrefs.SetInt(Keys.Logged_Session, 1);
        PlayerPrefs.SetInt(Keys.First_Try, 1);
        // //we get a JSON object from the server  
        
        //string downloadhandler;
        //downloadhandler = @"""Hi, "" I am programmer";
        var value = "'Field1','Field2','Field3'".Replace("'", "\"");
        var downloadhandler = "{'id':9464,'access':true,'key':'d61f3e54e37ece1d2ee4231a3d9c2110a731a292e1ffc4babbd8bf0205d6dc2b','userExists':true,'children':[{'cid':9788,'name':'ANDRES Prueba','lastname':'','active':true,'trial':false,'key':'d61f3e54e37ece1d2ee4231a3d9c2110a731a292e1ffc4babbd8bf0205d6dc2b','picture2':'https://storage.googleapis.com/storage-towi//avatars/default_user.png','age':6,'pruebaDate':''}],'suscriptionsAvailables':10}".Replace("'", "\"");
        
        //var downloadhandler = """[{"id":9464,"access":true,"key":"d61f3e54e37ece1d2ee4231a3d9c2110a731a292e1ffc4babbd8bf0205d6dc2b","userExists":true,"children":[{"cid":9788,"name":"ANDRES Prueba","lastname":"","active":true,"trial":false,"key":"d61f3e54e37ece1d2ee4231a3d9c2110a731a292e1ffc4babbd8bf0205d6dc2b","picture2:"https://storage.googleapis.com/storage-towi//avatars/default_user.png","age":6,"pruebaDate":"}],"suscriptionsAvailables":0}"]""".Replace("'", "\"");
        Debug.Log(downloadhandler);
        Debug.Log(value);
        //var downloadhandler = "{'cid':9788,'name':'ANDRES Prueba','lastname':'','active':true,'trial':false,'key':'d61f3e54e37ece1d2ee4231a3d9c2110a731a292e1ffc4babbd8bf0205d6dc2b','picture':'https://storage.googleapis.com/storage-towi//avatars/default_user.png','age':6,'pruebaDate':''".Replace("'", "\"");
        //downloadhandler.Replace("'", "\"");
        //Debug.Log(downloadhandler);
        // //downloadhandler = """[{"id":9464,"access":true,"key":"d61f3e54e37ece1d2ee4231a3d9c2110a731a292e1ffc4babbd8bf0205d6dc2b","userExists":true,"children":[{"cid":9788,"name":"ANDRES Prueba","lastname":"","active":true,"trial":false,"key":"d61f3e54e37ece1d2ee4231a3d9c2110a731a292e1ffc4babbd8bf0205d6dc2b","picture2:"https://storage.googleapis.com/storage-towi//avatars/default_user.png","age":6,"pruebaDate":"}],"suscriptionsAvailables":0}"]""";
        //JSONObject jsonObject = JSONObject.Parse(request.downloadHandler.text);
        JSONObject jsonObject = new JSONObject();
        jsonObject = JSONObject.Parse(downloadhandler);
        
        
        
        // downloadhandler = $@"   
        // {{
        //     'id': {id}
        // }}
        // ";
        // Debug.Log(downloadhandler);
        
        
        //JSONObject jsonObject = JSONObject.Parse(downloadhandler);    
        Debug.Log("esto es el JSobject"+jsonObject);
        // jsonObject.Add("id",9999);
        // jsonObject.Add("access",true);
        // //jsonObject.Add("key","d61f3e54e37ece1d2ee4231a3d9c2110a731a292e1ffc4babbd8bf0205d6dc2b");
        // jsonObject.Add("key","d");
        
        // jsonObject.Add("children",downloadhandler);
        
        // // //jsonObject.Add("children", [{"cid",9788,"name","ANDRES Prueba","lastname","","active",true,"trial",false,"key","d61f3e54e37ece1d2ee4231a3d9c2110a731a292e1ffc4babbd8bf0205d6dc2b","picture","https://storage.googleapis.com/storage-towi//avatars/default_user.png","age",6,"pruebaDate",""}]);
        // // //jsonObject.Add("children","ANDRES");
        // jsonObject.Add("suscriptionsAvailables",10);
        // jsonObject.Add("userExists",true);
        // // jsonObject.Add("status","01");
        // // jsonObject.Add("message","01");
        // //jsonObject.Add("children","children");
        // //JSONArray kids = new JSONArray();
        

        
        // JSONObject data = new JSONObject();
        
        // data.Add("cid", 9999);
        // data.Add("name", "Andres");
        // data.Add("lastname", "");
        // data.Add("active", true);
        
        // data.Add("trial", false);
        // data.Add("key", "d");
        // data.Add("picture", "https://storage.googleapis.com/storage-towi//avatars/default_user.png");
        // data.Add("age", 6);
        // data.Add("pruebaDate", "");
        
      
        
        // string json = JsonUtility.ToJson(data);
        // Debug.Log("esto es el object json " +"["+data+"]");
        // string a = "["+data+"]";
        // Debug.Log("esto es "+a);
        // JSONObject c = new JSONObject();
        // c.Add("key",a);
        // JSONValue b = a;
        // Debug.Log("Esto es c "+c);
        // Debug.Log("Esto es b "+b);
        // //jsonObject.Add("children", a);
        //string[] dataArray = new string[9];
        //JSONValue valueData = new JSONValue(a);
        //Debug.Log("Esto es el valueData "+valueData);
        //valueData = a;
        //jsonObject.Add("children", a);
        // { "kidData": [
        //     {"cid":9788},
        //     {"name":"ANDRES Prueba"},
        //     {"lastname":""},
        //     {"active":true},
        //     {"trial":false},
        //     {"key":"d61f3e54e37ece1d2ee4231a3d9c2110a731a292e1ffc4babbd8bf0205d6dc2b"},
        //     {"picture":"https://storage.googleapis.com/storage-towi//avatars/default_user.png"},
        //     {"age":6},
        //     {"pruebaDate":""}
        //     ]
        // }
        //JSONArray kids = jsonObject.GetValue("children").Array;
        JSONArray kids = jsonObject.GetValue("children").Array;
        Debug.Log("Esto es CHILDREN " +jsonObject.GetValue("children"));
        Debug.Log("Esto es kids  " +kids);
        if(kids == null)
        {
            Debug.Log("kids esta VACIO");
        }
        Debug.Log("DESPUES DE SUMAR "+ jsonObject.GetValue("suscriptionsAvailables"));



        //Forzar que sea un usuario nuevo
        newPaidUser = true;

        sessionManager.LoadUser(username, hash, jsonObject.GetValue("key").Str, null, (int)jsonObject.GetValue("id").Number);
        sessionManager.AddKids(kids);
        sessionManager.SyncProfiles(sessionManager.activeUser.userkey);
        menuController.LoggedNow();
        sessionManager.activeUser.trialAccount = false;
        sessionManager.activeUser.suscriptionsLeft = (int)jsonObject.GetNumber("suscriptionsAvailables");
        sessionManager.SaveSession();
        menuController.ClearInputs();
        if (newPaidUser)
        {
            Debug.Log("Entramos al if de newPaidUser");
            Debug.Log("Entramos al sessionManager" + sessionManager.activeUser.kids[0].userkey);
            string parentkey = sessionManager.activeUser.kids[0].userkey;
            Debug.Log("Pasamos el string");
            int id = sessionManager.activeUser.kids[0].id;
            sessionManager.SetKid(parentkey, id);
            if (System.Convert.ToBoolean(PlayerPrefs.GetInt(Keys.Buy_IAP)))
            {
                Debug.Log("Estamos en el if despues de SetKid");
                menuController.ConfirmKidPurchase();
            }
            else
            {
                Debug.Log("Estamos en el else para SystemConvert");
                menuController.ChangeAPrePaidCode(0);
            }
        }
        else
        {
            
            Debug.Log("Estamos en el else para el SetKids");
            menuController.SetKidsProfiles();
        }
        yield return null;

        //Post the URL to the site and download a result
        // using (UnityWebRequest request = UnityWebRequest.Post(post_url, form))
        // {
        //     Debug.Log("Entramos al request   "+request);
            
        //     yield return request.SendWebRequest();
        //     if (request.result == UnityWebRequest.Result.ConnectionError)
        //     {
        //         menuController.ShowWarning(9, menuController.ShowLogIn);
        //     }
        //     else if (request.result == UnityWebRequest.Result.ProtocolError)
        //     {
        //         JSONObject jsonObj = JSONObject.Parse(request.downloadHandler.text);
                
        //         Debug.Log(jsonObj);
        //         if (request.error.Contains("401"))
        //         {
        //             menuController.ShowWarning(15, menuController.ShowLogIn);
        //         }
        //         else if (request.error.Contains("404"))
        //         {
        //             menuController.ShowWarning(14, menuController.ShowLogIn);
        //         }
        //         else
        //         {
        //             menuController.ShowWarning(9, menuController.ShowLogIn);
        //         }
        //     }
        //     else
        //     {
        //         Debug.Log($"endpoint  {post_url} response is ñ{request.downloadHandler.text}");
        //         Debug.Log("EL TEXTO   "+request.downloadHandler.text);
                
        //         PlayerPrefs.SetInt(Keys.Logged_Session, 1);
        //         PlayerPrefs.SetInt(Keys.First_Try, 1);
        //         //we get a JSON object from the server  
        //         JSONObject jsonObject = JSONObject.Parse(request.downloadHandler.text);
        //         Debug.Log("esto es el campo children  "+ jsonObject.GetValue("children"));
        //         Debug.Log("ANTES DE SUMAR "+ jsonObject.GetValue("suscriptionsAvailables"));
        //         jsonObject.Add("suscriptionsAvailables", 10);
        //         Debug.Log("DESPUES DE SUMAR "+ jsonObject.GetValue("suscriptionsAvailables"));
        //         JSONArray kids = jsonObject.GetValue("children").Array;
        //         Debug.Log("ESTO ES CHILDRENfkid  "+jsonObject.GetValue("children"));
        //         Debug.Log("ESTO ES KIDS ARRAY  "+jsonObject.GetValue("children").Array);
        //         if(kids == null)
        //         {
        //             Debug.Log("kids esta vacio");
        //         }
        //         Debug.Log("ESTO ES KIDS   "+kids);

        //         sessionManager.LoadUser(username, hash, jsonObject.GetValue("key").Str, null, (int)jsonObject.GetValue("id").Number);
        //         sessionManager.AddKids(kids);
        //         sessionManager.SyncProfiles(sessionManager.activeUser.userkey);
        //         menuController.LoggedNow();
        //         sessionManager.activeUser.trialAccount = false;
        //         sessionManager.activeUser.suscriptionsLeft = (int)jsonObject.GetNumber("suscriptionsAvailables");
        //         sessionManager.SaveSession();
        //         menuController.ClearInputs();
        //         if (newPaidUser)
        //         {
        //             string parentkey = sessionManager.activeUser.kids[0].userkey;
        //             int id = sessionManager.activeUser.kids[0].id;
        //             sessionManager.SetKid(parentkey, id);
        //             if (System.Convert.ToBoolean(PlayerPrefs.GetInt(Keys.Buy_IAP)))
        //             {
        //                 menuController.ConfirmKidPurchase();
        //             }
        //             else
        //             {
        //                 menuController.ChangeAPrePaidCode(0);
        //             }
        //         }
        //         else
        //         {
        //             menuController.SetKidsProfiles();
        //         }
        //     }
        // }
    }

    public void IsActive(string user)
    {
        var tempUser = sessionManager.GetUser(user);

        if (tempUser != null)
        {
            if (PlayerPrefs.GetInt(Keys.Games_Saved) > 0 || PlayerPrefs.GetInt(Keys.Evaluations_Saved) > 0)
            {
                StartCoroutine(UpdateDataToSend(tempUser));
            }
            else
            {
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
        // Build form to post in server
        WWWForm form = new WWWForm();
        form.AddField("parent_email", user.username);

        using (UnityWebRequest request = UnityWebRequest.Post(activeUserUrl, form))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                menuController.HideAllCanvas();
                menuController.ShowWarning(8);
            }
            else
            {
                PlayerPrefs.SetInt(Keys.Logged_Session, 1);
                JSONObject jsonObject = JSONObject.Parse(request.downloadHandler.text);
                Debug.Log($"endpoint {activeUserUrl} response is {request.downloadHandler.text}");
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
                        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                        {
                            PlayerPrefs.SetInt(Keys.Games_Saved, dataRemaining);
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
                            if (response["code"].Str != "200" && response["code"].Str != "201")
                            {
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
                        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                        {
                            PlayerPrefs.SetInt(Keys.Games_Saved, dataRemaining);
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
                            if (response["code"].Str != "200" && response["code"].Str != "201")
                            {
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
            menuController.ShowSyncMessage(1);
        }
        else
        {
            menuController.ShowSyncMessage(2);
        }
    }

    IEnumerator UpdateDataToSend(SessionManager.User user)
    {
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
                        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                        {
                            PlayerPrefs.SetInt(Keys.Games_Saved, dataRemaining);
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
                            if (response["code"].Str != "200" && response["code"].Str != "201")
                            {
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
                        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                        {
                            PlayerPrefs.SetInt(Keys.Games_Saved, dataRemaining);
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
                            if (response["code"].Str != "200" && response["code"].Str != "201")
                            {
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
        bool temp = ValidateEmail(email, password);
        if(temp==true){StartCoroutine(PostRegisterParentAndKid(email, password, kidName, dateOfBirth, newPaidUser));}
        else{
            Debug.Log("ya fallo");
        }
    }

    public bool ValidateEmail(string email, string password)
    {
        bool warningUser = false;
        bool warningSerial = false;
        bool userCorrect = false;
        bool serialCorrect = false;


        string readFromFilePath = Application.streamingAssetsPath +"/"+ "serialKeys"+".txt";
        fileLines = File.ReadAllLines(readFromFilePath).ToList();
        string userPath = Application.streamingAssetsPath +"/"+ "users"+".txt";
        userLines = File.ReadAllLines(userPath).ToList();


        userTemp = email;
        serialTemp = password;
        Debug.Log(userTemp);
        Debug.Log(serialTemp);
        
        for(int i=0;i<fileLines.Count;i++)
            {
                //Debug.Log(fileLines[i]);
            }
            if(fileLines.Contains(serialTemp))
            {
                serialCorrect = true;
                Debug.Log("SI es un serial correcto");
            }
            else
            {
                warningSerial = true;
                Debug.Log("NO es un serial correcto");  
            }


        for(int i=0;i<userLines.Count;i++)
            {
                //Debug.Log(fileLines[i]);
            }
            if(userLines.Contains(userTemp))
            {
                userCorrect = true;
                Debug.Log("SI es un usuario correcto");
                //return true;
            }
            else
            {
                
                warningUser = true;
                Debug.Log("NO es un usuario correcto"); 
                //return false; 
            }
            
        if((warningSerial == true) || (warningUser==true))
        {
            return false;
            //warningPanel.SetActive(true);
        }
        if((userCorrect==true) && (serialCorrect==true))
        {
            return true;
            // SceneManager.LoadScene("NewLogin", LoadSceneMode.Single);
            // PlayerPrefs.SetInt("quitScene", 1);
        }
        return false;

            

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
        //menuController.CreateAccount();
        PostLogin(email, password, newPaidUser);
        Analytics.CustomEvent("register");

        JSONObject obj =  new JSONObject();
        obj.Add("code", "111");
        bool towi = true;
        bool pass = true;
                if (towi == true)
                {
                    if (pass == false)
                    {
                        Debug.Log("por alguna razon hubo fallo");
                        menuController.ShowWarning(13, () => menuController.ShowRegister(System.Convert.ToBoolean(PlayerPrefs.GetInt(Keys.Buy_IAP))));
                    }
                    else
                    {
                        PostLogin(email, password, newPaidUser);
                        Analytics.CustomEvent("register");
                    }
                }
                else
                {
                    PostLogin(email, password, newPaidUser);
                    Analytics.CustomEvent("register");
                }

        yield return null;
        // using (UnityWebRequest request = UnityWebRequest.Post(registerUrl, form))
        // {
        //     yield return request.SendWebRequest();
        //     if (request.result == UnityWebRequest.Result.ConnectionError)
        //     {
        //         menuController.ShowWarning(8);
        //     }
        //     else if (request.result == UnityWebRequest.Result.ProtocolError)
        //     {
        //         menuController.CreateAccount();
        //         menuController.ShowWarning(13);
        //     }
        //     else
        //     {
        //         JSONObject obj = JSONObject.Parse(request.downloadHandler.text);
        //         if (obj.ContainsKey("code"))
        //         {
        //             if (obj["code"].Str == "111")
        //             {
        //                 menuController.ShowWarning(13, () => menuController.ShowRegister(System.Convert.ToBoolean(PlayerPrefs.GetInt(Keys.Buy_IAP))));
        //             }
        //             else
        //             {
        //                 PostLogin(email, password, newPaidUser);
        //                 Analytics.CustomEvent("register");
        //             }
        //         }
        //         else
        //         {
        //             PostLogin(email, password, newPaidUser);
        //             Analytics.CustomEvent("register");
        //         }
        //     }
        // }
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
        //JSONObject jsonObt = JSONObject.Parse(request.downloadHandler.text);

        string downloadhandler;
        downloadhandler = "{'id':9464,'access':true,'key':'d61f3e54e37ece1d2ee4231a3d9c2110a731a292e1ffc4babbd8bf0205d6dc2b','userExists':true,'children':[{'cid':9788,'name':'ANDRES Prueba','lastname':'','active':true,'trial':false,'key':'d61f3e54e37ece1d2ee4231a3d9c2110a731a292e1ffc4babbd8bf0205d6dc2b','picture':'https://storage.googleapis.com/storage-towi//avatars/default_user.png','age':6,'pruebaDate':''}],'suscriptionsAvailables':0}";
        downloadhandler = "{'cid':9788,'name':'ANDRES Prueba','lastname':'','active':true,'trial':false,'key':'d61f3e54e37ece1d2ee4231a3d9c2110a731a292e1ffc4babbd8bf0205d6dc2b','picture':'https://storage.googleapis.com/storage-towi//avatars/default_user.png','age':6,'pruebaDate':''";
        //JSONObject jsonObt = JSONObject.Parse(request.downloadHandler.text);
        JSONObject jsonObt = new JSONObject();
        jsonObt.Add("id",9999);
        jsonObt.Add("access",true);
        jsonObt.Add("key","d61f3e54e37ece1d2ee4231a3d9c2110a731a292e1ffc4babbd8bf0205d6dc2b");
        
        jsonObt.Add("children",downloadhandler);
        jsonObt.Add("suscriptionsAvailables",9);
        
        // JSONObject jsonObt = new JSONObject
        // {
            
        // };
        
        sessionManager.activeUser.suscriptionsLeft = (int)jsonObt.GetNumber("suscriptionsAvailables");
        sessionManager.SyncProfiles(sessionManager.activeUser.userkey);
        menuController.ShowLoading();
        yield return new WaitForSeconds(5f);
        sessionManager.activeKid = sessionManager.activeUser.kids[sessionManager.activeUser.kids.Count - 1];
        sessionManager.SyncChildLevels();
        sessionManager.SaveSession();
        menuController.ShowGameMenu();
        menuController.ClearInputs();

        // using (UnityWebRequest request = UnityWebRequest.Post(newKidURL, form))
        // {
        //     yield return request.SendWebRequest();

        //     JSONObject jsonObt = JSONObject.Parse(request.downloadHandler.text);

        //     if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        //     {
        //         Debug.Log("por alguna razon llegamos aqui en Send Web DEEBRIA QUITAR ESTA PART DE CONEXION");
        //         menuController.ShowWarning(9);
        //         menuController.AddKidShower();
        //     }
        //     else
        //     {
        //         sessionManager.activeUser.suscriptionsLeft = (int)jsonObt.GetNumber("suscriptionsAvailables");
        //         sessionManager.SyncProfiles(sessionManager.activeUser.userkey);
        //         menuController.ShowLoading();
        //         yield return new WaitForSeconds(5f);
        //         sessionManager.activeKid = sessionManager.activeUser.kids[sessionManager.activeUser.kids.Count - 1];
        //         sessionManager.SyncChildLevels();
        //         sessionManager.SaveSession();
        //         menuController.ShowGameMenu();
        //         menuController.ClearInputs();
        //     }
        // }
    }
}