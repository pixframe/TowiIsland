using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseTheGame : MonoBehaviour {

    Button pauseButton;
    GameObject pausePanel;
    GameObject miniPausePanel;
    GameObject dontTouchPanel;
    GameObject loadingPanel;
    Text pauseText;
    Button goBackButton;
    Button goMenuButton;
    Button cancelButton;
    KiwiEarningPanel kiwiEarningPanel;
    [System.NonSerialized]
    public Button howToPlayButton;
    [System.NonSerialized]
    public Button playButton;

    [System.NonSerialized]
    public bool needTutorial;

    bool isDataSend = false;

    TextAsset textAsset;
    string[] stringsToShow;

	// Use this for initialization
	void Awake ()
    {
        textAsset = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Menus/PauseMenu");

        stringsToShow = TextReader.TextsToShow(textAsset);

        dontTouchPanel = transform.parent.GetChild(0).gameObject;

        pauseButton = transform.GetChild(0).GetComponent<Button>();
        pausePanel = transform.GetChild(1).gameObject;

        miniPausePanel = pausePanel.transform.GetChild(0).gameObject;
        pauseText = miniPausePanel.transform.GetChild(0).GetComponentInChildren<Text>();
        goBackButton = miniPausePanel.transform.GetChild(1).GetComponent<Button>();
        goMenuButton = miniPausePanel.transform.GetChild(2).GetComponent<Button>();
        cancelButton = miniPausePanel.transform.GetChild(3).GetComponent<Button>();

        howToPlayButton = transform.GetChild(2).GetComponent<Button>();
        playButton = transform.GetChild(3).GetComponent<Button>();
        kiwiEarningPanel = new KiwiEarningPanel(transform.GetChild(4).gameObject);
        loadingPanel = transform.GetChild(5).gameObject;

        pauseText.text = stringsToShow[0];
        goBackButton.GetComponentInChildren<Text>().text = stringsToShow[1];
        kiwiEarningPanel.continueButton.GetComponentInChildren<Text>().text = stringsToShow[1];
        goMenuButton.GetComponentInChildren<Text>().text = stringsToShow[2];
        playButton.GetComponentInChildren<Text>().text = stringsToShow[3];
        howToPlayButton.GetComponentInChildren<Text>().text = stringsToShow[4];
        pauseButton.onClick.AddListener(PauseButton);
        goMenuButton.onClick.AddListener(GoBackIsland);
        goBackButton.onClick.AddListener(GoBackGame);
        cancelButton.onClick.AddListener(GoBackGame);
        howToPlayButton.onClick.AddListener(HideTutorialButtons);
        playButton.onClick.AddListener(HideTutorialButtons);
        HideAllPanels();
    }

    void PauseButton()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
        }
        pauseButton.gameObject.SetActive(false);
        pausePanel.SetActive(true);
        dontTouchPanel.SetActive(true);
    }

    void GoBackGame()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        pauseButton.gameObject.SetActive(true);
        pausePanel.SetActive(false);
        dontTouchPanel.SetActive(false);
    }

    void GoBackIsland()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }

        SendInmiediatlyToIsland();
    }

    void HideAllPanels()
    {
        pauseButton.gameObject.SetActive(false);
        pausePanel.SetActive(false);
        dontTouchPanel.SetActive(false);
        howToPlayButton.gameObject.SetActive(false);
        playButton.gameObject.SetActive(false);
        kiwiEarningPanel.mainPanel.SetActive(false);
        loadingPanel.SetActive(false);
    }

    public void WantTutorial()
    {
        HideAllPanels();
        pauseButton.gameObject.SetActive(false);
        howToPlayButton.gameObject.SetActive(true);
        playButton.gameObject.SetActive(true);
    }

    public void HideTutorialButtons()
    {
        HideAllPanels();
        pauseButton.gameObject.SetActive(true);
    }

    public void ShowKiwiEarnings(int kiwisEarn)
    {
        HideAllPanels();
        kiwiEarningPanel.mainPanel.SetActive(true);

        if (kiwisEarn < 1)
        {
            kiwiEarningPanel.kiwiText.text = TextReader.beforeStrings[0];
        }
        else
        {
            kiwiEarningPanel.kiwiText.text = TextReader.commonStrings[11];
        }
        kiwiEarningPanel.amoutOfKiwisEarnText.text = "X " + kiwisEarn;
        if (PlayerPrefs.GetInt(Keys.First_Try) == 0)
        {
            kiwiEarningPanel.continueButton.onClick.AddListener(()=>
            {
                PrefsKeys.SetNextScene("NewLogin");
                SceneManager.LoadScene("Loader_Scene");
                PlayerPrefs.SetInt(Keys.First_Try, 1);
            });
        }
        else
        {
            kiwiEarningPanel.continueButton.onClick.AddListener(ReturnHome);
        }
    }

    void ShowLoadingScreen()
    {
        HideAllPanels();
        loadingPanel.SetActive(true);
    }

    public void DataIsSend()
    {
        isDataSend = true;
    }

    void ReturnHome()
    {
        ShowLoadingScreen();
        StartCoroutine(GoToTheGameCenterWhenDataIsSend());
    }

    IEnumerator GoToTheGameCenterWhenDataIsSend()
    {
        yield return new WaitUntil(() => isDataSend == true);

        SendInmiediatlyToIsland();
    }

    void SendInmiediatlyToIsland()
    {
        PrefsKeys.SetNextScene("GameCenter");
        SceneManager.LoadScene("Loader_Scene");
    }
}

struct KiwiEarningPanel
{
    public GameObject mainPanel;
    public Text kiwiText;
    public Text amoutOfKiwisEarnText;
    public Button continueButton;

    public KiwiEarningPanel(GameObject panel)
    {
        mainPanel = panel;
        kiwiText = panel.transform.GetChild(0).GetComponent<Text>();
        amoutOfKiwisEarnText = panel.transform.GetChild(1).GetComponent<Text>();
        continueButton = panel.transform.GetChild(2).GetComponent<Button>();
    }
}
