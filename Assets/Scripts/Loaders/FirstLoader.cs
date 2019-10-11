using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class FirstLoader : MonoBehaviour {

    AsyncOperation asyncLoad;

    public Image KiwiLoading;
    public TextMeshProUGUI loadText;

    string[] stringsToShow;

    // Use this for initialization
    void Start()
    {
        stringsToShow = TextReader.TextsToShow(Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Menus/Loading"));
        loadText.text = $"{stringsToShow[0]}...";
        StartCoroutine(LoadTheNextScene());
    }

    // Update is called once per frame
    void Update()
    {
        if (asyncLoad != null)
        {
            KiwiLoading.fillAmount = asyncLoad.progress;
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
        }
    }

    IEnumerator LoadTheNextScene()
    {
        PlayerPrefs.SetInt(Keys.Logged_Session, 0);
        asyncLoad = SceneManager.LoadSceneAsync("NewLogin");
        asyncLoad.allowSceneActivation = false;
        yield return asyncLoad;
    }

}
