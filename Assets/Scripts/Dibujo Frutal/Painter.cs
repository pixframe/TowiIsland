using UnityEngine;
using System.Collections;
using CoreDraw;
using Boomlagoon.JSON;

public class Painter : TouchLogicBU
{
	AudioSource audSrc;
	
	public Texture2D baseTex;
	public Texture2D baseTexVisible;
	public Texture2D objectiveTex;
	public Texture2D background;
	public Texture2D currentDrawing;
	public Texture2D sea;
	Texture2D currentrefVisible;
	public Texture2D refVisibleBg;
	public GUIStyle style;
	public GUIStyle styleSmall;
	public GUIStyle styleCompleted;
	public GUIStyle buttonStyle;
	public Texture2D scoreBG;
	public Texture2D scoreKiwi;
	public Texture2D scoreKiwiDisabled;
	public GUIStyle scoreStyle1;
	public GUIStyle scoreStyle2;
	public GUIStyle scoreStyle3;
	int accuracy = 0;
	float sin = 0;
	bool animateOcean = false;
	float scoreScreen = 0;
	bool clean = false;
	Vector2 dragStart;
	Vector2 dragEnd;
	public int numLevelsPerBlock;
	public int level;
	public int subLevel;
	int todayLevels;
	int todayOffset;
	int baseIdx;
	int bankIdx;
	bool nextBase = false;
	GameSaveLoad loader;
	JungleDrawingConfiguration configuration;
	string currentInstruction;
	string currentRef;
	ProgressHandler saveHandler;
	float mapScale=1;
	int kiwis=-1;
	int animationKiwis=0;
	public float kiwiAnimTime=2;
	float kiwiAnimCurrTime;
	float totalAccuracy=0;
	public Camera effectCamera;
	public ScoreEffects scoreEffects;
	SoundManager soundMng;
	bool mobile;
	float heightPos;
	float width;
	float height;
	float insTime = 2f;
	float insTimer;
	bool click;
	SessionManager sessionMng;
	float textureRefBgSizeTemp=0;
	float textureRefSizeTemp=0;
	float xAnim=0;
	float yAnim=0;

	public GUIStyle kiwiButton;
	public Texture loadingScreen;
	public Texture en_loadingScreen;
	public Texture loadingColor;
	bool fadeIn=false;
	bool fadeOut=false;
	bool check=false;
	float opacity=0;

    public float pickTime = 20;
    [HideInInspector]
    public float pickTimer;
    Scores scoreScript;

	LanguageLoader language;

	public enum Tool
	{
		Brush,
		Eraser
	}
	
	enum AccuracyLevel
	{
		None,
		Again,
		Ok,
		Perfect
	}
	int tool2 = 1;
	public Samples AntiAlias = Samples.Samples4;
	public Tool tool = Tool.Brush;
	public Texture[] toolimgs;
	public float lineWidth = 1;
	public float strokeWidth = 1;
	public Color col = Color.white;
	public Color colVisible = Color.white;
	public Color col2 = Color.white;
	public GUISkin gskin;
	public LineTool lineTool;
	public BrushTool brush;
	public EraserTool eraser;
	public Stroke stroke;
	public float zoom = 1f;
	public BezierPoint[] BezierPoints;
	public float xAdjust = 0;
	public Image[] ImageBank;
	string state;
	string currentLevel;
	AccuracyLevel accuracyL = AccuracyLevel.None;
	float endTime=8;
	bool returnToIsland=false;
	Vector2 mouse;
	bool drawStart = false;
	
	int passedLevels=0;
	int repeatedLevels=0;
	int playedLevels=0;
	bool isRepeated;
	int repeatedInARow=0;
	bool passed;
	float levelTime=0;
	float totalTime=0;
	
	//Pause
	public GUIStyle pauseButton;
	public Texture2D pauseBackground;
	public GUIStyle pauseContinue;
	public GUIStyle pauseIsland;
	public GUIStyle pauseExit;
	public GUIStyle pauseText;
	public GUIStyle pauseButtons;
	public string stateBeforePause="";
	bool tutorial;
	
	void clearTexture()
	{
		clean = true;
		Color[] pixelsBase = baseTex.GetPixels(0, 0, baseTex.width, baseTex.height, 0);
		Color[] pixelsBaseVisible = baseTexVisible.GetPixels(0, 0, baseTexVisible.width, baseTexVisible.height, 0);
		for (int x = 0; x < pixelsBase.Length; x++)
		{
			pixelsBase[x] = new Color(1, 1, 1, 0);
			pixelsBaseVisible[x] = new Color(1, 1, 1, 0);
		}
		baseTex.SetPixels(0, 0, baseTex.width, baseTex.height, pixelsBase);
		baseTex.Apply();
		baseTexVisible.SetPixels(0, 0, baseTexVisible.width, baseTexVisible.height, pixelsBaseVisible);
		baseTexVisible.Apply();
	}
	
	public static float CompareColors(Color a, Color b)
	{
		float delta = Mathf.Sqrt(Mathf.Pow(a.r - b.r, 2) + Mathf.Pow(a.g - b.g, 2) + Mathf.Pow(a.b - b.b, 2));
		return (1 - (delta / Mathf.Sqrt(Mathf.Pow(1, 2) + Mathf.Pow(1, 2) + Mathf.Pow(1, 2))));
	}
	
