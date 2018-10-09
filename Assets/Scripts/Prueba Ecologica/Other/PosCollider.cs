using UnityEngine;
using System.Collections;

public class PosCollider : MonoBehaviour
{
	UnPackLogic unPackScript;
	public GameObject correctObject;
	public bool add = false;
	float waitTimer;
	float waitTime = 1f;
	// Use this for initialization
	void Start ()
	{
		unPackScript = GameObject.Find("UnPack").GetComponent<UnPackLogic>();
		waitTimer = waitTime;
	}

	// Update is called once per frame
	void Update ()
	{
		if(!unPackScript.determine)
		{
			add = false;
		}
		else
		{
			waitTimer -= Time.deltaTime;
			if(waitTimer <= 0)
			{
				add = true;
				waitTimer = waitTime;
			}
		}
	}
//	void OnTriggerStay(Collider col)
//	{
//		if(unPackScript.determine && !add)
//		{
//			if(col.gameObject.name == correctObject.name)
//			{
//
//				unPackScript.spacialPres[unPackScript.essay]++;
//
//			}
//			add = true;
//		}
//
//	}
}
