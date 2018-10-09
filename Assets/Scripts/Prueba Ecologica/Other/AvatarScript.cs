using UnityEngine;
using System.Collections;
//script para manejo de instrucciones habladas y escritas
public class AvatarScript : MonoBehaviour
{
	//script var
	PackLogic packScript;

	PEMainLogic logicScript;
	AgeInput ageInputScript;

	AirportRouteLogic routeLogic;
	WaitingLogic waitScript;
	FlyPlaneLogic flyScript;
	PickUpCoinsLogic pickUpScript;
	UnPackLogic unPackScript;




	//game vars
	public GameObject dialog;
	string[] objs;
	string obj1;
	string obj2;
	string obj3;
	string obj4;
	string obj5;
	string obj6;
	string obj7;
	string obj8;
	string obj9;

	public string unPackObj1;
	public string unPackObj2;
	public string unPackObj3;

	int objNum;

	public bool objReady = false;
	
	//onGui vars
	float screenScale;
	public GUIStyle pauseButton;
	public Texture2D pauseBackground;
	public GUIStyle pauseContinue;
	public GUIStyle pauseIsland;
	public GUIStyle pauseExit;
	public GUIStyle pauseText;
	public GUIStyle styleX;
	public GUIStyle advanceStyle;
	public Texture bGBlue;
	public Texture moneda;



	//logic vars
	float timer;
	public float timerTime = 2f;
	public int numOfRounds;
	string stateBeforePause;
	bool mobile = false;
	public bool count;
	public int objC = 0;
	public bool packObjs = false;
	float timefWord = 3f;


	Rect label = new Rect(Screen.width * .5f, Screen.height * .5f, 250, 100);
	SoundManager soundMng;
	bool soundPlayed = false;
	int soundToPlay = 0;
	bool insDone = false;

	string finalInsState;
	int finalInsCount;
	int objCount;

	bool displayMes = false;

	public LanguageLoader language;

	SessionManager sessionMng;
	string objectO;
	string lang = "es";
	string tempS;

	// Use this for initialization
	void Start ()
	{
		sessionMng = GetComponent<SessionManager>();
		lang = sessionMng.activeUser.language;
		if(lang=="")
			lang="es";
		language = GetComponent<LanguageLoader>();
		language.LoadGameLanguage(lang);
		soundMng = GetComponent<SoundManager>();
		packScript = GameObject.Find("Pack").GetComponent<PackLogic>();
		logicScript = GameObject.FindGameObjectWithTag("Main").GetComponent<PEMainLogic>();
		ageInputScript = GameObject.Find("PlayerInfo").transform.Find("AgeInput").GetComponent<AgeInput>();
		routeLogic = GameObject.Find("AirportRoute").GetComponent<AirportRouteLogic>();
		waitScript = GameObject.Find("WaitingRoom").GetComponent<WaitingLogic>();
		flyScript = GameObject.Find("FlyPlane").GetComponent<FlyPlaneLogic>();
		pickUpScript = GameObject.Find("PickUpCoins").GetComponent<PickUpCoinsLogic>();
		unPackScript = GameObject.Find("UnPack").GetComponent<UnPackLogic>();

		if(SystemInfo.deviceType == DeviceType.Handheld)
		{
			mobile = true;
		}
		timer = timefWord;
		unPackObj1 = language.levelStrings[38];
		unPackObj2 = language.levelStrings[23];
		unPackObj3 = language.levelStrings[36];
	}

