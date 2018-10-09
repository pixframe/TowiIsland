using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Boomlagoon.JSON;

public class ShadowGameMain : MonoBehaviour
{
	//configuration
	GameSaveLoad loader;
	ShadowGameConfiguration configuration;

	
	
	//Scripts
	Timer timerScript;
	SelectionScript selS;
	ProgressHandler saveHandler;
	SoundManager soundMng;
	AudioSource audSource;
	Scores scoreScript;
	
	//Game Logic Variables
	public string state;
	string stateBeforePause;
	public int level;
	public int subLevel;
	int todayLevels;
	int todayOffset;
	public Vector3[] objPos;
	public ParticleSystem[] gases;
	public List<ParticleSystem> parts = new List<ParticleSystem>();
	Vector3 shadowPos;
	bool timerOn = false;
	Vector3 imageStorage = new Vector3(20f, 3f, -1f);
	bool objImageSet = false;
	GameObject objSel;
	string objName;
	int tries;
	bool levelSet = false;
	Vector3[] resultPos;
	bool objPresented = false;
	bool correct;
	int numObj;
	bool appearImages = false;
	int playedLevels;
	int levelsPassed;
	int levelsRepeated;
	float levelTime;
	float totalTime;
	public int numLevelsPerBlock;
	public float yGasDif = 2f;
	bool gasFinished = false;
	float waitTime = .1f;
	float waitTimer = 0f;
	string advState;
	bool ad = false;
	float endTime=5;
	bool play = false;
	float apTime = .5f;
	float apTimer;
	bool tutorial = false;
	float pickTime;
	public float pickTimer;
	int num;
	bool returnToIsland=false;
	public GUIStyle kiwiButton;
	public Texture loadingScreen;
	public Texture en_loadingScreen;
	public Texture loadingColor;
	bool fadeIn=false;
	bool fadeOut=false;
	float opacity=0;
	
	//Gamepaly variables
	public List<string> s = new List<string>();
	public GameObject[] shadows;
	public GameObject[] objects;
	GameObject[] objInUse;
	public GameObject shadow;
	public string[] options;
	public float shadowTime = 3f;
	Color shadowColor = new Color(0f, 0f, 0f, 0f);
	public float timeOfFade = 2f;
	bool inverse = false;
	
	//GUI variables
	Rect label;
	float screenScale;
	public GUIStyle pauseButton;
	public Texture2D pauseBackground;
	public GUIStyle pauseContinue;
	public GUIStyle pauseIsland;
	public GUIStyle pauseExit;
	public GUIStyle pauseText;
	public GUIStyle styleS;
	public GUIStyle advanceStyle;
	public Texture2D textBG;
	
	
	public Texture2D scoreBG;
	public Texture2D scoreKiwi;
	public Texture2D scoreKiwiDisabled;
	public GUIStyle scoreStyle1;
	public GUIStyle scoreStyle2;
	public GUIStyle scoreStyle3;
	public GUIStyle pauseButtons;
	int kiwis=-1;
	int animationKiwis=0;
	public float kiwiAnimTime=2;
	float kiwiAnimCurrTime;
	
	public ScoreEffects scoreEffects;
	SessionManager sessionMng;

	LanguageLoader language;
	
