using UnityEngine;
using System.Collections;

public class TestInteraction : MonoBehaviour {
	
	TestLogic main;
	Treasure target;
	public GameObject glow;
	Vector2 lastPosition=Vector2.zero;
	float traveledDistance=0;
	float totalTime=0;
	float totalDistance=0;
    public Texture pearlTexture;

    // Use this for initialization
    void Start ()
	{
		main = (TestLogic)GameObject.Find ("Main").GetComponent (typeof(TestLogic));
	}
	
	// Update is called once per frame
	void Update ()
	{
        switch(main.state)
        {
            case TestLogic.TestStates.Test:
                totalTime += Time.deltaTime;
                if (lastPosition != Vector2.zero)
                {
                    float tempD = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), lastPosition);
                    traveledDistance += tempD;
                    totalDistance += tempD;
                    //Debug.Log(elapsedTime);
                }
                lastPosition = new Vector2(transform.position.x, transform.position.z);
                if (Input.GetKeyDown("space") && target != null)
                {
                    Grab();
                    //glow.transform.position=new Vector3(-999,-999,-999);
                }
                break;
        }	
	}

    void Grab()
    {
        if (target.currentState == Treasure.state.Occupied)
        {
            main.found++;
            main.scoreEffects.DisplayNewObject(true,1, true, pearlTexture);
        }
        else
        {
            main.scoreEffects.DisplayNewObject(false, "Esta concha está vacía.", true, null);
        }
        main.results.Add(new TestLogic.RouteResult(target.order, target.currentState, Mathf.Round(totalTime * 100) / 100, Mathf.Round(traveledDistance * 100) / 100));
        traveledDistance = 0;
        lastPosition = Vector2.zero;
        target.SwitchState();
        main.PrintResults();
    }
	
	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Treasure") {
			target = other.gameObject.GetComponent<Treasure>();
			glow.transform.position=target.transform.position;
		} else if (other.tag == "NPC") {

		}
		
	}
	
	void OnTriggerExit (Collider other)
	{
		if (other.tag == "Treasure") {
			target=null;
			glow.transform.position=new Vector3(-900,-900-900);
		}
	}

    public void Reset()
    {
        totalTime = 0;
        totalDistance = 0;
        traveledDistance = 0;
        lastPosition = Vector2.zero;
    }
}
