using UnityEngine;
using System.Collections;

public class Branch : MonoBehaviour
{
	
	Vector3 originalPosition;
	bool offScreen;
	public bool moving;
	SoundTree mainTree;
	public bool nests;
	// Use this for initialization
	void Start ()
	{
		mainTree = GameObject.Find ("SoundTreeMain").GetComponent<SoundTree> ();
		moving = false;
		offScreen = false;
		originalPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (moving) 
		{
			Vector3 tempPosition=transform.position;
			if (!offScreen) 
			{
				transform.position=new Vector3(tempPosition.x,tempPosition.y -= 2 * Time.deltaTime,tempPosition.z);
				if (transform.position.y < -1.8) 
				{
					transform.position=new Vector3(tempPosition.x,4f,tempPosition.z);
					offScreen = true;
					if(nests)
						mainTree.SetNests();
				}
			}
			else
			{
				transform.position=new Vector3(tempPosition.x,tempPosition.y -= 2 * Time.deltaTime,tempPosition.z);
				if(Vector3.Distance(transform.position,originalPosition)<0.01f||transform.position.y<originalPosition.y)
				{
					moving=false;
					mainTree.branchesInPlace=true;
					offScreen=false;
					transform.position=originalPosition;
				}
			}
		}
	}

	public void Move ()
	{
		moving = true;
	}
}
