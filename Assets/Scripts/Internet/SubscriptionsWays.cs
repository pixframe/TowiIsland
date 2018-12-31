using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON;

public class SubscriptionsWays : MonoBehaviour
{

    string codeURL = Keys.Api_Web_Key + "api/subscription/redeem_code/";
    string IAPURL = Keys.Api_Web_Key + "api/subscription/create/";
    string activateKidURL = Keys.Api_Web_Key + "api/subscription/children/active/";
    string activateIAPRenew = Keys.Api_Web_Key + "api/subscription/children/update/";
    SessionManager sessionManager;
    MenuManager menuManager;

    // Use this for initialization
    void Start()
    {
        sessionManager = FindObjectOfType<SessionManager>();
        menuManager = GetComponent<MenuManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SendAPurchase(int userId, string userKey, int childId, int months)
    {
        StartCoroutine(SendPurchase(userId, userKey, childId, months));
    }

    IEnumerator SendPurchase(int userId, string userKey, int childId, int months)
    {
        yield return null;
    }

    //This method is used to send a´prepaid code and get an answer for it we start a corutine and works asyncronusly
    public void SendACode(int userId, int childId, string code, int typeOfShop)
    {
        StartCoroutine(SendCode(userId, childId, code, typeOfShop));
    }

    public void SendACode(int userId, string code, int typeOfShop)
    {
        StartCoroutine(SendCode(userId, code, typeOfShop));
    }

    //this is the corutine we use for prepaid code
    IEnumerator SendCode(int userId, int childId, string code, int typeOfShop)
    {
        //now we create a www form were we set the json that hva eall the info for the activaton
        WWWForm form = new WWWForm();
        form.AddField("code", code);
        form.AddField("child_id", childId);
        form.AddField("parent_id", userId);

        //We create a post
        WWW post = new WWW(codeURL, form);

        //wait for the post to get done and get answwer
        yield return post;
        Debug.Log(post.text);
        JSONObject jsonObject = JSONObject.Parse(post.text);
        if (post.text == "")
        {
            menuManager.ShopWithCode(typeOfShop);
            menuManager.ShowWarning(9);
        }
        else
        {
            if (jsonObject.ContainsKey("status"))
            {
                string status = jsonObject.GetString("status");
                if (status == "COUPON_NOT_FOUND")
                {
                    //JSONObject jsonObject = JSONObject.Parse(post.text);
                    menuManager.ShopWithCode(typeOfShop);
                    menuManager.ShowWarning(10);
                }
            }
            else
            {
                Debug.Log("no error found");
                sessionManager.activeKid.isActive = true;
                menuManager.ShowGameMenu();
                sessionManager.SyncProfiles(sessionManager.activeUser.userkey);
            }

        }
        //if error was null we activate the kid 
        //if we have an error we dela with it

    }

    IEnumerator SendCode(int userId, string code, int typeOfShop)
    {
        //now we create a www form were we set the json that hva eall the info for the activaton
        WWWForm form = new WWWForm();
        form.AddField("code", code);
        form.AddField("parent_id", userId);

        //We create a post
        WWW post = new WWW(codeURL, form);

        //wait for the post to get done and get answwer
        yield return post;
        Debug.Log(post.text);
        JSONObject jsonObject = JSONObject.Parse(post.text);
        if (post.text == "")
        {
            menuManager.ShopWithCode(typeOfShop);
            menuManager.ShowWarning(9);
        }
        else
        {
            if (jsonObject.ContainsKey("status"))
            {
                string status = jsonObject.GetString("status");
                if (status == "COUPON_NOT_FOUND")
                {
                    //JSONObject jsonObject = JSONObject.Parse(post.text);
                    menuManager.ShopWithCode(typeOfShop);
                    menuManager.ShowWarning(10);
                }
            }
            else
            {
                sessionManager.SyncProfiles(sessionManager.activeUser.userkey);
                sessionManager.activeUser.suscriptionsLeft = (int)jsonObject.GetNumber("suscriptionsAvailables");
                menuManager.AddKidShower();
            }

        }
        //if error was null we activate the kid 
        //if we have an error we dela with it

    }

    public void SendSubscriptionData(int kids, string ids, int parentId, string typeOfSubscription, UnityEngine.Purchasing.PurchaseEventArgs args)
    {
        StartCoroutine(SendIAP(kids, ids, parentId, typeOfSubscription, args));
    }

    IEnumerator SendIAP(int kids, string ids, int parentId, string typeOfSubscription, UnityEngine.Purchasing.PurchaseEventArgs args)
    {
        WWWForm form = new WWWForm();
        form.AddField("number_kids", kids);
        form.AddField("parent_id", parentId);
        form.AddField("childrens", ids);
        form.AddField("type", typeOfSubscription);

        Debug.Log(form.data);

        WWW post = new WWW(IAPURL, form);

        yield return post;

        Debug.Log(post.text);

        JSONObject jsonOBJ = JSONObject.Parse(post.text);

        if (jsonOBJ["status"].Str == "Succesful")
        {
            sessionManager.activeUser.suscriptionsLeft = (int)jsonOBJ.GetNumber("suscriptionsAvailables");
            sessionManager.SyncProfiles(sessionManager.activeUser.userkey);
            menuManager.SetKidsProfiles();
            FindObjectOfType<MyIAPManager>().ConfirmPurchaseProduct(args);
        }
        else
        {
            menuManager.ShowWarning(9);
        }
    }

    public void ActivateASingleKidAvailable(int kidId, int dadId)
    {
        StartCoroutine(ActivateAvailableKido(kidId, dadId));
    }

    IEnumerator ActivateAvailableKido(int kidId, int dadId)
    {
        WWWForm form = new WWWForm();
        form.AddField("parent_id", dadId);
        form.AddField("child_id", kidId);

        WWW post = new WWW(activateKidURL, form);

        yield return post;

        Debug.Log(post.text);

        if (post.text != "")
        {
            JSONObject jsonObj = JSONObject.Parse(post.text);
            if (jsonObj.GetString("status") == "Succesful")
            {
                sessionManager.activeKid = sessionManager.GetKid(kidId);
                sessionManager.SyncProfiles(sessionManager.activeUser.userkey);
                sessionManager.activeUser.suscriptionsLeft = (int)jsonObj.GetNumber("suscriptionsAvailables");
                sessionManager.SaveSession();
                menuManager.ShowGameMenu();
                Debug.Log("its done");
            }
            else
            {
                menuManager.ShowAccountWarning(0);
                menuManager.ShowWarning(8);
            }
        }
        else
        {
            menuManager.ShowAccountWarning(0);
            menuManager.ShowWarning(8);
        }
    }

    public void ActivateKidIAP(string ids, string date,int parentId)
    {
        StartCoroutine(StartActivateKidByIAP(ids, date, parentId));
    }

    IEnumerator StartActivateKidByIAP(string ids, string date, int parentId)
    {
        WWWForm form = new WWWForm();
        form.AddField("childrens", ids);
        form.AddField("parent_id", parentId);
        form.AddField("finished_date", date);

        WWW post = new WWW(activateIAPRenew, form);

        yield return post;

        Debug.Log(post.text);
        if (post.text != "")
        {
            JSONObject jsonObj = JSONObject.Parse(post.text);
            if (jsonObj.ContainsKey("successful"))
            {

            }
            else
            {
                menuManager.ShowAccountWarning(11);
            }
        }
        else
        {
            menuManager.ShowAccountWarning(11);
        }
    }
}
