using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class DemoEvaluationController : MonoBehaviour 
{
    int index = 0;
    DemoPanel demoPanel;
    [SerializeField]GameObject demoPanelObject;

    DemoGamePanel beforePanel;
    DemoGamePanel mainPanel;
    DemoGamePanel nextPanel;

    [SerializeField] GameObject[] panels;
    [SerializeField] Button leftButton;
    [SerializeField] Button rightButton;
    [SerializeField] Button backButton;

    [SerializeField]List<Sprite> icons;
    [SerializeField]List<Sprite> captures;
    List<string> colorStringsPanel = new List<string> { "5552CB", "1263C0", "363636", "959596", "3B4A6C", "9C4700", "44B3C2" };
    List<string> colorStringsLine = new List<string> { "FBB03B", "00FFFF", "00FFFF", "CCDD6C", "74DF66", "F67E7D", "4D2400" };
    string[] nameOfScene;

    enum DirectionOfSwipe { Left, Right, None };
    DirectionOfSwipe directtion = DirectionOfSwipe.None;

    // Use this for initialization
    void Start () 
    {
        beforePanel = new DemoGamePanel(panels[0]);
        mainPanel = new DemoGamePanel(panels[1]);
        nextPanel = new DemoGamePanel(panels[2]);
        nameOfScene = TextReader.TextsToSet("Components/DemoEvaluationCenter");

        demoPanel = new DemoPanel(demoPanelObject.transform, false);

        beforePanel.button.onClick.AddListener(() => ChangeMenus(DirectionOfSwipe.Left));
        nextPanel.button.onClick.AddListener(() => ChangeMenus(DirectionOfSwipe.Right));

        leftButton.onClick.AddListener(() => ChangeMenus(DirectionOfSwipe.Left));
        rightButton.onClick.AddListener(() => ChangeMenus(DirectionOfSwipe.Right));
        backButton.onClick.AddListener(GoBack);
        ChangeMenus();

        mainPanel.playButton.onClick.AddListener(GoToScene);
        mainPanel.playButton.GetComponentInChildren<TextMeshProUGUI>().text = nameOfScene[7];
    }

    public void GoBack() 
    {
        SceneManager.LoadScene("NewLogin");
    }

    public void GoToScene()
    {
        Debug.Log("open demo panel");
        demoPanel.ShowDemoPanel(GoToScene);
    }

    public void GoToScene(DemoPanel.Difficulty difficulty) 
    {
        EvaluationController evaluationController = GetComponent<EvaluationController>();
        switch (difficulty)
        {
            case DemoPanel.Difficulty.Easy:
                evaluationController.SetAge(6);
                break;
            case DemoPanel.Difficulty.Normal:
                evaluationController.SetAge(8);
                break;
            case DemoPanel.Difficulty.Hard:
                evaluationController.SetAge(10);
                break;
        }
        PrefsKeys.SetNextScene($"Evaluation_Scene{index + 1}");
        SceneManager.LoadScene("Loader_Scene");
    }

    public void ChangeMenus()
    {
        if (directtion == DirectionOfSwipe.Right)
        {
            index++;
        }
        else if (directtion == DirectionOfSwipe.Left)
        {
            index--;
        }

        if (index >= 7)
        {
            index = 0;
        }

        if (index == -1)
        {
            index += 7;
        }

        int previous = index - 1;

        if (previous < 0)
        {
            previous += 7;
        }

        int next = index + 1;
        if (next >= 7)
        {
            next = 0;
        }

        SetPanel(mainPanel, index, true);
        SetPanel(nextPanel, next);
        SetPanel(beforePanel, previous);
    }

    void ChangeMenus(DirectionOfSwipe dir)
    {
        if (dir == DirectionOfSwipe.Right)
        {
            index++;
        }
        else if (dir == DirectionOfSwipe.Left)
        {
            index--;
        }

        int count = Keys.Number_Of_Evaluation_Stages;

        if (index >= count)
        {
            index = 0;
        }

        if (index == -1)
        {
            index += count;
        }

        int previous = index - 1;

        if (previous == -1)
        {
            previous += count;
        }

        int next = index + 1;
        if (next >= count)
        {
            next = 0;
        }

        SetPanel(mainPanel, index, true);
        SetPanel(nextPanel, next);
        SetPanel(beforePanel, previous);
    }

    void SetPanel(DemoGamePanel panel, int number)
    {
        SetPanel(panel, number, false);
    }

    void SetPanel(DemoGamePanel panel, int number, bool isCenter)
    {
        ColorUtility.TryParseHtmlString($"#{colorStringsPanel[number]}", out Color color);
        panel.backgroundPanel.color = color;

        ColorUtility.TryParseHtmlString($"#{colorStringsLine[number]}", out Color colorLine);
        panel.barColor.color = colorLine;

        panel.iconImage.sprite = icons[number]; 

        panel.captureImage.sprite = captures[number];

        panel.gameText.text = nameOfScene[number];

        if (!isCenter) 
        {
            panel.playButton.gameObject.SetActive(false);
        }
    }

    public struct DemoGamePanel
    {
        public DemoGamePanel(GameObject gamePanel)
        {
            gameObject = gamePanel;
            button = gameObject.GetComponent<Button>();
            backgroundPanel = gamePanel.GetComponent<Image>();
            gameText = gamePanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            barColor = gamePanel.transform.GetChild(2).GetComponent<Image>();
            iconImage = gamePanel.transform.GetChild(3).GetComponent<Image>();
            captureImage = gamePanel.transform.GetChild(4).GetComponent<Image>();
            playButton = gamePanel.transform.GetChild(5).GetComponent<Button>();
        }

        public GameObject gameObject;
        public Button button;
        public Image backgroundPanel;
        public TextMeshProUGUI gameText;
        public Image barColor;
        public Image iconImage;
        public Image captureImage;
        public Button playButton;

    }
}
