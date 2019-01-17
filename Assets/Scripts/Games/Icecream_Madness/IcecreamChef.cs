using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcecreamChef : MonoBehaviour {

    bool hasSomething;
    bool move;

    float minx = -4.84f;
    float maxx = 4.84f;
    float miny = -3f;
    float maxy = 2.47f;

    public Transform trayPositioner;
    Table tableToGo;
    GameObject tray;

    Vector3 positionToGo;

    float speed = 5f;

    // Use this for initialization
    void Start()
    {
        /*food = transform.GetChild(0).GetComponent<FoodToDeliver>();
        food.gameObject.SetActive(hasSomething);*/
        trayPositioner = transform.GetChild(0);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (move)
        {
            Move();
        }
	}

    void Move()
    {
        var dir = (positionToGo - transform.position).normalized * Time.deltaTime * speed;
        transform.Translate(dir);
        if (Vector3.Distance(positionToGo, transform.position) < 0.2f)
        {
            ItsInPosition();
        }
    }

    void ItsInPosition()
    {
        move = false;
        tableToGo.DoTheAction();
        transform.position = positionToGo;
    }

    public bool IsHoldingSomething()
    {
        return hasSomething;
    }

    //Only Move the chef to somewhere
    public void MoveTheChef(Table table)
    {
        Debug.Log("Should Move");
        tableToGo = table;
        var posToGo = table.transform.position;
        positionToGo = posToGo;
        positionToGo = new Vector3(Mathf.Clamp(positionToGo.x, minx, maxx), Mathf.Clamp(positionToGo.y, miny, maxx), transform.position.z);
        move = true;
    }

    //This is used to leave a tray and free hands
    public void PutATray(Transform tablePosition)
    {
        GameObject trayToLeft = transform.GetChild(0).GetChild(0).gameObject;
        trayToLeft.transform.position = tablePosition.position;
        trayToLeft.transform.parent = tablePosition;
        trayToLeft.GetComponent<Tray>().SetLayers(tablePosition.parent.GetComponent<SpriteRenderer>().sortingOrder);
        tablePosition.GetComponentInParent<Table>().SetTray(trayToLeft);
        hasSomething = false;
    }

    public void GrabATray(GameObject trayGrabbed)
    {
        hasSomething = true;
        tray = trayGrabbed;
        trayGrabbed.transform.parent = trayPositioner;
        trayGrabbed.transform.position = trayPositioner.position;
        trayGrabbed.GetComponent<Tray>().SetLayers(GetComponent<SpriteRenderer>().sortingOrder);
    }

    public Tray GetHoldingTray()
    {
        return tray.GetComponent<Tray>();
    }
}
