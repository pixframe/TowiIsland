using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSelectorsController : MonoBehaviour {

    //UI objects needed
    public Image circleOfGame;
    public Text titleText;
    public Text descriptionText;
    public GameObject infoPanel;
    public Button backButton;
    public Button cancelButton;
    public Button readyButton;

    public Sprite[] iconsToShow;

    public TextAsset textAsset;
    public TextAsset gamesTextAsset;
    string[] stringsToShow;
    string[] gamesStrings;

    AsyncOperation asyncLoad;
	// Use this for initialization
	void Start () {
        stringsToShow = TextReader.TextsToShow(textAsset);
        gamesStrings = TextReader.TextsToShow(gamesTextAsset);
        cancelButton.onClick.AddListener(HideShowDescription);
        HideShowDescription();
        StartCoroutine(LoadLoader());
	}

    //this will show a description of the game seleceted in trhe island
    public void ShowAGameDescription(GameObject selectedPlace) {
        int index = selectedPlace.transform.GetSiblingIndex();
        infoPanel.SetActive(true);
        circleOfGame.sprite = iconsToShow[index];
        int stringIndex = index * 2;
        titleText.text = gamesStrings[stringIndex];
        descriptionText.text = gamesStrings[stringIndex + 1];
    }

    //this will hide the game before selected
    void HideShowDescription() {
        infoPanel.SetActive(false);
    }

    //this will let to finish the load of the next scene
    void MoveToNextScene() {
        asyncLoad.allowSceneActivation = true;
    }

    //this will set the scene to load in the next scene
    public void CallTheGameByName(string sceneName) {
        PrefsKeys.SetNextScene(sceneName);
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(MoveToNextScene);
    }

    //this will load the next scene fast enough to ren fast and smooth the game
    IEnumerator LoadLoader() {
        asyncLoad = SceneManager.LoadSceneAsync("Loader_Scene");
        asyncLoad.allowSceneActivation = false;
        yield return asyncLoad;
    }
}