	// Update is called once per frame
	void Update ()
	{
		if(packObjs && objReady)
		{
			if(count)
			{
				if(!soundPlayed)
				{
					switch (objs [objC]) {
					case "Alcancía":
						soundToPlay = 7;
						break;
					case "Cinturón":
						soundToPlay = 25;
						break;
					case "Pantalón":
						soundToPlay = 11;
						break;
					case "Cámara":
						soundToPlay = 22;
						break;
					case "Lápiz":
						soundToPlay = 55;
						break;
					case "Pino de Boliche":
						soundToPlay = 58;
						break;
					default:
						for(int i = 0; i < soundMng.sounds.Length; i++)
						{
							if(soundMng.sounds[i].name == objs[objC])
							{
								soundToPlay = i;
								break;
							}
						}
						break;
					}

					soundPlayed = true;
					soundMng.PlaySound(soundToPlay);
				}
				timer -= Time.deltaTime;
				if(timer <= 0)
				{
					soundPlayed = false;
					timer = timefWord;
					count = false;
				}
			}
			else
			{
				if(insDone)
				{
					if(objC == objs.Length - 1)
					{
						packScript.goToPick = false;
						packScript.avatar.SetActive(false);
						packScript.gameState = "PlayerPick";
						packObjs = false;
						objReady = false;
					}
					else
					{
						
						objC++;
						
					}
					count = true;
				}


			}
		}

		switch(packScript.gameState){
		case "Intro":

			break;
		case "Controls":

			break;
		case "Instructions":

			if(!packScript.assignObj)
			if(!packScript.failedA)
			{
				objs = new string[packScript.aElementList.Count];
				for(int i = 0; i < packScript.aElementList.Count; i++)
				{
					if(lang == "en")
					{
						objs[i] = TranslateObjs(packScript.aElementList[i]);
					}
					else
					{
						objs[i] = packScript.aElementList[i];
					}

				}
				objReady = true;
			}
			else
			{
				objs = new string[packScript.bElementList.Count];
				for(int i = 0; i < packScript.bElementList.Count; i++)
				{
					if(lang == "en")
					{
						objs[i] = TranslateObjs(packScript.bElementList[i]);
					}
					else
					{
						objs[i] = packScript.bElementList[i];
					}

				}
				objReady = true;
			}
			break;
		case "PlayerPick":
			break;
		case "CamRot":
//			if(packScript.avatar.activeSelf)
//			{
//				packScript.avatar.SetActive(false);
//			}
			break;
		case "Results":
			break;
		case "WeatherConditions":
//			if(packScript.weather != "")
//			{
//				if(!dialog.activeSelf)
//				dialog.SetActive(true);
//				if(weatherText.text == "")
//				weatherText.text = packScript.weather;
//			}
//			else
//			{
//				dialog.SetActive(false);
//				dialog = null;
//				dialog = transform.FindChild("WeatherSpeech").gameObject;
//			}

			break;
		case "FinalInstructions":
			break;
		case "Scores":
			break;
		}
	}
	string TranslateObjs(string s)
	{

		switch(s){
		case "Alcancía":
			tempS = "Piggy Bank";
			break;
		case "Aletas":
			tempS = "Flippers";
			break;
		case "Carrito":
			tempS = "Toy car";
			break;
		case "Cinturon":
			tempS = "Belt";
			break;
		case "Consola":
			tempS = "Console";
			break;
		case "Cubo de Rubik":
			tempS = "Rubik's Cube";
			break;
		case "Camara":
			tempS = "Camera";
			break;
		case "Globo terráqueo":
			tempS = "Globe";
			break;
		case "Lentes":
			tempS = "Sun glasses";
			break;
		case "Linterna":
			tempS = "Flashlight";
			break;
		case "Pantalon":
			tempS = "Pants";
			break;
		case "Patineta":
			tempS = "Skateboard";
			break;
		case "Piano":
			tempS = "Piano";
			break;
		case "Pino":
			tempS = "Pin";
			break;
		case "Raqueta":
			tempS = "Tennis racket";
			break;
		case "Robot":
			tempS = "Robot";
			break;
		case "Sombrero":
			tempS = "Sailor hat";
			break;
		case "Tabla de Surf":
			tempS = "Surfboard";
			break;
		case "Tableta":
			tempS = "Tablet";
			break;
		case "Tambor":
			tempS = "Drum";
			break;
		case "Tiro al Blanco":
			tempS = "Bullseye";
			break;

		}
		return tempS;
	}
	void OnGUI()
	{

		screenScale = (float)Screen.height / (float)768;
		pauseText.fontSize = (int)(60 * screenScale);
		styleX.fontSize = (int) (40 * screenScale);
		advanceStyle.fontSize = (int) (30 * screenScale);
		label =  new Rect(Screen.width * .4f, Screen.height * .5f, 250, 100);
		switch(logicScript.miniGame){
		case "InputAge":

			if(ageInputScript.incorrectInput)
			{
				if(!soundPlayed)
				{
					soundPlayed = true;
					soundMng.PlaySound(37);
				}
				GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
				styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
				GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[0], styleX);
				styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
				GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[0], styleX);
				if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 180 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
				{
					soundPlayed = false;
					ageInputScript.incorrectInput = false;
				}
			}
			break;
		case "BuyTicket":
			
