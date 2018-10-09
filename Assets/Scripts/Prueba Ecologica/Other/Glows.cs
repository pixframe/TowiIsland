using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Glows : MonoBehaviour
{
	ObjectContainer objContScript;
	public GameObject[] glows;
	public GameObject[] glowCont;
	bool glow = false;
	public int curIndex = 0;
	float timerGlow;
	GameObject normObj;
	GameObject glowObj;
	public float glowChange = .5f;
	float timerProv;

	// Use this for initialization
	void Start ()
	{
		objContScript = GameObject.Find("Pack").transform.Find("PackingObjects").GetComponent<ObjectContainer>();
		curIndex = 0;

	}
	
	// Update is called once per frame
	void Update ()
	{

		if(glow)
		{
			timerProv -= Time.deltaTime;
			if(timerGlow - timerProv >= glowChange)
			{
				if(!glowObj.activeSelf)
				{
					glowObj.SetActive(true);
					normObj.SetActive(false);
				}
				else
				{
					glowObj.SetActive(false);
					normObj.SetActive(true);
				}
				timerGlow = timerProv;
			}

			if(timerProv <= 0)
			{
				glow = false;
				glowObj.SetActive(false);
				normObj.SetActive(true);
				curIndex++;
			}
		}
	}
	public bool Glow(float time, List<GameObject> list)
	{

		if(!glow)
		{
			if(curIndex >= glows.Length)
			{
				curIndex = 0;
				glow = false;
				return true;
			}
			else 
			{
				timerGlow = time;
				timerProv = timerGlow;
				glow = true;
				glowObj = glows[curIndex].gameObject;
				foreach(GameObject o in list)
				{
					if(o.name == glowObj.name)
					{
						normObj = o;
						break;
					}
				}
				return false;
			}

		}
		return false;


	}
}
