using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PackLogic : MonoBehaviour
{
    //script variables
    GameSaveLoad loader;
    IntroGamesConfiguration configuration;
    public PEMainLogic logicScript;
    ObjectContainer objContScript;
    AvatarScript avS;
    public Animator camAnim;


    SelectionScript selectS;
    public CamComtrol camControlScript;
    Timer timerScript;
    public UI UIScript;

    //logic variables
    public string gameState;
    public string controlsState;
    public string resultState;
    public bool controlsOn = false;




    public List<GameObject> objectsSel = new List<GameObject>();

    public List<string> aElementList = new List<string>();
    public List<string> bElementList = new List<string>();
    public List<string> reverseAElementList = new List<string>();
    public List<string> reverseBElementList = new List<string>();
    public List<string> weatherObjPacked = new List<string>();
	public List<string> aElementListTutorial = new List<string>();
    public GameObject[] glowObjs;
    public int normListCount;
    public int reverseListCount;


    public int sublevel = 0;
    public bool failedA = false;
    public bool failedB = false;
    public bool reverse = false;
    public bool testDone = false;
    public bool objPacked = false;


    public bool insRead = false;




    public bool assignObj = true;
    public bool goToPick = false;
    public GameObject avatar;
    public GameObject curObjSelected;
    public GameObject s;
    public GameObject uiS;
    public GameObject suitcase;
    int objPerRound;
    GameObject objHighlighted;
    public bool weatherObjPick = false;
    public string weather;
    public int unPackObjNum = 3;
    public List<string> unPackObj = new List<string>();
    public bool unPackObjReady = false;
    public float camRotation = -3f;
    bool reverseIns = false;
    bool finishEarly = false;



    //tutorial vars
    public bool objSel = false;
    public bool rotationFinished = false;

    public bool packOrderTutorial = true;
    public bool failedTest = false;
    public int tutorialError = -1;


    // Use this for initialization
    void Start()
    {

        loader = GameObject.Find("Main").GetComponent<GameSaveLoad>();
        avS = GameObject.Find("Main").GetComponent<AvatarScript>();
        loader.Load(GameSaveLoad.game.introGames);
        configuration = (IntroGamesConfiguration)loader.configuration;
        camAnim = Camera.main.transform.GetComponent<Animator>();
        objContScript = GameObject.Find("Pack").transform.Find("PackingObjects").GetComponent<ObjectContainer>();
        logicScript = GameObject.Find("Main").GetComponent<PEMainLogic>();

        selectS = GameObject.Find("Main").GetComponent<SelectionScript>();
        camControlScript = Camera.main.GetComponent<CamComtrol>();
        timerScript = GameObject.Find("Main").GetComponent<Timer>();

        UIScript = Camera.main.GetComponent<UI>();
        assignObj = false;
        avatar.SetActive(false);
        LoadElements("a");


    }

    public void ReadyPick()
    {
        if (curObjSelected != null)
        {
            GlowObj(curObjSelected, false);
            curObjSelected.SetActive(false);
            curObjSelected = null;
        }

        finishEarly = true;
        if (!failedA)
            objPerRound = aElementList.Count;
        else
            objPerRound = bElementList.Count;
    }
    // Update is called once per frame
    void Update()
    {

        if (logicScript.miniGame == "Pack")
        {
            switch (gameState)
            {
                case "Intro":
                    //Plays intro of game
                    if (!avatar.activeSelf)
                    {
                        avatar.SetActive(true);
                    }
                    break;
                case "Controls":

                    switch (controlsState)
                    {
						//tutorial de como seleccionar los objetos
                        case "ClickTut":
                            if (!insRead)
                            {
                                if (!avatar.activeSelf)
                                {
                                    avatar.SetActive(true);
                                }
                            }
                            else
                            {
                                if (DownUpPress())
                                {
                                    if (selectS.objSelected.name != "SuitCase"/* && selectS.objSelected.tag != "WeatherObject"*/)
                                    {
                                        //highlight object
                                        //								highlightScript.HighLightMonkey(selectS.objSelected, true);
                                        objSel = true;
                                        curObjSelected = selectS.objSelected.gameObject;
                                        GlowObj(curObjSelected, true);
                                    }
                                }
                                if (objSel)
                                {
                                    if (timerScript.TimerFunc(.5f))
                                    {
                                        s = null;
                                        selectS.objSelected = null;
                                        insRead = false;
                                        controlsState = "PackTut";

                                    }
                                }
                            }

                            break;
						//tutorial de como empacarlos y en que orden
                        case "PackTut":
                            if (!insRead)
                            {
                                if (!avatar.activeSelf)
                                {
                                    avatar.SetActive(true);
                                }
                            }
                            else
                            {

                                if (DownUpPress())
                                {
                                    if (selectS.objSelected != null && selectS.objSelected.name == "SuitCase")
                                    {
                                        avS.packObjs = true;
                                        objSel = false;
                                        //								highlightScript.HighLightMonkey(curObjSelected, false);
                                        GlowObj(curObjSelected, false);
                                        curObjSelected.SetActive(false);
                                        curObjSelected = null;
                                        gameState = "Instructions";
                                        s = null;
                                        selectS.objSelected = null;
                                    }
                                }

                            }
                            break;
                            /*
                        case "OrderTut":
                            if (!insRead)
                            {
                                if (!avatar.activeSelf)
                                {
                                    avatar.SetActive(true);
                                }
                            }
                            else
                            {

                                if (DownUpPress())
                                {
                                    if (selectS.objSelected != null && selectS.objSelected.name == "SuitCase")
                                    {
                                        avS.packObjs = true;
                                        objSel = false;
                                        //								highlightScript.HighLightMonkey(curObjSelected, false);
                                        GlowObj(curObjSelected, false);
                                        curObjSelected.SetActive(false);
                                        curObjSelected = null;
                                        gameState = "Instructions";
                                        s = null;
                                        selectS.objSelected = null;
                                    }
                                }

                            }
                            break;*/
                    }
                    break;
                case "Instructions":
                    if (reverseIns)
                    {
                        if (!avatar.activeSelf)
                        {
                            avatar.SetActive(true);
                        }
                    }
                    else
                    {
                        if (!avatar.activeSelf)
                        {
                            avatar.SetActive(true);
                        }
                    }
                    if (assignObj)
                    {
                        if (!failedA)
                        {
                            avS.packObjs = true;
                            avS.objC = 0;
                            LoadElements("a");
                            assignObj = false;
                        }
                        else
                        {
                            if (!failedB)
                            {
                                avS.packObjs = true;
                                avS.objC = 0;
                                LoadElements("b");
                                assignObj = false;
                            }
                        }
                    }
                    else
                    {
                        if (!goToPick && timerScript.TimerFunc(1f))
                        {
                            ActivateObjects(objContScript.container1, true);
                            goToPick = true;
                        }
                    }

                    break;
                case "CamRot":
                    if (reverse)
                    {
                        sublevel = 0;
                        if (camAnim.GetBool("BackRot"))
                        {
                            gameState = "WeatherConditions";
                        }
                        else
                        {
                            avatar.SetActive(false);
                            camAnim.Play("CamRot");
                            if (!camAnim.GetBool("Rot"))
                            {
                                gameState = "Instructions";
                                reverseIns = true;
                            }
                        }

                    }
                    break;
                case "PlayerPick":
                    if (DownUpPress())//the player selects an object
                    {
                        if (s.name == "SuitCase")//to pack the object
                        {
                            objHighlighted = null;
                            if (curObjSelected != null)
                            {
                                //checks what type of object is stored
                                if (weatherObjPick)
                                {
                                    GlowObj(curObjSelected, false);
                                    weatherObjPacked.Add(curObjSelected.name);
                                    curObjSelected.SetActive(false);
                                }
                                else
                                {
                                    objPerRound++;
                                    GlowObj(curObjSelected, false);
                                    curObjSelected.SetActive(false);
                                    objectsSel.Add(curObjSelected);
                                    objPacked = true;
                                    //								if(!failedA)
                                    //								{
                                    //									objPerRound++;
                                    //									GlowObj(curObjSelected, false);
                                    //									curObjSelected.SetActive(false);
                                    //									objectsSel.Add(curObjSelected);
                                    //									objPacked = true;
                                    //								}
                                    //								else
                                    //								{
                                    //									if(!failedB)
                                    //									{
                                    //										objPerRound++;
                                    //										GlowObj(curObjSelected, false);
                                    //										curObjSelected.SetActive(false);
                                    //										objectsSel.Add(curObjSelected);
                                    //										objPacked = true;
                                    //									}
                                    //								}
                                }
                            }

                        }
                        else// 
                        {
                            //checks what type of object is selected
                            if (weatherObjPick)
                            {
                                if (selectS.objSelected.tag == "WeatherObject")
                                {
                                    curObjSelected = selectS.objSelected;
                                    if (curObjSelected != objHighlighted)
                                    {
                                        if (objHighlighted != null)
                                        {
                                            GlowObj(objHighlighted, false);
                                            GlowObj(curObjSelected, true);
                                            //										objHighlighted.SetActive(false);
                                        }

                                        GlowObj(curObjSelected, true);
                                    }

                                    objHighlighted = curObjSelected;
                                }

                            }
                            else
                            {
                                //							if(selectS.objSelected.tag != "WeatherObject")
                                //							{
                                curObjSelected = selectS.objSelected;
                                if (objHighlighted == null || curObjSelected.name != objHighlighted.name)
                                {
                                    if (objHighlighted != null)
                                    {
                                        GlowObj(objHighlighted, false);
                                        GlowObj(curObjSelected, true);
                                        //										objHighlighted.SetActive(false);
                                    }
                                    else
                                    {
                                        GlowObj(curObjSelected, true);
                                    }
                                }
                                objHighlighted = curObjSelected;
                                //							}
                            }
                        }
                    }


                    //runs when an object is packed
                    if ((finishEarly || (curObjSelected != null && objPacked)) && !weatherObjPick)
                    {
                        finishEarly = false;
                        //what type of list he has to select
                        if (!failedA)
                        {
                            if (objPerRound >= aElementList.Count)//checks to see if the right amount of objects are stored to check if they are correct
                            {
                                if (packOrderTutorial)
                                {
                                    failedTest = false;
                                    objHighlighted = null;
                                    if (!reverse)//first part of the pack test
                                    {
                                        for (int i = 0; i < objPerRound; i++)
                                        {
                                            if (aElementList[i] != objectsSel[i].name)//incorrect
                                            {
                                                //failedA = true;
                                                tutorialError = 1;
                                                failedTest = true;
                                                testDone = false;
                                                break;
                                            }
                                        }
                                        bool objFound = false;
                                        for (int i = 0; i < aElementList.Count; i++)
                                        {
                                            if (aElementList[i] == objectsSel[objPerRound - 1].name)//incorrect
                                            {
                                                objFound = true;
                                                break;
                                            }
                                        }
                                        if (!objFound)
                                            tutorialError = 0;
                                    }
                                    else//second part of the test
                                    {
                                        for (int i = 0; i < objPerRound; i++)
                                        {
                                            if (aElementList[aElementList.Count - 1 - i] != objectsSel[i].name)
                                            {
                                                tutorialError = 1;
                                                failedTest = true;
                                                testDone = false;
                                            }
                                        }
                                        bool objFound = false;
                                        for (int i = 0; i < aElementList.Count; i++)
                                        {
                                            if (aElementList[i] == objectsSel[objPerRound - 1].name)//incorrect
                                            {
                                                objFound = true;
                                                break;
                                            }
                                        }
                                        if (!objFound)
                                            tutorialError = 0;
                                    }
                                    if (!failedTest)
									{
                                        tutorialError = 2;
										packOrderTutorial = false;
									}

                                    camControlScript.rotationX = 0f;
                                    camControlScript.rotationY = 0f;
                                    objectsSel.Clear();
                                    assignObj = true;
                                    objPerRound = 0;
                                    if (!reverse)
                                    {
                                        ActivateObjects(objContScript.container1, true);
                                    }
                                    else
                                    {
                                        ActivateObjects(objContScript.container2, true);
                                    }
                                    gameState = "Instructions";                                    
                                }
                                else
                                {
                                    objHighlighted = null;
                                    if (!reverse)//first part of the pack test
                                    {
                                        if (objectsSel.Count < aElementList.Count)
                                        {
                                            failedA = true;
                                            testDone = false;
                                        }
                                        for (int i = 0; i < objectsSel.Count; i++)
                                        {
                                            if (aElementList[i] != objectsSel[i].name)//incorrect
                                            {
                                                //										Debug.Log("failA " + aElementList[i] + " " + objectsSel[i].name);
                                                failedA = true;
                                                //										i = objectsSel.Count + 1;
                                                testDone = false;
                                                break;
                                            }
                                        }

                                        if (!failedA)
                                        {
                                            if (aElementList.Count >= 9)
                                            {
                                                reverse = true;
                                                objPacked = false;
                                                failedA = false;
                                                failedB = false;
                                                assignObj = true;
                                                camAnim.enabled = true;
                                                camAnim.SetBool("Rot", true);
                                                gameState = "CamRot";
                                                objPerRound = 0;
                                                testDone = false;
                                                break;
                                            }
                                        }


                                    }
                                    else//second part of the test
                                    {
                                        if (objectsSel.Count < aElementList.Count)
                                        {
                                            failedA = true;
                                            testDone = false;
                                        }
                                        for (int i = 0; i < objectsSel.Count; i++)
                                        {
                                            if (aElementList[aElementList.Count - 1 - i] != objectsSel[i].name)
                                            {
                                                //										Debug.Log("failA " + aElementList[i] + " " + objectsSel[i].name);
                                                failedA = true;
                                                testDone = false;
                                            }
                                        }

                                        if (!failedA)
                                        {
                                            if (aElementList.Count >= 8)
                                            {
                                                weatherObjPick = true;
                                                camAnim.SetBool("BackRot", true);
                                                gameState = "CamRot";
                                                testDone = true;
                                                assignObj = false;
                                                break;
                                            }
                                            failedA = false;
                                        }
                                    }

                                    if (!failedA && sublevel < 6)
                                    {
                                        sublevel++;
                                    }
                                    //Camera.main.transform.rotation = Quaternion.identity;
                                    camControlScript.rotationX = 0f;
                                    camControlScript.rotationY = 0f;
                                    objectsSel.Clear();
                                    assignObj = true;
                                    objPerRound = 0;
                                    if (!reverse)
                                    {
                                        ActivateObjects(objContScript.container1, true);
                                    }
                                    else
                                    {
                                        ActivateObjects(objContScript.container2, true);
                                    }
                                    gameState = "Instructions";
                                }
                                curObjSelected = null;
                                objPacked = false;
                            }
                            else
                            {
                                if (packOrderTutorial)
                                {
                                    failedTest = false;
                                    objHighlighted = null;
                                    if (!reverse)//first part of the pack test
                                    {
                                        for (int i = 0; i < objPerRound; i++)
                                        {
                                            if (aElementList[i] != objectsSel[i].name)//incorrect
                                            {
                                                //failedA = true;
                                                tutorialError = 1;
                                                failedTest = true;
                                                testDone = false;
                                                break;
                                            }
                                        }
                                        bool objFound = false;
                                        for (int i = 0; i < aElementList.Count; i++)
                                        {
                                            if (aElementList[i] == objectsSel[objPerRound-1].name)//incorrect
                                            {
                                                objFound = true;
                                                break;
                                            }
                                        }
                                        if (!objFound)
                                            tutorialError = 0;
                                    }
                                    else//second part of the test
                                    {
                                        for (int i = 0; i < objPerRound; i++)
                                        {
                                            if (aElementList[aElementList.Count - 1 - i] != objectsSel[i].name)
                                            {
                                                tutorialError = 1;
                                                failedTest = true;
                                                testDone = false;
                                            }
                                        }
                                        bool objFound = false;
                                        for (int i = 0; i < aElementList.Count; i++)
                                        {
                                            if (aElementList[i] == objectsSel[objPerRound - 1].name)//incorrect
                                            {
                                                objFound = true;
                                                break;
                                            }
                                        }
                                        if (!objFound)
                                            tutorialError = 0;
                                    }
                                    if (failedTest)
                                    {
                                        camControlScript.rotationX = 0f;
                                        camControlScript.rotationY = 0f;
                                        objectsSel.Clear();
                                        assignObj = true;
                                        objPerRound = 0;
                                        if (!reverse)
                                        {
                                            ActivateObjects(objContScript.container1, true);
                                        }
                                        else
                                        {
                                            ActivateObjects(objContScript.container2, true);
                                        }
                                        gameState = "Instructions";
                                    }
                                }
                                objPacked = false;
                                curObjSelected = null;
                            }
                        }
                        else
                        {
                            if (!failedB)
                            {
                                if (objPerRound >= bElementList.Count)
                                {
                                    if (!reverse)
                                    {
                                        if (objectsSel.Count < bElementList.Count)
                                        {
                                            failedB = true;
                                            failedA = true;
                                        }
                                        for (int i = 0; i < objectsSel.Count; i++)
                                        {
                                            if (bElementList[i] != objectsSel[i].name)
                                            {
                                                failedB = true;
                                                failedA = true;
                                                break;
                                            }
                                        }

                                        if (!failedB)
                                        {
                                            if (bElementList.Count >= 9)
                                            {
                                                //											Debug.Log("bfinish" + Time.frameCount);
                                                reverse = true;
                                                objPacked = false;
                                                failedA = false;
                                                failedB = false;
                                                assignObj = true;
                                                camAnim.enabled = true;
                                                camAnim.SetBool("Rot", true);
                                                gameState = "CamRot";
                                                objPerRound = 0;
                                                testDone = false;
                                                break;
                                            }
                                            failedB = false;
                                            failedA = false;
                                        }
                                    }
                                    else
                                    {
                                        if (objectsSel.Count < bElementList.Count)
                                        {
                                            failedB = true;
                                            failedA = true;
                                        }
                                        for (int i = 0; i < objectsSel.Count; i++)
                                        {
                                            if (bElementList[bElementList.Count - 1 - i] != objectsSel[i].name)
                                            {
                                                failedB = true;
                                                failedA = true;
                                                break;
                                            }
                                            else
                                            {
                                                if (!failedB)
                                                {
                                                    if (bElementList.Count >= 8)
                                                    {

                                                        weatherObjPick = true;
                                                        camAnim.SetBool("BackRot", true);
                                                        gameState = "CamRot";
                                                        testDone = true;
                                                        assignObj = false;
                                                        break;
                                                    }
                                                    failedB = false;
                                                    failedA = false;
                                                }
                                            }
                                        }
                                    }

                                    if (!failedB)
                                    {
                                        if (!testDone)
                                        {
                                            objPacked = false;
                                            //										Debug.Log("Ins" + Time.frameCount);
                                            gameState = "Instructions";
                                        }
                                        if (!failedB && sublevel < 6)
                                        {
                                            sublevel++;
                                        }

                                    }
                                    //Camera.main.transform.rotation = Quaternion.identity;
                                    camControlScript.rotationX = 0f;
                                    camControlScript.rotationY = 0f;
                                    assignObj = true;
                                    objPerRound = 0;
                                    if (!reverse)
                                    {
                                        ActivateObjects(objContScript.container1, true);
                                    }
                                    else
                                    {
                                        ActivateObjects(objContScript.container2, true);
                                    }
                                    curObjSelected = null;
                                }
                                else
                                {
                                    objPacked = false;
                                    curObjSelected = null;
                                }
                            }

                        }
                    }
                    else
                    {
                        if (failedB && !weatherObjPick)
                        {
                            testDone = true;
                            if (testDone)
                            {
                                if (!reverse)
                                {
                                    reverse = true;
                                    objPacked = false;

                                    failedA = false;
                                    failedB = false;
                                    assignObj = true;
                                    camAnim.enabled = true;
                                    camAnim.SetBool("Rot", true);
                                    //								avatar.SetActive(true);
                                    gameState = "CamRot";
                                    testDone = false;
                                    break;
                                }
                                else
                                {
                                    weatherObjPick = true;
                                    camAnim.SetBool("BackRot", true);
                                    gameState = "CamRot";
                                    testDone = true;
                                    assignObj = false;
                                    break;
                                }
                            }
                        }
                    }

                    break;
                case "WeatherConditions":
                    if (weather == "")
                    {
                        int c = Random.Range(0, 3);
                        if (c == 0)
                        {

                            weather = avS.language.levelStrings[42];
                        }
                        else if (c == 1)
                        {
                            weather = avS.language.levelStrings[43];
                        }
                        else
                        {
                            weather = avS.language.levelStrings[44];
                        }
                    }
                    else
                    {
                        if (!goToPick && timerScript.TimerFunc(1f))
                        {
                            goToPick = true;
                        }
                    }


                    break;
                case "FinalInstructions":
                    unPackObjReady = true;
                    break;
                case "Scores":
                    break;


            }
        }

    }
    public void clearObjs()
    {
        foreach (GameObject o in objContScript.container1)
        {
            o.SetActive(false);
        }


        GameObject[] weath = GameObject.FindGameObjectsWithTag("WeatherObject");
        foreach (GameObject o in weath)
        {
            o.SetActive(false);
        }
        suitcase.SetActive(false);

    }
    public bool UIDownUpPress()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selectS.UISelectionFunc(Input.mousePosition))
            {
                uiS = selectS.objSelected;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (selectS.UISelectionFunc(Input.mousePosition))
            {
                if (uiS == selectS.objSelected)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool DownUpPress()
    {

        if (Input.GetMouseButtonDown(0))
        {

            if (selectS.SelectionFunc(Input.mousePosition))
            {
                s = selectS.objSelected;
            }


        }
        if (Input.GetMouseButtonUp(0))
        {

            if (selectS.SelectionFunc(Input.mousePosition))
            {
                if (s == selectS.objSelected)
                {

                    return true;
                }
            }


        }
        return false;
    }
    void LoadElements(string serie)
    {
        objectsSel.Clear();
        reverseAElementList.Clear();
        reverseBElementList.Clear();
        aElementList.Clear();
        bElementList.Clear();
		aElementListTutorial.Clear();
        if (serie == "a")
        {
            if (!reverse)
            {
				if(packOrderTutorial)
				{
					aElementList.Add("Piano");
					aElementList.Add("Tabla de surf");
					aElementList.Add("Lentes");

				}else{
	                foreach (string s in configuration.miniGame.packing.packingSublevel[sublevel].elementsA)
	                {
	                    aElementList.Add(s);
	                }
				}
                normListCount = aElementList.Count;
            }
            else
            {
				if(packOrderTutorial)
				{
					aElementList.Add("Cinturón");
					aElementList.Add("Patineta");

					reverseAElementList.Add("Cinturón");
					reverseAElementList.Add("Patineta");
					
				}else{
	                foreach (string s in configuration.miniGame.packing.packingSublevel[sublevel].reverseEleA)
	                {
	                    aElementList.Add(s);
	                    reverseAElementList.Add(s);
	                }
				}
                reverseListCount = reverseAElementList.Count;
            }

        }
        else if (serie == "b")
        {
            if (!reverse)
            {
                foreach (string s in configuration.miniGame.packing.packingSublevel[sublevel].elementsB)
                {
                    bElementList.Add(s);
                }
            }
            else
            {
                foreach (string s in configuration.miniGame.packing.packingSublevel[sublevel].reverseEleB)
                {
                    bElementList.Add(s);
                    reverseBElementList.Add(s);
                }
            }

        }
    }
    void GlowObj(GameObject obj, bool glow)
    {
        if (glow)
        {
            //turn on glow obj
            GameObject glowO = transform.Find("PackingObjects").transform.Find("GlowObjects").Find(obj.name).gameObject;
            obj.SetActive(false);
            glowO.SetActive(true);
        }
        else
        {
            //turn off glow obj
            GameObject glowO = transform.Find("PackingObjects").transform.Find("GlowObjects").Find(obj.name).gameObject;
            glowO.SetActive(false);
            obj.SetActive(true);
        }
    }
    public void ActivateObjects(List<GameObject> list, bool activate)
    {
        foreach (GameObject obj in list)
        {
            if (activate)
            {
                if (!obj.activeSelf)
                {
                    obj.SetActive(true);
                }
            }
            else
            {
                if (obj.activeSelf)
                {
                    obj.SetActive(false);
                }
            }

        }
    }

}
