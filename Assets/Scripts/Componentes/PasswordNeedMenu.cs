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

    readonly string messageNormal;
    readonly string badPassword;

    public PasswordNeedMenu(Transform panel) 
    {
        this.panel = panel;
        var text = TextReader.TextsToSet("Components/Password_Need");
        messageNormal = text[0];
        badPassword = text[3];
        needPassText = this.panel.Find("Message Config").GetComponent<TextMeshProUGUI>();
        needPassText.text = messageNormal;
        sendPassButton = this.panel.Find("SendPassButton").GetComponent<Button>();
        sendPassButton.GetComponentInChildren<TextMeshProUGUI>().text = text[2];
        passwordField = this.panel.Find("Password Field").GetComponent<TMP_InputField>();
        passwordField.placeholder.GetComponent<TextMeshProUGUI>().text = text[1];
        passwordField.inputType = TMP_InputField.InputType.Password;
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
}
