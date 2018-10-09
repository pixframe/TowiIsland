using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickUpCoinsLogic : MonoBehaviour
{
	PEMainLogic logicScript;
	Timer timerScript;
	PackLogic packScript;

	public string state;

	Coin coinScript;
	public int minuteCorrect;
	public int minuteIncorrect;
	public int minuteMissed;
	public int extraCorrect;
	public int extraIncorrect;
	public int extraMissed;
	public List<string> coinsSelected = new List<string>();
	public bool overMin = false;
	public int beforeMinClick = 0;
	// Use this for initialization
	void Start ()
	{
		logicScript = GameObject.FindGameObjectWithTag("Main").GetComponent<PEMainLogic>();
		timerScript = GameObject.FindGameObjectWithTag("Main").GetComponent<Timer>();
		packScript = GameObject.Find("Pack").GetComponent<PackLogic>();


		state = "Intro";
	}

	// Update is called once per frame
	void Update ()
	{
		if(logicScript.miniGame == "PickUpCoins")
		{
			switch(state){
			case "Intro":
				break;
			case "PickUpPhaseInTime":

				if(!timerScript.TimerFunc(55))
				{
					overMin = false;
					if(packScript.DownUpPress())
					{

						if(packScript.s.tag == "Coin")
						{
							coinsSelected.Add(packScript.s.name.Remove(0,4));
							coinScript = packScript.s.GetComponent<Coin>();
							if(coinScript.star)
							{
								minuteCorrect++;
							}
							else
							{
								minuteIncorrect++;
							}
							packScript.s.SetActive(false);
						}
					}
				}
				else
				{
					overMin = true;
					minuteMissed = 24 - minuteCorrect;
					state = "PickUpPhaseExtra";
				}
				break;
			case "PickUpPhaseExtra":

				if(packScript.DownUpPress())
				{
					if(packScript.s.tag == "Coin")
					{
						coinsSelected.Add(packScript.s.name);
						coinScript = packScript.s.GetComponent<Coin>();
						if(coinScript.star)
						{
							extraCorrect++;
						}
						else
						{
							extraIncorrect++;
						}
						packScript.s.SetActive(false);
					}
				}
				break;
			case "End":
				logicScript.curGameFinished = true;
				break;

			}
		}
	}

}
