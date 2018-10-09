using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreasureLogic : MonoBehaviour
{
	public bool touchEnabled=false;
	public GameObject character;
	public float waitTime = 100.0f;
	public string state = "Instructions";
	public string targetId;
	public Joystick joyScript;
	BotControlScript controller;
	
	
	public List<Item> itemList = new List<Item> ();
	public List<string> spawnedObjects = new List<string> ();
	public List<string> spawnedDistractors = new List<string> ();
	PlayerInteraction player;
	public TreasureSpawner[] spawners;
	public bool complete = false;
	public bool tryAgain = false;
	public bool reviewObjects = false;
	public Camera playerCamera;
	public Camera overviewCamera;
	public GameSaveLoad loader;
	[HideInInspector]
	public LevelConfiguration configuration;
	public int numLevelsPerBlock;
	public int level;
	public int subLevel;
	int todayLevels;
	int todayOffset;
	TreasureChest chest;
	bool firstLoad;
	bool gameWon;
	public GUIStyle instructionsStyle;
	public GUIStyle interactionStyle;
	public GUIStyle tutorialStyle;
	public GUIStyle quantityStyle;
	public GUIStyle nameStyle;
	public GUIStyle headerSmallStyle;
	public GUIStyle smallStyle;
	public GUIStyle readyButton;
	public GUIStyle kiwiButton;
	public GUIStyle pauseButtons;
	public Texture grabButtonStyle;
	public Texture greenBox;
	public Texture orangeBox;
	Vector3 playerPosition;
	int numberOfObjects = 0;
	int[]objectCount = new int[8];
	Texture[]texturesToDisplay = new Texture[8];
	public int objectLayout = 0;
	Vector2[]positions = new Vector2[8];
	Vector2[]positionsBag = new Vector2[8];
	Vector2[]positionsSmall = new Vector2[8];
	int[]differences = new int[8];
	public int horizontalSpace = 350;
	public int horizontalSpaceCorrection = 245;
	public int horizontalSpaceSmall = 150;
	public int verticalSpace = 20;
	int categoriesRemaining = 0;
	public int objectsToRemove = 0;
	public bool objectsMissing = false;
	Texture2D red;
	Texture2D blue;
	public bool tutorial;
	bool isTutorial=false;
	int tutorialStep = 0;
	float tutorialTime = 6f;
	float mapScale=1;
	float yOffsetCP=Screen.height*0.6f;
	float yOffsetSmall=Screen.height*0.35f;
	bool justPressed=false;
	public GUIStyle noStyle;
	public GUIStyle yesStyle;
	public Texture textBG;
	Vector3 characterPosition;
	bool correctedPhase=false;
	float endTime=8;
	bool returnToIsland=false;
	public GameObject joystick;
	public Texture2D bgColor;
	public GUIStyle textBagStyle;
	[HideInInspector]
	public SoundManager soundMng;
	SessionManager sessionMng;
	
	//Pause
	public GUIStyle pauseButton;
	public Texture2D pauseBackground;
	public GUIStyle pauseContinue;
	public GUIStyle pauseIsland;
	public GUIStyle pauseExit;
	public GUIStyle pauseText;
	public string stateBeforePause="";
	public Texture2D scoreBG;
	public Texture2D scoreKiwi;
	public Texture2D scoreKiwiDisabled;
	public GUIStyle scoreStyle1;
	public GUIStyle scoreStyle2;
	public GUIStyle scoreStyle3;
	int kiwis=-1;
	int animationKiwis=0;
	public float kiwiAnimTime=2;
	float kiwiAnimCurrTime;
	//itemList.Add("test");
	public Texture loadingScreen;
	public Texture en_loadingScreen;
	public Texture loadingColor;
	bool fadeIn=false;
	bool fadeOut=false;
	float opacity=0;

	//Progress
	ProgressHandler saveHandler;
	int playedLevels=0;
	int passedLevels=0;
	int repeatedLevels=0;
	int notSureCorrect=0;
	int notSureIncorrect=0;
	float levelTime=0;
	float totalTime=0;
	bool repeated=false;
	int tries=0;
	
	//tutorial
	bool tutObjFound = false;
	List<GameObject> tutObjs = new List<GameObject>();
	List <GameObject> arrows = new List<GameObject>();
	string tutObjName;
	public GameObject arrow;
	public bool grab = false;
	
	
	public ScoreEffects scoreEffects;

	[HideInInspector]
	public LanguageLoader language;

    public Scores scoreScript;
	
	// Use this for initialization
	void Start ()
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
		if(sessionMng.activeKid.tesoroTutorial == 0)
		{
			tutorial = true;
			isTutorial=true;
			sessionMng.activeKid.tesoroTutorial=1;
			sessionMng.SaveSession();
		}
		else
		{
			tutorial = false;
			isTutorial=false;
		}
		//Debug.Log (tutorial);
		kiwiAnimCurrTime = kiwiAnimTime;
		//tutorial = true;
		tutorialStep = 2;
		chest = (TreasureChest)GameObject.Find ("Treasure Chest").GetComponent (typeof(TreasureChest));
		chest.UpdateNames (language.levelStrings, 32, language.levelStrings.Length);
		level = sessionMng.activeKid.treasureDifficulty;
		subLevel = sessionMng.activeKid.treasureLevel;
        todayLevels = 0;
        todayOffset = sessionMng.activeKid.playedTreasure;
		soundMng = GetComponent<SoundManager> ();
        scoreScript = GetComponent<Scores>();
		
		//level = 0;
		//subLevel = 0;
		playerCamera.enabled = false;
		character = GameObject.Find ("Character").gameObject;
		characterPosition = character.transform.position;
		player = (PlayerInteraction)character.GetComponent (typeof(PlayerInteraction));
		playerPosition = character.gameObject.transform.position;
		
		controller = character.GetComponentInChildren<BotControlScript>();
		//checks for touch devices
		if(SystemInfo.deviceType == DeviceType.Handheld)
		{
			touchEnabled = true;
		}
		else 
		{
			touchEnabled = false;
		}
		//touchEnabled = true;
		if (touchEnabled)
		{
			player.GetComponent<BotControlScript> ().control = BotControlScript.ControlType.Touchscreen;
		}
		
		loader = (GameSaveLoad)GetComponent (typeof(GameSaveLoad));
		saveHandler=(ProgressHandler)GetComponent(typeof(ProgressHandler));
		firstLoad = true;
		gameWon = false;
		
		red = new Texture2D (120, 120);
		Color rgbColor = new Color (1, 0, 0);
		
		for (int i=0; i<120; i++) 
		{
			for (int j=0; j<120; j++) 
			{
				red.SetPixel (i, j, rgbColor);	
			}
		}
		red.Apply ();
		blue = new Texture2D (120, 120);
		rgbColor = new Color (0, 0, 1);
		
		for (int i=0; i<120; i++)
		{
			for (int j=0; j<120; j++) 
			{
				blue.SetPixel (i, j, rgbColor);	
			}
		}
		
		blue.Apply ();
		mapScale = (float)Screen.height / (float)768;
		instructionsStyle.fontSize = (int)(instructionsStyle.fontSize*mapScale);
		interactionStyle.fontSize = (int)(interactionStyle.fontSize*mapScale);
		quantityStyle.fontSize = (int)(quantityStyle.fontSize*mapScale);
		smallStyle.fontSize = (int)(smallStyle.fontSize * mapScale);
		nameStyle.fontSize = (int)(nameStyle.fontSize*mapScale);
		headerSmallStyle.fontSize = (int)(headerSmallStyle.fontSize * mapScale);
		tutorialStyle.fontSize = (int)(tutorialStyle.fontSize * mapScale);
		pauseText.fontSize = (int)(pauseText.fontSize * mapScale);
		scoreStyle1.fontSize = (int)(scoreStyle1.fontSize * mapScale);
		scoreStyle2.fontSize = (int)(scoreStyle2.fontSize * mapScale);
		scoreStyle3.fontSize = (int)(scoreStyle3.fontSize * mapScale);
		readyButton.fontSize = (int)(readyButton.fontSize * mapScale);
		kiwiButton.fontSize = (int)(kiwiButton.fontSize * mapScale);
		pauseButtons.fontSize = (int)(pauseButtons.fontSize * mapScale);
	}
	
	void SaveLevelProgress()
	{
		saveHandler.AddLevelData ("level", level);
		saveHandler.AddLevelData ("sublevel", subLevel);
		saveHandler.AddLevelData ("time", (int)levelTime);
		saveHandler.AddLevelData ("tutorial", isTutorial);
		saveHandler.AddLevelData ("passed", !tryAgain);
		List<string> tempPlayerObj = new List<string> ();
		List<int> tempPlayerObjQ = new List<int> ();
		for (int i=0; i<player.bag.Count; i++) {
			if(player.bag[i].number>0){
				tempPlayerObj.Add(player.bag[i].names[0]);
				tempPlayerObjQ.Add(player.bag[i].number);
			}
		}
		saveHandler.AddLevelData ("playerObjects", tempPlayerObj.ToArray());
		saveHandler.AddLevelData ("playerObjectsQuantity", tempPlayerObjQ.ToArray());
		
		List<string> tempCorrectObj = new List<string> ();
		List<int> tempCorrectObjQ = new List<int> ();
		for (int i=0; i<numberOfObjects; i++) {
			tempCorrectObj.Add(player.bag[i].names[0]);
			tempCorrectObjQ.Add(objectCount [i]);
		}
		saveHandler.AddLevelData ("correctObjects", tempCorrectObj.ToArray());
		saveHandler.AddLevelData ("correctObjectsQuantity", tempCorrectObjQ.ToArray());
		saveHandler.AddLevelData ("spawnedObjects", spawnedObjects.ToArray());
		saveHandler.AddLevelData ("spawnedDistractors", spawnedDistractors.ToArray());
		saveHandler.AddLevelData ("notSureCorrect", notSureCorrect);
		saveHandler.AddLevelData ("notSureIncorrect", notSureIncorrect);
		saveHandler.AddLevelData ("minObjects", configuration.levels[level].subLevels[subLevel].minObjectsQuantity);
		saveHandler.AddLevelData ("maxObjects", configuration.levels[level].subLevels[subLevel].maxObjectsQuantity);
		saveHandler.AddLevelData ("availableObjects", configuration.levels[level].subLevels[subLevel].availableObjects);
		saveHandler.AddLevelData ("availableCategories", configuration.levels[level].subLevels[subLevel].availableCategories);
		saveHandler.AddLevelData ("searchOrders", configuration.levels[level].subLevels[subLevel].search);
		saveHandler.AddLevelData ("availableDistractors", configuration.levels[level].subLevels[subLevel].distractors);
		if (!tutorial) 
		{
			if(!tryAgain)
			{
				if(tries>2)
				{
					if (subLevel > 0)
					{
						subLevel--;
					}
					else
					{
						if (level > 0)
						{
							level--;
							subLevel = configuration.levels[level].subLevels.Length-1;
						}
					}
				}else
				{
					if (subLevel < configuration.levels [level].subLevels.Length - 1) 
					{
						subLevel++;	
					}
					else 
					{
						if (level < configuration.levels.Length - 1) 
						{
							level++;
							subLevel = 0;
						}
					}
				}
			}
			sessionMng.activeKid.treasureDifficulty=level;
			sessionMng.activeKid.treasureLevel=subLevel;
			sessionMng.SaveSession();
		}
		isTutorial = false;
		levelTime = 0;
		correctedPhase=false;
		notSureCorrect = 0;
		notSureIncorrect = 0;
		playedLevels++;

        if (tryAgain)
        {
            scoreScript.prevCorMult=0;
            repeatedLevels++;
        }else
		{
            scoreScript.TempScoreSum();
            scoreScript.prevCorMult++;
			passedLevels++;
			todayLevels++;
			sessionMng.activeKid.playedTreasure=todayLevels+todayOffset;

		}
		
		//saveHandler.SetLevel();
		if (passedLevels+todayOffset >= numLevelsPerBlock)
		{
			CalculateKiwis();
			sessionMng.activeKid.blockedTesoro=1;
			SaveProgress (true);
			state = "CompletedActivity";
		} 
		else 
		{
			if(tryAgain)
			{
				tries++;
				repeated=true;
				soundMng.PlaySound(12);
			}
			else
			{
				tries=0;
				repeated=false;
				soundMng.PlaySound(9);
			}
			if(reviewObjects)
			{
				if(tryAgain)
					PrepareCorrection();
				else
					state="Complete";
			}
			else
			{
				state = "Complete";
			}
		}
		sessionMng.SaveSession ();
	}
	void SaveProgress(bool rank)
	{
		if (playedLevels > 0) 
		{
			saveHandler.CreateSaveBlock ("Tesoro", (int)totalTime, passedLevels, repeatedLevels, playedLevels);
			saveHandler.AddLevelsToBlock ();
			saveHandler.PostProgress (rank);
			Debug.Log (saveHandler.ToString ());
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (returnToIsland && !saveHandler.saving) 
		{
			fadeIn=true;
			//Application.LoadLevel("Archipielago");
		}
		levelTime += Time.deltaTime;
		totalTime += Time.deltaTime;
		if (!gameWon) 
		{
			if (firstLoad)
			{
				firstLoad = false;
				loader.Load (GameSaveLoad.game.treasure);
				configuration=(LevelConfiguration)loader.configuration;
				if(configuration.music==0)
				{
					AudioSource[] tempAudios = playerCamera.GetComponents<AudioSource>();
					tempAudios[0].volume=0;
					tempAudios[0].Stop();
				}else
				{
					AudioSource[] tempAudios = playerCamera.GetComponents<AudioSource>();
					tempAudios[0].Play();
				}
				if(configuration.sound==0)
				{
					AudioSource[] tempAudios = playerCamera.GetComponents<AudioSource>();
					tempAudios[1].volume=0;
					tempAudios[1].Stop();
					character.GetComponent<AudioSource>().volume=0;
					character.GetComponent<AudioSource>().Stop();
					GetComponent<AudioSource>().volume=0;
					GetComponent<AudioSource>().Stop();
				}else
				{
					AudioSource[] tempAudios = playerCamera.GetComponents<AudioSource>();
					tempAudios[1].Play();
				}
				Instructions ();
			}
			if(state == "CorrectionPhase" && objectsToRemove <= 0&&!objectsMissing)
			{
				tryAgain=false;
				state="SetComplete";
				waitTime=5;
				correctedPhase=true;
			}
			if ((state == "Complete") && !tutorial)
			{
				waitTime -= Time.deltaTime;
			}
			
			if (state == "CorrectionPhase")
			{
				justPressed=false;
				if (Input.GetMouseButtonDown (0))
				{
					Vector2 mousePos = new Vector2 (Input.mousePosition.x,Screen.height-Input.mousePosition.y);
					for (int i=0; i<player.bag.Count; i++) 
					{
						//GUI.DrawTexture (new Rect (positionsBag [i].x - 10*mapScale, positionsBag [i].y +yOffsetCP-10*mapScale, 235*mapScale, 135*mapScale), red);
						if (mousePos.x <= positionsBag [i].x + 225*mapScale && mousePos.x >= positionsBag [i].x-10*mapScale && mousePos.y >= positionsBag [i].y+yOffsetCP-10*mapScale && mousePos.y <= positionsBag [i].y+yOffsetCP + 125*mapScale) {
							if (i >= numberOfObjects)
							{
								player.bag [i].number--;
								objectsToRemove--;
								if (player.bag [i].number <= 0)
									player.bag.RemoveAt (i);
								if(objectsToRemove<=0&&objectsMissing)
									soundMng.PlaySound(10);
							}
							else 
							{
								if (player.bag [i].number > objectCount [i]) 
								{
									differences[i]++;
									player.bag [i].number--;
									objectsToRemove--;
									if(objectsToRemove<=0&&objectsMissing)
										soundMng.PlaySound(10);
								}
							}
							justPressed=true;
						}
					}
				}
				/*if(objectsToRemove<=0){
					state="Complete";
					tryAgain = false;
					waitTime = 3.0f;
				}*/
			}
			
			if(state=="CompletedActivity")
			{
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
                }
                else
                {
                    scoreScript.ScoreCounter();
                }
				
				//endTime-=Time.deltaTime;
				//if(endTime<=0){
				//	Application.LoadLevel("Archipielago");
				//}
			}
			
			if (state == "SetComplete") 
			{
				tutorialStep = 0;
				if (level < configuration.levels.Length) 
				{
					character.transform.position = playerPosition;
					character.SetActive (false);
					overviewCamera.enabled = true;
					playerCamera.enabled = false;
					character.GetComponent<BotControlScript>().enabled=false;
					if(correctedPhase)
					{
						SaveLevelProgress();
					}
					else
					{
						soundMng.PlaySound(8);
						state = "CheckComplete";
					}
				} 
				else
				{
					character.SetActive (false);
					overviewCamera.enabled = true;
					playerCamera.enabled = false;
					character.GetComponent<BotControlScript>().enabled=false;
					gameWon = true;	
					state = "GameOver";
				}
			}
			if (tutorial||state=="Instructions"||state=="CorrectionPhase")
			{
				if(touchEnabled)
				{
					switch (state) {
					case "Instructions"	:

						break;
					case "None"	:
						switch (tutorialStep) {
						case 0:
							//Debug.Log(controller.v);
							if (controller.v != 0) 
							{
								tutorialStep++;
								//								soundMng.PlaySound (3);
							}
							break;
						case 1:
							if (controller.h != 0)
							{
								tutorialStep++;
								//								soundMng.PlaySound (4);
							}
							break;
						case 2:
							if(!tutObjFound)
							{
								GameObject[] temp = GameObject.FindGameObjectsWithTag("Treasure");
								foreach(GameObject o in temp)
								{
									if(o.name != "SpawnZone")
									{
										Treasure t = o.GetComponent<Treasure>();
										//Debug.Log("Cat " + t.category + " item id " + itemList[0].id);
										if(t.category == itemList[0].id || t.id == itemList[0].id)
										{
											//Debug.Log("ins");
											tutObjs.Add(o);
											GameObject arrowT = Instantiate(arrow, new Vector3(o.transform.position.x, o.transform.position.y, o.transform.position.z), arrow.transform.localRotation) as GameObject;
											arrows.Add(arrowT);
											
										}
									}
									
								}
								
								tutObjFound = true;
							}
							
							if (player.currentObject != "") 
							{
								tutorialStep++;
								//								soundMng.PlaySound (5);
							}
							break;
						case 3:
							if (grab)
							{
								grab = false;
								foreach(GameObject a in arrows)
								{
									
									Destroy(a.gameObject);
									
								}
								tutObjFound = false;
								arrows.Clear();
								tutorialStep++;
								soundMng.PlaySound (6);
							}
							break;
						case 4:
							
							if(!tutObjFound)
							{
								GameObject arr = Instantiate(arrow, transform.position, arrow.transform.localRotation) as GameObject;
								tutObjFound = true;
							}
							
							tutorialTime -= Time.deltaTime;
							if (tutorialTime <= 0) 
							{
								tutorialStep++;
								tutorialTime = 5f;
								soundMng.PlaySound (7);
							}
							break;
						case 5:
							tutorialTime -= Time.deltaTime;
							if (tutorialTime <= 0) 
							{
								tutorialStep=0;
								tutorial=false;
								tutorialTime = 6f;
							}
							break;
						}
						break;
					case "CorrectionPhase":
						/*if (Input.GetMouseButtonDown (0)&&objectsToRemove <= 0&&!justPressed) 
						{
							state = "None";
							character.transform.position=characterPosition;
							character.SetActive (true);
							playerCamera.enabled = true;
							overviewCamera.enabled = false;
							tutorial=false;
						}*/
						if(!objectsMissing&&objectsToRemove <= 0)
						{
							state = "SetComplete";
							tutorial=false;
							tryAgain=false;
							correctedPhase=true;
						}
						break;
					case "Complete":
						switch (tutorialStep) {
						case 0:	
							waitTime -= Time.deltaTime;
							break;
						}
						break;
					}
				}
				else
				{
					switch (state) {
					case "Instructions"	:

						break;
					case "None"	:
						switch (tutorialStep) {
						case 0:
							if (Input.GetKeyDown ("up") || Input.GetKeyDown ("down")||Input.GetKeyDown ("w")||Input.GetKeyDown ("s")) 
							{
								tutorialStep++;
								soundMng.PlaySound (3);
							}
							break;
						case 1:
							if (Input.GetKeyDown ("right") || Input.GetKeyDown ("left")||Input.GetKeyDown ("a")||Input.GetKeyDown ("d"))
							{
								tutorialStep++;
								soundMng.PlaySound (4);
							}
							break;
						case 2:
							if(!tutObjFound)
							{
								GameObject[] temp = GameObject.FindGameObjectsWithTag("Treasure");
								foreach(GameObject o in temp)
								{
									if(o.name != "SpawnZone")
									{
										Treasure t = o.GetComponent<Treasure>();
										//Debug.Log("Cat " + t.category + " item id " + itemList[0].id);
										if(t.category == itemList[0].id || t.id == itemList[0].id)
										{
											//Debug.Log("ins");
											tutObjs.Add(o);
											GameObject arrowT = Instantiate(arrow, new Vector3(o.transform.position.x, o.transform.position.y, o.transform.position.z), arrow.transform.localRotation) as GameObject;
											arrows.Add(arrowT);
											
										}
									}
									
								}
								
								tutObjFound = true;
							}
							
							if (player.currentObject != "") 
							{
								tutorialStep++;
								soundMng.PlaySound (5);
							}
							break;
						case 3:
							if (Input.GetKeyDown ("space"))
							{
								foreach(GameObject a in arrows)
								{
									
									Destroy(a.gameObject);
									
								}
								tutObjFound = false;
								arrows.Clear();
								tutorialStep++;
								soundMng.PlaySound (6);
							}
							break;
						case 4:
							
							if(!tutObjFound)
							{
								GameObject arr = Instantiate(arrow, transform.position, arrow.transform.localRotation) as GameObject;
								tutObjFound = true;
							}
							
							tutorialTime -= Time.deltaTime;
							if (tutorialTime <= 0) 
							{
								tutorialStep++;
								tutorialTime = 5f;
								soundMng.PlaySound (7);
							}
							break;
						case 5:
							tutorialTime -= Time.deltaTime;
							if (tutorialTime <= 0) 
							{
								tutorialStep=0;
								tutorial=false;
								tutorialTime = 6f;
							}
							break;
						}
						break;
					case "CorrectionPhase":
						/*if (Input.GetMouseButtonDown (0)&&objectsToRemove <= 0&&!justPressed) 
						{
							state = "None";
							character.transform.position=characterPosition;
							character.SetActive (true);
							playerCamera.enabled = true;
							overviewCamera.enabled = false;
							tutorial=false;
						}*/
						if(!objectsMissing&&objectsToRemove <= 0)
						{
							state = "SetComplete";
							tutorial=false;
							tryAgain=false;
							correctedPhase=true;
						}
						break;
					case "Complete":
						switch (tutorialStep) {
						case 0:	
							waitTime -= Time.deltaTime;
							break;
						}
						break;
					}
				}
				
			}
			if (waitTime <= 0) 
			{
				switch (state) {
					/*case "CorrectionPhase":
					state = "None";
					quantityStyle.fontSize=(int)(55*mapScale);
					nameStyle.fontSize=(int)(24*mapScale);
					character.SetActive (true);
					playerCamera.enabled = true;
					overviewCamera.enabled = false;
					waitTime = 3.0f;
					break;*/
				case "Complete":
					if (tryAgain) 
					{
						PrepareCorrection();
					} 
					else 
					{
						tryAgain = false;
						complete = true;
					}
					break;
				}
			}
			if (complete) 
			{
				if (level < configuration.levels.Length) 
				{
					character.SetActive (false);
					Instructions ();
					complete = false;
				}
			}
		}
	}

	void CalculateKiwis()
	{
		if(kiwis==-1)
		{
			kiwis=0;
			int totalScore=(int)(((float)passedLevels/(float)playedLevels)*100);
			Debug.Log("Total Score"+totalScore.ToString());
			soundMng.pauseQueue=true;
			if(totalScore>80)
			{
				soundMng.AddSoundToQueue(18,false,false);
				kiwis=3;
			}else if(totalScore>60)
			{
				soundMng.AddSoundToQueue(17,false,false);
				kiwis=2;
			}else if(totalScore>20)
			{
				soundMng.AddSoundToQueue(16,false,false);
				kiwis=1;
			}
			int tempKiwis=sessionMng.activeKid.kiwis;
            sessionMng.activeKid.kiwis = tempKiwis + kiwis + scoreScript.GetExtraKiwis();
			sessionMng.SaveSession();
			if(kiwis==0)
			{
				soundMng.AddSoundToQueue(14,false,false);
				soundMng.AddSoundToQueue(15,true,false);
			}else{
				soundMng.AddSoundToQueue(13,false,false);
				soundMng.AddSoundToQueue(15,true,false);
			}
			GetComponent<ProfileSync>().UpdateProfile();
		}
	}

	void PrepareCorrection()
	{
		state = "CorrectionPhase";
		quantityStyle.fontSize=(int)(51*mapScale);
		nameStyle.fontSize=(int)(20*mapScale);
		objectsMissing = false;
		for (int i=0; i<player.bag.Count; i++) 
		{
			float h = 0;
			float h2 = 0;
			float w = Screen.width * 0.5f;
			if (player.bag.Count > 4) 
			{
				h += i > 3 ? verticalSpace*mapScale : 0;	
				h2 += i > 3 ? verticalSpace*mapScale : 0;	
			}
			if (i >= numberOfObjects) 
			{
				texturesToDisplay [i] = chest.GetTreasureImage (player.bag [i].id);
				objectsToRemove += player.bag [i].number;
				differences [i] -= player.bag [i].number;
			}
			else 
			{
				differences [i] = objectCount [i] - player.bag [i].number;
				if (differences [i] > 0)
					objectsMissing = true;
				objectsToRemove += Mathf.Max (player.bag [i].number - objectCount [i], 0);
			}
			positionsBag [i] = new Vector2 (w + (((i % 4) * horizontalSpaceCorrection*mapScale) - ((i < 4 ? player.bag.Count > 4 ? 4 : player.bag.Count : player.bag.Count - 4) * horizontalSpaceCorrection*mapScale) * 0.5f), h);
			positionsSmall [i] = new Vector2 (w + (((i % 4) * horizontalSpaceSmall*mapScale) - ((i < 4 ? numberOfObjects > 4 ? 4 : numberOfObjects : numberOfObjects - 4) * horizontalSpaceSmall*mapScale) * 0.5f), h2);
		}
		if(objectsToRemove>0)
		{
			soundMng.PlaySound(11);
		}
		else if(objectsMissing)
		{
			soundMng.PlaySound(10);
		}
		waitTime = 5.0f;
	}
	
	bool isCategorySpawned (string id)
	{
		for (int i=0; i<itemList.Count; i++) 
		{
			if (chest.GetTreasureCategory (itemList [i].id) == id)
				return true;
		}
		return false;
	}
	
	public string[] GetDistractors()
	{
		return configuration.levels [level].subLevels [subLevel].distractors;
	}
	public string[] GetAvailableObjects()
	{
		return configuration.levels [level].subLevels [subLevel].availableObjects;
	}
	
	void Instructions ()
	{
		itemList.Clear ();
		player.bag.Clear ();
		spawnedObjects.Clear ();
		spawnedDistractors.Clear ();
		player.images.Clear ();
		
		if(tutorial)
		{
			numberOfObjects = 1;
		}
		else
		{
			numberOfObjects = configuration.levels [level].subLevels [subLevel].search.Length;
		}

        string[] tempInstructionsObjectsArray = configuration.levels[level].subLevels[subLevel].availableObjects;
        string[] tempInstructionsCategories = configuration.levels[level].subLevels[subLevel].availableCategories;
        int tempInstructionsObjectsIdx = 0;
        int tempInstructionsCategoriesIdx = 0;
        int topObjectCount = spawners.Length;

        reshuffle(tempInstructionsObjectsArray);
        reshuffle(tempInstructionsCategories);

        int categoryCount = 0;

        for (int i = 0; i < configuration.levels[level].subLevels[subLevel].search.Length; i++)
        {
            if (configuration.levels[level].subLevels[subLevel].search[i] == "Category")
            {
                categoryCount++;
            }
        }

        List<SimpleItem> tempInstructionsObjects = new List<SimpleItem>();

        for (int i = 0; i < tempInstructionsObjectsArray.Length;i++ )
        {
            tempInstructionsObjects.Add(new SimpleItem(tempInstructionsObjectsArray[i],chest.GetTreasureCategory(tempInstructionsObjectsArray[i])));
        }

        for (int i = 0; i < categoryCount; i++)
        {
            string tempCategory = tempInstructionsCategories[Mathf.Min(i, tempInstructionsCategories.Length - 1)];
            tempInstructionsObjects.RemoveAll(x => x.category == tempCategory);
        }

        for (int i = 0; i < numberOfObjects; i++)
        {
            string tempSpawnType = configuration.levels[level].subLevels[subLevel].search[i];

            if (tutorial)
            {
                objectCount[i] = 1;
            }
            else
            {
                objectCount[i] = Random.Range(configuration.levels[level].subLevels[subLevel].minObjectsQuantity,
                                                configuration.levels[level].subLevels[subLevel].maxObjectsQuantity + 1);
            }

            topObjectCount -= objectCount[i];

            if (topObjectCount < 0)
            {
                break;
            }

            if (tempSpawnType == "Category")
            {
                categoriesRemaining += objectCount[i];

                targetId = tempInstructionsCategories[tempInstructionsCategoriesIdx];
                tempInstructionsCategoriesIdx = Mathf.Clamp(tempInstructionsCategoriesIdx + 1, 0, tempInstructionsCategories.Length - 1);

                texturesToDisplay[i] = chest.GetTreasureCategoryImage(targetId);
            }
            else
            {
                targetId = tempInstructionsObjects[tempInstructionsObjectsIdx].id;
                tempInstructionsObjectsIdx = Mathf.Clamp(tempInstructionsObjectsIdx + 1, 0, tempInstructionsObjects.Count - 1);

                texturesToDisplay[i] = chest.GetTreasureImage(targetId);
            }
            if (tempSpawnType == "Category")
            {
                itemList.Add(new Item(targetId, objectCount[i], chest.GetTreasurCategoryNames(targetId), tempSpawnType));
                player.bag.Add(new Item(targetId, 0, chest.GetTreasurCategoryNames(targetId), tempSpawnType));
                player.images.Add(chest.GetTreasureCategoryImage(targetId));
            }
            else
            {
                itemList.Add(new Item(targetId, objectCount[i], chest.GetTreasureNames(targetId), tempSpawnType));
                player.bag.Add(new Item(targetId, 0, chest.GetTreasureNames(targetId), tempSpawnType));
                player.images.Add(chest.GetTreasureImage(targetId));
            }
        }
		
		for (int i=0; i<numberOfObjects; i++) 
		{
			float h = 0;
			float h2 = 0;
			float w = Screen.width * 0.5f;
			if (numberOfObjects > 4) 
			{
				h += i > 3 ?verticalSpace*mapScale : 0;	
				h2 += i > 3 ?verticalSpace*mapScale:0;	
			}
			positions [i] = new Vector2 (w + (((i % 4) * horizontalSpace*mapScale) - ((i < 4 ? numberOfObjects > 4 ? 4 : numberOfObjects : numberOfObjects - 4) * horizontalSpace*mapScale) * 0.5f), h);
			positionsSmall [i] = new Vector2 (w + (((i % 4) * horizontalSpaceSmall * mapScale) - ((i < 4 ? numberOfObjects > 4 ? 4 : numberOfObjects : numberOfObjects - 4) * horizontalSpaceSmall*mapScale) * 0.5f), h2);
		}

        int[] instructionsSpawnerIndexes = new int[spawners.Length];
        for(int i=0;i<instructionsSpawnerIndexes.Length;i++)
        {
            instructionsSpawnerIndexes[i] = i; ;
        }
        reshuffle(instructionsSpawnerIndexes);

        int itemListIdx=0;
        int itemListSpawned = 0;

        for (int i = 0; i < instructionsSpawnerIndexes.Length; i++)
        {
            if (itemListIdx < itemList.Count)
            {
                spawners[instructionsSpawnerIndexes[i]].Spawn(itemList[itemListIdx]);
                itemListSpawned++;

                if (itemListSpawned >= itemList[itemListIdx].number)
                {
                    itemListSpawned = 0;
                    itemListIdx++;
                }
            }else
            {
                spawners[instructionsSpawnerIndexes[i]].Spawn(null);
            }
		}
		soundMng.PlaySound (0);
		state = "Instructions";
		waitTime = 5.0f;
		overviewCamera.enabled = true;
		playerCamera.enabled = false;
		character.GetComponent<BotControlScript>().enabled=false;
	}

    void reshuffle(string[] texts)
    {
        for (int t = 0; t < texts.Length; t++)
        {
            string tmp = texts[t];
            int r = Random.Range(t, texts.Length);
            texts[t] = texts[r];
            texts[r] = tmp;
        }
    }
    void reshuffle(int[] numbers)
    {
        for (int t = 0; t < numbers.Length; t++)
        {
            int tmp = numbers[t];
            int r = Random.Range(t, numbers.Length);
            numbers[t] = numbers[r];
            numbers[r] = tmp;
        }
    }
	
	void OnCollisionEnter (Collision other)
	{
		Debug.Log ("Collision");
	}
	
	void OnGUI ()
	{
		if (state == "Instructions") 
		{
			if (tutorial) 
			{
				instructionsStyle.normal.textColor=new Color(0.88f,0.88f,0.88f);
				GUI.Label (new Rect (Screen.width/2-498*mapScale, Screen.height*0.1f+2*mapScale, 1000*mapScale, 20*mapScale), language.levelStrings[0], instructionsStyle);
				instructionsStyle.normal.textColor=new Color(0.32f,0.32f,0.32f);
				GUI.Label (new Rect (Screen.width/2-500*mapScale, Screen.height*0.1f, 1000*mapScale, 20*mapScale), language.levelStrings[0], instructionsStyle);
			}
			else
			{
				instructionsStyle.normal.textColor=new Color(0.88f,0.88f,0.88f);
				GUI.Label (new Rect (Screen.width/2-498*mapScale, Screen.height*0.1f+2*mapScale, 1000*mapScale, 20*mapScale), language.levelStrings[1], instructionsStyle);
				instructionsStyle.normal.textColor=new Color(0.32f,0.32f,0.32f);
				GUI.Label (new Rect (Screen.width/2-500*mapScale, Screen.height*0.1f, 1000*mapScale, 20*mapScale), language.levelStrings[1], instructionsStyle);
			}

			if(GUI.Button(new Rect(Screen.width*0.5f-80*mapScale,Screen.height*0.82f-30*mapScale,160*mapScale,60*mapScale),language.levelStrings[25],readyButton))
			{
				if (touchEnabled) 
				{
					state = "None";
					character.transform.position=characterPosition;
					character.SetActive (true);
					playerCamera.enabled = true;
					character.GetComponent<BotControlScript>().enabled=true;
					overviewCamera.enabled = false;
					waitTime = 5.0f;
					//							if(tutorial)
					//								soundMng.PlaySound (2);
				}
				else
				{
					state = "None";
					character.transform.position=characterPosition;
					character.SetActive (true);
					playerCamera.enabled = true;
					character.GetComponent<BotControlScript>().enabled=true;
					overviewCamera.enabled = false;
					waitTime = 5.0f;
					if(tutorial)
						soundMng.PlaySound (2);
				}
			}
				//interactionStyle.normal.textColor=new Color(0.32f,0.32f,0.32f);
				//GUI.Label (new Rect (Screen.width/2-48*mapScale, Screen.height*0.8f+2*mapScale, 100*mapScale, 20*mapScale), "Da CLICK para continuar", interactionStyle);
				//interactionStyle.normal.textColor=new Color(0.88f,0.88f,0.88f);
				//GUI.Label (new Rect (Screen.width/2-50*mapScale, Screen.height*0.8f, 100*mapScale, 20*mapScale), "Da CLICK para continuar", interactionStyle);	
			
			float yOffset=Screen.height*0.3f;
			for (int i=0; i<numberOfObjects; i++) {
				GUI.DrawTexture (new Rect (positions [i].x, positions [i].y+yOffset, 240*mapScale, 130*mapScale), orangeBox);
				GUI.DrawTexture (new Rect (positions [i].x+5*mapScale, positions [i].y+yOffset+4*mapScale, 120*mapScale, 120*mapScale), texturesToDisplay [i]);
				GUI.Label (new Rect (positions [i].x + 110*mapScale, positions [i].y+yOffset+20*mapScale, 150*mapScale, 10*mapScale), objectCount [i].ToString (), quantityStyle);
				if(objectCount[i]==1){
					GUI.Label (new Rect (positions [i].x + 115*mapScale, positions [i].y+60*mapScale+yOffset, 120*mapScale, 80*mapScale), player.bag [i].names[0].ToString(), nameStyle);
				}else{
					GUI.Label (new Rect (positions [i].x + 115*mapScale, positions [i].y+60*mapScale+yOffset, 120*mapScale, 80*mapScale), player.bag [i].names[1].ToString(), nameStyle);
				}
			}
		}
		if (state == "None") 
		{
			if (tutorial) 
			{
				tutorialStyle.normal.textColor=new Color(0.88f,0.88f,0.88f);
				if(touchEnabled)
				{
					switch (tutorialStep) {
					case 0:
						GUI.Label (new Rect (Screen.width/2-498*mapScale, Screen.height*0.1f+2*mapScale, 1000*mapScale, 20*mapScale), language.levelStrings[2], tutorialStyle);
						break;
					case 1:
						GUI.Label (new Rect (Screen.width/2-498*mapScale, Screen.height*0.1f+2*mapScale, 1000*mapScale, 20*mapScale), language.levelStrings[3], tutorialStyle);
						break;
					case 2:
						GUI.Label (new Rect (Screen.width/2-498*mapScale,Screen.height*0.1f+2*mapScale, 1000*mapScale, 20*mapScale), language.levelStrings[4], tutorialStyle);
						break;
					case 3:
						GUI.Label (new Rect (Screen.width/2-498*mapScale, Screen.height*0.1f+2*mapScale, 1000*mapScale, 20*mapScale), language.levelStrings[5], tutorialStyle);
						break;
					case 4:
						GUI.Label (new Rect (Screen.width/2-498*mapScale, Screen.height*0.1f+2*mapScale, 1000*mapScale, 20*mapScale), language.levelStrings[6], tutorialStyle);
						break;
					case 5:
						GUI.Label (new Rect (Screen.width/2-498*mapScale, Screen.height*0.1f+2*mapScale, 1000*mapScale, 20*mapScale), language.levelStrings[7], tutorialStyle);
						break;
					}
				}
				else
				{
					switch (tutorialStep) {
					case 0:
						GUI.Label (new Rect (Screen.width/2-498*mapScale, Screen.height*0.1f+2*mapScale, 1000*mapScale, 20*mapScale), language.levelStrings[8], tutorialStyle);
						break;
					case 1:
						GUI.Label (new Rect (Screen.width/2-498*mapScale, Screen.height*0.1f+2*mapScale, 1000*mapScale, 20*mapScale), language.levelStrings[9], tutorialStyle);
						break;
					case 2:
						GUI.Label (new Rect (Screen.width/2-498*mapScale,Screen.height*0.1f+2*mapScale, 1000*mapScale, 20*mapScale), language.levelStrings[4], tutorialStyle);
						break;
					case 3:
						GUI.Label (new Rect (Screen.width/2-498*mapScale, Screen.height*0.1f+2*mapScale, 1000*mapScale, 20*mapScale), language.levelStrings[5], tutorialStyle);
						break;
					case 4:
						GUI.Label (new Rect (Screen.width/2-498*mapScale, Screen.height*0.1f+2*mapScale, 1000*mapScale, 20*mapScale), language.levelStrings[6], tutorialStyle);
						break;
					case 5:
						GUI.Label (new Rect (Screen.width/2-498*mapScale, Screen.height*0.1f+2*mapScale, 1000*mapScale, 20*mapScale), language.levelStrings[7], tutorialStyle);
						break;
					}
				}
				
				tutorialStyle.normal.textColor=new Color(0.32f,0.32f,0.32f);
				if(touchEnabled)
				{
					switch (tutorialStep) {
					case 0:
						GUI.Label (new Rect (Screen.width/2-500*mapScale, Screen.height*0.1f, 1000*mapScale, 20*mapScale), language.levelStrings[2], tutorialStyle);
						break;
					case 1:
						GUI.Label (new Rect (Screen.width/2-500*mapScale, Screen.height*0.1f, 1000*mapScale, 20*mapScale), language.levelStrings[3], tutorialStyle);
						break;
					case 2:
						GUI.Label (new Rect (Screen.width/2-500*mapScale,Screen.height*0.1f, 1000*mapScale, 20*mapScale), language.levelStrings[4], tutorialStyle);
						break;
					case 3:
						GUI.Label (new Rect (Screen.width/2-500*mapScale, Screen.height*0.1f, 1000*mapScale, 20*mapScale), language.levelStrings[5], tutorialStyle);
						break;
					case 4:
						GUI.Label (new Rect (Screen.width/2-500*mapScale, Screen.height*0.1f, 1000*mapScale, 20*mapScale), language.levelStrings[6], tutorialStyle);
						break;
					case 5:
						GUI.Label (new Rect (Screen.width/2-500*mapScale, Screen.height*0.1f, 1000*mapScale, 20*mapScale), language.levelStrings[7], tutorialStyle);
						break;
					}
				}
				else 
				{
					switch (tutorialStep) {
					case 0:
						GUI.Label (new Rect (Screen.width/2-500*mapScale, Screen.height*0.1f, 1000*mapScale, 20*mapScale), language.levelStrings[8], tutorialStyle);
						break;
					case 1:
						GUI.Label (new Rect (Screen.width/2-500*mapScale, Screen.height*0.1f, 1000*mapScale, 20*mapScale), language.levelStrings[9], tutorialStyle);
						break;
					case 2:
						GUI.Label (new Rect (Screen.width/2-500*mapScale,Screen.height*0.1f, 1000*mapScale, 20*mapScale), language.levelStrings[4], tutorialStyle);
						break;
					case 3:
						GUI.Label (new Rect (Screen.width/2-500*mapScale, Screen.height*0.1f, 1000*mapScale, 20*mapScale), language.levelStrings[10], tutorialStyle);
						break;
					case 4:
						GUI.Label (new Rect (Screen.width/2-500*mapScale, Screen.height*0.1f, 1000*mapScale, 20*mapScale), language.levelStrings[6], tutorialStyle);
						break;
					case 5:
						GUI.Label (new Rect (Screen.width/2-500*mapScale, Screen.height*0.1f, 1000*mapScale, 20*mapScale), language.levelStrings[7], tutorialStyle);
						break;
					}
				}
				
			}
			if(touchEnabled)
			{
				joystick.SetActive(true);
				Rect grabRect = new Rect(Screen.width*0.8f-86*mapScale,Screen.height*0.75f-86*mapScale,172*mapScale,172*mapScale);
				for(int i = 0; i < Input.touchCount; i++)
				{
					if(Input.GetTouch(i).phase == TouchPhase.Began&&grabRect.Contains(new Vector2(Input.GetTouch(i).position.x,Screen.height-Input.GetTouch(i).position.y)))
					{
						grab = true;
						player.SendMessage("Grab");
					}
				}
				GUI.DrawTexture(grabRect,grabButtonStyle);
			}
		}else{
			joystick.SetActive(false);
		}
		if (state == "CorrectionPhase") {
			for (int i=0; i<player.bag.Count; i++) {
				if (i < numberOfObjects) {
					GUI.DrawTexture (new Rect (positionsSmall [i].x, positionsSmall [i].y+yOffsetSmall, 120*mapScale, 65*mapScale), greenBox);
					GUI.DrawTexture (new Rect (positionsSmall [i].x+2*mapScale, positionsSmall [i].y+yOffsetSmall+1*mapScale, 60*mapScale, 60*mapScale), texturesToDisplay [i]);
					GUI.Label (new Rect (positionsSmall [i].x + 60*mapScale, positionsSmall [i].y +yOffsetSmall, 60*mapScale, 65*mapScale), objectCount [i].ToString (), smallStyle);
				}
				if (differences [i] < 0)
					GUI.DrawTexture (new Rect (positionsBag [i].x - 9*mapScale, positionsBag [i].y +yOffsetCP-9*mapScale, 243*mapScale, 134*mapScale), red);
				if (differences [i] > 0 && objectsToRemove <= 0)
					GUI.DrawTexture (new Rect (positionsBag [i].x - 9*mapScale, positionsBag [i].y +yOffsetCP-9*mapScale, 243*mapScale, 134*mapScale), blue);
				
				GUI.DrawTexture (new Rect (positionsBag [i].x, positionsBag [i].y+yOffsetCP, 225*mapScale, 115*mapScale), orangeBox);
				GUI.DrawTexture (new Rect (positionsBag [i].x+6*mapScale, positionsBag [i].y+yOffsetCP+6*mapScale, 100*mapScale, 100*mapScale), texturesToDisplay [i]);
				GUI.Label (new Rect (positionsBag [i].x + 108*mapScale, positionsBag [i].y+yOffsetCP+18*mapScale, 110*mapScale, 10*mapScale), player.bag [i].number.ToString (), quantityStyle);
				if(player.bag [i].number==1){
					GUI.Label (new Rect (positionsBag [i].x + 108*mapScale, positionsBag [i].y+60*mapScale+yOffsetCP, 110*mapScale, 55*mapScale), player.bag [i].names[0].ToString(), nameStyle);
				}else{
					GUI.Label (new Rect (positionsBag [i].x + 108*mapScale, positionsBag [i].y+60*mapScale+yOffsetCP, 110*mapScale, 55*mapScale), player.bag [i].names[1].ToString(), nameStyle);
				}
				
				//GUI.DrawTexture (new Rect (positionsBag [i].x, positionsBag [i].y, 100, 100), texturesToDisplay [i]);
				//GUI.Label (new Rect (positionsBag [i].x + 120, positionsBag [i].y + 40, 100, 20), player.bag [i].number.ToString (), quantityStyle);
				//GUI.Label (new Rect (positionsBag [i].x + 120, positionsBag [i].y + 40, 100, 20), player.bag [i].names[0].ToString(), nameStyle);
			}
			if (tutorial) {
				if (objectsToRemove > 0) {
					//GUI.Label (new Rect (Screen.width/2-500*mapScale, Screen.height*0.1f, 1000*mapScale, 20*mapScale), "Busca estos objetos en la playa", instructionsStyle);
					instructionsStyle.normal.textColor=new Color(0.88f,0.88f,0.88f);
					GUI.Label (new Rect (Screen.width/2-498*mapScale, Screen.height*0.07f+2*mapScale, 1000*mapScale, 150*mapScale), language.levelStrings[11], instructionsStyle);
					instructionsStyle.normal.textColor=new Color(0.32f,0.32f,0.32f);
					GUI.Label (new Rect (Screen.width/2-500*mapScale, Screen.height*0.07f, 1000*mapScale, 150*mapScale), language.levelStrings[11], instructionsStyle);
				} else if (objectsMissing) {
					instructionsStyle.normal.textColor=new Color(0.88f,0.88f,0.88f);
					GUI.Label (new Rect (Screen.width/2-548*mapScale, Screen.height*0.07f+2*mapScale, 1100*mapScale, 150*mapScale), language.levelStrings[12], instructionsStyle);
					instructionsStyle.normal.textColor=new Color(0.32f,0.32f,0.32f);
					GUI.Label (new Rect (Screen.width/2-550*mapScale, Screen.height*0.07f, 1100*mapScale, 150*mapScale), language.levelStrings[12], instructionsStyle);
					//GUI.Label (new Rect (positionsBag [0].x, Screen.height / 2 - 230, 100, 20), "y compara cuantos tienes y cuantos te pidieron", instructionsStyle);
					if(GUI.Button(new Rect(Screen.width*0.5f-80*mapScale,Screen.height*0.85f-30*mapScale,160*mapScale,60*mapScale),language.levelStrings[25],readyButton))
					{
						state = "None";
						character.transform.position=characterPosition;
						character.SetActive (true);
						playerCamera.enabled = true;
						character.GetComponent<BotControlScript>().enabled=true;
						overviewCamera.enabled = false;
						tutorial=false;
					}
					//interactionStyle.normal.textColor=new Color(0.32f,0.32f,0.32f);
					//GUI.Label (new Rect (Screen.width/2-48*mapScale, Screen.height*0.8f+2*mapScale, 100*mapScale, 20*mapScale), "Da CLICK para continuar", interactionStyle);
					//interactionStyle.normal.textColor=new Color(0.88f,0.88f,0.88f);
					//GUI.Label (new Rect (Screen.width/2-50*mapScale, Screen.height*0.8f, 100*mapScale, 20*mapScale), "Da CLICK para continuar", interactionStyle);
				}
				
			} else {
				if (objectsToRemove > 0){
					instructionsStyle.normal.textColor=new Color(0.88f,0.88f,0.88f);
					GUI.Label (new Rect (Screen.width/2-548*mapScale, Screen.height*0.07f+2*mapScale, 1100*mapScale, 150*mapScale), language.levelStrings[11], instructionsStyle);
					instructionsStyle.normal.textColor=new Color(0.32f,0.32f,0.32f);
					GUI.Label (new Rect (Screen.width/2-550*mapScale, Screen.height*0.07f, 1100*mapScale, 150*mapScale), language.levelStrings[11], instructionsStyle);
				}else if (objectsMissing){
					instructionsStyle.normal.textColor=new Color(0.88f,0.88f,0.88f);
					GUI.Label (new Rect (Screen.width/2-548*mapScale, Screen.height*0.07f+2*mapScale, 1100*mapScale, 150*mapScale), language.levelStrings[12], instructionsStyle);
					instructionsStyle.normal.textColor=new Color(0.32f,0.32f,0.32f);
					GUI.Label (new Rect (Screen.width/2-550*mapScale, Screen.height*0.07f, 1100*mapScale, 150*mapScale), language.levelStrings[12], instructionsStyle);
					if(GUI.Button(new Rect(Screen.width*0.5f-80*mapScale,Screen.height*0.85f-30*mapScale,160*mapScale,60*mapScale),language.levelStrings[25],readyButton))
					{
						state = "None";
						character.transform.position=characterPosition;
						character.SetActive (true);
						playerCamera.enabled = true;
						character.GetComponent<BotControlScript>().enabled=true;
						overviewCamera.enabled = false;
						tutorial=false;
					}
					//interactionStyle.normal.textColor=new Color(0.32f,0.32f,0.32f);
					//GUI.Label (new Rect (Screen.width/2-48*mapScale, Screen.height*0.8f+2*mapScale, 100*mapScale, 20*mapScale), "Da CLICK para continuar", interactionStyle);
					//interactionStyle.normal.textColor=new Color(0.88f,0.88f,0.88f);
					//GUI.Label (new Rect (Screen.width/2-50*mapScale, Screen.height*0.8f, 100*mapScale, 20*mapScale), "Da CLICK para continuar", interactionStyle);
				}
			}
			headerSmallStyle.normal.textColor=new Color(0.88f,0.88f,0.88f);
			GUI.Label (new Rect (Screen.width*0.5f-98*mapScale,yOffsetSmall-48*mapScale, 200*mapScale, 20*mapScale), language.levelStrings[29], headerSmallStyle);
			GUI.Label (new Rect (Screen.width*0.5f-98*mapScale, yOffsetCP-48*mapScale, 200*mapScale, 20*mapScale), language.levelStrings[30], headerSmallStyle);
			headerSmallStyle.normal.textColor=new Color(0.32f,0.32f,0.32f);
			GUI.Label (new Rect (Screen.width*0.5f-100*mapScale,yOffsetSmall-50*mapScale, 200*mapScale, 20*mapScale), language.levelStrings[29], headerSmallStyle);
			GUI.Label (new Rect (Screen.width*0.5f-100*mapScale, yOffsetCP-50*mapScale, 200*mapScale, 20*mapScale), language.levelStrings[30], headerSmallStyle);
			
			GUI.Label (new Rect (10, 10, 100, 20), complete.ToString ());
		}
		if (state == "CheckComplete") {
			float yOffset=100;
			GUI.DrawTexture(new Rect(Screen.width*0.5f-500*mapScale,Screen.height*0.5f-200*mapScale-yOffset*mapScale,1000*mapScale,400*mapScale),textBG);
			instructionsStyle.normal.textColor=new Color(0.32f,0.32f,0.32f);
			GUI.Label (new Rect (Screen.width / 2 - 498*mapScale, Screen.height / 2 - 98*mapScale-yOffset*mapScale, 1000*mapScale, 200*mapScale), language.levelStrings[13], instructionsStyle);
			instructionsStyle.normal.textColor=new Color(0.88f,0.88f,0.88f);
			GUI.Label (new Rect (Screen.width / 2 - 500*mapScale, Screen.height / 2 - 100*mapScale-yOffset*mapScale, 1000*mapScale, 200*mapScale), language.levelStrings[13], instructionsStyle);
			if (GUI.Button (new Rect (Screen.width *0.5f+290*mapScale, Screen.height * 0.5f+170*mapScale-yOffset*mapScale, 100 * mapScale, 100 * mapScale), "", noStyle)) {
				tutorial=false;
				reviewObjects=false;
				state = "CheckComplete2";
				if(tryAgain)
					notSureCorrect++;
				else{
					notSureIncorrect++;
				}
			}
			if (GUI.Button (new Rect (Screen.width *0.5f+400*mapScale, Screen.height * 0.5f+170*mapScale-yOffset*mapScale, 100 * mapScale, 100 * mapScale), "", yesStyle)) {
				tutorial=false;
				if(tryAgain)
					notSureIncorrect++;
				else{
					notSureCorrect++;
				}
				SaveLevelProgress();
			}
		}
		if (state == "CheckComplete2") {
			float yOffset=100;
			GUI.DrawTexture(new Rect(Screen.width*0.5f-500*mapScale,Screen.height*0.5f-200*mapScale-yOffset*mapScale,1000*mapScale,400*mapScale),textBG);
			instructionsStyle.normal.textColor=new Color(0.32f,0.32f,0.32f);
			GUI.Label (new Rect (Screen.width / 2 - 498*mapScale, Screen.height / 2 - 99*mapScale-yOffset*mapScale, 1000*mapScale, 200*mapScale), language.levelStrings[14], instructionsStyle);
			instructionsStyle.normal.textColor=new Color(0.88f,0.88f,0.88f);
			GUI.Label (new Rect (Screen.width / 2 - 500*mapScale, Screen.height / 2 - 100*mapScale-yOffset*mapScale, 1000*mapScale, 200*mapScale), language.levelStrings[14], instructionsStyle);
			if (GUI.Button (new Rect (Screen.width *0.5f+290*mapScale, Screen.height * 0.5f+170*mapScale-yOffset*mapScale, 100 * mapScale, 100 * mapScale), "", noStyle)) {
				tutorial=false;
				state = "None";
				character.transform.position=characterPosition;
				character.SetActive (true);
				playerCamera.enabled = true;
				character.GetComponent<BotControlScript>().enabled=true;
				overviewCamera.enabled = false;
				waitTime = 5.0f;
			}
			if (GUI.Button (new Rect (Screen.width *0.5f+400*mapScale, Screen.height * 0.5f+170*mapScale-yOffset*mapScale, 100 * mapScale, 100 * mapScale), "", yesStyle)) {
				tutorial=false;
				reviewObjects=true;
				SaveLevelProgress();
			}
		}
		if (state == "Complete") {
			float yOffset=100;
			GUI.DrawTexture(new Rect(Screen.width*0.5f-500*mapScale,Screen.height*0.5f-200*mapScale-yOffset*mapScale,1000*mapScale,400*mapScale),textBG);
			instructionsStyle.normal.textColor=new Color(0.32f,0.32f,0.32f);
			if (tryAgain)
				GUI.Label (new Rect (Screen.width / 2 - 498*mapScale, Screen.height / 2 - 98*mapScale-yOffset*mapScale, 1000*mapScale, 200*mapScale), language.levelStrings[15], instructionsStyle);
			else
			{
				if(reviewObjects)
					GUI.Label (new Rect (Screen.width / 2 - 498*mapScale, Screen.height / 2 - 99*mapScale-yOffset*mapScale, 1000*mapScale, 200*mapScale), language.levelStrings[16], instructionsStyle);
				else
					GUI.Label (new Rect (Screen.width / 2 - 498*mapScale, Screen.height / 2 - 98*mapScale-yOffset*mapScale, 1000*mapScale, 200*mapScale), language.levelStrings[17], instructionsStyle);
			}
			instructionsStyle.normal.textColor=new Color(0.88f,0.88f,0.88f);
			if (tryAgain)
				GUI.Label (new Rect (Screen.width / 2 - 500*mapScale, Screen.height / 2 - 100*mapScale-yOffset*mapScale, 1000*mapScale, 200*mapScale), language.levelStrings[15], instructionsStyle);
			else
			{
				if(reviewObjects)
					GUI.Label (new Rect (Screen.width / 2 - 500*mapScale, Screen.height / 2 - 100*mapScale-yOffset*mapScale, 1000*mapScale, 200*mapScale), language.levelStrings[16], instructionsStyle);
				else
					GUI.Label (new Rect (Screen.width / 2 - 500*mapScale, Screen.height / 2 - 100*mapScale-yOffset*mapScale, 1000*mapScale, 200*mapScale), language.levelStrings[17], instructionsStyle);
			}
		}
		if (gameWon) {
			GUI.Label (new Rect (Screen.width / 2 - 100, Screen.height / 2 - 20, 100, 20), language.levelStrings[31], instructionsStyle);
		}
		
		if (state == "CompletedActivity") {
			/*float yOffset = 100;
			GUI.DrawTexture (new Rect (Screen.width * 0.5f - 500 * mapScale, Screen.height * 0.5f - 200 * mapScale - yOffset * mapScale, 1000 * mapScale, 400 * mapScale), textBG);
			instructionsStyle.normal.textColor = new Color (0.32f, 0.32f, 0.32f);
			GUI.Label (new Rect (Screen.width / 2 - 398 * mapScale, Screen.height / 2 - 98 * mapScale - yOffset * mapScale, 800 * mapScale, 200 * mapScale), "Muchas gracias por ayudarme! Volvamos a la isla", instructionsStyle);
			instructionsStyle.normal.textColor = new Color (0.88f, 0.88f, 0.88f);
			GUI.Label (new Rect (Screen.width / 2 - 400 * mapScale, Screen.height / 2 - 100 * mapScale - yOffset * mapScale, 800 * mapScale, 200 * mapScale), "Muchas gracias por ayudarme! Volvamos a la isla", instructionsStyle);*/
			
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
				GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 150 * mapScale, 800 * mapScale, 100 * mapScale), language.levelStrings[18],scoreStyle1);
			else
				GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 150 * mapScale, 800 * mapScale, 100 * mapScale), language.levelStrings[19],scoreStyle1);
			if(kiwis==0)
				GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 90 * mapScale, 800 * mapScale, 100 * mapScale), language.levelStrings[20],scoreStyle2);
			else{
				if(kiwis>1)
					GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 90 * mapScale, 800 * mapScale, 100 * mapScale), language.levelStrings[21]+" "+kiwis.ToString()+" "+language.levelStrings[22],scoreStyle2);
				else
					GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 90 * mapScale, 800 * mapScale, 100 * mapScale), language.levelStrings[21]+" "+kiwis.ToString()+" "+language.levelStrings[23],scoreStyle2);
			}
			if(GUI.Button(new Rect(Screen.width*0.5f-80*mapScale,Screen.height*0.5f+110*mapScale,160*mapScale,60*mapScale),language.levelStrings[25],kiwiButton))
			{
				returnToIsland=true;
			}
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
		
		if(state!="Pause"&&state!="CompletedActivity"){
			if(GUI.Button(new Rect(10*mapScale,10*mapScale,71*mapScale,62*mapScale),"",pauseButton))
			{
				Time.timeScale=0.0f;
				stateBeforePause=state;
				state="Pause";
			}
		}
		if (state == "Pause") {
			float pauseY=Screen.height*0.5f-177*mapScale;
			GUI.DrawTexture(new Rect(0,pauseY,Screen.width,354*mapScale),pauseBackground);
			GUI.Label(new Rect(Screen.width*0.5f-100*mapScale,pauseY-40*mapScale,200*mapScale,60*mapScale),language.levelStrings[24],pauseText);
			if(GUI.Button(new Rect(Screen.width*0.5f-200*mapScale,pauseY+50*mapScale,366*mapScale,66*mapScale),"",pauseContinue))
			{
				Time.timeScale=1.0f;
				state=stateBeforePause;
			}else
				if(GUI.Button(new Rect(Screen.width*0.5f-200*mapScale,pauseY+140*mapScale,382*mapScale,66*mapScale),"",pauseIsland))
			{
				if(!returnToIsland){
					SaveProgress(false);
					returnToIsland=true;
					Time.timeScale=1.0f;
				}
				//Application.LoadLevel("Archipielago");
			}
			GUI.Label(new Rect(Screen.width*0.5f-110*mapScale,pauseY+50*mapScale,366*mapScale,66*mapScale),language.levelStrings[26],pauseButtons);
			GUI.Label(new Rect(Screen.width*0.5f-110*mapScale,pauseY+140*mapScale,382*mapScale,66*mapScale),language.levelStrings[27],pauseButtons);
			/*else
				if(GUI.Button(new Rect(Screen.width*0.5f-200*mapScale,pauseY+230*mapScale,162*mapScale,67*mapScale),"",pauseExit))
			{
				Time.timeScale=1.0f;
				Application.Quit();
			}*/
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
	[System.Serializable]
	public class Item
	{
		public string id;
		public int number;
		public string[] names;
        public string type;
		
		public Item (string objectID, int quantity,string[]name,string type)
		{
			id = objectID;
			number = quantity;
			names=name;
            this.type = type;
		}
	}
    public class SimpleItem
    {
        public string id;
        public string category;

        public SimpleItem(string id, string category)
        {
            this.id = id;
            this.category = category;
        }
    }
}
