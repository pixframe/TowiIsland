using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniIslandController : MonoBehaviour {

    [System.NonSerialized]
    //This list will store all the buys the childs made
    public List<bool> buyedObjects = new List<bool>(); 

    SessionManager sessionManager;

	// Use this for initialization
	void Awake ()
    {
        sessionManager = FindObjectOfType<SessionManager>();

        for (int i = 0; i < transform.childCount - 1; i++)
        {
            if (sessionManager.activeKid.buyedIslandObjects.Contains(i))
            {
                buyedObjects.Add(true);
            }
            else
            {
                buyedObjects.Add(false);
            }

        }

        for (int i = 0; i < transform.childCount - 1; i++)
        {
            transform.GetChild(i + 1).gameObject.SetActive(buyedObjects[i]);
        }

	}
}