			break;
		case "Pack":
			switch(packScript.gameState){
			case "Intro":
				if(!soundPlayed)
				{
					soundPlayed = true;
					soundMng.PlaySound(0);
				}

				GUI.DrawTexture(new Rect(Screen.width * .5f  - 100 * screenScale, Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), bGBlue);
				styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
				GUI.Label(new Rect(Screen.width * .5f - 98 * screenScale, Screen.height * .5f - 198 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[1], styleX);
				styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
				GUI.Label(new Rect(Screen.width * .5f - 100 * screenScale , Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[1], styleX);
				if(GUI.Button(new Rect(Screen.width * .5f + 280 * screenScale, Screen.height * .5f + 300 * screenScale , 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
				{
					soundPlayed = false;
					packScript.gameState = "Controls";
					packScript.controlsState = "ClickTut";
				}
				break;
			case "Controls":
				switch(packScript.controlsState){
				case "ClickTut":
					if(!packScript.insRead)
					{
						if(mobile)
						{
//							soundMng.PlaySound(1);
							GUI.DrawTexture(new Rect(Screen.width * .5f  - 100 * screenScale, Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), bGBlue);
							styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
							GUI.Label(new Rect(Screen.width * .5f - 98 * screenScale , Screen.height * .5f - 198 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[2], styleX);
							styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
							GUI.Label(new Rect(Screen.width * .5f -100 * screenScale , Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[2], styleX);
						}
						else
						{
							if(!soundPlayed)
							{
								soundPlayed = true;
								soundMng.PlaySound(1);
							}

							GUI.DrawTexture(new Rect(Screen.width * .5f  - 100 * screenScale, Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), bGBlue);
							styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
							GUI.Label(new Rect(Screen.width * .5f - 98 * screenScale , Screen.height * .5f - 198 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[3], styleX);
							styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
							GUI.Label(new Rect(Screen.width * .5f -100 * screenScale , Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[3], styleX);
						}

						if(GUI.Button(new Rect(Screen.width * .5f + 280 * screenScale, Screen.height * .5f + 300 * screenScale , 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
						{
							soundPlayed = false;
							packScript.avatar.SetActive(false);
							packScript.insRead = true;
						}
					}

					break;
				case "PackTut":
					if(!packScript.insRead)
					{
						if(mobile)
						{
//							soundMng.PlaySound(2);
							GUI.DrawTexture(new Rect(Screen.width * .5f  - 100 * screenScale, Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), bGBlue);
							styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
							GUI.Label(new Rect(Screen.width * .5f - 98 * screenScale , Screen.height * .5f - 198 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[4], styleX);
							styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
							GUI.Label(new Rect(Screen.width * .5f -100 * screenScale , Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[4], styleX);
						}
						else
						{
							if(!soundPlayed)
							{
								soundPlayed = true;
								soundMng.PlaySound(2);
							}

							GUI.DrawTexture(new Rect(Screen.width * .5f  - 100 * screenScale, Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), bGBlue);
							styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
							GUI.Label(new Rect(Screen.width * .5f - 98 * screenScale , Screen.height * .5f - 198 * screenScale, 540 * screenScale, 520 * screenScale),language.levelStrings[5], styleX);
							styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
							GUI.Label(new Rect(Screen.width * .5f -100 * screenScale , Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale),language.levelStrings[5], styleX);
						}

						if(GUI.Button(new Rect(Screen.width * .5f + 280 * screenScale, Screen.height * .5f + 300 * screenScale , 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
						{
							soundPlayed = false;
							packScript.avatar.SetActive(false);
							packScript.insRead = true;


						}
					}
					break;
				}
				break;
			case "Instructions":
//				objReady = true;
				if(packObjs)
				if(!packScript.testDone)
				{
                    if(packScript.failedTest&&insDone)
                    {
                        packScript.failedTest = false;
                        insDone = false;
                        count = false;
                        soundPlayed = false;
                    }
                    if (!packScript.failedTest && packScript.tutorialError==2)
                    {
                        packScript.packOrderTutorial = false;
                        packScript.tutorialError = -1;
                        insDone = false;
                        count = false;
                        soundPlayed = false;
                    }
					if(packScript.reverse)
					{
						GUI.DrawTexture(new Rect(Screen.width * .5f  - 100 * screenScale, Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), bGBlue);
						if(!insDone)
						{
							if(!soundPlayed)
							{
								soundPlayed = true;
                                if (packScript.packOrderTutorial)
                                {
                                    switch (packScript.tutorialError)
                                    {
                                        case -1:
                                            soundMng.PlaySoundQueue(new int[] { 4, 46 });
                                            //soundMng.PlaySound(4);
                                            break;
                                        case 0:
                                            soundMng.PlaySound(43);
                                            break;
                                        case 1:
                                            soundMng.PlaySound(44);
                                            break;
                                    }
                                }
                                else
                                {
                                    soundMng.PlaySound(45);
                                }
							}

                            string dialogText = "";
                            if (packScript.packOrderTutorial)
                            {
                                switch (packScript.tutorialError)
                                {
                                    case -1:
									dialogText = language.levelStrings[6] + "\n¡Vamos a intentarlo!";
                                        break;
                                    case 0:
									dialogText = "¡Ups! Ese objeto no venía en la lista, pon atención a los objetos y empácalos de atrás para adelante. Volvamos a intentarlo.";
                                        break;
                                    case 1:
                                        dialogText = "¡Ups! Recuerda que los tienes que empacar de atrás para adelante. Volvamos a intentarlo.";
                                        break;
                                }
                            }else
                            {
                                dialogText = "Muy bien. Empecemos";
                            }

							styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
                            GUI.Label(new Rect(Screen.width * .5f - 98 * screenScale, Screen.height * .5f - 198 * screenScale, 540 * screenScale, 520 * screenScale), dialogText, styleX);
							styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
                            GUI.Label(new Rect(Screen.width * .5f - 100 * screenScale, Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), dialogText, styleX);
                            if (packScript.packOrderTutorial)
                            {
                                if (!soundMng.IsPlaying() && !soundMng.queuePlaying)
                                {
                                    if (GUI.Button(new Rect(Screen.width * .5f + 280 * screenScale, Screen.height * .5f + 300 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
                                    {
                                        count = true;
                                        soundPlayed = false;
                                        insDone = true;
                                        packScript.tutorialError = -1;
                                    }
                                }
                            }
                            else
                            {
                                if (!soundMng.IsPlaying())
                                {
                                    count = true;
                                    soundPlayed = false;
                                    insDone = true;
                                }
                            }

						}
						else
						{
							if(objReady)
							{
								if(count)
								{
									styleX.fontSize = (int) (60 * screenScale);
									styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
									GUI.Label(new Rect(Screen.width * .5f - 98 * screenScale , Screen.height * .5f - 288 * screenScale, 540 * screenScale, 520 * screenScale), "\n\n\n\n" + objs[objC] , styleX);
									styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
									GUI.Label(new Rect(Screen.width * .5f -100 * screenScale , Screen.height * .5f - 290 * screenScale, 540 * screenScale, 520 * screenScale), "\n\n\n\n" + objs[objC] , styleX);
									styleX.fontSize = (int) (40 * screenScale);								
								}
								
							}
						}

					}
					else
					{
						GUI.DrawTexture(new Rect(Screen.width * .5f  - 100 * screenScale, Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), bGBlue);
						if(!insDone)
						{
							if(!soundPlayed)
							{
								soundPlayed = true;
                                if (packScript.packOrderTutorial)
                                {
                                    switch (packScript.tutorialError)
                                    {
                                        case -1:
                                            soundMng.PlaySoundQueue(new int[]{3, 46});
                                            //soundMng.PlaySound(3);
                                            break;
                                        case 0:
                                            soundMng.PlaySound(41);
                                            break;
                                        case 1:
                                            soundMng.PlaySound(42);
                                            break;
                                    }
                                }
                                else
                                {
                                    soundMng.PlaySound(45);
                                }
							}

                            string dialogText = "";
                            if (packScript.packOrderTutorial)
                            {
                                switch (packScript.tutorialError)
                                {
                                    case -1:
										dialogText = language.levelStrings[7] + "\n¡Vamos a intentarlo!";
                                        break;
                                    case 0:
									dialogText = "Ese objeto no venía en la lista, pon atención a los objetos y empácalos en orden.\nVolvamos a intentarlo.";
                                        break;
                                    case 1:
									dialogText = "¡Ups! Recuerda que los tienes que empacar en el mismo orden en que los escuchaste.\nVolvamos a intentarlo.";
                                        break;
                                }
                            }
                            else
                            {
                                dialogText = "Muy bien. Empecemos";
                            }

                            styleX.normal.textColor = new Color(0.32f, 0.32f, 0.32f);
                            GUI.Label(new Rect(Screen.width * .5f - 98 * screenScale, Screen.height * .5f - 198 * screenScale, 540 * screenScale, 520 * screenScale), dialogText, styleX);
                            styleX.normal.textColor = new Color(0.88f, 0.88f, 0.88f);
                            GUI.Label(new Rect(Screen.width * .5f - 100 * screenScale, Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), dialogText, styleX);
                            if (packScript.packOrderTutorial)
                            {
                                if (!soundMng.IsPlaying() && !soundMng.queuePlaying)
                                {
                                    if (GUI.Button(new Rect(Screen.width * .5f + 280 * screenScale, Screen.height * .5f + 300 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
                                    {
                                        count = true;
                                        soundPlayed = false;
                                        insDone = true;
                                        packScript.tutorialError = -1;
                                    }
                                }
                            }
                            else
                            {
                                if (!soundMng.IsPlaying())
                                {
                                    count = true;
                                    soundPlayed = false;
                                    insDone = true;
                                }
                            }

						}
						else
						{
							if(objReady)
							{

								if(count)
								{
									styleX.fontSize = (int) (60 * screenScale);
									styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
									GUI.Label(new Rect(Screen.width * .5f - 98 * screenScale , Screen.height * .5f - 288 * screenScale, 540 * screenScale, 520 * screenScale), "\n\n\n\n" + objs[objC] , styleX);
									styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
									GUI.Label(new Rect(Screen.width * .5f -100 * screenScale , Screen.height * .5f - 290 * screenScale, 540 * screenScale, 520 * screenScale), "\n\n\n\n" + objs[objC] , styleX);
									styleX.fontSize = (int) (40 * screenScale);
								}

							}
						}

					}
				}
				break;
			case "CamRot":
                packScript.packOrderTutorial = true;
				objReady = false;
				insDone = false;
				count = false;
				soundPlayed = false;

				break;
			case "PlayerPick":
				if(packScript.weatherObjPick)
				{
					if(GUI.Button(new Rect(Screen.width * .5f + 280 * screenScale, Screen.height * .5f + 300 * screenScale , 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
					{
						soundPlayed = false;
						packScript.curObjSelected = null;
//						Camera.main.transform.rotation = Quaternion.identity;
						packScript.camControlScript.rotationX = 0f;
						packScript.camControlScript.rotationY = 0f;
						packScript.UIScript.advanceBut.SetActive(false);
						packScript.gameState = "FinalInstructions";
						finalInsState = "SetUp";
					}
				}else
                {
                    if (!packScript.packOrderTutorial)
                    {
                        if (GUI.Button(new Rect(Screen.width * .5f + 280 * screenScale, Screen.height * .5f + 300 * screenScale, 200 * screenScale, 80 * screenScale), "Ya terminé", advanceStyle))
                        {
                            packScript.ReadyPick();
                        }
                    }
                }

				break;
			case "WeatherConditions":
				if(packScript.weather != "")
				{
					if(!soundPlayed)
					{
						soundPlayed = true;

						switch (packScript.weather) {
						case "frio":
							int[] sounds = new int[]{59, 27, 50};
							soundMng.PlaySoundQueue(sounds);
							break;
						case "caliente":
							int[] soundsC = new int[]{59, 26, 50};
							soundMng.PlaySoundQueue(soundsC);
							break;
						case "lluvioso":
							int[] soundsL = new int[]{59, 28, 50};
							soundMng.PlaySoundQueue(soundsL);
							break;


						}
						/*for(int i = 0; i < soundMng.sounds.Length; i++)
						{
							if(soundMng.sounds[i].name == packScript.weather)
							{
								int[] sounds = new int[]{5, i, 50};
								soundMng.PlaySoundQueue(sounds);
								break;
							}
						}*/
					}

					GUI.DrawTexture(new Rect(Screen.width * .5f  - 100 * screenScale, Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), bGBlue);
					styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
                    GUI.Label(new Rect(Screen.width * .5f - 98 * screenScale, Screen.height * .5f - 198 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[8] + packScript.weather + "\n¿Necesitas algo?", styleX);
					styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * .5f -100 * screenScale , Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[8] + packScript.weather + "\n¿Necesitas algo?", styleX);

					if(packScript.goToPick)
						if(GUI.Button(new Rect(Screen.width * .5f + 280 * screenScale, Screen.height * .5f + 300 * screenScale , 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
					{
						packScript.avatar.SetActive(false);
						packScript.weatherObjPick = true;
						packScript.gameState = "PlayerPick";
						packScript.goToPick = false;
						soundPlayed = false;
					}
				}

				break;
			case "FinalInstructions":

				switch(finalInsState){
				case "SetUp":
					if(!packScript.avatar.activeSelf)
					{
						packScript.avatar.SetActive(true);
						finalInsState = "Ins";
						break;
					}
					break;
				case "Ins":
					if(!soundPlayed)
					{
						soundPlayed = true;
						if(finalInsCount == 0)
						{
							soundMng.PlaySound(6);
						}
						else
						{
							soundMng.PlaySound(40);
						}

					}
					if(finalInsCount == 0)
					{
						GUI.DrawTexture(new Rect(Screen.width * .5f  - 100 * screenScale, Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), bGBlue);
						styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
						GUI.Label(new Rect(Screen.width * .5f - 98 * screenScale , Screen.height * .5f - 198 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[9] + " \n", styleX);
						styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
						GUI.Label(new Rect(Screen.width * .5f -100 * screenScale , Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[9] + " \n", styleX);
					}
					else
					{
						GUI.DrawTexture(new Rect(Screen.width * .5f  - 100 * screenScale, Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), bGBlue);
						styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
						GUI.Label(new Rect(Screen.width * .5f - 98 * screenScale , Screen.height * .5f - 198 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[10], styleX);
						styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
						GUI.Label(new Rect(Screen.width * .5f -100 * screenScale , Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[10], styleX);
					}

					if(!soundMng.IsPlaying())
					{
						soundPlayed = false;
						finalInsState = "Obj";
						objCount = 1;
						break;
					}
					break;
				case "Obj":
					if(unPackObj1 != "" && unPackObj2 != "" && unPackObj3 != "")
					{
						if(finalInsCount < 3)
						{
							switch(objCount){
							case 1:
								if(!soundPlayed)
								{
									soundPlayed = true;
									soundMng.PlaySound(15);
								}
								GUI.DrawTexture(new Rect(Screen.width * .5f  - 100 * screenScale, Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), bGBlue);
								styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
								GUI.Label(new Rect(Screen.width * .5f - 98 * screenScale , Screen.height * .5f - 198 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[38], styleX);
								styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
								GUI.Label(new Rect(Screen.width * .5f -100 * screenScale , Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[38], styleX);
								if(!soundMng.IsPlaying())
								{
									soundPlayed = false;
									objCount = 2;
									break;
								}
								break;
							case 2:
								if(!soundPlayed)
								{
									soundPlayed = true;
									soundMng.PlaySound(9);
								}
								GUI.DrawTexture(new Rect(Screen.width * .5f  - 100 * screenScale, Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), bGBlue);
								styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
								GUI.Label(new Rect(Screen.width * .5f - 98 * screenScale , Screen.height * .5f - 198 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[23], styleX);
								styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
								GUI.Label(new Rect(Screen.width * .5f -100 * screenScale , Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[23], styleX);
								if(!soundMng.IsPlaying())
								{
									soundPlayed = false;
									objCount = 3;
									break;
								}
								break;
							case 3:
								if(!soundPlayed)
								{
									soundPlayed = true;
									soundMng.PlaySound(12);
								}
								GUI.DrawTexture(new Rect(Screen.width * .5f  - 100 * screenScale, Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), bGBlue);
								styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
								GUI.Label(new Rect(Screen.width * .5f - 98 * screenScale , Screen.height * .5f - 198 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[36], styleX);
								styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
								GUI.Label(new Rect(Screen.width * .5f -100 * screenScale , Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[36], styleX);
								if(!soundMng.IsPlaying())
								{
									soundPlayed = false;
									finalInsCount++;
									if(finalInsCount < 3)
									{
										finalInsState = "Ins";
									}
									else
									{
										finalInsState = "Ready";
									}
									break;
								}
								break;

							}
						}
					}
					break;
				case "Ready":
					GUI.DrawTexture(new Rect(Screen.width * .5f  - 100 * screenScale, Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), bGBlue);
					styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * .5f - 98 * screenScale , Screen.height * .5f - 198 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[9] + " \n" + unPackObj1 + "\n" + unPackObj2 + "\n" + unPackObj3, styleX);
					styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * .5f -100 * screenScale , Screen.height * .5f - 200 * screenScale, 540 * screenScale, 520 * screenScale), language.levelStrings[9] + " \n" + unPackObj1 + "\n" + unPackObj2 + "\n" + unPackObj3, styleX);
					if(GUI.Button(new Rect(Screen.width * .5f + 280 * screenScale, Screen.height * .5f + 300 * screenScale , 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
					{
						soundPlayed = false;
						packScript.avatar.SetActive(false);
						packScript.camAnim.enabled = false;
						packScript.logicScript.curGameFinished = true;
					}
					break;
				}



				break;
			case "Scores":
				break;
			}
			break;
		case "PlanRoute":
			switch(routeLogic.state){
			case "Intro":
				if(!soundPlayed)
				{
					soundPlayed = true;
					soundMng.PlaySound(29);
				}
				GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
				styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
				GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[11], styleX);
				styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
				GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[11], styleX);

				if (!soundMng.IsPlaying() && !soundMng.queuePlaying)
				{
					if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 180 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
					{
						soundPlayed = false;
						routeLogic.state = "Lab1";
					}
				}
				break;
			}
			break;
		case "WaitingRoom":
			switch(waitScript.state){
			case "Intro":
				if(!soundPlayed)
				{
					soundPlayed = true;
					soundMng.PlaySound(30);
				}
				GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
				styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
				GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[12], styleX);
				styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
				GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[12], styleX);
				if(!soundMng.IsPlaying())
				{
					if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 180 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
					{
						soundPlayed = false;
                        waitScript.StartTutorial();
                        //waitScript.state = "Tutorial";
						//waitScript.state = "GiveNum";
					}
				}

				break;
                case "Tutorial":
                    if(waitScript.tutorialDone)
                    {
                        GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
                        switch (waitScript.tutorialError)
                        {
                            case -1:
                                if (!soundPlayed)
                                {
                                    soundPlayed = true;
                                    soundMng.PlaySound(45);
                                }
                                styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
				                GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), "Muy bien, empecemos.", styleX);
				                styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
                                GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), "Muy bien, empecemos.", styleX);
								
								if (!soundMng.IsPlaying() && !soundMng.queuePlaying){
										if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 180 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
							            {
								            //soundPlayed = false;
		                                    soundPlayed = false;
		                                    waitScript.StartGame();
								            //waitScript.state = "GiveNum";
							            }
								}
                                break;
                            case 0:
                                if (!soundPlayed)
                                {
                                    soundPlayed = true;
                                    soundMng.PlaySound(47);
                                }
                                styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
                                GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), "¡Cuidado! Olvidaste hacer clic cuando el vuelo empezaba con KW. Volvamos a intentarlo.", styleX);
				                styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
                                GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), "¡Cuidado! Olvidaste hacer clic cuando el vuelo empezaba con KW. Volvamos a intentarlo.", styleX);
								
								if (!soundMng.IsPlaying() && !soundMng.queuePlaying){
									if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 180 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
						            {
							            //soundPlayed = false;
	                                    soundPlayed = false;
	                                    waitScript.StartTutorial();
							            //waitScript.state = "GiveNum";
						            }
								}
                                break;
                            case 1:
                                if (!soundPlayed)
                                {
                                    soundPlayed = true;
                                    soundMng.PlaySound(48);
                                }
                                styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
				                GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), "¡Ups! Este vuelo no empieza con KW. Volvamos a intentarlo.", styleX);
				                styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
                                GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), "¡Ups! Este vuelo no empieza con KW. Volvamos a intentarlo.", styleX);
								if (!soundMng.IsPlaying() && !soundMng.queuePlaying){
									if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 180 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
						            {
							            //soundPlayed = false;
	                                    soundPlayed = false;
	                                    waitScript.StartTutorial();
							            //waitScript.state = "GiveNum";
						            }
								}
                                break;
                            case 2:
                                if (!soundPlayed)
                                {
                                    soundPlayed = true;
                                    soundMng.PlaySound(49);
                                }
                                styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
								GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 810 * screenScale, 300 * screenScale), "Recuerda hacer un solo clic cuando el vuelo comience con KW. Volvamos a intentarlo", styleX);
				                styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
                                GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 810 * screenScale, 300 * screenScale), "Recuerda hacer un solo clic cuando el vuelo comience con KW. Volvamos a intentarlo", styleX);
								if (!soundMng.IsPlaying() && !soundMng.queuePlaying){
									if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 180 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
						            {
							            //soundPlayed = false;
	                                    soundPlayed = false;
	                                    waitScript.StartTutorial();
							            //waitScript.state = "GiveNum";
						            }
								}
                                break;
                        }
                    }
                break;
			}
		break;

		case "FlyPlane":
			switch(flyScript.state){
				
			case "Intro":
				if(!soundPlayed)
				{
					soundPlayed = true;
					soundMng.PlaySound(31);
				}
				if(mobile)
				{
					GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
					styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[13], styleX);
					styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[13], styleX);
				}
				else 
				{
					GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
					styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[14], styleX);
					styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[14], styleX);
				}
				
				if (!soundMng.IsPlaying() && !soundMng.queuePlaying)
				{
					if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 180 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
					{
						soundPlayed = false;
						flyScript.StartTutorial ();
						flyScript.state = "SetArrowTutorial"; //flyScript.state = "SetArrow";
					}
				}
				break;
			
			case "TutorialError0":
				if(!soundPlayed)
				{
					soundPlayed = true;
					soundMng.PlaySound(51);
				}
				if(mobile)
				{
					GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
					styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[13], styleX);
					styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[13], styleX);
				}
				else 
				{
					GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
					styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[47], styleX);
					styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[47], styleX);
				}

				if (!soundMng.IsPlaying() && !soundMng.queuePlaying)
				{
					if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 180 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
					{
						soundPlayed = false;
						flyScript.StartTutorial ();
						flyScript.state = "SetArrowTutorial"; //flyScript.state = "SetArrow";
					}
				}
			break;
			
			case "TutorialError1":
				if(!soundPlayed)
				{
					soundPlayed = true;
					soundMng.PlaySound(53);
				}
				if(mobile)
				{
					GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
					styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[13], styleX);
					styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[13], styleX);
				}
				else 
				{
					GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
					styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[48], styleX);
					styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[48], styleX);
				}

				if (!soundMng.IsPlaying() && !soundMng.queuePlaying)
				{
					if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 180 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
					{
						soundPlayed = false;
						flyScript.StartTutorial ();
						flyScript.state = "SetArrowTutorial"; //flyScript.state = "SetArrow";
					}
				}
				break;
			case "TutorialError2":
				if(!soundPlayed)
				{
					soundPlayed = true;
					soundMng.PlaySound(52);
				}
				if(mobile)
				{
					GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
					styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[13], styleX);
					styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[13], styleX);
				}
				else 
				{
					GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
					styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[49], styleX);
					styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[49], styleX);
				}

				if (!soundMng.IsPlaying() && !soundMng.queuePlaying)
				{
					if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 180 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
					{
						soundPlayed = false;
						flyScript.StartTutorial ();
						flyScript.state = "SetArrowTutorial"; //flyScript.state = "SetArrow";
					}
				}
				break;

			case "TutorialError3":
				if(!soundPlayed)
				{
					soundPlayed = true;
					soundMng.PlaySound(52);
				}
				if(mobile)
				{
					GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
					styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[13], styleX);
					styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[13], styleX);
				}
				else 
				{
					GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
					styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[50], styleX);
					styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[50], styleX);
				}

