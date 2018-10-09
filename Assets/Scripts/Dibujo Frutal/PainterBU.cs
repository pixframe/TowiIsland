using UnityEngine;
using System.Collections;
using CoreDraw;
using Boomlagoon.JSON;

public class PainterBU : MonoBehaviour
{

    public Texture2D baseTex;
    public Texture2D baseTexVisible;
    public Texture2D objectiveTex;
    public Texture2D background;
    public Texture2D currentDrawing;
    public Texture2D sea;
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

	int passedLevels=0;
	int repeatedLevels=0;
	int playedLevels=0;
	bool isRepeated;
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
	public string stateBeforePause="";

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
        GUI.DrawTexture(new Rect(0, 0, background.width, background.height), background);
        GUI.DrawTexture(new Rect(xAdjust, 0, currentDrawing.width * zoom, currentDrawing.height * zoom), currentDrawing);
        GUI.DrawTexture(new Rect(xAdjust, 0, baseTexVisible.width * zoom, baseTexVisible.height * zoom), baseTexVisible);
        //GUI.Label (new Rect (0, 0, 100, 100), accuracy.ToString (), style);
        GUI.Label(new Rect(Screen.width * 0.5f - 100, 20, 200, 60), currentInstruction, styleSmall);

		if (state != "Pause") {
			if (GUI.Button (new Rect (Screen.width / 2 - 75, Screen.height - 70, 150, 60), "¡Listo!", buttonStyle))
				CompareDrawing ();
		}
        if ((animateOcean || nextBase)&&state!="Pause")
        {
            switch (accuracyL)
            {
                case AccuracyLevel.Again:
                    GUI.Label(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f - 100, 200, 200), "Vuelve a dibujar", style);
                    break;
                case AccuracyLevel.Ok:
                    GUI.Label(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f - 100, 200, 200), "Muy Bien", style);
                    break;
                case AccuracyLevel.Perfect:
                    GUI.Label(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f - 100, 200, 200), "Excelente Dibujo", style);
                    break;
            }
        }
        GUI.DrawTexture(new Rect(0, -1200 + Mathf.Sin(sin) * 1200, sea.width * (2 - 1 * Mathf.Sin(sin)), sea.height), sea);
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
				GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 150 * mapScale, 800 * mapScale, 100 * mapScale), "¡Felicidades!",scoreStyle1);
			else
				GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 150 * mapScale, 800 * mapScale, 100 * mapScale), "¡Oh!",scoreStyle1);
			if(kiwis==0)
				GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 90 * mapScale, 800 * mapScale, 100 * mapScale), "No ganaste ningún Kiwi",scoreStyle2);
			else{
				if(kiwis>1)
					GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 90 * mapScale, 800 * mapScale, 100 * mapScale), "Ganaste "+kiwis.ToString()+" Kiwis",scoreStyle2);
				else
					GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f - 90 * mapScale, 800 * mapScale, 100 * mapScale), "Ganaste "+kiwis.ToString()+" Kiwi",scoreStyle2);
			}
			GUI.Label (new Rect (Screen.width * 0.5f - 400 * mapScale, Screen.height * 0.5f + 130 * mapScale, 800 * mapScale, 100 * mapScale), "Volvamos a la Isla",scoreStyle3);

			//styleCompleted.normal.textColor=new Color(0.32f,0.32f,0.32f);
			//GUI.Label(new Rect(Screen.width*0.5f-398*mapScale,Screen.height*0.5f-98*mapScale,800*mapScale,200*mapScale),"Felicidades! dibujas muy bien, volvamos a la isla",styleCompleted);
			//styleCompleted.normal.textColor=new Color(1,1,1);
			//GUI.Label(new Rect(Screen.width*0.5f-400*mapScale,Screen.height*0.5f-100*mapScale,800*mapScale,200*mapScale),"Felicidades! dibujas muy bien, volvamos a la isla",styleCompleted);
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
			GUI.Label(new Rect(Screen.width*0.5f-100*mapScale,pauseY-40*mapScale,200*mapScale,60*mapScale),"PAUSA",pauseText);
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
			}else
				if(GUI.Button(new Rect(Screen.width*0.5f-200*mapScale,pauseY+230*mapScale,162*mapScale,67*mapScale),"",pauseExit))
			{
				Time.timeScale=1.0f;
				Application.Quit();
			}
		}
		//GUI.color = new Color(1,1,1,0.50f);
        //GUI.DrawTexture (Rect (0,0,objectiveTex.width*zoom,objectiveTex.height*zoom),objectiveTex);
        //GUI.color = new Color(1,1,1,1);
		effectCamera.Render ();
    }

    Vector2 preDrag;

    void Start()
    {
		kiwiAnimCurrTime = kiwiAnimTime;
		soundMng = GetComponent<SoundManager> ();
		//PlayerPrefs.SetInt("ArenaMagica_level",0);
		//PlayerPrefs.SetInt("ArenaMagica_subLevel",0);
		mapScale = (float)Screen.height / (float)768;
		style.fontSize = (int)(style.fontSize * mapScale);
		styleSmall.fontSize = (int)(styleSmall.fontSize * mapScale);
		styleCompleted.fontSize = (int)(styleCompleted.fontSize * mapScale);
		pauseText.fontSize = (int)(pauseText.fontSize * mapScale);
		scoreStyle1.fontSize = (int)(scoreStyle1.fontSize * mapScale);
		scoreStyle2.fontSize = (int)(scoreStyle2.fontSize * mapScale);
		scoreStyle3.fontSize = (int)(scoreStyle3.fontSize * mapScale);
		saveHandler=(ProgressHandler)GetComponent(typeof(ProgressHandler));
        clearTexture();
        loader = GetComponent<GameSaveLoad>();
        loader.Load(GameSaveLoad.game.jungleDrawing);
        configuration = (JungleDrawingConfiguration)loader.configuration;
		level=PlayerPrefs.GetInt("ArenaMagica_level",0);
		subLevel=PlayerPrefs.GetInt("ArenaMagica_subLevel",0);
		todayLevels = 0;
		todayOffset=PlayerPrefs.GetInt("PlayedArenaMagica",0);
		//level = 0;
		//subLevel = 0;
		//PlayerPrefs.DeleteAll ();
        state = "Start";
		//SaveProgress();
    }

    void Update()
    {
		if (returnToIsland && !saveHandler.saving) {
			Application.LoadLevel("Archipielago");
		}
		totalTime += Time.deltaTime;
		levelTime += Time.deltaTime;
        switch (state)
        {
            case "Start":
                SetLevel();
                state = "Play";
				soundMng.PlaySoundQueue(new int[]{1,0});
                break;
            case "Play":
                if (scoreScreen > 0)
                    scoreScreen -= Time.deltaTime;
                else if (nextBase)
                {
                    SetNextBase();
                }
                zoom = Screen.height / 768.0f;
                xAdjust = Screen.width * 0.5f - 683 * zoom;
                Rect imgRect = new Rect(0, 0, baseTex.width * zoom, baseTex.height * zoom);
                Vector2 mouse = Input.mousePosition;
                mouse.y = Screen.height - mouse.y;
                mouse.x -= xAdjust;
                //Debug.Log(mouse);
                if (animateOcean && scoreScreen <= 0)
                {
                    sin += 0.5f * Time.deltaTime;
                    if (sin > Mathf.PI / 2 && !clean)
                    {
                        clearTexture();
                        accuracyL = AccuracyLevel.None;
                        accuracy = 0;
						if(playedLevels>=5){
							state="CompletedActivity";
						}else{
                        	SetLevel();
						}
                    }
                    if (sin >= Mathf.PI)
                    {
                        accuracyL = AccuracyLevel.None;
                        animateOcean = false;
                        sin = 0;
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
                    dragStart = Vector2.zero;
                    dragEnd = Vector2.zero;
                }
                preDrag = dragEnd;
                break;
			case "CompletedActivity":
				if(kiwis==-1)
				{
					kiwis=0;
					totalAccuracy/=playedLevels;
				Debug.Log("Total Accuracy"+totalAccuracy.ToString());
					if(totalAccuracy>20)
					{
						kiwis=1;
					}
					if(totalAccuracy>60)
					{
						kiwis=2;
					}
					if(totalAccuracy>80)
					{
						kiwis=3;
					}
					int tempKiwis=PlayerPrefs.GetInt("Kiwis",0);
					PlayerPrefs.SetInt("Kiwis",tempKiwis+kiwis);
					if(kiwis==0)
					{
						soundMng.PlaySoundQueue(new int[]{6,7});
					}else{
						soundMng.PlaySoundQueue(new int[]{5,7});
					}
				}
				kiwiAnimCurrTime-=Time.deltaTime;
				if(kiwiAnimCurrTime<=0)
				{
					kiwiAnimCurrTime=kiwiAnimTime;
					if(animationKiwis<kiwis)
						animationKiwis++;
				}
				endTime-=Time.deltaTime;
				if(endTime<=0){
					Application.LoadLevel("Archipielago");
				}
				break;
        }
    }

    void SetLevel()
    {
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
    }

    void CompareDrawing()
    {
        if (!clean && !animateOcean)
        {
            if (baseIdx < ImageBank[bankIdx].baseImages.Length - 1)
                nextBase = true;
            else
                animateOcean = true;
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
            if (accuracy >= 40)
            {
				if(accuracy>=70)
				{
					soundMng.PlaySound(4);
				}else{
					soundMng.PlaySound(3);
				}
				scoreEffects.DisplayScore(10);
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
					PlayerPrefs.SetInt("ArenaMagica_level",level);
					PlayerPrefs.SetInt("ArenaMagica_subLevel",subLevel);
                }
            }
            else
            {
				soundMng.PlaySound(2);
				scoreEffects.DisplayError();
                baseIdx = 0;
                nextBase = false;
                animateOcean = true;
            }
            if (accuracy >= 70)
                accuracyL = AccuracyLevel.Perfect;
			SaveLevelProgress();
			if(accuracyL==AccuracyLevel.Again){
				isRepeated=true;
			}else{
				isRepeated=false;
			}
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
		PlayerPrefs.SetInt ("PlayedArenaMagica",todayLevels+todayOffset);
		if (isRepeated)
			repeatedLevels++;
		if (passed)
				passedLevels++;
		levelTime = 0;
		//saveHandler.SetLevel();
		if (playedLevels+todayOffset >= numLevelsPerBlock) {
			PlayerPrefs.SetInt("BlockedDibujoFrutal",1);
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
    }

}