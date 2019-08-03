using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class MyIAPManager : MonoBehaviour, IStoreListener {


    static IStoreController m_StoreController;          // The Unity Purchasing system.
    static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
    IGooglePlayStoreExtensions m_GooglePlayStoreExtensions;
    IAppleExtensions m_AppleExtensions;

    // Product identifiers for all products capable of being purchased: 
    // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
    // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
    // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

    //One month identifiers
    public static string oneMonthSuscription = "Suscripcion mensual";
    static string oneMonthApple = "one_month_subscription";
    static string oneMonthGoolge = "one_month_subscription";
    public static string oneMonthSuscriptionTwo = "Suscripcion mensual dos niños";
    static string oneMonthAppleTwo = "one_month_two_kids";
    static string oneMonthGoolgeTwo = "one_month_two_kids";
    public static string oneMonthSuscriptionThree = "Suscripcion mensual tres niños";
    static string oneMonthAppleThree = "one_moth_three_kids";
    static string oneMonthGoolgeThree = "one_moth_three_kids";
    public static string oneMonthSuscriptionFour = "Suscripcion mensual cuatro niños";
    static string oneMonthAppleFour = "one_moth_four_kids";
    static string oneMonthGoolgeFour = "one_month_four_kids";
    public static string oneMonthSuscriptionFive = "Suscripcion mensual cinco niños";
    static string oneMonthAppleFive = "one_month_five_kids";
    static string oneMonthGoolgeFive = "one_month_five_kids ";

    //Three month identifiers
    public static string threeMonthSuscription = "Suscripcion trimestral";
    static string threeMonthApple = "three_month_subscription";
    static string threeMonthGoolge = "three_month_subscription";
    public static string threeMonthSuscriptionTwo = "Suscripcion trimestral dos niños";
    static string threeMonthAppleTwo = "three_month_two_kids";
    static string threeMonthGoolgeTwo = "three_month_two_kids";
    public static string threeMonthSuscriptionThree = "Suscripcion trimestral tres niños";
    static string threeMonthAppleThree = "three_month_three_kids";
    static string threeMonthGoolgeThree = "three_month_three_kids";
    public static string threeMonthSuscriptionFour = "Suscripcion trimestral cuatro niños";
    static string threeMonthAppleFour = "three_month_four_kids";
    static string threeMonthGoolgeFour = "three_month_four_kids";
    public static string threeMonthSuscriptionFive = "Suscripcion trimestral cinco niños";
    static string threeMonthAppleFive = "three_month_five_kids";
    static string threeMonthGoolgeFive = "three_month_five_kids";

    MenuManager menuManager;
    bool isSubscribedInIAP;

    DateTime dateExpiration;
    int kids;
    int subscriptions;

    List<int> ids = new List<int>();

    PurchaseEventArgs eventArgs;

    void Awake()
    {
        if (FindObjectsOfType<MyIAPManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }

        menuManager = FindObjectOfType<MenuManager>();
    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(oneMonthSuscription, ProductType.Subscription, new IDs(){
                { oneMonthApple, AppleAppStore.Name },
                { oneMonthGoolge, GooglePlay.Name }
        });
        builder.AddProduct(oneMonthSuscriptionTwo, ProductType.Subscription, new IDs(){
                { oneMonthAppleTwo, AppleAppStore.Name },
                { oneMonthGoolgeTwo, GooglePlay.Name }
        });
        builder.AddProduct(oneMonthSuscriptionThree, ProductType.Subscription, new IDs(){
                { oneMonthAppleThree, AppleAppStore.Name },
                { oneMonthGoolgeThree, GooglePlay.Name }
        });
        builder.AddProduct(oneMonthSuscriptionFour, ProductType.Subscription, new IDs(){
                { oneMonthAppleFour, AppleAppStore.Name },
                { oneMonthGoolgeFour, GooglePlay.Name }
        });
        builder.AddProduct(oneMonthSuscriptionFive, ProductType.Subscription, new IDs(){
                { oneMonthAppleFive, AppleAppStore.Name },
                { oneMonthGoolgeFive, GooglePlay.Name }
        });

        builder.AddProduct(threeMonthSuscription, ProductType.Subscription, new IDs()
        {
            { threeMonthApple, AppleAppStore.Name},
            { threeMonthGoolge, GooglePlay.Name}
        });
        builder.AddProduct(threeMonthSuscriptionTwo, ProductType.Subscription, new IDs()
        {
            { threeMonthAppleTwo, AppleAppStore.Name},
            { threeMonthGoolgeTwo, GooglePlay.Name}
        });
        builder.AddProduct(threeMonthSuscriptionThree, ProductType.Subscription, new IDs()
        {
            { threeMonthAppleThree, AppleAppStore.Name},
            { threeMonthGoolgeThree, GooglePlay.Name}
        });
        builder.AddProduct(threeMonthSuscriptionFour, ProductType.Subscription, new IDs()
        {
            { threeMonthAppleFour, AppleAppStore.Name},
            { threeMonthGoolgeFour, GooglePlay.Name}
        });
        builder.AddProduct(threeMonthSuscriptionFive, ProductType.Subscription, new IDs()
        {
            { threeMonthAppleFive, AppleAppStore.Name},
            { threeMonthGoolgeFive, GooglePlay.Name}
        });

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    public void BuySubscriptionOneMonth(int numOfKids)
    {
        Debug.Log("Buying One Month");
        switch (numOfKids)
        {
            case 1:
                BuyProductID(oneMonthSuscription);
                break;
            case 2:
                BuyProductID(oneMonthSuscriptionTwo);
                break;
            case 3:
                BuyProductID(oneMonthSuscriptionThree);
                break;
            case 4:
                BuyProductID(oneMonthSuscriptionFour);
                break;
            case 5:
                BuyProductID(oneMonthSuscriptionFive);
                break;
            default:
                BuyProductID(oneMonthSuscription);
                break;
        }

    }

    public void BuySubscriptionThreeMonths(int numOfKids)
    {
        Debug.Log("Buying Three Month");
        switch (numOfKids)
        {
            case 1:
                BuyProductID(threeMonthSuscription);
                break;
            case 2:
                BuyProductID(threeMonthSuscriptionTwo);
                break;
            case 3:
                BuyProductID(threeMonthSuscriptionThree);
                break;
            case 4:
                BuyProductID(threeMonthSuscriptionFour);
                break;
            case 5:
                BuyProductID(threeMonthSuscriptionFive);
                break;
            default:
                BuyProductID(threeMonthSuscription);
                break;
        }
    }

    public string CostInCurrency(int months)
    {
        Product product;
        if (months == 1)
        {
            product = m_StoreController.products.WithID(oneMonthSuscription);
        }
        else
        {
            product = m_StoreController.products.WithID(threeMonthSuscription);
        }
        if (product != null && product.availableToPurchase)
        {
            return product.metadata.localizedPriceString;
        }
        return "";
    }


    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                FindObjectOfType<MenuManager>().ShowWarning(8);
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) => {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }


    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
        m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
        m_GooglePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
        Dictionary<string, string> introductory_info_dict = m_AppleExtensions.GetIntroductoryPriceDictionary();
        foreach (var item in controller.products.all)
        {
            if (item.availableToPurchase)
            {
                if (item.receipt != null)
                {
                    if (item.definition.type == ProductType.Subscription)
                    {
                        if (checkIfProductIsAvailableForSubscriptionManager(item.receipt))
                        {
                            string intro_json = (introductory_info_dict == null || !introductory_info_dict.ContainsKey(item.definition.storeSpecificId)) ? null : introductory_info_dict[item.definition.storeSpecificId];
                            SubscriptionManager p = new SubscriptionManager(item, intro_json);
                            SubscriptionInfo info = p.getSubscriptionInfo();

                            if (info.isSubscribed() == Result.True)
                            {
                                isSubscribedInIAP = true;

                                switch (info.getProductId())
                                {
                                    case "one_month_subscription":
                                        kids = 1;
                                        break;
                                    case "one_month_two_kids":
                                        kids = 2;
                                        break;
                                    case "one_moth_three_kids":
                                        kids = 3;
                                        break;
                                    case "one_moth_four_kids":
                                        kids = 4;
                                        break;
                                    case "one_month_four_kids":
                                        kids = 4;
                                        break;
                                    case "one_month_five_kids":
                                        kids = 5;
                                        break;
                                    case "three_month_subscription":
                                        kids = 1;
                                        break;
                                    case "three_month_two_kids":
                                        kids = 2;
                                        break;
                                    case "three_month_three_kids":
                                        kids = 3;
                                        break;
                                    case "three_month_four_kids":
                                        kids = 4;
                                        break;
                                    case "three_month_five_kids":
                                        kids = 5;
                                        break;
                                }

                                dateExpiration = info.getExpireDate();
                            }

                            Debug.Log("product id is: " + info.getProductId());
                            Debug.Log("purchase date is: " + info.getPurchaseDate());
                            Debug.Log("subscription next billing date is: " + info.getExpireDate());
                            Debug.Log("is subscribed? " + info.isSubscribed().ToString());
                            Debug.Log("is expired? " + info.isExpired().ToString());
                            Debug.Log("is cancelled? " + info.isCancelled());
                            Debug.Log("product is in free trial peroid? " + info.isFreeTrial());
                            Debug.Log("product is auto renewing? " + info.isAutoRenewing());
                            Debug.Log("subscription remaining valid time until next billing date is: " + info.getRemainingTime());
                            Debug.Log("is this product in introductory price period? " + info.isIntroductoryPricePeriod());
                            Debug.Log("the product introductory localized price is: " + info.getIntroductoryPrice());
                            Debug.Log("the product introductory price period is: " + info.getIntroductoryPricePeriod());
                            Debug.Log("the number of product introductory price period cycles is: " + info.getIntroductoryPricePeriodCycles());
                        }
                        else
                        {
                            Debug.Log("This product is not available for SubscriptionManager class, only products that are purchase by 1.19+ SDK can use this class.");
                        }
                    }
                    else
                    {
                        Debug.Log("the product is not a subscription product");
                    }
                }
                else
                {

                }
            }
        }
    }


    public bool IsStillSuscribed()
    {
        return isSubscribedInIAP;
    }

    public DateTime ExpireDate()
    {
        return dateExpiration;
    }

    public int GetKidData()
    {
        return kids;
    }

    private bool checkIfProductIsAvailableForSubscriptionManager(string receipt)
    {
        var receipt_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);
        if (!receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload"))
        {
            Debug.Log("The product receipt does not contain enough information");
            return false;
        }
        var store = (string)receipt_wrapper["Store"];
        var payload = (string)receipt_wrapper["Payload"];

        if (payload != null)
        {
            switch (store)
            {
                case GooglePlay.Name:
                    {
                        var payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
                        if (!payload_wrapper.ContainsKey("json"))
                        {
                            Debug.Log("The product receipt does not contain enough information, the 'json' field is missing");
                            return false;
                        }
                        var original_json_payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode((string)payload_wrapper["json"]);
                        if (original_json_payload_wrapper == null || !original_json_payload_wrapper.ContainsKey("developerPayload"))
                        {
                            Debug.Log("The product receipt does not contain enough information, the 'developerPayload' field is missing");
                            return false;
                        }
                        var developerPayloadJSON = (string)original_json_payload_wrapper["developerPayload"];
                        var developerPayload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(developerPayloadJSON);
                        if (developerPayload_wrapper == null || !developerPayload_wrapper.ContainsKey("is_free_trial") || !developerPayload_wrapper.ContainsKey("has_introductory_price_trial"))
                        {
                            Debug.Log("The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
                            return false;
                        }
                        return true;
                    }
                case AppleAppStore.Name:
                case AmazonApps.Name:
                case MacAppStore.Name:
                    {
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }
        return false;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public void ConfirmPurchaseProduct()
    {
        UnityEngine.Analytics.Analytics.CustomEvent("subscribe");
        m_StoreController.ConfirmPendingPurchase(eventArgs.purchasedProduct);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        eventArgs = args;
        if (String.Equals(args.purchasedProduct.definition.id, oneMonthSuscription, StringComparison.Ordinal))
        {
            menuManager.SetKidProfilesToAddASubscription(1, "monthly_inApp");
            return PurchaseProcessingResult.Pending;
        }
        else if (String.Equals(args.purchasedProduct.definition.id, oneMonthSuscriptionTwo, StringComparison.Ordinal))
        {
            menuManager.SetKidProfilesToAddASubscription(2, "monthly_inApp");
            return PurchaseProcessingResult.Pending;
        }
        else if (String.Equals(args.purchasedProduct.definition.id, oneMonthSuscriptionThree, StringComparison.Ordinal))
        {
            menuManager.SetKidProfilesToAddASubscription(3, "monthly_inApp");
            return PurchaseProcessingResult.Pending;
        }
        else if (String.Equals(args.purchasedProduct.definition.id, oneMonthSuscriptionFour, StringComparison.Ordinal))
        {
            menuManager.SetKidProfilesToAddASubscription(4, "monthly_inApp");
            return PurchaseProcessingResult.Pending;
        }
        else if (String.Equals(args.purchasedProduct.definition.id, oneMonthSuscriptionFive, StringComparison.Ordinal))
        {
            menuManager.SetKidProfilesToAddASubscription(5, "monthly_inApp");
            return PurchaseProcessingResult.Pending;
        }
        else if (String.Equals(args.purchasedProduct.definition.id, threeMonthSuscription, StringComparison.Ordinal))
        {
            menuManager.SetKidProfilesToAddASubscription(1, "quarterly_inApp");
            return PurchaseProcessingResult.Pending;
        }
        else if (String.Equals(args.purchasedProduct.definition.id, threeMonthSuscriptionTwo, StringComparison.Ordinal))
        {
            menuManager.SetKidProfilesToAddASubscription(2, "quarterly_inApp");
            return PurchaseProcessingResult.Pending;
        }
        else if (String.Equals(args.purchasedProduct.definition.id, threeMonthSuscriptionThree, StringComparison.Ordinal))
        {
            menuManager.SetKidProfilesToAddASubscription(3, "quarterly_inApp");
            return PurchaseProcessingResult.Pending;
        }
        else if (String.Equals(args.purchasedProduct.definition.id, threeMonthSuscriptionFour, StringComparison.Ordinal))
        {
            menuManager.SetKidProfilesToAddASubscription(4, "quarterly_inApp");
            return PurchaseProcessingResult.Pending;
        }
        else if (String.Equals(args.purchasedProduct.definition.id, threeMonthSuscriptionFive, StringComparison.Ordinal))
        {
            menuManager.SetKidProfilesToAddASubscription(5, "quarterly_inApp");
            return PurchaseProcessingResult.Pending;
        }
        // Or ... an unknown product has been purchased by this user. Fill in additional products here....
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        FindObjectOfType<MenuManager>().ShopNumOfKids();
        FindObjectOfType<MenuManager>().ShowWarning(8);
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}
