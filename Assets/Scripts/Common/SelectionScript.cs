using UnityEngine;
using System.Collections;

public class SelectionScript : MonoBehaviour
{
	public RaycastHit selection;
	public LayerMask selectable;
	public GameObject objSelected;
	public Camera uiCam;
	void Start()
	{
		if(Application.loadedLevelName == "PruebaEcologica")
		{
//			uiCam = Camera.main.transform.FindChild("UICamera").camera;
		}

	}

	public bool SelectionFunc(Vector3 position)
	{
		Ray ray = Camera.main.ScreenPointToRay(position);
		if(Physics.Raycast(ray, out selection, Mathf.Infinity, selectable))
		{
			objSelected = selection.transform.gameObject;
			
			return true;
		}
		else
		{
			return false;
		}
		
	}
	public bool UISelectionFunc(Vector3 position)
	{
		Ray ray = uiCam.ScreenPointToRay(position);
		if(Physics.Raycast(ray, out selection, Mathf.Infinity, selectable))
		{
			objSelected = selection.transform.gameObject;
			
			return true;
		}
		else
		{
			return false;
		}

	}
}
