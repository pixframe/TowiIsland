using UnityEngine;
using System.Collections;

public class BuyTicketLogic : MonoBehaviour
{
	PEMainLogic logicScript;
	TextInput textInputScript;

	SpriteRenderer nivel1;
	SpriteRenderer nivel2;
	SpriteRenderer nivel3;
	public int nameMaxChars = 20;
	public int addressMaxChars = 20;
	public int birthdayMaxChars = 10;
	int age;

	string steps;
	public string nameS;
	public string dateS;
	public string AddrS;
	string nameDefault;
	string dateDefault;
	string addDefault;

	float screenScale;
	public GUIStyle advanceStyle;
	public GUIStyle ageStyle;
	// Use this for initialization
	void Start ()
	{
		logicScript = GameObject.Find("Main").transform.GetComponent<PEMainLogic>();
//		textInputScript = GameObject.Find("Main").transform.GetComponent<TextInput>();
		nivel1 = transform.Find("nivel1").GetComponent<SpriteRenderer>();
		nivel2 = transform.Find("nivel2").GetComponent<SpriteRenderer>();
		nivel3 = transform.Find("nivel3").GetComponent<SpriteRenderer>();
		nivel1.enabled = false;
		nivel2.enabled = false;
		nivel3.enabled = false;
		steps = "";
		nameS = "";
		dateS = "";
		AddrS = "";
		nameDefault = "Nombre";
		dateDefault = "Fecha";
		addDefault = "Donde vives";
	}
	
