using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnPackLogic : MonoBehaviour
{

	PackLogic packScript;
	PEMainLogic logicScript;
	Timer timerScript;
	SelectionScript selS;
	Glows glowsScript;
	ObjectContainer objContScript;

	public GameObject[] pos;
	public List<GameObject> objPlaced = new List<GameObject>();
	public List<GameObject> objSequence = new List<GameObject>();
	public List<string> unPackedFObjs = new List<string>();
	public List<GameObject> groupOne = new List<GameObject>();
	public List<GameObject> groupTwo = new List<GameObject>();
	public List<string> cat = new List<string>();
	public string state;
	GameObject emptyRoom;
	GameObject sampleRoom;
	public float timeOfPic = 5f;
	public bool drag = false;
	public GameObject objectSelected;
	public int correctFirstObj;
	public bool determine = false;
	public bool check = false;
	public int[] correct;
	public int[] incorrect;
	public int[] repeated;
	public int[] fourFirst;
	public int[] fourLast;
	public int[] grouping;
	public int[] spacialPres;
	public GameObject[] slots;
	int numElePlaced = 0;
	public int essay = 0;
	string r;
	bool remove = false;
	int count;
	int playerAge;
	public int divAge = 9;
	public string prevCatObj;
	public string curCatObj;

	float showTimer;

	//new vars
	public GameObject unPackUI;
	Vector3 screenPoint;
	GameObject backg;
	GameObject objectsG;
	bool shown = false;
	public List<GameObject> unPObjs = new List<GameObject>();
	public List<GameObject> deactivatedList = new List<GameObject>();
	RaycastHit hit;
	public LayerMask posLayer;
	bool corObj;
	bool force = false;
	float screenScale;
	public bool advance = false;

    public bool overcollider = false;

    public Texture mouseFeedbackTexture;
    public float feedbackTime;
    public float feedbackScaleRate;
    public float opacityRate;
    List<WaitingLogic.MouseFeedback> feedbackList;
    float scale = 1;

	// Use this for initialization
	void Start ()
	{
        scale = Screen.height / 768f;
        feedbackList = new List<WaitingLogic.MouseFeedback>();
		packScript = GameObject.Find("Pack").GetComponent<PackLogic>();
		logicScript = GameObject.Find("Main").GetComponent<PEMainLogic>();
//		emptyRoom = transform.FindChild("EmptyRoom").gameObject;
//		sampleRoom = transform.FindChild("RoomPhoto").gameObject;
		timerScript = GameObject.Find("Main").GetComponent<Timer>();
		selS = GameObject.Find("Main").GetComponent<SelectionScript>();
		glowsScript = GetComponent<Glows>();
		objContScript = GameObject.Find("Pack").transform.Find("PackingObjects").GetComponent<ObjectContainer>();

//		emptyRoom.SetActive(false);
//		sampleRoom.SetActive(false);
		state = "FirstObjectsIntro";
		numElePlaced = 0;
		essay = 0;
		showTimer = timeOfPic;
		unPackUI = transform.Find("UnPackCam").gameObject;
		objectsG = unPackUI.transform.Find("Objects").gameObject;
		backg = unPackUI.transform.Find("backg").gameObject;
		objectsG.SetActive(false);
		backg.SetActive(false);

		foreach(GameObject o in slots)
		{
			o.SetActive(false);
		}
	
	}

	// Update is called once per frame
	void Update ()
	{
		screenScale = (float)Screen.height / (float)768;
		if(state == "UnPackFirstObjects" || state == "UnPack")
		{
			if(state == "UnPackFirstObjects")
			{
				if(!slots[0].activeSelf)
				{
					foreach(GameObject o in slots)
					{
						if(!o.activeSelf)
						{
							o.SetActive(true);
						}
					}
				}
			}
			else
			{
				if(slots[0].activeSelf)
				{
					foreach(GameObject o in slots)
					{
						o.SetActive(false);
					}
				}
			}
			if(!drag)
			{
				/*if(objectSelected != null)
				{
//					objectSelected.transform.parent =  unPackUI.transform.FindChild("Objects").transform;
//					objectSelected.layer = 18;
				}*/
				if(!objectsG.activeSelf && !check)
				{
					objectsG.SetActive(true);
					backg.SetActive(true);
				}

			}
			else
			{
				objectSelected.transform.parent = Camera.main.transform;
				objectSelected.transform.parent = null;
//				objectSelected.layer = 0;
				if(objectsG.activeSelf)
				{
					objectsG.SetActive(false);
					backg.SetActive(false);
				}
			}
		}
		else if(state == "ShowImage" || state == "Intro") 
		{
			objectsG.SetActive(false);
			backg.SetActive(false);
		}

		if(logicScript.miniGame == "UnPack")
		{
//			emptyRoom.SetActive(true);
//			sampleRoom.SetActive(true);
			switch(state){

			case "FirstObjectsIntro":
				packScript.ActivateObjects (objContScript.container1, false);
				packScript.ActivateObjects (objContScript.container2, false);

				playerAge = logicScript.ageOfPlayer;
				if (playerAge <= divAge) {
					objSequence = groupOne;
				} else {
					objSequence = groupTwo;
				}
				packScript.ActivateObjects (objSequence, true);

				//Activa los elementos que si se tienen que desempacar
				//buscar como optimizar que no sea un foreach anidado
				foreach (GameObject obj in objSequence)
				{
					foreach (Transform objPantalla in objectsG.transform)
						if(objPantalla.name == obj.name)
							objPantalla.GetComponent<UnPackObj> ().unPackObj = true;
					
					//objectsG.transform.Find(obj.name).GetComponent<UnPackObj> ().unPackObj = true;
				}

				glowsScript.glows = new GameObject[objSequence.Count];
				for(int i = 0; i < objSequence.Count; i++)
				{
					foreach(GameObject g in glowsScript.glowCont)
					{
						if(g.name == objSequence[i].name)
						{
							//Debug.Log("glows assign");
							glowsScript.glows[i] = g;
						}
					}
				}
				break;

			case "UnPackFirstObjects":

				if(Input.GetMouseButtonDown(0))
				{
					check = false;
					if(selS.UISelectionFunc(Input.mousePosition))
					{
						if(selS.objSelected.tag == "UnPackObject")
						{
							drag = true;
							objectSelected = selS.objSelected;
							screenPoint = unPackUI.GetComponent<Camera>().WorldToScreenPoint(objectSelected.transform.position);
						}
					}
				}
				
				if(Input.GetMouseButtonUp(0) && drag)
				{
					drag = false;
                    if (overcollider)
                    {
                        check = true;
                    }else
                    {
                        objectSelected.transform.parent = unPackUI.transform.Find("Objects").transform;
                        objectSelected.GetComponent<UnPackObj>().IniPos();
                    }
				}
				
				if(drag)
				{

					Vector3 pos = unPackUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
					objectSelected.transform.position = new Vector3(objectSelected.transform.position.x, pos.y, pos.z);
				}
				break;
			case "Intro":
				if(slots[0].activeSelf)
				{
					foreach(GameObject o in slots)
					{
						o.SetActive(false);
					}
				}
				break;
			case "ShowImage":

				if(!shown)
				{
					packScript.ActivateObjects(objSequence, true);
					shown = true;
				}


				if(glowsScript.Glow(2f, objSequence))
				{
					packScript.ActivateObjects(objSequence, false);
					packScript.clearObjs();
					state = "UnPack";
					objectsG.SetActive(true);
					backg.SetActive(true);

					break;
				}
				
				break;
			case "UnPack":
				shown = false;

				//Pasa al siguiente intento o termina la actividad
				if(advance)
				{
					Debug.Log ("Correctas: " + correct[essay]); 
					Debug.Log ("Incorrectas: " + incorrect[essay]); 
					Debug.Log ("Repetidas: " + repeated[essay]); 
					Debug.Log ("Four First: " + fourFirst[essay]); 
					Debug.Log ("Four Last: " + fourLast[essay]); 
					Debug.Log ("Precision: " + spacialPres[essay]);
					Debug.Log ("Grouping: " + grouping[essay]);

					advance = false;

					//Reinicia el estatus de los elementos que se seleccionaron
					foreach(GameObject o in deactivatedList)
					{
						o.layer = 18;
						UnPackObj unP = o.GetComponent<UnPackObj>();
						unP.objUsed = false;
						o.GetComponent<Rigidbody>().isKinematic = true;
						o.GetComponent<Rigidbody>().useGravity = false;
						unP.IniPos();
						o.SetActive(true);
                        o.transform.parent = unPackUI.transform.Find("Objects").transform;
					}

					//Reinicia el estatus de listas para el nuevo intento 
					deactivatedList.Clear();
					objPlaced.Clear ();
					objectsG.SetActive(false);
					backg.SetActive(false);
					numElePlaced = 0;

					//Control de cuantos ensayos lleva
					essay++;

					if(essay >= 3)
					{
						state = "End";
						break;
					}
					else
					{
						determine = true;
						state = "InsAgain";
						break;
					}
				}

				if(Input.GetMouseButtonDown(0))
				{
					check = false;
					if(selS.UISelectionFunc(Input.mousePosition))
					{
						if(selS.objSelected.tag == "UnPackObject" && !selS.objSelected.GetComponent<UnPackObj>().objUsed)
						{
							drag = true;
							objectSelected = selS.objSelected;
							screenPoint = unPackUI.GetComponent<Camera>().WorldToScreenPoint(objectSelected.transform.position);
						}
					}
				}

				if(Input.GetMouseButtonUp(0) && drag)
				{
					drag = false;
					check = true;
					if(!objectSelected.GetComponent<UnPackObj>().unPackObj)
					{
						corObj = false;
					}
					else
					{
						//Debug.Log("ray");
						Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
						if(Physics.Raycast(ray, out hit, Mathf.Infinity, posLayer))
						{
							//Debug.Log("rayhit");
							if(hit.transform.GetComponent<PosCollider>().correctObject.name == objectSelected.name)
							{
								//Debug.Log("samename");
								foreach(GameObject o in objSequence)
								{
									if(o.name == objectSelected.name)
									{
                                        feedbackList.Add(new WaitingLogic.MouseFeedback(Input.mousePosition, 1, feedbackScaleRate, feedbackTime, opacityRate));
										//Debug.Log("samenamein seq");
										corObj = true;
										o.SetActive(true);
										spacialPres[essay]++; //en precision se toman en cuenta las repetidas
									}
								}
							}
							else
							{
								Debug.Log("no same name");
								foreach(GameObject o in objSequence)
								{
									if(o.name == objectSelected.name)
									{
										//Debug.Log("put in place");
										corObj = true;
										o.SetActive(true);
									}
								}
							}
						}
						else
						{
							foreach(GameObject o in objSequence)
							{
								if(o.name == objectSelected.name)
								{
									corObj = true;
									o.SetActive(true);
								}
							}
						}
					}

				}

				if(drag)
				{
					Vector3 pos = unPackUI.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
					objectSelected.transform.position = new Vector3(objectSelected.transform.position.x, pos.y, pos.z);
				}
				
				break;
			case "InsAgain":
				/*foreach(GameObject o in deactivatedList)
				{
					o.transform.parent = unPackUI.transform.FindChild("Objects").transform;
				}*/
				if(!determine)
				{						
					if(essay >= 4)
					{
						state = "End";
					}
				}
				else
				{

					count = 0;
					foreach(GameObject obj in pos)
					{
						if(obj.GetComponent<PosCollider>().add)
						{

							count++;
						}
					}

					if(count == 10)
					{
						determine = false;

					}
				}
				break;
			case "End":
				Debug.Log("end");

				logicScript.curGameFinished = true;
				break;
			}
		}
        for (int i = 0; i < feedbackList.Count; i++)
        {
            feedbackList[i].Update();
        }
        feedbackList.RemoveAll(a => a.destroy);
	}
    void OnGUI()
    {
        for (int i = 0; i < feedbackList.Count; i++)
        {
            GUI.color = new Color(1, 1, 1, feedbackList[i].opacity);
            GUI.DrawTexture(new Rect(feedbackList[i].feedBackPos.x - feedbackList[i].feedbackSize * 0.5f * scale, Screen.height - feedbackList[i].feedBackPos.y - feedbackList[i].feedbackSize * 0.5f * scale, feedbackList[i].feedbackSize * scale, feedbackList[i].feedbackSize * scale), mouseFeedbackTexture);
            GUI.color = new Color(1, 1, 1, 1);
            feedbackList[i].Update();
        }
    }

	void OnTriggerStay(Collider col)
	{
	  if(col.tag == "UnPackObject")
	   if(state == "UnPack")
		if(check)
		{
			if(col.tag == "UnPackObject")
			{
				if(col.gameObject == objectSelected)
				{
					bool repeatedNow = false;

					foreach (GameObject obj in objPlaced) {	
						if (obj.name == objectSelected.name) {
							repeated [essay]++;
							repeatedNow = true;
							break;
						}
					}

					if (!repeatedNow) {
						if(col.GetComponent<UnPackObj>().unPackObj)
						{
							correct[essay]++;
							selS.objSelected.GetComponent<UnPackObj>().objUsed = true;
							check = false;
						}
						else
						{
							incorrect[essay]++;
							selS.objSelected.GetComponent<UnPackObj>().objUsed = true;
							check = false;
						}
					}

							

					objPlaced.Add(objectSelected);
					cat.Add(objectSelected.GetComponent<UnPackObj>().category);

					if(curCatObj != "")
					{
						Debug.Log("gas");
						prevCatObj = curCatObj;
					}

					curCatObj = objectSelected.GetComponent<UnPackObj>().category;
					
					if(curCatObj == prevCatObj)
					{
						grouping[essay]++;
						curCatObj = "";
						prevCatObj = "";
						remove = true;
					}

					if(numElePlaced < 4)
					{
						for(int i = 0; i < 4; i++)
						{
							if(objectSelected.name == objSequence[i].name)
							{
								fourFirst[essay]++;
							}
						}
						for(int i = objSequence.Count - 1; i > objSequence.Count - 5; i--)
						{
							if(objectSelected.name == objSequence[i].name)
							{
								fourLast[essay]++;
							}
						}
					}
					numElePlaced++;

					if(numElePlaced == 4)
					{
						switch (fourLast[essay]) {
							case 1:
								fourLast[essay] = 25;
							break;
							case 2:
								fourLast [essay] = 50;
							break;
							case 3:
								fourLast[essay] = 75;
							break;
							case 4:
								fourLast[essay] = 100;
							break;
						}

						switch (fourFirst[essay]) {
							case 1:
								fourFirst[essay] = 25;
							break;
							case 2:
								fourFirst[essay] = 50;
							break;
							case 3:
								fourFirst[essay] = 75;
							break;
							case 4:
								fourFirst[essay] = 100;
							break;
						}

						/*if(fourLast[essay] == 1)
						{
							fourLast[essay] = 25;
						}
						else if(fourLast[essay] == 2)
						{
							fourLast[essay] = 50;
						}
						else if(fourLast[essay] == 3)
						{
							fourLast[essay] = 75;
						}
						else if(fourLast[essay] == 4)
						{
							fourLast[essay] = 100;
						}

						if(fourFirst[essay] == 1)
						{
							fourFirst[essay] = 25;
						}
						else if(fourFirst[essay] == 2)
						{
							fourFirst[essay] = 50;
						}
						else if(fourFirst[essay] == 3)
						{
							fourFirst[essay] = 75;
						}
						else if(fourFirst[essay] == 4)
						{
							fourFirst[essay] = 100;
						}*/
					}
				}
			}

			foreach(GameObject i in objSequence)
			{
				if(col.transform.name == i.name)
				{
					objectSelected.transform.parent =  unPackUI.transform.Find("Objects").transform;
				
				}
			}
			
			if(corObj)
			{
				objectSelected.SetActive(false);
				objectSelected.GetComponent<UnPackObj>().IniPos();
				deactivatedList.Add(objectSelected);
			}
			else
			{
				objectSelected.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
				objectSelected.layer = 0;
				objectSelected.GetComponent<Rigidbody>().isKinematic = false;
				objectSelected.GetComponent<Rigidbody>().useGravity = true;
				force = true;
				deactivatedList.Add(objectSelected);
			}

			corObj = false;
			check = false;
		}
	}

	void FixedUpdate()
	{
		if(force)
		{
			objectSelected.GetComponent<Rigidbody>().AddForce(Vector3.right * 10f, ForceMode.Impulse);
			force = false;
			objectSelected.transform.parent =  unPackUI.transform.Find("Objects").transform;
		}
	}
		
}
