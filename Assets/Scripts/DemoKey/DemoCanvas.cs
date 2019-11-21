using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DemoCanvas : MonoBehaviour {

    DemoKey key;
    Button pauseButton;
    Button returnGame;
    Button goBackMenu;
    GameObject pausePanel;

    // Use this for initialization
    void Start() {
        pauseButton = transform.GetChild(0).GetComponent<Button>();
        pausePanel = transform.GetChild(1).gameObject;
        returnGame = pausePanel.transform.GetChild(0).GetComponent<Button>();
        goBackMenu = pausePanel.transform.GetChild(1).GetComponent<Button>();
        pauseButton.onClick.AddListener(EnableMenu);
        returnGame.onClick.AddListener(ReturnGame);
        goBackMenu.onClick.AddListener(ReturnToMenu);
        pausePanel.SetActive(false);
        key = FindObjectOfType<DemoKey>();
        if (key == null) {
            Destroy(this.gameObject);
        }
        var texts = TextReader.TextsToSet("Components/PauseEvaluation");
        returnGame.GetComponentInChildren<TextMeshProUGUI>().text = texts[0];
        goBackMenu.GetComponentInChildren<TextMeshProUGUI>().text = texts[1];
	}

    void EnableMenu() {
        pauseButton.gameObject.SetActive(false);
        pausePanel.SetActive(true);
    }

    void ReturnGame() {
        pauseButton.gameObject.SetActive(true);
        pausePanel.SetActive(false);
    }

    void ReturnToMenu() {
        SceneManager.LoadScene("DemoEvaluationLoader");
    }
}
