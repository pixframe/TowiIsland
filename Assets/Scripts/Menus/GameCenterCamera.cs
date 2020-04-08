using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class GameCenterCamera : MonoBehaviour {

    GameMenusManager manager;
    Animator anim;
    SessionManager sessionManager;

	// Use this for initialization
	public void StartTheThingNow ()
    {
        manager = FindObjectOfType<GameMenusManager>();
        anim = GetComponent<Animator>();
        sessionManager = FindObjectOfType<SessionManager>();

        if (!sessionManager.activeKid.firstsGames.Contains(false) && !FindObjectOfType<DemoKey>() && PlayerPrefs.GetInt(Keys.firstAnim) == 0)
        {
            anim.enabled = true;
            PlayerPrefs.SetInt(Keys.firstAnim, 1);
        }
        else
        {
            anim.enabled = false;
            ShowThePanels();
        }
	}

    public void ShowThePanels()
    {
        manager.FinishAnim();
    }
}
