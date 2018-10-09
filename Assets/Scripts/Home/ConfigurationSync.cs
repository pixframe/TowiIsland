using UnityEngine;
using System.Collections;

public class ConfigurationSync : MonoBehaviour {
	SessionManager sessionMng;
	// Use this for initialization
	void Start () {
		sessionMng = GetComponent<SessionManager> ();
		StartCoroutine (TestDownload ());
	}
	
	// Update is called once per frame
	void Update () {
	
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
}
