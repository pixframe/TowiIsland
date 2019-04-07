using UnityEngine;
using System.Collections;

public class AgeLevelSelection : MonoBehaviour {
	public GUIStyle buttonStyle;
	public GUIStyle textStyle;
	public GUIStyle confirmationStyle;
	public GUIStyle noButtonStyle;
	public GUIStyle yesButtonStyle;
	public GUIStyle backButton;
	public Texture bg;
	public Texture BgColor;
	public Texture2D es_yesButton;
	public Texture2D es_yesButtonHover;
	public Texture2D en_yesButton;
	public Texture2D en_yesButtonHover;
	float scale=1;
	float screenScale=1;
	public float yOffset;
	bool showConfirmation=false;
	bool showReturn;
	int action = -1;
	SessionManager sessionMng;
	LanguageLoader language;
	// Use this for initialization
	void Start () {
		scale = (float)Screen.width / (float)1366;
		screenScale = Screen.height / 768.0f;
		buttonStyle.fontSize = (int)((float)buttonStyle.fontSize * scale);
		textStyle.fontSize = (int)((float)textStyle.fontSize * scale);
		confirmationStyle.fontSize = (int)((float)confirmationStyle.fontSize * screenScale);
		backButton.fontSize = (int)((float)backButton.fontSize * screenScale);
		sessionMng = GetComponent<SessionManager> ();
		string lang = sessionMng.activeUser.language;
		if(lang=="")
			lang="es";
		language = GetComponent<LanguageLoader>();
		language.LoadGameLanguage(lang);
		switch(lang)
		{
			case "es":
				yesButtonStyle.normal.background=es_yesButton;
				yesButtonStyle.hover.background=es_yesButtonHover;
			break;
			case "en":
				yesButtonStyle.normal.background=en_yesButton;
				yesButtonStyle.hover.background=en_yesButtonHover;
			break;
		}
		showReturn = sessionMng.activeKid.ageSet;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void SetKinder()
	{
		sessionMng.activeKid.riverDifficulty = 0;
		sessionMng.activeKid.riverLevel = 0;
		sessionMng.activeKid.birdsDifficulty = 0;
		sessionMng.activeKid.birdsLevel = 0;
		sessionMng.activeKid.sandDifficulty = 0;
		sessionMng.activeKid.sandLevel = 0;
		sessionMng.activeKid.monkeyDifficulty = 0;
		sessionMng.activeKid.monkeyLevel = 0;
		sessionMng.activeKid.lavaDifficulty = 0;
		sessionMng.activeKid.lavaLevel = 0;
		sessionMng.activeKid.treasureDifficulty = 0;
		sessionMng.activeKid.treasureLevel = 0;
		sessionMng.activeKid.ageSet = true;

		sessionMng.activeKid.dontSyncArbolMusical=1;
		sessionMng.activeKid.dontSyncRio=1;
		sessionMng.activeKid.dontSyncArenaMagica=1;
		sessionMng.activeKid.dontSyncDondeQuedoLaBolita=1;
		sessionMng.activeKid.dontSyncSombras=1;
		sessionMng.activeKid.dontSyncTesoro=1;

		sessionMng.SaveSession ();
		if (sessionMng.activeKid.avatar != "") {
			Application.LoadLevel ("Archipielago");
		}else
		{
			Application.LoadLevel ("Selection");
		}
	}
	void SetPrimary()
	{
		sessionMng.activeKid.riverDifficulty = 1;
		sessionMng.activeKid.riverLevel = 0;
		sessionMng.activeKid.birdsDifficulty = 1;
		sessionMng.activeKid.birdsLevel = 0;
		sessionMng.activeKid.sandDifficulty = 0;
		sessionMng.activeKid.sandLevel = 8;
		sessionMng.activeKid.monkeyDifficulty = 0;
		sessionMng.activeKid.monkeyLevel = 0;
		sessionMng.activeKid.lavaDifficulty = 1;
		sessionMng.activeKid.lavaLevel = 0;
		sessionMng.activeKid.treasureDifficulty = 1;
		sessionMng.activeKid.treasureLevel = 0;
		sessionMng.activeKid.ageSet = true;

		sessionMng.activeKid.dontSyncArbolMusical=1;
		sessionMng.activeKid.dontSyncRio=1;
		sessionMng.activeKid.dontSyncArenaMagica=1;
		sessionMng.activeKid.dontSyncDondeQuedoLaBolita=1;
		sessionMng.activeKid.dontSyncSombras=1;
		sessionMng.activeKid.dontSyncTesoro=1;

		sessionMng.SaveSession ();
		if (sessionMng.activeKid.avatar != "") {
			Application.LoadLevel ("Archipielago");
		}else
		{
			Application.LoadLevel ("Selection");
		}
	}
	void SetPrimaryUpper()
	{
		sessionMng.activeKid.riverDifficulty = 2;
		sessionMng.activeKid.riverLevel = 0;
		sessionMng.activeKid.birdsDifficulty = 2;
		sessionMng.activeKid.birdsLevel = 0;
		sessionMng.activeKid.sandDifficulty = 0;
		sessionMng.activeKid.sandLevel = 15;
		sessionMng.activeKid.monkeyDifficulty = 2;
		sessionMng.activeKid.monkeyLevel = 0;
		sessionMng.activeKid.lavaDifficulty = 2;
		sessionMng.activeKid.lavaLevel = 0;
		sessionMng.activeKid.treasureDifficulty = 2;
		sessionMng.activeKid.treasureLevel = 0;
		sessionMng.activeKid.ageSet = true;

		sessionMng.activeKid.dontSyncArbolMusical=1;
		sessionMng.activeKid.dontSyncRio=1;
		sessionMng.activeKid.dontSyncArenaMagica=1;
		sessionMng.activeKid.dontSyncDondeQuedoLaBolita=1;
		sessionMng.activeKid.dontSyncSombras=1;
		sessionMng.activeKid.dontSyncTesoro=1;

		sessionMng.SaveSession ();
		if (sessionMng.activeKid.avatar != "") {
			Application.LoadLevel ("Archipielago");
		}else
		{
			Application.LoadLevel ("Selection");
		}
	}

	void OnGUI()
	{
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), bg);

