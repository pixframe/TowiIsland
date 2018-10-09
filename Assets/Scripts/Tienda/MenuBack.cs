using UnityEngine;
using System.Collections;

public class MenuBack : MonoBehaviour {

	bool hover=false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0))
		{
			if(hover)
			{
				SendMessageUpwards("Back");
			}
		}
	}

	void OnMouseEnter()
	{
		hover = true;
	}

	void OnMouseExit()
	{
		hover = false;
	}
}
