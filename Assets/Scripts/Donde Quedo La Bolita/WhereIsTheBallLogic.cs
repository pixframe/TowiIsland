using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(GameSaveLoad))]



public class WhereIsTheBallLogic : MonoBehaviour
{
	//script acces variables
	public GameSaveLoad loader;
	public WhereIsTheBallConfiguration configuration;
	public SelectionScript selectScript;
	public Tutorials tutScript;
	ProgressHandler saveHandler;
	SoundManager soundMng;
	AudioSource audioSrc;
	public ScoreEffects scoreE;
	Scores scoreScript;


	
	//Game variables
	public int numberOfMonkeys = 0;
	public int numberOfObjects = 0;
	
	public GameObject[] monkeys;
	int numOfMovments = 0;
	int progNumOfMov = 0;
	float timeForMovement = 20f;
	int prevPos;
	public List<GameObject> objectsUsed = new List<GameObject>();
	public float waitTime = 5f;
	float waitTimer;
	int mon1;
	int mon2;
	int moveType;
	GameObject mon1Obj;
	GameObject mon2Obj;
	Vector3 monkey1Pos;
	Vector3 monkey2Pos;
	public float speed;
	public float speed1 = .2f;
	public float speed2 = .068f;
	public float distToChange = .1f;
	int objNumFind;
	int correctObj;
	public bool tutorial = false;
	public bool moveToPos = false;
	public bool plat = false;
	public List<GameObject> plats = new List<GameObject>();
	public List<GameObject> monPlats = new List<GameObject>();
	int platCount;
	bool showObj = false;
	string startState;
	string finishState;
	bool objAssigned = false;
	string ballName;
	public List<int> mon1num, mon2num, moveTypeNum = new List<int>();
	int retryNumCount;
	public int numLevelsPerBlock = 5;
	bool play = false;
	bool returnToIsland=false;
	public GUIStyle kiwiButton;
	public Texture loadingScreen;
	public Texture en_loadingScreen;
	public Texture loadingColor;
	bool fadeIn=false;
	bool fadeOut=false;
	float opacity=0;
	public float pickTime = 5;
	public float pickTimer;

	
	
	//level variables
	public int level;
	public int subLevel;
	int todayOffset;
	public bool objectAssigned = false;
	public List<int> objectPos = new List<int>();
	public GameObject[] objectsStored;
	public List<GameObject> monkeysActive = new  List<GameObject>();
	bool setLevel = false;
	public bool[] monInPos;
	bool correct;
	int tries = 0;
	int posCount = 0;
	GameObject monkWithObject;
	float timeOfLevel;
	float totalTime;
	int levelsPlayed;
	int levelsRepeated;
	int levelsPassed;

	//gui vars
	Rect AdvanceBut;
	float screenScale=1;
	public GUIStyle pauseButton;
	public Texture2D pauseBackground;
	public Texture2D tutBackground;
	public GUIStyle pauseContinue;
	public GUIStyle pauseIsland;
	public GUIStyle pauseExit;
	public GUIStyle pauseText;
	public GUIStyle advanceStyle;
	public GUIStyle tutStyle;


	
	
	//logic variables
	public string state;
	public string showState;
	public string chooseState;
	public string insState;
	public string moveState;
	public int moveStage;
	string stateBeforePause;
	bool monPicked = false;
	bool cor = true;
	float hideTimer;
	float hideTime = 2f;


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
	float endTime = 5f;
	float resultTimer;
	float resultTime = 3f;
	
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
		levelsPlayed = 0;
		kiwiAnimCurrTime = kiwiAnimTime;
		loader = GetComponent<GameSaveLoad>();
		selectScript = GetComponent<SelectionScript>();
		tutScript = GameObject.Find("Tutorial").GetComponent<Tutorials>();
		saveHandler = GetComponent<ProgressHandler>();
		soundMng = GetComponent<SoundManager>();
		audioSrc = GetComponent<AudioSource>();
		screenScale = (float)Screen.height / (float)768;
		scoreStyle1.fontSize = (int)(scoreStyle1.fontSize * screenScale);
		scoreStyle2.fontSize = (int)(scoreStyle2.fontSize * screenScale);
		scoreStyle3.fontSize = (int)(scoreStyle3.fontSize * screenScale);
		kiwiButton.fontSize = (int)(kiwiButton.fontSize * screenScale);
		pauseButtons.fontSize = (int)(pauseButtons.fontSize * screenScale);
		level = sessionMng.activeKid.monkeyDifficulty;
		subLevel = sessionMng.activeKid.monkeyLevel;
		
		state = "Start";
		
		
		// loads game information from the xml file 
		loader.Load(GameSaveLoad.game.whereIsTheBall);
		configuration = (WhereIsTheBallConfiguration)loader.configuration; 

		if(configuration.music==0)
		{
			GameObject tempCamera = GameObject.Find("Main Camera");
			AudioSource[] tempAudios = tempCamera.GetComponents<AudioSource>();
			tempAudios[0].volume=0;
			tempAudios[0].Stop();
		}else
		{
			GameObject tempCamera = GameObject.Find("Main Camera");
			AudioSource[] tempAudios = tempCamera.GetComponents<AudioSource>();
			tempAudios[0].Play();
		}
		if(configuration.sound==0)
		{
			GameObject tempCamera = GameObject.Find("Main Camera");
			AudioSource[] tempAudios = tempCamera.GetComponents<AudioSource>();
			tempAudios[1].volume=0;
			tempAudios[1].Stop();
			GetComponent<AudioSource>().volume=0;
			GetComponent<AudioSource>().Stop();
		}else
		{
			GameObject tempCamera = GameObject.Find("Main Camera");
			AudioSource[] tempAudios = tempCamera.GetComponents<AudioSource>();
			tempAudios[1].Play();
		}
		
		prevPos = 100;
		
		startState = "SetLevel";
		
		waitTimer = waitTime;
		tutorial = true;
		


		posCount = 0;
		
		Application.targetFrameRate = 60;
		platCount = 0;
		monPicked = false;
		hideTimer = hideTime;

