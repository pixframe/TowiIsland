using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AirportRouteLogic : MonoBehaviour
{
	PEMainLogic mainLogic;
	CarControl carCScript;
    TrailRenderer trail;
	PackLogic packScript;
	public List<GameObject> labyrinths = new List<GameObject>();

	GameObject car;
	public string state;

	public bool startLab = false;
	public bool labFinished = false;
	public int labNum;


	public float[] times;
	public float[] latency;
	public int[] hits;
	public int[] crosses;
	public int[] deadEnds;

	// Use this for initialization
	void Start ()
	{
		carCScript = transform.Find("Car").GetComponent<CarControl>();
        trail = transform.Find("Car").GetComponent<TrailRenderer>();
		packScript = GameObject.Find("Pack").GetComponent<PackLogic>();
		mainLogic = GameObject.Find("Main").GetComponent<PEMainLogic>();
		times = new float[3];
		latency = new float[3];
		hits = new int[3];
		crosses = new int[3];
		deadEnds = new int[3];
		car = transform.Find("Car").gameObject;
		
	}

	// Update is called once per frame
	void Update ()
	{

		switch(state){
		case "Intro":
			break;
		case "Lab1":
			carCScript.carOn = true;
			times[0] += Time.deltaTime;
			labNum = 0;
            if(trail.time==0)
                trail.time = 6000;
			if(mainLogic.ageOfPlayer <= 8)
			{
				if(times[0] >= 240)
				{
					carCScript.carOn = false;
					state = "CamTrans";
				}
			}
			else if(mainLogic.ageOfPlayer > 8)
			{
				if(times[0] >= 350)
				{
					carCScript.carOn = false;
					state = "CamTrans";
				}
			}
			break;
		case "Lab2":
			carCScript.carOn = true;
			times[1] += Time.deltaTime;
			labNum = 1;
			if(mainLogic.ageOfPlayer <= 8)
			{
				if(times[1] >= 240)
				{
					carCScript.carOn = false;
					state = "CamTrans";
				}
			}
			else if(mainLogic.ageOfPlayer > 8)
			{
				if(times[1] >= 350)
				{
					carCScript.carOn = false;
					state = "CamTrans";
				}
			}
			break;
		case "Lab3":
			carCScript.carOn = true;
			times[2] += Time.deltaTime;
			labNum = 2;
			if(mainLogic.ageOfPlayer <= 8)
			{
				if(times[2] >= 240)
				{
                    mainLogic.curGameFinished = true;
                    carCScript.carOn = false;
                    state = "Default";
                    carCScript.carSelected = false;
				}
			}
			else if(mainLogic.ageOfPlayer > 8)
			{
				if(times[2] >= 350)
				{
                    mainLogic.curGameFinished = true;
                    carCScript.carOn = false;
                    state = "Default";
                    carCScript.carSelected = false;
				}
			}
			break;
		case "CamTrans":
            trail.time = 0;
			carCScript.carOn = false;
			carCScript.carSelected = false;
			switch(labNum){
			case 0:

				car.transform.position = labyrinths[1].transform.Find("Start").transform.position;
				Vector3 trans1 = labyrinths[1].transform.Find("CamPos").transform.position - Camera.main.transform.position;
				Camera.main.transform.Translate(trans1.normalized);
				if(labyrinths[1].transform.Find("CamPos").transform.position.y - Camera.main.transform.position.y <= 0)
				{
					Camera.main.transform.position = labyrinths[1].transform.Find("CamPos").transform.position;
					carCScript.latencyOn = true;
					state = "Lab2";
                    trail.time = 6000;
				}
				break;

			case 1:

				car.transform.position = labyrinths[2].transform.Find("Start").transform.position;
				Vector3 trans2 = labyrinths[2].transform.Find("CamPos").transform.position - Camera.main.transform.position;
				Camera.main.transform.Translate(trans2.normalized);
				if(labyrinths[2].transform.Find("CamPos").transform.position.y - Camera.main.transform.position.y <= 0)
				{
					Camera.main.transform.position = labyrinths[2].transform.Find("CamPos").transform.position;
					carCScript.latencyOn = true;
					state = "Lab3";
                    trail.time = 6000;
				}
				break;
			}
			break;
		}
	}
}
