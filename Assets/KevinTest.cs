using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KevinTest : MonoBehaviour {


	public GameObject cubeR;
	public GameObject cubeL;


	Touch myTouch ;

	
	// Update is called once per frame
	void Update () 
	{
		if (Input.touchCount > 0) 
		{
			myTouch = Input.GetTouch(0);
		}
			

		if (Input.GetMouseButtonDown (0) && Input.mousePosition.x < Screen.width / 2
		    || myTouch.position.x < Screen.width / 2) 
		{
			Debug.Log ("LadoIzquierdo");
			cubeL.SetActive (true);
			StartCoroutine (LOff ());
		}


		if (Input.GetMouseButtonDown (0) && Input.mousePosition.x > Screen.width / 2
		    || myTouch.position.x > Screen.width / 2) 
		{
			Debug.Log ("LadoDerecho");
			cubeR.SetActive (true);
			StartCoroutine (TOff ());
		}
			
	}

	IEnumerator LOff()
	{
		yield return new WaitForSeconds (1.0f);
		cubeL.SetActive (false);
	}

	IEnumerator TOff()
	{
		yield return new WaitForSeconds (1.0f);
		cubeR.SetActive (false);
	}
}
