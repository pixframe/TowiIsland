using UnityEngine;
using System.Collections;
using Boomlagoon.JSON;
using System;

public class TrialRegisterController : MonoBehaviour {

	private string secretKey = "$k1w1GAMES$";
	public string startTrialURL;
	public string getTrialURL;
	public string registerParentURL;
	public string registerKidURL;
	public string registerParentKidURL;
	public string subscribeURL;
	TrialManager trialMng;

	void Start()
	{
		trialMng = GetComponent<TrialManager> ();
	}

	public void StartTrial(){
		StartCoroutine(PostStartTrial());
	}
	
	IEnumerator PostStartTrial()
	{
		string hash = Md5Sum(SystemInfo.deviceUniqueIdentifier);
		string post_url = startTrialURL;
		string code = "";
		WWWForm form = new WWWForm();
		form.AddField("md5_device", hash);

		WWW hs_post = new WWW(post_url,form);
		yield return hs_post; 

		if (hs_post.error == null) {
			JSONObject jsonObject = JSONObject.Parse (hs_post.text);
			Debug.Log (jsonObject.GetValue ("code").Str);
			code=jsonObject.GetValue ("code").Str;
			if (jsonObject.GetValue ("code").Str == "200"||jsonObject.GetValue ("code").Str == "405") {
				PlayerPrefs.SetInt("TrialID",(int)jsonObject.GetValue ("trial_id").Number);
			}
		}

		if(code=="405")
		{
			post_url = getTrialURL;
			
			form = new WWWForm();
			form.AddField("trial_id", PlayerPrefs.GetInt("TrialID",0));
			
			hs_post = new WWW(post_url,form);
			yield return hs_post; 
			
			if (hs_post.error == null) {
				JSONObject jsonObject = JSONObject.Parse (hs_post.text);
				Debug.Log (jsonObject.GetValue ("code").Str);
				//Debug.Log (jsonObject.GetValue ("time_left").Str);
				if (jsonObject.GetValue ("code").Str == "200") {
					PlayerPrefs.SetInt("TrialRemaining",int.Parse(jsonObject.GetValue ("time_left").Str.Split(' ')[0])+1);
					trialMng.SetRemaining(PlayerPrefs.GetInt("TrialRemaining",0));
				}else if (jsonObject.GetValue ("code").Str == "408") {
					PlayerPrefs.SetInt("TrialRemaining",0);
					trialMng.SetRemaining(PlayerPrefs.GetInt("TrialRemaining",0));
				}
			}
		}
	}
	
	public void GetTrial(TimeSpan span, int remDays, DateTime start, DateTime today){
		Debug.Log(StartCoroutine(PostGetTrial(span,remDays,start,today)));
	}
	
	IEnumerator PostGetTrial(TimeSpan span, int remDays, DateTime start, DateTime today)
	{
		int trialNumber = PlayerPrefs.GetInt ("TrialID", 0);
		string post_url = "";
		WWWForm form;
		WWW hs_post;

		if(trialNumber==0&&start!=null)
		{
			string hash = Md5Sum(SystemInfo.deviceUniqueIdentifier);
			post_url = startTrialURL;
			TimeSpan startedSince=today-start;
			form = new WWWForm();
			form.AddField("md5_device", hash);
			string testString=String.Format("{0:00}:{1:00}:{2:00}",(int)startedSince.Days,startedSince.Hours,startedSince.Minutes);
			form.AddField("trial_started_on",testString);
			
			hs_post = new WWW(post_url,form);
			yield return hs_post; 
			
			if (hs_post.error == null) {
				JSONObject jsonObject = JSONObject.Parse (hs_post.text);
				Debug.Log (jsonObject.GetValue ("code").Str);
				if (jsonObject.GetValue ("code").Str == "200"||jsonObject.GetValue ("code").Str == "405") {
					PlayerPrefs.SetInt("TrialID",(int)jsonObject.GetValue ("trial_id").Number);
					trialNumber=PlayerPrefs.GetInt("TrialID",0);
				}
			}
		}

		if(trialNumber!=0)
		{
			post_url = getTrialURL;
			
			form = new WWWForm();
			form.AddField("trial_id", trialNumber);
			
			hs_post = new WWW(post_url,form);
			yield return hs_post; 
			
			if (hs_post.error == null) {
				JSONObject jsonObject = JSONObject.Parse (hs_post.text);
				Debug.Log (jsonObject.GetValue ("code").Str);
				//Debug.Log (jsonObject.GetValue ("time_left").Str);
				if (jsonObject.GetValue ("code").Str == "200") {
					PlayerPrefs.SetInt("TrialRemaining",int.Parse(jsonObject.GetValue ("time_left").Str.Split(' ')[0])+1);
					trialMng.SetRemaining(PlayerPrefs.GetInt("TrialRemaining",0));
				} else if (jsonObject.GetValue ("code").Str == "408") {
					PlayerPrefs.SetInt("TrialRemaining",0);
					trialMng.SetRemaining(PlayerPrefs.GetInt("TrialRemaining",0));
				}
				else
				{
					if(span.Days>0)
					{	
						remDays-=span.Days;
						remDays = Mathf.Max(0,remDays);
						PlayerPrefs.SetInt("TrialRemaining",remDays);
						trialMng.SetRemaining(remDays);
					}
				}
			}else{
				if(span.Days>0)
				{	
					remDays-=span.Days;
					remDays = Mathf.Max(0,remDays);
					PlayerPrefs.SetInt("TrialRemaining",remDays);
					trialMng.SetRemaining(remDays);
				}
			}
		}
	}

