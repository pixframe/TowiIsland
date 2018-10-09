using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
	public enum menuAction {Play,Options,About,Exit,Upgrade, Pruebas};
	public menuAction action;
	public Transform mainRef;
	Login mainScript;
	public Sprite hover;
    bool isHovering = false;
	Sprite original;
	SpriteRenderer sr;

	public bool isPrueba = false;
	public bool isPlay = false;

	// Use this for initialization
	void Awake () {
		mainScript = mainRef.GetComponent<Login> ();
		sr = GetComponent<SpriteRenderer> ();
		original = sr.sprite;
	}
	
	// Update is called once per frame
	void Update () {
		if(!mainScript.fadeIn&&!mainScript.changeProfiles&&isHovering&&mainScript.currentState==Login.Phase.Menu){
			if(Input.GetMouseButtonDown(0)){
				doAction();
				SetNormal();
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

	void doAction()
	{
		switch (action) 
		{
			case menuAction.Play:
			isPlay = true;
				if(Login.local)
				{
					mainScript.sessionMng.LoadLocal();
					mainScript.currentState=Login.Phase.Loading;
					mainScript.fadeIn=true;
				}else
				{
					if(mainScript.sessionMng.activeUser==null)
						mainScript.DisplayLogin();
					else
						mainScript.DisplayProfiles();
				}
			Debug.Log ("is play  " + isPlay);
			break;
			case menuAction.Options:
				mainScript.DisplayOptions();
			break;
			case menuAction.About:
				mainScript.DisplayCredits();
			break;
			case menuAction.Exit:
				mainScript.CloseApplication();;
			break;

			case menuAction.Pruebas:

				isPrueba = true;
				if(Login.local)
				{
					mainScript.sessionMng.LoadLocal();
					mainScript.currentState=Login.Phase.Loading;
					mainScript.fadeIn=true;
				}else
				{
					if(mainScript.sessionMng.activeUser==null)
						mainScript.DisplayLogin();
					else
						mainScript.DisplayProfiles();
				}				
				Debug.Log ("is pureba  " + isPrueba);
			break;
		}
	}
}