		todayOffset = sessionMng.activeKid.playedMonkey;
		resultTimer = resultTime;
		pickTimer = pickTime;
	}


	
	// Update is called once per frame
	void Update ()
	{
		if (returnToIsland && !saveHandler.saving) 
		{
			fadeIn=true;
			//Application.LoadLevel("Archipielago");
		}
		timeOfLevel += Time.deltaTime;
		totalTime += Time.deltaTime;
		switch(state){

		case "Start"://switch que controla la logica principal del juego
			switch(startState){
			case "SetLevel"://genera la dificultad del nivel y ocurre al inicio de cada nivel
				if(!setLevel)
				{
					
					foreach(GameObject obj in objectsUsed)
					{
						obj.SetActive(false);
					}
					
					SetLevel();
					startState = "PlatAppear";
				}
				break;
			case "PlatAppear": //prepara a los changos a moverse a su posicion
				moveToPos = true; 
				plat = false;
				waitTimer = waitTime;
				startState = "MoveToPos";

				break;
			case "MoveToPos"://Mueve a los changos a su posicion y checa que lleguen 
				if(moveToPos)
				{
					posCount = 0;
					foreach(GameObject mon in monkeysActive)
					{
						MoveToPosition mov = mon.GetComponent<MoveToPosition>();
						if(mov.move)
						mov.MoveToPosFunc();
						if(mov.inPos)
						{
							posCount++;
						}
					}
					if(posCount == monkeysActive.Count)
					{
						foreach(GameObject obj in monPlats)
						{
							obj.SetActive(true);
						}
						
						foreach(GameObject obj in plats)
						{
							obj.SetActive(false);
						}
						
						moveToPos = false;
						showObj = true;
						startState = "PlatsElevate";
					}
				}

				break;
			case "PlatsElevate"://eleva a los changos 
				foreach(GameObject obj in monkeysActive)
				{
					if(SetPlat(obj, "Normal", 2f, 3f))
					{
						platCount++;
					}
				}
				if(platCount == plats.Count && platCount != 0)
				{
					play = false;
					state = "ShowObjects";
					startState = "SetLevel";
					break;
				}
				platCount = 0;
				break;
			}
			break;
		case "ShowObjects"://enseña y esconde los objetos
			plat = true;
			switch(showState){
			
			case "Show"://enseña el objeto
				foreach(GameObject obj in objectsUsed)
				{
					//play animation to show the objects
					obj.SetActive(true);

					obj.transform.Find("TutArrow").gameObject.SetActive(true);
				}
				if(!tutorial)
				{
					waitTimer -= Time.deltaTime;
					if(waitTimer <= 0)
					{
						
						showState = "HideObject";
						
					}	

				}

				
				break;
			case "HideObject"://esconde el objeto
				waitTimer = waitTime;
				foreach(GameObject obj in objectsUsed)
				{
					//play animation to hide the object
					obj.SetActive(false);
				}
				showState = "WaitForMovements";
				break;
			case "WaitForMovements"://espera a que los movinientos esperen

				if(!tutorial)
				{
					//add a counter for players to be ready for the movement
					if(tutScript.Timer(1f))
					{
						state = "Move";
						moveState = "PickMonkeys";
					}	
				}
				break;
			}
			//objects are shown to the player and hid

			break;
		case "Move"://determina los movimientos aleatorios de los changos dependiendo de su posicion y la posicion del chango con el que hara el cambio 
			switch(moveState){
			case "PickMonkeys"://escoge dos changos activos aleatoriamente
				if(tries == 0)
				{
					mon1 =  Random.Range(0, monkeysActive.Count);
				
					
					mon2 = Random.Range(0, monkeysActive.Count);
					
					while(mon2 == mon1)
					{
						mon2 = Random.Range(0, monkeysActive.Count);				
					}
					
					mon1Obj = monkeys[mon1];
					mon2Obj = monkeys[mon2];
					monkey1Pos = mon1Obj.transform.position;
					monkey2Pos = mon2Obj.transform.position;
					moveState = "DetermineMovement";
					mon1num.Add(mon1);
					mon2num.Add(mon2);
				}
				else//usa los mismos changos si el nivel se repite
				{
					mon1 = mon1num[retryNumCount];
					mon2 = mon2num[retryNumCount];
					mon1Obj = monkeys[mon1];
					mon2Obj = monkeys[mon2];
					monkey1Pos = mon1Obj.transform.position;
					monkey2Pos = mon2Obj.transform.position;
					moveState = "DetermineMovement";
				}

				break;
			case "DetermineMovement"://determina el tipo de movimiento dependiendo de la distacia al otro chango 
				if(tries == 0)// si es un nuevo nivel 
				{
					float distance = Vector3.Distance(monkey1Pos, monkey2Pos);
					if(distance < 3f)//si la distancia es menor a tres sacara un numero entre cero y tres que son los tipos de movimietos que hay para dos changos que se encuentran un a lado del otro
					{
						moveType = Random.Range(0, 4);
						speed1 = (Vector3.Distance(monkey1Pos, monkey2Pos) + 2) / speed;
						speed2 =(Vector3.Distance(monkey1Pos, monkey2Pos) / speed);
						
					}
					else if(distance > 3f)//si la distacia es mayor a 3 sacara un numero aleatorio entre 4 y 6  que son pertinentes a los movimientos entre dos changos separados por uno o mas changos
					{
						moveType = Random.Range(4, 6);
						speed1 = (Vector3.Distance(monkey1Pos, monkey2Pos) + 2) / speed;
						speed2 =(Vector3.Distance(monkey1Pos, monkey2Pos) / speed);
						
					}
					moveTypeNum.Add(moveType);
					moveStage = 1;
					moveState = "Movement";
				}
				else//repite la secuencia de movimientos anterior al repetir el nivel 
				{
					float distance = Vector3.Distance(monkey1Pos, monkey2Pos);
					if(distance < 3f)
					{
						speed1 = (Vector3.Distance(monkey1Pos, monkey2Pos) + 2) / speed;
						speed2 =(Vector3.Distance(monkey1Pos, monkey2Pos) / speed);
					}
					else if(distance > 3f)
					{
						speed1 = (Vector3.Distance(monkey1Pos, monkey2Pos) + 2) / speed;
						speed2 =(Vector3.Distance(monkey1Pos, monkey2Pos) / speed);
					}
					moveType = moveTypeNum[retryNumCount];
					retryNumCount++;
					moveStage = 1;
					moveState = "Movement";
				}

				
				
				break;
			case "Movement"://realiza los movimientos pertinentes
				
				if(numOfMovments <= 0)
				{

					state = "Choose";
					chooseState = "Instructions";
					
				}
				else
				{
#region	movement		
					
					switch(moveType){
					// from 0 to 3 are movements for monkeys that are next to each other
					case 0://mon1 front, mon2 sideways
						
						switch(moveStage){
						case 1:
							Vector3 firstPos = monkey1Pos + Vector3.back;
							mon1Obj.transform.Translate(Vector3.back * speed1 * Time.deltaTime);
							mon2Obj.transform.Translate((monkey1Pos - mon2Obj.transform.position).normalized * speed2 * Time.deltaTime);

							if(CheckDistance(monkey1Pos, firstPos, mon1Obj.transform.position))
							{
								moveStage = 2;
							}
							break;
						case 2:
							Vector3 secondPos = monkey2Pos + Vector3.back;
							mon1Obj.transform.Translate((secondPos - mon1Obj.transform.position).normalized * speed1 * Time.deltaTime);
							mon2Obj.transform.Translate((monkey1Pos - mon2Obj.transform.position).normalized * speed2 * Time.deltaTime);

							if(CheckDistance(monkey1Pos + Vector3.back, secondPos, mon1Obj.transform.position))
							{
								moveStage = 3;
							}
							break;
						case 3:
							mon1Obj.transform.Translate((monkey2Pos - mon1Obj.transform.position).normalized * speed1 * Time.deltaTime);
							mon2Obj.transform.Translate((monkey1Pos - mon2Obj.transform.position).normalized * speed2 * Time.deltaTime);
							if(CheckDistance(monkey2Pos + Vector3.back, monkey2Pos, mon1Obj.transform.position))
							{
								moveState = "Stop";
							}
							break;
						}
						break;
					case 1://mon1 back, mon2 sideways
						switch(moveStage){
						case 1:
		
							Vector3 firstPos = monkey1Pos + Vector3.forward;
							mon1Obj.transform.Translate(Vector3.forward * speed1 * Time.deltaTime);
							mon2Obj.transform.Translate((monkey1Pos - mon2Obj.transform.position).normalized * speed2 * Time.deltaTime);
							if(CheckDistance(monkey1Pos, firstPos, mon1Obj.transform.position))
							{
								moveStage = 2;
							}
							break;
						case 2:
	
							Vector3 secondPos = monkey2Pos + Vector3.forward;
							mon1Obj.transform.Translate((secondPos - mon1Obj.transform.position).normalized * speed1 * Time.deltaTime);
							mon2Obj.transform.Translate((monkey1Pos - mon2Obj.transform.position).normalized * speed2 * Time.deltaTime);
							if(CheckDistance(monkey1Pos + Vector3.forward, secondPos, mon1Obj.transform.position))
							{
								moveStage = 3;
							}
							break;
						case 3:
							mon1Obj.transform.Translate((monkey2Pos - mon1Obj.transform.position).normalized * speed1 * Time.deltaTime);
							mon2Obj.transform.Translate((monkey1Pos - mon2Obj.transform.position).normalized * speed2 * Time.deltaTime);
							if(CheckDistance(monkey2Pos + Vector3.forward, monkey2Pos, mon1Obj.transform.position))
							{
								moveState = "Stop";
							}
							break;
						}
						break;
					case 2://mon1 sideways, mon2 front
						switch(moveStage){
						case 1:
		
							Vector3 firstPos = monkey2Pos + Vector3.back;
							mon2Obj.transform.Translate(Vector3.back * speed1 * Time.deltaTime);
							mon1Obj.transform.Translate((monkey2Pos - mon1Obj.transform.position).normalized * speed2 * Time.deltaTime);
							if(CheckDistance(monkey2Pos, firstPos, mon2Obj.transform.position))
							{
								moveStage = 2;
							}
							break;
						case 2:
	
							Vector3 secondPos = monkey1Pos + Vector3.back;
							mon2Obj.transform.Translate((secondPos - mon2Obj.transform.position).normalized * speed1 * Time.deltaTime);
							mon1Obj.transform.Translate((monkey2Pos - mon1Obj.transform.position).normalized * speed2 * Time.deltaTime);
							if(CheckDistance(monkey2Pos + Vector3.back, secondPos, mon2Obj.transform.position))
							{
								moveStage = 3;
							}
							break;
						case 3:
							mon2Obj.transform.Translate((monkey1Pos - mon2Obj.transform.position).normalized * speed1 * Time.deltaTime);
							mon1Obj.transform.Translate((monkey2Pos - mon1Obj.transform.position).normalized * speed2 * Time.deltaTime);
							if(CheckDistance(monkey1Pos + Vector3.back, monkey1Pos, mon2Obj.transform.position))
							{
								moveState = "Stop";
							}
							break;
						}
						break;
					case 3://mon1 sideways, mon2 back
						switch(moveStage){
						case 1:
		
							Vector3 firstPos = monkey2Pos + Vector3.forward;
							mon2Obj.transform.Translate(Vector3.forward * speed1 * Time.deltaTime);
							mon1Obj.transform.Translate((monkey2Pos - mon1Obj.transform.position).normalized * speed2 * Time.deltaTime);
							if(CheckDistance(monkey2Pos, firstPos, mon2Obj.transform.position))
							{
								moveStage = 2;
							}
							break;
						case 2:
							Vector3 secondPos = monkey1Pos + Vector3.forward;										
							mon2Obj.transform.Translate((secondPos - mon2Obj.transform.position).normalized * speed1 * Time.deltaTime);
							mon1Obj.transform.Translate((monkey2Pos - mon1Obj.transform.position).normalized * speed2 * Time.deltaTime);
							if(CheckDistance(monkey2Pos + Vector3.forward, secondPos, mon2Obj.transform.position))
							{
								moveStage = 3;
							}
							break;
						case 3:
							mon2Obj.transform.Translate((monkey1Pos - mon2Obj.transform.position).normalized * speed1 * Time.deltaTime);
							mon1Obj.transform.Translate((monkey2Pos - mon1Obj.transform.position).normalized * speed2 * Time.deltaTime);
							if(CheckDistance(monkey1Pos + Vector3.forward, monkey1Pos, mon2Obj.transform.position))
							{
								moveState = "Stop";
							}
							break;
						}
						break;
					//from 4 to 5 are movements for monkeys that are separated by a another
					case 4://mon1 front, mon2 back

						switch(moveStage){
						case 1:
		
							Vector3 firstPos = monkey1Pos + Vector3.back;
							mon1Obj.transform.Translate(Vector3.back * speed1 * Time.deltaTime);
							mon2Obj.transform.Translate(Vector3.forward * speed1 * Time.deltaTime);
							if(CheckDistance(monkey1Pos, firstPos, mon1Obj.transform.position))
							{
								moveStage = 2;
							}
							break;
						case 2:
	
							Vector3 secondPos = monkey2Pos + Vector3.back;
							Vector3 secondPos2 = monkey1Pos + Vector3.forward;
							mon1Obj.transform.Translate((secondPos - mon1Obj.transform.position).normalized * speed1 * Time.deltaTime);
							mon2Obj.transform.Translate((secondPos2 - mon2Obj.transform.position).normalized * speed1 * Time.deltaTime);
							if(CheckDistance(monkey1Pos + Vector3.back, monkey2Pos + Vector3.back, mon1Obj.transform.position))
							{
								moveStage = 3;
							}
							break;
						case 3:
							mon1Obj.transform.Translate((monkey2Pos - mon1Obj.transform.position).normalized * speed1 * Time.deltaTime);
							mon2Obj.transform.Translate((monkey1Pos - mon2Obj.transform.position).normalized * speed1 * Time.deltaTime);
							if(CheckDistance(monkey2Pos + Vector3.back, monkey2Pos, mon1Obj.transform.position))
							{
								moveState = "Stop";
							}
							break;
						}
						break;
					case 5://mon1 back, mon2 front

						switch(moveStage){
						case 1:
							Vector3 firstPos = monkey1Pos + Vector3.forward;
							mon1Obj.transform.Translate(Vector3.forward * speed1 * Time.deltaTime);
							mon2Obj.transform.Translate(Vector3.back * speed1 * Time.deltaTime);
							if(CheckDistance(monkey1Pos, firstPos, mon1Obj.transform.position))
							{
								moveStage = 2;
							}
							break;
						case 2:
	
							Vector3 secondPos = monkey2Pos + Vector3.forward;
							Vector3 secondPos2 = monkey1Pos + Vector3.back;
							mon1Obj.transform.Translate((secondPos - mon1Obj.transform.position).normalized * speed1 * Time.deltaTime);
							mon2Obj.transform.Translate((secondPos2 - mon2Obj.transform.position).normalized * speed1 * Time.deltaTime);
							if(CheckDistance(monkey1Pos + Vector3.forward, secondPos, mon1Obj.transform.position))
							{
								moveStage = 3;
							}
							break;
						case 3:
							mon1Obj.transform.Translate((monkey2Pos - mon1Obj.transform.position).normalized * speed1 * Time.deltaTime);
							mon2Obj.transform.Translate((monkey1Pos - mon2Obj.transform.position).normalized * speed1 * Time.deltaTime);
							if(CheckDistance(monkey2Pos + Vector3.forward, monkey2Pos, mon1Obj.transform.position))
							{
								moveState = "Stop";
							}
							break;
						}
						break;
					}
#endregion
				}
				
				break;
			case "Stop"://stops the current movement 
				numOfMovments--;
				mon1Obj.transform.position = monkey2Pos;
				mon2Obj.transform.position = monkey1Pos;
				moveState = "PickMonkeys";
				
				break;
				
			}
			break;
		case "Choose"://aqui es donde el jugador puede escoger el o los changos con objetos dependiendo de la instruccion
			pickTimer -= Time.deltaTime;
			switch(chooseState){
			case "ChooseMonkey"://input del jugador
				switch(insState){
				case "FindOne"://solo un objeto

					if(Input.GetMouseButtonUp(0))
					{
						if(selectScript.SelectionFunc(Input.mousePosition))
						{

							monPicked = true;
							if(selectScript.objSelected.GetComponentInChildren<Monkey>().withObject)
							{
								levelsPassed++;
								scoreE.DisplayScore(scoreScript.TempScoreSum(), configuration.sound==1);

								correct = true;
								monPicked = true;
								objectsUsed[0].SetActive(true);
								objectsUsed[0].transform.Find("TutArrow").gameObject.SetActive(false);
								if(tutorial)
								{
//									highlightScript.HighLightMonkey(monkWithObject, false);
								}
								state = "Result";
								pickTimer = pickTime;
								break;

							}
							else
							{

								if(tutorial)
								{
									scoreE.DisplayError(configuration.sound==1);
									correct = false;
									cor = false;

//									highlightScript.HighLightMonkey(monkWithObject, true);
								}
								else
								{
									scoreE.DisplayError(configuration.sound==1);
									monPicked = false;
									correct = false;
									levelsRepeated++;
									tries++;
									state = "Result";
								}
							}
						}
														
					}
					
					
					break;
				case "FindTwo"://dos objetos
					if(Input.GetMouseButtonUp(0))
					{
						if(selectScript.SelectionFunc(Input.mousePosition))
						{

							Monkey obj = selectScript.objSelected.GetComponentInChildren<Monkey>();
							if(obj.withObject)
							{	
								scoreE.DisplayScore(scoreScript.TempScoreSum(),configuration.sound==1);

								obj.objectGrabbed.SetActive(true);
								obj.objectGrabbed.transform.Find("TutArrow").gameObject.SetActive(false);
								objNumFind--;
								cor = false;
								pickTimer = pickTime;
							}
							else
							{
								scoreE.DisplayError(configuration.sound==1);
								correct = false;
								levelsRepeated++;
								tries++;
								state = "Result";
							}
						}
					}

					if(objNumFind == 0)
					{
						scoreE.DisplayScore(scoreScript.TempScoreSum(), configuration.sound==1);

						levelsPassed++;
						correct = true;
						state = "Result";
						pickTimer = pickTime;
						break;
					}

					
					break;
				case "FindOneOfTwo":// uno de dos objetos
					if(Input.GetMouseButtonUp(0))
					{
						if(selectScript.SelectionFunc(Input.mousePosition))
						{
							Monkey obj = selectScript.objSelected.GetComponentInChildren<Monkey>();
							if(obj.withObject)
							{
								if(objectsUsed[correctObj] == obj.objectGrabbed)
								{
									levelsPassed++;
									scoreE.DisplayScore(scoreScript.TempScoreSum(),configuration.sound==1);

									obj.objectGrabbed.SetActive(true);
									obj.objectGrabbed.transform.Find("TutArrow").gameObject.SetActive(false);
									correct = true;
									state = "Result";
									pickTimer = pickTime;
									break;
									
								}
								else
								{
									scoreE.DisplayError(configuration.sound==1);
									correct = false;
									levelsRepeated++;
									tries++;
									state = "Result";
									
								}
							}
							else
							{
								scoreE.DisplayError(configuration.sound==1);
								levelsRepeated++;
								correct = false;
								tries++;
								state = "Result";
							
							}
						}
					}
					break;
				}						
				break;
			}
			break;
		case "Result"://determina si se incrementa, se mantiene o se regresa de nivel
			//if(!tutorial)
			//{
				resultTimer -= Time.deltaTime;
				if(resultTimer <= 0)
				{
					if(correct)
					{
						scoreScript.prevCorMult++;
						state = "IncreaseDifficulty";
					}
					else
					{
						scoreScript.prevCorMult = 0;
						if(tries == 3)
						{
							state = "DecreaseDifficulty";
						}
						else
						{
							state = "SameLevel";
						}
					}
					resultTimer = resultTime;
				}
			//}


			
			finishState = "PlatsReverse";
			break;
		case "SameLevel"://se repite el nivel y se resetea a los changos de posicicon para que salgan de la pantalla
			switch(finishState){
			case "PlatsReverse":
				foreach(GameObject obj in objectsUsed)
				{
					//play animation to hide the object
					obj.SetActive(false);
				}
				platCount = 0;
				foreach(GameObject obj in monkeysActive)
				{
					if(SetPlat(obj, "Reverse", 0, 5f))
					{
						platCount++;
					}
				}

				if(platCount == monkeysActive.Count && platCount != 0)
				{
					finishState = "HidePlats";
				}
				platCount = 0;
				break;
			case "HidePlats":

				foreach(GameObject obj in monPlats)
				{
					obj.SetActive(false);
				}
				
				foreach(GameObject obj in plats)
				{
					obj.SetActive(true);
				}
				finishState = "MonReturn";
				break;
			case "MonReturn":
				posCount = 0;
				foreach(GameObject mon in monkeysActive)
				{
					
					MoveToPosition mov = mon.GetComponent<MoveToPosition>();
					mov.MoveOutOfPosFunc();
					if(mov.inPos)
					{
						posCount++;
					}
				}
				if(posCount == monkeysActive.Count)
				{
					finishState = "SaveProgress";
				}
				break;
			case "SaveProgress":
				SaveLevelProgress();//salva el nivel
				break;
			case "PlatsBurry":
				if(plat)
				{
					platCount = 0;
					foreach(GameObject obj in plats)
					{
						if(SetPlat(obj, "Reverse", -.1f, .05f))
						{
							platCount++;
						}
					}
					if(platCount == plats.Count && platCount != 0)
					{
						plat = false;
					}
					platCount = 0;
				}
				else
				{
					setLevel = false;
					if(levelsPlayed +todayOffset>= numLevelsPerBlock)
					{
						CalculateKiwis();
						sessionMng.activeKid.blockedDondeQuedoLaBolita=1;
						sessionMng.SaveSession();
						state = "CompleteActivity";
					}
					else
					{
						state = "Start";
					}
				}
				break;
			}
			break;
		case "IncreaseDifficulty"://incrementa el nivel
			switch(finishState){
			case "PlatsReverse":
				foreach(GameObject obj in objectsUsed)
				{
					//play animation to hide the object
					obj.SetActive(false);
				}
				platCount = 0;
				foreach(GameObject obj in monkeysActive)
				{
					if(SetPlat(obj, "Reverse", 0, 5f))
					{
						platCount++;
					}
				}
				
				if(platCount == monkeysActive.Count && platCount != 0)
				{
					finishState = "HidePlats";
				}

				break;
			case "HidePlats":
				tries = 0;
				foreach(GameObject obj in monPlats)
				{
					obj.SetActive(false);
				}
				
				foreach(GameObject obj in plats)
				{
					obj.SetActive(true);
				}
				finishState = "MonReturn";
				break;
			case "MonReturn":
				posCount = 0;
				foreach(GameObject mon in monkeysActive)
				{
					MoveToPosition mov = mon.GetComponent<MoveToPosition>();
					mov.MoveOutOfPosFunc();
					if(mov.inPos)
					{
						posCount++;
					}
				}
				if(posCount == monkeysActive.Count)
				{
					finishState = "SaveProgress";
				}
				break;
			case "SaveProgress":
				SaveLevelProgress();//salva el nivel
				break;
			case "PlatsBurry":
				if(plat)
				{
					platCount = 0;
					foreach(GameObject obj in plats)
					{
						if(SetPlat(obj, "Reverse", -.1f, .05f))
						{
							platCount++;
						}
					}
					if(platCount == plats.Count && platCount != 0)
					{
						
						plat = false;
					}
					platCount = 0;
				}
				else
				{

					setLevel = false;
					if(level == configuration.levels.Length - 1 && subLevel == configuration.levels[level].subLevels.Length - 1)
					{

					}
					else
					{
						subLevel++;
						if(subLevel >= configuration.levels[level].subLevels.Length)
						{
							level++;
							subLevel = 0;
						}	
					}
					
					if(level == 0 && subLevel == 0)
					{
						tutorial = true;
					}
					else 
					{
						tutorial = false;
					}
					if(levelsPlayed +todayOffset>= numLevelsPerBlock)
					{
						Debug.Log("++");
						CalculateKiwis();
						sessionMng.activeKid.blockedDondeQuedoLaBolita=1;
						sessionMng.SaveSession();
						state = "CompleteActivity";
					}
					else
					{
						state = "Start";
					}
					sessionMng.activeKid.monkeyDifficulty=level;
					sessionMng.activeKid.monkeyLevel=subLevel;
					sessionMng.SaveSession();
				}
				break;
			}
			break;
		case "DecreaseDifficulty"://regresa un nivel
			switch(finishState){
			case "PlatsReverse":
				foreach(GameObject obj in objectsUsed)
				{
					//play animation to hide the object
					obj.SetActive(false);
				}
				platCount = 0;
				foreach(GameObject obj in monkeysActive)
				{
					if(SetPlat(obj, "Reverse", 0f, 5f))
					{
						platCount++;
					}
				}
				
				if(platCount == monkeysActive.Count && platCount != 0)
				{
					finishState = "HidePlats";
				}
				platCount = 0;
				break;
			case "HidePlats":
				tries = 0;
				foreach(GameObject obj in monPlats)
				{
					obj.SetActive(false);
				}
				
				foreach(GameObject obj in plats)
				{
					obj.SetActive(true);
				}
				finishState = "MonReturn";
				break;
			case "MonReturn":
				posCount = 0;
				foreach(GameObject mon in monkeysActive)
				{
					
					MoveToPosition mov = mon.GetComponent<MoveToPosition>();
					mov.MoveOutOfPosFunc();
					if(mov.inPos)
					{
						posCount++;
					}
				}
				if(posCount == monkeysActive.Count)
				{
					finishState = "SaveProgress";
				}
				break;
			case "SaveProgress":
				SaveLevelProgress();//salva el nivel
				break;
			case "PlatsBurry":
				if(plat)
				{
					platCount = 0;
					foreach(GameObject obj in plats)
					{
						if(SetPlat(obj, "Reverse", -.1f, .05f))
						{
							platCount++;
						}
					}
					if(platCount == plats.Count && platCount != 0)
					{
						plat = false;

					}
					platCount = 0;
				}
				else
				{
//					Debug.Log("dfg");
					setLevel = false;
					subLevel--;
					if(subLevel < 0)
					{
						level--;
						subLevel = configuration.levels[level].subLevels.Length - 1;
					}
					if(level == 0 && subLevel == 0)
					{
						tutorial = true;
					}
					else 
					{
						tutorial = false;
					}
					if(levelsPlayed +todayOffset>= numLevelsPerBlock)
					{
						Debug.Log("--");
						CalculateKiwis();
						sessionMng.activeKid.blockedDondeQuedoLaBolita=1;
						sessionMng.SaveSession();
						state = "CompleteActivity";
					}
					else
					{
						state = "Start";
					}
					sessionMng.activeKid.monkeyDifficulty=level;
					sessionMng.activeKid.monkeyLevel=subLevel;
					sessionMng.SaveSession();
				}
				break;
			}
			break;
		case "CompleteActivity"://presenta la pantalla con el score y presenta los kiwis obtenidos

			if(!scoreScript.finalScore)
			{
				kiwiAnimCurrTime-=Time.deltaTime;
				if(kiwiAnimCurrTime<=0)
				{
					kiwiAnimCurrTime=kiwiAnimTime;
					if(animationKiwis<kiwis)
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

	void CalculateKiwis()//calcula el numero de kiwis obtenidos durante los niveles juagdos
	{
		if(kiwis==-1)
		{
			kiwis=0;
			int totalScore=(int)(((float)levelsPassed/(float)levelsPlayed)*100);
			Debug.Log("Total Score"+totalScore.ToString());
			soundMng.pauseQueue=true;
			if(totalScore>80)
			{
				soundMng.AddSoundToQueue(21,false,false);
				kiwis=3;
			}else if(totalScore>60)
			{
				soundMng.AddSoundToQueue(20,false,false);
				kiwis=2;
			}else if(totalScore>20)
			{
				soundMng.AddSoundToQueue(19,false,false);
				kiwis=1;
			}
			int tempKiwis=sessionMng.activeKid.kiwis;
            sessionMng.activeKid.kiwis = tempKiwis + kiwis + scoreScript.GetExtraKiwis();
			sessionMng.SaveSession();
			if(kiwis==0)
			{
				soundMng.AddSoundToQueue(17,false,false);
				soundMng.AddSoundToQueue(18,true,false);
			}else{
				soundMng.AddSoundToQueue(16,false,false);
				soundMng.AddSoundToQueue(18,true,false);
			}
			GetComponent<ProfileSync>().UpdateProfile();
		}
	}

	void SaveLevelProgress()//salva todas las variables y metricas
	{
		saveHandler.AddLevelData("levelKey", "Monos");
		saveHandler.AddLevelData("level", level);
		saveHandler.AddLevelData("subLevel", subLevel);
		saveHandler.AddLevelData("numOfMonkeys", numberOfMonkeys);
		saveHandler.AddLevelData("numOfObjects", numberOfObjects);
		saveHandler.AddLevelData("numOfMovements", progNumOfMov);
		saveHandler.AddLevelData("timeOfMovements", timeForMovement);
		saveHandler.AddLevelData("instructions", insState);
		saveHandler.AddLevelData("correct", correct);
		saveHandler.AddLevelData("time", (int)timeOfLevel);
		timeOfLevel = 0;
		levelsPlayed++;
		sessionMng.activeKid.playedMonkey = levelsPlayed + todayOffset;
		sessionMng.SaveSession ();
		//saveHandler.SetLevel();
		if(levelsPlayed+todayOffset >= numLevelsPerBlock)
		{
			SaveProgress(true);
			finishState = "PlatsBurry";
		}
		else
		{
			finishState = "PlatsBurry";
		}

	}
	void SaveProgress(bool rank)
	{
		if(levelsPlayed>0)
		{
			saveHandler.CreateSaveBlock("DondeQuedoLaBolita", (int)totalTime, levelsPassed, levelsRepeated, levelsPlayed);
			saveHandler.AddLevelsToBlock();
			saveHandler.PostProgress(rank);
			Debug.Log(saveHandler.ToString());
		}
	}
	void OnGUI()
	{
//		AdvanceBut = new Rect()
		screenScale = (float)Screen.height / (float)768;
		pauseText.fontSize = 60;
		pauseText.fontSize = (int)(pauseText.fontSize * screenScale);
		tutStyle.fontSize = 35;
		tutStyle.fontSize = (int)(tutStyle.fontSize * screenScale);
		advanceStyle.fontSize = 30;
		advanceStyle.fontSize = (int)(advanceStyle.fontSize * screenScale);
		switch(state){
		case "Start":
			switch(startState){
			case "MoveToPos":


				if(moveToPos && tutorial)
				{
					if(!play)
					soundMng.PlaySound(0);
					play = true;
					GUI.DrawTexture(new Rect(Screen.width*0.5f - 500 * screenScale, Screen.height * 0.5f + 150 * screenScale, 1000 * screenScale, 200 * screenScale), tutBackground);
					tutStyle.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * 0.5f - 398 * screenScale, Screen.height * 0.5f + 108 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[0], tutStyle);
					tutStyle.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * 0.5f - 400 * screenScale, Screen.height * 0.5f + 110 *screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[0], tutStyle);
				}

				break;
			case "PlatsElevate":
				if(!moveToPos && tutorial)
				{

					GUI.DrawTexture(new Rect(Screen.width*0.5f - 500 * screenScale, Screen.height * 0.5f + 150 * screenScale, 1000 * screenScale, 200 * screenScale), tutBackground);
					tutStyle.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * 0.5f - 398 * screenScale, Screen.height * 0.5f + 108 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[0], tutStyle);
					tutStyle.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * 0.5f - 400 * screenScale, Screen.height * 0.5f + 110 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[0], tutStyle);
					
//					if(!audioSrc.isPlaying)
//					{
//						play = false;
//						if(tutorial)
//						{
//							
//							if(tutScript.advance)
//							{
//								showObj = false;
//								tutScript.advance = false;
//								state = "ShowObjects";
//							}	
//						}
//					}
				}
				break;
			}
			break;
		case "ShowObjects":
			switch(showState){
			case "Show":
				if(tutorial)
				{
					if(hideTimer != hideTime)
					{
						hideTimer -= Time.deltaTime;
						if(hideTimer <= 0)
						{
							showState = "HideObject";
							hideTimer = hideTime;
						}
					}
					else
					{
						if(!play)
						soundMng.PlaySound(1);
						play = true;
						GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f + 150 * screenScale, 1000 * screenScale, 200 * screenScale), tutBackground);
						tutStyle.normal.textColor = new Color(0.32f,0.32f,0.32f);
						GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f + 108 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[1], tutStyle);
						tutStyle.normal.textColor = new Color(0.88f,0.88f,0.88f);
						GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f + 110 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[1], tutStyle);
						if(!audioSrc.isPlaying)
							if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 140 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[20], advanceStyle))
						{
							play = false;
							hideTimer -= Time.deltaTime;
						}
					}
				}
				break;
			case "WaitForMovements":
				if(tutorial)
				{
					if(!play)
					soundMng.PlaySound(2);
					play = true;
					GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f + 150 * screenScale, 1000 * screenScale, 200 * screenScale), tutBackground);
					tutStyle.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f + 108 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[2], tutStyle);
					tutStyle.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f + 110 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[2], tutStyle);
					if(!audioSrc.isPlaying)
						if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 140 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[20], advanceStyle))
					{
						play = false;
						state = "Move";
						moveState = "PickMonkeys";
					}
				}
				break;
			}
			break;
		case "Choose":
			switch(chooseState){
			case "Instructions":
				switch(insState){
				case "FindOne":

					if(!play)
					soundMng.PlaySound(3);
					play = true;
					if(tutorial)
					{
						GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f + 150 * screenScale, 1000 * screenScale, 200 * screenScale), tutBackground);
						tutStyle.normal.textColor = new Color(0.32f,0.32f,0.32f);
						GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f + 108 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[3], tutStyle);
						tutStyle.normal.textColor = new Color(0.88f,0.88f,0.88f);
						GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f + 110 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[3], tutStyle);
						if(!audioSrc.isPlaying)
						if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 140 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[20], advanceStyle))
						{
							play = false;
							chooseState = "ChooseMonkey";
						}
					}
					else
					{
						GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f + 150 * screenScale, 1000 * screenScale, 200 * screenScale), tutBackground);
						tutStyle.normal.textColor = new Color(0.32f,0.32f,0.32f);
						GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f + 108 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[3], tutStyle);
						tutStyle.normal.textColor = new Color(0.88f,0.88f,0.88f);
						GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f + 110 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[3], tutStyle);
						if(!audioSrc.isPlaying)
						{
							play = false;
							chooseState = "ChooseMonkey";
						}
					}

					break;
				case "FindTwo":
					if(!play)
					soundMng.PlaySound(4);
					play = true;
					GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f + 150 * screenScale, 1000 * screenScale, 200 * screenScale), tutBackground);
					tutStyle.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f + 108 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[4], tutStyle);
					tutStyle.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f + 110 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[4], tutStyle);
					if(!audioSrc.isPlaying)
					{
						play = false;
						chooseState = "ChooseMonkey";
					}
					break;
				case "FindOneOfTwo":
					if(!objAssigned)
					{
						if(tries == 0)
						correctObj = Random.Range(0, 2);
						

						if(correctObj == 0)
						{
							soundMng.AddSoundToQueue(14);
							ballName = language.levelStrings[6];
						}
						else
						{
							soundMng.AddSoundToQueue(15);
							ballName = language.levelStrings[7];
						}
						objAssigned = true;
					}
					if(!play)
					soundMng.PlaySound(5);
					play = true;
					GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f + 150 * screenScale, 1000 * screenScale, 200 * screenScale), tutBackground);
					tutStyle.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f + 108 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[5]+" " + ballName, tutStyle);
					tutStyle.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f + 110 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[5]+" " + ballName, tutStyle);
					if(!audioSrc.isPlaying)
					{
						play = false;
						chooseState = "ChooseMonkey";
						objAssigned = false;
					}
					break;
				}
				break;
			case "ChooseMonkey":
				switch(insState){
				case "FindOne":
					if(tutorial && monPicked)
					{
						if(!cor)
						{
							if(!play)
							soundMng.PlaySound(6);
							play = true;
							GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f + 150 * screenScale, 1000 * screenScale, 200 * screenScale), tutBackground);
							tutStyle.normal.textColor = new Color(0.32f,0.32f,0.32f);
							GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f + 108 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[8], tutStyle);
							tutStyle.normal.textColor = new Color(0.88f,0.88f,0.88f);
							GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f + 110 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[8], tutStyle);
							if(!audioSrc.isPlaying)
								if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 140 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[20], advanceStyle))
							{
								play = false;
								cor = true;
							}
						}

					}
					break;
				case "FindTwo":
