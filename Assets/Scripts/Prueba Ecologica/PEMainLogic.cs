using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PEMainLogic : MonoBehaviour
{
	//Script variables
	MetricsProcessing metScript;
	GameSaveLoad loader;
	ProgressHandler saveHandler;
	public FadeInOut fadeScript;
	public IntroGamesConfiguration configuration;
	PackLogic packScript;
	AirportRouteLogic routeLogic;
	FlyPlaneLogic flyLogic;
	WaitingLogic waitLogic;
	PickUpCoinsLogic coinsLogic;
	UnPackLogic unPackLog;
	FirstObj firstunPackLog;


	//logic variables
	public string miniGame;
	public bool curGameFinished = false;
	public string nextGame;
	public float timeOfFade;
	public int minAge1 = 4;
	public int maxAge1 = 6;
	public int minAge2 = 7;
	public int maxAge2 = 9;
	public int minAge3 = 10;
	public int maxAge3 = 12;


	//info input vars


	//...........pack game vars
	public List<GameObject> tierUsed = new List<GameObject>();
	public List<GameObject> weatherTierUsed = new List<GameObject>();
	public GameObject[] sPoints;
	public int numObjPerRound;
	public string[] oTier1;
	public string[] oTier2;
	public string[] oTier3;
	public string[] oWTier1;
	public string[] oWTier2;
	public string[] oWTier3;

	//unpack vars
	public List<GameObject> unPObj = new List<GameObject>();
	
	
	//data variables
	public int ageOfPlayer;
	public string nameOfPlayer;
	public string addressOfPlayer;
	public string birthdayOfPlayer;
	public string currentDate;
	float testSectionTime;
	float totalTestTime;

	float lat;




	// Use this for initialization
	void Start ()
	{
		metScript = GameObject.Find("Metrics").GetComponent<MetricsProcessing>();
		fadeScript = Camera.main.GetComponent<FadeInOut>();
		packScript = GameObject.Find("Pack").GetComponent<PackLogic>();
		routeLogic = GameObject.Find("AirportRoute").GetComponent<AirportRouteLogic>();
		flyLogic = GameObject.Find("FlyPlane").GetComponent<FlyPlaneLogic>();
		waitLogic = GameObject.Find("WaitingRoom").GetComponent<WaitingLogic>();
		coinsLogic = GameObject.Find("PickUpCoins").GetComponent<PickUpCoinsLogic>();
		unPackLog = GameObject.Find("UnPack").GetComponent<UnPackLogic>();
        saveHandler = GetComponent<ProgressHandler>();	
		sPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

		loader = GetComponent<GameSaveLoad>();
		loader.Load(GameSaveLoad.game.introGames);
		configuration = (IntroGamesConfiguration)loader.configuration;

		Application.targetFrameRate = 60;
		miniGame = "FadeOut";
		nextGame = "InputAge";//nextGame = "InputAge";
		
		ageOfPlayer = 0;
		nameOfPlayer = "0";
		addressOfPlayer = "0";
		birthdayOfPlayer = "0";
		currentDate = "0";
		SetLevel();
		Camera.main.orthographic = true;
		
	}
	
	// Update is called once per frame
	void Update ()
	{

		totalTestTime += Time.deltaTime;

		switch(miniGame){
		case "InputAge":

			testSectionTime += Time.deltaTime;
			if(curGameFinished)
			{

				nextGame = "BuyTicket";	
				curGameFinished = false;
				SaveLevelProgress();
				miniGame = "BuyTicket";

			}
			break;	
		case "BuyTicket":
			testSectionTime += Time.deltaTime;
			if(curGameFinished)
			{
				nextGame = "Pack";
				curGameFinished = false;
				SaveLevelProgress();
				miniGame = "FadeOut";
				packScript.gameState = "Intro";
			}
			break;
		case "Pack":
			testSectionTime += Time.deltaTime;
			if(curGameFinished)
			{
				nextGame = "PlanRoute";
				curGameFinished = false;
				SaveLevelProgress();
				miniGame = "FadeOut";
			}
			else
			{
				
			}
			break;
		case "PlanRoute":
			testSectionTime += Time.deltaTime;
			if(curGameFinished)
			{
				nextGame = "WaitingRoom";
				curGameFinished = false;
				SaveLevelProgress();
				miniGame = "FadeOut";
			}
			else
			{
				if(routeLogic.state == "")
				{
					routeLogic.state = "Intro";
				}
			}
			break;
		case "WaitingRoom":
			testSectionTime += Time.deltaTime;
			if(curGameFinished)
			{
				nextGame = "FlyPlane";
				curGameFinished = false;
				SaveLevelProgress();
				miniGame = "FadeOut";
			}
			break;
		case "FlyPlane":
			testSectionTime += Time.deltaTime;
			if(curGameFinished)
			{
				nextGame = "PickUpCoins";
				curGameFinished = false;
				SaveLevelProgress();
				miniGame = "FadeOut";
			}
			break;
		case "PickUpCoins":
			testSectionTime += Time.deltaTime;
			if(curGameFinished)
			{
				nextGame = "UnPack";
				curGameFinished = false;
				SaveLevelProgress();
				miniGame = "FadeOut";
			}
			break;
		case "UnPack":
			testSectionTime += Time.deltaTime;
			if(curGameFinished)
			{
				nextGame = "End";
				curGameFinished = false;
				SaveLevelProgress();
				miniGame = "FadeOut";
			}
			else
			{
				
			}
			break;
		case "FadeIn":
			
			if(fadeScript.mat.color.a <= 0f)
			{
				
				miniGame = nextGame;
				break;
			}
			if(!fadeScript.fade && fadeScript.mat.color.a >= 1f)
			{
				fadeScript.FadeFunc(true, timeOfFade);
			}
			break;
		case "FadeOut":
			
			if(!fadeScript.fade && fadeScript.mat.color.a <= 0f)
			{
				fadeScript.FadeFunc(false, timeOfFade);
			}
			
			break;
		case "End":
            miniGame = "Default";
			metScript.Process();
			SaveProgress();
			saveHandler.PostProgress(false);
			break;
		case "Default":
            if(!saveHandler.saving)
            {
                Application.LoadLevel("Login");
            }
            break;
		}
	}
	void SaveLevelProgress()
	{
		//totalTestTime += testSectionTime; Se estaba duplicando el tiempo ya que en update tambien se actualiza la variable totalTestTime
		switch(miniGame){
		case "InputAge":
			saveHandler.AddLevelData("playerAge", ageOfPlayer);
			saveHandler.AddLevelData("playerBirth", birthdayOfPlayer);
			saveHandler.AddLevelData("InputAgeTimeOfComp", (int)testSectionTime);
			testSectionTime = 0;
			//saveHandler.SetLevel();
			break;	
		case "BuyTicket":
			/*if(ageOfPlayer <= maxAge1)
			{
				saveHandler.AddLevelData("playerName", nameOfPlayer);
			}
			else if(ageOfPlayer > maxAge1 && ageOfPlayer <= maxAge2)
			{
				saveHandler.AddLevelData("playerName", nameOfPlayer);
				saveHandler.AddLevelData("playerAddress", addressOfPlayer);
			}
			else if(ageOfPlayer > maxAge2)
			{
				saveHandler.AddLevelData("playerName", nameOfPlayer);
				saveHandler.AddLevelData("playerAddress", addressOfPlayer);
				saveHandler.AddLevelData("currentDate", currentDate);
			}*/
			saveHandler.AddLevelData("playerName", nameOfPlayer);
			saveHandler.AddLevelData("playerAddress", addressOfPlayer);
			saveHandler.AddLevelData("currentDate", currentDate);

			saveHandler.AddLevelData("buyTicketTimeOfComp", (int)testSectionTime);
			testSectionTime = 0;
			//saveHandler.SetLevel();
			break;
		case "Pack":
			saveHandler.AddLevelData("normalPackScore", packScript.normListCount-1);
			saveHandler.AddLevelData("reversePackScore", packScript.reverseListCount-1);
			saveHandler.AddLevelData("packTimeOfComp", (int)testSectionTime);
			saveHandler.AddLevelData("packTypeOfWeather", packScript.weather);
			saveHandler.AddLevelData("weatherObjectPacked", packScript.weatherObjPacked);

			saveHandler.AddLevelData("objectToRemember", packScript.unPackObj);
			metScript.values[6] = packScript.reverseAElementList.Count;
			metScript.values[7] = packScript.normListCount;


		
			testSectionTime = 0;
			//saveHandler.SetLevel();
			break;
		case "PlanRoute":
			saveHandler.AddLevelData ("timeOfLab1", routeLogic.times [0]);
			saveHandler.AddLevelData ("timeOfLab2", routeLogic.times [1]);
			saveHandler.AddLevelData ("timeOfLab3", routeLogic.times [2]);
			saveHandler.AddLevelData ("latenciesOfLab1", routeLogic.latency [0]);
			saveHandler.AddLevelData ("latenciesOfLab2", routeLogic.latency [1]);
			saveHandler.AddLevelData ("latenciesOfLab3", routeLogic.latency [2]);
			saveHandler.AddLevelData ("hitsOfLab1", routeLogic.hits [0]);
			saveHandler.AddLevelData ("hitsOfLab2", routeLogic.hits [1]);
			saveHandler.AddLevelData ("hitsOfLab3", routeLogic.hits [2]);
			int totalHitsLabs = routeLogic.hits [0] + routeLogic.hits [1] + routeLogic.hits [2];
			saveHandler.AddLevelData ("totalHits", totalHitsLabs);
			saveHandler.AddLevelData ("XHits", (float)totalHitsLabs / 3);
			saveHandler.AddLevelData ("crossesOfLab1", routeLogic.crosses [0]);
			saveHandler.AddLevelData ("crossesOfLab2", routeLogic.crosses [1]);
			saveHandler.AddLevelData ("crossesOfLab3", routeLogic.crosses [2]);
			int totalCrossesLabs = routeLogic.crosses [0] + routeLogic.crosses [1] + routeLogic.crosses [2];
			saveHandler.AddLevelData ("totalCrosses", totalCrossesLabs);
			saveHandler.AddLevelData ("XCrosses", (float) totalCrossesLabs / 3);
			saveHandler.AddLevelData ("deadEndsOfLab1", routeLogic.deadEnds [0]);
			saveHandler.AddLevelData ("deadEndsOfLab2", routeLogic.deadEnds [1]);
			saveHandler.AddLevelData ("deadEndsOfLab3", routeLogic.deadEnds [2]);
			int totalDeadEndsLabs = routeLogic.deadEnds [0] + routeLogic.deadEnds [1] + routeLogic.deadEnds [2];
			saveHandler.AddLevelData("totalDeadEnds", totalDeadEndsLabs);
			saveHandler.AddLevelData("XDeadEnds", (float) totalDeadEndsLabs/3);
			saveHandler.AddLevelData("airportRouteTimeOfComp", (int)testSectionTime);

			metScript.values[3] = routeLogic.deadEnds[0] + routeLogic.deadEnds[1] + routeLogic.deadEnds[2];
			metScript.values[4] = routeLogic.crosses[0] + routeLogic.crosses[1] + routeLogic.crosses[2];
			metScript.values[5] = testSectionTime;
			testSectionTime = 0;
			//saveHandler.SetLevel();
			break;
		case "WaitingRoom":
			saveHandler.AddLevelData("waitRoomCorrect", waitLogic.correct);
			saveHandler.AddLevelData("waitRoomIncorrect", waitLogic.incorrect);
			saveHandler.AddLevelData("waitRoomMissed", waitLogic.missed);
			saveHandler.AddLevelData("timeBetweenFlights", waitLogic.timeBetweenCalls);
			metScript.values[9] = waitLogic.correct;

			testSectionTime = 0;
			//saveHandler.SetLevel();
			break;
		case "FlyPlane":
			saveHandler.AddLevelData("flyPlaneCorrect", flyLogic.correct);
			saveHandler.AddLevelData("flyPlaneIncorrect", flyLogic.incorrect);
			saveHandler.AddLevelData("flyPlaneMissed", flyLogic.missed);
			saveHandler.AddLevelData("flyPlaneCorrectGreen", flyLogic.correctGreen);
			saveHandler.AddLevelData("flyPlaneIncorrectGreen", flyLogic.incorrectGreen);
			saveHandler.AddLevelData("flyPlaneMissedGreen", flyLogic.missedGreen);
			saveHandler.AddLevelData("flyPlaneTimeforInput", flyLogic.timeForInput);
			saveHandler.AddLevelData("flyPlaneTimeOfComp", (int)testSectionTime);
			saveHandler.AddLevelData("InputLatency", flyLogic.latency);
			int perc = flyLogic.correct * 100;
			perc = (int)perc / 15;

			metScript.values[1] = perc;

			for(int i = 0; i < flyLogic.latency.Length; i++)
			{
				lat += flyLogic.latency[i];
			}
			lat = lat / flyLogic.latency.Length;
//			metScript.values[2] = lat;
			testSectionTime = 0;
			//saveHandler.SetLevel();
			break;
		case "PickUpCoins":
			saveHandler.AddLevelData("pUpCoinsMinCorrect", coinsLogic.minuteCorrect);
			saveHandler.AddLevelData("pUpCoinsMinIncorrect", coinsLogic.minuteIncorrect);
			saveHandler.AddLevelData("pUpCoinsMinMissed", coinsLogic.minuteMissed);
			saveHandler.AddLevelData("pUpCoinsExtraCorrect", coinsLogic.extraCorrect);
			saveHandler.AddLevelData("pUpCoinsExtraIncorrect", coinsLogic.extraIncorrect);
			saveHandler.AddLevelData("pUpCoinsExtraMissed", coinsLogic.extraMissed);
			saveHandler.AddLevelData("CoinsSelected", coinsLogic.coinsSelected);
			saveHandler.AddLevelData("pUpCoinsTimeOfComp", (int)testSectionTime);
			saveHandler.AddLevelData("clickBeforeMin", coinsLogic.beforeMinClick);
			metScript.values[0] = coinsLogic.minuteCorrect + coinsLogic.extraCorrect;

			testSectionTime = 0;
			//saveHandler.SetLevel();
			break;
		case "UnPack":

			saveHandler.AddLevelData ("unPackedFirstObjs", unPackLog.unPackedFObjs);
			saveHandler.AddLevelData ("unPackedFirstCorrect", unPackLog.correctFirstObj); //firstunPackLog.correct
			saveHandler.AddLevelData ("unPackedFirstPerc", (float)(unPackLog.correctFirstObj/ 3) * 100); //(firstunPackLog.correct / 3 * 100)
			saveHandler.AddLevelData ("unPackCorrectSample1", unPackLog.correct [0]);
			saveHandler.AddLevelData ("unPackCorrectSample2", unPackLog.correct [1]);
			saveHandler.AddLevelData ("unPackCorrectSample3", unPackLog.correct [2]);
			saveHandler.AddLevelData ("unPackIncorrectSample1", unPackLog.incorrect [0]);
			saveHandler.AddLevelData ("unPackIncorrectSample2", unPackLog.incorrect [1]);
			saveHandler.AddLevelData ("unPackIncorrectSample3", unPackLog.incorrect [2]);
			saveHandler.AddLevelData ("unPackRepeatedSample1", unPackLog.repeated [0]);
			saveHandler.AddLevelData ("unPackRepeatedSample2", unPackLog.repeated [1]);
			saveHandler.AddLevelData ("unPackRepeatedSample3", unPackLog.repeated [2]);
			saveHandler.AddLevelData ("unPackFourFirstSample", unPackLog.fourFirst);
			saveHandler.AddLevelData ("unPackFourLastSample", unPackLog.fourLast);
			saveHandler.AddLevelData ("unPackGroupingSample", unPackLog.grouping);
			saveHandler.AddLevelData ("unPackSpacialPrecisionSample", unPackLog.spacialPres);
			saveHandler.AddLevelData ("unPackPicTime", (int)unPackLog.timeOfPic);
			saveHandler.AddLevelData ("unPackTimeOfComp", (int)testSectionTime);
			int totalUnPackCorrect = unPackLog.correct [0] + unPackLog.correct [1] + unPackLog.correct [2];
			saveHandler.AddLevelData("unPackTotalCorrect", totalUnPackCorrect);
			saveHandler.AddLevelData("unPackXTotalCorrect", (float)totalUnPackCorrect/3);
			saveHandler.AddLevelData("unPackPercTotalCorrect", (float)totalUnPackCorrect/(unPackLog.objSequence.Count*3)*100); //checar si esta bien este porciento
			int totalUnPackIncorrect = unPackLog.incorrect[0] + unPackLog.incorrect[1] + unPackLog.incorrect[2];
			saveHandler.AddLevelData("unPackTotalIncorrect", totalUnPackIncorrect);
			saveHandler.AddLevelData("unPackXTotalIncorrect", (float)totalUnPackIncorrect/3);
			saveHandler.AddLevelData("unPackPercTotalIncorrect", (float)totalUnPackIncorrect/(unPackLog.objSequence.Count*3)*100); //checar si esta bien este porciento


			metScript.values[8] = unPackLog.correct[0] + unPackLog.correct[1] + unPackLog.correct[2] + unPackLog.correct[3];
			testSectionTime = 0;
			//saveHandler.SetLevel();
			break;
		}

	}
	void SaveProgress()
	{
		saveHandler.CreateSaveBlock("PruebaEcologica", (int)totalTestTime, 0, 0, 7);
		saveHandler.AddLevelsToBlock();
		Debug.Log(saveHandler.ToString());
	}
	void SetLevel()
	{
		flyLogic.arrowColorTutorial = configuration.miniGame.flyPlane.arrowColorTutorial;
		flyLogic.arrowDirTutorial = configuration.miniGame.flyPlane.arrowDirectionTutorial;
        flyLogic.arrowColor = configuration.miniGame.flyPlane.arrowColor;
		flyLogic.arrowDir = configuration.miniGame.flyPlane.arrowDirection;

	}
}