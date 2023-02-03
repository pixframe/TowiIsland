using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

public class SerialKeyManager : MonoBehaviour
{
    string serialTemp;
    string userTemp;
    public TMP_InputField serialInput;
    public TMP_InputField userInput;
    List<string> fileLines;
    List<string> userLines;
    public GameObject warningPanel;
    public GameObject serialPanel;
    public GameObject newKidPanel;
    public GameObject GameMenuPanel;

    //List<string> keysUsed = new List<string>();
    int keyNumber = 0;
    string[] keyUsed = new string[11];

    // public TMP_InputField newKidNameInput;
    // public TMP_InputField newKidDay;
    // public TMP_InputField newKidMonth;
    // public TMP_InputField newKidYear;

    int quitScene;

    // Start is called before the first frame update



    public void ValidateSerial()
    {

    }

    private void Awake() {
        // string readFromFilePath = Application.streamingAssetsPath +"/"+ "serialKeys"+".txt";
        // fileLines = File.ReadAllLines(readFromFilePath).ToList();
        // string userPath = Application.streamingAssetsPath +"/"+ "users"+".txt";
        // userLines = File.ReadAllLines(userPath).ToList();
        // if(quitScene == 1)
        // {
        //     SceneManager.LoadScene("NewLogin", LoadSceneMode.Single);
        // }
        
    }
    