//					if(objNumFind == 1)
//					{
//						if(!cor)
//						{
//							if(!play)
//							soundMng.PlaySound(7);
//							play = true;
//							GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 150 * screenScale, 1000 * screenScale, 300 * screenScale), tutBackground);
//							tutStyle.normal.textColor = new Color(0.32f,0.32f,0.32f);
//							GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), "Muy Bien ahora encuentra\nel segundo objeto!!!", tutStyle);
//							tutStyle.normal.textColor = new Color(0.88f,0.88f,0.88f);
//							GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), "Muy Bien ahora encuentra\nel segundo objeto!!!", tutStyle);
//							if(!audioSrc.isPlaying)
//							if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 130 * screenScale, 160 * screenScale, 80 * screenScale), "LISTO", advanceStyle))
//							{
//								play = false;
//								cor = true;
//							}
//						}
//
//					}
					break;
				}
				break;

			}
			break;
		case "Result":
			switch(insState){
			case "FindOne":
				if(correct)
				{
				
					if(tutorial)
					{
						if(!play)
						soundMng.PlaySound(8);
						play = true;
						GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f + 150 * screenScale, 1000 * screenScale, 200 * screenScale), tutBackground);
						tutStyle.normal.textColor = new Color(0.32f,0.32f,0.32f);
						GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f + 108 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[9], tutStyle);
						tutStyle.normal.textColor = new Color(0.88f,0.88f,0.88f);
						GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f + 110 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[9], tutStyle);
						if(!audioSrc.isPlaying)
							if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 140 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[20], advanceStyle))
						{
							play = false;
							state = "IncreaseDifficulty";
						}
					}
