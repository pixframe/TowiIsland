using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameMenu : BaseMenu
{
    MenuManager manager;
    public GameObject tryLogo;
    public GameObject logoIcon;
    public Button gamesButton;
    public Button evaluationButton;
    public KidProfileCanvas kidProfile;
    Button singOutButton;
    Button settingsButton;
    Button evalButton;
    Button aboutButton;
#if UNITY_WEBGL
    Transform buyButton;
#endif

    const string pathOfFirstMenu = "Login/FirstMenu";

    public GameMenu(GameObject panel, MenuManager managerToRelay)
    {
        manager = managerToRelay;
        gameObject = panel;
        tryLogo = gameObject.transform.Find("Try Logo").gameObject;
        logoIcon = gameObject.transform.Find("Game Logo").gameObject;
        gamesButton = gameObject.transform.Find("Games Button").GetComponent<Button>();
        evaluationButton = gameObject.transform.Find("Evaluation Button").GetComponent<Button>();
        kidProfile = new KidProfileCanvas(gameObject.transform.Find("Change Kid Profile Button").gameObject);
        singOutButton = gameObject.transform.Find("Sing Out Button").GetComponent<Button>();
        settingsButton = gameObject.transform.Find("Settings Button").GetComponent<Button>();
        evalButton = gameObject.transform.Find("Eval Button").GetComponent<Button>();
        aboutButton = gameObject.transform.Find("About Button").GetComponent<Button>();
        SetStaticButtonFuctions();
#if UNITY_WEBGL
#endif
    }

    //This is where we show the first menu of the before login or sign in
    public void ShowFirstMenu()
    {
        var textsToSet = TextReader.TextsToSet(pathOfFirstMenu);

        gameObject.SetActive(true);

        gamesButton.GetComponentInChildren<TextMeshProUGUI>().text = textsToSet[0];
        //evaluationButton.GetComponentInChildren<TextMeshProUGUI>().text = textsToSet[1];
        tryLogo.GetComponentInChildren<TextMeshProUGUI>().text = textsToSet[3];

        tryLogo.SetActive(true);
        logoIcon.SetActive(false);

        gamesButton.gameObject.SetActive(true);


        for (int i = 0; i < gamesButton.transform.childCount; i++)
        {
            var child = gamesButton.transform.GetChild(i);
            if (!child.GetComponent<TextMeshProUGUI>())
            {
                gamesButton.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
#if UNITY_WEBGL
        evaluationButton.gameObject.SetActive(false);
#else
        //evaluationButton.gameObject.SetActive(true);
#endif
        //singOutButton.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        aboutButton.gameObject.SetActive(false);
        kidProfile.gameObject.SetActive(false);
        evalButton.gameObject.SetActive(false);
        SetImageColor(gamesButton.GetComponent<Image>(), TowiDictionary.ColorHexs["activeYellow"]);
        //SetImageColor(evaluationButton.GetComponent<Image>(), TowiDictionary.ColorHexs["activeGreen"]);

        gamesButton.onClick.RemoveAllListeners();
        evaluationButton.onClick.RemoveAllListeners();

        gamesButton.onClick.AddListener(manager.ShowLogIn);

        //evaluationButton.onClick.AddListener(manager.ShowAdd);
    }

    public void ShowThisMenu()
    {
        gameObject.SetActive(true);
        manager.WriteTheText(evaluationButton, 0);
        manager.WriteTheText(gamesButton, 1);

        gamesButton.gameObject.SetActive(true);
        for (int i = 0; i < gamesButton.transform.childCount; i++)
        {
            gamesButton.transform.GetChild(i).gameObject.SetActive(true);
        }
        //evaluationButton.gameObject.SetActive(true);
        for (int i = 0; i < evaluationButton.transform.childCount; i++)
        {
            //evaluationButton.transform.GetChild(i).gameObject.SetActive(true);
        }

        gamesButton.onClick.RemoveAllListeners();
        evaluationButton.onClick.RemoveAllListeners();

        gamesButton.onClick.AddListener(manager.ShowRegisteredAdd);
        //evaluationButton.onClick.AddListener(manager.ShowRegisteredAdd);

        SetImageColor(gamesButton.GetComponent<Image>(), TowiDictionary.ColorHexs["deactivated"]);
        //SetImageColor(evaluationButton.GetComponent<Image>(), TowiDictionary.ColorHexs["deactivated"]);

        //singOutButton.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(true);
        aboutButton.gameObject.SetActive(true);
        kidProfile.gameObject.SetActive(true);

        if(PlayerPrefs.GetInt("IsAvailable") == 0)
        {
            evalButton.gameObject.SetActive(true);
        }
        else if(PlayerPrefs.GetInt("IsAvailable") == 1)
        {
            evalButton.gameObject.SetActive(false);
        }

        tryLogo.SetActive(false);
        logoIcon.SetActive(true);
    }

    public void ShowThisMenu(bool isActiveTheCurrentKid, bool isEvaluationAvailable, int licencesToActivate)
    {
        gameObject.SetActive(true);
        SetDynamicButtonFunctions(isActiveTheCurrentKid, isEvaluationAvailable, licencesToActivate);

        //singOutButton.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(true);
        aboutButton.gameObject.SetActive(true);
        kidProfile.gameObject.SetActive(true);


        if (PlayerPrefs.GetInt("IsAvailable") == 0)
        {
            evalButton.gameObject.SetActive(true);
        }
        else if (PlayerPrefs.GetInt("IsAvailable") == 1)
        {
            evalButton.gameObject.SetActive(false);
        }

        tryLogo.SetActive(false);
        logoIcon.SetActive(true);
    }

    public void HideThisMenu()
    {
        gameObject.SetActive(false);
    }

    void SetStaticButtonFuctions()
    {
        aboutButton.onClick.AddListener(manager.ShowCredits);
        settingsButton.onClick.AddListener(manager.ShowSettings);
        evalButton.onClick.AddListener(manager.ShowEval);
        kidProfile.buttonOfProfile.onClick.AddListener(manager.SetKidsProfiles);
        singOutButton.onClick.AddListener(manager.ShowSingOutWarning);
    }

    public void SetDynamicButtonFunctions(bool isActiveTheCurrentKid, bool evaluationAvailable, int licencesToActivate)
    {
        gamesButton.onClick.RemoveAllListeners();
        evaluationButton.onClick.RemoveAllListeners();
        manager.WriteTheText(evaluationButton, 0);

        if (evaluationAvailable)
        {
            evaluationButton.gameObject.SetActive(true);
        }
        else
        {
            evaluationButton.gameObject.SetActive(false);
        }

        manager.WriteTheText(gamesButton, 1);

        if (isActiveTheCurrentKid)
        {
            gamesButton.onClick.AddListener(manager.LoadGameMenus);
            evaluationButton.onClick.AddListener(manager.ShowDisclaimer);
        }
        else
        {
            gamesButton.onClick.AddListener(() => manager.ShowSubscriptionPanel(manager.LoadGameMenus));
            evaluationButton.onClick.AddListener(() => manager.ShowSubscriptionPanel(manager.ShowGameMenu));
        }



        SetImageColor(gamesButton.GetComponent<Image>(), TowiDictionary.ColorHexs["activeYellow"]);
        SetImageColor(evaluationButton.GetComponent<Image>(), TowiDictionary.ColorHexs["activeGreen"]);

        PlayerPrefs.SetInt(Keys.First_Try, 1);
    }

    void SetImageColor(Image imageToChange, string colorToSet)
    {
        Color colorToPut;
        ColorUtility.TryParseHtmlString(colorToSet, out colorToPut);
        imageToChange.color = colorToPut;
    }
}

