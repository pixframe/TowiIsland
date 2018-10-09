using UnityEngine;
using System.Collections;

public class ArrowController : MonoBehaviour {
	Login mainRef;
	public Sprite hover;
	Sprite original;
	SpriteRenderer sr;
	bool isHovering=false;

	public GameObject pruebaBut;
	public GameObject playBut;

	// Use this for initialization
	void Awake () 
	{
		mainRef=GameObject.Find ("Interface").GetComponent<Login> ();
		sr = GetComponent<SpriteRenderer> ();
		original = sr.sprite;
	}
	
	void Update () 
	{
		if(!mainRef.fadeIn&&!mainRef.changeProfiles&&isHovering&&(mainRef.currentState==Login.Phase.Profiles||mainRef.currentState==Login.Phase.About)){
			if(Input.GetMouseButtonDown(0))
			{
				/*pruebaBut.GetComponent<MenuController> ().isPrueba = false;
				playBut.GetComponent<MenuController> ().isPlay = false;*/

				Application.LoadLevel ("Login");

				//mainRef.ReturnToMenu();

			}
		}
	}
	
	public void SetHover()
	{
		isHovering = true;
		sr.sprite = hover;
	}
	
	public void SetNormal()
	{
		isHovering = false;
		sr.sprite = original;
	}

	public void Back()
	{
		mainRef.ReturnToMenu ();
	}
}