//					else
//					{
//						GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f + 150 * screenScale, 1000 * screenScale, 200 * screenScale), tutBackground);
//						tutStyle.normal.textColor = new Color(0.32f,0.32f,0.32f);
//						GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f + 108 * screenScale, 800 * screenScale, 300 * screenScale), "Muy bien!!!!\nEncontraste el objeto.", tutStyle);
//						tutStyle.normal.textColor = new Color(0.88f,0.88f,0.88f);
//						GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f + 110 * screenScale, 800 * screenScale, 300 * screenScale), "Muy bien!!!!\nEncontraste el objeto.", tutStyle);
//						if(!audioSrc.isPlaying)
//						{
//							play = false;
//							state = "IncreaseDifficulty";
//						}
//					}


				}
//				else
//				{
//					if(!play)
//					soundMng.PlaySound(9);
//					play = true;
//					GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f + 150 * screenScale, 1000 * screenScale, 200 * screenScale), tutBackground);
//					tutStyle.normal.textColor = new Color(0.32f,0.32f,0.32f);
//					GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f + 108 * screenScale, 800 * screenScale, 300 * screenScale), "Oh No!!!\nese changuito no tiene el objeto.", tutStyle);
//					tutStyle.normal.textColor = new Color(0.88f,0.88f,0.88f);
//					GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f + 110 * screenScale, 800 * screenScale, 300 * screenScale), "Oh No!!!\nese changuito no tiene el objeto.", tutStyle);
//					if(!audioSrc.isPlaying)
//					{
//						play = false;
//						if(tries == 3)
//						{
//							state = "DecreaseDifficulty";	
//						}
//						else
//						{
//							state = "SameLevel";
//						}
//					}
//
//				}	
				break;
			case "FindTwo":