		textStyle.normal.textColor=new Color(0.5f,0.3f,0.5f);
		GUI.Label (new Rect (Screen.width * 0.5f - 497 * scale, Screen.height * 0.5f - 267 * scale, 1000 * scale, 50 * scale), language.levelStrings[0],textStyle);
		textStyle.normal.textColor=new Color(0.9f,0.9f,0.9f);
		GUI.Label (new Rect (Screen.width * 0.5f - 500 * scale, Screen.height * 0.5f - 270 * scale, 1000 * scale, 50 * scale), language.levelStrings[0],textStyle);
		if(GUI.Button(new Rect(Screen.width*0.5f-150*scale,Screen.height*0.5f-250*scale+yOffset*scale,300*scale,150*scale),language.levelStrings[1],buttonStyle))
		{
			if(sessionMng.activeKid.ageSet)
			{
				action=0;
				showConfirmation=true;
			}
			else{
				SetKinder();
			}
		}
		if(GUI.Button(new Rect(Screen.width*0.5f-150*scale,Screen.height*0.5f-75*scale+yOffset*scale,300*scale,150*scale),language.levelStrings[2],buttonStyle))
		{
			if(sessionMng.activeKid.ageSet)
			{
				action=1;
				showConfirmation=true;
			}
			else{
				SetPrimary();
			}
		}
		if(GUI.Button(new Rect(Screen.width*0.5f-150*scale,Screen.height*0.5f+100*scale+yOffset*scale,300*scale,150*scale),language.levelStrings[3],buttonStyle))
		{
			if(sessionMng.activeKid.ageSet)
			{
				action=2;
				showConfirmation=true;
			}
			else{
				SetPrimaryUpper();
			}
		}
		if(showReturn)
		{
			if (GUI.Button (new Rect (0, Screen.height-80*screenScale, 190*screenScale,80*screenScale), language.levelStrings[4],backButton)) {
				if (sessionMng.activeKid.avatar != "") {
					Application.LoadLevel ("Archipielago");
				}else
				{
					Application.LoadLevel ("Selection");
				}
			}
		}
		if(showConfirmation)
		{	
			float offset=-200;
			GUI.DrawTexture(new Rect(Screen.width*0.5f-400*screenScale,Screen.height*0.5f+offset*screenScale,800*screenScale,150*screenScale),BgColor);
			confirmationStyle.normal.textColor=new Color(0.5f,0.5f,0.5f);
			GUI.Label(new Rect(Screen.width*0.5f-348*screenScale,Screen.height*0.5f+2*screenScale+offset*screenScale,700*screenScale,150*screenScale),language.levelStrings[5],confirmationStyle);
			confirmationStyle.normal.textColor=new Color(1,1,1);
			GUI.Label(new Rect(Screen.width*0.5f-350*screenScale,Screen.height*0.5f+offset*screenScale,700*screenScale,150*screenScale),language.levelStrings[5],confirmationStyle);
			if (GUI.Button (new Rect (Screen.width * 0.5f+255*screenScale, Screen.height * 0.5f+130*screenScale+offset*screenScale, 70 * screenScale, 70 * screenScale), "", noButtonStyle)) {
				showConfirmation=false;
			}
			if (GUI.Button (new Rect (Screen.width * 0.5f + 330 * screenScale, Screen.height * 0.5f+130*screenScale+offset*screenScale, 70 * screenScale, 70 * screenScale), "", yesButtonStyle)) {
				switch(action)
				{
					case 0:
						SetKinder();
					break;
					case 1:
						SetPrimary();
					break;
					case 2:
						SetPrimaryUpper();
					break;
				}
			}
		}
	}
}
