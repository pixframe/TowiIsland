using UnityEngine;
using System.Collections;
using Boomlagoon.JSON;

public class ProfileSync : MonoBehaviour {
	SessionManager sessionMng;
    string updateProfileURL = "http://www.towi.com.mx/api/update_profile.php";
	public bool updateOnStart;
	// Use this for initialization
	void Start () {
		sessionMng = GetComponent<SessionManager> ();
		//StartCoroutine (TestDownload ());
		if(updateOnStart)
			UpdateProfile ();
	}
	
	public void UpdateProfile()
	{
		if(!Login.local)
			StartCoroutine (PostUpdateProfile ());
	}

	private IEnumerator TestDownload()
	{
		WWW www = new WWW("http://www.towi.com.mx/xml/ConfigJungleDrawing.xml");
		//yield return www;

		yield return www; 
		
		if (www.error == null) 
		{
			if (www.text != "") 
			{
				sessionMng.activeKid.xmlArenaMagica=www.text;
				sessionMng.SaveSession();
			}
			Debug.Log (www.text);
		}
		//string fullPath = Application.persistentDataPath + "/zipped_file.zip";
		//File.WriteAllBytes (fullPath, www.bytes);
		
		//progress = "downloaded, unzipping...";
	}
	
	private IEnumerator PostUpdateProfile()
	{
		string post_url = updateProfileURL;
		
		JSONObject data = new JSONObject ();
        data.Add("userKey", sessionMng.activeKid.userkey);
		data.Add("cid", sessionMng.activeKid.id);
		data.Add("kiwis", sessionMng.activeKid.kiwis);
		data.Add("avatar", sessionMng.activeKid.avatar);
		data.Add("activeDay", sessionMng.activeKid.activeDay);
		data.Add("rioTutorial", sessionMng.activeKid.rioTutorial);
		data.Add("tesoroTutorial", sessionMng.activeKid.tesoroTutorial);
		data.Add("arbolMusicalTutorial", sessionMng.activeKid.arbolMusicalTutorial);
		Debug.Log (data.ToString ());
		WWWForm form = new WWWForm();
		form.AddField("jsonToDb", data.ToString());
		form.AddField("avatarClothes", sessionMng.activeKid.avatarClothes);
		form.AddField("ownedItems", sessionMng.activeKid.ownedItems);
		form.AddField("activeMissions", sessionMng.activeKid.activeMissions.ToString());
		//form.AddField("missionList", sessionMng.activeKid.missionList);
		
		WWW hs_post = new WWW(post_url,form);
		yield return hs_post; 
		
		if (hs_post.error == null) {
			JSONObject jsonObject = JSONObject.Parse (hs_post.text);
			Debug.Log (jsonObject.GetValue ("code").Str);
			if (jsonObject.GetValue ("code").Str == "200") {
				sessionMng.activeKid.syncProfile=true;
			}
		}else
		{
			sessionMng.activeKid.syncProfile=false;
		}
	}
}