	void OnGUI()
	{
		
		GUI.depth = 1;
		
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background);
		//		GUI.Label(new Rect(Screen.height * .5f, Screen.width * .5f - 600 * mapScale, 300 * mapScale, 200 * mapScale), "Mobile " + mobile, styleSmall);
		//		GUI.Label(new Rect(Screen.width * .5f, Screen.height * .5f - 400 * mapScale, 300 * mapScale, 200 * mapScale), "Click " + click, styleSmall);
		GUI.DrawTexture(new Rect(xAdjust, 0, currentDrawing.width * zoom, currentDrawing.height * zoom), currentDrawing);
		GUI.DrawTexture(new Rect(xAdjust, 0, baseTexVisible.width * zoom, baseTexVisible.height * zoom), baseTexVisible);
		//GUI.Label (new Rect (0, 0, 100, 100), accuracy.ToString (), style);
		if(state!="Pause"&&state!="CompletedActivity")
		{
			
			if(GUI.Button(new Rect(10*mapScale,10*mapScale,71*mapScale,62*mapScale),"",pauseButton))
			{
				
				Time.timeScale=0.0f;
				stateBeforePause=state;
				state="Pause";
			}
			
		}
		switch(state){
		case "Start":
			if(currentRef=="")
			{
				heightPos = 320;
			}
			else{
				heightPos = 520;
				float textureRefBgSize=0.6f;
				float textureRefSize=0.4f;
				GUI.DrawTexture(new Rect(Screen.width*0.5f-(currentrefVisible.width * textureRefBgSize*0.5f*mapScale), Screen.height*0.5f-(currentrefVisible.height * textureRefBgSize*0.5f*mapScale), currentrefVisible.width * textureRefBgSize*mapScale, currentrefVisible.height * textureRefBgSize*mapScale), refVisibleBg);
				GUI.DrawTexture(new Rect(Screen.width*0.5f-(currentrefVisible.width * textureRefSize*0.5f*mapScale), Screen.height*0.5f-(currentrefVisible.height * textureRefSize*0.5f*mapScale), currentrefVisible.width * textureRefSize*mapScale, currentrefVisible.height * textureRefSize*mapScale), currentrefVisible);
			}
			GUI.Label(new Rect(Screen.width * 0.5f - 100, Screen.height * .5f - 370 * mapScale +  heightPos * mapScale, 200, 60), currentInstruction, styleSmall);
			break;
		case "Ins":
			if(heightPos >= 0)
			{
				float font = (float)styleSmall.fontSize;
				if(font > 18)
					font -= 18 * Time.deltaTime;
				styleSmall.fontSize = (int)font;
				heightPos -= (320 * Time.deltaTime);
				textureRefSizeTemp=Mathf.Clamp(textureRefSizeTemp-Time.deltaTime*0.5f,0.25f,0.4f);
				textureRefBgSizeTemp=Mathf.Clamp(textureRefBgSizeTemp-Time.deltaTime*0.5f,0.35f,0.6f);
				if(currentrefVisible!=null)
				{
					if((xAnim+Screen.width*0.5f-(currentrefVisible.width * textureRefBgSizeTemp*0.5f*mapScale))>127*mapScale)
					{
						xAnim-=400*Time.deltaTime*mapScale;
					}
					if((yAnim+ Screen.height*0.5f-(currentrefVisible.height * textureRefBgSizeTemp*0.5f*mapScale))>5.5f*mapScale)
					{
						yAnim-=250*Time.deltaTime*mapScale;
					}
					}
			}
			if(currentRef!="")
			{
				GUI.DrawTexture(new Rect(xAnim+Screen.width*0.5f-(currentrefVisible.width * textureRefBgSizeTemp*0.5f*mapScale),yAnim+ Screen.height*0.5f-(currentrefVisible.height * textureRefBgSizeTemp*0.5f*mapScale), currentrefVisible.width * textureRefBgSizeTemp*mapScale, currentrefVisible.height * textureRefBgSizeTemp*mapScale), refVisibleBg);
				GUI.DrawTexture(new Rect(xAnim+Screen.width*0.5f-(currentrefVisible.width * textureRefSizeTemp*0.5f*mapScale),yAnim+ Screen.height*0.5f-(currentrefVisible.height * textureRefSizeTemp*0.5f*mapScale), currentrefVisible.width * textureRefSizeTemp*mapScale, currentrefVisible.height * textureRefSizeTemp*mapScale), currentrefVisible);
			}
			GUI.Label(new Rect(Screen.width * 0.5f - 100, Screen.height * .5f - 370 * mapScale +  heightPos * mapScale, 200, 60), currentInstruction, styleSmall);
			break;
		case "Play":
			if(currentRef!=""&&!animateOcean)
			{
				float textureRefBgSize=0.35f;
				float textureRefSize=0.25f;
				GUI.DrawTexture(new Rect(150*mapScale+(currentrefVisible.width*textureRefSize*0.5f-currentrefVisible.width*textureRefBgSize*0.5f)*mapScale, 2*mapScale, currentrefVisible.width * textureRefBgSize*mapScale, currentrefVisible.height * textureRefBgSize*mapScale), refVisibleBg);
				GUI.DrawTexture(new Rect(150*mapScale, 2*mapScale+(currentrefVisible.height*textureRefBgSize*0.5f-currentrefVisible.height*textureRefSize*0.5f)*mapScale, currentrefVisible.width * textureRefSize*mapScale, currentrefVisible.height * textureRefSize*mapScale), currentrefVisible);
			}
			GUI.Label(new Rect(Screen.width * 0.5f - 100, Screen.height * .5f - 370 * mapScale +  heightPos * mapScale, 200, 60), currentInstruction, styleSmall);
			buttonStyle.fontSize = 30;
			buttonStyle.fontSize = (int)(buttonStyle.fontSize * mapScale);
			if(!click)
			{
				if (GUI.Button (new Rect (Screen.width / 2 - 75 * mapScale , Screen.height - 70 * mapScale, 150 * mapScale, 60 * mapScale), language.levelStrings[89], buttonStyle))
				{
					if(check)
					{

						currentInstruction = "";
						CompareDrawing ();
					}
				}
			}
			break;
		}
		
		
		if ((animateOcean || nextBase)&&state!="Pause")
		{
			switch (accuracyL)
			{
			case AccuracyLevel.Again:
				styleSmall.fontSize = 50;
				styleSmall.fontSize = (int)(styleSmall.fontSize * mapScale);
				GUI.Label(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f - 100, 200, 200), language.levelStrings[8], style);
				break;
			case AccuracyLevel.Ok:
				styleSmall.fontSize = 50;
				styleSmall.fontSize = (int)(styleSmall.fontSize * mapScale);
				GUI.Label(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f - 100, 200, 200), language.levelStrings[9], style);
				break;
			case AccuracyLevel.Perfect:
				styleSmall.fontSize = 50;
				styleSmall.fontSize = (int)(styleSmall.fontSize * mapScale);
				GUI.Label(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f - 100, 200, 200), language.levelStrings[10], style);
				break;
			}
		}
		GUI.DrawTexture(new Rect(0, -1200*mapScale + Mathf.Sin(sin) * 1200*mapScale, sea.width * (2 - 1 * Mathf.Sin(sin))*mapScale, sea.height*mapScale), sea);
		
		if (state == "CompletedActivity") {
			
			GUI.DrawTexture (new Rect (0, Screen.height * 0.5f - 237 * mapScale, Screen.width, 475 * mapScale), scoreBG);
			
			switch(animationKiwis){
			case 0:	
				GUI.DrawTexture (new Rect (Screen.width * 0.5f - 150 * mapScale, Screen.height * 0.5f - 10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwiDisabled);
				GUI.DrawTexture (new Rect (Screen.width * 0.5f - 45 * mapScale, Screen.height * 0.5f - 10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwiDisabled);
				GUI.DrawTexture (new Rect (Screen.width * 0.5f +60 * mapScale, Screen.height * 0.5f -10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwiDisabled);		
				break;
			case 1:	
				GUI.DrawTexture (new Rect (Screen.width * 0.5f - 150 * mapScale, Screen.height * 0.5f - 10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwi);
				GUI.DrawTexture (new Rect (Screen.width * 0.5f - 45 * mapScale, Screen.height * 0.5f - 10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwiDisabled);
				GUI.DrawTexture (new Rect (Screen.width * 0.5f +60 * mapScale, Screen.height * 0.5f -10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwiDisabled);
				break;
			case 2:	
				GUI.DrawTexture (new Rect (Screen.width * 0.5f - 150 * mapScale, Screen.height * 0.5f - 10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwi);
				GUI.DrawTexture (new Rect (Screen.width * 0.5f - 45 * mapScale, Screen.height * 0.5f - 10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwi);
				GUI.DrawTexture (new Rect (Screen.width * 0.5f +60 * mapScale, Screen.height * 0.5f -10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwiDisabled);
				break;
			case 3:	
				GUI.DrawTexture (new Rect (Screen.width * 0.5f - 150 * mapScale, Screen.height * 0.5f - 10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwi);
				GUI.DrawTexture (new Rect (Screen.width * 0.5f - 45 * mapScale, Screen.height * 0.5f - 10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwi);
				GUI.DrawTexture (new Rect (Screen.width * 0.5f +60 * mapScale, Screen.height * 0.5f -10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwi);
				break;
			}
			if(kiwis>0)
				GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 150 * mapScale, 800 * mapScale, 100 * mapScale), language.levelStrings[0],scoreStyle1);
			else
				GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 150 * mapScale, 800 * mapScale, 100 * mapScale), language.levelStrings[1],scoreStyle1);
			if(kiwis==0)
				GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 90 * mapScale, 800 * mapScale, 100 * mapScale), language.levelStrings[2],scoreStyle2);
			else{
				if(kiwis>1)
					GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 90 * mapScale, 800 * mapScale, 100 * mapScale), language.levelStrings[3]+" "+kiwis.ToString()+" "+language.levelStrings[4],scoreStyle2);
				else
					GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 90 * mapScale, 800 * mapScale, 100 * mapScale), language.levelStrings[3]+" "+kiwis.ToString()+" "+language.levelStrings[5],scoreStyle2);
			}
			//GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f + 130 * mapScale, 800 * mapScale, 100 * mapScale), "Volvamos a la Isla",scoreStyle3);

			if(GUI.Button(new Rect(Screen.width*0.5f-80*mapScale,Screen.height*0.5f+110*mapScale,160*mapScale,60*mapScale),language.levelStrings[7],kiwiButton))
			{
				returnToIsland=true;
			}
			//styleCompleted.normal.textColor=new Color(0.32f,0.32f,0.32f);
			//GUI.Label(new Rect(Screen.width*0.5f-398*mapScale,Screen.height*0.5f-98*mapScale,800*mapScale,200*mapScale),"Felicidades! dibujas muy bien, volvamos a la isla",styleCompleted);
			//styleCompleted.normal.textColor=new Color(1,1,1);
			//GUI.Label(new Rect(Screen.width*0.5f-400*mapScale,Screen.height*0.5f-100*mapScale,800*mapScale,200*mapScale),"Felicidades! dibujas muy bien, volvamos a la isla",styleCompleted);
            if (scoreScript.finalScore)
            {
                scoreScript.scoreStyle.fontSize = 50;
                scoreScript.scoreStyle.fontSize = (int)(scoreScript.scoreStyle.fontSize * mapScale);
                GUI.Label(new Rect(Screen.width * 0.5f + 270 * mapScale, Screen.height * 0.5f - 90 * mapScale, 100 * mapScale, 50 * mapScale), scoreScript.scoreString, scoreScript.scoreStyle);

                scoreScript.GuiExtraKiwisDisplay();
                if (scoreScript.scoreCounter >= scoreScript.kiwiMilestone)
                {
                    GUI.DrawTexture(new Rect(Screen.width * 0.5f + 245 * mapScale, Screen.height * 0.5f - 20 * mapScale, 150 * mapScale, 150 * mapScale), scoreKiwi);
                    GUI.Label(new Rect(Screen.width * 0.5f + 380 * mapScale, Screen.height * 0.5f + 50 * mapScale, 100 * mapScale, 50 * mapScale), "x" + scoreScript.extraKiwis, scoreScript.scoreStyle);
                }
            }
		}
		
		
		
		
		if (state == "Pause") 
		{
			float pauseY=Screen.height*0.5f-177*mapScale;
			GUI.DrawTexture(new Rect(0,pauseY,Screen.width,354*mapScale),pauseBackground);
			GUI.Label(new Rect(Screen.width*0.5f-100*mapScale,pauseY-40*mapScale,200*mapScale,60*mapScale),language.levelStrings[6],pauseText);
			if(GUI.Button(new Rect(Screen.width*0.5f-200*mapScale,pauseY+50*mapScale,366*mapScale,66*mapScale),"",pauseContinue))
			{
				Time.timeScale=1.0f;
				state=stateBeforePause;
			}
			else
				if(GUI.Button(new Rect(Screen.width*0.5f-200*mapScale,pauseY+140*mapScale,382*mapScale,66*mapScale),"",pauseIsland))
			{
				if(!returnToIsland){
					SaveProgress(false);
					returnToIsland=true;
					Time.timeScale=1.0f;
				}
				//Application.LoadLevel("Archipielago");
			}
			GUI.Label(new Rect(Screen.width*0.5f-110*mapScale,pauseY+50*mapScale,366*mapScale,66*mapScale),language.levelStrings[90],pauseButtons);
			GUI.Label(new Rect(Screen.width*0.5f-110*mapScale,pauseY+140*mapScale,382*mapScale,66*mapScale),language.levelStrings[91],pauseButtons);
			/*else
				if(GUI.Button(new Rect(Screen.width*0.5f-200*mapScale,pauseY+230*mapScale,162*mapScale,67*mapScale),"",pauseExit))
			{
				Time.timeScale=1.0f;
				Application.Quit();
			}*/
		}
		//GUI.color = new Color(1,1,1,0.50f);
		//GUI.DrawTexture (Rect (0,0,objectiveTex.width*zoom,objectiveTex.height*zoom),objectiveTex);
		//GUI.color = new Color(1,1,1,1);
		if(fadeIn){
			opacity+=1*Time.deltaTime;
			if(opacity>=1){
				opacity=1;
				Application.LoadLevel("Archipielago");
				//fadeIn=false;
			}
			GUI.color=new Color(1,1,1,opacity);
		}else if (fadeOut){
			opacity-=1*Time.deltaTime;
			if(opacity<=0){
				opacity=0;
				fadeOut=false;
			}
			GUI.color=new Color(1,1,1,opacity);
		}
		if(fadeIn)
		{
			GUI.DrawTexture(new Rect (0, 0, Screen.width, Screen.height), loadingColor);
			GUI.DrawTexture(new Rect(Screen.width/2-Screen.height/2,0,Screen.height,Screen.height),loadingScreen);
		}
		GUI.color=new Color(1,1,1,1);
		effectCamera.Render ();
	}
	
	Vector2 preDrag;
	
	void Start()
	{
		sessionMng = GetComponent<SessionManager> ();
		string lang = sessionMng.activeUser.language;
		if(lang=="")
			lang="es";
		language = GetComponent<LanguageLoader>();
		language.LoadGameLanguage(lang);
		switch(lang)
		{
			case "es":

			break;
			case "en":
				loadingScreen=en_loadingScreen;
			break;
		}
		int tempLangIdx = 11;
		for(int i=0;i<ImageBank.Length;i++)
		{
			for(int x=0;x<ImageBank[i].baseImages.Length;x++)
			{
				ImageBank[i].baseImages[x].instruction=language.levelStrings[tempLangIdx++];
			}
		}
		kiwiAnimCurrTime = kiwiAnimTime;
        pickTimer = pickTime;
		soundMng = GetComponent<SoundManager> ();
        scoreScript = GetComponent<Scores>();
		mapScale = (float)Screen.height / (float)768;
		style.fontSize = (int)(style.fontSize * mapScale);
		styleSmall.fontSize = 50;
		styleSmall.fontSize = (int)(styleSmall.fontSize * mapScale);
		styleCompleted.fontSize = (int)(styleCompleted.fontSize * mapScale);
		pauseText.fontSize = (int)(pauseText.fontSize * mapScale);
		scoreStyle1.fontSize = (int)(scoreStyle1.fontSize * mapScale);
		scoreStyle2.fontSize = (int)(scoreStyle2.fontSize * mapScale);
		scoreStyle3.fontSize = (int)(scoreStyle3.fontSize * mapScale);
		kiwiButton.fontSize = (int)(kiwiButton.fontSize * mapScale);
		pauseButtons.fontSize = (int)(pauseButtons.fontSize * mapScale);
		saveHandler=(ProgressHandler)GetComponent(typeof(ProgressHandler));
		clearTexture();
		loader = GetComponent<GameSaveLoad>();
		loader.Load(GameSaveLoad.game.jungleDrawing);
		configuration = (JungleDrawingConfiguration)loader.configuration;
		if(configuration.sound==0)
		{
			GameObject tempCam = GameObject.Find("Camera");
			AudioSource[] tempAudios = tempCam.GetComponents<AudioSource>();
			tempAudios[1].volume=0;
			tempAudios[1].Stop();
			GetComponent<AudioSource>().volume=0;
			GetComponent<AudioSource>().Stop();
		}else
		{
			GameObject tempCam = GameObject.Find("Camera");
			AudioSource[] tempAudios = tempCam.GetComponents<AudioSource>();
			tempAudios[1].Play();
		}
		if(configuration.music==0)
		{
			GameObject tempCam = GameObject.Find("Camera");
			AudioSource[] tempAudios = tempCam.GetComponents<AudioSource>();
			tempAudios[0].volume=0;
			tempAudios[0].Stop();
		}else
		{
			GameObject tempCam = GameObject.Find("Camera");
			AudioSource[] tempAudios = tempCam.GetComponents<AudioSource>();
			tempAudios[0].Play();
		}
		level = sessionMng.activeKid.sandDifficulty;
		subLevel = sessionMng.activeKid.sandLevel;
		//level = 0;
		//subLevel = 18;
		todayLevels = 0;
		todayOffset = sessionMng.activeKid.playedSand;
		audSrc = GetComponent<AudioSource>();
		check = true;
		state = "Start";
		//SaveProgress();
		if(SystemInfo.deviceType == DeviceType.Handheld)
		{
			mobile = true;
		}
		else 
		{
			mobile = false;
		}	
		
		heightPos = 320;
		tutorial = true;
		soundMng.PlaySound(0);
		insTimer = insTime;
		SetLevel();
		drawStart = false;
	}
	
	void Update()
	{
		if (returnToIsland && !saveHandler.saving) 
		{
			fadeIn=true;
			//Application.LoadLevel("Archipielago");
		}
		if(mobile)
		{

			if(Input.touches.Length <= 0)
			{
				touch2Watch = 64;
				if (dragStart != Vector2.zero && !animateOcean)
				{
					dragStart = Vector2.zero;
					dragEnd = Vector2.zero;
				}
			}
		}
		
		
		zoom = Screen.height / 768.0f;
		xAdjust = Screen.width * 0.5f - 683 * zoom;

		totalTime += Time.deltaTime;
		levelTime += Time.deltaTime;
		switch (state)
		{
		case "Start":
			
			if(tutorial)
			{
				if(!audSrc.isPlaying)
				{
					if(configuration.sound==1)
						audSrc.volume = 1;
					audSrc.loop=false;
					soundMng.PlaySound(1);
					textureRefBgSizeTemp=0.6f;
					textureRefSizeTemp=0.4f;
					xAnim=0;
					yAnim=0;
					state = "Ins";	
				}
			}
			else
			{
				insTimer -= Time.deltaTime;
				if(insTimer <= 0)
				{
					if(configuration.sound==1)
						audSrc.volume = 1;
					audSrc.loop=false;
					soundMng.PlaySound(1);
					insTimer = insTime;
					textureRefBgSizeTemp=0.6f;
					textureRefSizeTemp=0.4f;
					xAnim=0;
					yAnim=0;
					state = "Ins";
				}
			}
			
			
			break;
		case "Ins":
			if(!audSrc.isPlaying)
			{
                pickTimer = pickTime;
				state = "Play";
			}	
			
			break;
		case "InsAnim":
			
			break;
		case "Play":
            pickTimer -= Time.deltaTime;
			if (scoreScreen > 0)
			{
				scoreScreen -= Time.deltaTime;
			}
			else if (nextBase)
			{
				SetNextBase();
			}
			Rect imgRect = new Rect(0, 0, baseTex.width * zoom, baseTex.height * zoom);
			try{
			if(mobile)
			{

			
				if(Input.touches.Length > 0)
				{
					
					Rect r = new Rect (Screen.width / 2 - 75 * mapScale , Screen.height - 70 * mapScale, 150 * mapScale, 60 * mapScale);
					Vector2 mousp = Input.GetTouch(touch2Watch).position;
					
					float rY = r.y + Screen.height - r.y ;
					float rY2 = Screen.height - (r.y + r.height);
					float mY = Screen.height - mousp.y;
					if(mousp.x >= r.x && mousp.x <= r.x + r.width && mY >= r.y  && mY <= r.y + r.height)
					{
						
					}
					else
					{
						click = true;
					}
					
				}
				else 
				{
					click = false;
				}
				
				mouse=new Vector2(0,0);	
			
				if(TouchLogicBU.currTouch == touch2Watch)
				{
					
					mouse = Input.GetTouch(touch2Watch).position;
					
					mouse.y = Screen.height - mouse.y;
					mouse.x -= xAdjust;
				}
				if (animateOcean && scoreScreen <= 0)
				{
					sin += 0.5f * Time.deltaTime;
					if (sin > Mathf.PI / 2 && !clean)
					{
						styleSmall.fontSize = 50;
						styleSmall.fontSize = (int)(styleSmall.fontSize * mapScale);
						SetLevel();
						heightPos = 320;
						clearTexture();
						accuracyL = AccuracyLevel.None;
						accuracy = 0;
						
					}
					if (sin >= Mathf.PI)
					{
						accuracyL = AccuracyLevel.None;
						animateOcean = false;
						sin = 0;
						
						if(playedLevels+todayOffset>= numLevelsPerBlock)
						{
							CalculateKiwis();
							state="CompletedActivity";
						}
						else
						{
							tutorial = false;
							check=true;
							state = "Start";
						}
					}
					else
					{
						sin = sin % Mathf.PI;
					}
				}
				
				if (touch2Watch == TouchLogicBU.currTouch && !animateOcean)
				{
					if (dragStart == Vector2.zero)
					{
						return;
					}
					dragEnd = mouse - new Vector2(imgRect.x, imgRect.y);
					dragEnd.x = Mathf.Clamp(dragEnd.x, 0, imgRect.width);
					dragEnd.y = imgRect.height - Mathf.Clamp(dragEnd.y, 0, imgRect.height);
					dragEnd.x = Mathf.Round(dragEnd.x / zoom);
					dragEnd.y = Mathf.Round(dragEnd.y / zoom);
					
					if (tool == Tool.Brush)
					{
						Brush(dragEnd, preDrag);
					}
					if (tool == Tool.Eraser)
					{
						Eraser(dragEnd, preDrag);
					}
				}
				
			}
			else
			{
				Rect r = new Rect (Screen.width / 2 - 75 * mapScale , Screen.height - 70 * mapScale, 150 * mapScale, 60 * mapScale);
				Vector2 mousp = Input.mousePosition;
				float rY = r.y + Screen.height - r.y ;
				float rY2 = Screen.height - (r.y + r.height);
				float mY = Screen.height - mousp.y;
				if(mousp.x >= r.x && mousp.x <= r.x + r.width && mY >= r.y  && mY <= r.y + r.height)
				{
					
				}
				else
				{
					if(Input.GetMouseButtonDown(0))
					{
						click = true;
					}
					else if(Input.GetMouseButtonUp(0))
					{

						click = false;
					}
					
				}
				mouse = Input.mousePosition;
				mouse.y = Screen.height - mouse.y;
				mouse.x -= xAdjust;
				if (animateOcean && scoreScreen <= 0)
				{
					sin += 0.5f * Time.deltaTime;
					if (sin > Mathf.PI / 2 && !clean)
					{
						styleSmall.fontSize = 50;
						styleSmall.fontSize = (int)(styleSmall.fontSize * mapScale);
						SetLevel();
						heightPos = 320;
						clearTexture();
						accuracyL = AccuracyLevel.None;
						accuracy = 0;
						
					}
					if (sin >= Mathf.PI)
					{
						accuracyL = AccuracyLevel.None;
						animateOcean = false;
						sin = 0;
						
						if(playedLevels+todayOffset >= numLevelsPerBlock)
						{
							CalculateKiwis();
							state="CompletedActivity";
						}
						else
						{
							tutorial = false;
							check=true;
							state = "Start";
						}
					}
					else
					{
						sin = sin % Mathf.PI;
					}
				}
				
				if (Input.GetKeyDown("mouse 0") && !animateOcean)
				{
					
					clean = false;
					if (imgRect.Contains(mouse))
					{
						if(!soundMng.IsPlaying())
						{

							//audSrc.loop = true;

							Rect re = new Rect (Screen.width / 2 - 75 * mapScale , Screen.height - 70 * mapScale, 150 * mapScale, 60 * mapScale);
							Vector2 mousP = Input.mousePosition;
							float ry = re.y + Screen.height - re.y ;
							float ry2 = Screen.height - (re.y + re.height);
							float my = Screen.height - mousP.y;
							if(mousP.x >= re.x && mousP.x <= re.x + re.width && my >= re.y  && my <= re.y + re.height)
							{

							}
							else{
								audSrc.loop = true;
								if(configuration.sound==1)
									audSrc.volume = .05f;
								soundMng.PlaySound(8);
							}

						}
						dragStart = mouse - new Vector2(imgRect.x, imgRect.y);
						dragStart.y = imgRect.height - dragStart.y;
						dragStart.x = Mathf.Round(dragStart.x / zoom);
						dragStart.y = Mathf.Round(dragStart.y / zoom);
						//LineStart (mouse - Vector2 (imgRect.x,imgRect.y));
						
						dragEnd = mouse - new Vector2(imgRect.x, imgRect.y);
						dragEnd.x = Mathf.Clamp(dragEnd.x, 0, imgRect.width);
						dragEnd.y = imgRect.height - Mathf.Clamp(dragEnd.y, 0, imgRect.height);
						dragEnd.x = Mathf.Round(dragEnd.x / zoom);
						dragEnd.y = Mathf.Round(dragEnd.y / zoom);
					}
					else
					{
						dragStart = Vector3.zero;
					}
					
				}
				if (Input.GetKey("mouse 0") && !animateOcean)
				{
					if (dragStart == Vector2.zero)
					{
						return;
					}

					dragEnd = mouse - new Vector2(imgRect.x, imgRect.y);
					dragEnd.x = Mathf.Clamp(dragEnd.x, 0, imgRect.width);
					dragEnd.y = imgRect.height - Mathf.Clamp(dragEnd.y, 0, imgRect.height);
					dragEnd.x = Mathf.Round(dragEnd.x / zoom);
					dragEnd.y = Mathf.Round(dragEnd.y / zoom);
					
					if (tool == Tool.Brush)
					{
						Brush(dragEnd, preDrag);
					}
					if (tool == Tool.Eraser)
					{
						Eraser(dragEnd, preDrag);
					}
				}
				if (Input.GetKeyUp("mouse 0") && dragStart != Vector2.zero && !animateOcean)
				{
					audSrc.Stop();
					drawStart = true;
					dragStart = Vector2.zero;
					dragEnd = Vector2.zero;
				}
			}
			}
			catch(System.Exception ex)
			{
		
			}
			preDrag = dragEnd;
			break;
		case "CompletedActivity":
            if (!scoreScript.finalScore)
            {
                kiwiAnimCurrTime -= Time.deltaTime;
                if (kiwiAnimCurrTime <= 0)
                {
                    kiwiAnimCurrTime = kiwiAnimTime;
                    if (animationKiwis < kiwis)
                    {
                        animationKiwis++;
                    }
                    else
                    {
                        scoreScript.ScoreAddValue();
                    }
                }
            }else
            {
                scoreScript.ScoreCounter();
            }
			break;
		}
	}

	void CalculateKiwis()
	{
		if(kiwis==-1)
		{
			kiwis=0;
			totalAccuracy/=playedLevels;
			Debug.Log("Total Accuracy"+totalAccuracy.ToString());
			soundMng.pauseQueue=true;
			if(totalAccuracy>70)
			{
				soundMng.AddSoundToQueue(12,false,false);
				kiwis=3;
			}else if(totalAccuracy>40)
			{
				soundMng.AddSoundToQueue(11,false,false);
				kiwis=2;
			}else if(totalAccuracy>20)
			{
				soundMng.AddSoundToQueue(10,false,false);
				kiwis=1;
			}
			int tempKiwis=sessionMng.activeKid.kiwis;
            sessionMng.activeKid.kiwis = tempKiwis + kiwis + scoreScript.GetExtraKiwis();
			sessionMng.SaveSession();
			if(kiwis==0)
			{
				if(configuration.sound==1)
					audSrc.volume = 1;
				audSrc.loop=false;
				soundMng.AddSoundToQueue(6,false,false);
				soundMng.AddSoundToQueue(7,true,false);
			}else{
				if(configuration.sound==1)
					audSrc.volume = 1;
				audSrc.loop=false;
				soundMng.AddSoundToQueue(5,false,false);
				soundMng.AddSoundToQueue(7,true,false);
			}
			GetComponent<ProfileSync>().UpdateProfile();
		}
	}
	
	void OnTouchBeganAnywhere()
	{
		Debug.Log ("StartTouch");
		if(state=="Play"&&touch2Watch==64)
		{

			touch2Watch = TouchLogicBU.currTouch;
			if (!animateOcean)
			{
				//Debug.Log ("StartTouch !animateOcean");
				clean = false;
				zoom = Screen.height / 768.0f;
				Rect imgRect = new Rect(0, 0, baseTex.width * zoom, baseTex.height * zoom);
				if (imgRect.Contains(mouse))
				{

					if(!soundMng.IsPlaying())
					{
						Rect r = new Rect (Screen.width / 2 - 75 * mapScale , Screen.height - 70 * mapScale, 150 * mapScale, 60 * mapScale);
						Vector2 mousp = Input.GetTouch(touch2Watch).position;;
						float rY = r.y + Screen.height - r.y ;
						float rY2 = Screen.height - (r.y + r.height);
						float mY = Screen.height - mousp.y;
						if(mousp.x >= r.x && mousp.x <= r.x + r.width && mY >= r.y  && mY <= r.y + r.height)
						{

						}
						else
						{
							audSrc.loop = true;
							if(configuration.sound==1)
								audSrc.volume = .05f;
							soundMng.PlaySound(8);
						}


					}
					//Debug.Log ("StartTouch Contains");
					//Debug.Log ("StartTouch");
					dragStart = mouse - new Vector2(imgRect.x, imgRect.y);
					dragStart.y = imgRect.height - dragStart.y;
					dragStart.x = Mathf.Round(dragStart.x / zoom);
					dragStart.y = Mathf.Round(dragStart.y / zoom);
					//LineStart (mouse - Vector2 (imgRect.x,imgRect.y));
					
					dragEnd = mouse - new Vector2(imgRect.x, imgRect.y);
					dragEnd.x = Mathf.Clamp(dragEnd.x, 0, imgRect.width);
					dragEnd.y = imgRect.height - Mathf.Clamp(dragEnd.y, 0, imgRect.height);
					dragEnd.x = Mathf.Round(dragEnd.x / zoom);
					dragEnd.y = Mathf.Round(dragEnd.y / zoom);
				}
				else
				{
					dragStart = Vector3.zero;
				}		
			}
		}
	}
	
	void OnTouchMovedAnywhere()
	{
		if(TouchLogicBU.currTouch == touch2Watch)
		{
			
		}
	}
	
	void OnTouchEndedAnywhere()
	{
		if(TouchLogicBU.currTouch == touch2Watch || Input.touches.Length <= 0)
		{
			touch2Watch = 64;
			if (dragStart != Vector2.zero && !animateOcean)
			{
				if(soundMng.IsPlaying())
				{
					audSrc.loop = false;
					audSrc.Stop();
				}
				dragStart = Vector2.zero;
				dragEnd = Vector2.zero;
			}
		}
	}
	
	void OnTouchStayedAnywhere()
	{
		
	}
	
	void SetLevel()
	{
		drawStart = false;
		baseIdx = 0;
		int imgIdx = Random.Range(0, configuration.levels[level].subLevels[subLevel].availableDrawings.Length);
		for (int i = 0; i < ImageBank.Length; i++)
		{
			if (ImageBank[i].key == configuration.levels[level].subLevels[subLevel].availableDrawings[imgIdx])
			{
				bankIdx = i;
				currentLevel=configuration.levels[level].subLevels[subLevel].availableDrawings[imgIdx];
				currentDrawing =Resources.Load("Arena/"+ImageBank[i].visibleImage) as Texture2D;
				objectiveTex = Resources.Load("Arena/"+ImageBank[i].baseImages[baseIdx].texture)as Texture2D;
				currentInstruction = ImageBank[i].baseImages[baseIdx].instruction;
				currentRef=ImageBank[i].baseImages[baseIdx].visibleRef;
				if(currentRef!="")
				{
					currentrefVisible=Resources.Load("Arena/"+ImageBank[i].baseImages[baseIdx].visibleRef)as Texture2D;
				}
			}
		}
	}
	
	void SetNextBase()
	{
		nextBase = false;
		baseIdx++;
		currentDrawing =Resources.Load("Arena/"+ImageBank[bankIdx].visibleImage) as Texture2D;
		objectiveTex = Resources.Load("Arena/"+ImageBank[bankIdx].baseImages[baseIdx].texture)as Texture2D;
		currentInstruction = ImageBank[bankIdx].baseImages[baseIdx].instruction;
		currentRef=ImageBank[bankIdx].baseImages[baseIdx].visibleRef;
		if(currentRef!="")
		{
			currentrefVisible=Resources.Load("Arena/"+currentRef)as Texture2D;
		}
		insTimer = insTime;
		tutorial = false;
		check = true;
		state="Start";
	}
	
	void CompareDrawing()
	{
		check=false;
		if (!clean && !animateOcean)
		{
			if (baseIdx < ImageBank[bankIdx].baseImages.Length - 1)
			{
				drawStart = false;
				nextBase = true;
			}
			else
			{
				animateOcean = true;
				if(configuration.sound==1)
					soundMng.PlaySound(9, 0, 0.3f);
			}
			
			dragStart = Vector2.zero;
			dragEnd = Vector2.zero;
			scoreScreen = 3;
			Color[] pixelsBase = baseTex.GetPixels(0, 0, baseTex.width, baseTex.height, 0);
			Color[] pixelsObjective = objectiveTex.GetPixels(0, 0, objectiveTex.width, objectiveTex.height, 0);
			float average = 0;
			int totalPixels = 0;
			for (int i = 0; i < pixelsBase.Length; i++)
			{
				if (pixelsObjective[i] == new Color(0, 0, 0) || pixelsBase[i] == new Color(0, 0, 0))
				{
					average += CompareColors(pixelsBase[i], pixelsObjective[i]);
					totalPixels++;
					if (average > 0)
					{
						bool test = true;
					}
				}
			}
			if (average > 0)
				average /= totalPixels;
			average = Mathf.Ceil(average * 100);
			accuracy = (int)average;
			accuracyL = AccuracyLevel.Again;
			passed=false;
			if(configuration.sound==1)
				audSrc.volume = 1;
			audSrc.loop=false;
			if (accuracy >= 40)
			{
                scoreScript.prevCorMult++;
				repeatedInARow=0;
				if(accuracy>=70)
				{
					soundMng.PlaySound(4);
				}else{
					soundMng.PlaySound(3);
				}
                scoreEffects.DisplayScore(scoreScript.TempScoreSum(), configuration.sound == 1);
				passed=true;
				accuracyL = AccuracyLevel.Ok;
				if (!nextBase)
				{
					if (subLevel < configuration.levels [level].subLevels.Length - 1) {
						subLevel++;	
					} else {
						if (level < configuration.levels.Length - 1) {
							level++;
							subLevel = 0;
						}
					}
					sessionMng.activeKid.sandDifficulty=level;
					sessionMng.activeKid.sandLevel=subLevel;
					sessionMng.SaveSession();
				}
			}
			else
			{
                scoreScript.prevCorMult=0;
				repeatedInARow++;
				if(repeatedInARow>1)
				{
					repeatedInARow=0;
					if (subLevel > 0) {
						subLevel--;	
					} else {
						if (level > 1) {
							level--;
							subLevel= configuration.levels[level].subLevels.Length-1;
						}
					}
					sessionMng.activeKid.sandDifficulty=level;
					sessionMng.activeKid.sandLevel=subLevel;
					sessionMng.SaveSession();
				}
				if(configuration.sound==1)
					audSrc.volume = 1;
				audSrc.loop=false;
				soundMng.PlaySound(2);
				scoreEffects.DisplayError(configuration.sound==1);
				baseIdx = 0;
				nextBase = false;
				animateOcean = true;
				if(configuration.sound==1)
					soundMng.PlaySound(9, 0, .3f);

			}
			if (accuracy >= 70)
				accuracyL = AccuracyLevel.Perfect;
			if(accuracyL==AccuracyLevel.Again){
				isRepeated=true;
			}else{
				isRepeated=false;
			}
			SaveLevelProgress();
			Debug.Log(average);
			totalAccuracy+=average;
		}
	}
	
	public void Brush(Vector2 p1, Vector2 p2)
	{
		Drawing.NumSamples = AntiAlias;
		if (p2 == Vector2.zero)
		{
			p2 = p1;
		}
		Drawing.PaintLine(p1, p2, brush.width, col, brush.hardness, baseTex);
		Drawing.PaintLine(p1, p2, brush.width, colVisible, brush.hardness, baseTexVisible);
		baseTex.Apply();
		baseTexVisible.Apply();
	}
	
	public void Eraser(Vector2 p1, Vector2 p2)
	{
		Drawing.NumSamples = AntiAlias;
		if (p2 == Vector2.zero)
		{
			p2 = p1;
		}
		Drawing.PaintLine(p1, p2, eraser.width, Color.white, eraser.hardness, baseTex);
		baseTex.Apply();
	}
	
	void test()
	{
		float startTime = Time.realtimeSinceStartup;
		int w = 100;
		int h = 100;
		BezierPoint p1 = new BezierPoint(new Vector2(10, 0), new Vector2(5, 20), new Vector2(20, 0));
		BezierPoint p2 = new BezierPoint(new Vector2(50, 10), new Vector2(40, 20), new Vector2(60, -10));
		BezierCurve c = new BezierCurve(p1.main, p1.control2, p2.control1, p2.main);
		p1.curve2 = c;
		p2.curve1 = c;
		Vector2 elapsedTime = new Vector2((Time.realtimeSinceStartup - startTime) * 10, 0);
		float startTime2 = Time.realtimeSinceStartup;
		for (int i = 0; i < w * h; i++)
		{
			Mathfx.IsNearBezier(new Vector2(Random.value * 80, Random.value * 30), p1, p2, 10);
		}
		
		Vector2 elapsedTime2 = new Vector2((Time.realtimeSinceStartup - startTime2) * 10, 0);
		Debug.Log("Drawing took " + elapsedTime.ToString() + "  " + elapsedTime2.ToString());
	}
	
	void SaveLevelProgress()
	{
		saveHandler.AddLevelData ("levelKey", currentLevel);
		saveHandler.AddLevelData ("level", level);
		saveHandler.AddLevelData ("subLevel", subLevel);
		saveHandler.AddLevelData ("time",(int)levelTime);
		saveHandler.AddLevelData ("passed", passed);
		saveHandler.AddLevelData ("repeated", isRepeated);
		saveHandler.AddLevelData ("accuracy", accuracy);
		playedLevels++;
		todayLevels++;
		sessionMng.activeKid.playedSand = todayLevels + todayOffset;
		sessionMng.SaveSession ();
		if (isRepeated)
			repeatedLevels++;
		if (passed)
			passedLevels++;
		levelTime = 0;
		//saveHandler.SetLevel();
		if (playedLevels+todayOffset >= numLevelsPerBlock) {
			sessionMng.activeKid.blockedArenaMagica=1;
			sessionMng.SaveSession();
			SaveProgress (true);
		}
	}
	void SaveProgress(bool rank)
	{
		if (playedLevels > 0) {
			saveHandler.CreateSaveBlock ("ArenaMagica", (int)totalTime, passedLevels, repeatedLevels, playedLevels);
			saveHandler.AddLevelsToBlock ();
			saveHandler.PostProgress (rank);
			Debug.Log (saveHandler.ToString ());
		}
	}
	
	[System.Serializable]
	public class LineTool
	{
		public float width = 1;
	}
	
	[System.Serializable]
	public class EraserTool
	{
		public float width = 1;
		public float hardness = 1;
	}
	
	[System.Serializable]
	public class BrushTool
	{
		public float width = 1;
		public float hardness = 0;
		public float spacing = 10;
	}
	
	[System.Serializable]
	public class Stroke
	{
		public bool enabled = false;
		public float width = 1;
	}
	
	[System.Serializable]
	public class Image
	{
		public string key;
		public string visibleImage;
		public BaseImage[] baseImages;
	}
	
	[System.Serializable]
	public class BaseImage
	{
		public string instruction;
		public string texture;
		public string visibleRef;
	}
	
}