    void Start() {

        if(PlayerPrefs.HasKey("nKey"))
        {
            keyNumber = PlayerPrefs.GetInt("nKey");
        }
        else
        {
            PlayerPrefs.SetInt("nKey", 1);
        }
        
        if(PlayerPrefs.HasKey("Key01"))
        {
            keyUsed[1]=PlayerPrefs.GetString("Key01");
        }
        else
        {
            PlayerPrefs.SetString("Key01", null);
            
        }
        
        if(PlayerPrefs.HasKey("Key02"))
        {
            keyUsed[2]=PlayerPrefs.GetString("Key02");

        }
        else
        {
            PlayerPrefs.SetString("Key02", null);
        }
        if(PlayerPrefs.HasKey("Key03"))
        {
            keyUsed[3]=PlayerPrefs.GetString("Key03");
        }
        else
        {
            PlayerPrefs.SetString("Key03", null);
        }
        
        if(PlayerPrefs.HasKey("Key04"))
        {
            keyUsed[4]=PlayerPrefs.GetString("Key04");
        }
        else
        {
            PlayerPrefs.SetString("Key04", null);
        }
        if(PlayerPrefs.HasKey("Key05"))
        {
            keyUsed[5]=PlayerPrefs.GetString("Key05");
        }
        else
        {
            PlayerPrefs.SetString("Key05", null);
        }
        if(PlayerPrefs.HasKey("Key06"))
        {
            keyUsed[6]=PlayerPrefs.GetString("Key06");
        }
        else
        {
            PlayerPrefs.SetString("Key06", null);
        }
        if(PlayerPrefs.HasKey("Key07"))
        {
            keyUsed[7]=PlayerPrefs.GetString("Key07");
        }
        else
        {
            PlayerPrefs.SetString("Key07", null);
        }
        if(PlayerPrefs.HasKey("Key08"))
        {
            keyUsed[8]=PlayerPrefs.GetString("Key08");
        }
        else
        {
            PlayerPrefs.SetString("Key08", null);
        }
        if(PlayerPrefs.HasKey("Key09"))
        {
            keyUsed[9]=PlayerPrefs.GetString("Key09");
        }
        else
        {
            PlayerPrefs.SetString("Key09", null);
        }
        if(PlayerPrefs.HasKey("Key10"))
        {
            keyUsed[10]=PlayerPrefs.GetString("Key10");
        }
        else
        {
            PlayerPrefs.SetString("Key10", "null");
        }

        keyUsed[0]=null;
     
    }
    public void ReadString()
    {
        Debug.Log("Contraseñas usadas " + keyNumber);
        
        
        for(int i=0;i<keyUsed.Length;i++)
        {
            Debug.Log(keyUsed[i]);
        }
        bool warningUser = false;
        bool warningSerial = false;
        bool userCorrect = false;
        bool serialCorrect = false;
        userTemp = userInput.text;
        serialTemp = serialInput.text;
        Debug.Log(userTemp);
        Debug.Log(serialTemp);
        
        

        //string readFromFilePath = Application.streamingAssetsPath +"/"+ "text"+".txt";
        
        string[] userArr = new string[1500000];
        string[] serialArr = new string[1500000];
        
        TextAsset usersList = (TextAsset)Resources.Load("users", typeof (TextAsset));
        string usersContent = usersList.text;
        
        TextAsset serialsList = (TextAsset)Resources.Load("serialKeys", typeof (TextAsset));
        string serialsContent = serialsList.text;


        userArr= usersContent.Split(char.Parse("\n")); 
        serialArr= serialsContent.Split(char.Parse("\n")); 
        for (int i = 0; i<userArr.Length; i++)
        {
            userArr[i] = userArr[i].TrimEnd();
        }
        for (int i = 0; i<serialArr.Length; i++)
        {
            serialArr[i] = serialArr[i].TrimEnd();
        }

        if(userTemp == null || serialTemp == null || userTemp == "" || serialTemp == "" || keyUsed.Contains(serialTemp) )
        {
            warningSerial = true;
            warningUser = true;
            Debug.Log("Ya fue usado"); 
            warningPanel.SetActive(true);
            newKidPanel.SetActive(false); 
        }
        else 
        {
            
            if(serialArr.Contains(serialTemp))
            {
                serialCorrect = true;
                Debug.Log("SI es un serial correcto");
            }
            else
            {
                warningSerial = true;
                Debug.Log("NO es un serial correcto");  
            }


            if(userArr.Contains(userTemp))
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
                warningPanel.SetActive(true);
                newKidPanel.SetActive(false);   
                //warningPanel.SetActive(true);
            }
            if((userCorrect==true) && (serialCorrect==true))
            {

                ClearInputs();
                PlayerPrefs.SetInt("quitScene", 1);
                serialPanel.SetActive(false);
                newKidPanel.SetActive(true);
                
                //keysUsed.Add(serialTemp);
                keyNumber += 1;
                PlayerPrefs.SetInt("nKey", keyNumber);
                string temp = SetKey(keyNumber);
                PlayerPrefs.SetString(temp, serialTemp);  
                // SceneManager.LoadScene("NewLogin", LoadSceneMode.Single);
                // PlayerPrefs.SetInt("quitScene", 1);
            }
        }
        
        
        
   }


    public string SetKey(int keyNumber)
    {
        string keyCode;
        switch(keyNumber)
        {
            case 0:

            break;
            case 1:
            keyCode = "Key01";
            return keyCode;
            //break;
            
            case 2:
            keyCode = "Key02";
            return keyCode;
            
            case 3:
            keyCode = "Key03";
            return keyCode;
            
            case 4:
            keyCode = "Key04";
            return keyCode;
            
            case 5:
            keyCode = "Key05";
            return keyCode;
            
            case 6:
            keyCode = "Key06";
            return keyCode;
            
            case 7:
            keyCode = "Key07";
            return keyCode;
            
            case 8:
            keyCode = "Key08";
            return keyCode;
            
            case 9:
            keyCode = "Key09";
            return keyCode;
            
            case 10:
            keyCode = "Key10";
            return keyCode;
        }
        return null;
    }
    public void ClearInputs()
    {
        serialInput.text = "";
        userInput.text = "";
        
    }
    public void CloseWarning()
    {
        warningPanel.SetActive(false);
    }

    public void Close()
    {
        GameMenuPanel.SetActive(true);
        newKidPanel.SetActive(false);
        serialPanel.SetActive(false);   
    }

    
}
