using UnityEngine;
using System.Collections;
using Boomlagoon.JSON;
using System;

public class Interface : MonoBehaviour {

	public Texture2D mapButtonTexture;
	public Texture2D mapButtonTextureHover;
	public Texture2D en_mapButtonTexture;
	public Texture2D en_mapButtonTextureHover;
	public Texture2D avatarButtonTexture;
	public Texture2D storeButtonTexture;
	public Texture2D storeButtonTextureHover;
    /*<DirectedGraph xmlns="http://schemas.microsoft.com/vs/2009/dgml">
  <Nodes>
    <Node Id="(@1 @2)" Visibility="Hidden" />
    <Node Id="(@3 Type=(Name=menu ParentType=Interface))" Visibility="Hidden" />
    <Node Id="@4" Category="CodeSchema_Class" CodeSchemaProperty_IsPublic="True" CommonLabel="Interface" Icon="Microsoft.VisualStudio.Class.Public" IsDragSource="True" Label="Interface" SourceLocation="(Assembly=file:///C:/Users/draga/Documents/Repos/TowiToGo/Assets/Scripts/Home/Interface.cs StartLineNumber=5 StartCharacterOffset=13 EndLineNumber=5 EndCharacterOffset=22)" />
  </Nodes>
  <Links>
    <Link Source="(@1 @2)" Target="@4" Category="Contains" />
    <Link Source="@4" Target="(@3 Type=(Name=menu ParentType=Interface))" Category="Contains" />
  </Links>
  <Categories>
    <Category Id="CodeSchema_Class" Label="Clase" BasedOn="CodeSchema_Type" Icon="CodeSchema_Class" />
    <Category Id="CodeSchema_Type" Label="Tipo" Icon="CodeSchema_Class" />
    <Category Id="Contains" Label="Contiene" Description="Indica si el origen del vínculo contiene el objeto de destino." IsContainment="True" />
  </Categories>
  <Properties>
    <Property Id="CodeSchemaProperty_IsPublic" Label="Público" Description="Marca para indicar que el ámbito es Public." DataType="System.Boolean" />
    <Property Id="CommonLabel" DataType="System.String" />
    <Property Id="Icon" Label="Icono" DataType="System.String" />
    <Property Id="IsContainment" DataType="System.Boolean" />
    <Property Id="IsDragSource" Label="IsDragSource" Description="IsDragSource" DataType="System.Boolean" />
    <Property Id="Label" Label="Etiqueta" Description="Etiqueta Displayable de un objeto Annotatable." DataType="System.String" />
    <Property Id="SourceLocation" Label="Número de línea de inicio" DataType="Microsoft.VisualStudio.GraphModel.CodeSchema.SourceLocation" />
    <Property Id="Visibility" Label="Visibilidad" Description="Define si un nodo del gráfico está visible o no." DataType="System.Windows.Visibility" />
  </Properties>
  <QualifiedNames>
    <Name Id="Assembly" Label="Ensamblado" ValueType="Uri" />
    <Name Id="File" Label="Archivo" ValueType="Uri" />
    <Name Id="Name" Label="Nombre" ValueType="System.String" />
    <Name Id="ParentType" Label="Tipo primario" ValueType="System.Object" />
    <Name Id="Type" Label="Tipo" ValueType="System.Object" />
  </QualifiedNames>
  <IdentifierAliases>
    <Alias n="1" Uri="Assembly=$(VsSolutionUri)/TowiToGo.csproj" />
    <Alias n="2" Uri="File=$(VsSolutionUri)/Assets/Scripts/Home/Interface.cs" />
    <Alias n="3" Uri="Assembly=$(2f2bc597-3b0c-d151-cab0-d98a96a15635.OutputPathUri)" />
    <Alias n="4" Id="(@3 Type=Interface)" />
  </IdentifierAliases>
  <Paths>
    <Path Id="2f2bc597-3b0c-d151-cab0-d98a96a15635.OutputPathUri" Value="file:///C:/Users/draga/Documents/Repos/TowiToGo/Temp/UnityVS_bin/Debug/Assembly-CSharp.dll" />
    <Path Id="VsSolutionUri" Value="file:///C:/Users/draga/Documents/Repos/TowiToGo" />
  </Paths>
</DirectedGraph>*/
	public Texture2D en_storeButtonTexture;
	public Texture2D en_storeButtonTextureHover;
	public Texture2D difficultyButtonTexture;
	public Texture2D difficultyButtonTextureHover;
	public Texture2D en_difficultyButtonTexture;
	public Texture2D en_difficultyButtonTextureHover;
	public Texture2D es_yesButtonTexture;
	public Texture2D es_yesButtonTextureHover;
	public Texture2D en_yesButtonTexture;
	public Texture2D en_yesButtonTextureHover;
	public GUIStyle mapButtonStyle;
	public GUIStyle avatarButtonStyle;
	public GUIStyle storeButtonStyle;
	public GUIStyle difficultyButtonStyle;
	public enum menu {Intro,Home,Map,Avatar,None,Loading,LoadingIntro};
	public menu interfaceState;
	menu nextState=menu.None;
	bool otherScene=false;
	int screenIdx=0;

