using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IslandShoppingManager : MonoBehaviour {

    public GameObject container;
    public GameObject infoPanel;
    public GameObject kiwiInfo;
    public GameObject kiwiLogoPanel;
    public GameObject finishPartciles;

    public TextMeshProUGUI confirmationText;
    public TextMeshProUGUI priceTag;
    public TextMeshProUGUI kiwiAmountText;

    public Button viewButton;
    public Button goBackButton;
    public Button yesButton;
    public Button noButton;

    //This are the prices we have for the island shopping
    public static int[] IslandPrices = new int[] { 20,40,20,20,20,20,10,50,50,200,30,20,50,50,40,60,60,20,120,80,50,20,40,100,60,40,120,40,30,30,40,150,100,20,100,60,300,20,20,50};

    List<IslandShopButton> buttons = new List<IslandShopButton>();

    MiniIslandController miniIsland;
    SessionManager sessionManager;

    int kiwiAmout = 0;
    float startPosition;

    string[] stringsToShow;

    CameraIslandShop cam;

    // Use this for initialization
    void Start()
    {

        sessionManager = FindObjectOfType<SessionManager>();
        miniIsland = FindObjectOfType<MiniIslandController>();
        cam = FindObjectOfType<CameraIslandShop>();

        kiwiAmout = sessionManager.activeKid.kiwis;
        UpdateKiwiAmount();

        stringsToShow = TextReader.TextsToShow(Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}MiniGames/IslandShop"));

        for (int i = 0; i < container.transform.childCount; i++)
        {
            buttons.Add(new IslandShopButton(container.transform.GetChild(i).GetComponent<Button>()));
            int x = i;
            buttons[i].mainButton.onClick.AddListener(() => CloseTheDeal(x));
            buttons[i].priceText.text = "x " + IslandPrices[i];
        }

        startPosition = buttons[0].mainButton.GetComponent<RectTransform>().localPosition.x;

        viewButton.onClick.AddListener(ViewIsland);
        goBackButton.onClick.AddListener(GoBack);

        SetCorrectButtons();
        viewButton.GetComponentInChildren<TextMeshProUGUI>().text = stringsToShow[5];
    }

    // Update is called once per frame
    void Update() {

    }

    //Here we set the buttons of objects that are still available
    void SetCorrectButtons()
    {
        int objectsLeftsToSpawn = 0;
        float sizeOfContainer = 127f; //This is the original size of the object that contains all the shop buttons

        float addableSize = 150f; //This is the size added for every new button

        //We iterate trough all the buttons
        for (int i = 0; i < miniIsland.buyedObjects.Count; i++)
        {
            //We check if the object its still available to buy
            if (!miniIsland.buyedObjects[i])
            {
                objectsLeftsToSpawn++;//We add numbers to this object to determine how many 
                if (objectsLeftsToSpawn > 1)
                {
                    sizeOfContainer += addableSize;
                }
            }

            buttons[i].mainButton.gameObject.SetActive(!miniIsland.buyedObjects[i]);

            if (objectsLeftsToSpawn > 1)
            {
                buttons[i].mainButton.GetComponent<RectTransform>().localPosition = new Vector2(startPosition + (addableSize * (objectsLeftsToSpawn - 1)), buttons[i].mainButton.GetComponent<RectTransform>().localPosition.y);
            }
            else
            {
                buttons[i].mainButton.GetComponent<RectTransform>().localPosition = new Vector2(startPosition, buttons[i].mainButton.GetComponent<RectTransform>().localPosition.y);
            }
        }

        container.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeOfContainer, container.GetComponent<RectTransform>().sizeDelta.y);
    }

    void ActivateMiniIslandObject(int activateObject)
    {
        kiwiAmout -= IslandPrices[activateObject];
        miniIsland.transform.GetChild(activateObject + 1).gameObject.SetActive(true);
        miniIsland.buyedObjects[activateObject] = true;
        sessionManager.activeKid.kiwis = kiwiAmout;
        UpdateKiwiAmount();
        SetCorrectButtons();
        ViewIsland();
        finishPartciles.transform.position = miniIsland.transform.GetChild(activateObject + 1).position;
        finishPartciles.GetComponent<ParticleSystem>().Play();
        sessionManager.activeKid.buyedIslandObjects.Add(activateObject);
        sessionManager.UpdateProfile();
    }

    void CloseTheDeal(int objectToActivate)
    {

        if (HasEnoughMoneyToBuy(IslandPrices[objectToActivate], kiwiAmout))
        {
            HidePanels();
            infoPanel.SetActive(true);

            confirmationText.text = stringsToShow[0];
            priceTag.gameObject.SetActive(true);
            priceTag.text = "x " + IslandPrices[objectToActivate];
            yesButton.GetComponentInChildren<TextMeshProUGUI>().text = stringsToShow[2];
            noButton.GetComponentInChildren<TextMeshProUGUI>().text = stringsToShow[3];
            kiwiLogoPanel.SetActive(true);
            noButton.gameObject.SetActive(true);

            yesButton.onClick.RemoveAllListeners();
            noButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(() => ActivateMiniIslandObject(objectToActivate));
            noButton.onClick.AddListener(ViewPanels);
        }
        else
        {
            HidePanels();
            infoPanel.SetActive(true);

            confirmationText.text = stringsToShow[1];
            priceTag.gameObject.SetActive(false);
            yesButton.GetComponentInChildren<TextMeshProUGUI>().text = stringsToShow[4];
            kiwiLogoPanel.SetActive(false);
            noButton.gameObject.SetActive(false);

            yesButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(ViewPanels);
        }
    }

    public bool HasEnoughMoneyToBuy(int costOfObject, int kiwisOfPlayer)
    {
        return costOfObject <= kiwisOfPlayer;
    }

    void ViewPanels()
    {
        infoPanel.SetActive(false);
        goBackButton.gameObject.SetActive(true);
        container.transform.parent.parent.gameObject.SetActive(true);
        kiwiInfo.SetActive(true);
    }

    void HidePanels()
    {
        goBackButton.gameObject.SetActive(false);
        container.transform.parent.parent.gameObject.SetActive(false);
        infoPanel.SetActive(false);
        kiwiInfo.SetActive(false);
    }

    void ViewIsland()
    {
        if (cam.EnableMoveCamera())
        {
            viewButton.GetComponentInChildren<TextMeshProUGUI>().text = stringsToShow[6];
            HidePanels();
        }
        else
        {
            viewButton.GetComponentInChildren<TextMeshProUGUI>().text = stringsToShow[5];
            ViewPanels();
        }
    }

    void UpdateKiwiAmount()
    {
        kiwiAmountText.text = kiwiAmout.ToString("000");
    }

    void GoBack()
    {
        FindObjectOfType<GameMenusManager>().ReturnToNormality();
    }
}

struct IslandShopButton
{
    public Button mainButton;
    public GameObject objectShown;
    public TextMeshProUGUI priceText;
    public Image kiwiImage;

    public IslandShopButton(Button buttonToBeMain)
    {
        mainButton = buttonToBeMain;
        objectShown = mainButton.transform.GetChild(0).gameObject;
        priceText = mainButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        kiwiImage = mainButton.transform.GetChild(2).GetComponent<Image>();
    }
}
