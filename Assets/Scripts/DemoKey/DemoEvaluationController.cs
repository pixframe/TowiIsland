using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoEvaluationController : MonoBehaviour {

    public GameObject canvitas;
    public GameObject canvitas2;
    public GameObject canvitas3;
    public Dropdown droper;
    public InputField ageInput;
    public Dropdown dropi;
    public Button button3;
    public Text stringData;

    FlashProbes probes;
    int numberOfSceneLoaders;
    Button[] sceneLoaders;
    Text[] sceneNamesTexts;
    InputField nameOfKid;
    InputField ageOfKid;
    Button startTheTest;

	// Use this for initialization
	void Start () {
        probes = FindObjectOfType<FlashProbes>();
        probes.ResetTheScene();
        numberOfSceneLoaders = canvitas.transform.childCount;
        sceneLoaders = new Button[numberOfSceneLoaders];
        sceneNamesTexts = new Text[numberOfSceneLoaders];
        for (int i = 0; i < numberOfSceneLoaders; i++) {
            if (i == numberOfSceneLoaders-1)
            {
                sceneLoaders[i] = canvitas.transform.GetChild(i).GetComponent<Button>();
                sceneNamesTexts[i] = sceneLoaders[i].gameObject.transform.GetChild(0).GetComponent<Text>();
                sceneNamesTexts[i].text = "Go Back";
                sceneLoaders[i].onClick.AddListener(LoadTheScene);
                break;
            }
            sceneLoaders[i] = canvitas.transform.GetChild(i).GetComponent<Button>();
            string sceneToLoad = "Scene" + (i + 1).ToString();
            sceneLoaders[i].onClick.AddListener(() => LoadTheScene(sceneToLoad));
            sceneNamesTexts[i] = sceneLoaders[i].gameObject.transform.GetChild(0).GetComponent<Text>();
            sceneNamesTexts[i].text = "Scene " + (i + 1).ToString(); ;
        }
        droper.onValueChanged.AddListener(delegate { SetTheCanvas(); });
        nameOfKid = canvitas2.transform.GetChild(0).GetComponent<InputField>();
        ageOfKid = canvitas2.transform.GetChild(1).GetComponent<InputField>();
        startTheTest = canvitas2.transform.GetChild(2).GetComponent<Button>();
        startTheTest.onClick.AddListener(LoadFlashProbe);
        button3.onClick.AddListener(LoadTheWaitRoom);
        stringData.text = Application.persistentDataPath;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void GoBackMenu() {

    }

    void LoadTheScene()
    {
        PrefsKeys.SetNextScene("NewLogin");
        SceneManager.LoadScene("Loader_Scene");
    }

    void LoadTheScene(string nam)
    {
        SetAge();
        probes.DestryTheScript();
        PrefsKeys.SetNextScene("Evaluation_" + nam);
        SceneManager.LoadScene("Loader_Scene");
    }

    void LoadFlashProbe()
    {
        probes.ResetTheScene();
        SceneManager.LoadScene("Evaluation_Scene3");
    }

    void SetTheCanvas()
    {
        switch (droper.value)
        {
            case 0:
                canvitas.SetActive(true);
                canvitas2.SetActive(false);
                canvitas3.SetActive(false);
                break;
            case 1:
                canvitas.SetActive(false);
                canvitas2.SetActive(true);
                canvitas3.SetActive(false);
                break;
            case 2:
                canvitas.SetActive(false);
                canvitas2.SetActive(false);
                canvitas3.SetActive(true);
                break;
        }
    }

    int GetTheNumber()
    {
        return int.Parse(ageInput.text);
    }

    void SetAge()
    {
        EvaluationController eva = FindObjectOfType<EvaluationController>().GetComponent<EvaluationController>();
        eva.SetAge(GetTheNumber());
    }

    void LoadTheWaitRoom()
    {
        if (dropi.value == 0)
        {
            probes.DestryTheScript();
            SceneManager.LoadScene("Evaluation_Scene4");
        }
        else
        {
            SceneManager.LoadScene("Evaluation_Scene4");
        }
    }
}