	//Map Textures
	public Texture mapBack;
	public Texture mapFront;
	public Texture flag;
	Rect mapPosition;
	float mapScale=1;

	//Activity Textures
	public Texture locationActive;
	public Texture locationHover;
	public Texture activityBox;
	public Texture activityButton;
	public Texture loadingScreen;
	public Texture en_loadingScreen;
	public Texture arrow;
	public Texture BgColor;
	public Texture kiwiTexture;
	public Activity[]activityList;
	public GUIStyle activityButtonStyle;
	public GUIStyle titleStyle;
	public GUIStyle descriptionStyle;
	public GUIStyle locationStyle;
	public GUIStyle exitMapStyle;
	public GUIStyle exitActivityStyle;
	public GUIStyle textStyle;
	public GUIStyle kiwiScoreStyle;
	public GUIStyle yesButtonStyle;
	public GUIStyle noButtonStyle;
	public GUIStyle confirmationStyle;
	int kiwis=0;
	int activeMission;
	int nextActiveMission;
	float opacityActivity=0;
	float arrowSin=0;
	bool showArrow=true;
	int arrowIdx=-1;
	public bool fadeInActivity=false;
	public bool fadeOutActivity=false;
	public Texture loadingColor;

	float opacity=1;
	public bool fadeIn=false;
	public bool fadeOut=false;
	public bool hideUI;
	bool showConfirmation=false;
	SessionManager sessionMng;

	//Warnings
	public GUIStyle warningStyle;
	bool showWarning = false;
	int warningType = 0;
	float warningTime = 0;
	int warningIdx=0;

	LanguageLoader language;

