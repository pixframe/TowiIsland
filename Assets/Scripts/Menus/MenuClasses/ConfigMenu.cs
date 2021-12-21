using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfigMenu : BaseMenu
{
    public Button languageButton;
    public Button automaticButton;
    public Button backButton;
    public Button logoButton;
    public Button updateDataButton;
    public Button timeLimitButton;
    public TextMeshProUGUI versionText;

    public GameObject languagePanelHandler;
    public Button englishLanguageButton;
    public Button spanishLanguageButton;

    public GameObject changeConfigPanel;
    public GameObject timeLimitPanel;
    public TextMeshProUGUI timeLimitLabel;
    public TMP_Dropdown dropdwonTime;
    public Button saveButton;
    public TextMeshProUGUI timeAmountLabel;
    public Button plusButton;
    public Button lessButton;
    public Toggle toogleActivate;
    public TextMeshProUGUI textActivate;
    public PasswordNeedMenu passwordNeedMenu;

    public ConfigMenu(GameObject baseGameObject)
    {
        Initialization(baseGameObject);
        languageButton = gameObject.transform.Find("Language Button").GetComponent<Button>();
        backButton = gameObject.transform.Find("Back Button").GetComponent<Button>();
        logoButton = gameObject.transform.Find("Logo Button").GetComponent<Button>();
        updateDataButton = gameObject.transform.Find("Upadte Button").GetComponent<Button>();
        versionText = gameObject.transform.Find("Version Number Text").GetComponent<TextMeshProUGUI>();
        timeLimitButton = gameObject.transform.Find("Time Limit").GetComponent<Button>();

        languagePanelHandler = gameObject.transform.Find("Language Panel Handler").gameObject;
        englishLanguageButton = languagePanelHandler.transform.GetChild(0).GetComponent<Button>();
        spanishLanguageButton = languagePanelHandler.transform.GetChild(1).GetComponent<Button>();
        automaticButton = languagePanelHandler.transform.GetChild(languagePanelHandler.transform.childCount - 1).GetComponent<Button>();

        timeLimitPanel = gameObject.transform.Find("Time Limit Panel").gameObject;
        timeLimitLabel = timeLimitPanel.transform.Find("Time Limit Label").GetComponent<TextMeshProUGUI>();
        dropdwonTime = timeLimitPanel.transform.Find("Dropdown Time").GetComponent<TMP_Dropdown>();
        saveButton = timeLimitPanel.transform.Find("Save Time Config Button").GetComponent<Button>();
        timeAmountLabel = timeLimitPanel.transform.Find("Time Label").GetComponentInChildren<TextMeshProUGUI>();
        plusButton = timeLimitPanel.transform.Find("Plus Button").GetComponent<Button>();
        lessButton = timeLimitPanel.transform.Find("Less Button").GetComponent<Button>();
        toogleActivate = timeLimitPanel.transform.Find("Change Time Limit").GetComponent<Toggle>();
        textActivate = toogleActivate.GetComponentInChildren<TextMeshProUGUI>();

        passwordNeedMenu = new PasswordNeedMenu(timeLimitPanel.transform.Find("Pasword Need Component"));
    }
}