	// Use this for initialization
	void Start ()
	{
		scoreScript = GetComponent<Scores>();
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
		kiwiAnimCurrTime = kiwiAnimTime;
		screenScale = (float)Screen.height / (float)768;
		scoreStyle1.fontSize = (int)(scoreStyle1.fontSize * screenScale);
		scoreStyle2.fontSize = (int)(scoreStyle2.fontSize * screenScale);
		scoreStyle3.fontSize = (int)(scoreStyle3.fontSize * screenScale);
		kiwiButton.fontSize = (int)(kiwiButton.fontSize * screenScale);
		pauseButtons.fontSize = (int)(pauseButtons.fontSize * screenScale);
		shadows = GameObject.FindGameObjectsWithTag("Shadow");
		objects = GameObject.FindGameObjectsWithTag("SObject");
		timerScript = GameObject.Find("Main").GetComponent<Timer>();
		selS = GameObject.Find("Main").GetComponent<SelectionScript>();
		saveHandler = GetComponent<ProgressHandler>();
		loader = GameObject.Find("Main").GetComponent<GameSaveLoad>();
		loader.Load(GameSaveLoad.game.shadowGame);
		configuration = (ShadowGameConfiguration)loader.configuration;
		if(configuration.music==0)
		{
			GameObject mainCamera=GameObject.Find("Main Camera");
			mainCamera.GetComponents<AudioSource>()[0].Stop();
		}else
		{
			GameObject mainCamera=GameObject.Find("Main Camera");
			mainCamera.GetComponents<AudioSource>()[0].Play();
		}
		if(configuration.sound==0)
		{
			GameObject mainCamera=GameObject.Find("Main Camera");
			mainCamera.GetComponents<AudioSource>()[1].volume=0;
			mainCamera.GetComponents<AudioSource>()[1].Stop();
			GetComponent<AudioSource>().volume=0;
			GetComponent<AudioSource>().Stop();
		}else
		{
			GameObject mainCamera=GameObject.Find("Main Camera");
			mainCamera.GetComponents<AudioSource>()[1].Play();
		}
		shadowPos = GameObject.Find("ShadowPos").transform.position;
		soundMng = GetComponent<SoundManager>();
		audSource = GetComponent<AudioSource>();
		Color color;
		foreach(GameObject o in shadows)
		{
			color = o.GetComponent<SpriteRenderer>().color;
			color.a = 0f;
			o.GetComponent<SpriteRenderer>().color = color;
		}
		resultPos = new Vector3[2];
		resultPos[0] = new Vector3(-1.25f, shadowPos.y, -1f);
		resultPos[1] = new Vector3(1.25f, shadowPos.y, -1f);
		sessionMng.activeKid.laveLevel = 0;
		level = sessionMng.activeKid.lavaDifficulty;
		subLevel = sessionMng.activeKid.laveLevel;
		
		todayLevels=0;
		todayOffset = sessionMng.activeKid.playedLava;
		sessionMng.SaveSession ();
		
		numObj = 0;
		state = "Intro";
		waitTimer = waitTime;
		
		apTimer = apTime;
		//		SetLevel();
		tutorial = true;
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (returnToIsland && !saveHandler.saving) 
		{
			fadeIn=true;
			//Application.LoadLevel("Archipielago");
		}
		totalTime += Time.deltaTime;
		levelTime += Time.deltaTime;
		switch(state){
		case "Intro":
			//ongui handles
			break;
		case "SetLevel":
			ResetLevel();
			SetLevel();
			state = "Instructions";
			break;
		case "Instructions":
			
			//ongui handles
			break;
		case "ShadowAppears":
			//shadow appears
			if(timerOn)
			{
				if(timerScript.TimerFunc(shadowTime))
				{
					shadowColor.a = 0f;
					shadow.transform.position = imageStorage;
					state = "PlayerPick";
				}
			}
			else
			{
				shadow.transform.position = shadowPos;
				if(ShadowAppear(shadow, timeOfFade, "appear"))
				{
					timerOn = true;
				}
			}
			
			break;
		case "PlayerPick":
			if(!objImageSet)
			{
				if(!appearImages)
				{
					for(int i = 0; i < options.Length; i++)
					{
						ShadowAppear(objInUse[i], 0f, "transparent");
						objInUse[i].transform.position = objPos[i];
						
						
					}
					appearImages = true;
				}
				else
				{
					if(numObj != objInUse.Length)
					{
						for(int i = 0; i < objInUse.Length; i++)
						{
							if(!gases[i].isPlaying)
							{
								gases[i].transform.position = new Vector3(objPos[i].x, objPos[i].y - yGasDif, objPos[i].z);
								gases[i].Play();
								if(configuration.sound==1)
									soundMng.PlaySound(9,0,0.1f);
							}
						}
						
						numObj++;
						//						
					}
					else
					{
						apTimer -= Time.deltaTime;
						if(apTimer <= 0)
						{
							apTimer = apTime;
							for(int i = 0; i < objInUse.Length; i++)
							{
								ShadowAppear(objInUse[i], 0f, "appear");
							}
							objImageSet = true;
							appearImages = false;
							numObj = 0;
						}
						
					}
				}
				
			}
			else
			{
				//onj selection
				if(!audSource.isPlaying && !gases[0].IsAlive())
				{
					
					pickTimer -= Time.deltaTime;
					num = (int)pickTimer + 1;
					if(pickTimer > 0)
					{
						if(Input.GetMouseButtonDown(0))
						{
							if(selS.SelectionFunc(Input.mousePosition))
							{
								objSel = selS.objSelected;
							}
						}
						if(Input.GetMouseButtonUp(0))
						{
							if(selS.SelectionFunc(Input.mousePosition))
							{
								if(objSel == selS.objSelected)
								{
									play = false;
									objName = objSel.GetComponent<Objects>().shadow;
									state = "Result";
								}
							}
						}
					}
					else
					{
						play = false;
						objSel=null;
						objName = "NoPick";
						state = "Result";
					}
					
				}
				
			}
			
			break;
		case "Result":
			tutorial = false;
			if(objName == shadow.name)
			{
				levelsPassed++;
				correct = true;
                scoreEffects.DisplayScore(scoreScript.TempScoreSum(), configuration.sound == 1);
				tries = 0;
				if (subLevel < configuration.levels [level].subLevels.Length - 1) {
					//					Debug.Log("sublevel++");
					subLevel++;	
				} else {
					if (level < configuration.levels.Length - 1) {
						level++;
						subLevel = 0;
					}
				}
				scoreScript.prevCorMult++;
				sessionMng.activeKid.lavaDifficulty=level;
				sessionMng.activeKid.laveLevel=subLevel;
				sessionMng.SaveSession();
			}
			else 
			{
				scoreScript.prevCorMult = 0;
				levelsRepeated++;
				correct = false;
				scoreEffects.DisplayError(configuration.sound==1);
				if(tries < 2)
				{
					tries++;
				}
				else
				{
					tries = 0;
					subLevel--;
					if(subLevel < 0)
					{
						subLevel = 0;
						level--;
						if(level < 0)
						{
							level = 0;
						}
					}
				}
				sessionMng.activeKid.lavaDifficulty=level;
				sessionMng.activeKid.laveLevel=subLevel;
				sessionMng.SaveSession();
			}
			levelSet = false;
			advState = "HideOptionsGas";
			state = "Advance";
			break;
		case "Advance":
			switch(advState){
			case "HideOptionsGas":
				
				for(int i = 0; i < objInUse.Length; i++)
				{
					if(!gases[i].isPlaying)
					{
						gases[i].transform.position = new Vector3(objPos[i].x, objPos[i].y - yGasDif, objPos[i].z);
						gases[i].Play();
						if(configuration.sound==1)
							soundMng.PlaySound(9,0,0.1f);
					}
				}
				advState = "HideOptions";
				
				break;
			case "HideOptions":
				waitTimer -= Time.deltaTime;
				if(waitTimer <= 0)
				{
					waitTimer = waitTime;
					for(int i = 0; i < objInUse.Length; i++)
					{
						objInUse[i].transform.position = imageStorage;
					}
					advState = "ShowOptionsGas";
				}
				
				
				break;
			case "ShowOptionsGas":
				if(!gases[0].isPlaying)
				{
					for(int i = 0; i < 2; i++)
					{
						if(!parts[i].isPlaying)
						{
							parts[i].transform.position = new Vector3(resultPos[i].x, resultPos[i].y - yGasDif, resultPos[i].z);
							parts[i].Play();
							if(configuration.sound==1)
								soundMng.PlaySound(9,0,0.1f);
						}
					}
					advState = "ShowOptions";
				}
				
				
				break;
			case "ShowOptions":
				waitTimer -= Time.deltaTime;
				if(waitTimer <= 0)
				{
					waitTimer = waitTime;
					if(!objPresented)
					{
						shadow.transform.position = resultPos[0];
						shadowColor = shadow.GetComponent<SpriteRenderer>().color;
						shadowColor.a = 1f;
						shadow.GetComponent<SpriteRenderer>().color = shadowColor;
						if(objSel == null)
						{
							objSel = shadow;
						}
						objSel.transform.position = resultPos[1];
						
					}
					advState = "WaitForClick";
				}
				
				
				break;
			case "WaitForClick":
				if(!parts[0].isPlaying)
				{
					gasFinished = true;
					objPresented = true;
				}
				break;
			}
			break;
		case "SaveProgress":
			SaveLevelProgress();
			break;
		case "CompletedActivity":
			if(!scoreScript.finalScore)
			{
				kiwiAnimCurrTime-=Time.deltaTime;
				if(kiwiAnimCurrTime<=0)
				{
					kiwiAnimCurrTime=kiwiAnimTime;
                    if (animationKiwis < kiwis)
                    {
                        animationKiwis++;
                    }
                    else
                    {
                        scoreScript.ScoreAddValue();
                    }
				}
			}
			else
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
			int totalScore=(int)(((float)levelsPassed/(float)playedLevels)*100);
			Debug.Log("Total Score"+totalScore.ToString());
			soundMng.pauseQueue=true;
			if(totalScore>80)
			{
				soundMng.AddSoundToQueue(12,false,false);
				kiwis=3;
			}else if(totalScore>60)
			{
				soundMng.AddSoundToQueue(11,false,false);
				//soundMng.PlaySound(11,0,0.2f);
				kiwis=2;
			}else if(totalScore>20)
			{
				soundMng.AddSoundToQueue(10,false,false);
				//soundMng.PlaySound(10,0,0.2f);
				kiwis=1;
			}
			int tempKiwis=sessionMng.activeKid.kiwis;
            sessionMng.activeKid.kiwis = tempKiwis + kiwis + scoreScript.GetExtraKiwis();
			sessionMng.SaveSession();
			if(kiwis==0)
			{
				soundMng.AddSoundToQueue(7,false,false);
				soundMng.AddSoundToQueue(8,true,false);
			}else{
				soundMng.AddSoundToQueue(6,false,false);
				soundMng.AddSoundToQueue(8,true,false);
				//soundMng.PlaySoundQueue(new int[]{6,8});
			}
			GetComponent<ProfileSync>().UpdateProfile();
		}
	}

