using UnityEngine;
using System.Collections;

public class SwipeTest : MonoBehaviour {

    public Swipe swipeControls;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	     if (swipeControls.SwipeLeft)
            Debug.Log("Swipe Left MDFFF");
        if (swipeControls.SwipeRight)
            Debug.Log("Swipe Right MDFF");
        if (swipeControls.Tap)
            Debug.Log("Tap");
    }
}
