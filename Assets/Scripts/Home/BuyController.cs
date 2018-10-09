using UnityEngine;
using System.Collections;
using Boomlagoon.JSON;

public class BuyController : MonoBehaviour {
    string buyURL = "http://www.towi.com.mx/api/try_buy.php";
	Login mainRef;

	void Start()
	{
		mainRef = GetComponent<Login> ();
	}

	public void TryActivate(string serial)
	{
		StartCoroutine (PostTryActivate (serial));
	}

	IEnumerator PostTryActivate(string serial){
		string hash = Md5Sum(SystemInfo.deviceUniqueIdentifier);
		string post_url = buyURL;
	
		WWWForm form = new WWWForm();
		form.AddField("serial", serial);
		form.AddField("md5_device", hash);
		
		WWW hs_post = new WWW(post_url,form);
		yield return hs_post; 
		
		if (hs_post.error == null) {
			JSONObject jsonObject = JSONObject.Parse (hs_post.text);
			Debug.Log (jsonObject.GetValue ("code").Str);
			switch(jsonObject.GetValue ("code").Str)
			{
			case "200":
				PlayerPrefs.SetInt("purchased", 1);
				mainRef.currentState = Login.Phase.Menu;
				mainRef.menu.gameObject.SetActive(true);
				mainRef.waitingForPurchase = false;
				mainRef.HideBG();
				mainRef.loaderRef.color= new Color(1,1,1,0);
				break;
			case "400":
				mainRef.errorText=mainRef.language.levelStrings[39];
				break;
			case "403":
                mainRef.errorText = mainRef.language.levelStrings[42];
				break;
			case "405":
                mainRef.errorText = mainRef.language.levelStrings[43];
				break;
			}
		}
	}

	public  string Md5Sum(string strToEncrypt)
	{
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
		byte[] bytes = ue.GetBytes(strToEncrypt);
		
		// encrypt bytes
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);
		
		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";
		
		for (int i = 0; i < hashBytes.Length; i++)
		{
			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}
		
		return hashString.PadLeft(32, '0');
	}
}