	public void RegisterParentAndKid(string name, string lastName, string email, string password, string kidName, string kidLastName){
		StartCoroutine(PostRegisterParentAndKid(name,lastName,email,password,kidName,kidLastName));
	}

	IEnumerator PostRegisterParentAndKid(string name, string lastName, string email, string password, string kidName, string kidLastName)
	{
		int trialNumber = PlayerPrefs.GetInt ("TrialID", 0);
		string psswdHash = Md5Sum(password);
		string post_url = registerParentKidURL;
		
		JSONObject data = new JSONObject ();
		data.Add("name_parent", name);
		data.Add("lastname_parent", lastName);
		data.Add("email_parent", email);
		data.Add("password_parent", psswdHash);
		data.Add("name_child", kidName);
		data.Add("lastname_child", kidLastName);
		if(trialNumber!=0)
			data.Add("trial_id", trialNumber);
		Debug.Log (data.ToString ());
		WWWForm form = new WWWForm();
		form.AddField("jsonToDb", data.ToString());
		
		WWW hs_post = new WWW(post_url,form);
		yield return hs_post; 
		
		if (hs_post.error == null) {
			JSONObject jsonObject = JSONObject.Parse (hs_post.text);
			Debug.Log (jsonObject.GetValue ("code").Str);
			switch(jsonObject.GetValue ("code").Str)
			{
				case "200":
					PlayerPrefs.SetString ("ParentKey",jsonObject.GetValue ("key").Str);
                    trialMng.loginRef.sessionMng.LoadUser(email, psswdHash, jsonObject.GetValue("key").Str, null, (int)jsonObject.GetValue("id").Number);
                    trialMng.loginRef.sessionMng.AddKid((int)jsonObject.GetValue("child_id").Number, kidName + " " + kidLastName, jsonObject.GetValue("key").Str, jsonObject.GetBoolean("active"), jsonObject.GetBoolean("trial"));
					trialMng.loginRef.sessionMng.SaveSession();
					trialMng.loginRef.CreateProfiles();
					PlayerPrefs.SetInt("SubscriptionTrial",1);
					trialMng.loginRef.showRegister=false;
					trialMng.loginRef.DisplayProfiles();
				break;
                case "201":
                    trialMng.loginRef.errorText = "El usuario se registró correctamente pero el periodo de prueba ha expirado.";
                break;
				case "400":
					trialMng.loginRef.errorText= trialMng.loginRef.language.levelStrings[39];
				break;
				case "407":
                    trialMng.loginRef.errorText = trialMng.loginRef.language.levelStrings[40];
				break;
				case "111":
					trialMng.loginRef.registerStep=1;
                    trialMng.loginRef.errorText = trialMng.loginRef.language.levelStrings[41];
				break;
			}
		}else
		{
            trialMng.loginRef.errorText = trialMng.loginRef.language.levelStrings[39];
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
