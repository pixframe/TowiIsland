using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotController : MonoBehaviour {

    /*SandDrawingController manager;

    bool inside = false;
    bool putItFirst = false;
	// Use this for initialization
	void Start () {
        manager = FindObjectOfType<SandDrawingController>();
        Invoke("CalificateTheDot", 0.1f);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D target)
    {
        if (target.tag == "SpawnPoint")
        {
            Debug.Log("this is dot is out");
            if (target.GetComponent<DotController>().firstSet()) {
                Destroy(this.gameObject);
            }
        }
        if (target.tag == "Wall")
        {
            inside = true;
            GetComponent<Collider2D>().isTrigger = true;
        }
    }

    void CalificateTheDot() {
        putItFirst = true;
        if (inside)
        {
            manager.AddAnInsideDot();
        }
        else {
            manager.AddNotInsideDot();
        }
    }

    public bool firstSet() {
        return putItFirst;
    } */
}
