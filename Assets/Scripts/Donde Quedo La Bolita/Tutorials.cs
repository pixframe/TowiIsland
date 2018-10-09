using UnityEngine;
using System.Collections;

public class Tutorials : MonoBehaviour
{
	//scripts vars
	WhereIsTheBallLogic logicScript;
	
	
	//Logic vars
	public string state;
	public string showState;
	public string chooseState;
	public float time = 2f;
	public float timer;
	public bool advance = false;
	
	
	//tut vars
	public GUIText line1;
	public GUIText line2;
	public GUIText line3;
	
	
	// Use this for initialization
	void Start ()
	{
		logicScript = GameObject.FindGameObjectWithTag("Main").GetComponent<WhereIsTheBallLogic>();
		line1 = transform.Find("Line1").GetComponent<GUIText>();
		line2 = transform.Find("Line2").GetComponent<GUIText>();
		line3 = transform.Find("Line3").GetComponent<GUIText>();
		timer = 0f;
		advance = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		advance = true;
//		state = logicScript.state;
//		showState = logicScript.showState;
//		chooseState = logicScript.chooseState;
//		if(logicScript.tutorial && !logicScript.moveToPos)
//		{
//			
//			if(state == "Start")
//			{
//				line1.text = "A los changuitos les gusta esconder todo lo que encuentran.";
//				if(!advance && Timer(2f))
//				{
//					advance = true; 
//					line2.text = "Da click para avanzar!!!";
//					line2.pixelOffset = new Vector2(50, 0);
//					
//				}
//				
//			}
//			else if(state == "ShowObjects")
//			{
//				
//				if(showState == "Show")
//				{
//					line1.text = "Recuerda bien que changito tiene el objeto.";
//					line1.pixelOffset = new Vector2(50, 0);
//					if(!advance && Timer(2f))
//					{
//						advance = true; 
//						line2.text = "Da click para avanzar!!!";
//						line2.pixelOffset = new Vector2(50, 0);
//					}
//					
//				}
//				else if(showState == "WaitForMovements")
//				{
//					line1.text = "Estas Listo?!?!";
//					line1.pixelOffset = new Vector2(120, 0);
//					if(!advance && Timer(2f))
//					{
//						advance = true; 
//						line2.text = "Da click para avanzar!!!";
//						line2.pixelOffset = new Vector2(50, 0);
//					}
//				}
//			}
//			
//		}
//		else if(!logicScript.tutorial)
//		{
//			if(state == "Choose")
//			{
//				if(chooseState == "Instructions")
//				{
//					
//				}
//			}
//		}
	}
	
	public bool Timer(float time)
	{
		
		timer += Time.deltaTime;
		
		if(timer >= time)
		{
			timer = 0;
			return true;
		}
		else
		{
			return false;
		}
	}
	public void ClearLines(int lineNum1, int lineNum2, int lineNum3)// if linenum1,2,3 are one it gets ereased
	{
		if(line1.text != "" && lineNum1 == 1)
		{
			line1.text = "";
		}
		if(line2.text != "" && lineNum2 == 1)
		{
			line2.text = "";
		}
		if(line3.text != "" && lineNum3 == 1)
		{
			line3.text = "";
		}
	}
}