//				if(correct)
//				{
//					if(!play)
//					soundMng.PlaySound(10);
//					play = true;
//					GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f + 150 * screenScale, 1000 * screenScale, 200 * screenScale), tutBackground);
//					tutStyle.normal.textColor = new Color(0.32f,0.32f,0.32f);
//					GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f + 108 * screenScale, 800 * screenScale, 300 * screenScale), "Muy bien!!!!\nYa encontraste los dos objetos!!", tutStyle);
//					tutStyle.normal.textColor = new Color(0.88f,0.88f,0.88f);
//					GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f + 110 * screenScale, 800 * screenScale, 300 * screenScale), "Muy bien!!!!\nYa encontraste los dos objetos!!", tutStyle);
//					if(!audioSrc.isPlaying)
//					{
//						play = false;
//						state = "IncreaseDifficulty";
//					}
//				}
//				else
//				{
//					if(!play)
//					soundMng.PlaySound(11);
//					play = true;
//					GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f + 150 * screenScale, 1000 * screenScale, 200 * screenScale), tutBackground);
//					tutStyle.normal.textColor = new Color(0.32f,0.32f,0.32f);
//					GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f + 108 * screenScale, 800 * screenScale, 300 * screenScale), "Oh No!!!\nEse changuito no tenía\nningún objeto!!", tutStyle);
//					tutStyle.normal.textColor = new Color(0.88f,0.88f,0.88f);
//					GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f + 110 * screenScale, 800 * screenScale, 300 * screenScale), "Oh No!!!\nEse changuito no tenía\nningún objeto!!", tutStyle);
//					if(!audioSrc.isPlaying)
//					{
//						play = false;
//						if(tries == 3)
//						{
//							state = "DecreaseDifficulty";	
//						}
//						else
//						{
//							state = "SameLevel";
//						}
//					}
//				}
				break;
			case "FindOneOfTwo":
