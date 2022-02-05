using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public struct GameCenterShopPanel
{
    public GameCenterShopPanel(GameObject gameObject)
    {
        this.gameObject = gameObject;
        this.gameButton = gameObject.transform.Find("GameButton").GetComponent<Button>();
        this.subscriptionButton = gameObject.transform.Find("SubscriptionButton").GetComponent<Button>();
        this.shopText = gameObject.transform.Find("ShopText").GetComponent<TextMeshProUGUI>();
        this.cancelButton = gameObject.transform.Find("CancelButton").GetComponent<Button>();
        UpdateCancelFunction(() => gameObject.SetActive(false));
    }

    public void UpdateText(string text)
    {
        shopText.text = text;
    }

    public void UpdateTexts(string shopTexts, string gameButtonText, string suscriptionButtonText)
    {
        shopText.text = shopTexts;
        gameButton.GetComponentInChildren<TextMeshProUGUI>().text = gameButtonText;
        subscriptionButton.GetComponentInChildren<TextMeshProUGUI>().text = suscriptionButtonText;
    }

    public void UpdateCancelFunction(UnityAction action) 
    {
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(action);
    }

    public GameObject gameObject;
    public Button gameButton;
    public Button subscriptionButton;
    public TextMeshProUGUI shopText;
    public Button cancelButton;
}
