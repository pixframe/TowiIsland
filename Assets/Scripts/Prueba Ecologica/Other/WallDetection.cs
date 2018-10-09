using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallDetection : MonoBehaviour
{
	//scripts 
	AirportRouteLogic routeLogic;
	CarControl carCScript;
	PEMainLogic mainLogic;

	Vector3 enterPos;
	Vector3 exitPos;
	Collider colHitEnter;
	Collider colHitExit;
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
		routeLogic = transform.parent.GetComponent<AirportRouteLogic>();
		carCScript = GetComponent<CarControl>();
		mainLogic = GameObject.FindGameObjectWithTag("Main").GetComponent<PEMainLogic>();
	}

	// Update is called once per frame
	void Update ()
	{
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
    private Vector3 GetPointOfContact()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            return hit.point;
        }
        return transform.position;
    }
	void OnTriggerEnter(Collider col)
	{
		if(carCScript.carOn)
		{
			if(col.name == "Wall")
			{
                Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
                //pos.y = Screen.height - pos.y;
                feedbackList.Add(new WaitingLogic.MouseFeedback(pos, 50, feedbackScaleRate, feedbackTime, opacityRate));
				colHitEnter = col;
				enterPos = transform.position;
				
			}
			else if(col.name == "Exit")
			{
//				routeLogic.labNum++;
				if(routeLogic.labNum == 2)
				{
					mainLogic.curGameFinished = true;
					carCScript.carOn = false;
					routeLogic.state = "Default";
					carCScript.carSelected = false;
				}
				else
				{
					carCScript.carOn = false;
					routeLogic.state = "CamTrans";
				}

				
			}
			else if(col.name == "DeadEnd")
			{
				routeLogic.deadEnds[routeLogic.labNum]++;
			}
		}

	}
	void OnTriggerExit(Collider col)
	{
		if(carCScript.carOn)
		{
			if(col.name == "Wall")
			{
				colHitExit = col;
				exitPos = transform.position;
				
				TypeOfCol(enterPos, exitPos);

			}
		}

	}
	void TypeOfCol(Vector3 pos1, Vector3 pos2)
	{
		if(colHitExit.bounds.extents.x < colHitExit.bounds.extents.y)
		{
			if(Vector3.Distance(new Vector3(pos1.x, 0, 0), new Vector3(pos2.x, 0, 0)) > colHitEnter.bounds.extents.x * 2)
			{
				routeLogic.crosses[routeLogic.labNum]++;
				routeLogic.hits[routeLogic.labNum]++;
			}
			else
			{
				routeLogic.hits[routeLogic.labNum]++;
			}
		}
		else if(colHitExit.bounds.extents.y < colHitExit.bounds.extents.x)
		{
			if(Vector3.Distance(new Vector3(0, pos1.y, 0), new Vector3(0, pos2.y, 0)) > colHitEnter.bounds.extents.y * 2)
			{
				routeLogic.crosses[routeLogic.labNum]++;
				routeLogic.hits[routeLogic.labNum]++;
				
			}
			else
			{
				routeLogic.hits[routeLogic.labNum]++;
			}
		}

	}
}
