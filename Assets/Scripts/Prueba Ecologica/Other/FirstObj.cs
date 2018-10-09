using UnityEngine;
using System.Collections;

public class FirstObj : MonoBehaviour
{
	UnPackLogic unPackScript;
	PackLogic packScript;
	public int correct;
	public int incorrect;
	int objPlaced;
	// Use this for initialization
	void Start ()
	{
		unPackScript = GameObject.Find("UnPack").GetComponent<UnPackLogic>();
		packScript = GameObject.Find("Pack").GetComponent<PackLogic>();
		objPlaced = 0;
	}

	// Update is called once per frame
	void Update ()
	{

	}

    void OnTriggerExit(Collider col)
    {
        unPackScript.overcollider = false;
    }

	void OnTriggerStay (Collider col)
	{
        unPackScript.overcollider = true;
		if(unPackScript.state == "UnPackFirstObjects")
		{

			if(unPackScript.check)
			{
			
				if(col.tag == "UnPackObject")
				{

					objPlaced++;
					if(objPlaced <= 3)
					{
						unPackScript.objectSelected.transform.position = GameObject.Find("Pos" + (objPlaced)).transform.position;
					}
					unPackScript.unPObjs.Add(col.gameObject);
					unPackScript.unPackedFObjs.Add(col.name);
					foreach(string s in packScript.unPackObj)
					{
						if(col.gameObject.name == s)
						{
							correct++;
							unPackScript.correctFirstObj++;
						}
					}
				}
				unPackScript.check = false;
				if(objPlaced == 3)
				{
					foreach(GameObject o in unPackScript.unPObjs)
					{
						o.transform.parent = unPackScript.unPackUI.transform.Find("Objects").transform;
						o.GetComponent<UnPackObj>().IniPos();
					}
					unPackScript.unPObjs.Clear();
					incorrect = 3 - correct;
					unPackScript.state = "Intro";
					gameObject.SetActive(false);
				}
			}

		}
	}
}
