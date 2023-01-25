using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.IO;
using System.Linq;

public class PasswordNeedMenu
{
    public Transform panel;
    public TextMeshProUGUI needPassText;
    public TMP_InputField passwordField;
    public Button sendPassButton;

    string messageNormal;
    string badPassword;

    public PasswordNeedMenu(Transform panel) 
    {
        this.panel = panel;
        needPassText = this.panel.Find("Message Config").GetComponent<TextMeshProUGUI>();
        sendPassButton = this.panel.Find("SendPassButton").GetComponent<Button>();
        passwordField = this.panel.Find("Password Field").GetComponent<TMP_InputField>();
        passwordField.inputType = TMP_InputField.InputType.Password;

        SetTexts();
    }

    public void SendPass(UnityAction action, string currentPass) 
    {
        sendPassButton.onClick.RemoveAllListeners();
        sendPassButton.onClick.AddListener(() => DoTheAction(action, currentPass));
    }

    void DoTheAction(UnityAction action, string currentPass) 
    {
        string serialTemp;
        string userTemp;

        List<string> fileLines;
        List<string> userLines;
        string readFromFilePath = Application.streamingAssetsPath +"/"+ "serialKeys"+".txt";
        fileLines = File.ReadAllLines(readFromFilePath).ToList();
        string userPath = Application.streamingAssetsPath +"/"+ "users"+".txt";
        userLines = File.ReadAllLines(userPath).ToList();

        bool warningUser = false;
        bool warningSerial = false;
        bool userCorrect = false;
        bool serialCorrect = false;

        userTemp = "hola";
        serialTemp = passwordField.text;

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
        //Read the text from directly from the test.txt file
        //StreamReader reader = new StreamReader(readFromFilePath);
        //Debug.Log(reader.ReadToEnd());
        //reader.Close();
        if(warningSerial || warningUser)
        {
            needPassText.text = $"{badPassword}\n{messageNormal}";
            passwordField.text = string.Empty;
            //warningPanel.SetActive(true);
        }
        if(serialCorrect)
        {
            action();
            //SceneManager.LoadScene("NewLogin", LoadSceneMode.Single);
            PlayerPrefs.SetInt("quitScene", 1);
            passwordField.text = "";
        }
   

        // if (passwordField.text == currentPass)
        // {
        //     action();
        //     passwordField.text = string.Empty;
        // }
        // else
        // {
        //     needPassText.text = $"{badPassword}\n{messageNormal}";
        //     passwordField.text = string.Empty;
        // }
    }

    public void SetTexts() 
    {
        var text = TextReader.TextsToSet("Components/Password_Need");
        messageNormal = text[0];
        passwordField.placeholder.GetComponent<TextMeshProUGUI>().text = text[1];
        sendPassButton.GetComponentInChildren<TextMeshProUGUI>().text = text[2];
        badPassword = text[3];
        needPassText.text = messageNormal;
    }
}
