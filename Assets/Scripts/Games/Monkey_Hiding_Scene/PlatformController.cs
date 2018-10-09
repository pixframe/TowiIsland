using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour {

    float yPosition;
    float moveSpeed;

    bool moving;

    int way;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (moving) {
            if (way == 0)
            {
                PlatformGoingUp();
                PlatformSetUp();
            }
            else
            {
                PlatformGoingDown();
                PlatformSetDown();
            }
        }
	}

    void PlatformGoingUp()
    {
        if (transform.position.y <= yPosition)
        {
            transform.Translate(Vector3.up * Time.deltaTime * moveSpeed);
        }
    }

    void PlatformSetUp() {
        if (transform.position.y > yPosition)
        {
            Vector3 newPos = transform.position;
            newPos.y = yPosition;
            transform.position = newPos;
            moving = false;
        }
    }

    void PlatformGoingDown() {
        if (transform.position.y >= yPosition)
        {
            transform.Translate(Vector3.down * Time.deltaTime * moveSpeed);
        }
    }

    void PlatformSetDown() {
        if (transform.position.y < yPosition)
        {
            Vector3 newPos = transform.position;
            newPos.y = yPosition;
            transform.position = newPos;
            moving = false;
        }
    }

    public void MoveThePlatform(int direction, float position) {
        moving = true;
        way = direction;
        yPosition = position;
    }
}