	// Update is called once per frame
	void Update ()
	{

		if(logicScript.miniGame == "BuyTicket")
		{

			age = logicScript.ageOfPlayer;
			if(age <= logicScript.maxAge1)
			{
				steps = "Step1";
			}
			else if(age <= logicScript.maxAge2 && age >= logicScript.minAge2)
			{
				steps = "Step2";
			}
			else if(age >= logicScript.minAge3)
			{
				steps = "Step3";
			}
			switch(steps){

			case "Step1":
				if(!nivel1.enabled)
				{
					nivel1.enabled = true;
				}
				break;
			case "Step2":
				if(!nivel2.enabled)
				{
					nivel2.enabled = true;
				}
				break;
			case "Step3":
				if(!nivel3.enabled)
				{
					nivel3.enabled = true;
				}
				break;

			}


		}
	}
	void OnGUI()
	{
		if(logicScript.miniGame == "BuyTicket")
		{
			screenScale = (float)Screen.height / (float)768;
			advanceStyle.fontSize = (int)(35 * screenScale);
			ageStyle.fontSize = (int) (35 * screenScale);
			switch(steps){
				
			case "Step1":
				GUI.SetNextControlName("Nombre");
				nameS = GUI.TextField(new Rect(Screen.width * .5f - 320 * screenScale, Screen.height * .5f + 70 * screenScale, 350 * screenScale, 100 * screenScale), nameS, ageStyle);
				if(UnityEngine.Event.current.type == EventType.Repaint)
				{

					if(GUI.GetNameOfFocusedControl() == "Nombre")
					{
						if(nameS == nameDefault)
						{
							nameS = "";
						}
					}
					else
					{
						if(nameS == "")
						{
							nameS = nameDefault;
						}
					}
				}

				if(GUI.Button(new Rect(Screen.width * .5f + 225 * screenScale, Screen.height * .5f + 240 * screenScale, 160 * screenScale, 80 * screenScale), "LISTO", advanceStyle))
				{
					if (nameS == nameDefault || nameS == "")
						nameS = "Sin respuesta";
					
					logicScript.nameOfPlayer = nameS;
					logicScript.addressOfPlayer = "NA";
					logicScript.currentDate = "NA";
					nivel1.enabled = false;
					logicScript.curGameFinished = true;
				}
				
				break;
			case "Step2":
				GUI.SetNextControlName("Nombre");
				nameS = GUI.TextField(new Rect(Screen.width * .5f - 320 * screenScale, Screen.height * .5f + 30 * screenScale, 350 * screenScale, 100 * screenScale), nameS, ageStyle);
				if(UnityEngine.Event.current.type == EventType.Repaint)
				{
					if(GUI.GetNameOfFocusedControl() == "Nombre")
					{
						if(nameS == nameDefault)
						{
							nameS = "";
						}
					}
					else
					{
						if(nameS == "")
						{
							nameS = nameDefault;
						}
					}

				}
				GUI.SetNextControlName("Add");
				AddrS = GUI.TextField(new Rect(Screen.width * .5f - 320 * screenScale, Screen.height * .5f + 110 * screenScale, 350 * screenScale, 100 * screenScale), AddrS, ageStyle);
				if(UnityEngine.Event.current.type == EventType.Repaint)
				{
					if(GUI.GetNameOfFocusedControl() == "Add")
					{
						if(AddrS == addDefault)
						{
							AddrS = "";
						}
					}
					else
					{
						if(AddrS == "")
						{
							AddrS = addDefault;
						}
					}
				}
				if(GUI.Button(new Rect(Screen.width * .5f + 225 * screenScale, Screen.height * .5f + 240 * screenScale, 160 * screenScale, 80 * screenScale), "LISTO", advanceStyle))
				{
					if (nameS == nameDefault || nameS == "")
						nameS = "Sin respuesta";
					
					if (AddrS == addDefault || AddrS == "")
						AddrS = "Sin respuesta";
					
					logicScript.nameOfPlayer = nameS;
					logicScript.addressOfPlayer = AddrS;
					logicScript.currentDate = "NA";
					nivel1.enabled = false;
					logicScript.curGameFinished = true;
				}
				break;
			case "Step3":
				GUI.SetNextControlName("Nombre");
				nameS = GUI.TextField(new Rect(Screen.width * .5f - 320 * screenScale, Screen.height * .5f + 30 * screenScale , 350 * screenScale, 100 * screenScale), nameS, ageStyle);
				if(UnityEngine.Event.current.type == EventType.Repaint)
				{
					if(GUI.GetNameOfFocusedControl() == "Nombre")
					{
						if(nameS == nameDefault)
						{
							nameS = "";
						}
					}
					else
					{
						if(nameS == "")
						{
							nameS = nameDefault;
						}
					}
					
				}
				GUI.SetNextControlName("Add");
				AddrS = GUI.TextField(new Rect(Screen.width * .5f - 320 * screenScale, Screen.height * .5f + 110 * screenScale, 350 * screenScale, 100 * screenScale), AddrS, ageStyle);
				if(UnityEngine.Event.current.type == EventType.Repaint)
				{
					if(GUI.GetNameOfFocusedControl() == "Add")
					{
						if(AddrS == addDefault)
						{
							AddrS = "";
						}
					}
					else
					{
						if(AddrS == "")
						{
							AddrS = addDefault;
						}
					}
				}
				GUI.SetNextControlName("Birth");
				dateS = GUI.TextField(new Rect(Screen.width * .5f - 320 * screenScale, Screen.height * .5f + 200 * screenScale, 350 * screenScale, 100 * screenScale), dateS, ageStyle);
				if(UnityEngine.Event.current.type == EventType.Repaint)
				{
					if(GUI.GetNameOfFocusedControl() == "Birth")
					{
						if(dateS == dateDefault)
						{
							dateS = "";
						}
					}
					else
					{
						if(dateS == "")
						{
							dateS = dateDefault;
						}
					}
				}
				if(GUI.Button(new Rect(Screen.width * .5f + 225 * screenScale, Screen.height * .5f + 235 * screenScale, 160 * screenScale, 80 * screenScale), "LISTO", advanceStyle))
				{
					if (nameS == nameDefault || nameS == "")
						nameS = "Sin respuesta";

					if (AddrS == addDefault || AddrS == "")
						AddrS = "Sin respuesta";

					if (dateS == dateDefault || dateS == "")
						dateS = "Sin respuesta";
					
					logicScript.nameOfPlayer = nameS;
					logicScript.addressOfPlayer = AddrS;
					logicScript.currentDate = dateS;
					nivel1.enabled = false;
					logicScript.curGameFinished = true;
				}
				break;
				
			}
		}

	}
}

