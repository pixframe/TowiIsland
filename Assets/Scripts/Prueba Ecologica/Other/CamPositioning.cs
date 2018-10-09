using UnityEngine;
using System.Collections;

public class CamPositioning : MonoBehaviour
{
	PEMainLogic logicScript;
	AirportRouteLogic routeLogic;
	PackLogic packScript;

	public GameObject agePos;
	public GameObject ticketPos;
	public GameObject packPos;
	public GameObject planPos;
	public GameObject waitPos;
	public GameObject flyPos;
	public GameObject coinsPos;
	public GameObject unPackPos;
	
	// Use this for initialization
	void Start ()
	{
		logicScript = GameObject.Find("Main").transform.GetComponent<PEMainLogic>();
		transform.position = agePos.transform.position;
		Camera.main.orthographicSize = 4.79f;
		packScript = GameObject.Find("Pack").GetComponent<PackLogic>();
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(logicScript.miniGame == "FadeOut" && logicScript.fadeScript.mat.color.a >= 1f)
		{
				switch(logicScript.nextGame){
				
			case "InputAge":
				Camera.main.orthographic = true;
				transform.position = agePos.transform.position;
				logicScript.miniGame = "FadeIn";
				break;	
			case "BuyTicket":
				transform.position = ticketPos.transform.position;
				logicScript.miniGame = "FadeIn";
				break;
			case "Pack":
				Camera.main.orthographic = false;
				transform.position = packPos.transform.position;
				transform.Rotate(0, 104, 0);

				logicScript.miniGame = "FadeIn";
				break;
			case "PlanRoute":
				transform.position = planPos.transform.position;
				Camera.main.transform.rotation = Quaternion.identity;
				Camera.main.orthographic = true;
				Camera.main.orthographicSize = 4.79f;
				logicScript.miniGame = "FadeIn";
				break;
			case "WaitingRoom":
				transform.position = waitPos.transform.position;
				Camera.main.orthographic = true;
				Camera.main.orthographicSize = 4.79f;
				logicScript.miniGame = "FadeIn";
				break;
			case "FlyPlane":
				transform.position = flyPos.transform.position;
				Camera.main.orthographic = true;
				logicScript.miniGame = "FadeIn";
				break;
			case "PickUpCoins":
				transform.position = coinsPos.transform.position;
				Camera.main.orthographic = true;
				Camera.main.orthographicSize = 5.4f;
				logicScript.miniGame = "FadeIn";
				break;
			case "UnPack":
				Camera.main.orthographic = false;
//				Camera.main.orthographicSize = 6;
//				transform.position = unPackPos.transform.position;
				transform.position = packPos.transform.position;
				packScript.clearObjs();
				transform.Rotate(0, 104, 0);

				logicScript.miniGame = "FadeIn";
				break;
			case "End":
				logicScript.miniGame = "End";
				break;
			}	
			
		}
		
	}
}
