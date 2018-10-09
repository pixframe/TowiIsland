using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdCamera : MonoBehaviour {

    bool MoveNow = false;

    enum Phases { up, teleport, up2};
    Phases camPhases = Phases.up;

    Vector3 pos1;
    Vector3 pos2;
    Vector3 pos3;
    Vector3 posGoal;

    BirdsSingingManager manager;

	// Use this for initialization
	void Start () {
        pos1 = transform.position;
        pos2 = pos1;
        pos2.y += 4f;
        pos3 = pos1;
        pos3.y -= 3f;
        posGoal = pos2;
        manager = FindObjectOfType<BirdsSingingManager>();
	}
	
	// Update is called once per frame
	void Update () {
        if (MoveNow)
        {
            if (camPhases == Phases.up)
            {
                transform.Translate(Vector3.up * Time.deltaTime * 3f);
                if (transform.position.y >= pos2.y)
                {
                    camPhases = Phases.teleport;
                    manager.GoToNewPos();
                }
            }
            else if (camPhases == Phases.teleport)
            {
                transform.position = pos3;
                camPhases = Phases.up2;
            }
            else if (camPhases == Phases.up2)
            {
                transform.Translate(Vector3.up * Time.deltaTime * 3f);
                if (transform.position.y >= pos1.y)
                {
                    MoveNow = false;
                    transform.position = pos1;
                    manager.SetNewGame();
                }
            }
        }
	}

    public void StartMoving()
    {
        camPhases = Phases.up;
        MoveNow = true;
    }
}
