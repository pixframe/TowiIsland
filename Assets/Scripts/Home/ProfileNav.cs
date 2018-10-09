using UnityEngine;
using System.Collections;

public class ProfileNav : MonoBehaviour {

	Login mainRef;
	public bool forward;
	Transform camRef;
	// Use this for initialization
	void Start () {
		camRef=GameObject.Find("Camera").transform;
		mainRef=GameObject.Find ("Interface").GetComponent<Login> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(!mainRef.fadeIn&&mainRef.currentState==Login.Phase.Profiles){
			RaycastHit hit;
			Ray collRay=camRef.GetComponent<Camera>().ScreenPointToRay(new Vector3(Input.mousePosition.x,Input.mousePosition.y,20));
			int insID=-1;
			if (Physics.Raycast(collRay, out hit))
			{
				if(hit.collider.transform==transform)
				{
					if(Input.GetMouseButtonDown(0)){
						if(forward)
							mainRef.NextProfiles();
						else
							mainRef.PrevProfiles();
					}
				}
			}
		}
	}

	void OnMouseEnter() {

	}
	void OnMouseExit() {

	}
	
	void OnMouseOver() {
		/*if(!mainRef.fadeIn){
			if(Input.GetMouseButtonDown(0)){
				if(forward)
					mainRef.NextProfiles();
				else
					mainRef.PrevProfiles();
			}
		}*/
	}
}