//				if(correct)
//				{
//					if(!play)
//					soundMng.PlaySound(12);
//					play = true;
//					GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f + 150 * screenScale, 1000 * screenScale, 200 * screenScale), tutBackground);
//					tutStyle.normal.textColor = new Color(0.32f,0.32f,0.32f);
//					GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f + 108 * screenScale, 800 * screenScale, 300 * screenScale), "Muy bien!!!!\nEncontraste el objeto correcto!!!", tutStyle);
//					tutStyle.normal.textColor = new Color(0.88f,0.88f,0.88f);
//					GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f + 110 * screenScale, 800 * screenScale, 300 * screenScale), "Muy bien!!!!\nEncontraste el objeto correcto!!!", tutStyle);
//					if(!audioSrc.isPlaying)
//					{
//						play = false;
//						state = "IncreaseDifficulty";
//					}
//				}
//				else
//				{
//					if(!play)
//					soundMng.PlaySound(13);
//					play = true;
//					GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f + 150 * screenScale, 1000 * screenScale, 200 * screenScale), tutBackground);
//					tutStyle.normal.textColor = new Color(0.32f,0.32f,0.32f);
//					GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f + 108 * screenScale, 800 * screenScale, 300 * screenScale), "Oh No!!!\nNo encontraste el objeto\ncorrecto!!", tutStyle);
//					tutStyle.normal.textColor = new Color(0.88f,0.88f,0.88f);
//					GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f + 110 * screenScale, 800 * screenScale, 300 * screenScale), "Oh No!!!\nNo encontraste el objeto\ncorrecto!!", tutStyle);
//					if(!audioSrc.isPlaying)
//					{
//						play = false;
//						if(tries == 3)
//						{
//							state = "DecreaseDifficulty";	
//						}
//						else
//						{
//							state = "SameLevel";
//						}
//					}
//				}
				break;
			}
			break;
		case "CompleteActivity":
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
				GUI.Label (new Rect (Screen.width * 0.5f - 400 * screenScale, Screen.height * 0.5f - 150 * screenScale, 800 * screenScale, 100 * screenScale), language.levelStrings[10],scoreStyle1);
			else
				GUI.Label (new Rect (Screen.width * 0.5f - 400 * screenScale, Screen.height * 0.5f - 150 * screenScale, 800 * screenScale, 100 * screenScale), language.levelStrings[11],scoreStyle1);
			if(kiwis==0)
				GUI.Label (new Rect (Screen.width * 0.5f - 400 * screenScale, Screen.height * 0.5f - 90 * screenScale, 800 * screenScale, 100 * screenScale), language.levelStrings[12],scoreStyle2);
			else{
				if(kiwis>1)
					GUI.Label (new Rect (Screen.width * 0.5f - 400 * screenScale, Screen.height * 0.5f - 90 * screenScale, 800 * screenScale, 100 * screenScale), language.levelStrings[13]+" "+kiwis.ToString()+" "+language.levelStrings[14],scoreStyle2);
				else
					GUI.Label (new Rect (Screen.width * 0.5f - 400 * screenScale, Screen.height * 0.5f - 90 * screenScale, 800 * screenScale, 100 * screenScale), language.levelStrings[13]+" "+kiwis.ToString()+" "+language.levelStrings[15],scoreStyle2);
			}
			if(GUI.Button(new Rect(Screen.width*0.5f-80*screenScale,Screen.height*0.5f+110*screenScale,160*screenScale,60*screenScale),language.levelStrings[21],kiwiButton))
			{
				returnToIsland=true;
			}


			if(scoreScript.finalScore)
			{
				scoreScript.scoreStyle.fontSize = 50;
				scoreScript.scoreStyle.fontSize = (int)(scoreScript.scoreStyle.fontSize * screenScale);
				GUI.Label(new Rect(Screen.width * 0.5f + 270 * screenScale, Screen.height * 0.5f - 90 * screenScale, 100 *screenScale, 50 * screenScale), scoreScript.scoreString, scoreScript.scoreStyle);

				scoreScript.GuiExtraKiwisDisplay();
                if (scoreScript.scoreCounter >= scoreScript.kiwiMilestone)
				{
					GUI.DrawTexture (new Rect (Screen.width * 0.5f + 245 * screenScale, Screen.height * 0.5f - 20 * screenScale, 150 * screenScale, 150 * screenScale), scoreKiwi);
					GUI.Label(new Rect(Screen.width * 0.5f + 380 * screenScale, Screen.height * 0.5f + 50 * screenScale, 100 *screenScale, 50 * screenScale), "x" + scoreScript.extraKiwis, scoreScript.scoreStyle);
				}
			}
			break;
		case "Pause":
			float pauseY = Screen.height * 0.5f - 177 * screenScale;
			GUI.DrawTexture(new Rect(0, pauseY, Screen.width, 354 * screenScale), pauseBackground);
			GUI.Label(new Rect(Screen.width * 0.5f - 100 * screenScale, pauseY - 40 * screenScale, 200 * screenScale, 60 * screenScale), language.levelStrings[16], pauseText);
			if(GUI.Button(new Rect(Screen.width * 0.5f - 200 * screenScale, pauseY + 50 * screenScale, 366 * screenScale, 66 * screenScale), "", pauseContinue))
			{
				Time.timeScale = 1f;
				state = stateBeforePause;
			}
			else if(GUI.Button(new Rect(Screen.width * 0.5f-200 * screenScale, pauseY + 140 * screenScale, 382 * screenScale, 66 * screenScale), "", pauseIsland))
			{
				if(!returnToIsland){
					SaveProgress(false);
					returnToIsland=true;
					Time.timeScale=1.0f;
				}
				//Application.LoadLevel("Archipielago");
			}
			GUI.Label(new Rect(Screen.width*0.5f-110*screenScale,pauseY+50*screenScale,366*screenScale,66*screenScale),language.levelStrings[18],pauseButtons);
			GUI.Label(new Rect(Screen.width*0.5f-110*screenScale,pauseY+140*screenScale,382*screenScale,66*screenScale),language.levelStrings[19],pauseButtons);
