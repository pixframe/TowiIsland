using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ocean : MonoBehaviour {

    Animator anim;
    SandDrawingController controller;

	// Use this for initialization
	void Start ()
    {
        anim = GetComponent<Animator>();
        controller = FindObjectOfType<SandDrawingController>();
	}

    public void StopTheAnim()
    {
        anim.enabled = false;
        controller.AfterTheClean();
    }

    public void EraseTheSand()
    {
        controller.CleanTheSand();
    }
}
