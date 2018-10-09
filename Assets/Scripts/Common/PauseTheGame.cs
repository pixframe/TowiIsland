using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseTheGame : MonoBehaviour {

    Button pauseButton;
    GameObject pausePanel;
    GameObject miniPausePanel;
    Text pauseText;
    Button goBackButton;
    Button goMenuButton;
    Button cancelButton;
    [System.NonSerialized]
    public Button howToPlayButton;
    [System.NonSerialized]
    public Button playButton;

    [System.NonSerialized]
    public bool needTutorial;

    TextAsset textAsset;
    string[] stringsToShow;

	// Use this for initialization
	void Awake ()
    {
        textAsset = Resources.Load<TextAsset>("StringsToShow/Menus/PauseMenu");

        stringsToShow = TextReader.TextsToShow(textAsset);

        pauseButton = transform.GetChild(0).GetComponent<Button>();
        pausePanel = transform.GetChild(1).gameObject;

        miniPausePanel = pausePanel.transform.GetChild(0).gameObject;
        pauseText = miniPausePanel.transform.GetChild(0).GetComponent<Text>();
        goBackButton = miniPausePanel.transform.GetChild(1).GetComponent<Button>();
        goMenuButton = miniPausePanel.transform.GetChild(2).GetComponent<Button>();
        cancelButton = miniPausePanel.transform.GetChild(3).GetComponent<Button>();

        pauseText.text = stringsToShow[0];
        goBackButton.GetComponentInChildren<Text>().text = stringsToShow[1];
        goMenuButton.GetComponentInChildren<Text>().text = stringsToShow[2];

        howToPlayButton = transform.GetChild(2).GetComponent<Button>();
        playButton = transform.GetChild(3).GetComponent<Button>();

        pauseButton.onClick.AddListener(PauseButton);
        goMenuButton.onClick.AddListener(GoBackIsland);
        goBackButton.onClick.AddListener(GoBackGame);
        cancelButton.onClick.AddListener(GoBackGame);
        howToPlayButton.onClick.AddListener(HideTutorialButtons);
        playButton.onClick.AddListener(HideTutorialButtons);
    }

    void PauseButton()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
        }
        pauseButton.gameObject.SetActive(false);
        pausePanel.SetActive(true);
    }

    void GoBackGame()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        pauseButton.gameObject.SetActive(true);
        pausePanel.SetActive(false);
    }

    void GoBackIsland()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        SceneManager.LoadScene("GameCenter");
    }

    public void WantTutorial()
    {
        pauseButton.gameObject.SetActive(false);
        pausePanel.SetActive(false);
        howToPlayButton.gameObject.SetActive(true);
        playButton.gameObject.SetActive(true);
    }

    void HideTutorialButtons()
    {
        pauseButton.gameObject.SetActive(true);
        pausePanel.SetActive(false);
        howToPlayButton.gameObject.SetActive(false);
        playButton.gameObject.SetActive(false);
    }
}