				if (!soundMng.IsPlaying() && !soundMng.queuePlaying)
				{
					if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 180 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
					{
						soundPlayed = false;
						flyScript.StartTutorial ();
						flyScript.state = "SetArrowTutorial"; //flyScript.state = "SetArrow";
					}
				}
			break;

			case "TutorialEnd":
				if(!soundPlayed)
				{
					soundPlayed = true;
					soundMng.PlaySound(60);
				}
				if(mobile)
				{
					GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
					styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[51], styleX);
					styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[51], styleX);
				}
				else 
				{
					GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
					styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[51], styleX);
					styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[51], styleX);
				}

				if (!soundMng.IsPlaying() && !soundMng.queuePlaying)
				{
					if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 180 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
					{
						soundPlayed = false;
						flyScript.state = "SetArrow"; //flyScript.state = "SetArrow";
					}
				}
				break;
			


			case "Arrive":
				if(!soundPlayed)
				{
					soundPlayed = true;
					soundMng.PlaySound(32);
				}
				GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
				styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
				GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[15], styleX);
				styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
				GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[15], styleX);
				if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 180 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
				{
					soundPlayed = false;
					flyScript.state = "End";
				}
				break;
			}
			break;
		case "PickUpCoins":
			switch(pickUpScript.state){
			case "Intro":
				if(!soundPlayed)
				{
					soundPlayed = true;
					soundMng.PlaySound(33);
				}
				GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
				GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
				GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
				GUI.DrawTexture(new Rect(Screen.width * .5f - 45 * screenScale, Screen.height * .5f + 100 * screenScale, 85 * screenScale, 85 * screenScale), moneda);
				styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
				GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[16], styleX);
				styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
				GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[16], styleX);

				if (!soundMng.IsPlaying() && !soundMng.queuePlaying){
					if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 180 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
					{
						soundPlayed = false;
						pickUpScript.state = "PickUpPhaseInTime";
					}
				}

				break;
			case "PickUpPhaseInTime":
				if(GUI.Button(new Rect(Screen.width - 160 * screenScale, Screen.height - 40 * screenScale, 160 * screenScale, 40 * screenScale), language.levelStrings[45], advanceStyle))
				{
					if(!pickUpScript.overMin)
					{
						pickUpScript.beforeMinClick++;
						pickUpScript.state = "PausePhase";
						Time.timeScale = 0f;
					}
					else
					{
						pickUpScript.state = "End";
						pickUpScript.extraMissed = pickUpScript.minuteMissed - pickUpScript.extraCorrect;
					}

				}


				break;
			case "PausePhase":

				GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
				styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
				GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[17], styleX);
				styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
				GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[17], styleX);
				if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 180 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
				{
					Time.timeScale = 1f;
					pickUpScript.state = "PickUpPhaseInTime";

				}
				break;
			case "PickUpPhaseExtra":
				if(GUI.Button(new Rect(Screen.width - 160 * screenScale, Screen.height - 80 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
				{
					pickUpScript.state = "End";
					pickUpScript.extraMissed = pickUpScript.minuteMissed - pickUpScript.extraCorrect;
				}
				break;
			}
			break;
		case "UnPack":

			switch(unPackScript.state){
			case "FirstObjectsIntro":
				if(!soundPlayed)
				{
					soundPlayed = true;
					soundMng.PlaySound(34);
				}
				GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
				styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
				GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[18], styleX);
				styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
				GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[18], styleX);

				if (!soundMng.IsPlaying() && !soundMng.queuePlaying){
					if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 180 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
					{
						soundPlayed = false;
						unPackScript.state = "UnPackFirstObjects";
					}
				}

				break;

			case "UnPackFirstObjects":
				if(GUI.Button(new Rect(Screen.width * .5f + 300 * screenScale, Screen.height * .5f - 375 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
				{
					soundPlayed = false;
					unPackScript.state = "Intro";
				}
				break;

			case "Intro":
				if(!soundPlayed)
				{
					soundPlayed = true;
					soundMng.PlaySound(35);
				}
				GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
				styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
				GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[19], styleX);
				styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
				GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[19], styleX);
				if (!soundMng.IsPlaying() && !soundMng.queuePlaying){
					if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 180 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
					{
						soundPlayed = false;
						unPackScript.state = "ShowImage";
					}
				}
				break;
			case "UnPack":
				if(GUI.Button(new Rect(Screen.width * .5f + 300 * screenScale, Screen.height * .5f - 375 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
				{
					unPackScript.advance = true;
				}
				break;
			case "InsAgain":
				if(unPackScript.essay < 4)
				{
					if(!soundPlayed)
					{
						soundPlayed = true;
						soundMng.PlaySound(36);
					}
					GUI.DrawTexture(new Rect(Screen.width * .5f - 500 * screenScale, Screen.height * .5f - 200 * screenScale, 1000 * screenScale, 400 * screenScale), bGBlue);
					styleX.normal.textColor = new Color(0.32f,0.32f,0.32f);
					GUI.Label(new Rect(Screen.width * .5f - 398 * screenScale, Screen.height * .5f - 148 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[20], styleX);
					styleX.normal.textColor = new Color(0.88f,0.88f,0.88f);
					GUI.Label(new Rect(Screen.width * .5f - 400 * screenScale, Screen.height * .5f - 150 * screenScale, 800 * screenScale, 300 * screenScale), language.levelStrings[20], styleX);
					
					if(!unPackScript.determine)
					{
						if(unPackScript.essay < 4)
						{
							if(GUI.Button(new Rect(Screen.width * .5f + 340 * screenScale, Screen.height * .5f + 180 * screenScale, 160 * screenScale, 80 * screenScale), language.levelStrings[45], advanceStyle))
							{
								soundPlayed = false;
								unPackScript.state = "ShowImage";
							}
						}
					}


				}

				break;
			}
			break;
		case "Pause":
//			float pauseY = Screen.height * 0.5f - 177 * screenScale;
//			GUI.DrawTexture(new Rect(0, pauseY, Screen.width, 354 * screenScale), pauseBackground);
//			GUI.Label(new Rect(Screen.width * 0.5f - 100 * screenScale, pauseY - 40 * screenScale, 200 * screenScale, 60 * screenScale), language.levelStrings[46], pauseText);
//			if(GUI.Button(new Rect(Screen.width * 0.5f - 200 * screenScale, pauseY + 50 * screenScale, 366 * screenScale, 66 * screenScale), "", pauseContinue))
//			{
//				Time.timeScale = 1f;
//				logicScript.miniGame = stateBeforePause;
//			}
//			else if(GUI.Button(new Rect(Screen.width * 0.5f - 200 * screenScale, pauseY + 230 * screenScale, 162 * screenScale, 67 * screenScale), "", pauseExit))
//			{
//				Time.timeScale = 1.0f;
//				Application.Quit();
//			}

			break;
		case "Island":
			break;
		}
//		if(logicScript.miniGame != "Pause" && logicScript.miniGame != "FadeOut" && logicScript.miniGame != "FadeIn")
//		{
//			if(GUI.Button(new Rect(10 * screenScale, 10 * screenScale, 71 * screenScale, 62 * screenScale), "", pauseButton))
//			{
//				Time.timeScale = 0f;
//				stateBeforePause = logicScript.miniGame;
//				logicScript.miniGame = "Pause";
//			}
//		}
	}


}