//			else if(GUI.Button(new Rect(Screen.width * 0.5f - 200 * screenScale, pauseY + 230 * screenScale, 162 * screenScale, 67 * screenScale), "", pauseExit))
//			{
//				Time.timeScale = 1.0f;
//				Application.Quit();
//			}
			break;

		}
		if(state != "Pause")
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
	bool CheckDistance(Vector3 initialPos, Vector3 finalPos, Vector3 objectPosition)
	{
		if(Vector3.Distance(objectPosition, initialPos) >= Vector3.Distance(initialPos, finalPos))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	bool SetPlat(GameObject plat, string option, float height, float speed)
	{

		switch(option){
		case "Normal":
			if(plat.transform.position.y >= height)
			{
				plat.transform.position = new Vector3(plat.transform.position.x, height, plat.transform.position.z);
				return true;
			}
			else
			{
				plat.transform.Translate(Vector3.up * Time.deltaTime * speed);
			}

			break;
		case "Reverse":
		
			if(plat.transform.position.y <= height)
			{
				plat.transform.position = new Vector3(plat.transform.position.x, height, plat.transform.position.z);
				return true;
			}
			else
			{
				plat.transform.Translate(Vector3.down * Time.deltaTime * speed);
			}
			break;
		}
		return false;
	}
	
	void SetLevel()
	{
		//Clears objects and variables
		objectsUsed.Clear();
		if(tries == 0)
		{
			mon1num.Clear();
			mon2num.Clear();
			moveTypeNum.Clear();
			objectPos.Clear();
		}


	
		monkeysActive.Clear();
		monPlats.Clear();
		retryNumCount = 0;
		
		numberOfMonkeys = 0;
		numberOfObjects = 0;
		timeForMovement = 0;
		numOfMovments = 0;
		for(int i = 0; i < monkeys.Length; i++)
		{
			monkeys[i].SetActive(false);
		}
		for(int i = 0; i < monkeys.Length; i++)
		{
			monkeys[i].transform.Find("MonModel").transform.Find("ObjPos").GetComponent<Monkey>().CleanObject();
		}
		
		//sets the level
		numberOfMonkeys = configuration.levels[level].subLevels[subLevel].monkeys;
		if(numberOfMonkeys < 3)
			numberOfMonkeys = 3;
		if(numberOfMonkeys > 5)
			numberOfMonkeys = 5;
		numberOfObjects = configuration.levels[level].subLevels[subLevel].objectNum;
		objNumFind = numberOfObjects;
		timeForMovement = configuration.levels[level].subLevels[subLevel].time;
		numOfMovments = configuration.levels[level].subLevels[subLevel].movementNum;
		progNumOfMov = numOfMovments;
		insState = configuration.levels[level].subLevels[subLevel].instructions;
		speed = timeForMovement / (float)numOfMovments;
		//activates the monkeys accordingly to the number of monkeys in this sublevel
		for(int i = 0; i < numberOfMonkeys; i++)
		{
			monkeys[i].SetActive(true);
			monkeysActive.Add(monkeys[i]);
			monPlats.Add(monkeys[i].transform.Find("plataformaK").gameObject);
			monkeys[i].transform.Find("plataforma").gameObject.SetActive(false);
			
		}
		
		//assigns each object used in the sublevel to a random monkey
		if(tries == 0)
		{
			for(int i = 0; i < numberOfObjects; i++)
			{
				objectPos.Add( Random.Range(0, numberOfMonkeys)); // gives the objects to the monkeys
				
				while(objectPos[i] == prevPos)//checks for a repeated position
				{
					objectPos[i] = Random.Range(0, numberOfMonkeys);
				}
				
				prevPos = objectPos[i]; //store current pos to a prevpos var to check repeated pos
				Monkey mon = monkeys[objectPos[i]].GetComponentInChildren<Monkey>();
				mon.objectGrabbed = objectsStored[i];
				mon.GrabObject();//parents the monkey to the object and moves it to the hand position
				objectsUsed.Add(objectsStored[i]);
				if(tutorial)
				{
					monkWithObject = monkeys[objectPos[i]];
				}
			}
			prevPos = 100;
		}
		else
		{
			for(int i = 0; i < numberOfObjects; i++)
			{
				Monkey mon = monkeys[objectPos[i]].GetComponentInChildren<Monkey>();
				mon.objectGrabbed = objectsStored[i];
				mon.GrabObject();//parents the monkey to the object and moves it to the hand position
				objectsUsed.Add(objectsStored[i]);
			}
		}


		plats.Clear();
		foreach(GameObject mon in monkeys)
		{
			mon.GetComponent<MoveToPosition>().posAssigned = false;
		}
		showState = "Show";
		setLevel = true;
		plat = true;

		
		
	}
}
