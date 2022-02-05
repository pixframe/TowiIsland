using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubscriptionPromoMenu : BaseMenu
{
    public Button cancelButton;
    public Button notNowButton;
    public Button subscribeButton;
    public TextMeshProUGUI promoSubscriptionText;

    public SubscriptionPromoMenu(GameObject baseGameObject, UnityAction subscribeAction, UnityAction cancelAction)
    {
        gameObject = baseGameObject;
        promoSubscriptionText = gameObject.transform.Find("PromoSubscriptionText").GetComponent<TextMeshProUGUI>();
        notNowButton = gameObject.transform.Find("NotNowButton").GetComponent<Button>();
        subscribeButton = gameObject.transform.Find("SubscribeButton").GetComponent<Button>();
        cancelButton = gameObject.transform.Find("CancelButton").GetComponent<Button>();
        notNowButton.onClick.AddListener(cancelAction);
        subscribeButton.onClick.AddListener(subscribeAction);
        cancelButton.onClick.AddListener(cancelAction);
    }

    public void SetActive(bool active, UnityAction cancelAction)
    {
        SetActive(active);
        cancelButton.onClick.RemoveAllListeners();
        notNowButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(cancelAction);
        notNowButton.onClick.AddListener(cancelAction);
    }
}
