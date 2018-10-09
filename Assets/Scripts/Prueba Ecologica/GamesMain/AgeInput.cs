using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AgeInput : MonoBehaviour
{
	
	//script variables
	PEMainLogic logicScript;
//	GUIText insText;
	TextInput inputTextScript;
	GameObject ageSheet;
	public Rect ageOfPlayerInput;
	public GUIStyle ageStyle;
	public GUIStyle advanceStyle;
	public Color cColor;
	//logic variables
	int i;
	public int maxNumChar = 3;
	public string stringToEdit;
	public string birthday;
	public string defaultAge = "--";
	public string defaultBirth;

	float screenScale;
	public bool incorrectInput;

    //Ui Elements to Input
    [Header("UI Elements")]
    public TextInput ageInput;
    public TextInput birthdayInput;


	// Use this for initialization
	void Start ()
	{

//		insText = transform.FindChild("Instructions").transform.GetComponent<GUIText>();
		logicScript = GameObject.Find("Main").GetComponent<PEMainLogic>();
		inputTextScript = GameObject.Find("Main").GetComponent<TextInput>();
		ageSheet = transform.Find("edad").gameObject;
		ageSheet.SetActive(false);
		defaultAge = "--";
		defaultBirth = "Cumpleaños";
		stringToEdit = "";
		birthday = "";
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(logicScript.miniGame == "InputAge")
		{
			if(!ageSheet.activeSelf)
			{
				ageSheet.SetActive(true);
				
			}


			if(logicScript.ageOfPlayer == 0)
			{
				logicScript.ageOfPlayer = int.Parse(inputTextScript.InputText(stringToEdit, true, false, maxNumChar));
			}
			else
			{
//				ageSheet.SetActive(false);
				logicScript.curGameFinished = true;
			}
			
		}

		

	}
	void OnGUI()
	{
		ageStyle.clipping = TextClipping.Clip;
		screenScale = (float)Screen.height / (float)768;
		ageStyle.fontSize = (int) (25 * screenScale);
		advanceStyle.fontSize = (int) (25 * screenScale);
		GUI.skin.settings.cursorColor = Color.black;
		GUI.skin.settings.cursorFlashSpeed = 1f;
		if(logicScript.miniGame == "InputAge")
		{
			if(!incorrectInput)
			{
				if(Input.GetKeyDown(KeyCode.Return))
				{
					Debug.Log("enter");
				}
				else
				{
					GUI.SetNextControlName("Age");
					stringToEdit = GUI.TextField(new Rect(Screen.width * .5f - 255 * screenScale, Screen.height * .5f + 195 * screenScale, 148 * screenScale, 100 * screenScale), stringToEdit, 100, ageStyle);


					if(UnityEngine.Event.current.type == EventType.Repaint)
					{
						if (GUI.GetNameOfFocusedControl () == "Age")	
						{
							
							if (stringToEdit == defaultAge)
							{
								stringToEdit = "";
							}
							
						}
						else
							
						{
							
							if (stringToEdit == "") 
							{
								stringToEdit = defaultAge;
							}
							
						}
					}
					GUI.SetNextControlName("Birth");
					birthday = GUI.TextField(new Rect(Screen.width * .5f - 20 * screenScale, Screen.height * .5f + 195 * screenScale, 250 * screenScale, 100 * screenScale), birthday, 100, ageStyle);
					if(UnityEngine.Event.current.type == EventType.Repaint)
					{
						if (GUI.GetNameOfFocusedControl () == "Birth")	
						{
							
							if (birthday == defaultBirth)
							{
								birthday = "";
							}
							
						}
						else
							
						{
							
							if (birthday == "") 
							{
								birthday = defaultBirth;
							}
							
						}
					}
				}
				if(GUI.Button(new Rect(Screen.width * .5f + 226 * screenScale, Screen.height * .5f + 260 * screenScale, 160 * screenScale, 80 * screenScale), "LISTO", advanceStyle))
				{
					int i;
					if(int.TryParse(stringToEdit, out i))
					{
						int plAge = int.Parse(stringToEdit);

						if (stringToEdit == defaultAge || stringToEdit == "")
							plAge = 9999;
						
						if (birthday == defaultBirth || birthday == "")
							birthday = "Sin respuesta";
						
						ageSheet.SetActive(false);
						logicScript.ageOfPlayer = plAge;
						logicScript.birthdayOfPlayer = birthday;
						logicScript.curGameFinished = true;
						incorrectInput = false;
					}
					else
					{
//						
						incorrectInput = true;
					}
					
				}
			}


		}

	}
}
