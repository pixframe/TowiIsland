using UnityEngine;
using System.Collections;

public class CameraHandler : MonoBehaviour {
	public bool enableUI=false;
	SoundManager soundMng;
	// Use this for initialization
	void Start ()
	{
		soundMng = GetComponent<SoundManager> ();
		if (PlayerPrefs.GetInt ("PlayIntro") == 1) 
		//if (true) 
		{
			PlayerPrefs.SetInt("PlayIntro",0);
			soundMng.PlaySound(0);
		} else 
		{
			GetComponent<Animator>().enabled=false;
			enableUI=true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!GetComponent<AudioSource>().isPlaying)
		{
			GetComponent<AudioSource>().loop=true;
			soundMng.PlaySound(1);
		}
	}

	public void showUI(){
		Interface temp = GameObject.Find ("Interface").GetComponent<Interface> ();
		if(PlayerPrefs.GetInt("Map",0)==1)
		{
			temp.fadeIn = true;
			temp.interfaceState=Interface.menu.Map;
		}else{
			temp.interfaceState = Interface.menu.Home;
			temp.fadeIn = true;
		}
		temp.hideUI = false;
	}
}
