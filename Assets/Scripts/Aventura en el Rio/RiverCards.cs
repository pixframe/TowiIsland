using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RiverCards : MonoBehaviour {

	public Texture cardT;
	public Texture leftArrowST;
	public Texture leftArrowBT;
	public Texture rightArrowST;
	public Texture rightArrowBT;
	public Texture zonesST;
	public Texture zonesBT;
	public Texture zonesBIT;
	public Texture redBubbleT;
	public Texture blueBubbleT;
	public Texture whiteBubbleT;
	public Texture blockT;
	public Texture[] objects;
	public GUIStyle headerStyle;
	public GUIStyle descriptionStyle;
	float scale;
	public float smallObj=0.3f;
	public float bigObj=0.5f;
	List<Card> instructions;
	List<Vector2> positions;
	List<float> yAnimation;
	float yHide=0;
	public float cardWidth;
	int currentCard;
	float testTime;
	bool appear;
	bool disappear;
	bool hide;
	bool hidden;
	int tutorials;
	bool visualTutorial;
	MainRiver mainLogic;

	// Use this for initialization
	void Awake () {
		appear = false;
		disappear = false;
		hide = false;
		hidden = false;
		scale = Screen.height / 768.0f;
		headerStyle.fontSize = (int)((float)headerStyle.fontSize * scale);
		descriptionStyle.fontSize = (int)((float)descriptionStyle.fontSize * scale);
		instructions = new List<Card> ();
		positions=new List<Vector2>();
		yAnimation = new List<float> ();
		instructions.Clear ();
		positions.Clear ();
		yAnimation.Clear ();
		currentCard = -1;
		testTime = 3;
		visualTutorial = false;
		//tutorials = 0xfffffff;
		mainLogic=GetComponent<MainRiver> ();
	}

	void Start()
	{
		//AddInstructions (0,"Acomoda los objetos en el lado correcto.");
		//AddInstructions (1,"Acomoda los objetos del bosque en la playa y los de la playa en el bosque.");
		//AddInstructions (2,"Deja ir la BELLOTA.",objects[0]);
		//AddInstructions (3,"Pon los objetos en burbujas BLANCAS donde pertenecen.");
		//AddInstructions (4,"Pon los objetos en burbujas BLANCAS al revés.");
		//AddInstructions (5,"Pon los objetos en burbujas AZULES en el BOSQUE.");
		//AddInstructions (6,"Pon los objetos en burbujas ROJAS en la PLAYA.");
		//AddInstructions (7,"Deja ir el primer objeto después del TIBURÓN.",objects[0]);
		//AddInstructions (8,"Pon el primer objeto después de un TIBURÓN donde pertenece.",objects[0]);
		//AddInstructions (9,"Pon el primer objeto después de un TIBURÓN al revés.",objects[0]);
		//StartCards ();
	}

	public void SetCards()
	{
		tutorials = mainLogic.sessionMng.activeKid.rioTutorial;
	}
	// Update is called once per frame
	void Update () {
		if(appear)
		{
			for(int i=0;i<currentCard+1;i++)
			{
				yAnimation[i]-=700*Time.deltaTime;
				if(yAnimation[i]<0)
					yAnimation[i]=0;
			}
		}
		if(disappear)
		{
			for(int i=0;i<yAnimation.Count;i++)
			{
				yAnimation[i]-=700*Time.deltaTime;
				if(yAnimation[i]<-1000)
				{
					disappear=false;
				}
			}
			if(!disappear)
			{
				instructions.Clear ();
				positions.Clear ();
				yAnimation.Clear ();
				currentCard = -1;
			}
		}
		if(hide)
		{
			if(!hidden)
			{
				yHide+=900*Time.deltaTime;
				if(yHide>Screen.height)
				{
					hidden=true;
				}
				if(hidden)
				{
                    if (currentCard < instructions.Count)
                    {
                        mainLogic.StartVisualInstructions(instructions[currentCard].index, instructions[currentCard].keyObj);
                    }
				}
			}
		}else
		{
			if(hidden)
			{
				yHide-=700*Time.deltaTime;
				if(yHide<0)
				{
					yHide=0;
					hidden=false;
				}
				if(!hidden)
				{
					if(currentCard<instructions.Count-1)
					{
						mainLogic.soundMng.pauseQueue=false;
						currentCard++;
						if (((1 << instructions [currentCard].index) & tutorials)>0) {
							tutorials&=~(1 << instructions [currentCard].index);
							mainLogic.sessionMng.activeKid.rioTutorial=tutorials;
							mainLogic.sessionMng.SaveSession();
							visualTutorial=true;
							mainLogic.soundMng.pauseQueue=true;
						}else
						{
							visualTutorial=false;
							mainLogic.soundMng.pauseQueue=false;
						}
						mainLogic.soundMng.ContinueQueue ();
					}else
					{
						mainLogic.DisplayButton();
					}
				}
			}
		}
		/*testTime -= Time.deltaTime;
		if(testTime<0)
		{
			testTime=3;
			ClipEnded();
		}*/
	}

	public void AddInstructions(int instructionsIndex)
	{
		instructions.Add(new Card(instructionsIndex,"",null,""));
	}
	public void AddInstructions(int instructionsIndex, string instruction)
	{
		instructions.Add(new Card(instructionsIndex,instruction,null,""));
	}
	public void AddInstructions(int instructionsIndex, string instruction, Texture img)
	{
		instructions.Add(new Card(instructionsIndex,instruction,img,""));
	}
	public void AddInstructions(int instructionsIndex, string instruction, Texture img, string obj)
	{
		instructions.Add(new Card(instructionsIndex,instruction,img,obj));
	}

	public void StartCards()
	{
		float tempWidth = 0;
		for(int i=0;i<instructions.Count;i++)
		{
			yAnimation.Add(Screen.height);
			positions.Add (new Vector2 (Screen.width*0.5f+tempWidth - cardWidth*scale * (instructions.Count-1) * 0.5f, Screen.height * 0.5f));
			tempWidth += cardWidth*scale;
		}
		currentCard = 0;
		appear = true;
		if (((1 << instructions [currentCard].index) & tutorials)>0) {
			tutorials&=~(1 << instructions [currentCard].index);
			mainLogic.sessionMng.activeKid.rioTutorial=tutorials;
			mainLogic.sessionMng.SaveSession();
			visualTutorial=true;
			mainLogic.soundMng.pauseQueue=true;
		}else
		{
			visualTutorial=false;
			mainLogic.soundMng.pauseQueue=false;
		}
		mainLogic.soundMng.PlayQueue ();
	}
	public void DestroyCards()
	{
		appear = false;
		disappear = true;
	}

	void ClipEnded()
	{
		Debug.Log("ClipEnded");
		if(currentCard<instructions.Count-1)
		{
			if(visualTutorial)
			{
				StartCoroutine(StartVisual());
			}else{
				mainLogic.soundMng.pauseQueue=false;
				currentCard++;
				if (((1 << instructions [currentCard].index) & tutorials)>0) {
					tutorials&=~(1 << instructions [currentCard].index);
					mainLogic.sessionMng.activeKid.rioTutorial=tutorials;
					mainLogic.sessionMng.SaveSession();
					visualTutorial=true;
					mainLogic.soundMng.pauseQueue=true;
				}else
				{
					visualTutorial=false;
					mainLogic.soundMng.pauseQueue=false;
				}
				mainLogic.soundMng.ContinueQueue ();
			}
		}
	}

	IEnumerator StartVisual() {
		yield return new WaitForSeconds(2.0f);
		hide = true;
		//mainLogic.StartVisualInstructions(instructions[currentCard].index,instructions[currentCard].keyObj);
	}

	public void ContinueInstructions()
	{
		//tutorials |= 1 << instructions [currentCard].index;
		mainLogic.sessionMng.activeKid.rioTutorial = tutorials;
		mainLogic.sessionMng.SaveSession ();
		mainLogic.soundMng.ContinueQueue();
	}

	public void VisualTutorialEnded()
	{
		hide = false;
	}
	
	void InstructionsEnded()
	{
		if(visualTutorial)
		{
			StartCoroutine(StartVisual());
		}else
		{
			mainLogic.DisplayButton();
		}
	}

	void OnGUI()
	{
		if(mainLogic.state=="Instructions")
		{
			for(int i=0;i<instructions.Count;i++)
			{
				switch(instructions[i].index)
				{
					case 0:
						GUI.DrawTexture (new Rect (positions[i].x - 165 * scale, positions[i].y+yHide+yAnimation[i]- 207 * scale, 330 * scale, 414 * scale), cardT);
					GUI.DrawTexture (new Rect (positions[i].x - 131 * scale, positions[i].y+yHide+yAnimation[i] - 130 * scale, 262 * scale, 170 * scale), zonesBT);
					GUI.DrawTexture (new Rect (positions[i].x - 50 * scale*bigObj, positions[i].y+yHide+yAnimation[i] - 110 * scale*bigObj, 180 * scale*bigObj, 150 * scale*bigObj), objects[0]);
					GUI.DrawTexture (new Rect (positions[i].x - 50 * scale, positions[i].y+yHide+yAnimation[i] - 30 * scale, 46 * scale, 46 * scale), leftArrowBT);
					GUI.DrawTexture (new Rect (positions[i].x - 130 * scale*bigObj, positions[i].y+yHide+yAnimation[i] - 260 * scale*bigObj, 180 * scale*bigObj, 150 * scale*bigObj), objects[1]);
					GUI.DrawTexture (new Rect (positions[i].x +15* scale, positions[i].y+yHide+yAnimation[i] - 110 * scale, 46 * scale, 46 * scale), rightArrowBT);
					GUI.Label (new Rect (positions[i].x - 150 * scale, positions[i].y+yHide+yAnimation[i] - 185 * scale, 300 * scale, 50 * scale), mainLogic.language.levelStrings[43], headerStyle);
					GUI.Label (new Rect (positions[i].x - 165 * scale, positions[i].y+yHide+yAnimation[i] +50 * scale, 330 * scale, 200 * scale), instructions[i].instruction, descriptionStyle);
					break;
					case 1:
					GUI.DrawTexture (new Rect (positions[i].x - 165 * scale, positions[i].y+yHide+yAnimation[i] - 207 * scale, 330 * scale, 414 * scale), cardT);
					GUI.DrawTexture (new Rect (positions[i].x - 131 * scale, positions[i].y+yHide+yAnimation[i] - 130 * scale, 262 * scale, 170 * scale), zonesBT);
					GUI.DrawTexture (new Rect (positions[i].x - 50 * scale*bigObj, positions[i].y+yHide+yAnimation[i] - 110 * scale*bigObj, 180 * scale*bigObj, 150 * scale*bigObj), objects[1]);
					GUI.DrawTexture (new Rect (positions[i].x - 50 * scale, positions[i].y+yHide+yAnimation[i] - 30 * scale, 46 * scale, 46 * scale), leftArrowBT);
					GUI.DrawTexture (new Rect (positions[i].x - 130 * scale*bigObj, positions[i].y+yHide+yAnimation[i] - 260 * scale*bigObj, 180 * scale*bigObj, 150 * scale*bigObj), objects[0]);
					GUI.DrawTexture (new Rect (positions[i].x +15* scale, positions[i].y+yHide+yAnimation[i] - 110 * scale, 46 * scale, 46 * scale), rightArrowBT);
					GUI.Label (new Rect (positions[i].x - 150 * scale, positions[i].y+yHide+yAnimation[i] - 185 * scale, 300 * scale, 50 * scale), mainLogic.language.levelStrings[44], headerStyle);
					GUI.Label (new Rect (positions[i].x - 165 * scale, positions[i].y+yHide+yAnimation[i] +50 * scale, 330 * scale, 200 * scale), instructions[i].instruction, descriptionStyle);
					break;
					case 2:
					GUI.DrawTexture (new Rect (positions[i].x - 165 * scale, positions[i].y+yHide+yAnimation[i] - 207 * scale, 330 * scale, 414 * scale), cardT);
					GUI.DrawTexture (new Rect (positions[i].x - 131 * scale, positions[i].y+yHide+yAnimation[i] - 130 * scale, 262 * scale, 170 * scale), zonesBT);
					GUI.DrawTexture (new Rect (positions[i].x - 90 * scale*bigObj, positions[i].y+yHide+yAnimation[i] - 190 * scale*bigObj, 180 * scale*bigObj, 150 * scale*bigObj), instructions[i].img);
					GUI.DrawTexture (new Rect (positions[i].x - 50 * scale, positions[i].y+yHide+yAnimation[i] - 210 * scale*bigObj, 100 * scale, 100 * scale), blockT);
					GUI.Label (new Rect (positions[i].x - 150 * scale, positions[i].y+yHide+yAnimation[i] - 185 * scale, 300 * scale, 50 * scale), mainLogic.language.levelStrings[45], headerStyle);
					GUI.Label (new Rect (positions[i].x - 165 * scale, positions[i].y+yHide+yAnimation[i] +50 * scale, 330 * scale, 200 * scale), instructions[i].instruction, descriptionStyle);
					break;
					case 3:
					GUI.DrawTexture (new Rect (positions[i].x - 165 * scale, positions[i].y+yHide+yAnimation[i]- 207 * scale, 330 * scale, 414 * scale), cardT);
					GUI.DrawTexture (new Rect (positions[i].x - 131 * scale, positions[i].y+yHide+yAnimation[i] - 130 * scale, 262 * scale, 170 * scale), zonesBIT);
					GUI.DrawTexture (new Rect (positions[i].x - 110 * scale*smallObj, positions[i].y+yHide+yAnimation[i] - 110 * scale*smallObj, 180 * scale*smallObj, 150 * scale*smallObj), objects[0]);
					GUI.DrawTexture (new Rect (positions[i].x - 55 * scale, positions[i].y+yHide+yAnimation[i] - 110 * scale, 46 * scale, 46 * scale), leftArrowBT);
					GUI.DrawTexture (new Rect (positions[i].x - 40 * scale*smallObj, positions[i].y+yHide+yAnimation[i] - 370 * scale*smallObj, 180 * scale*smallObj, 150 * scale*smallObj), objects[0]);
					GUI.DrawTexture (new Rect (positions[i].x - 20 * scale, positions[i].y+yHide+yAnimation[i] - 400 * scale*smallObj, 70 * scale, 70 * scale), whiteBubbleT);
					GUI.DrawTexture (new Rect (positions[i].x +15* scale, positions[i].y+yHide+yAnimation[i] - 30 * scale, 46 * scale, 46 * scale), rightArrowBT);
					GUI.Label (new Rect (positions[i].x - 150 * scale, positions[i].y+yHide+yAnimation[i] - 185 * scale, 300 * scale, 50 * scale), mainLogic.language.levelStrings[46], headerStyle);
					GUI.Label (new Rect (positions[i].x - 165 * scale, positions[i].y+yHide+yAnimation[i] +50 * scale, 330 * scale, 200 * scale), instructions[i].instruction, descriptionStyle);
					break;
					case 4:
					GUI.DrawTexture (new Rect (positions[i].x - 165 * scale, positions[i].y+yHide+yAnimation[i]- 207 * scale, 330 * scale, 414 * scale), cardT);
					GUI.DrawTexture (new Rect (positions[i].x - 131 * scale, positions[i].y+yHide+yAnimation[i] - 130 * scale, 262 * scale, 170 * scale), zonesBT);
					GUI.DrawTexture (new Rect (positions[i].x - 50 * scale*smallObj, positions[i].y+yHide+yAnimation[i] - 110 * scale*smallObj, 180 * scale*smallObj, 150 * scale*smallObj), objects[0]);
					GUI.DrawTexture (new Rect (positions[i].x - 50 * scale, positions[i].y+yHide+yAnimation[i] - 30 * scale, 46 * scale, 46 * scale), leftArrowBT);
					GUI.DrawTexture (new Rect (positions[i].x - 110 * scale*smallObj, positions[i].y+yHide+yAnimation[i] - 370 * scale*smallObj, 180 * scale*smallObj, 150 * scale*smallObj), objects[0]);
					GUI.DrawTexture (new Rect (positions[i].x - 40 * scale, positions[i].y+yHide+yAnimation[i] - 400 * scale*smallObj, 70 * scale, 70 * scale), whiteBubbleT);
					GUI.DrawTexture (new Rect (positions[i].x +15* scale, positions[i].y+yHide+yAnimation[i] - 110 * scale, 46 * scale, 46 * scale), rightArrowBT);
					GUI.Label (new Rect (positions[i].x - 150 * scale, positions[i].y+yHide+yAnimation[i] - 185 * scale, 300 * scale, 50 * scale), mainLogic.language.levelStrings[46], headerStyle);
					GUI.Label (new Rect (positions[i].x - 165 * scale, positions[i].y+yHide+yAnimation[i] +50 * scale, 330 * scale, 200 * scale), instructions[i].instruction, descriptionStyle);
					break;
					case 5:
					GUI.DrawTexture (new Rect (positions[i].x - 165 * scale, positions[i].y+yHide+yAnimation[i]- 207 * scale, 330 * scale, 414 * scale), cardT);
					GUI.DrawTexture (new Rect (positions[i].x - 131 * scale, positions[i].y+yHide+yAnimation[i] - 130 * scale, 262 * scale, 170 * scale), zonesBIT);
					GUI.DrawTexture (new Rect (positions[i].x - 110 * scale*smallObj, positions[i].y+yHide+yAnimation[i] - 110 * scale*smallObj, 180 * scale*smallObj, 150 * scale*smallObj), objects[1]);
					GUI.DrawTexture (new Rect (positions[i].x - 55 * scale, positions[i].y+yHide+yAnimation[i] - 110 * scale, 46 * scale, 46 * scale), leftArrowBT);
					GUI.DrawTexture (new Rect (positions[i].x - 40 * scale*smallObj, positions[i].y+yHide+yAnimation[i] - 370 * scale*smallObj, 180 * scale*smallObj, 150 * scale*smallObj), objects[1]);
					GUI.DrawTexture (new Rect (positions[i].x - 20 * scale, positions[i].y+yHide+yAnimation[i] - 400 * scale*smallObj, 70 * scale, 70 * scale), blueBubbleT);
					GUI.DrawTexture (new Rect (positions[i].x +15* scale, positions[i].y+yHide+yAnimation[i] - 30 * scale, 46 * scale, 46 * scale), rightArrowBT);
					GUI.Label (new Rect (positions[i].x - 150 * scale, positions[i].y+yHide+yAnimation[i] - 185 * scale, 300 * scale, 50 * scale), mainLogic.language.levelStrings[47], headerStyle);
					GUI.Label (new Rect (positions[i].x - 165 * scale, positions[i].y+yHide+yAnimation[i] +50 * scale, 330 * scale, 200 * scale), instructions[i].instruction, descriptionStyle);
					break;
					case 6:
					GUI.DrawTexture (new Rect (positions[i].x - 165 * scale, positions[i].y+yHide+yAnimation[i]- 207 * scale, 330 * scale, 414 * scale), cardT);
					GUI.DrawTexture (new Rect (positions[i].x - 131 * scale, positions[i].y+yHide+yAnimation[i] - 130 * scale, 262 * scale, 170 * scale), zonesBT);
					GUI.DrawTexture (new Rect (positions[i].x - 50 * scale*smallObj, positions[i].y+yHide+yAnimation[i] - 110 * scale*smallObj, 180 * scale*smallObj, 150 * scale*smallObj), objects[0]);
					GUI.DrawTexture (new Rect (positions[i].x - 50 * scale, positions[i].y+yHide+yAnimation[i] - 30 * scale, 46 * scale, 46 * scale), leftArrowBT);
					GUI.DrawTexture (new Rect (positions[i].x - 110 * scale*smallObj, positions[i].y+yHide+yAnimation[i] - 370 * scale*smallObj, 180 * scale*smallObj, 150 * scale*smallObj), objects[0]);
					GUI.DrawTexture (new Rect (positions[i].x - 40 * scale, positions[i].y+yHide+yAnimation[i] - 400 * scale*smallObj, 70 * scale, 70 * scale), redBubbleT);
					GUI.DrawTexture (new Rect (positions[i].x +15* scale, positions[i].y+yHide+yAnimation[i] - 110 * scale, 46 * scale, 46 * scale), rightArrowBT);
					GUI.Label (new Rect (positions[i].x - 150 * scale, positions[i].y+yHide+yAnimation[i] - 185 * scale, 300 * scale, 50 * scale), mainLogic.language.levelStrings[48], headerStyle);
					GUI.Label (new Rect (positions[i].x - 165 * scale, positions[i].y+yHide+yAnimation[i] +50 * scale, 330 * scale, 200 * scale), instructions[i].instruction, descriptionStyle);
					break;
					case 7:
					GUI.DrawTexture (new Rect (positions[i].x - 165 * scale, positions[i].y+yHide+yAnimation[i]- 207 * scale, 330 * scale, 414 * scale), cardT);
					GUI.DrawTexture (new Rect (positions[i].x - 131 * scale, positions[i].y+yHide+yAnimation[i] - 130 * scale, 262 * scale, 170 * scale), zonesST);
					GUI.DrawTexture (new Rect (positions[i].x - 90 * scale*smallObj, positions[i].y+yHide+yAnimation[i] - 90 * scale*smallObj, 180 * scale*smallObj, 150 * scale*smallObj), objects[0]);
					GUI.DrawTexture (new Rect (positions[i].x - 70 * scale, positions[i].y+yHide+yAnimation[i] - 20 * scale, 46 * scale, 46 * scale), leftArrowST);
					GUI.DrawTexture (new Rect (positions[i].x - 90 * scale*smallObj, positions[i].y+yHide+yAnimation[i] - 420 * scale*smallObj, 180 * scale*smallObj, 150 * scale*smallObj), objects[0]);
						//GUI.DrawTexture (new Rect (positions[i].x +20* scale, positions[i].y+yAnimation[i] - 120 * scale, 46 * scale, 46 * scale), rightArrowST);
					GUI.DrawTexture (new Rect (positions[i].x - 90 * scale*smallObj, positions[i].y+yHide+yAnimation[i] - 260 * scale*smallObj, 180 * scale*smallObj, 150 * scale*smallObj), instructions[i].img);
					GUI.DrawTexture (new Rect (positions[i].x - 35 * scale, positions[i].y+yHide+yAnimation[i] - 460 * scale*smallObj, 70 * scale, 70 * scale), blockT);
					GUI.Label (new Rect (positions[i].x - 150 * scale, positions[i].y+yHide+yAnimation[i] - 185 * scale, 300 * scale, 50 * scale), mainLogic.language.levelStrings[45], headerStyle);
					GUI.Label (new Rect (positions[i].x - 165 * scale, positions[i].y+yHide+yAnimation[i] +50 * scale, 330 * scale, 200 * scale), instructions[i].instruction, descriptionStyle);
					break;
					case 8:
					GUI.DrawTexture (new Rect (positions[i].x - 165 * scale, positions[i].y+yHide+yAnimation[i]- 207 * scale, 330 * scale, 414 * scale), cardT);
					GUI.DrawTexture (new Rect (positions[i].x - 131 * scale, positions[i].y+yHide+yAnimation[i] - 130 * scale, 262 * scale, 170 * scale), zonesST);
					GUI.DrawTexture (new Rect (positions[i].x - 90 * scale*smallObj, positions[i].y+yHide+yAnimation[i] - 90 * scale*smallObj, 180 * scale*smallObj, 150 * scale*smallObj), objects[0]);
					GUI.DrawTexture (new Rect (positions[i].x - 70 * scale, positions[i].y+yHide+yAnimation[i] - 20 * scale, 46 * scale, 46 * scale), leftArrowST);
					GUI.DrawTexture (new Rect (positions[i].x - 90 * scale*smallObj, positions[i].y+yHide+yAnimation[i] - 420 * scale*smallObj, 180 * scale*smallObj, 150 * scale*smallObj), objects[0]);
					GUI.DrawTexture (new Rect (positions[i].x +20* scale, positions[i].y+yHide+yAnimation[i] - 120 * scale, 46 * scale, 46 * scale), rightArrowST);
					GUI.DrawTexture (new Rect (positions[i].x - 90 * scale*smallObj, positions[i].y+yHide+yAnimation[i] - 260 * scale*smallObj, 180 * scale*smallObj, 150 * scale*smallObj), instructions[i].img);
					GUI.Label (new Rect (positions[i].x - 150 * scale, positions[i].y+yHide+yAnimation[i] - 185 * scale, 300 * scale, 50 * scale), mainLogic.language.levelStrings[45], headerStyle);
					GUI.Label (new Rect (positions[i].x - 165 * scale, positions[i].y+yHide+yAnimation[i] +50 * scale, 330 * scale, 200 * scale), instructions[i].instruction, descriptionStyle);
					break;
					case 9:
					GUI.DrawTexture (new Rect (positions[i].x - 165 * scale, positions[i].y+yHide+yAnimation[i]- 207 * scale, 330 * scale, 414 * scale), cardT);
					GUI.DrawTexture (new Rect (positions[i].x - 131 * scale, positions[i].y+yHide+yAnimation[i] - 130 * scale, 262 * scale, 170 * scale), zonesST);
					GUI.DrawTexture (new Rect (positions[i].x - 90 * scale*smallObj, positions[i].y+yHide+yAnimation[i] - 90 * scale*smallObj, 180 * scale*smallObj, 150 * scale*smallObj), objects[1]);
					GUI.DrawTexture (new Rect (positions[i].x - 70 * scale, positions[i].y+yHide+yAnimation[i] - 20 * scale, 46 * scale, 46 * scale), leftArrowST);
					GUI.DrawTexture (new Rect (positions[i].x - 90 * scale*smallObj, positions[i].y+yHide+yAnimation[i] - 420 * scale*smallObj, 180 * scale*smallObj, 150 * scale*smallObj), objects[1]);
					GUI.DrawTexture (new Rect (positions[i].x +20* scale, positions[i].y+yHide+yAnimation[i] - 120 * scale, 46 * scale, 46 * scale), rightArrowST);
					GUI.DrawTexture (new Rect (positions[i].x - 90 * scale*smallObj, positions[i].y+yHide+yAnimation[i] - 260 * scale*smallObj, 180 * scale*smallObj, 150 * scale*smallObj), instructions[i].img);
					GUI.Label (new Rect (positions[i].x - 150 * scale, positions[i].y+yHide+yAnimation[i] - 185 * scale, 300 * scale, 50 * scale),mainLogic.language.levelStrings[45], headerStyle);
					GUI.Label (new Rect (positions[i].x - 165 * scale, positions[i].y+yHide+yAnimation[i] +50 * scale, 330 * scale, 200 * scale), instructions[i].instruction, descriptionStyle);
					break;
				}
			}
		}
	}

	public class Card
	{
		public int index;
		public string instruction;
		public Texture img;
		public string keyObj;
		public Card(int idx, string instruc, Texture image, string keyobj)
		{
			index=idx;
			instruction=instruc;
			img=image;
			keyObj=keyobj;
		}
	}
}
