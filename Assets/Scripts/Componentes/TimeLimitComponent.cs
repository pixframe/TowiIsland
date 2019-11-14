using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TimeLimitComponent : MonoBehaviour
{
    TextMeshProUGUI timeText;
    Button continueButton;
    Button outButton;
    PasswordNeedMenu passwordNeedMenu;
    SessionManager sessionManager;

    private void Start()
    {
        Time.timeScale = 0;
        var safeArea = transform.Find("Safe Area");
        continueButton = safeArea.Find("Continue Button").GetComponent<Button>();
        continueButton.onClick.AddListener(NeedToResume);
        outButton = safeArea.Find("Out Button").GetComponent<Button>();
        outButton.onClick.AddListener(()=>GoToSomeScene("GameCenter"));
        passwordNeedMenu = new PasswordNeedMenu(safeArea.Find("Pasword Need Component"));
        sessionManager = FindObjectOfType<SessionManager>();
    }


    void GoToSomeScene(string text) 
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(text);
    }

    void NeedToResume() 
    {
        passwordNeedMenu.panel.gameObject.SetActive(true);

        passwordNeedMenu.SendPass(ResumeTheGame, sessionManager.activeUser.psswdHash);
    }

    void ResumeTheGame() 
    {
        Time.timeScale = 1;
        Destroy(this.gameObject);
    }
}
