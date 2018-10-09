using UnityEngine;
using System.Collections;

public class ItemsNav : MonoBehaviour {
	public bool forward=true;
	bool hover=false;
	MenuControl menuRef;
	// Use this for initialization
	void Start () {
		GameObject temp = GameObject.Find ("Menu");
		menuRef = temp.GetComponent<MenuControl> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0))
		{
			if(hover)
			{
				if(!menuRef.itemsTransition)
				{
					hover=false;
					if(forward)
						SendMessageUpwards("NextItems");
					else
						SendMessageUpwards("PrevItems");
				}
			}
		}
	}

	void OnMouseOver()
	{
		if(!menuRef.itemsTransition)
			hover = true;
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
