using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SelectorController : MonoBehaviour {
	Transform confetti;
	public Transform target;
	SelectionControl targetScript;

	float mapScale=1;
	public GUIStyle yesStyle;
	public Texture2D es_yesTexture;
	public Texture2D es_yesTextureHover;
	public Texture2D en_yesTexture;
	public Texture2D en_yesTextureHover;
	public GUIStyle noStyle;
	public GUIStyle textStyle;
	public GUIStyle readyButton;
	bool selected=false;
	bool fadeIn=false;
	bool fadeOut=true;
	float opacity=1;
	public enum state {None,Select,Transition,Confirmation,Finish,Loading,LoadingIntro};
	public state stateGame=state.LoadingIntro;
	state nextState=state.None;
	float waitTime=3f;
	public Texture loadingScreen;
	public Texture en_loadingScreen;
	public Texture loadingColor;
	SessionManager sessionMng;
	SoundManager soundMng;
	LanguageLoader language;
	// Use this for initialization
	void Start () {
		sessionMng = GetComponent<SessionManager> ();
		string lang = sessionMng.activeUser.language;
		if(lang=="")
			lang="es";
		language = GetComponent<LanguageLoader>();
		language.LoadGameLanguage(lang);
		switch(lang)
		{
		case "es":
			yesStyle.normal.background=es_yesTexture;
			yesStyle.hover.background=es_yesTextureHover;
			break;
		case "en":
			yesStyle.normal.background=en_yesTexture;
			yesStyle.hover.background=en_yesTextureHover;
			loadingScreen=en_loadingScreen;
			break;
		}
		soundMng = GetComponent<SoundManager> ();
		mapScale = (float)Screen.height / (float)768;
		textStyle.fontSize =(int)(textStyle.fontSize*mapScale);	
		readyButton.fontSize =(int)(readyButton.fontSize*mapScale);	
		confetti = transform.Find ("Confetti");
		PlayerPrefs.SetInt("Map",0);
		target = null;
		targetScript = null;
		fadeIn = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(target){
			if(stateGame==state.Select){
				nextState=state.Transition;
				fadeOut=true;
			}
			if(!targetScript)
				targetScript=target.GetComponent<SelectionControl>();
			confetti.transform.position=new Vector3(target.transform.position.x,confetti.transform.position.y,target.transform.position.z);
			if(!targetScript.getClose&&!selected&&stateGame==state.Transition){
				stateGame=state.Confirmation;
				fadeIn=true;
			}
		}
		if (stateGame == state.Finish) {
			waitTime-=Time.deltaTime;
			if(waitTime<=0){
				stateGame=state.Loading;
				fadeIn=true;
			}
		}
	}
	void OnGUI(){

		if(fadeIn){
			opacity+=1*Time.deltaTime;
			if(opacity>=1){
				opacity=1;
				fadeIn=false;
			}
			GUI.color=new Color(1,1,1,opacity);
		}else if (fadeOut) {
			opacity-=1*Time.deltaTime;
			if(opacity<=0){
				opacity=0;
				fadeOut=false;
				switch(stateGame){
					case state.Select:
						stateGame=state.Transition;
					break;
					case state.Confirmation:
						if(selected)
							stateGame=state.Finish;
						else
							stateGame=state.Select;
					break;
				}
				nextState=state.None;
			}
			GUI.color=new Color(1,1,1,opacity);
		}
		switch (stateGame) {
			case state.Select: {
				textStyle.alignment=TextAnchor.MiddleCenter;
				textStyle.normal.textColor=new Color(0.3f,0.3f,0.3f);
				GUI.Label (new Rect (Screen.width/2 - 248 * mapScale, Screen.height * 0.2f+2*mapScale, 500 * mapScale, 70 * mapScale), language.levelStrings[0], textStyle);
				textStyle.normal.textColor=new Color(0.9f,0.9f,0.9f);
				GUI.Label (new Rect (Screen.width/2 - 250 * mapScale, Screen.height * 0.2f, 500 * mapScale, 70 * mapScale), language.levelStrings[0], textStyle);
				break;
			}
			case state.Confirmation:{
				textStyle.alignment=TextAnchor.MiddleRight;
				textStyle.normal.textColor=new Color(0.3f,0.3f,0.3f);
				GUI.Label (new Rect (Screen.width * 0.7f - 798 * mapScale, Screen.height * 0.2f+2*mapScale, 850 * mapScale, 70 * mapScale), language.levelStrings[1], textStyle);
				textStyle.normal.textColor=new Color(0.9f,0.9f,0.9f);	
				GUI.Label (new Rect (Screen.width * 0.7f - 800 * mapScale, Screen.height * 0.2f, 850 * mapScale, 70 * mapScale), language.levelStrings[1], textStyle);
			//"seguro que quieres a este personaje"
				if(!selected){
					if (GUI.Button (new Rect (Screen.width * 0.77f, Screen.height * 0.2f, 70 * mapScale, 70 * mapScale), "", noStyle)) {
						nextState=state.Select;
						fadeOut=true;
						targetScript.resetSpotlight();
						target=null;
						targetScript=null;
					}
					if (GUI.Button (new Rect (Screen.width * 0.77f + 90 * mapScale, Screen.height * 0.2f, 70 * mapScale, 70 * mapScale), "", yesStyle)) {
						soundMng.PlaySound(0);
						int childCount=confetti.transform.childCount;
						/*for(int i=0;i<childCount;i++)
						{
							confetti.transform.GetChild(i).GetComponent<ParticleEmitter>().Emit();
						}*/
						selected=true;
						nextState=state.Finish;
						fadeOut=true;
						sessionMng.activeKid.avatar=target.name;
						sessionMng.SaveSession();
					}
				}
				break;
			}
			case state.Loading:
				GUI.DrawTexture(new Rect (0, 0, Screen.width, Screen.height), loadingColor);
				GUI.DrawTexture(new Rect(Screen.width/2-Screen.height/2,0,Screen.height,Screen.height),loadingScreen);
				if(!fadeIn&&!fadeOut)
				{
                    PrefsKeys.SetNextScene("GameMenus");
                    SceneManager.LoadScene("Loader_Scene");
					/*if(PlayerPrefs.HasKey("Tests"))
						Application.LoadLevel("Archipielago");
					else
						Application.LoadLevel("PruebaEcologica");*/
				}
			break;
			case state.LoadingIntro:
			GUI.DrawTexture(new Rect (0, 0, Screen.width, Screen.height), loadingColor);
			GUI.DrawTexture(new Rect(Screen.width/2-Screen.height/2,0,Screen.height,Screen.height),loadingScreen);
			if(!fadeIn&&!fadeOut)
				stateGame=state.Select;
			break;
		}
		if(sessionMng.activeKid.avatar!=""&&nextState!=state.Finish&&stateGame!=state.Finish&&stateGame!=state.Loading)
		{
			GUI.color=new Color(1,1,1,1);
			if (GUI.Button (new Rect (0, 0, 190*mapScale, 80*mapScale), language.levelStrings[2],readyButton)) {
				stateGame=state.Loading;
				fadeIn=true;
			}
		}
	}
}