	void SaveLevelProgress()
	{
		saveHandler.AddLevelData("levelKey", "Sombras");
		saveHandler.AddLevelData("level", level);
		saveHandler.AddLevelData("sublevel", subLevel);
		saveHandler.AddLevelData("shadow", shadow.name);
		saveHandler.AddLevelData("shadowTime", shadowTime);
		saveHandler.AddLevelData("numOfOptions", options.Length);
		saveHandler.AddLevelData("options", options);
		saveHandler.AddLevelData("pickTime", (int)pickTime);
		saveHandler.AddLevelData("correct", correct);
		saveHandler.AddLevelData("time", (int)levelTime);
		levelTime = 0;
		playedLevels++;
		todayLevels++;
		sessionMng.activeKid.playedLava = todayLevels + todayOffset;
		
		//saveHandler.SetLevel();
		if(playedLevels+todayOffset >= numLevelsPerBlock)
		{
			CalculateKiwis();
			sessionMng.activeKid.blockedSombras=1;
			SaveProgress(true);
			state = "CompletedActivity";
		}
		else
		{
			state = "SetLevel";
		}
		sessionMng.SaveSession ();
		
	}
	void SaveProgress(bool rank)
	{
		if(playedLevels>0)
		{
			saveHandler.CreateSaveBlock("JuegoDeSombras", (int)totalTime, levelsPassed, levelsRepeated, playedLevels);
			saveHandler.AddLevelsToBlock();
			saveHandler.PostProgress(rank);
			Debug.Log(saveHandler.ToString());
		}
	}
	bool ShadowAppear(GameObject s, float time, string mode)
	{
		if(mode == "transparent")
		{
			shadowColor = s.GetComponent<SpriteRenderer>().color;
			shadowColor.a = 0;
			s.GetComponent<SpriteRenderer>().color = shadowColor;
			return true;
		}
		else if(mode == "appear")
		{
			SpriteRenderer sr = s.GetComponent<SpriteRenderer>();
			if(shadowColor.a == 0f)
			{
				shadowColor = sr.color;
			}
			float fPerSecond = 1 / time;
			
			shadowColor.a += fPerSecond * Time.deltaTime;
			sr.color = shadowColor;
			
			if(shadowColor.a >= 1)
			{
				shadowColor = new Color(0, 0, 0, 0);
				return true;
			}
		}
		else
		{
			Debug.LogError("Please select a correct mode 'transparent' or 'appear'");
		}
		
		return false;
	}
	void ResetLevel()
	{
		foreach(GameObject o in objects)
		{
			o.transform.position = imageStorage;
		}
		foreach(GameObject o in shadows)
		{
			SpriteRenderer sr = o.GetComponent<SpriteRenderer>();
			shadowColor = sr.color;
			shadowColor.a = 0;
			sr.color = shadowColor;
			o.transform.position = imageStorage;
		}
	}
	void SetLevel()
	{
		if(inverse)
		{
			shadow.transform.localScale = new Vector3(-shadow.transform.localScale.x, shadow.transform.localScale.y, shadow.transform.localScale.z);
		}
		parts.Clear();
		s.Clear();
		objImageSet = false;
		timerOn = false;
		shadow = null;
		shadowTime = configuration.levels[level].subLevels[subLevel].timeOfShadow;
		inverse = configuration.levels[level].subLevels[subLevel].inverse;
		pickTime = configuration.levels[level].subLevels[subLevel].pickTime;
		pickTimer = pickTime;
		string shadowName = configuration.levels[level].subLevels[subLevel].objectOptions[Random.Range(0, configuration.levels[level].subLevels[subLevel].objectOptions.Length)];
		
		foreach(GameObject o in shadows)
		{
			if(o.name == shadowName + "Sombra")
			{
				shadow = o;
			}
		}
		if(inverse)
		{
			shadow.transform.localScale = new Vector3(-shadow.transform.localScale.x, shadow.transform.localScale.y, shadow.transform.localScale.z);
		}
		
		options = new string[configuration.levels[level].subLevels[subLevel].numOfOptions];
		int r = Random.Range(0, options.Length);
		options[r] = shadowName;
		
		foreach(string ss in configuration.levels[level].subLevels[subLevel].objectOptions)
		{
			
			if(ss != shadowName)
			{
				s.Add(ss);
			}
		}
		for(int i = 0;i < configuration.levels[level].subLevels[subLevel].numOfOptions; i++)
		{
			if(i != r)
			{
				int k =Random.Range(0, s.Count);
				options[i] = s[k];
				s.Remove(options[i]);
			}
			
		}
		objInUse = new GameObject[options.Length];
		for(int i = 0; i < options.Length; i++)
		{
			objInUse[i] = GameObject.Find(options[i]).gameObject;
		}
		
		
		if(options.Length != 4)
		{
			objPos = new Vector3[options.Length];
			for(int i = 0; i < objPos.Length; i++)
			{
				
				if(i == 0)
				{
					objPos[i] = new Vector3(0f, shadowPos.y, -1f);
					gases[i].transform.position = new Vector3(objPos[i].x, objPos[i].y - yGasDif, objPos[i].z);
					parts.Add(gases[i]);
				}
				else if(i == 1)
				{
					objPos[i] = new Vector3(-2.5f, shadowPos.y, -1f);
					gases[i].transform.position = new Vector3(objPos[i].x, objPos[i].y - yGasDif, objPos[i].z);
					parts.Add(gases[i]);
				}
				else if(i == 2)
				{
					objPos[i] = new Vector3(2.5f, shadowPos.y, -1f);
					gases[i].transform.position = new Vector3(objPos[i].x, objPos[i].y - yGasDif, objPos[i].z);
					parts.Add(gases[i]);
				}
				else if(i == 3)
				{
					objPos[i] = new Vector3(-5f, shadowPos.y, -1f);
					gases[i].transform.position = new Vector3(objPos[i].x, objPos[i].y - yGasDif, objPos[i].z);
					parts.Add(gases[i]);
				}
				else if(i == 4)
				{
					objPos[i] = new Vector3(5f, shadowPos.y, -1f);
					gases[i].transform.position = new Vector3(objPos[i].x, objPos[i].y - yGasDif, objPos[i].z);
					parts.Add(gases[i]);
				}
				
			}
		}
		else
		{
			objPos = new Vector3[options.Length];
			for(int i = 0; i < objPos.Length; i++)
			{
				if(i == 0)
				{
					objPos[i] = new Vector3(-1.25f, shadowPos.y, -1f);
					gases[i].transform.position = new Vector3(objPos[i].x, objPos[i].y - yGasDif, objPos[i].z);
					parts.Add(gases[i]);
				}
				else if(i == 1)
				{
					objPos[i] = new Vector3(1.25f, shadowPos.y, -1f);
					gases[i].transform.position = new Vector3(objPos[i].x, objPos[i].y - yGasDif, objPos[i].z);
					parts.Add(gases[i]);
				}
				else if(i == 2)
				{
					objPos[i] = new Vector3(-3.75f, shadowPos.y, -1f);
					gases[i].transform.position = new Vector3(objPos[i].x, objPos[i].y - yGasDif, objPos[i].z);
					parts.Add(gases[i]);
				}
				else if(i == 3)
				{
					objPos[i] = new Vector3(3.75f, shadowPos.y, -1f);
					gases[i].transform.position = new Vector3(objPos[i].x, objPos[i].y - yGasDif, objPos[i].z);
					parts.Add(gases[i]);
				}
			}
		}
		
		levelSet = true;
		
	}
	void OnGUI()
	{
		float yOffSet = 200 * screenScale;
		screenScale = (float)Screen.height / (float)768;
		pauseText.fontSize = (int)(60 * screenScale);
		styleS.fontSize = (int)(35 * screenScale);
		advanceStyle.fontSize = (int)(30 * screenScale);
		label = new Rect(Screen.width * .2f, Screen.height * .2f, 300 * screenScale, 400 * screenScale);
		switch(state){
		case "Intro":
			if(!play)
				soundMng.PlaySound(0);
			play = true;
			GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), textBG);
			styleS.normal.textColor = new Color(0.32f,0.32f,0.32f);
			GUI.Label(new Rect(Screen.width * .5f - 399 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[0], styleS);
			styleS.normal.textColor = new Color(0.88f,0.88f,0.88f);
			GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[0], styleS);
			if(!audSource.isPlaying)
			{
				play = false;
				state = "SetLevel";
			}
			break;
		case "Instructions":
			if(tutorial)
			{
				if(!play)
					soundMng.PlaySound(1);
				play = true;
				GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), textBG);
				styleS.normal.textColor = new Color(0.32f,0.32f,0.32f);
				GUI.Label(new Rect(Screen.width * .5f - 399 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[1], styleS);
				styleS.normal.textColor = new Color(0.88f,0.88f,0.88f);
				GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[1], styleS);
				if(!audSource.isPlaying)
					if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 180 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[16], advanceStyle))
				{
					play = false;
					state = "ShadowAppears";
				}
			}
			else
			{
				state = "ShadowAppears";
			}
			
			break;
		case "ShadowAppears":
			GUI.DrawTexture(new Rect(Screen.width * .5f - 200 * screenScale, Screen.height * .5f - 100 *screenScale - yOffSet , 400 * screenScale, 200 * screenScale), textBG);
			styleS.normal.textColor = new Color(0.32f,0.32f,0.32f);
			GUI.Label(new Rect(Screen.width * .5f - 198 * screenScale, Screen.height * .5f - 98 * screenScale - yOffSet, 400 * screenScale, 200 * screenScale), language.levelStrings[2], styleS);
			styleS.normal.textColor = new Color(0.88f,0.88f,0.88f);
			GUI.Label(new Rect(Screen.width * .5f - 200 * screenScale, Screen.height * .5f - 100 * screenScale - yOffSet, 400 * screenScale, 200 * screenScale), language.levelStrings[2], styleS);
			
			break;
		case "PlayerPick":
			if(tutorial)
			{
				if(!gases[0].isPlaying)
					if(objImageSet)
				{
					if(!play)
						soundMng.PlaySound(2);
					play = true;
					if(audSource.isPlaying)
					{
						GUI.DrawTexture(new Rect(Screen.width * .5f - 200 * screenScale, Screen.height * .5f - 100 *screenScale - yOffSet , 400 * screenScale, 200 * screenScale), textBG);
						styleS.normal.textColor = new Color(0.32f,0.32f,0.32f);
						GUI.Label(new Rect(Screen.width * .5f - 198 * screenScale, Screen.height * .5f - 98 * screenScale - yOffSet, 400 * screenScale, 200 * screenScale), language.levelStrings[3], styleS);
						styleS.normal.textColor = new Color(0.88f,0.88f,0.88f);
						GUI.Label(new Rect(Screen.width * .5f - 200 * screenScale, Screen.height * .5f - 100 * screenScale - yOffSet, 400 * screenScale, 200 * screenScale), language.levelStrings[3], styleS);
					}
					else
					{
						//counter
						
						GUI.DrawTexture(new Rect(Screen.width * .5f - 200 * screenScale, Screen.height * .5f - 100 *screenScale - yOffSet , 400 * screenScale, 200 * screenScale), textBG);
						styleS.normal.textColor = new Color(0.32f,0.32f,0.32f);
						GUI.Label(new Rect(Screen.width * .5f - 198 * screenScale, Screen.height * .5f - 98 * screenScale - yOffSet, 400 * screenScale, 200 * screenScale), "" + num, styleS);
						styleS.normal.textColor = new Color(0.88f,0.88f,0.88f);
						GUI.Label(new Rect(Screen.width * .5f - 200 * screenScale, Screen.height * .5f - 100 * screenScale - yOffSet, 400 * screenScale, 200 * screenScale), "" + num, styleS);
					}
					
				}
			}
			else
			{
				if(!gases[0].isPlaying)
				{
					//counter
					
					GUI.DrawTexture(new Rect(Screen.width * .5f - 200 * screenScale, Screen.height * .5f - 100 *screenScale - yOffSet , 400 * screenScale, 200 * screenScale), textBG);
					styleS.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * .5f - 198 * screenScale, Screen.height * .5f - 98 * screenScale - yOffSet, 400 * screenScale, 200 * screenScale), "" + num, styleS);
					styleS.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * .5f - 200 * screenScale, Screen.height * .5f - 100 * screenScale - yOffSet, 400 * screenScale, 200 * screenScale), "" + num, styleS);
				}
			}
			
			
			break;
		case "Result":
			break;
		case "Advance":
			
			if(gasFinished && objPresented)
			{
				if(correct)
				{
					if(!play && tutorial)
					{
						soundMng.PlaySound(3);
					}
						
					play = true;
					GUI.DrawTexture(new Rect(Screen.width * .5f - 300 * screenScale, Screen.height * .5f - 100 *screenScale - yOffSet - yOffSet * .2f , 600 * screenScale, 200 * screenScale), textBG);
					styleS.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * .5f - 198 * screenScale, Screen.height * .5f - 98 * screenScale - yOffSet - yOffSet * .2f, 400 * screenScale, 200 * screenScale), language.levelStrings[4], styleS);
					styleS.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * .5f - 200 * screenScale, Screen.height * .5f - 100 * screenScale - yOffSet - yOffSet * .2f, 400 * screenScale, 200 * screenScale), language.levelStrings[4], styleS);
				}
				else
				{
					if(!play && tutorial)
					{
						soundMng.PlaySound(4);
					}
						
					play = true;
					GUI.DrawTexture(new Rect(Screen.width * .5f - 300 * screenScale, Screen.height * .5f - 100 *screenScale - yOffSet - yOffSet * .2f , 600 * screenScale, 200 * screenScale), textBG);
					styleS.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * .5f - 198 * screenScale, Screen.height * .5f - 98 * screenScale - yOffSet - yOffSet * .2f, 400 * screenScale, 200 * screenScale), language.levelStrings[5],styleS);
					styleS.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * .5f - 200 * screenScale, Screen.height * .5f - 100 * screenScale - yOffSet - yOffSet * .2f, 400 * screenScale, 200 * screenScale), language.levelStrings[5],styleS);
				}
				
				if(objPresented)
				{
					if(!audSource.isPlaying)
					{
						play = false;
						if(!gases[0].isPlaying)
						{
							for(int i = 0; i < 2; i++)
							{
								if(!parts[i].isPlaying)
								{
									parts[i].transform.position = new Vector3(resultPos[i].x, resultPos[i].y - yGasDif, resultPos[i].z);
									parts[i].Play();
									if(configuration.sound==1)
										soundMng.PlaySound(9,0,0.1f);;
								}
							}
						}
						ad = true;
						objPresented = false;
						gasFinished = false;
						shadowColor = shadow.GetComponent<SpriteRenderer>().color;
						shadowColor.a = 0f;
						shadow.transform.position = imageStorage;
						objSel.transform.position = imageStorage;
						
					}
					
					
				}
			}
			
			if(!parts[0].isPlaying && ad)
			{
				ad = false;
				state = "SaveProgress";
			}
			
			break;
		case "CompletedActivity":
			GUI.DrawTexture (new Rect (0, Screen.height * 0.5f - 237 * screenScale, Screen.width, 475 * screenScale), scoreBG);
			
			switch(animationKiwis){
			case 0:	
				GUI.DrawTexture (new Rect (Screen.width * 0.5f - 150 * screenScale, Screen.height * 0.5f - 10 * screenScale, 90 * screenScale, 90 * screenScale), scoreKiwiDisabled);
				GUI.DrawTexture (new Rect (Screen.width * 0.5f - 45 * screenScale, Screen.height * 0.5f - 10 * screenScale, 90 * screenScale, 90 * screenScale), scoreKiwiDisabled);
				GUI.DrawTexture (new Rect (Screen.width * 0.5f +60 * screenScale, Screen.height * 0.5f -10 * screenScale, 90 * screenScale, 90 * screenScale), scoreKiwiDisabled);		
				break;
			case 1:	
				GUI.DrawTexture (new Rect (Screen.width * 0.5f - 150 * screenScale, Screen.height * 0.5f - 10 * screenScale, 90 * screenScale, 90 * screenScale), scoreKiwi);
				GUI.DrawTexture (new Rect (Screen.width * 0.5f - 45 * screenScale, Screen.height * 0.5f - 10 * screenScale, 90 * screenScale, 90 * screenScale), scoreKiwiDisabled);
				GUI.DrawTexture (new Rect (Screen.width * 0.5f +60 * screenScale, Screen.height * 0.5f -10 * screenScale, 90 * screenScale, 90 * screenScale), scoreKiwiDisabled);
				break;
			case 2:	
				GUI.DrawTexture (new Rect (Screen.width * 0.5f - 150 * screenScale, Screen.height * 0.5f - 10 * screenScale, 90 * screenScale, 90 * screenScale), scoreKiwi);
				GUI.DrawTexture (new Rect (Screen.width * 0.5f - 45 * screenScale, Screen.height * 0.5f - 10 * screenScale, 90 * screenScale, 90 * screenScale), scoreKiwi);
				GUI.DrawTexture (new Rect (Screen.width * 0.5f +60 * screenScale, Screen.height * 0.5f -10 * screenScale, 90 * screenScale, 90 * screenScale), scoreKiwiDisabled);
				break;
			case 3:	
				GUI.DrawTexture (new Rect (Screen.width * 0.5f - 150 * screenScale, Screen.height * 0.5f - 10 * screenScale, 90 * screenScale, 90 * screenScale), scoreKiwi);
				GUI.DrawTexture (new Rect (Screen.width * 0.5f - 45 * screenScale, Screen.height * 0.5f - 10 * screenScale, 90 * screenScale, 90 * screenScale), scoreKiwi);
				GUI.DrawTexture (new Rect (Screen.width * 0.5f +60 * screenScale, Screen.height * 0.5f -10 * screenScale, 90 * screenScale, 90 * screenScale), scoreKiwi);
				break;
			}
			if(kiwis>0)
				GUI.Label (new Rect (Screen.width * 0.5f - 400 * screenScale, Screen.height * 0.5f - 150 * screenScale, 800 * screenScale, 100 * screenScale), language.levelStrings[6],scoreStyle1);
			else
				GUI.Label (new Rect (Screen.width * 0.5f - 400 * screenScale, Screen.height * 0.5f - 150 * screenScale, 800 * screenScale, 100 * screenScale), language.levelStrings[7],scoreStyle1);
			if(kiwis==0)
				GUI.Label (new Rect (Screen.width * 0.5f - 400 * screenScale, Screen.height * 0.5f - 90 * screenScale, 800 * screenScale, 100 * screenScale), language.levelStrings[8],scoreStyle2);
			else{
				if(kiwis>1)
					GUI.Label (new Rect (Screen.width * 0.5f - 400 * screenScale, Screen.height * 0.5f - 90 * screenScale, 800 * screenScale, 100 * screenScale), language.levelStrings[9]+" "+kiwis.ToString()+" "+language.levelStrings[10],scoreStyle2);
				else
					GUI.Label (new Rect (Screen.width * 0.5f - 400 * screenScale, Screen.height * 0.5f - 90 * screenScale, 800 * screenScale, 100 * screenScale), language.levelStrings[9]+" "+kiwis.ToString()+" "+language.levelStrings[11],scoreStyle2);
			}
			if(GUI.Button(new Rect(Screen.width*0.5f-80*screenScale,Screen.height*0.5f+110*screenScale,160*screenScale,60*screenScale),language.levelStrings[13],kiwiButton))
			{
				returnToIsland=true;
			}
			if(scoreScript.finalScore)
			{
				scoreScript.scoreStyle.fontSize = 50;
				scoreScript.scoreStyle.fontSize = (int)(scoreScript.scoreStyle.fontSize * screenScale);
				GUI.Label(new Rect(Screen.width * 0.5f + 270 * screenScale, Screen.height * 0.5f - 90 * screenScale, 100 *screenScale, 50 * screenScale), scoreScript.scoreString, scoreScript.scoreStyle);
				
				scoreScript.GuiExtraKiwisDisplay();
				if(scoreScript.scoreCounter >= scoreScript.kiwiMilestone)
				{
					GUI.DrawTexture (new Rect (Screen.width * 0.5f + 245 * screenScale, Screen.height * 0.5f - 20 * screenScale, 150 * screenScale, 150 * screenScale), scoreKiwi);
					GUI.Label(new Rect(Screen.width * 0.5f + 380 * screenScale, Screen.height * 0.5f + 50 * screenScale, 100 *screenScale, 50 * screenScale), "x" + scoreScript.extraKiwis, scoreScript.scoreStyle);
				}
				
			}
			break;
		case "Pause":
			float pauseY = Screen.height * 0.5f - 177 * screenScale;
			GUI.DrawTexture(new Rect(0, pauseY, Screen.width, 354 * screenScale), pauseBackground);
			GUI.Label(new Rect(Screen.width * 0.5f - 100 * screenScale, pauseY - 40 * screenScale, 200 * screenScale, 60 * screenScale), language.levelStrings[12], pauseText);
			if(GUI.Button(new Rect(Screen.width * 0.5f - 200 * screenScale, pauseY + 50 * screenScale, 366 * screenScale, 66 * screenScale), "", pauseContinue))
			{
				Time.timeScale = 1f;
				state = stateBeforePause;
			}
			else if(GUI.Button(new Rect(Screen.width * 0.5f-200 * screenScale, pauseY + 140 * screenScale, 382 * screenScale, 66 * screenScale), "", pauseIsland))
			{
				if(!returnToIsland){
					SaveProgress(true);
					returnToIsland=true;
					Time.timeScale=1.0f;
				}
				//Application.LoadLevel("Archipielago");
			}
			GUI.Label(new Rect(Screen.width*0.5f-110*screenScale,pauseY+50*screenScale,366*screenScale,66*screenScale),language.levelStrings[14],pauseButtons);
			GUI.Label(new Rect(Screen.width*0.5f-110*screenScale,pauseY+140*screenScale,382*screenScale,66*screenScale),language.levelStrings[15],pauseButtons);
			//			else if(GUI.Button(new Rect(Screen.width * 0.5f - 200 * screenScale, pauseY + 230 * screenScale, 162 * screenScale, 67 * screenScale), "", pauseExit))
			//			{
			//				Time.timeScale = 1.0f;
			//				Application.Quit();
			//			}
			break;
			
		}
		if(state != "Pause"&&state!="CompletedActivity")
		{
			if(GUI.Button(new Rect(10 * screenScale, 10 * screenScale, 71 * screenScale, 62 * screenScale), "", pauseButton))
			{
				Time.timeScale = 0f;
				stateBeforePause = state;
				state = "Pause";
			}
			
		}
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
	}
}
