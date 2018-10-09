using UnityEngine;
using System.Collections;

public class KillBox : MonoBehaviour {
	// Update is called once per frame
	void Update () {
		if(transform.position.y<-10)
		{
			int option = Random.Range(0,2);
			if(option==0)
			{
				transform.position=new Vector3(-10,2.5f,Random.Range(0f,20f));
				transform.rotation=Quaternion.Euler(new Vector3(0,130,0));
			}else
			{
				transform.position=new Vector3(10,2.5f,Random.Range(0f,20f));
				transform.rotation=Quaternion.Euler(new Vector3(0,230,0));
			}
		}
	}
}
