using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour
{
	
	Vector3 iniPos;
	public float timer = 0f;
	WhereIsTheBallLogic wScritp;
	
	// Use this for initialization
	void Start ()
	{
		wScritp = GameObject.Find("Main").transform.GetComponent<WhereIsTheBallLogic>();
		
		iniPos = transform.position;
		Application.targetFrameRate = 60;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		timer += Time.deltaTime;
		if(Vector3.Distance(transform.position, iniPos ) >= 10)
		{
//			Debug.Log(Time.time);
		}
		else
		{
			transform.Translate(Vector3.forward  * wScritp.speed1 * Time.deltaTime);	
		}
	}
	void FixedUpdate()
	{
		
	}
}
