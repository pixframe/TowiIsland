using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubscriptionPromoMenu : BaseMenu
{
    public Button cancelButton;
    public Button notNowButton;
    public Button subscribeButton;
    public TextMeshProUGUI promoSubscriptionText;

    public SubscriptionPromoMenu(GameObject baseGameObject, MenuManager manager)
    {
        gameObject = baseGameObject;
        promoSubscriptionText = gameObject.transform.Find("PromoSubscriptionText").GetComponent<TextMeshProUGUI>();
        notNowButton = gameObject.transform.Find("NotNowButton").GetComponent<Button>();
        subscribeButton = gameObject.transform.Find("SubscribeButton").GetComponent<Button>();
        cancelButton = gameObject.transform.Find("CancelButton").GetComponent<Button>();
        notNowButton.onClick.AddListener(manager.LoadGameMenus);
        subscribeButton.onClick.AddListener(() => manager.SetShop(0));
        cancelButton.onClick.AddListener(manager.LoadGameMenus);
    }
}
