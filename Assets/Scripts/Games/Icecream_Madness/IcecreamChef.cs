using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class IcecreamChef : MonoBehaviour {

    bool hasSomething;
    bool move;

    const string idleAnim = "Idle";
    const string idleHoldingAnim = "IdleConObjeto";
    const string walkAnim = "Caminar";
    const string walkHoldingAnim = "CaminarConObjeto";

    float minx = -4.84f;
    float maxx = 4.84f;
    float miny = -3f;
    float maxy = 2.47f;

    public UnityEngine.Transform trayPositioner;
    Table tableToGo;
    GameObject tray;

    UnityArmatureComponent armature;

    Vector3 positionToGo;

    float speed = 5f;

    IcecreamMadnessManager manager;

    // Use this for initialization
    void Start()
    {
        /*food = transform.GetChild(0).GetComponent<FoodToDeliver>();
        food.gameObject.SetActive(hasSomething);*/
        trayPositioner = transform.GetChild(0);
        armature = GetComponent<UnityArmatureComponent>();
        armature.animation.Play(idleAnim);
        manager = FindObjectOfType<IcecreamMadnessManager>();
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
        UpdateTrayTransformPosition();
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
        if (hasSomething)
        {
            armature.animation.Play(idleHoldingAnim);
        }
        else
        {
            armature.animation.Play(idleAnim);
        }

    }

    public bool IsHoldingSomething()
    {
        return hasSomething;
    }

    //Only Move the chef to somewhere
    public void MoveTheChef(Table table)
    {
        if (manager.IsGameOnAction())
        {
            if (hasSomething)
            {
                armature.animation.Play(walkHoldingAnim);
            }
            else
            {
                armature.animation.Play(walkAnim);
            }

            tableToGo = table;
            var posToGo = table.transform.position;
            if (posToGo.x < transform.position.x)
            {
                transform.localScale = new Vector3(-0.8f, 0.8f, 0.8f);
            }
            else
            {
                transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }
            positionToGo = posToGo;
            positionToGo = new Vector3(Mathf.Clamp(positionToGo.x, minx, maxx), Mathf.Clamp(positionToGo.y, miny, maxx), transform.position.z);
            move = true;
        }
    }

    //This is used to leave a tray and free hands
    /// <summary>
    /// This frees the chef of holding the tray an put it into a table as a GameObject
    /// </summary>
    /// <param name="tablePosition"></param>
    public void PutATray(UnityEngine.Transform tablePosition)
    {
        GameObject trayToLeft = transform.GetChild(0).GetChild(0).gameObject;
        trayToLeft.transform.position = tablePosition.position;
        trayToLeft.transform.parent = tablePosition;
        trayToLeft.GetComponent<Tray>().SetLayers(tablePosition.parent.GetComponent<SpriteRenderer>().sortingOrder);
        tablePosition.GetComponentInParent<Table>().SetTray(trayToLeft);
        hasSomething = false;

        tray = null;
    }

    public void GrabATray(GameObject trayGrabbed)
    {
        hasSomething = true;
        tray = trayGrabbed;
        trayGrabbed.transform.parent = trayPositioner;
        trayGrabbed.transform.position = trayPositioner.position;

        UpdateTrayTransformPosition();

        trayGrabbed.GetComponent<Tray>().SetLayers(armature.sortingOrder);
        trayGrabbed.GetComponent<Tray>().SetImage();
    }

    public Tray GetHoldingTray()
    {
        return tray.GetComponent<Tray>();
    }

    void UpdateTrayTransformPosition()
    {
        if (tray != null)
        {
            Vector3 posOfThings = new Vector3(0, armature.armature.GetBone("Mano_Der").offset.y + armature.armature.GetBone("Mano_Der").animationPose.y, transform.position.z);
            tray.transform.localPosition = posOfThings;
        }
    }
}
