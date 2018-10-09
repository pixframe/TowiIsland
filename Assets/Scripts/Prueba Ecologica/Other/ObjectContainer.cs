using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectContainer : MonoBehaviour
{
	GameObject[] con;
	public List<GameObject> container1 = new List<GameObject>();
	public List<GameObject> container2 = new List<GameObject>();


	// Use this for initialization
	void Start ()
	{

		FillContainer(1);
		FillContainer(2);

	}

	public void FillContainer(int cont)
	{
		if(cont == 1)
		{
			con = GameObject.FindGameObjectsWithTag("PackingObject1");
			foreach(GameObject c in con)
			{
				container1.Add(c);
			}
			GameObject[] conW = GameObject.FindGameObjectsWithTag("WeatherObject");
			foreach(GameObject c in conW)
			{
				container1.Add(c);
			}
		}
		else if(cont == 2)
		{
			con = GameObject.FindGameObjectsWithTag("PackingObject2");
			foreach(GameObject c in con)
			{
				container2.Add(c);
			}
		}
		else 
		{
			Debug.LogError("The value used in the function doesn't have a container");
		}


	}

}
