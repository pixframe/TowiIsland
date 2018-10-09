using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainRiver : MonoBehaviour
{
	GameObject mainCamera;
    float spawnTime = 0f;
    RiverLibrary forest;
    RiverLibrary beach;
    public int correctObjects;
    public int incorrectObjects;
    int totalCorrectObjects = 0;
    int totalIncorrectObjects = 0;
    GameSaveLoad loader;
	[HideInInspector]
    public RiverConfiguration configuration;
    public int numLevelsPerBlock;
    public int level;
    public int subLevel;
    int todayLevels;
    int todayOffset;
    AnimatedUVs water;
    public float forestMargin = 0;
    public float beachMargin = 0;
    int tutorialPhase = 0;
    int currentTutorial = 0;
    float tutorialTime;
    GameObject tutorialObject;
    RiverObject tutorialRiverObj;
	string tutorialSpecial;
    Vector2 handPosition;
    public Texture hand;
    public Texture closedHand;
    public Camera cam;
	public GUIStyle kiwiButton;
	public Texture loadingScreen;
	public Texture en_loadingScreen;
	public Texture loadingColor;
	bool fadeIn=false;
	bool fadeOut=false;
	float opacity=0;

    Hashtable riverObjects;
    public bool reverse;
    public int remainingObjects;
    public int activeObjects;
    bool specialLeave;
    bool specialReverse;
    [HideInInspector]
    public string state;
    float mapScale = 1;
    string instructionText = "";
    public GUIStyle instructionsStyle;
	public GUIStyle instructionsStyleSmall;
    public GUIStyle buttonStyle;
    public Texture instructionsBG;
    bool tutorial = false;
    float endTime = 8;
    bool returnToIsland = false;
    bool displayButton = false;
	bool left=false;
	float handSpeed=0.07f;
	float handSpeed2=0.05f;
	int objectSpeed=4;
	float waitGrab=3;
	float pickupTime=0.2f;
	float dropTime=0.2f;

    //Pause
    public GUIStyle pauseButton;
    public Texture2D pauseBackground;
    public GUIStyle pauseContinue;
    public GUIStyle pauseIsland;
    public GUIStyle pauseExit;
    public GUIStyle pauseText;
    public string stateBeforePause = "";

    //Progress
    ProgressHandler saveHandler;
    int playedLevels = 0;
	int repeatedLevels = 0;
	int failedLevels=0;
	bool repeatLevel=false;
    float levelTime = 0;
    float totalTime = 0;
    List<string> levelObjects;

    public List<string> reverseObjects = new List<string>();
    public List<string> neutralObjects = new List<string>();
    public List<string> forceForestObjects = new List<string>();
    public List<string> forceBeachObject = new List<string>();
    public List<string> specialReverseObjects = new List<string>();
    public List<string> specialLeaveObjects = new List<string>();

    public Texture2D scoreBG;
    public Texture2D scoreKiwi;
    public Texture2D scoreKiwiDisabled;
    public GUIStyle scoreStyle1;
    public GUIStyle scoreStyle2;
    public GUIStyle scoreStyle3;
    int kiwis = -1;
    int animationKiwis = 0;
    public float kiwiAnimTime = 2;
    float kiwiAnimCurrTime;
    public ScoreEffects scoreEffects;
    [HideInInspector]
    public SoundManager soundMng;
    RiverCards cardsMng;
	[HideInInspector]
	public SessionManager sessionMng;
	[HideInInspector]
	public LanguageLoader language;
	public GUIStyle pauseButtons;

    public float pickTime = 5;
    public float pickTimer;
    public Scores scoreScript;
    // Use this for initialization
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
        soundMng = GetComponent<SoundManager>();
        cardsMng = GetComponent<RiverCards>();
        scoreScript = GetComponent<Scores>();
        kiwiAnimCurrTime = kiwiAnimTime;
        levelObjects = new List<string>();
        mapScale = (float)Screen.height / (float)768;
        instructionsStyle.fontSize = (int)(instructionsStyle.fontSize * mapScale);
		instructionsStyleSmall.fontSize = (int)(instructionsStyleSmall.fontSize * mapScale);
        buttonStyle.fontSize = (int)(buttonStyle.fontSize * mapScale);
        pauseText.fontSize = (int)(pauseText.fontSize * mapScale);
        scoreStyle1.fontSize = (int)(scoreStyle1.fontSize * mapScale);
        scoreStyle2.fontSize = (int)(scoreStyle2.fontSize * mapScale);
        scoreStyle3.fontSize = (int)(scoreStyle3.fontSize * mapScale);
		kiwiButton.fontSize = (int)(kiwiButton.fontSize * mapScale);
		pauseButtons.fontSize = (int)(pauseButtons.fontSize * mapScale);
        state = "Instructions";
        forest = GameObject.Find("Forest").GetComponent<RiverLibrary>();
        beach = GameObject.Find("Beach").GetComponent<RiverLibrary>();
		forest.UpdateNames (language.levelStrings, 9, 15,lang);
		beach.UpdateNames (language.levelStrings, 16, 26,lang);
        water = GameObject.Find("Water").GetComponent<AnimatedUVs>();
        correctObjects = 0;
        incorrectObjects = 0;
        loader = GetComponent<GameSaveLoad>();
        loader.Load(GameSaveLoad.game.river);
        configuration = (RiverConfiguration)loader.configuration;
		mainCamera = GameObject.Find ("Main Camera");
		if(configuration.music==0)
		{
			mainCamera.GetComponents<AudioSource>()[0].Stop();
		}else
		{
			mainCamera.GetComponents<AudioSource>()[0].Play();
		}
		if(configuration.sound==0)
		{
			mainCamera.GetComponents<AudioSource>()[1].volume=0;
			mainCamera.GetComponents<AudioSource>()[1].Stop();
			GetComponent<AudioSource>().volume=0;
			GetComponent<AudioSource>().Stop();
		}else
		{
			mainCamera.GetComponents<AudioSource>()[1].Play();
		}
        saveHandler = (ProgressHandler)GetComponent(typeof(ProgressHandler));
        riverObjects = new Hashtable();
        reverse = false;
        specialLeave = false;
        specialReverse = false;
		level = sessionMng.activeKid.riverDifficulty;
		subLevel = sessionMng.activeKid.riverLevel;
        todayLevels = 0;
		todayOffset = sessionMng.activeKid.playedRiver;
        pickTimer = pickTime;
        //level = 0;
        //subLevel = 0;
		cardsMng.SetCards();
        SetLevel();
    }

    void OnGUI()
    {

        //Margenes zonas
        //GUI.DrawTexture (new Rect (Screen.width * forestMargin, 0, 2, Screen.height), instructionsBG);
        //GUI.DrawTexture (new Rect (Screen.width * beachMargin, 0, 2, Screen.height), instructionsBG);

        float yOffset = 70 * mapScale;
        switch (state)
        {
            case "Instructions":
                //GUI.DrawTexture(new Rect(Screen.width*0.5f-498*mapScale,Screen.height*0.5f-148*mapScale-yOffset,1000*mapScale,300*mapScale),instructionsBG);
                instructionsStyle.normal.textColor = new Color(0.32f, 0.32f, 0.32f);
                //GUI.Label(new Rect(Screen.width*0.5f-448*mapScale,Screen.height*0.5f-148*mapScale-yOffset,900*mapScale,300*mapScale),instructionText,instructionsStyle);
                instructionsStyle.normal.textColor = new Color(0.88f, 0.88f, 0.88f);
                //GUI.Label(new Rect(Screen.width*0.5f-450*mapScale,Screen.height*0.5f-150*mapScale-yOffset,900*mapScale,300*mapScale),instructionText,instructionsStyle);
                if (displayButton)
                {
                    if (GUI.Button(new Rect(Screen.width * 0.5f - 100 * mapScale, Screen.height * 0.5f + 280 * mapScale - yOffset, 200 * mapScale, 100 * mapScale), language.levelStrings[27], buttonStyle))
                    {
                        cardsMng.DestroyCards();
						activeObjects=0;
						correctObjects=0;
						incorrectObjects=0;
                        state = "Play";
                        displayButton = false;
                    }
                }
                break;
            case "Tutorial":
                switch (currentTutorial)
                {
                    case 0:
				GUI.Label(new Rect(Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.1f, 800 * mapScale, 200 * mapScale), language.levelStrings[28], instructionsStyle);
                        switch (tutorialPhase)
                        {
                            case 0:

                                break;
                            case 1:

                                break;
                            case 2:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
                                break;
                            case 3:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
                                break;
                            case 4:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
                                break;
                            case 5:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
                                break;
                            case 6:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
                                break;
                            case 7:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
                                break;
                            case 8:

                                break;
                            case 9:

                                break;
                            case 10:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
                                break;
                            case 11:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
                                break;
                            case 12:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
                                break;
                            case 13:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
                                break;
                            case 14:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
                                break;
                            case 15:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
                                break;
                        }
                        break;
                    case 1:
				GUI.Label(new Rect(Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.1f, 800 * mapScale, 200 * mapScale), language.levelStrings[28], instructionsStyle);
                        switch (tutorialPhase)
                        {
                            case 0:

                                break;
                            case 1:

                                break;
                            case 2:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
                                break;
                            case 3:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
                                break;
                            case 4:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
                                break;
                            case 5:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
                                break;
                            case 6:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
                                break;
                            case 7:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
                                break;
                            case 8:

                                break;
                            case 9:

                                break;
                            case 10:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
                                break;
                            case 11:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
                                break;
                            case 12:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
                                break;
                            case 13:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
                                break;
                            case 14:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
                                break;
                            case 15:
                                GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
                                break;
                        }
                        break;
                    case 2:
				GUI.Label(new Rect(Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.1f, 800 * mapScale, 200 * mapScale), language.levelStrings[28], instructionsStyle);
						switch (tutorialPhase)
						{
						case 0:
							
							break;
						case 1:
							
							break;
						case 2:
							GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
							break;
						case 3:
							GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
							break;
						case 4:

							break;
						}
                        break;
                    case 3:
				GUI.Label(new Rect(Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.1f, 800 * mapScale, 200 * mapScale), language.levelStrings[28], instructionsStyle);
				switch (tutorialPhase)
				{
				case 0:
					
					break;
				case 1:
					
					break;
				case 2:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 3:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 4:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 5:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 6:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 7:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 8:
					
					break;
				case 9:
					
					break;
				case 10:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 11:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 12:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 13:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 14:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 15:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				}
                        break;
                    case 4:
				GUI.Label(new Rect(Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.1f, 800 * mapScale, 200 * mapScale), language.levelStrings[28], instructionsStyle);
				switch (tutorialPhase)
				{
				case 0:
					
					break;
				case 1:
					
					break;
				case 2:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 3:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 4:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 5:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 6:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 7:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 8:
					
					break;
				case 9:
					
					break;
				case 10:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 11:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 12:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 13:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 14:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 15:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				}
                        break;
                    case 5:
				GUI.Label(new Rect(Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.1f, 800 * mapScale, 200 * mapScale), language.levelStrings[28], instructionsStyle);
				switch (tutorialPhase)
				{
				case 0:
					
					break;
				case 1:
					
					break;
				case 2:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 3:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 4:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 5:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 6:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 7:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 8:
					
					break;
				case 9:
					
					break;
				case 10:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 11:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 12:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 13:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 14:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 15:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				}
                        break;
                    case 6:
				GUI.Label(new Rect(Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.1f, 800 * mapScale, 200 * mapScale), language.levelStrings[28], instructionsStyle);
				switch (tutorialPhase)
				{
				case 0:
					
					break;
				case 1:
					
					break;
				case 2:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 3:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 4:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 5:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 6:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 7:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 8:
					
					break;
				case 9:
					
					break;
				case 10:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 11:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 12:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 13:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 14:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 15:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				}
                        break;
                    case 7:
				GUI.Label(new Rect(Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.1f, 800 * mapScale, 200 * mapScale), language.levelStrings[28], instructionsStyle);
				switch (tutorialPhase)
				{
				case 0:
					
					break;
				case 1:
					
					break;
				case 2:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 3:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 4:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 5:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 6:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 7:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 8:
					
					break;
				case 9:
					
					break;
				case 10:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 11:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 12:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 13:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 14:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 15:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 16:

					break;
				case 17:
					
					break;
				case 18:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 19:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 20:

					break;
				case 21:

					break;
				}
                        break;
                    case 8:
				GUI.Label(new Rect(Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.1f, 800 * mapScale, 200 * mapScale), language.levelStrings[28], instructionsStyle);
				switch (tutorialPhase)
				{
				case 0:
					
					break;
				case 1:
					
					break;
				case 2:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 3:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 4:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 5:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 6:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 7:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 8:
					
					break;
				case 9:
					
					break;
				case 10:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 11:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 12:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 13:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 14:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 15:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 16:
					
					break;
				case 17:
					
					break;
				case 18:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 19:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 20:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 21:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 22:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 23:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 24:
					break;
				}
                        break;
                    case 9:
				GUI.Label(new Rect(Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.1f, 800 * mapScale, 200 * mapScale), language.levelStrings[28], instructionsStyle);
				switch (tutorialPhase)
				{
				case 0:
					
					break;
				case 1:
					
					break;
				case 2:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 3:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 4:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 5:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 6:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 7:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 8:
					
					break;
				case 9:
					
					break;
				case 10:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 11:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 12:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 13:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 14:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 15:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 16:
					
					break;
				case 17:
					
					break;
				case 18:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 19:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 20:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 21:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), closedHand);
					break;
				case 22:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 23:
					GUI.DrawTexture(new Rect(handPosition.x - 35 * mapScale, handPosition.y - 35 * mapScale, 70 * mapScale, 70 * mapScale), hand);
					break;
				case 24:
					break;
				}
                        break;
                }
                break;
            case "Play":
				instructionsStyleSmall.normal.textColor = new Color(0.32f, 0.32f, 0.32f);
			GUI.Label(new Rect(202 * mapScale, Screen.height * 0.5f - 248 * mapScale, 200 * mapScale, 100 * mapScale), language.levelStrings[29], instructionsStyleSmall);
				instructionsStyleSmall.normal.textColor = new Color(0.88f, 0.88f, 0.88f);
			GUI.Label(new Rect(200 * mapScale, Screen.height * 0.5f - 250 * mapScale, 200 * mapScale, 100 * mapScale),language.levelStrings[29], instructionsStyleSmall);

				instructionsStyleSmall.normal.textColor = new Color(0.32f, 0.32f, 0.32f);
			GUI.Label(new Rect(Screen.width - 300 * mapScale, Screen.height * 0.5f - 248 * mapScale, 200 * mapScale, 100 * mapScale), language.levelStrings[30], instructionsStyleSmall);
				instructionsStyleSmall.normal.textColor = new Color(0.88f, 0.88f, 0.88f);
			GUI.Label(new Rect(Screen.width - 302 * mapScale, Screen.height * 0.5f - 250 * mapScale, 200 * mapScale, 100 * mapScale), language.levelStrings[30], instructionsStyleSmall);
                break;
            case "CompletedActivity":
                GUI.DrawTexture(new Rect(0, Screen.height * 0.5f - 237 * mapScale, Screen.width, 475 * mapScale), scoreBG);

                switch (animationKiwis)
                {
                    case 0:
                        GUI.DrawTexture(new Rect(Screen.width * 0.5f - 150 * mapScale, Screen.height * 0.5f - 10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwiDisabled);
                        GUI.DrawTexture(new Rect(Screen.width * 0.5f - 45 * mapScale, Screen.height * 0.5f - 10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwiDisabled);
                        GUI.DrawTexture(new Rect(Screen.width * 0.5f + 60 * mapScale, Screen.height * 0.5f - 10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwiDisabled);
                        break;
                    case 1:
                        GUI.DrawTexture(new Rect(Screen.width * 0.5f - 150 * mapScale, Screen.height * 0.5f - 10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwi);
                        GUI.DrawTexture(new Rect(Screen.width * 0.5f - 45 * mapScale, Screen.height * 0.5f - 10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwiDisabled);
                        GUI.DrawTexture(new Rect(Screen.width * 0.5f + 60 * mapScale, Screen.height * 0.5f - 10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwiDisabled);
                        break;
                    case 2:
                        GUI.DrawTexture(new Rect(Screen.width * 0.5f - 150 * mapScale, Screen.height * 0.5f - 10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwi);
                        GUI.DrawTexture(new Rect(Screen.width * 0.5f - 45 * mapScale, Screen.height * 0.5f - 10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwi);
                        GUI.DrawTexture(new Rect(Screen.width * 0.5f + 60 * mapScale, Screen.height * 0.5f - 10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwiDisabled);
                        break;
                    case 3:
                        GUI.DrawTexture(new Rect(Screen.width * 0.5f - 150 * mapScale, Screen.height * 0.5f - 10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwi);
                        GUI.DrawTexture(new Rect(Screen.width * 0.5f - 45 * mapScale, Screen.height * 0.5f - 10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwi);
                        GUI.DrawTexture(new Rect(Screen.width * 0.5f + 60 * mapScale, Screen.height * 0.5f - 10 * mapScale, 90 * mapScale, 90 * mapScale), scoreKiwi);
                        break;
                }
                if (kiwis > 0)
                    GUI.Label(new Rect(Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 150 * mapScale, 800 * mapScale, 100 * mapScale), language.levelStrings[31], scoreStyle1);
                else
				GUI.Label(new Rect(Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 150 * mapScale, 800 * mapScale, 100 * mapScale), language.levelStrings[32], scoreStyle1);
                if (kiwis == 0)
				GUI.Label(new Rect(Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 90 * mapScale, 800 * mapScale, 100 * mapScale), language.levelStrings[33], scoreStyle2);
                else
                {
                    if (kiwis > 1)
					GUI.Label(new Rect(Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 90 * mapScale, 800 * mapScale, 100 * mapScale), language.levelStrings[34]+" " + kiwis.ToString() + " "+language.levelStrings[35], scoreStyle2);
                    else
					GUI.Label(new Rect(Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 90 * mapScale, 800 * mapScale, 100 * mapScale), language.levelStrings[34]+" " + kiwis.ToString() + " "+language.levelStrings[36], scoreStyle2);
                }
			if(GUI.Button(new Rect(Screen.width*0.5f-80*mapScale,Screen.height*0.5f+110*mapScale,160*mapScale,60*mapScale),language.levelStrings[38],kiwiButton))
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
                break;
            case "Pause":
                float pauseY = Screen.height * 0.5f - 177 * mapScale;
                GUI.DrawTexture(new Rect(0, pauseY, Screen.width, 354 * mapScale), pauseBackground);
			GUI.Label(new Rect(Screen.width * 0.5f - 100 * mapScale, pauseY - 40 * mapScale, 200 * mapScale, 60 * mapScale), language.levelStrings[37], pauseText);
                if (GUI.Button(new Rect(Screen.width * 0.5f - 200 * mapScale, pauseY + 50 * mapScale, 366 * mapScale, 66 * mapScale), "", pauseContinue))
                {
                    Time.timeScale = 1.0f;
                    state = stateBeforePause;
                }
                else
                    if (GUI.Button(new Rect(Screen.width * 0.5f - 200 * mapScale, pauseY + 140 * mapScale, 382 * mapScale, 66 * mapScale), "", pauseIsland))
                    {
                        if (!returnToIsland)
                        {
                            SaveProgress(false);
                            returnToIsland = true;
                            Time.timeScale = 1.0f;
                        }
                        //Application.LoadLevel("Archipielago");
                    }
				GUI.Label(new Rect(Screen.width*0.5f-110*mapScale,pauseY+50*mapScale,366*mapScale,66*mapScale),language.levelStrings[39],pauseButtons);
				GUI.Label(new Rect(Screen.width*0.5f-110*mapScale,pauseY+140*mapScale,382*mapScale,66*mapScale),language.levelStrings[40],pauseButtons);
			/*else
				if(GUI.Button(new Rect(Screen.width*0.5f-200*mapScale,pauseY+230*mapScale,162*mapScale,67*mapScale),"",pauseExit))
			{
				Application.Quit();
			}*/
                break;
        }
        if (state != "Pause" && state != "CompletedActivity")
        {
            if (GUI.Button(new Rect(10 * mapScale, 10 * mapScale, 71 * mapScale, 62 * mapScale), "", pauseButton))
            {
                Time.timeScale = 0.0f;
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

    public void StartVisualInstructions(int index,string special)
    {
        state = "Tutorial";
        currentTutorial = index;
        tutorialPhase = 0;
		tutorialSpecial = special;
    }

    void SetLevel()
    {
        levelObjects.Clear();
        riverObjects.Clear();
        reverseObjects.Clear();
        neutralObjects.Clear();
        forceForestObjects.Clear();
        forceBeachObject.Clear();
        specialReverseObjects.Clear();
        specialLeaveObjects.Clear();
        correctObjects = 0;
        incorrectObjects = 0;
        reverse = configuration.levels[level].subLevels[subLevel].reverse;
        water.uvTargetRate.y = -0.4f * configuration.levels[level].subLevels[subLevel].speed;
        if (reverse)
        {
            soundMng.AddSoundToQueue(1, false);
			instructionText = language.levelStrings[0];
			cardsMng.AddInstructions(1, language.levelStrings[0]);
        }
        else
        {
            soundMng.AddSoundToQueue(0, false);
			instructionText = language.levelStrings[1];
			cardsMng.AddInstructions(0, language.levelStrings[1]);
        }
        remainingObjects=configuration.levels [level].subLevels [subLevel].totalObjects;
        //remainingObjects = 5;
        activeObjects = 0;
        List<string> objectList = new List<string>();
        for (int i = 0; i < configuration.levels[level].subLevels[subLevel].availableObjects.Length; i++)
        {
            riverObjects.Add(configuration.levels[level].subLevels[subLevel].availableObjects[i], new riverSpecs());
            objectList.Add(configuration.levels[level].subLevels[subLevel].availableObjects[i]);
        }
        int specialObject = Random.Range(0, objectList.Count);
        string specialObjectName = objectList[specialObject];
        if (configuration.levels[level].subLevels[subLevel].neutralObjects)
        {
            objectList.RemoveAt(specialObject);
            (riverObjects[specialObjectName] as riverSpecs).neutral = true;
            neutralObjects.Add(specialObjectName);
        }
        else if (configuration.levels[level].subLevels[subLevel].specialReverse)
        {
            objectList.RemoveAt(specialObject);
            (riverObjects[specialObjectName] as riverSpecs).specialL = true;
            specialReverseObjects.Add(specialObjectName);
        }
        else if (configuration.levels[level].subLevels[subLevel].specialLeave)
        {
            objectList.RemoveAt(specialObject);
            (riverObjects[specialObjectName] as riverSpecs).specialR = true;
            specialLeaveObjects.Add(specialObjectName);
        }
        int divisor = 1;
        if (configuration.levels[level].subLevels[subLevel].reverseObjects)
        {
            divisor++;
            if (reverse)
            {
                soundMng.AddSoundToQueue(2, false);
				instructionText += "\n\n"+language.levelStrings[2];
				cardsMng.AddInstructions(3, language.levelStrings[2]);
            }
            else
            {
                soundMng.AddSoundToQueue(3, false);
				instructionText += "\n\n"+language.levelStrings[3];
				cardsMng.AddInstructions(4, language.levelStrings[3]);
            }
        }

        if (configuration.levels[level].subLevels[subLevel].forceForest)
        {
            divisor++;
            soundMng.AddSoundToQueue(4, false);
			instructionText += "\n\n"+language.levelStrings[4];
			cardsMng.AddInstructions(5, language.levelStrings[4]);
        }

        if (configuration.levels[level].subLevels[subLevel].forceBeach)
        {
            divisor++;
            soundMng.AddSoundToQueue(5, false);
			instructionText += "\n\n"+language.levelStrings[5];
			cardsMng.AddInstructions(6, language.levelStrings[5]);
        }

        int sizeDivided = riverObjects.Count / divisor;
        if (configuration.levels[level].subLevels[subLevel].reverseObjects)
        {
            for (int i = 0; i < sizeDivided; i++)
            {
                int randomObj = Random.Range(0, objectList.Count);
                (riverObjects[objectList[randomObj]] as riverSpecs).reverse = true;
                reverseObjects.Add(objectList[randomObj]);
                objectList.RemoveAt(randomObj);
            }
        }
        if (configuration.levels[level].subLevels[subLevel].forceForest)
        {
            for (int i = 0; i < sizeDivided; i++)
            {
                int randomObj = Random.Range(0, objectList.Count);
                (riverObjects[objectList[randomObj]] as riverSpecs).forest = true;
                forceForestObjects.Add(objectList[randomObj]);
                objectList.RemoveAt(randomObj);
            }
        }
        if (configuration.levels[level].subLevels[subLevel].forceBeach)
        {
            for (int i = 0; i < sizeDivided; i++)
            {
                int randomObj = Random.Range(0, objectList.Count);
                (riverObjects[objectList[randomObj]] as riverSpecs).beach = true;
                forceBeachObject.Add(objectList[randomObj]);
                objectList.RemoveAt(randomObj);
            }
        }

        if (configuration.levels[level].subLevels[subLevel].neutralObjects)
        {
            string[] tempNames;
            tempNames = forest.GetNames(specialObjectName);
            if (tempNames == null)
                tempNames = beach.GetNames(specialObjectName);
            int tempSound = -1;
            tempSound = forest.GetSound(specialObjectName);
            if (tempSound == -1)
                tempSound = beach.GetSound(specialObjectName);
            soundMng.AddSoundToQueue(6, true,false);
            if (tempSound != -1)
                soundMng.AddSoundToQueue(tempSound, false);
            Texture tempImg = null;
            if (tempImg == null)
                tempImg = beach.GetImage(specialObjectName);
            if (tempImg == null)
                tempImg = forest.GetImage(specialObjectName);
			instructionText += "\n\n"+language.levelStrings[6]+" " + tempNames[1] + " " + tempNames[0].ToUpper();
			cardsMng.AddInstructions(2, language.levelStrings[6]+" " + tempNames[1] + " " + tempNames[0].ToUpper(), tempImg,specialObjectName);
        }
        else if (configuration.levels[level].subLevels[subLevel].specialReverse)
        {
            string[] tempNames;
            tempNames = forest.GetNames(specialObjectName);
            if (tempNames == null)
                tempNames = beach.GetNames(specialObjectName);
            int tempSound = -1;
            tempSound = forest.GetSound(specialObjectName);
            if (tempSound == -1)
                tempSound = beach.GetSound(specialObjectName);
			soundMng.AddSoundToQueue(7, true,false);
            if (tempSound != -1)
                soundMng.AddSoundToQueue(tempSound, false);

            Texture tempImg = null;
            if (tempImg == null)
                tempImg = beach.GetImage(specialObjectName);
            if (tempImg == null)
                tempImg = forest.GetImage(specialObjectName);
			instructionText += "\n\n"+language.levelStrings[7]+" " + tempNames[2] + " " + tempNames[0];
			cardsMng.AddInstructions(7, language.levelStrings[7]+" " + tempNames[2] + " " + tempNames[0].ToUpper(), tempImg,specialObjectName);

        }
        else if (configuration.levels[level].subLevels[subLevel].specialLeave)
        {
            string[] tempNames;
            tempNames = forest.GetNames(specialObjectName);
            if (tempNames == null)
                tempNames = beach.GetNames(specialObjectName);
            int tempSound = -1;
            tempSound = forest.GetSound(specialObjectName);
            if (tempSound == -1)
                tempSound = beach.GetSound(specialObjectName);

            if (reverse)
            {
				soundMng.AddSoundToQueue(8, true,false);
                if (tempSound != -1)
                    soundMng.AddSoundToQueue(tempSound, false);

                Texture tempImg = null;
                if (tempImg == null)
                    tempImg = beach.GetImage(specialObjectName);
                if (tempImg == null)
                    tempImg = forest.GetImage(specialObjectName);
				instructionText += "\n\n"+language.levelStrings[8]+" " + tempNames[2] + " " + tempNames[0].ToUpper() + " "+language.levelStrings[41];
				cardsMng.AddInstructions(8, language.levelStrings[8]+" " + tempNames[2] + " " + tempNames[0].ToUpper() + " "+language.levelStrings[41], tempImg,specialObjectName);
            }
            else
            {
				soundMng.AddSoundToQueue(9, true,false);
                if (tempSound != -1)
                    soundMng.AddSoundToQueue(tempSound, false);
                Texture tempImg = null;
                if (tempImg == null)
                    tempImg = beach.GetImage(specialObjectName);
                if (tempImg == null)
                    tempImg = forest.GetImage(specialObjectName);
				instructionText += "\n\n"+language.levelStrings[8]+" " + tempNames[2] + " " + tempNames[0].ToUpper() + " "+language.levelStrings[42];
				cardsMng.AddInstructions(9, language.levelStrings[8]+" " + tempNames[2] + " " + tempNames[0].ToUpper() + " "+language.levelStrings[42], tempImg,specialObjectName);
            }
        }
        cardsMng.StartCards();
    }

    public void DisplayButton()
    {
        displayButton = true;
    }

    void SaveLevelProgress()
    {
        saveHandler.AddLevelData("level", level);
        saveHandler.AddLevelData("sublevel", subLevel);
        saveHandler.AddLevelData("time", (int)levelTime);
        saveHandler.AddLevelData("tutorial", tutorial);
        saveHandler.AddLevelData("reverse", reverse);
        saveHandler.AddLevelData("speed", configuration.levels[level].subLevels[subLevel].speed);
        saveHandler.AddLevelData("correctObjects", correctObjects);
        saveHandler.AddLevelData("incorrectObjects", incorrectObjects);
        saveHandler.AddLevelData("levelObjects", levelObjects.ToArray());
        saveHandler.AddLevelData("availableObjects", configuration.levels[level].subLevels[subLevel].availableObjects);
        saveHandler.AddLevelData("reverseObjects", reverseObjects.ToArray());
        saveHandler.AddLevelData("neutralObjects", neutralObjects.ToArray());
        saveHandler.AddLevelData("forceForestObjects", forceForestObjects.ToArray());
        saveHandler.AddLevelData("forceBeachForest", forceBeachObject.ToArray());
        saveHandler.AddLevelData("specialReverseObjects", specialReverseObjects.ToArray());
        saveHandler.AddLevelData("specialLeaveObjects", specialLeaveObjects.ToArray());
        levelTime = 0;
        playedLevels++;
		//if (repeatLevel) 
		//{

		//}
		if (failedLevels > 0) 
		{
			repeatedLevels++;
			repeatLevel=true;
			if(failedLevels>1)
				failedLevels=0;
		}
        todayLevels++;
		totalCorrectObjects += correctObjects;
		totalIncorrectObjects += incorrectObjects;
		sessionMng.activeKid.playedRiver = todayLevels + todayOffset;

        if (playedLevels + todayOffset >= numLevelsPerBlock)
        {
			CalculateKiwis ();
			sessionMng.activeKid.blockedRio=1;
            SaveProgress(true);
            state = "CompletedActivity";
        }
        else
        {
            //saveHandler.SetLevel();
            state = "Instructions";
        }
		sessionMng.SaveSession ();
    }
    void SaveProgress(bool rank)
    {
        if (playedLevels > 0)
        {
            saveHandler.CreateSaveBlock("Rio", (int)totalTime, playedLevels-repeatedLevels, repeatedLevels, playedLevels);
            saveHandler.AddLevelsToBlock();
            saveHandler.PostProgress(rank);
            Debug.Log(saveHandler.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (returnToIsland && !saveHandler.saving)
        {
			fadeIn=true;
            //Application.LoadLevel("Archipielago");
        }
        levelTime += Time.deltaTime;
        totalTime += Time.deltaTime;
        switch (state)
        {
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
                }
                else
                {
                    scoreScript.ScoreCounter();
                }
                /*endTime -= Time.deltaTime;
                if (endTime <= 0)
                {
                    Application.LoadLevel("Archipielago");
                }*/
                break;
            case "Play":
                spawnTime -= Time.deltaTime;
                pickTimer-=Time.deltaTime;
                if (spawnTime < 0)
                {
                    if (remainingObjects > 0)
                    {
                        spawnTime = 6;
                        remainingObjects--;
                        activeObjects++;
                        int random = Random.Range(0, configuration.levels[level].subLevels[subLevel].availableObjects.Length);
                        riverSpecs tempSpecs = (riverSpecs)riverObjects[configuration.levels[level].subLevels[subLevel].availableObjects[random]];
                        levelObjects.Add(configuration.levels[level].subLevels[subLevel].availableObjects[random]);
                        Debug.Log(configuration.levels[level].subLevels[subLevel].availableObjects[random]);
                        forest.Spawn(configuration.levels[level].subLevels[subLevel].availableObjects[random], !tempSpecs.forest && !tempSpecs.beach ? tempSpecs.reverse || specialReverse ? !reverse : reverse : false, tempSpecs.neutral || specialLeave, tempSpecs.reverse, tempSpecs.forest, tempSpecs.beach, configuration.levels[level].subLevels[subLevel].speed);
                        beach.Spawn(configuration.levels[level].subLevels[subLevel].availableObjects[random], !tempSpecs.forest && !tempSpecs.beach ? tempSpecs.reverse || specialReverse ? !reverse : reverse : false, tempSpecs.neutral || specialLeave, tempSpecs.reverse, tempSpecs.forest, tempSpecs.beach, configuration.levels[level].subLevels[subLevel].speed);
                        specialLeave = tempSpecs.specialL;
                        specialReverse = tempSpecs.specialR;
                    }
                    else
                    {
                        if (activeObjects <= 0)
                        {
							if(incorrectObjects>=correctObjects)
							{
								failedLevels++;
								if(failedLevels>1)
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
								}
							}
							else
							{
								failedLevels=0;
	                            if (subLevel < configuration.levels[level].subLevels.Length - 1)
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
							sessionMng.activeKid.riverDifficulty=level;
							sessionMng.activeKid.riverLevel=subLevel;
							sessionMng.SaveSession();
                            SaveLevelProgress();
                            if (state != "CompletedActivity")
                                SetLevel();
                        }
                    }
                }
                break;
            case "Tutorial":
                switch (currentTutorial)
                {
                    case 0:
                        switch (tutorialPhase)
                        {
                            case 0:
                                tutorialObject = forest.Spawn("Acorn", false, false, false, false, false, objectSpeed);
                                tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
                                tutorialRiverObj.tutorial = true;
                                tutorialPhase++;
					tutorialTime = waitGrab;
                                break;
                            case 1:
                                tutorialTime -= Time.deltaTime;
                                if (tutorialTime <= 0)
                                {
                                    handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
                                    tutorialRiverObj.riverSpeed = 0;
                                    tutorialPhase++;
                                }
                                break;
                            case 2:
                                Vector3 tempPos = cam.WorldToScreenPoint(tutorialObject.transform.position);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y), handSpeed);
                                if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
                                {
                                    tutorialRiverObj.SimulateDrag(handPosition);
						tutorialTime = pickupTime;
                                    tutorialPhase++;
                                }
                                break;
                            case 3:
                                tutorialTime -= Time.deltaTime;
                                if (tutorialTime <= 0)
                                    tutorialPhase++;
                                break;
                            case 4:
                                handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.2f, handPosition.y), handSpeed);
                                tutorialRiverObj.SimulateDrag(handPosition);
                                if (handPosition.x <= Screen.width * 0.21f)
                                {
                                    tutorialTime = dropTime;
                                    tutorialPhase++;
                                }
                                break;
                            case 5:
                                tutorialTime -= Time.deltaTime;
                                if (tutorialTime <= 0)
                                {
                                    tutorialRiverObj.riverSpeed = 2;
                                    tutorialRiverObj.simulateDrop = true;
                                    tutorialPhase++;
                                    tutorialTime = 1;
                                }
                                break;
                            case 6:
                                tutorialTime -= Time.deltaTime;
                                if (tutorialTime <= 0)
                                {
                                    tutorialPhase++;
                                }
                                break;
                            case 7:
                                Vector2 endPos = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
					handPosition = Vector2.Lerp(handPosition, endPos, handSpeed2);
                                if (Vector2.Distance(handPosition, endPos) < 1)
                                {
                                    tutorialPhase++;
                                }
                                break;
                            case 8:
					tutorialObject = beach.Spawn("Star", false, false, false, false, false,objectSpeed);
                                tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
                                tutorialRiverObj.tutorial = true;
                                tutorialPhase++;
					tutorialTime = waitGrab;
                                break;
                            case 9:
                                tutorialTime -= Time.deltaTime;
                                if (tutorialTime <= 0)
                                {
                                    handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
                                    tutorialRiverObj.riverSpeed = 0;
                                    tutorialPhase++;
                                }
                                break;
                            case 10:
                                tempPos = cam.WorldToScreenPoint(tutorialObject.transform.position);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y), handSpeed);
                                if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
                                {
                                    tutorialRiverObj.SimulateDrag(handPosition);
						tutorialTime = pickupTime;
                                    tutorialPhase++;
                                }
                                break;
                            case 11:
                                tutorialTime -= Time.deltaTime;
                                if (tutorialTime <= 0)
                                    tutorialPhase++;
                                break;
                            case 12:
					handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.8f, handPosition.y), handSpeed);
                                tutorialRiverObj.SimulateDrag(handPosition);
                                if (handPosition.x >= Screen.width * 0.79f)
                                {
						tutorialTime = dropTime;
                                    tutorialPhase++;
                                }
                                break;
                            case 13:
                                tutorialTime -= Time.deltaTime;
                                if (tutorialTime <= 0)
                                {
                                    tutorialRiverObj.riverSpeed = 2;
                                    tutorialRiverObj.simulateDrop = true;
                                    tutorialPhase++;
                                    tutorialTime = 1;
                                }
                                break;
                            case 14:
                                tutorialTime -= Time.deltaTime;
                                if (tutorialTime <= 0)
                                {
                                    tutorialPhase++;
                                }
                                break;
                            case 15:
                                endPos = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
					handPosition = Vector2.Lerp(handPosition, endPos, handSpeed2);
                                if (Vector2.Distance(handPosition, endPos) < 1)
                                {
                                    state = "Instructions";
                                    cardsMng.VisualTutorialEnded();
                                }
                                break;
                        }
                        break;
                    case 1:
                        switch (tutorialPhase)
                        {
                            case 0:
					tutorialObject = forest.Spawn("Acorn", true, false, false, false, false, objectSpeed);
                                tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
                                tutorialRiverObj.tutorial = true;
                                tutorialPhase++;
					tutorialTime = waitGrab;
                                break;
                            case 1:
                                tutorialTime -= Time.deltaTime;
                                if (tutorialTime <= 0)
                                {
                                    handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);

                                    tutorialRiverObj.riverSpeed = 0;
                                    tutorialPhase++;
                                }
                                break;
                            case 2:
                                Vector3 tempPos = cam.WorldToScreenPoint(tutorialObject.transform.position);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y), handSpeed);
                                if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
                                {
                                    tutorialRiverObj.SimulateDrag(handPosition);
						tutorialTime = pickupTime;
                                    tutorialPhase++;
                                }
                                break;
                            case 3:
                                tutorialTime -= Time.deltaTime;
                                if (tutorialTime <= 0)
                                    tutorialPhase++;
                                break;
                            case 4:
					handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.8f, handPosition.y), handSpeed);
                                tutorialRiverObj.SimulateDrag(handPosition);
                                if (handPosition.x >= Screen.width * 0.79f)
                                {
						tutorialTime = dropTime;
                                    tutorialPhase++;
                                }
                                break;
                            case 5:
                                tutorialTime -= Time.deltaTime;
                                if (tutorialTime <= 0)
                                {
                                    tutorialRiverObj.riverSpeed = 2;
                                    tutorialRiverObj.simulateDrop = true;
                                    tutorialPhase++;
                                    tutorialTime = 1;
                                }
                                break;
                            case 6:
                                tutorialTime -= Time.deltaTime;
                                if (tutorialTime <= 0)
                                {
                                    tutorialPhase++;
                                }
                                break;
                            case 7:
                                Vector2 endPos = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
					handPosition = Vector2.Lerp(handPosition, endPos, handSpeed2);
                                if (Vector2.Distance(handPosition, endPos) < 1)
                                {
                                    tutorialPhase++;
                                }
                                break;
                            case 8:
					tutorialObject = beach.Spawn("Star", true, false, false, false, false, objectSpeed);
                                tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
                                tutorialRiverObj.tutorial = true;
                                tutorialPhase++;
					tutorialTime = waitGrab;
                                break;
                            case 9:
                                tutorialTime -= Time.deltaTime;
                                if (tutorialTime <= 0)
                                {
                                    handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
                                    tutorialRiverObj.riverSpeed = 0;
                                    tutorialPhase++;
                                }
                                break;
                            case 10:
                                tempPos = cam.WorldToScreenPoint(tutorialObject.transform.position);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y), handSpeed);
                                if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
                                {
                                    tutorialRiverObj.SimulateDrag(handPosition);
						tutorialTime = pickupTime;
                                    tutorialPhase++;
                                }
                                break;
                            case 11:
                                tutorialTime -= Time.deltaTime;
                                if (tutorialTime <= 0)
                                    tutorialPhase++;
                                break;
                            case 12:
					handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.2f, handPosition.y), handSpeed);
                                tutorialRiverObj.SimulateDrag(handPosition);
                                if (handPosition.x <= Screen.width * 0.21f)
                                {
						tutorialTime = dropTime;
                                    tutorialPhase++;
                                }
                                break;
                            case 13:
                                tutorialTime -= Time.deltaTime;
                                if (tutorialTime <= 0)
                                {
                                    tutorialRiverObj.riverSpeed = 2;
                                    tutorialRiverObj.simulateDrop = true;
                                    tutorialPhase++;
                                    tutorialTime = 1;
                                }
                                break;
                            case 14:
                                tutorialTime -= Time.deltaTime;
                                if (tutorialTime <= 0)
                                {
                                    tutorialPhase++;
                                }
                                break;
                            case 15:
                                endPos = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
					handPosition = Vector2.Lerp(handPosition, endPos, handSpeed2);
                                if (Vector2.Distance(handPosition, endPos) < 1)
                                {
                                    state = "Instructions";
                                    cardsMng.VisualTutorialEnded();
                                }
                                break;
                        }
                        break;
                    case 2:
				switch (tutorialPhase)
				{
				case 0:
					tutorialObject = forest.Spawn(tutorialSpecial, false, true, false, false, false, objectSpeed);
					if(!tutorialObject)
						tutorialObject = beach.Spawn(tutorialSpecial, false, true, false, false, false, objectSpeed);
					tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
					tutorialRiverObj.tutorial = true;
					tutorialPhase++;
					tutorialTime = 2;
					break;
				case 1:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
						tutorialPhase++;
					}
					break;
				case 2:
					Vector3 tempPos = new Vector3(Screen.width*0.7f,Screen.height*0.3f,0);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y), handSpeed);
					if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
					{
						tutorialPhase++;
					}
					break;
				case 3:
					Vector2 endPos = new Vector2(Screen.width * 1.2f,Screen.height * 1.1f);
					handPosition = Vector2.Lerp(handPosition, endPos, handSpeed2);
					if (!tutorialObject)
					{
						tutorialTime=2;
						tutorialPhase++;
					}
					break;
				case 4:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						state = "Instructions";
						cardsMng.VisualTutorialEnded();
					}
					break;
				}
                        break;
                    case 3:
				switch (tutorialPhase)
				{
				case 0:
					tutorialObject = forest.Spawn("Acorn", true, false, false, false, false, objectSpeed);
					tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
					tutorialRiverObj.tutorial = true;
					tutorialPhase++;
					tutorialTime = waitGrab;
					break;
				case 1:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
						
						tutorialRiverObj.riverSpeed = 0;
						tutorialPhase++;
					}
					break;
				case 2:
					Vector3 tempPos = cam.WorldToScreenPoint(tutorialObject.transform.position);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y), handSpeed);
					if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
					{
						tutorialRiverObj.SimulateDrag(handPosition);
						tutorialTime = pickupTime;
						tutorialPhase++;
					}
					break;
				case 3:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
						tutorialPhase++;
					break;
				case 4:
					handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.8f, handPosition.y), handSpeed);
					tutorialRiverObj.SimulateDrag(handPosition);
					if (handPosition.x >= Screen.width * 0.79f)
					{
						tutorialTime = dropTime;
						tutorialPhase++;
					}
					break;
				case 5:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialRiverObj.riverSpeed = 2;
						tutorialRiverObj.simulateDrop = true;
						tutorialPhase++;
						tutorialTime = 1;
					}
					break;
				case 6:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialPhase++;
					}
					break;
				case 7:
					Vector2 endPos = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
					handPosition = Vector2.Lerp(handPosition, endPos, handSpeed2);
					if (Vector2.Distance(handPosition, endPos) < 1)
					{
						tutorialPhase++;
					}
					break;
				case 8:
					tutorialObject = forest.Spawn("Acorn", false, false, true, false, false,objectSpeed);
					tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
					tutorialRiverObj.tutorial = true;
					tutorialPhase++;
					tutorialTime = waitGrab;
					break;
				case 9:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
						tutorialRiverObj.riverSpeed = 0;
						tutorialPhase++;
					}
					break;
				case 10:
					tempPos = cam.WorldToScreenPoint(tutorialObject.transform.position);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y), handSpeed);
					if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
					{
						tutorialRiverObj.SimulateDrag(handPosition);
						tutorialTime = pickupTime;
						tutorialPhase++;
					}
					break;
				case 11:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
						tutorialPhase++;
					break;
				case 12:
					handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.2f, handPosition.y), handSpeed);
					tutorialRiverObj.SimulateDrag(handPosition);
					if (handPosition.x <= Screen.width * 0.21f)
					{
						tutorialTime = dropTime;
						tutorialPhase++;
					}
					break;
				case 13:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialRiverObj.riverSpeed = 2;
						tutorialRiverObj.simulateDrop = true;
						tutorialPhase++;
						tutorialTime = 1;
					}
					break;
				case 14:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialPhase++;
					}
					break;
				case 15:
					endPos = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
					handPosition = Vector2.Lerp(handPosition, endPos, handSpeed2);
					if (Vector2.Distance(handPosition, endPos) < 1)
					{
						state = "Instructions";
						cardsMng.VisualTutorialEnded();
					}
					break;
				}
                        break;
                    case 4:
				switch (tutorialPhase)
				{
				case 0:
					tutorialObject = forest.Spawn("Acorn", false, false, false, false, false, objectSpeed);
					tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
					tutorialRiverObj.tutorial = true;
					tutorialPhase++;
					tutorialTime = waitGrab;
					break;
				case 1:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
						
						tutorialRiverObj.riverSpeed = 0;
						tutorialPhase++;
					}
					break;
				case 2:
					Vector3 tempPos = cam.WorldToScreenPoint(tutorialObject.transform.position);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y), handSpeed);
					if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
					{
						tutorialRiverObj.SimulateDrag(handPosition);
						tutorialTime = pickupTime;
						tutorialPhase++;
					}
					break;
				case 3:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
						tutorialPhase++;
					break;
				case 4:
					handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.2f, handPosition.y), handSpeed);
					tutorialRiverObj.SimulateDrag(handPosition);
					if (handPosition.x <= Screen.width * 0.21f)
					{
						tutorialTime = dropTime;
						tutorialPhase++;
					}
					break;
				case 5:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialRiverObj.riverSpeed = 2;
						tutorialRiverObj.simulateDrop = true;
						tutorialPhase++;
						tutorialTime = 1;
					}
					break;
				case 6:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialPhase++;
					}
					break;
				case 7:
					Vector2 endPos = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
					handPosition = Vector2.Lerp(handPosition, endPos, handSpeed2);
					if (Vector2.Distance(handPosition, endPos) < 1)
					{
						tutorialPhase++;
					}
					break;
				case 8:
					tutorialObject = forest.Spawn("Acorn", true, false, true, false, false, objectSpeed);
					tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
					tutorialRiverObj.tutorial = true;
					tutorialPhase++;
					tutorialTime = waitGrab;
					break;
				case 9:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
						tutorialRiverObj.riverSpeed = 0;
						tutorialPhase++;
					}
					break;
				case 10:
					tempPos = cam.WorldToScreenPoint(tutorialObject.transform.position);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y), handSpeed);
					if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
					{
						tutorialRiverObj.SimulateDrag(handPosition);
						tutorialTime = pickupTime;
						tutorialPhase++;
					}
					break;
				case 11:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
						tutorialPhase++;
					break;
				case 12:
					handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.8f, handPosition.y), handSpeed);
					tutorialRiverObj.SimulateDrag(handPosition);
					if (handPosition.x >= Screen.width * 0.79f)
					{
						tutorialTime = dropTime;
						tutorialPhase++;
					}
					break;
				case 13:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialRiverObj.riverSpeed = 2;
						tutorialRiverObj.simulateDrop = true;
						tutorialPhase++;
						tutorialTime = 1;
					}
					break;
				case 14:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialPhase++;
					}
					break;
				case 15:
					endPos = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
					handPosition = Vector2.Lerp(handPosition, endPos, handSpeed2);
					if (Vector2.Distance(handPosition, endPos) < 1)
					{
						state = "Instructions";
						cardsMng.VisualTutorialEnded();
					}
					break;
				}
                        break;
                    case 5:
				switch (tutorialPhase)
				{
				case 0:
					tutorialObject = beach.Spawn("Star", false, false, false, false, false, objectSpeed);
					tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
					tutorialRiverObj.tutorial = true;
					tutorialPhase++;
					tutorialTime = waitGrab;
					break;
				case 1:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
						
						tutorialRiverObj.riverSpeed = 0;
						tutorialPhase++;
					}
					break;
				case 2:
					Vector3 tempPos = cam.WorldToScreenPoint(tutorialObject.transform.position);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y), handSpeed);
					if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
					{
						tutorialRiverObj.SimulateDrag(handPosition);
						tutorialTime = pickupTime;
						tutorialPhase++;
					}
					break;
				case 3:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
						tutorialPhase++;
					break;
				case 4:
					handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.8f, handPosition.y), handSpeed);
					tutorialRiverObj.SimulateDrag(handPosition);
					if (handPosition.x >= Screen.width * 0.79f)
					{
						tutorialTime = dropTime;
						tutorialPhase++;
					}
					break;
				case 5:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialRiverObj.riverSpeed = 2;
						tutorialRiverObj.simulateDrop = true;
						tutorialPhase++;
						tutorialTime = 1;
					}
					break;
				case 6:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialPhase++;
					}
					break;
				case 7:
					Vector2 endPos = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
					handPosition = Vector2.Lerp(handPosition, endPos, handSpeed2);
					if (Vector2.Distance(handPosition, endPos) < 1)
					{
						tutorialPhase++;
					}
					break;
				case 8:
					tutorialObject = beach.Spawn("Star", false, false, false, true, false, objectSpeed);
					tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
					tutorialRiverObj.tutorial = true;
					tutorialPhase++;
					tutorialTime = waitGrab;
					break;
				case 9:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
						tutorialRiverObj.riverSpeed = 0;
						tutorialPhase++;
					}
					break;
				case 10:
					tempPos = cam.WorldToScreenPoint(tutorialObject.transform.position);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y), handSpeed);
					if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
					{
						tutorialRiverObj.SimulateDrag(handPosition);
						tutorialTime = pickupTime;
						tutorialPhase++;
					}
					break;
				case 11:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
						tutorialPhase++;
					break;
				case 12:
					handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.2f, handPosition.y), handSpeed);
					tutorialRiverObj.SimulateDrag(handPosition);
					if (handPosition.x <= Screen.width * 0.21f)
					{
						tutorialTime = dropTime;
						tutorialPhase++;
					}
					break;
				case 13:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialRiverObj.riverSpeed = 2;
						tutorialRiverObj.simulateDrop = true;
						tutorialPhase++;
						tutorialTime = 1;
					}
					break;
				case 14:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialPhase++;
					}
					break;
				case 15:
					endPos = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
					handPosition = Vector2.Lerp(handPosition, endPos, handSpeed2);
					if (Vector2.Distance(handPosition, endPos) < 1)
					{
						state = "Instructions";
						cardsMng.VisualTutorialEnded();
					}
					break;
				}
                        break;
                    case 6:
				switch (tutorialPhase)
				{
				case 0:
					tutorialObject = forest.Spawn("Acorn", false, false, false, false, false, objectSpeed);
					tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
					tutorialRiverObj.tutorial = true;
					tutorialPhase++;
					tutorialTime = waitGrab;
					break;
				case 1:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
						
						tutorialRiverObj.riverSpeed = 0;
						tutorialPhase++;
					}
					break;
				case 2:
					Vector3 tempPos = cam.WorldToScreenPoint(tutorialObject.transform.position);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y), handSpeed);
					if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
					{
						tutorialRiverObj.SimulateDrag(handPosition);
						tutorialTime = pickupTime;
						tutorialPhase++;
					}
					break;
				case 3:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
						tutorialPhase++;
					break;
				case 4:
					handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.2f, handPosition.y), handSpeed);
					tutorialRiverObj.SimulateDrag(handPosition);
					if (handPosition.x <= Screen.width * 0.21f)
					{
						tutorialTime = dropTime;
						tutorialPhase++;
					}
					break;
				case 5:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialRiverObj.riverSpeed = 2;
						tutorialRiverObj.simulateDrop = true;
						tutorialPhase++;
						tutorialTime = 1;
					}
					break;
				case 6:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialPhase++;
					}
					break;
				case 7:
					Vector2 endPos = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
					handPosition = Vector2.Lerp(handPosition, endPos, handSpeed2);
					if (Vector2.Distance(handPosition, endPos) < 1)
					{
						tutorialPhase++;
					}
					break;
				case 8:
					tutorialObject = forest.Spawn("Acorn", false, false, false, false, true, objectSpeed);
					tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
					tutorialRiverObj.tutorial = true;
					tutorialPhase++;
					tutorialTime = waitGrab;
					break;
				case 9:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
						tutorialRiverObj.riverSpeed = 0;
						tutorialPhase++;
					}
					break;
				case 10:
					tempPos = cam.WorldToScreenPoint(tutorialObject.transform.position);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y),handSpeed);
					if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
					{
						tutorialRiverObj.SimulateDrag(handPosition);
						tutorialTime = pickupTime;
						tutorialPhase++;
					}
					break;
				case 11:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
						tutorialPhase++;
					break;
				case 12:
					handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.8f, handPosition.y), handSpeed);
					tutorialRiverObj.SimulateDrag(handPosition);
					if (handPosition.x >= Screen.width * 0.79f)
					{
						tutorialTime = dropTime;
						tutorialPhase++;
					}
					break;
				case 13:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialRiverObj.riverSpeed = 2;
						tutorialRiverObj.simulateDrop = true;
						tutorialPhase++;
						tutorialTime = 1;
					}
					break;
				case 14:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialPhase++;
					}
					break;
				case 15:
					endPos = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
					handPosition = Vector2.Lerp(handPosition, endPos, handSpeed2);
					if (Vector2.Distance(handPosition, endPos) < 1)
					{
						state = "Instructions";
						cardsMng.VisualTutorialEnded();
					}
					break;
				}
                        break;
                    case 7:
				switch (tutorialPhase)
				{
				case 0:
					tutorialObject = forest.Spawn("Acorn", false, false, false, false, false, objectSpeed);
					tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
					tutorialRiverObj.tutorial = true;
					tutorialPhase++;
					tutorialTime = waitGrab;
					break;
				case 1:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
						
						tutorialRiverObj.riverSpeed = 0;
						tutorialPhase++;
					}
					break;
				case 2:
					Vector3 tempPos = cam.WorldToScreenPoint(tutorialObject.transform.position);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y), handSpeed);
					if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
					{
						tutorialRiverObj.SimulateDrag(handPosition);
						tutorialTime = pickupTime;
						tutorialPhase++;
					}
					break;
				case 3:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
						tutorialPhase++;
					break;
				case 4:
					handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.2f, handPosition.y), handSpeed);
					tutorialRiverObj.SimulateDrag(handPosition);
					if (handPosition.x <= Screen.width * 0.21f)
					{
						tutorialTime = dropTime;
						tutorialPhase++;
					}
					break;
				case 5:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialRiverObj.riverSpeed = 2;
						tutorialRiverObj.simulateDrop = true;
						tutorialPhase++;
						tutorialTime = 0.5f;
					}
					break;
				case 6:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialPhase++;
					}
					break;
				case 7:
					Vector2 endPos = new Vector2(Screen.width * 0.5f,Screen.height * 1.2f);
					handPosition = Vector2.Lerp(handPosition, endPos, handSpeed2);
					if (Vector2.Distance(handPosition, endPos) < 1)
					{
						tutorialPhase++;
					}
					break;
				case 8:
					left=true;
					tutorialObject = forest.Spawn(tutorialSpecial, false, false, false, false, false, objectSpeed);
					if(!tutorialObject)
					{
						left=false;
						tutorialObject = beach.Spawn(tutorialSpecial, false, false, false, false, false, objectSpeed);
					}
					tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
					tutorialRiverObj.tutorial = true;
					tutorialPhase++;
					tutorialTime = waitGrab;
					break;
				case 9:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
						tutorialRiverObj.riverSpeed = 0;
						tutorialPhase++;
					}
					break;
				case 10:
					tempPos = cam.WorldToScreenPoint(tutorialObject.transform.position);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y), handSpeed);
					if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
					{
						tutorialRiverObj.SimulateDrag(handPosition);
						tutorialTime = pickupTime;
						tutorialPhase++;
					}
					break;
				case 11:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
						tutorialPhase++;
					break;
				case 12:
					if(left)
					{
						handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.2f, handPosition.y), handSpeed);
						if (handPosition.x <= Screen.width * 0.21f)
						{
							tutorialTime = dropTime;
							tutorialPhase++;
						}
					}else{
						handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.8f, handPosition.y), handSpeed);
						if (handPosition.x >= Screen.width * 0.79f)
						{
							tutorialTime = dropTime;
							tutorialPhase++;
						}
					}
					tutorialRiverObj.SimulateDrag(handPosition);
					break;
				case 13:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialRiverObj.riverSpeed = 2;
						tutorialRiverObj.simulateDrop = true;
						tutorialPhase++;
						tutorialTime = 0.5f;
					}
					break;
				case 14:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialPhase++;
					}
					break;
				case 15:
					endPos = new Vector2(Screen.width * 0.5f,Screen.height * 1.2f);
					handPosition = Vector2.Lerp(handPosition, endPos,handSpeed2);
					if (Vector2.Distance(handPosition, endPos) < 1)
					{
						tutorialPhase++;
					}
					break;
				case 16:
					tutorialObject = forest.Spawn("Acorn", false, true, false, false, false, objectSpeed);
					tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
					tutorialRiverObj.tutorial = true;
					tutorialPhase++;
					tutorialTime = 2;
					break;
				case 17:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
						tutorialPhase++;
					}
					break;
				case 18:
					tempPos = new Vector3(Screen.width*0.7f,Screen.height*0.3f,0);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y), handSpeed);
					if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
					{
						tutorialPhase++;
					}
					break;
				case 19:
					endPos = new Vector2(Screen.width * 0.7f,Screen.height * 1.2f);
					handPosition = Vector2.Lerp(handPosition, endPos, handSpeed2);
					if (!tutorialObject)
					{
						tutorialTime=2;
						tutorialPhase++;
					}
					break;
				case 20:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
						tutorialPhase++;
					break;
				case 21:
					state = "Instructions";
					cardsMng.VisualTutorialEnded();
					break;
				}
                        break;
                    case 8:
				switch (tutorialPhase)
				{
				case 0:
					tutorialObject = forest.Spawn("Acorn", true, false, false, false, false, objectSpeed);
					tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
					tutorialRiverObj.tutorial = true;
					tutorialPhase++;
					tutorialTime = waitGrab;
					break;
				case 1:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
						
						tutorialRiverObj.riverSpeed = 0;
						tutorialPhase++;
					}
					break;
				case 2:
					Vector3 tempPos = cam.WorldToScreenPoint(tutorialObject.transform.position);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y), handSpeed);
					if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
					{
						tutorialRiverObj.SimulateDrag(handPosition);
						tutorialTime = pickupTime;
						tutorialPhase++;
					}
					break;
				case 3:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
						tutorialPhase++;
					break;
				case 4:
					handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.8f, handPosition.y), handSpeed);
					tutorialRiverObj.SimulateDrag(handPosition);
					if (handPosition.x >= Screen.width * 0.79f)
					{
						tutorialTime = dropTime;
						tutorialPhase++;
					}
					break;
				case 5:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialRiverObj.riverSpeed = 2;
						tutorialRiverObj.simulateDrop = true;
						tutorialPhase++;
						tutorialTime = 0.5f;
					}
					break;
				case 6:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialPhase++;
					}
					break;
				case 7:
					Vector2 endPos = new Vector2(Screen.width * 0.5f,Screen.height * 1.2f);
					handPosition = Vector2.Lerp(handPosition, endPos,handSpeed2);
					if (Vector2.Distance(handPosition, endPos) < 1)
					{
						tutorialPhase++;
					}
					break;
				case 8:
					left=true;
					tutorialObject = forest.Spawn(tutorialSpecial, false, false, false, false, false, objectSpeed);
					if(!tutorialObject)
					{
						left=false;
						tutorialObject = beach.Spawn(tutorialSpecial, false, false, false, false, false, objectSpeed);
					}
					tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
					tutorialRiverObj.tutorial = true;
					tutorialPhase++;
					tutorialTime = waitGrab;
					break;
				case 9:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
						tutorialRiverObj.riverSpeed = 0;
						tutorialPhase++;
					}
					break;
				case 10:
					tempPos = cam.WorldToScreenPoint(tutorialObject.transform.position);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y),handSpeed);
					if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
					{
						tutorialRiverObj.SimulateDrag(handPosition);
						tutorialTime = pickupTime;
						tutorialPhase++;
					}
					break;
				case 11:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
						tutorialPhase++;
					break;
				case 12:
					if(left)
					{
						handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.2f, handPosition.y), handSpeed);
						if (handPosition.x <= Screen.width * 0.21f)
						{
							tutorialTime = dropTime;
							tutorialPhase++;
						}
					}else{
						handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.8f, handPosition.y), handSpeed);
						if (handPosition.x >= Screen.width * 0.79f)
						{
							tutorialTime = dropTime;
							tutorialPhase++;
						}
					}
					tutorialRiverObj.SimulateDrag(handPosition);
					break;
				case 13:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialRiverObj.riverSpeed = 2;
						tutorialRiverObj.simulateDrop = true;
						tutorialPhase++;
						tutorialTime = 0.5f;
					}
					break;
				case 14:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialPhase++;
					}
					break;
				case 15:
					endPos = new Vector2(Screen.width * 0.5f,Screen.height * 1.2f);
					handPosition = Vector2.Lerp(handPosition, endPos, handSpeed2);
					if (Vector2.Distance(handPosition, endPos) < 1)
					{
						tutorialPhase++;
					}
					break;
				case 16:
					tutorialObject = forest.Spawn("Acorn", false, false, false, false, false, objectSpeed);
					tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
					tutorialRiverObj.tutorial = true;
					tutorialPhase++;
					tutorialTime = waitGrab;
					break;
				case 17:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
						
						tutorialRiverObj.riverSpeed = 0;
						tutorialPhase++;
					}
					break;
				case 18:
					tempPos = cam.WorldToScreenPoint(tutorialObject.transform.position);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y), handSpeed);
					if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
					{
						tutorialRiverObj.SimulateDrag(handPosition);
						tutorialTime = pickupTime;
						tutorialPhase++;
					}
					break;
				case 19:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
						tutorialPhase++;
					break;
				case 20:
					handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.2f, handPosition.y), handSpeed);
					tutorialRiverObj.SimulateDrag(handPosition);
					if (handPosition.x <= Screen.width * 0.21f)
					{
						tutorialTime = dropTime;
						tutorialPhase++;
					}
					break;
				case 21:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialRiverObj.riverSpeed = 2;
						tutorialRiverObj.simulateDrop = true;
						tutorialPhase++;
						tutorialTime = 0.5f;
					}
					break;
				case 22:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialPhase++;
					}
					break;
				case 23:
					endPos = new Vector2(Screen.width * 0.5f,Screen.height * 1.2f);
					handPosition = Vector2.Lerp(handPosition, endPos, handSpeed2);
					if (Vector2.Distance(handPosition, endPos) < 1)
					{
						tutorialPhase++;
					}
					break;
				case 24:
					state = "Instructions";
					cardsMng.VisualTutorialEnded();
					break;
				}
                        break;
                    case 9:
				switch (tutorialPhase)
				{
				case 0:
					tutorialObject = forest.Spawn("Acorn", false, false, false, false, false, objectSpeed);
					tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
					tutorialRiverObj.tutorial = true;
					tutorialPhase++;
					tutorialTime = waitGrab;
					break;
				case 1:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
						
						tutorialRiverObj.riverSpeed = 0;
						tutorialPhase++;
					}
					break;
				case 2:
					Vector3 tempPos = cam.WorldToScreenPoint(tutorialObject.transform.position);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y), handSpeed);
					if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
					{
						tutorialRiverObj.SimulateDrag(handPosition);
						tutorialTime = pickupTime;
						tutorialPhase++;
					}
					break;
				case 3:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
						tutorialPhase++;
					break;
				case 4:
					handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.2f, handPosition.y), handSpeed);
					tutorialRiverObj.SimulateDrag(handPosition);
					if (handPosition.x <= Screen.width * 0.21f)
					{
						tutorialTime = dropTime;
						tutorialPhase++;
					}
					break;
				case 5:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialRiverObj.riverSpeed = 2;
						tutorialRiverObj.simulateDrop = true;
						tutorialPhase++;
						tutorialTime = 0.5f;
					}
					break;
				case 6:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialPhase++;
					}
					break;
				case 7:
					Vector2 endPos = new Vector2(Screen.width * 0.5f,Screen.height * 1.2f);
					handPosition = Vector2.Lerp(handPosition, endPos, handSpeed2);
					if (Vector2.Distance(handPosition, endPos) < 1)
					{
						tutorialPhase++;
					}
					break;
				case 8:
					left=true;
					tutorialObject = forest.Spawn(tutorialSpecial, false, false, false, false, false, objectSpeed);
					if(!tutorialObject)
					{
						left=false;
						tutorialObject = beach.Spawn(tutorialSpecial, false, false, false, false, false, objectSpeed);
					}
					tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
					tutorialRiverObj.tutorial = true;
					tutorialPhase++;
					tutorialTime = waitGrab;
					break;
				case 9:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
						tutorialRiverObj.riverSpeed = 0;
						tutorialPhase++;
					}
					break;
				case 10:
					tempPos = cam.WorldToScreenPoint(tutorialObject.transform.position);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y), handSpeed);
					if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
					{
						tutorialRiverObj.SimulateDrag(handPosition);
						tutorialTime = pickupTime;
						tutorialPhase++;
					}
					break;
				case 11:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
						tutorialPhase++;
					break;
				case 12:
					if(left)
					{
						handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.2f, handPosition.y), handSpeed);
						if (handPosition.x <= Screen.width * 0.21f)
						{
							tutorialTime = dropTime;
							tutorialPhase++;
						}
					}else{
						handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.8f, handPosition.y), handSpeed);
						if (handPosition.x >= Screen.width * 0.79f)
						{
							tutorialTime = dropTime;
							tutorialPhase++;
						}
					}
					tutorialRiverObj.SimulateDrag(handPosition);
					break;
				case 13:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialRiverObj.riverSpeed = 2;
						tutorialRiverObj.simulateDrop = true;
						tutorialPhase++;
						tutorialTime = 0.5f;
					}
					break;
				case 14:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialPhase++;
					}
					break;
				case 15:
					endPos = new Vector2(Screen.width * 0.5f,Screen.height * 1.2f);
					handPosition = Vector2.Lerp(handPosition, endPos, handSpeed2);
					if (Vector2.Distance(handPosition, endPos) < 1)
					{
						tutorialPhase++;
					}
					break;
				case 16:
					tutorialObject = forest.Spawn("Acorn", true, false, false, false, false, objectSpeed);
					tutorialRiverObj = tutorialObject.GetComponent<RiverObject>();
					tutorialRiverObj.tutorial = true;
					tutorialPhase++;
					tutorialTime = waitGrab;
					break;
				case 17:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						handPosition = new Vector2(Screen.height * 0.7f, Screen.width * 1.2f);
						
						tutorialRiverObj.riverSpeed = 0;
						tutorialPhase++;
					}
					break;
				case 18:
					tempPos = cam.WorldToScreenPoint(tutorialObject.transform.position);
					handPosition = Vector2.Lerp(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y), handSpeed);
					if (Vector2.Distance(handPosition, new Vector2(tempPos.x, Screen.height - tempPos.y)) < 1)
					{
						tutorialRiverObj.SimulateDrag(handPosition);
						tutorialTime = pickupTime;
						tutorialPhase++;
					}
					break;
				case 19:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
						tutorialPhase++;
					break;
				case 20:
					handPosition = Vector2.Lerp(handPosition, new Vector2(Screen.width * 0.8f, handPosition.y), handSpeed);
					tutorialRiverObj.SimulateDrag(handPosition);
					if (handPosition.x >= Screen.width * 0.79f)
					{
						tutorialTime = dropTime;
						tutorialPhase++;
					}
					break;
				case 21:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialRiverObj.riverSpeed = 2;
						tutorialRiverObj.simulateDrop = true;
						tutorialPhase++;
						tutorialTime = 0.5f;
					}
					break;
				case 22:
					tutorialTime -= Time.deltaTime;
					if (tutorialTime <= 0)
					{
						tutorialPhase++;
					}
					break;
				case 23:
					endPos = new Vector2(Screen.width * 0.5f,Screen.height * 1.2f);
					handPosition = Vector2.Lerp(handPosition, endPos, handSpeed2);
					if (Vector2.Distance(handPosition, endPos) < 1)
					{
						tutorialPhase++;
					}
					break;
				case 24:
					state = "Instructions";
					cardsMng.VisualTutorialEnded();
					break;
				}
                        break;
                }
                break;
        }
    }

	void CalculateKiwis()
	{
		if (kiwis == -1)
		{
			kiwis = 0;
			int totalScore = (int)(((float)totalCorrectObjects / (float)(totalCorrectObjects + totalIncorrectObjects)) * 100);
			Debug.Log("Total Score" + totalScore.ToString());
			soundMng.pauseQueue=true;
			if(totalScore>80)
			{
				soundMng.AddSoundToQueue(35,false,false);
				kiwis=3;
			}else if(totalScore>60)
			{
				soundMng.AddSoundToQueue(34,false,false);
				kiwis=2;
			}else if(totalScore>20)
			{
				soundMng.AddSoundToQueue(33,false,false);
				kiwis=1;
			}
			int tempKiwis = sessionMng.activeKid.kiwis;
            sessionMng.activeKid.kiwis = tempKiwis + kiwis + scoreScript.GetExtraKiwis();
			sessionMng.SaveSession();
			if (kiwis == 0)
			{
				soundMng.AddSoundToQueue(26,false,false);
				soundMng.AddSoundToQueue(27,true,false);
			}
			else
			{
				soundMng.AddSoundToQueue(25,false,false);
				soundMng.AddSoundToQueue(27,true,false); 
			}
			GetComponent<ProfileSync>().UpdateProfile();
		}
	}

    public void addScore(bool correct)
    {
        if (correct)
        {
            correctObjects++;
			Debug.Log("Correct "+correctObjects);
            scoreEffects.DisplayScore(scoreScript.TempScoreSum(), configuration.sound == 1);
            scoreScript.prevCorMult++;
        }
        else
        {
			scoreEffects.DisplayError(configuration.sound==1);
            scoreScript.prevCorMult=0;
            incorrectObjects++;
			Debug.Log("Incorrect "+incorrectObjects);
        }
        pickTimer = pickTime;
    }

    public class riverSpecs
    {
        public bool reverse;
        public bool neutral;
        public bool forest;
        public bool beach;
        public bool specialL;
        public bool specialR;
        public riverSpecs()
        {

        }
    }
}