	void Awake()
	{
		Resources.UnloadUnusedAssets ();
	}
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
				mapButtonStyle.normal.background = mapButtonTexture;
				mapButtonStyle.hover.background = mapButtonTextureHover;
				storeButtonStyle.normal.background=storeButtonTexture;
				storeButtonStyle.hover.background=storeButtonTextureHover;
				difficultyButtonStyle.normal.background=difficultyButtonTexture;
				difficultyButtonStyle.hover.background=difficultyButtonTextureHover;
				yesButtonStyle.normal.background=es_yesButtonTexture;
				yesButtonStyle.hover.background=es_yesButtonTextureHover;
			break;
			case "en":
				mapButtonStyle.normal.background = en_mapButtonTexture;
				mapButtonStyle.hover.background = en_mapButtonTextureHover;
				storeButtonStyle.normal.background=en_storeButtonTexture;
				storeButtonStyle.hover.background=en_storeButtonTextureHover;
				difficultyButtonStyle.normal.background=en_difficultyButtonTexture;
				difficultyButtonStyle.hover.background=en_difficultyButtonTextureHover;
				yesButtonStyle.normal.background=en_yesButtonTexture;
				yesButtonStyle.hover.background=en_yesButtonTextureHover;
				loadingScreen=en_loadingScreen;
			break;
		}
		int langIdx = 0;
		for(int i=0;i<activityList.Length;i++)
		{
			activityList[i].title=language.levelStrings[langIdx++];
			activityList[i].description=language.levelStrings[langIdx++];
		}
		fadeOut = true;
		hideUI = false;
		mapScale = (float)Screen.height / (float)mapFront.height;
		mapPosition=new Rect(Screen.width/2-(mapFront.width*mapScale)/2,0,mapFront.width*mapScale,Screen.height);
		kiwis = sessionMng.activeKid.kiwis;
		for (int i=0; i<activityList.Length; i++) {
			activityList[i].active=false;
			activityList[i].unlocked=false;
			activityList[i].today=false;
		}
		ReadSchedule ();
		//if (activityList.Length > 0)
		//	activityList [0].active = true;
		activeMission = -1;
		opacityActivity = 0;
		titleStyle.fontSize =(int)(48 * mapScale);
		descriptionStyle.fontSize=(int)(24 * mapScale);
		textStyle.fontSize = (int)((float)textStyle.fontSize * (float)mapScale);
		kiwiScoreStyle.fontSize = (int)((float)kiwiScoreStyle.fontSize * (float)mapScale);
		confirmationStyle.fontSize = (int)((float)confirmationStyle.fontSize * (float)mapScale);
		warningStyle.fontSize = (int)((float)warningStyle.fontSize * (float)mapScale);
		interfaceState = menu.LoadingIntro;
		
		
		//Handheld.ClearShaderCache();
		/*sessionMng.activeKid.playedSombras = 0;
		sessionMng.activeKid.playedDondeQuedoLaBolita = 0;
		sessionMng.activeKid.playedArenaMagica = 0;
		sessionMng.activeKid.playedRio = 0;
		sessionMng.activeKid.playedArbolMusical = 0;
		sessionMng.activeKid.playedTesoro = 0;

		sessionMng.activeKid.blockedSombras = 0;
		sessionMng.activeKid.blockedDondeQuedoLaBolita = 0;
		sessionMng.activeKid.blockedArenaMagica = 0;
		sessionMng.activeKid.blockedRio = 0;
		sessionMng.activeKid.blockedArbolMusical = 0;
		sessionMng.activeKid.blockedTesoro = 0;*/
	}

	void OnGUI(){
		if(hideUI)
			GUI.color=new Color(1,1,1,0);
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
				showWarning=false;
				opacity=0;
				fadeOut=false;
				if(interfaceState!=menu.LoadingIntro){
					interfaceState=nextState;
					fadeIn=true;
				}
			}
			GUI.color=new Color(1,1,1,opacity);
		}
		switch (interfaceState) {
			case menu.Home:
			if(GUI.Button(new Rect(Screen.width*0.2f,Screen.height-mapButtonTexture.height*mapScale,mapButtonTexture.width*mapScale,mapButtonTexture.height*mapScale),"",mapButtonStyle)){
				if(!showConfirmation)
				{
					fadeOut=true;
					nextState=menu.Map;
				}
			}
			if(GUI.Button(new Rect(Screen.width*0.38f,Screen.height-avatarButtonTexture.height*0.78f*mapScale,avatarButtonTexture.width*0.78f*mapScale,avatarButtonTexture.height*0.78f*mapScale),"",avatarButtonStyle)){
				if(!showConfirmation)
				{
					fadeOut=true;
					otherScene=true;
					screenIdx=0;
					nextState=menu.Loading;
				}
			}
			if(GUI.Button(new Rect(Screen.width*0.5f,Screen.height-storeButtonTexture.height*0.78f*mapScale,storeButtonTexture.width*0.78f*mapScale,storeButtonTexture.height*0.78f*mapScale),"",storeButtonStyle)){
				if(!showConfirmation)
				{
					fadeOut=true;
					otherScene=true;
					screenIdx=1;
					nextState=menu.Loading;
				}
			}
			if(GUI.Button(new Rect(Screen.width*0.635f,Screen.height-difficultyButtonTexture.height*0.78f*mapScale,difficultyButtonTexture.width*0.78f*mapScale,difficultyButtonTexture.height*0.78f*mapScale),"",difficultyButtonStyle)){
				if(!showConfirmation)
				{
					fadeOut=true;
					otherScene=true;
					screenIdx=2;
					nextState=menu.Loading;
				}
			}
			if(GUI.Button(new Rect(0,0,172*mapScale,68*mapScale),"",exitMapStyle)&&!fadeOut){
				showConfirmation=true;
			}
			titleStyle.normal.textColor=new Color(0,0,0);
			GUI.Label(new Rect(0,0,172*mapScale,68*mapScale),language.levelStrings[16],textStyle);
			titleStyle.normal.textColor=new Color(0.32f,0.32f,0.32f);
			if(showConfirmation)
			{	
				float offset=-200;
				GUI.DrawTexture(new Rect(Screen.width*0.5f-400*mapScale,Screen.height*0.5f+offset*mapScale,800*mapScale,150*mapScale),BgColor);
				confirmationStyle.normal.textColor=new Color(0.5f,0.5f,0.5f);
				GUI.Label(new Rect(Screen.width*0.5f-348*mapScale,Screen.height*0.5f+2*mapScale+offset*mapScale,700*mapScale,150*mapScale),language.levelStrings[12],confirmationStyle);
				confirmationStyle.normal.textColor=new Color(1,1,1);
				GUI.Label(new Rect(Screen.width*0.5f-350*mapScale,Screen.height*0.5f+offset*mapScale,700*mapScale,150*mapScale),language.levelStrings[12],confirmationStyle);
				if (GUI.Button (new Rect (Screen.width * 0.5f+255*mapScale, Screen.height * 0.5f+130*mapScale+offset*mapScale, 70 * mapScale, 70 * mapScale), "", noButtonStyle)) {
					showConfirmation=false;
				}
				if (GUI.Button (new Rect (Screen.width * 0.5f + 330 * mapScale, Screen.height * 0.5f+130*mapScale+offset*mapScale, 70 * mapScale, 70 * mapScale), "", yesButtonStyle)) {
					Application.LoadLevel("Login");
				}
			}
			GUI.DrawTexture(new Rect(Screen.width-300*mapScale,20*mapScale,90*mapScale,90*mapScale),kiwiTexture);
			GUI.Label(new Rect(Screen.width-200*mapScale,30*mapScale,90*mapScale,90*mapScale),kiwis.ToString(),kiwiScoreStyle);
			break;
			case menu.Map:
				arrowSin+=Time.deltaTime;
				arrowSin%=Mathf.PI;
				GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),mapBack);
				GUI.DrawTexture(mapPosition,mapFront);
				if(GUI.Button(new Rect(Screen.width-172*mapScale,Screen.height-68*mapScale,172*mapScale,68*mapScale),"",exitMapStyle)&&!fadeInActivity&&!fadeOutActivity){
					PlayerPrefs.SetInt("Map",0);
					fadeOut=true;
					nextState=menu.Home;
				}
				titleStyle.normal.textColor=new Color(0,0,0);
				GUI.Label(new Rect(Screen.width-172*mapScale,Screen.height-68*mapScale,172*mapScale,68*mapScale),language.levelStrings[16],textStyle);
				titleStyle.normal.textColor=new Color(0.32f,0.32f,0.32f);
				bool allLocked=true;
				for(int i=0;i<activityList.Length;i++){
					if(!activityList[i].today)
					{
						Color currColor=GUI.color;
						GUI.color=new Color(1,1,1,currColor.a*0.5f);
						if(GUI.Button(new Rect(mapPosition.x+(activityList[i].position.x-16)*mapScale,(activityList[i].position.y-16)*mapScale,50*mapScale,50*mapScale),"",locationStyle)&&!fadeInActivity&&!fadeOutActivity&&GUIUtility.hotControl==0)
						{
							showWarning=true;
							warningType=0;
							warningTime=6;
							warningIdx=i;
						}
						GUI.color=currColor;
					}else
					{
						if(activityList[i].unlocked)
						{
							allLocked=false;
							if(!fadeIn&&!fadeOut)
								GUI.color=new Color(1,1,1,1);
							if(activityList[i].active){
								GUI.DrawTexture(new Rect(mapPosition.x+(activityList[i].position.x-16)*mapScale,(activityList[i].position.y-16)*mapScale,50*mapScale,50*mapScale),locationHover);
							}else{
								if(GUI.Button(new Rect(mapPosition.x+(activityList[i].position.x-16)*mapScale,(activityList[i].position.y-16)*mapScale,50*mapScale,50*mapScale),"",locationStyle)&&!fadeInActivity&&!fadeOutActivity&&GUIUtility.hotControl==0)
								{
									if(activeMission==-1){
										showArrow=false;
										activeMission=i;
										activityList[activeMission].active=true;
										fadeInActivity=true;
									}else{
										Vector2 activityPos=new Vector2(100*mapScale,100*mapScale);	
										Rect testCollision=new Rect(activityPos.x+(656+130-229)*mapScale,activityPos.y+260*mapScale,229*mapScale,92*mapScale);
										if(!testCollision.Contains(new Vector2(Input.mousePosition.x,Screen.height-Input.mousePosition.y))){
											nextActiveMission=i;
											fadeOutActivity=true;
										}else{
											fadeOut=true;
											nextState=menu.Loading;
										}
									}
								}
							}
						}else
						{
							if(!fadeIn&&!fadeOut)
								GUI.color=new Color(1,1,1,0.5f);
							if(GUI.Button(new Rect(mapPosition.x+(activityList[i].position.x-16)*mapScale,(activityList[i].position.y-16)*mapScale,50*mapScale,50*mapScale),"",locationStyle)&&!fadeInActivity&&!fadeOutActivity&&GUIUtility.hotControl==0)
							{
								showWarning=true;
								warningType=1;
								warningTime=6;
								warningIdx=i;
							}
						}
					}
					if(activeMission!=i&&activityList[i].unlocked&&activityList[i].today)
						GUI.DrawTexture(new Rect(mapPosition.x+(activityList[i].position.x-13)*mapScale,(activityList[i].position.y-46)*mapScale,56*mapScale,60*mapScale),flag);
				}
				if(!fadeIn&&!fadeOut)
					GUI.color=new Color(1,1,1,1);
				if(showArrow&&!allLocked)
					GUI.DrawTexture(new Rect(mapPosition.x+(activityList[arrowIdx].position.x-23)*mapScale,(activityList[arrowIdx].position.y-100-20*Mathf.Sin(arrowSin))*mapScale,67*mapScale,89*mapScale),arrow);
				if(allLocked)
				{
					GUI.DrawTexture(new Rect(Screen.width*0.5f-500*mapScale,Screen.height*0.5f-200*mapScale,1000*mapScale,400*mapScale),BgColor);
				GUI.Label(new Rect(Screen.width*0.5f-450*mapScale,Screen.height*0.5f-50*mapScale,900*mapScale,100*mapScale),language.levelStrings[13],textStyle);
				}
				if(fadeInActivity){
					opacityActivity+=1*Time.deltaTime;
					if(opacityActivity>=1){
						opacityActivity=1;
						fadeInActivity=false;
					}
					GUI.color=new Color(1,1,1,opacityActivity);
				}else if (fadeOutActivity) {
					opacityActivity-=1*Time.deltaTime;
					if(opacityActivity<=0){
						opacityActivity=0;
						fadeOutActivity=false;
						activityList[activeMission].active=false;
						activeMission=nextActiveMission;
						if(activeMission!=-1){
							activityList[activeMission].active=true;
							fadeInActivity=true;
						}
					}
					GUI.color=new Color(1,1,1,opacityActivity);
				}

				if(activeMission!=-1){
					Vector2 activityPos=new Vector2(100*mapScale,100*mapScale);		
					GUIHelper.DrawLine(new Vector2(activityPos.x+128*mapScale, activityPos.y+128*mapScale), new Vector2(mapPosition.x+(activityList[activeMission].position.x)*mapScale, (activityList[activeMission].position.y)*mapScale), new Color(0.07f,0.13f,0.33f),(int)(4*mapScale));
					if(!fadeOut&&!fadeIn)
						GUI.color=new Color(1,1,1,1);	
					GUI.DrawTexture(new Rect(mapPosition.x+(activityList[activeMission].position.x-13)*mapScale,(activityList[activeMission].position.y-46)*mapScale,56*mapScale,60*mapScale),flag);
					if(!fadeOut&&!fadeIn)
						GUI.color=new Color(1,1,1,opacityActivity);	
					GUI.DrawTexture(new Rect(activityPos.x+128*mapScale,activityPos.y+40*mapScale,656*mapScale,240*mapScale),activityBox);
					GUI.TextArea(new Rect(activityPos.x+350*mapScale,activityPos.y+50*mapScale,380*mapScale,100*mapScale),activityList[activeMission].title,titleStyle);
					GUI.TextArea(new Rect(activityPos.x+350*mapScale,activityPos.y+170*mapScale,380*mapScale,110*mapScale),activityList[activeMission].description,descriptionStyle);
					GUI.DrawTexture(new Rect(activityPos.x,activityPos.y,319*mapScale,316*mapScale),activityList[activeMission].preview);
					//GUI.DrawTexture(new Rect(activityPos.x+(656+130-229)*mapScale,activityPos.y+260*mapScale,229*mapScale,92*mapScale),activityButton);
					if(GUI.Button(new Rect(activityPos.x+(656+130-229)*mapScale,activityPos.y+260*mapScale,229*mapScale,92*mapScale),"",activityButtonStyle)&&!fadeInActivity&&!fadeOutActivity){
						//Application.LoadLevel(activityList[activeMission].key);	
						//LoadLevel(activityList[activeMission].key);
						PlayerPrefs.SetInt("Map",1);
						fadeOut=true;
						nextState=menu.Loading;
						PlayerPrefs.SetInt("Map",1);
					}
					titleStyle.normal.textColor=new Color(0,0,0);
					GUI.Label(new Rect(activityPos.x+(656+130-229)*mapScale,activityPos.y+260*mapScale,229*mapScale,92*mapScale),language.levelStrings[17],textStyle);
					titleStyle.normal.textColor=new Color(0.32f,0.32f,0.32f);
					if(GUI.Button(new Rect(activityPos.x+726*mapScale,activityPos.y+30*mapScale,70*mapScale,70*mapScale),"",exitActivityStyle)&&!fadeInActivity&&!fadeOutActivity){
						nextActiveMission=-1;
						fadeOutActivity=true;
					}
				}
			if(showWarning)
			{
				if(!fadeIn&&!fadeOut)
					GUI.color=new Color(1,1,1,1);
				GUI.BeginGroup(new Rect(mapPosition.x+activityList[warningIdx].position.x*mapScale-150*mapScale,activityList[warningIdx].position.y*mapScale-160*mapScale,300*mapScale,150*mapScale));
				warningTime-=Time.deltaTime;
				if(warningTime<=0)
				{
					showWarning=false;
				}
				switch(warningType)
				{
					case 0:
						GUI.DrawTexture(new Rect(0,0,300*mapScale,150*mapScale),BgColor);
					GUI.Label(new Rect(0,0,280*mapScale,140*mapScale),language.levelStrings[14],warningStyle);
					break;
					case 1:
						GUI.DrawTexture(new Rect(0,0,300*mapScale,200*mapScale),BgColor);
					GUI.Label(new Rect(0,0,280*mapScale,140*mapScale),language.levelStrings[15],warningStyle);
					break;
				}
				GUI.EndGroup();
			}
			break;
			case menu.Loading:
				GUI.DrawTexture(new Rect (0, 0, Screen.width, Screen.height), loadingColor);
				GUI.DrawTexture(new Rect(Screen.width/2-Screen.height/2,0,Screen.height,Screen.height),loadingScreen);
				if(!fadeIn&&!fadeOut){
					if(otherScene)
					{
						switch(screenIdx)
						{
							case 0:
								Application.LoadLevel("Selection");
							break;
							case 1:
								Application.LoadLevel("TiendaRopa");
							break;
							case 2:
								Application.LoadLevel("AgeSelection");
							break;
						}
					}
					else
					{	
						Application.LoadLevel(activityList[activeMission].key);	
					}
				}
			break;
			case menu.LoadingIntro:
				GUI.DrawTexture(new Rect (0, 0, Screen.width, Screen.height), loadingColor);
				GUI.DrawTexture(new Rect(Screen.width/2-Screen.height/2,0,Screen.height,Screen.height),loadingScreen);
				if(!fadeIn&&!fadeOut){
					interfaceState=menu.Intro;
					CameraHandler tempCameraH =GameObject.Find("Main Camera").GetComponent<CameraHandler>();
					if(tempCameraH.enableUI){
						tempCameraH.showUI();
					}
				}
			break;
		}
	}

	public void UpdateBlocked()
	{
		bool setArrow=false;
		for(int idx=0;idx<activityList.Length;idx++){
			switch(activityList[idx].key)
			{
			case "DibujoFrutal":
				activityList[idx].unlocked=sessionMng.activeKid.blockedArenaMagica==0;
				break;
			case "RecoleccionTesoro":
				activityList[idx].unlocked=sessionMng.activeKid.blockedTesoro==0;
				break;
			case "Rio":
				activityList[idx].unlocked=sessionMng.activeKid.blockedRio==0;
				break;
			case "DondeQuedoLaBolita":
				activityList[idx].unlocked=sessionMng.activeKid.blockedDondeQuedoLaBolita==0;
				break;
			case "Sombras":
				activityList[idx].unlocked=sessionMng.activeKid.blockedSombras==0;
				break;
			case "ArbolMusical":
				activityList[idx].unlocked=sessionMng.activeKid.blockedArbolMusical==0;
				break;
			}
			if(activityList[idx].unlocked&&activityList[idx].today&&!setArrow)
			{
				setArrow=true;
				arrowIdx=idx;
			}
		}
	}

	IEnumerator LoadLevel(string key){
		AsyncOperation async = Application.LoadLevelAsync(key);
		yield return async;
		Debug.Log("Loading complete");
	}
	
	JSONObject CreateTestList(){
		JSONObject tempList = new JSONObject ();
		JSONArray tempArray = new JSONArray ();
		JSONArray scheduleArray = new JSONArray ();
		JSONObject tempObject = new JSONObject ();
		tempArray.Add ("DibujoFrutal");
		tempArray.Add ("Rio");
		tempArray.Add("RecoleccionTesoro");
		tempArray.Add("ArbolMusical");
		tempArray.Add("DondeQuedoLaBolita");
		tempArray.Add("Sombras");
		tempObject.Add ("day", tempArray);
		scheduleArray.Add (tempObject);
		tempObject = new JSONObject ();
		tempArray = new JSONArray ();
		tempArray.Add ("ArbolMusical");
		tempArray.Add ("DondeQuedoLaBolita");
		tempArray.Add("Sombras");
		tempObject.Add ("day", tempArray);
		scheduleArray.Add (tempObject);
		tempObject = new JSONObject ();
		tempArray = new JSONArray ();
		tempArray.Add ("Sombras");
		tempArray.Add ("Rio");
		tempArray.Add("ArbolMusical");
		tempArray.Add("RecoleccionTesoro");
		tempObject.Add ("day", tempArray);
		scheduleArray.Add (tempObject);
		tempObject = new JSONObject ();
		tempArray = new JSONArray ();
		tempArray.Add ("DibujoFrutal");
		tempArray.Add ("ArbolMusical");
		tempArray.Add("DondeQuedoLaBolita");
		tempObject.Add ("day", tempArray);
		scheduleArray.Add (tempObject);
		tempObject = new JSONObject ();
		tempArray = new JSONArray ();
		tempList.Add ("data", scheduleArray);
		Debug.Log (tempList.ToString ());
		return tempList;
	}
	void ReadSchedule(){
		int day=System.DateTime.Now.DayOfYear;
		int activeDay = -1;

		activeDay=sessionMng.activeKid.activeDay;

		//Modificacion para Testng
		//sessionMng.activeKid.activeMissions = "";
		//sessionMng.activeKid.missionList = "";

		if(day!=activeDay||sessionMng.activeKid.activeMissions.ToString()=="")
		{

			//Modificacion para Testng
			//LoadSchedule(CreateTestList ());


			/*if (sessionMng.activeKid.missionList!="") {
				string listJson = sessionMng.activeKid.missionList;
				JSONObject listObject=JSONObject.Parse(listJson);
				LoadSchedule(listObject);
			} else {
				LoadSchedule(CreateTestList ());
			}*/
			activeDay=day;

			sessionMng.activeKid.activeDay=activeDay;
			sessionMng.activeKid.playedLava=0;
			sessionMng.activeKid.playedMonkey=0;
			sessionMng.activeKid.playedSand=0;
			sessionMng.activeKid.playedRiver=0;
			sessionMng.activeKid.playedBird=0;
			sessionMng.activeKid.playedTreasure=0;

			sessionMng.activeKid.blockedSombras=0;
			sessionMng.activeKid.blockedDondeQuedoLaBolita=0;
			sessionMng.activeKid.blockedArenaMagica=0;
			sessionMng.activeKid.blockedRio=0;
			sessionMng.activeKid.blockedArbolMusical=0;
			sessionMng.activeKid.blockedTesoro=0;
		}else{
			JSONObject missions=JSONObject.Parse(sessionMng.activeKid.activeMissions.ToString());
			//JSONArray missionsA=missionsfull["data"].Array;
			//JSONObject missions=missionsA[0].Obj;
 			JSONArray dayData=missions["day"].Array;
			for(int i=0;i<dayData.Length;i++){
				for(int idx=0;idx<activityList.Length;idx++){
					if(activityList[idx].key==dayData[i].Str){
						switch(activityList[idx].key)
						{
						case "DibujoFrutal":
							activityList[idx].unlocked=sessionMng.activeKid.blockedArenaMagica==0;
							activityList[idx].today=true;
							break;
						case "RecoleccionTesoro":
							activityList[idx].unlocked=sessionMng.activeKid.blockedTesoro==0;
							activityList[idx].today=true;
							break;
						case "Rio":
							activityList[idx].unlocked=sessionMng.activeKid.blockedRio==0;
							activityList[idx].today=true;
								break;
						case "DondeQuedoLaBolita":
							activityList[idx].unlocked=sessionMng.activeKid.blockedDondeQuedoLaBolita==0;
							activityList[idx].today=true;
							break;
						case "Sombras":
							activityList[idx].unlocked=sessionMng.activeKid.blockedSombras==0;
							activityList[idx].today=true;
							break;
						case "ArbolMusical":
							activityList[idx].unlocked=sessionMng.activeKid.blockedArbolMusical==0;
							activityList[idx].today=true;
							break;
						}
						if(arrowIdx==-1&&activityList[idx].unlocked)
							arrowIdx=idx;
					}
				}
			}
		}
		sessionMng.SaveSession ();
	}

	void LoadSchedule(JSONObject listObject){
		JSONArray listArray=listObject["data"].Array;
		JSONArray dayData=listArray[0].Obj["day"].Array;
		for(int i=0;i<dayData.Length;i++){
			for(int idx=0;idx<activityList.Length;idx++){
				if(activityList[idx].key==dayData[i].Str){
					//Modificacion Pruebas
					//Debug.Log("Blocked"+activityList[idx].key+": "+PlayerPrefs.GetInt("Blocked"+activityList[idx].key,0).ToString());

					switch(activityList[idx].key)
					{
						case "DibujoFrutal":
							sessionMng.activeKid.blockedArenaMagica=0;
							activityList[idx].unlocked=sessionMng.activeKid.blockedArenaMagica==0;
							activityList[idx].today=true;
							break;
						case "RecoleccionTesoro":
							sessionMng.activeKid.blockedTesoro=0;
							activityList[idx].unlocked=sessionMng.activeKid.blockedTesoro==0;
							activityList[idx].today=true;
							break;
						case "Rio":
							sessionMng.activeKid.blockedRio=0;
							activityList[idx].unlocked=sessionMng.activeKid.blockedRio==0;
							activityList[idx].today=true;
							break;
						case "DondeQuedoLaBolita":
							sessionMng.activeKid.blockedDondeQuedoLaBolita=0;
							activityList[idx].unlocked=sessionMng.activeKid.blockedDondeQuedoLaBolita==0;
							activityList[idx].today=true;
							break;
						case "Sombras":
							sessionMng.activeKid.blockedSombras=0;
							activityList[idx].unlocked=sessionMng.activeKid.blockedSombras==0;
							activityList[idx].today=true;
							break;
						case "ArbolMusical":
							sessionMng.activeKid.blockedArbolMusical=0;
							activityList[idx].unlocked=sessionMng.activeKid.blockedArbolMusical==0;
							activityList[idx].today=true;
							break;
					}
					//Debug.Log(activityList[idx].unlocked);
					if(arrowIdx==-1&&activityList[idx].unlocked)
						arrowIdx=idx;
					sessionMng.SaveSession();
				}
			}
		}
		sessionMng.activeKid.activeMissions[0] = listArray [0].Obj.ToString ();
		listArray.Remove(0);
		if(listArray.Length<=0){
			//sessionMng.activeKid.missionList ="";
		}else{
			//sessionMng.activeKid.missionList =listObject.ToString();
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
	[System.Serializable]
	public class Activity
	{
		public string key;
		public Vector2 position;
		public Texture preview;
		public string title;
		public string description;
		public bool active;
		public bool unlocked;
		public bool today;
	}

	public static class GUIHelper
		
	{
		
		// The texture used by DrawLine(Color)
		
		private static Texture2D _coloredLineTexture;
		
		
		
		// The color used by DrawLine(Color)
		
		private static Color _coloredLineColor;
		
		
		
		/// <summary>
		
		/// Draw a line between two points with the specified color and a thickness of 1
		
		/// </summary>
		
		/// <param name="lineStart">The start of the line</param>
		
		/// <param name="lineEnd">The end of the line</param>
		
		/// <param name="color">The color of the line</param>
		
		public static void DrawLine(Vector2 lineStart, Vector2 lineEnd, Color color)
			
		{
			
			DrawLine(lineStart, lineEnd, color, 1);
			
		}
		
		
		
		/// <summary>
		
		/// Draw a line between two points with the specified color and thickness
		
		/// Inspired by code posted by Sylvan
		
		/// http://forum.unity3d.com/threads/17066-How-to-draw-a-GUI-2D-quot-line-quot?p=407005&viewfull=1#post407005
		
		/// </summary>
		
		/// <param name="lineStart">The start of the line</param>
		
		/// <param name="lineEnd">The end of the line</param>
		
		/// <param name="color">The color of the line</param>
		
		/// <param name="thickness">The thickness of the line</param>
		
		public static void DrawLine(Vector2 lineStart, Vector2 lineEnd, Color color, int thickness)
			
		{
			
			if (_coloredLineTexture == null || _coloredLineColor != color)
				
			{
				
				_coloredLineColor = color;
				
				_coloredLineTexture = new Texture2D(1, 1);
				
				_coloredLineTexture.SetPixel(0, 0, _coloredLineColor);
				
				_coloredLineTexture.wrapMode = TextureWrapMode.Repeat;
				
				_coloredLineTexture.Apply();
				
			}
			
			DrawLineStretched(lineStart, lineEnd, _coloredLineTexture, thickness);
			
		}
		
		
		
		/// <summary>
		
		/// Draw a line between two points with the specified texture and thickness.
		
		/// The texture will be stretched to fill the drawing rectangle.
		
		/// Inspired by code posted by Sylvan
		
		/// http://forum.unity3d.com/threads/17066-How-to-draw-a-GUI-2D-quot-line-quot?p=407005&viewfull=1#post407005
		
		/// </summary>
		
		/// <param name="lineStart">The start of the line</param>
		
		/// <param name="lineEnd">The end of the line</param>
		
		/// <param name="texture">The texture of the line</param>
		
		/// <param name="thickness">The thickness of the line</param>
		
		public static void DrawLineStretched(Vector2 lineStart, Vector2 lineEnd, Texture2D texture, int thickness)
			
		{
			
			Vector2 lineVector = lineEnd - lineStart;
			
			float angle = Mathf.Rad2Deg * Mathf.Atan(lineVector.y / lineVector.x);
			
			if (lineVector.x < 0)
				
			{
				
				angle += 180;
				
			}
			
			
			
			if (thickness < 1)
				
			{
				
				thickness = 1;
				
			}
			
			
			
			// The center of the line will always be at the center
			
			// regardless of the thickness.
			
			int thicknessOffset = (int)Mathf.Ceil(thickness / 2);
			
			
			
			GUIUtility.RotateAroundPivot(angle,
			                             
			                             lineStart);
			
			GUI.DrawTexture(new Rect(lineStart.x,
			                         
			                         lineStart.y - thicknessOffset,
			                         
			                         lineVector.magnitude,
			                         
			                         thickness),
			                
			                texture);
			
			GUIUtility.RotateAroundPivot(-angle, lineStart);
			
		}
		
		
		
		/// <summary>
		
		/// Draw a line between two points with the specified texture and a thickness of 1
		
		/// The texture will be repeated to fill the drawing rectangle.
		
		/// </summary>
		
		/// <param name="lineStart">The start of the line</param>
		
		/// <param name="lineEnd">The end of the line</param>
		
		/// <param name="texture">The texture of the line</param>
		
		public static void DrawLine(Vector2 lineStart, Vector2 lineEnd, Texture2D texture)
			
		{
			
			DrawLine(lineStart, lineEnd, texture, 1);
			
		}
		
		
		
		/// <summary>
		
		/// Draw a line between two points with the specified texture and thickness.
		
		/// The texture will be repeated to fill the drawing rectangle.
		
		/// Inspired by code posted by Sylvan and ArenMook
		
		/// http://forum.unity3d.com/threads/17066-How-to-draw-a-GUI-2D-quot-line-quot?p=407005&viewfull=1#post407005
		
		/// http://forum.unity3d.com/threads/28247-Tile-texture-on-a-GUI?p=416986&viewfull=1#post416986
		
		/// </summary>
		
		/// <param name="lineStart">The start of the line</param>
		
		/// <param name="lineEnd">The end of the line</param>
		
		/// <param name="texture">The texture of the line</param>
		
		/// <param name="thickness">The thickness of the line</param>
		
		public static void DrawLine(Vector2 lineStart, Vector2 lineEnd, Texture2D texture, int thickness)
			
		{
			
			Vector2 lineVector = lineEnd - lineStart;
			
			float angle = Mathf.Rad2Deg * Mathf.Atan(lineVector.y / lineVector.x);
			
			if (lineVector.x < 0)
				
			{
				
				angle += 180;
				
			}
			
			
			
			if (thickness < 1)
				
			{
				
				thickness = 1;
				
			}
			
			
			
			// The center of the line will always be at the center
			
			// regardless of the thickness.
			
			int thicknessOffset = (int)Mathf.Ceil(thickness / 2);
			
			
			
			Rect drawingRect = new Rect(lineStart.x,
			                            
			                            lineStart.y - thicknessOffset,
			                            
			                            Vector2.Distance(lineStart, lineEnd),
			                            
			                            (float) thickness);
			
			GUIUtility.RotateAroundPivot(angle,
			                             
			                             lineStart);
			
			GUI.BeginGroup(drawingRect);
			
			{
				
				int drawingRectWidth = Mathf.RoundToInt(drawingRect.width);
				
				int drawingRectHeight = Mathf.RoundToInt(drawingRect.height);
				
				
				
				for (int y = 0; y < drawingRectHeight; y += texture.height)
					
				{
					
					for (int x = 0; x < drawingRectWidth; x += texture.width)
						
					{
						
						GUI.DrawTexture(new Rect(x,
						                         
						                         y,
						                         
						                         texture.width,
						                         
						                         texture.height),
						                
						                texture);
						
					}
					
				}
				
			}
			
			GUI.EndGroup();
			
			GUIUtility.RotateAroundPivot(-angle, lineStart);
			
		}
		
	}

}

