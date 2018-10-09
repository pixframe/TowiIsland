using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class SoundTree : MonoBehaviour
{
	public Transform[] birdPositions;
	public AudioClip[]sounds;
	bool[]soundFlags;
	public GameObject[]nests;
	public GameObject[]birds;
	List<int> soundInstructions;
	int numberOfSounds = 3;
	int numberOfBirds = 3;
	public AudioSource source;
	public AudioSource tutSource;
	int sequencePlaying = -1;
	public string state;
	public GUIStyle style;
	public GUIStyle readyButton;
	public int birdsSet = 0;
	float tempLevelUp = 2;
	public GameObject[]branches;
	bool movingBranches;
	public bool branchesInPlace;
	GameSaveLoad loader;
	[HideInInspector]
	public SoundTreeConfiguration configuration;
	public int numLevelsPerBlock;
	public int level;
	public int subLevel;
	int todayLevels;
	int todayOffset;
	bool tutorial;
	public int tutorialPhase;
	public bool clickedBirds;
	float tutorialTime;
	int correctBird;
	float mapScale=1;
	List<Branch> branchesScript;
	float endTime=5;
	bool returnToIsland=false;
	SoundManager soundMng;
	bool startedSequence = false;
	bool resetBirds=false;
	public GUIStyle kiwiButton;
	public Texture loadingScreen;
	public Texture en_loadingScreen;
	public Texture loadingColor;
	bool fadeIn=false;
	bool fadeOut=false;
	float opacity=0;
    public Scores scoreScript;
    [HideInInspector]
    public float pickTime = 5;
    private float pickTimerTest;
	
	//Pause
	public GUIStyle pauseButton;
	public Texture2D pauseBackground;
	public GUIStyle pauseContinue;
	public GUIStyle pauseIsland;
	public GUIStyle pauseExit;
	public GUIStyle pauseText;
	public GUIStyle pauseButtons;
	public string stateBeforePause="";
	
	//Progress
	ProgressHandler saveHandler;
	int playedLevels=0;
	public int errors = 0;
	public int correct=0;
	int totalCorrect=0;
	int totalErrors=0;
	float levelTime=0;
	float totalTime=0;
	int failedLevels=0;
	bool repeatLevel=false;
	int repeatedLevels=0;
	public int[]birdsListenedPre;
	public int[]birdsListened;
	public float listeWait=3;
	float listeWaitRem;
	bool[] soundPlayed;

	public Texture2D scoreBG;
	public Texture2D scoreKiwi;
	public Texture2D scoreKiwiDisabled;
	public GUIStyle scoreStyle1;
	public GUIStyle scoreStyle2;
	public GUIStyle scoreStyle3;
	int kiwis=-1;
	int animationKiwis=0;
	public float kiwiAnimTime=0.5f;
	float kiwiAnimCurrTime;

	public ScoreEffects scoreEffects;
	SessionManager sessionMng;

	LanguageLoader language;

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
		source = Camera.main.GetComponent<AudioSource>();
		tutSource = GetComponent<AudioSource>();
		soundMng = GetComponent<SoundManager> ();
        scoreScript = GetComponent<Scores>();
		kiwiAnimCurrTime = kiwiAnimTime;
		mapScale = (float)Screen.height / (float)768;
		style.fontSize = (int)(style.fontSize * mapScale);
		readyButton.fontSize = (int)(readyButton.fontSize * mapScale);
		scoreStyle1.fontSize = (int)(scoreStyle1.fontSize * mapScale);
		scoreStyle2.fontSize = (int)(scoreStyle2.fontSize * mapScale);
		scoreStyle3.fontSize = (int)(scoreStyle3.fontSize * mapScale);
		kiwiButton.fontSize = (int)(kiwiButton.fontSize * mapScale);
		pauseButtons.fontSize = (int)(pauseButtons.fontSize * mapScale);
		correctBird = 0;
		tutorialPhase = 0;
		loader = GetComponent<GameSaveLoad> ();
		loader.Load (GameSaveLoad.game.soundTree);
		configuration = (SoundTreeConfiguration)loader.configuration;
		soundMng.PlaySound (0,0,configuration.sound);
		tutorialTime = 6f;
		clickedBirds = false;
		//tutorial = sessionMng.activeKid.arbolMusicalTutorial==1;
		tutorial = true;
		if(!tutorial)
			tutorialPhase=1;
		level = sessionMng.activeKid.birdsDifficulty;
		subLevel = sessionMng.activeKid.birdsLevel;
		todayLevels = 0;
		todayOffset=sessionMng.activeKid.playedBird;
		listeWaitRem = listeWait;
		birdsListenedPre = new int[5];
		birdsListened = new int[5];
		movingBranches = false;
		branchesInPlace = false;
		state = "Start";
		soundFlags = new bool[sounds.Length];
		soundInstructions = new List<int> ();
		if(configuration.sound==0)
		{
			//audio.volume=0;
			//audio.Stop();
			GameObject tempCam = GameObject.Find("Camera");
			tempCam.GetComponent<AudioSource>().volume=0;
			tempCam.GetComponent<AudioSource>().Stop();
		}else
		{
			GameObject tempCam = GameObject.Find("Camera");
			tempCam.GetComponent<AudioSource>().Play();
		}
		if(configuration.music==0)
		{
			
		}else
		{
			
		}
		saveHandler=(ProgressHandler)GetComponent(typeof(ProgressHandler));
		branchesScript = new List<Branch> ();
		for(int i=0;i<branches.Length;i++)
			branchesScript.Add(branches[i].GetComponent<Branch>());
		SetNests ();

	}
	
	void Update ()
	{
		if (returnToIsland && !saveHandler.saving) 
		{
			fadeIn=true;
		}
		totalTime += Time.deltaTime;
		levelTime += Time.deltaTime;
		switch (state) {
		case "Start":
			SetLevel ();
			state = "Birds";
			break;
		case "Birds":
			if (tutorial) 
			{
				tutorialTime -= Time.deltaTime;
				if (tutorialTime <= 0) 
				{
					switch (tutorialPhase) {
					case 0:
						soundMng.PlaySound (1,0,configuration.sound);
						tutorialPhase = 1;
						break;
					case 1:
						if (clickedBirds)
						{
							soundMng.PlaySound (2,0,configuration.sound);
							tutorialPhase = 2;
						}
						break;
					}
				}
			}
			else
			{
				if(clickedBirds)
				{
					tutorialPhase = 2;
				}
			}
			if(!source.isPlaying)
			{
				for(int i = 0; i < numberOfBirds; i++)
				{
					if(birdsListenedPre[i] == 1)
					{
						clickedBirds = true;
					}
					else
					{
						clickedBirds = false;
						break;
					}
				}
			}

			break;
		case "Listen":
			tutorialPhase = 1;
			clickedBirds = false;
			if(listeWaitRem<=0)
			{
				if (sequencePlaying > -1 && !GetComponent<AudioSource>().isPlaying)
				{
					if(!startedSequence)
					{
						GetComponent<AudioSource>().clip = sounds [soundInstructions [0]];
						GetComponent<AudioSource>().Play ();
						startedSequence=true;
					}
					else
					{
						if (sequencePlaying < soundInstructions.Count - 1) 
						{
							sequencePlaying++;
							GetComponent<AudioSource>().clip = sounds [soundInstructions [sequencePlaying]];
							GetComponent<AudioSource>().Play ();
						} 
						else 
						{
                            //pickTimer = pickTime;
							state = "Play";
							listeWaitRem=listeWait;
							if(tutorial){
								for(int i=0;i<birds.Length;i++)
								{
									birds[i].layer=2;
								}
								resetBirds=true;
								soundMng.PlaySound (4,0,configuration.sound);
							}
						}
					}
				}
			}
			else
			{
				listeWaitRem-=Time.deltaTime;
			}
			break;
		case "Play":
            pickTimerTest -= Time.deltaTime;
			if(!soundMng.IsPlaying()&&resetBirds)
			{
				resetBirds=false;
				for(int i=0;i<birds.Length;i++)
				{
					birds[i].layer=0;
				}
			}
			if (tutorial && tutorialPhase == 0) 
			{
				tutorialPhase++;
				birds [correctBird].GetComponent<DragBird> ().SetHighlight (true);
			}
			if (birdsSet == numberOfSounds) 
			{
				state = "LevelUp";
				if (tutorial)
				{
					tutorialPhase = 1;
					birds [correctBird].GetComponent<DragBird> ().SetHighlight (false);
				}
			}
			break;
		case "LevelUp":
			tempLevelUp -= Time.deltaTime;
			if(tempLevelUp<=0)
			{
				if (!movingBranches) 
				{
					movingBranches = true;
					for (int i=0; i<branches.Length; i++)
						branches [i].GetComponent<Branch> ().Move ();
					if (!tutorial) 
					{
						if (errors>= (float)numberOfSounds/2.0f)
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
						}else
						{
							failedLevels=0;
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
						sessionMng.activeKid.birdsDifficulty=level;
						sessionMng.activeKid.birdsLevel=subLevel;
						sessionMng.SaveSession();
						SaveLevelProgress();
					}
					else
					{
						tutorial=false;
						sessionMng.activeKid.arbolMusicalTutorial=0;
						sessionMng.SaveSession();
						SaveLevelProgress();
					}
					int lastBird=0;
					for (int i=0; i<configuration.levels [level].subLevels [subLevel].birds; i++)
					{
						lastBird++;
						birds [i].GetComponent<DragBird> ().Move ();
					}
					for (int i=lastBird; i<birds.Length; i++)
						birds [i].GetComponent<DragBird> ().SetBirdAway();
				}
				bool readyBranch=true;
				for(int i=0;i<branchesScript.Count;i++)
				{
					if(branchesScript[0].moving)
					{
						readyBranch=false;
						break;
					}
				}
				if (readyBranch) 
				{
					for (int i=0; i<birds.Length; i++)
						birds [i].GetComponent<DragBird> ().halo.SetActive(true);
					movingBranches = false;
					tempLevelUp = 2;
					state = "Start";
//					soundMng.PlaySound (3);
				}
			}
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
			/*endTime-=Time.deltaTime;
			if(endTime<=0){
				Application.LoadLevel("Archipielago");
			}*/
			break;
		}
	}
    public float GetPickTimer()
    {
        return pickTimerTest;
    }
    public void ResetPickTimer()
    {
        pickTimerTest = pickTime;
    }
	void CalculateKiwis()
	{
		if(kiwis==-1)
		{
			kiwis=0;
			int totalScore=(int)(((float)totalCorrect/(float)(totalCorrect+totalErrors))*100);
			Debug.Log("Total Score"+totalScore.ToString());
			soundMng.pauseQueue=true;
			if(totalScore>80)
			{
				soundMng.AddSoundToQueue(12,false,false);
				kiwis=3;
			}else if(totalScore>60)
			{
				soundMng.AddSoundToQueue(11,false,false);
				kiwis=2;
			}else if(totalScore>20)
			{
				soundMng.AddSoundToQueue(10,false,false);
				kiwis=1;
			}
			int tempKiwis=sessionMng.activeKid.kiwis;
            sessionMng.activeKid.kiwis = tempKiwis + kiwis + scoreScript.GetExtraKiwis();
			sessionMng.SaveSession();
			if(kiwis==0)
			{
				soundMng.AddSoundToQueue(8,false,false);
				soundMng.AddSoundToQueue(9,true,false);
			}
			else
			{
				soundMng.AddSoundToQueue(7,false,false);
				soundMng.AddSoundToQueue(9,true,false);
			}
			GetComponent<ProfileSync>().UpdateProfile();
		}
	}

	public void SetNests(){
		for (int i=0; i<nests.Length; i++) 
		{
			nests [i].GetComponent<Nest> ().soundIndex = -1;
			nests [i].SetActive (false);
		}
		for (int i=0; i<configuration.levels [level].subLevels [subLevel].nests; i++) 
		{
			nests [i].SetActive (true);
		}
	}
	
	void SaveLevelProgress()
	{
		correct = numberOfSounds;
		saveHandler.AddLevelData ("birds", numberOfBirds);
		saveHandler.AddLevelData ("nests", numberOfSounds);
		saveHandler.AddLevelData ("level", level);
		saveHandler.AddLevelData ("sublevel", subLevel);
		saveHandler.AddLevelData ("tutorial", tutorial);
		List<string> tempList = new List<string> ();
		for (int i=0; i<soundInstructions.Count; i++)
		{
			tempList.Add(sounds[soundInstructions[i]].name);
		}
		saveHandler.AddLevelData ("sound", tempList.ToArray ());
		tempList.Clear ();
		for (int i=0; i<numberOfBirds; i++) 
		{
			tempList.Add(sounds[birds[i].GetComponent<DragBird>().soundIndex].name);
		}
		saveHandler.AddLevelData ("birdSound", tempList.ToArray());
		saveHandler.AddLevelData ("time", (int)levelTime);
		tempList.Clear ();
		/*for (int i=0; i<numberOfBirds; i++) {
			tempList.Add(birdsListenedPre[i]);
		}*/
		saveHandler.AddLevelData ("birdListenedPre", birdsListenedPre);
		tempList.Clear ();
		/*for (int i=0; i<numberOfBirds; i++) {
			tempList.Add(birdsListened[i]);
		}*/
		saveHandler.AddLevelData ("birdListened", birdsListened);
		saveHandler.AddLevelData ("errors", errors);
		saveHandler.AddLevelData ("correct", correct);
		levelTime = 0;
		playedLevels++;
		todayLevels++;
		totalCorrect += correct;
		totalErrors += errors;
		sessionMng.activeKid.playedBird = todayLevels + todayOffset;
		//if (repeatLevel) 
		//{
		//	repeatedLevels++;
		//}
		if (failedLevels > 0) 
		{
			repeatedLevels++;
			repeatLevel=true;
			if(failedLevels>1)
				failedLevels=0;
		}

		//saveHandler.SetLevel();
		if (playedLevels+todayOffset >= numLevelsPerBlock) 
		{
			sessionMng.activeKid.blockedArbolMusical=1;
			SaveProgress (true);
			CalculateKiwis();
			state="CompletedActivity";
		}
		sessionMng.SaveSession ();
	}
	void SaveProgress(bool rank){
		if (playedLevels > 0) 
		{
			saveHandler.CreateSaveBlock ("ArbolMusical", (int)totalTime, playedLevels-repeatedLevels, repeatedLevels, playedLevels);
			saveHandler.AddLevelsToBlock ();
			saveHandler.PostProgress (rank);
			Debug.Log (saveHandler.ToString ());
		}
	}
	void SetLevel ()
	{
		birdsSet = 0;
		numberOfSounds = configuration.levels [level].subLevels [subLevel].nests;
		numberOfBirds = configuration.levels [level].subLevels [subLevel].birds;
		soundInstructions.Clear ();
		branchesInPlace = false;
		for (int i=0; i<numberOfBirds ; i++) 
		{
			DragBird tempDragBird=birds [i].GetComponent<DragBird>();
			tempDragBird.Reset();
			if(!tempDragBird.active)
			{
				tempDragBird.SetBird(birdPositions[i].position);
			}
			//birds [i].GetComponent<DragBird> ().Reset ();
			//birds [i].SetActive (false);
		}
		for (int i=0; i<birdsListenedPre.Length; i++) 
		{
			birdsListenedPre[i]=0;
		}
		for (int i=0; i<birdsListened.Length; i++)
		{
			birdsListened[i]=0;
		}
		correct = 0;
		errors = 0;
		levelTime = 0;
		//for (int i=0; i<numberOfBirds; i++) {
		//birds [i].SetActive (true);
		//}
		for (int i=0; i<soundFlags.Length; i++) 
		{
			soundFlags [i] = false;	
		}
		int[] correctSounds = new int[numberOfSounds];
		for (int i=0; i<correctSounds.Length; i++)
		{
			correctSounds[i]=-1;
		}
		for(int i=0;i<correctSounds.Length;i++)
		{
			int random;
			bool found;
			do
			{
				found=false;
				random=Random.Range(0,numberOfBirds);
				for(int x=0;x<correctSounds.Length;x++)
				{
					if(correctSounds[x]==random)
					{
						found=true;
						break;
					}
				}
			}
			while(found);
			correctSounds[i]=random;
		}
		int index = 0;
		while (index<numberOfBirds) 
		{
			int random = Random.Range (0, configuration.categories[configuration.levels[level].subLevels[subLevel].category[index]].sounds.Length);
			int soundIndex=0;
			for(int i=0;i<sounds.Length;i++)
			{
				if(sounds[i].name==configuration.categories[configuration.levels[level].subLevels[subLevel].category[index]].sounds[random])
				{
					soundIndex=i;
					break;
				}
			}
			int randomBird = Random.Range (0, numberOfBirds);
			while (birds[randomBird].GetComponent<DragBird>().soundIndex!=-1)
				randomBird = Random.Range (0, numberOfBirds);
			correctBird = randomBird;
			for(int idx=0;idx<correctSounds.Length;idx++)
			{
				if(randomBird==correctSounds[idx])
				{
					nests [soundInstructions.Count].GetComponent<Nest> ().soundIndex = soundIndex;
					soundInstructions.Add (soundIndex);
				}
			}
			birds [randomBird].GetComponent<DragBird> ().soundIndex = soundIndex;
			index++;
		}
	}
	
	public void PlaySound (int index)
	{
//		AudioSource.PlayClipAtPoint (sounds [index], Camera.main.transform.position);	
		source.clip = sounds[index];
		source.Play();
	}
	
	void OnGUI ()
	{
		switch (state) 
		{
		case "Birds":
			if (tutorial) 
			{
				switch (tutorialPhase) {
				case 0:
					GUI.Label (new Rect (Screen.width*0.5f-400*mapScale, 100*mapScale, 800*mapScale, 100*mapScale), language.levelStrings[0], style);
					break;
				case 1:
					GUI.Label (new Rect (Screen.width*0.5f-400*mapScale, 100*mapScale, 800*mapScale, 100*mapScale), language.levelStrings[1], style);
					break;
				case 2:
					GUI.Label (new Rect (Screen.width*0.5f-400*mapScale, 100*mapScale, 800*mapScale, 100*mapScale), language.levelStrings[2], style);
					//GUI.Label (new Rect (300, 30, 100, 20), "Presiona el botón cuando estés listo.",style);
					if (GUI.Button (new Rect (Screen.width*0.5f+190*mapScale, 180*mapScale, 190*mapScale, 80*mapScale),language.levelStrings[3],readyButton)) {
						state = "Listen";
						tutorialPhase = 0;
						sequencePlaying = 0;
						startedSequence = false;
						if(soundInstructions.Count>1)
							soundMng.PlaySound(6,0,configuration.sound);
						else
							soundMng.PlaySound(5,0,configuration.sound);
					}
					break;
				}
			}
			else 
			{
				if(tutorialPhase == 2)
				{
					GUI.Label (new Rect (Screen.width*0.5f-400*mapScale, 100*mapScale, 800*mapScale, 100*mapScale), language.levelStrings[4], style);
					//GUI.Label (new Rect (300, 30, 100, 20), "Presiona el botón cuando estés listo.",style);
					if (GUI.Button (new Rect (Screen.width*0.5f+190*mapScale, 200*mapScale, 190*mapScale, 80*mapScale),language.levelStrings[3],readyButton)) 
					{
						state = "Listen";
						sequencePlaying = 0;
						startedSequence = false;
						if(soundInstructions.Count>1)
							soundMng.PlaySound(6,0,configuration.sound);
						else
							soundMng.PlaySound(5,0,configuration.sound);
					}
				}

			}
			break;
		case "Play":
			if (tutorial) 
			{
				GUI.Label (new Rect (Screen.width*0.5f-400*mapScale, 100*mapScale, 800*mapScale, 100*mapScale), language.levelStrings[5], style);
			}
			break;
		case "LevelUp":
			GUI.Label (new Rect (Screen.width*0.5f-400*mapScale, 100*mapScale, 800*mapScale, 100*mapScale), language.levelStrings[6], style);
			break;
		case "Listen":
			if (numberOfSounds > 1)
				GUI.Label (new Rect (Screen.width*0.5f-400*mapScale, 100*mapScale, 800*mapScale, 100*mapScale), language.levelStrings[7], style);
			else
				GUI.Label (new Rect (Screen.width*0.5f-400*mapScale, 100*mapScale, 800*mapScale, 100*mapScale), language.levelStrings[8], style);
			break;
		case "CompletedActivity":
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
				GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 150 * mapScale, 800 * mapScale, 100 * mapScale), language.levelStrings[12],scoreStyle1);
			else
				GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 150 * mapScale, 800 * mapScale, 100 * mapScale), language.levelStrings[13],scoreStyle1);
			if(kiwis==0)
				GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 90 * mapScale, 800 * mapScale, 100 * mapScale), language.levelStrings[14],scoreStyle2);
			else{
				if(kiwis>1)
					GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 90 * mapScale, 800 * mapScale, 100 * mapScale), language.levelStrings[15]+" "+kiwis.ToString()+" "+language.levelStrings[16],scoreStyle2);
				else
					GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 90 * mapScale, 800 * mapScale, 100 * mapScale), language.levelStrings[15]+" "+kiwis.ToString()+" "+language.levelStrings[17],scoreStyle2);
			}
			if(GUI.Button(new Rect(Screen.width*0.5f-80*mapScale,Screen.height*0.5f+110*mapScale,160*mapScale,60*mapScale),language.levelStrings[19],kiwiButton))
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
			float pauseY=Screen.height*0.5f-177*mapScale;
			GUI.DrawTexture(new Rect(0,pauseY,Screen.width,354*mapScale),pauseBackground);
			GUI.Label(new Rect(Screen.width*0.5f-100*mapScale,pauseY-40*mapScale,200*mapScale,60*mapScale),language.levelStrings[18],pauseText);
			if(GUI.Button(new Rect(Screen.width*0.5f-200*mapScale,pauseY+50*mapScale,366*mapScale,66*mapScale),"",pauseContinue))
			{
				Time.timeScale=1.0f;
				state=stateBeforePause;
			}
			else
				if(GUI.Button(new Rect(Screen.width*0.5f-200*mapScale,pauseY+140*mapScale,382*mapScale,66*mapScale),"",pauseIsland))
			{
				if(!returnToIsland)
				{
					SaveProgress(false);
					returnToIsland=true;
					Time.timeScale=1.0f;
				}
				//Application.LoadLevel("Archipielago");
			}
			GUI.Label(new Rect(Screen.width*0.5f-110*mapScale,pauseY+50*mapScale,366*mapScale,66*mapScale),language.levelStrings[20],pauseButtons);
			GUI.Label(new Rect(Screen.width*0.5f-110*mapScale,pauseY+140*mapScale,382*mapScale,66*mapScale),language.levelStrings[21],pauseButtons);
			/*else
				if(GUI.Button(new Rect(Screen.width*0.5f-200*mapScale,pauseY+230*mapScale,162*mapScale,67*mapScale),"",pauseExit))
			{
				Time.timeScale=1.0f;
				Application.Quit();
			}*/
			break;
		}
		if(state!="Pause"&&state!="CompletedActivity"){
			if(GUI.Button(new Rect(10*mapScale,10*mapScale,71*mapScale,62*mapScale),"",pauseButton))
			{
				Time.timeScale=0.0f;
				stateBeforePause=state;
				state="Pause";
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
