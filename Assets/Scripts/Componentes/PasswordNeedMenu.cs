using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

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
        if (passwordField.text == currentPass)
        {
            action();
            passwordField.text = string.Empty;
        }
        else
        {
            needPassText.text = $"{badPassword}\n{messageNormal}";
            passwordField.text = string.Empty;
        }
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
