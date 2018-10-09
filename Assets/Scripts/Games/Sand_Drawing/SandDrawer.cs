using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandDrawer : MonoBehaviour {

    public GameObject dot;
    public GameObject dotCollector;
    bool drawing = false;

    // Use this for initialization
    void Start()
    {

    }
	
	// Update is called once per frame
	void Update () {
        if (drawing)
        {
            Instantiate(dot, transform.position, dot.transform.rotation, dotCollector.transform);
        }
	}


    public void StartDrawing() {
        drawing = true;
    }

    public void StopDrawing() {
        drawing = false;
    }
}
