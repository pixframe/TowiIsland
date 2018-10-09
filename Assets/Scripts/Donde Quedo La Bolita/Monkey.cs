using UnityEngine;
using System.Collections;

//este script se encuantra en cada uno de los changos y se usa para agarrar el objeto y emparentarlo al chango, tambien se usa para desenparentar el objeto
public class Monkey : MonoBehaviour
{
	public GameObject objectGrabbed;
	public bool withObject = false;
	public int moveNum = 0;
	
	public void GrabObject()//enparenta el objeto al chango que lo contiene 
	{
		if(objectGrabbed != null)
		{
			objectGrabbed.transform.position = this.transform.position;
			objectGrabbed.transform.parent = this.gameObject.transform;
			withObject = true;
		}
		
	}
	
	public void CleanObject()//quita el objeto del chango
	{
		
		if(objectGrabbed != null)
		{
			objectGrabbed = null;
			withObject = false;
		}
	}
}
