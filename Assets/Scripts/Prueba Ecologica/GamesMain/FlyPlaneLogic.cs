using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlyPlaneLogic : MonoBehaviour
{
	Timer timerScript;
	PEMainLogic logicScript;
	PackLogic packScript;
	public string state;
	public string[] arrowColor;
	public string[] arrowDir;
	public string[] arrowColorTutorial;
	public string[] arrowDirTutorial;
	public int tutorialError = -1;
	GameObject arrow;
	ParticleSystem arrowPart;
	public float timeForInput;
	public int posDir = 0;
	public int posDirTutorial = 0;
	public int correctTutorial;
	public int incorrectTutorial;
	public int missedTutorial;
	public int correct;
	public int incorrect;
	public int missed;
	public int correctGreen;
	public int incorrectGreen;
	public int missedGreen;
	GameObject clouds;
	bool keyPressed = false;
	public float cloudsMoveDist = 4f;
	float timerCount;
	bool cor = false;
	public bool mobile = false;
	public float[] latency;

	//Variable tutorial
	public bool tutorialDone = false;

	//GUI VARS
	float screenScale;
	public Texture2D inputArrowRight;
	public Texture2D inputArrowLeft;
	public Texture2D inputArrowUp;
	public Texture2D inputArrowDown;
	public GUIStyle butStyle;


	//Touch myTouch ;
	//public Swipe swipeControls;

	bool isLeft = false;
	bool isRight = false;

	public GameObject canvas;
	//Use this for initialization
	void Start ()
	{
		latency =  new float[30];
		packScript = GameObject.Find("Pack").GetComponent<PackLogic>();
		timerScript = GameObject.FindGameObjectWithTag("Main").GetComponent<Timer>();
		logicScript = GameObject.FindGameObjectWithTag("Main").GetComponent<PEMainLogic>();
		//arrow = transform.FindChild("Arrow").gameObject;
		//arrowPart = arrow.transform.FindChild("ArrowPart").GetComponent<ParticleSystem>();
		clouds = transform.Find("Clouds").gameObject;
		state = "Intro";
		//arrow.GetComponent<SpriteRenderer>().enabled = false;

		timerCount = timeForInput;
		if(SystemInfo.deviceType == DeviceType.Handheld)
		{
			mobile = true;
		}
		else
		{
			mobile = false;
		}
	}

	public void RightBut()
	{
		isRight = true;
		StartCoroutine(Reset());
	}

	public void LeftBut()
	{
		isLeft = true;
		StartCoroutine(Reset());
	}

	IEnumerator Reset()
	{
		yield return new WaitForSeconds(0.1f);
		isLeft = false;
		isRight = false;

	}

	// Update is called once per frame
	void Update ()
	{

		/*if (Input.touchCount > 0) 
		{
			myTouch = Input.GetTouch(0);
		}*/

		if(logicScript.miniGame == "FlyPlane")
		switch(state){

		case "Intro":
		break;

		case "TutorialEnd":
		break;

		case "SetArrowTutorial":
			timerCount = timeForInput;
			keyPressed = false;
			canvas.gameObject.SetActive(true);

			if(posDirTutorial == arrowDirTutorial.Length || tutorialDone)
			{
				switch (tutorialError) {
					case -1:
						state = "TutorialEnd";
					break;
					case 0:
						state = "TutorialError0";//las NO verdes van de lado normal
					break;
					case 1:
						state = "TutorialError1";//las verdes van de lado contrario
					break;
					case 2:
						state = "TutorialError2";//las flechas de arriba o abajo no se usan 
					break;
					case 3:
						state = "TutorialError3"; //missed
					break;
				}
				
				break;	
			}

			if(arrow!=null)
			{
				arrow.GetComponent<SpriteRenderer>().enabled = false;
			}

			ArrowColor(posDirTutorial);
			
			arrowPart.Clear();
			ArrowPosTutorial(posDirTutorial);
			ArrowDirTutorial(posDirTutorial);
			arrow.GetComponent<SpriteRenderer>().enabled = true;
			
			state = "InputTutorial";
		break;

		case "InputTutorial":
			
			timerCount -= Time.deltaTime;
			if(true)
			{
				if(timerCount > 0)
				{
					if(!keyPressed)
					{
						
						if(Input.GetKeyDown(KeyCode.RightArrow) 
						|| Input.GetMouseButtonDown(0)&& Input.mousePosition.x  > Screen.width/2)

						{
							latency[posDirTutorial] = (timeForInput - timerCount);
							
							keyPressed = true;
							if(arrowColorTutorial[posDirTutorial] != "ArrowGreen")
							{
								if(arrowDirTutorial[posDirTutorial] == "Right") 
								{
									arrow.GetComponent<SpriteRenderer>().enabled = false;
									arrowPart.Play();
									correctTutorial++;
									cor = true;
									Debug.Log("Correct");
								}
								else
								{
									arrow.GetComponent<SpriteRenderer>().enabled = false;
									arrowPart.Play();
									incorrectTutorial++;
									tutorialError = 0; //las NO verdes van de lado normal
									tutorialDone = true;

									cor = true;
									Debug.Log("Incorrect");
								}
								
							}
							else
							{
								if(arrowDirTutorial[posDirTutorial] == "Left" )//lado contrario
								{
									arrow.GetComponent<SpriteRenderer>().enabled = false;
									arrowPart.Play();
									correctTutorial++;
									cor = true;
									Debug.Log("Correct");
								}
								else
								{
									arrow.GetComponent<SpriteRenderer>().enabled = false;
									arrowPart.Play();
									incorrectTutorial++;
									tutorialDone = true;
									tutorialError = 1; //las verdes son de lado contrario
									cor = true;
									Debug.Log("Incorrect");
								}
								
							}
							
						}
						else if(Input.GetKeyDown(KeyCode.LeftArrow) 
							|| Input.GetMouseButtonDown(0) && Input.mousePosition.x  < Screen.width/2)

						if(true)
						{
							latency[posDirTutorial] = (timeForInput - timerCount);
							
							keyPressed = true;
							if (arrowColorTutorial[posDirTutorial] != "ArrowGreen")
							{
								
								if(arrowDirTutorial[posDirTutorial] == "Left" )
								{
									arrow.GetComponent<SpriteRenderer>().enabled = false;
									arrowPart.Play();
									correctTutorial++;
									cor = true;
									Debug.Log("Correct");
								}
								else
								{
									arrow.GetComponent<SpriteRenderer>().enabled = false;
									arrowPart.Play();
									incorrectTutorial++;
									tutorialError = 0; //las NO verdes van de lado normal
									tutorialDone = true;
									cor = true;
									Debug.Log("Incorrect");
								}
								
							}
							else
							{
								
								if(arrowDirTutorial[posDirTutorial] == "Right" )//lado contrario
								{
									arrow.GetComponent<SpriteRenderer>().enabled = false;
									arrowPart.Play();
									correctTutorial++;
									cor = true;
									Debug.Log("Correct");
								}
								else
								{
									arrow.GetComponent<SpriteRenderer>().enabled = false;
									arrowPart.Play();
									incorrectTutorial++;
									tutorialError = 1; //las verdes van de lado contrario
									tutorialDone = true;
									cor = true;
									Debug.Log("Incorrect");
								}
							}
						}
						else if(Input.GetKeyDown(KeyCode.UpArrow))
						{
							latency[posDirTutorial] = (timeForInput - timerCount);
							
							keyPressed = true;

							arrow.GetComponent<SpriteRenderer>().enabled = false;;
							arrowPart.Play();;
							incorrectTutorial++;
							tutorialError = 2; //las flechas de arriba o abajo no se usan
							tutorialDone = true;
							cor = true;
							Debug.Log("Incorrect");
						
						}
						else if(Input.GetKeyDown(KeyCode.DownArrow))
						{
							latency[posDirTutorial] = (timeForInput - timerCount);
							
							keyPressed = true;

							arrow.GetComponent<SpriteRenderer>().enabled = false;
							arrowPart.Play();
							incorrectTutorial++;
							tutorialError = 2; //las flechas de arriba o abajo no se usan
							cor = true;
							Debug.Log("Incorrect");

						}
					}
					
					if(cor)
					{
						//if(!arrowPart.IsAlive())
						//{
							keyPressed = false;
							posDirTutorial++;
							Debug.Log(posDirTutorial);
							timerCount = 0;
							cor = false;
							state = "SetArrowTutorial";
							break;
						//}
						
					}

				}
				else
				{
					//if(!arrowPart.IsAlive())
					//{
						if(!keyPressed)
						{
							arrow.GetComponent<SpriteRenderer>().enabled = false;
							missedTutorial++;
							Debug.Log("MissedTut");
							tutorialError = 3; //missed flecha
							tutorialDone = true;
						}
						cor = false;
						posDirTutorial++;
						Debug.Log(posDirTutorial);
						timerCount = 0;
						state = "SetArrowTutorial";
						break;
					//}
				}
			}
			
		break;

		case "SetArrow":
			timerCount = timeForInput;
			keyPressed = false;
			if(posDir == arrowDir.Length)
			{
				state = "Arrive";
				break;
			}
            if(arrow!=null)
            {
                arrow.GetComponent<SpriteRenderer>().enabled = false;
            }
            ArrowColor(posDir);

			arrowPart.Clear();
			ArrowPos(posDir);
			ArrowDir(posDir);
			arrow.GetComponent<SpriteRenderer>().enabled = true;

			state = "Input";
		break;

		case "Input":
			canvas.gameObject.SetActive(false);
			timerCount -= Time.deltaTime;
			if(true)
			{
				if(timerCount > 0)
				{
					if(!keyPressed)
					{

						if(Input.GetKeyDown(KeyCode.RightArrow) || 
							Input.GetMouseButtonDown(0)&& Input.mousePosition.x  > Screen.width/2)
						{
							latency[posDir] = (timeForInput - timerCount);

							keyPressed = true;
							if(arrowColor[posDir] != "ArrowGreen")
							{
								if(arrowDir[posDir] == "Right")
								{
									arrow.GetComponent<SpriteRenderer>().enabled = false;
									arrowPart.Play();
									correct++;
									cor = true;
	                                Debug.Log("Correct");
								}
								else
								{
									arrow.GetComponent<SpriteRenderer>().enabled = false;
									arrowPart.Play();
									incorrect++;
									cor = true;
									Debug.Log("Incorrect");
								}
								
							}
							else
							{
								if(arrowDir[posDir] == "Left")//lado contrario
								{
									arrow.GetComponent<SpriteRenderer>().enabled = false;
									arrowPart.Play();
									correct++;
									correctGreen++;
									cor = true;
									Debug.Log("Correct");
								}
								else
								{
									arrow.GetComponent<SpriteRenderer>().enabled = false;
									arrowPart.Play();
									incorrect++;
									incorrectGreen++;
									cor = true;
									Debug.Log("Incorrect");
								}
								
							}
							
						}
						else if(Input.GetKeyDown(KeyCode.LeftArrow) || 
							Input.GetMouseButtonDown(0)&& Input.mousePosition.x  < Screen.width/2)
						{
							latency[posDir] = (timeForInput - timerCount);

							keyPressed = true;
                            if (arrowColor[posDir] != "ArrowGreen")
							{

								if(arrowDir[posDir] == "Left")
								{
									arrow.GetComponent<SpriteRenderer>().enabled = false;
									arrowPart.Play();
									correct++;
									cor = true;
									Debug.Log("Correct");
								}
								else
								{
									arrow.GetComponent<SpriteRenderer>().enabled = false;
									arrowPart.Play();
									incorrect++;
									cor = true;
									Debug.Log("Incorrect");
								}
								
							}
							else
							{

								if(arrowDir[posDir] == "Right")//lado contrario
								{
									arrow.GetComponent<SpriteRenderer>().enabled = false;
									arrowPart.Play();
									correct++;
									correctGreen++;
									cor = true;
									Debug.Log("Correct");
								}
								else
								{
									arrow.GetComponent<SpriteRenderer>().enabled = false;
									arrowPart.Play();
									incorrect++;
									incorrectGreen++;
									cor = true;
									Debug.Log("Incorrect");
								}
							}
						}
						else if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
						{
							latency[posDir] = (timeForInput - timerCount);

							keyPressed = true;

							arrow.GetComponent<SpriteRenderer>().enabled = false;
							arrowPart.Play();
							incorrect++;

							if (arrowColor [posDir] != "ArrowGreen")
								incorrectGreen++;
							
							cor = true;
							Debug.Log("Incorrect");
                            /*if (arrowColor[posDir] != "ArrowGreen" && arrowDir[posDir] == "Up")
							{

								arrow.GetComponent<SpriteRenderer>().enabled = false;
								arrowPart.Play();
								correct++;
								cor = true;
                                Debug.Log("Correct");
							}
							else
							{

								arrow.GetComponent<SpriteRenderer>().enabled = false;
								arrowPart.Play();
								incorrect++;
								cor = true;
								Debug.Log("Incorrect");
							}*/
						}
						/*else if(Input.GetKeyDown(KeyCode.DownArrow))
						{
							latency[posDir] = (timeForInput - timerCount);

							keyPressed = true;
                            if (arrowColor[posDir] != "ArrowGreen" && arrowDir[posDir] == "Down")
							{

								arrow.GetComponent<SpriteRenderer>().enabled = false;
								arrowPart.Play();
								correct++;
								cor = true;
                                Debug.Log("Correct");
							}
							else
							{
							
								arrow.GetComponent<SpriteRenderer>().enabled = false;
								arrowPart.Play();
								incorrect++;
								cor = true;
								Debug.Log("Incorrect");
							}
						}*/
					}

					if(cor)
					{
						//if(!arrowPart.IsAlive())
						//{
                            keyPressed = false;
							posDir++;
							timerCount = 0;
							cor = false;
							state = "SetArrow";
							break;
						//}

					}
					
					
				}
				else
				{
					//if(!arrowPart.IsAlive())
					//{
						if(!keyPressed)
						{
							arrow.GetComponent<SpriteRenderer>().enabled = false;
							missed++;
							if (arrowColor [posDir] == "ArrowGreen") {
								missedGreen++;
							}
							Debug.Log("Missed");
						}
						cor = false;
						posDir++;
						state = "SetArrow";

						break;
					//}
				}
			}

			break;
		case "Arrive":
			break;
		case "End":

			logicScript.curGameFinished = true;
			break;
		}
	}

	//Funciones para setear el color y posicion de las flechas

	void ArrowColorTutorial(int posNum)
	{
		arrow = transform.Find(arrowColorTutorial[posNum]).gameObject;
		arrowPart = arrow.transform.Find("ArrowPart").GetComponent<ParticleSystem>();
	}
	
	//Esta funcion se puede usar si se desea cambiar la nube en la que aparece la flecha. Por ahora el default es en la nube del centro
	void ArrowPosTutorial(int posNum)
	{
		arrow.transform.position = transform.Find("MiddleCloud").transform.position;
	}
	
	void ArrowDirTutorial(int dirNum)
	{
		arrow.transform.rotation = Quaternion.identity;
		if(arrowDirTutorial[dirNum] == "Right")
		{
			arrow.transform.Rotate(new Vector3(0, 0, 0));
		}
		else if(arrowDirTutorial[dirNum] == "Left")
		{
			arrow.transform.Rotate(new Vector3(0, 0, 180));
		}
		else if(arrowDirTutorial[dirNum] == "Up")
		{
			arrow.transform.Rotate(new Vector3(0, 0, 90));
		}
		else if(arrowDirTutorial[dirNum] == "Down")
		{
			arrow.transform.Rotate(new Vector3(0, 0, 270));
		}
	}

    void ArrowColor(int posNum)
    {
        arrow = transform.Find(arrowColor[posNum]).gameObject;
        arrowPart = arrow.transform.Find("ArrowPart").GetComponent<ParticleSystem>();
    }

	//Esta funcion se puede usar si se desea cambiar la nube en la que aparece la flecha. Por ahora el default es en la nube del centro
	void ArrowPos(int posNum)
	{
		arrow.transform.position = transform.Find("MiddleCloud").transform.position;
	}

	void ArrowDir(int dirNum)
	{
		arrow.transform.rotation = Quaternion.identity;
		if(arrowDir[dirNum] == "Right")
		{
			arrow.transform.Rotate(new Vector3(0, 0, 0));
		}
		else if(arrowDir[dirNum] == "Left")
		{
			arrow.transform.Rotate(new Vector3(0, 0, 180));
		}
		else if(arrowDir[dirNum] == "Up")
		{
			arrow.transform.Rotate(new Vector3(0, 0, 90));
		}
		else if(arrowDir[dirNum] == "Down")
		{
			arrow.transform.Rotate(new Vector3(0, 0, 270));
		}
	}

	public void StartTutorial()
	{

		tutorialDone = false;
		tutorialError = -1;
		posDirTutorial = 0;
		state = "Intro";
	}
	
	public void StartGame()
	{
		state = "Input";
	}

	void OnGUI()
	{
		screenScale = (float)Screen.height / (float)768;
		if(state == "Input" )
		{
			if(mobile) //Se debe repensar la lógica de mobiles ya que no tiene la actualización de todo el juego
			{
				GUI.DrawTexture(new Rect(Screen.width * .5f + 120 * screenScale, Screen.height * .5f + 250 * screenScale, 100 * screenScale, 100 * screenScale), inputArrowRight);
				GUI.DrawTexture(new Rect(Screen.width * .5f - 220 * screenScale, Screen.height * .5f + 250 * screenScale, 100 * screenScale, 100 * screenScale), inputArrowLeft);
				GUI.DrawTexture(new Rect(Screen.width * .5f - 60 * screenScale, Screen.height * .5f + 120 * screenScale, 100 * screenScale, 100 * screenScale), inputArrowUp);
				GUI.DrawTexture(new Rect(Screen.width * .5f - 60 * screenScale, Screen.height * .5f + 250 * screenScale, 100 * screenScale, 100 * screenScale), inputArrowDown);
				if(timerCount > 0)
				{
					if(!keyPressed)
					{
						if(GUI.Button(new Rect(Screen.width * .5f + 120 * screenScale, Screen.height * .5f + 250 * screenScale, 100 * screenScale, 100 * screenScale), "", butStyle))
						{
							latency[posDir] = (timeForInput - timerCount);
							keyPressed = true;
                            if (arrowColor[posDir] != "ArrowGreen" && arrowDir[posDir] == "Right")
							{

								arrow.GetComponent<SpriteRenderer>().enabled = false;
								arrowPart.Play();
								correct++;
								cor = true;
                                Debug.Log("Correct");
								
								
							}
							else
							{
								arrow.GetComponent<SpriteRenderer>().enabled = false;
								arrowPart.Play();
								incorrect++;
								cor = true;
								Debug.Log("Incorrect");
							}
						}
						if(GUI.Button(new Rect(Screen.width * .5f - 220 * screenScale, Screen.height * .5f + 250 * screenScale, 100 * screenScale, 100 * screenScale), "", butStyle))
						{
							latency[posDir] = (timeForInput - timerCount);
							keyPressed = true;
                            if (arrowColor[posDir] != "ArrowGreen" && arrowDir[posDir] == "Left")
							{

								arrow.GetComponent<SpriteRenderer>().enabled = false;
								arrowPart.Play();
								correct++;
								cor = true;
                                Debug.Log("Correct");
								
							}
							else
							{
								arrow.GetComponent<SpriteRenderer>().enabled = false;
								arrowPart.Play();
								incorrect++;
								cor = true;
								Debug.Log("Incorrect");
							}
						}
						if(GUI.Button(new Rect(Screen.width * .5f - 60 * screenScale, Screen.height * .5f + 120 * screenScale, 100 * screenScale, 100 * screenScale), "", butStyle))
						{
							latency[posDir] = (timeForInput - timerCount);
							keyPressed = true;
                            if (arrowColor[posDir] != "ArrowGreen" && arrowDir[posDir] == "Up")
							{

								arrow.GetComponent<SpriteRenderer>().enabled = false;;
								arrowPart.Play();
								correct++;
								cor = true;
                                Debug.Log("Correct");
							}
							else
							{
								arrow.GetComponent<SpriteRenderer>().enabled = false;
								arrowPart.Play();
								incorrect++;
								cor = true;
								Debug.Log("Incorrect");
							}
						}
						if(GUI.Button(new Rect(Screen.width * .5f - 60 * screenScale, Screen.height * .5f + 250 * screenScale, 100 * screenScale, 100 * screenScale), "", butStyle))
						{
							latency[posDir] = (timeForInput - timerCount);
							keyPressed = true;
                            if (arrowColor[posDir] != "ArrowGreen" && arrowDir[posDir] == "Down")
							{

								arrow.GetComponent<SpriteRenderer>().enabled = false;
								arrowPart.Play();
								correct++;
								cor = true;
                                Debug.Log("Correct");
							}
							else
							{
								arrow.GetComponent<SpriteRenderer>().enabled = false;
								arrowPart.Play();
								incorrect++;
								cor = true;
								Debug.Log("Incorrect");
							}
						}
					}
					else
					{
						GUI.DrawTexture(new Rect(Screen.width * .5f + 120 * screenScale, Screen.height * .5f + 250 * screenScale, 100 * screenScale, 100 * screenScale), inputArrowRight);
						GUI.DrawTexture(new Rect(Screen.width * .5f - 220 * screenScale, Screen.height * .5f + 250 * screenScale, 100 * screenScale, 100 * screenScale), inputArrowLeft);
						GUI.DrawTexture(new Rect(Screen.width * .5f - 60 * screenScale, Screen.height * .5f + 120 * screenScale, 100 * screenScale, 100 * screenScale), inputArrowUp);
						GUI.DrawTexture(new Rect(Screen.width * .5f - 60 * screenScale, Screen.height * .5f + 250 * screenScale, 100 * screenScale, 100 * screenScale), inputArrowDown);
					}


					if(cor)
					{
						//if(!arrowPart.isPlaying)
						//{

							posDir++;
							timerCount = 0;
							cor = false;
							state = "SetArrow";

						//}
					}
				}
				else if(!keyPressed)
				{
					//if(!arrowPart.IsAlive())
					//{
						if(!keyPressed)
						{
							arrow.GetComponent<SpriteRenderer>().enabled = false;
							missed++;
							if (arrowColor [posDir] == "ArrowGreen") {
								missedGreen++;
							}
							Debug.Log("Missed");
						}
						state = "SetArrow";
						cor = false;
						posDir++;

					//}
				}
			}
		}
		else if(state == "SetArrow")
		{
			if(mobile)
			{
				GUI.DrawTexture(new Rect(Screen.width * .5f + 120 * screenScale, Screen.height * .5f + 250 * screenScale, 100 * screenScale, 100 * screenScale), inputArrowRight);
				GUI.DrawTexture(new Rect(Screen.width * .5f - 220 * screenScale, Screen.height * .5f + 250 * screenScale, 100 * screenScale, 100 * screenScale), inputArrowLeft);
				GUI.DrawTexture(new Rect(Screen.width * .5f - 60 * screenScale, Screen.height * .5f + 120 * screenScale, 100 * screenScale, 100 * screenScale), inputArrowUp);
				GUI.DrawTexture(new Rect(Screen.width * .5f - 60 * screenScale, Screen.height * .5f + 250 * screenScale, 100 * screenScale, 100 * screenScale), inputArrowDown);
			}

		}
	}
}
