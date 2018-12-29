using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadManager : MonoBehaviour {

    AsyncOperation asyncLoad;

    public Image KiwiLoading;
    public Text loadText;

    string[] stringsToShow;

    // Use this for initialization
    void Start ()
    {
        stringsToShow = TextReader.TextsToShow(Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Menus/Loading"));
        loadText.text = $"{stringsToShow[0]}...";
        StartCoroutine(LoadTheNextScene());
	}
	
	// Update is called once per frame
	void Update () {
        if (asyncLoad != null) {
            KiwiLoading.fillAmount = asyncLoad.progress;
            if (asyncLoad.progress >= 0.9f) {
                asyncLoad.allowSceneActivation = true;
            }
        }
	}

    IEnumerator LoadTheNextScene()
    {
        asyncLoad = SceneManager.LoadSceneAsync(PrefsKeys.GetNextScene());
        asyncLoad.allowSceneActivation = false;
        yield return asyncLoad;
    }
}
    