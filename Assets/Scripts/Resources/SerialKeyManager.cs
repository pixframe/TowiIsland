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

    // public TMP_InputField newKidNameInput;
    // public TMP_InputField newKidDay;
    // public TMP_InputField newKidMonth;
    // public TMP_InputField newKidYear;

    int quitScene;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(fileLines.Count);
        Debug.Log(userLines.Count);
    }


    public void ValidateSerial()
    {

    }

    private void Awake() {
        string readFromFilePath = Application.streamingAssetsPath +"/"+ "serialKeys"+".txt";
        fileLines = File.ReadAllLines(readFromFilePath).ToList();
        string userPath = Application.streamingAssetsPath +"/"+ "users"+".txt";
        userLines = File.ReadAllLines(userPath).ToList();
        // if(quitScene == 1)
        // {
        //     SceneManager.LoadScene("NewLogin", LoadSceneMode.Single);
        // }
    }
    public void ReadString()
    {
        bool warningUser = false;
        bool warningSerial = false;
        bool userCorrect = false;
        bool serialCorrect = false;
        userTemp = userInput.text;
        serialTemp = serialInput.text;
        Debug.Log(serialTemp);
        Debug.Log(userTemp);
        //string readFromFilePath = Application.streamingAssetsPath +"/"+ "text"+".txt";
        
        
        //Debug.Log(fileLines);

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
            }
            else
            {
                warningUser = true;
                Debug.Log("NO es un usuario correcto");  
            }    
        //Read the text from directly from the test.txt file
        //StreamReader reader = new StreamReader(readFromFilePath);
        //Debug.Log(reader.ReadToEnd());
        //reader.Close();
        if(warningSerial || warningUser)
        {
            warningPanel.SetActive(true);
        }
        if(userCorrect && serialCorrect)
        {
            //SceneManager.LoadScene("NewLogin", LoadSceneMode.Single);
            PlayerPrefs.SetInt("quitScene", 1);
            serialPanel.SetActive(false);
            newKidPanel.SetActive(true);
            ClearInputs();
        }
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

    // void CreateAKid()
    // {
    //     if (newKidNameInput.text != "" && newKidDay.text != "" && newKidMonth.text != "" && newKidYear.text != "")
    //     {
    //         if (KidDateIsOK(1))
    //         {
    //             // string dob = DefineTheDateOfBirth(1);
    //             // string nameKid = newKidNameInput.text;
    //             // int id = sessionManager.activeUser.id;
    //             // ShowLoading();
    //             // logInScript.RegisterAKid(dob, nameKid, id);
    //         }
    //         else
    //         {
    //             //ShowWarning(12);
    //         }
    //     }
    //     else
    //     {
    //         //ShowWarning(4);
    //     }
    // }

    // bool KidDateIsOK(int typeOfKid)
    // {
    //     int year = 0;
    //     int month = 0;
    //     int day = 0;
    //     if (typeOfKid == 0)
    //     {
    //         year = int.Parse(registerMenu.yearInput.text);
    //         month = int.Parse(registerMenu.monthInput.text);
    //         day = int.Parse(registerMenu.dayInput.text);
    //     }
    //     if (typeOfKid != 0)
    //     {
    //         year = int.Parse(newKidYear.text);
    //         month = int.Parse(newKidMonth.text);
    //         day = int.Parse(newKidDay.text);
    //     }

    //     List<int> months1 = new List<int> { 1, 3, 5, 7, 8, 10, 12 };
    //     List<int> months2 = new List<int> { 4, 6, 9, 11 };

    //     if (year > 999 && day > 0 && month > 0 && month < 13)
    //     {
    //         if (months1.Contains(month) && day < 32 || months2.Contains(month) && day < 31
    //             || year % 4 == 0 && month == 2 && day < 30 || year % 4 != 0 && month == 2 && day < 29)
    //         {
    //             dobYMD = new int[] { year, month, day };
    //             return true;
    //         }
    //         else
    //         {
    //             return false;
    //         }
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }
}
