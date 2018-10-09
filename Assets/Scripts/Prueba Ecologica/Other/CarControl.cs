using UnityEngine;
using System.Collections;

public class CarControl : MonoBehaviour
{
	//scripts
	SelectionScript selScript;
	AirportRouteLogic routeLogic;
	PEMainLogic mainScript;

	public bool carOn = false;
	public bool latencyOn = true;

	//functionality vars
	public bool carSelected = false;
	Vector3 curPos;
	Vector3 prevPos;
	Vector3 lookDir;
	Vector3 prevFow;
	ParticleSystem carFlames;
	ParticleSystem carFlamesMove1;
	ParticleSystem carFlamesMove2;

	// Use this for initialization
	void Start ()
	{
		mainScript = GameObject.FindGameObjectWithTag("Main").GetComponent<PEMainLogic>();
		selScript = GameObject.FindGameObjectWithTag("Main").GetComponent<SelectionScript>();
		routeLogic = transform.parent.GetComponent<AirportRouteLogic>();
		transform.position = routeLogic.labyrinths[0].transform.Find("Start").position;
		carFlames = transform.Find("CarStill").GetComponent<ParticleSystem>();
		carFlamesMove1 = transform.Find("CarPointer1").GetComponent<ParticleSystem>();
		carFlamesMove2 = transform.Find("CarPointer2").GetComponent<ParticleSystem>();
	}

	// Update is called once per frame
	void Update ()
 	{

		if(mainScript.miniGame == "PlanRoute" && routeLogic.state != "CamTrans" && carOn)
		{
			if(!carSelected)
			{
				if(!carFlames.isPlaying)
				{
					Debug.Log("clear");
					carFlames.Clear();
					carFlames.Play();
					if(carFlamesMove1.isPlaying)
					{
						carFlamesMove1.Stop();
						carFlamesMove1.Clear();
					}
					if(carFlamesMove2.isPlaying)
					{
						carFlamesMove2.Stop();
						carFlamesMove2.Clear();
					}
				}
				if(latencyOn)
				{
					routeLogic.latency[routeLogic.labNum] += Time.deltaTime;
				}

				if(Input.GetMouseButtonDown(0))
				{
					if(selScript.SelectionFunc(Input.mousePosition))
					{
						Debug.Log("clear");
						carFlames.Stop();
						carFlames.Clear();
						carFlamesMove1.Play();
						carFlamesMove2.Play();
						carSelected = true;
						latencyOn = false;
						prevPos = transform.position;
					}
				}

			}
			else
			{
				//car movement
				if(Input.GetMouseButton(0) && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.x < Screen.width && Input.mousePosition.y < Screen.height)
				{
					curPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					transform.position = new Vector3(prevPos.x, prevPos.y, transform.position.z);
					curPos.z = transform.position.z;
					lookDir =  curPos - transform.position;
					if( lookDir != Vector3.zero)
					{
						Quaternion rotation = Quaternion.LookRotation(lookDir, transform.up);
						transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
					}

					prevPos = curPos;

				}
				else
				{
					carSelected = false;
				}
				if(Input.GetMouseButtonUp(0))
				{
					carSelected = false;
				}

			}

		}

	}
}
