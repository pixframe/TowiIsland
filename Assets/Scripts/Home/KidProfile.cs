using UnityEngine;
using System.Collections;

public class KidProfile : MonoBehaviour {

	Login mainRef;
	public int id;
    public string parentkey;
	public Sprite hover;
	bool isHovering=false;
	Sprite original;
	SpriteRenderer sr;

	// Use this for initialization
	void Awake () {
		mainRef=GameObject.Find ("Interface").GetComponent<Login> ();
		sr = GetComponent<SpriteRenderer> ();
		original = sr.sprite;
	}
	
	// Update is called once per frame
	void Update () {
		if(!mainRef.fadeIn&&!mainRef.changeProfiles&&isHovering&&mainRef.currentState==Login.Phase.Profiles){
			if(Input.GetMouseButtonDown(0)){
                mainRef.SetKid(parentkey, id);
			}
		}
	}

	void OnMouseEnter() {
		//sr.sprite = hover;
	}
	void OnMouseExit() {
		//sr.sprite = original;
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

	void OnMouseOver() {

	}
}
