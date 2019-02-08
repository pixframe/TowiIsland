using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {

    #region Variables

    #region Variables used in the game

    #region Scripts
    //This script is the manager of the game so will perform all the collect data and perform things
    CarGame carManager;
    #endregion

    #region Componets for the car
    //here will have all the components that the cra need in its animations
    //this one is the sprite that is show every time the car hits something
    GameObject hitSprite;
    //this particle system is used for the waiting paeriond
    ParticleSystem carFlames;
    //these particular particles sistems are the one that makes look the car is having a flames
    ParticleSystem flames0;
    ParticleSystem flames1;

    ParticleSystem indicativeHalo;
    //this is the collider that interact with the enviroment
    Collider2D wallHited;
    //this one will make a trail that follow the movement of the car
    TrailRenderer trail;

    SpriteRenderer rendi;

    Color firstColor;
    #endregion

    #region Positions
    //these will be the positions used in the car to make it playable
    //this will be update and its the current position the car is heading to
    Vector3 currentPosition;
    //this one is the previos poistion the car was
    Vector3 previousPosition;
    //this is the position of the car whn firts collide with something is used in determine more complex data than hits
    Vector3 enterPosition;
    //this is the position of the car after finisihng the collision is used in determine more complex data than hits
    Vector3 exitPosition;
    #endregion

    #region LayerMask
    //This layer will help to determine which is the layer of the car
    public LayerMask carLayer;
    #endregion

    #region Booleans
    //this will say if a car is or not selected
    bool carSelected;
    //this will say if the car has been move for the first time
    bool firstMove;
    //this will make a sprite bigger
    bool spriteTime;
    //this will see if the car is moving when using car arrows
    bool isMovingWithArrows;
    #endregion

    #region Numbers

    float diminuter = 1f;
    //this is the speed the car oves with arrows;
    float speed = 2f;
    #endregion

    enum DirectionToGo { Up, Down, Right, Left};
    DirectionToGo direction;

    #endregion

    #region Variables used in data record

    #region Numbers
    //this one will count every time the car hits
    [System.NonSerialized]
    public int hits;
    //this one will count every time the car crosses
    [System.NonSerialized]
    public int crosses;
    //this one will count every Time the car change from one to another
    [System.NonSerialized]
    public int changesOfRoutes;
    //this will count to how many dead ends the player have arrived
    [System.NonSerialized]
    public int deadEnds;
    #endregion

    #endregion

    #endregion

    // Use this for initialization
    void Start()
    {
        carSelected = false;
        previousPosition = transform.position;
        hitSprite = transform.GetChild(0).gameObject;
        rendi = hitSprite.GetComponent<SpriteRenderer>();
        carManager = FindObjectOfType<CarGame>();
        carFlames = gameObject.transform.GetChild(3).gameObject.GetComponent<ParticleSystem>();
        flames0 = gameObject.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>();
        flames1 = gameObject.transform.GetChild(2).gameObject.GetComponent<ParticleSystem>();
        indicativeHalo = gameObject.transform.GetChild(4).gameObject.GetComponent<ParticleSystem>();
        indicativeHalo.Play();
        carFlames.Stop();
        flames0.Stop();
        flames1.Stop();
        trail = GetComponent<TrailRenderer>();
        trail.time = 6000;
        trail.sortingLayerName = "Kiwi";
        firstColor = rendi.color;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, -1.5f, 11f), Mathf.Clamp(transform.localPosition.y, -0.4f, 9.2f), transform.localPosition.z);

        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            if (Input.GetMouseButton(0))
            {
                if (carSelected)
                {
                    CarMovement();
                }
                else
                {
                    IsCarSelected();
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (carSelected)
                {
                    carSelected = false;
                    Cursor.visible = true;
                    carFlames.Play();
                    flames0.Stop();
                    flames1.Stop();
                }
            }
            else
            {
                if (carSelected)
                {
                    carSelected = false;
                    Cursor.visible = true;
                }
            }
        }
        else if(SystemInfo.deviceType == DeviceType.Desktop)
        {
            CarMovementPC();
        }

        if (spriteTime)
        {
            hitSprite.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f) * Time.deltaTime;
            Color colorToPut = rendi.color;
            colorToPut.a -= diminuter * Time.deltaTime;
            rendi.color = colorToPut;

            if (rendi.color.a <= 0)
            {
                ResetTheSprite();
                spriteTime = false;
            }
        }
    }

    //This will check the first interaction with the thngs
    void OnTriggerEnter2D(Collider2D target)
    {
        if (target.tag == "Wall")
        {
            ResetTheSprite();
            hitSprite.SetActive(true);
            enterPosition = transform.position;
            spriteTime = true;
        }
        //this one will check the arrive to the finish line
        else if (target.tag == "Finish")
        {
            carManager.FinishLaberynth();
        }
    }

    //This will check the exit interaction with the things
    void OnTriggerExit2D(Collider2D target)
    {
        //This one check the collision with the walls
        if (target.tag == "Wall")
        {
            hits++;
            exitPosition = transform.position;
            wallHited = target;
            DoesTheCarCross();
        }
        if (target.tag == "Deadends")
        {
            DeadEndController deadend = target.GetComponent<DeadEndController>();
            deadend.HitTheColi();
            deadEnds++;
            Debug.Log("dead ends hits " + deadEnds);
        }
        if (target.tag == "DesActivater")
        {
            DeadEndController deadend = target.transform.parent.GetComponent<DeadEndController>();
            deadend.DesactivateTheColi();
        }
        if (target.tag == "Activater")
        {
            DeadEndController deadend = target.transform.parent.GetComponent<DeadEndController>();
            deadend.ActivateTheColi();
        }
    }

    #region Functions

    //this will check if a car is selected in case is not it will try to select one
    void IsCarSelected() {
        if (SelectedCar() != null)
        {
            carSelected = true;
            Cursor.visible = false;
            flames0.Play();
            flames1.Play();
            indicativeHalo.Stop();
            carFlames.Stop();
        }
        else {
            carSelected = false;
        }

    }

    //this one wil return a selected car to me able to move
    GameObject SelectedCar() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit;
        hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, carLayer);
        if (hit)
        {
            carManager.StartSolving();
            return hit.transform.gameObject;
        }
        else {
            return null;
        }

    }

    //this script allows the movement;
    void CarMovement() {
        currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentPosition.z = transform.position.z;
        transform.position = previousPosition;
        Vector3 dir = currentPosition - transform.position;
        if (dir != Vector3.zero) {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            angle -= 90;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), 10 * Time.deltaTime);
        }
        if (!firstMove && currentPosition != previousPosition)
        {
            carManager.StartSolving();
            firstMove = true;
        }
        previousPosition = currentPosition;
    }

    void CarMovementPC()
    {
        if (!isMovingWithArrows)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
                direction = DirectionToGo.Up;
                MoveNow();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                transform.eulerAngles = new Vector3(0f, 0f, 180f);
                direction = DirectionToGo.Down;
                MoveNow();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                transform.eulerAngles = new Vector3(0f, 0f, 90f);
                direction = DirectionToGo.Left;
                MoveNow();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                transform.eulerAngles = new Vector3(0f, 0f, 270f);
                direction = DirectionToGo.Right;
                MoveNow();
            }
        }

        if (isMovingWithArrows)
        {
            Move();
        }

        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            StopMove();
        }
    }

    void MoveNow()
    {
        isMovingWithArrows = true;
        if (!firstMove)
        {
            carManager.StartSolving();
            firstMove = true;
        }
        carSelected = true;
        Cursor.visible = false;
        flames0.Play();
        flames1.Play();
        indicativeHalo.Stop();
        carFlames.Stop();
    }

    void Move()
    {
        transform.position += transform.up * speed * Time.deltaTime;
    }

    void StopMove()
    {
        isMovingWithArrows = false;
        carSelected = false;
        Cursor.visible = true;
        carFlames.Play();
        flames0.Stop();
        flames1.Stop();
    }

    //This checks if the car cross the colider and if it does augment the couont for the crosses
    void DoesTheCarCross() {

        //we determine that the samll bound will be the one that we are going to consider because if
        //a car crosses by the smallest part and it returns true then the result should be that the player cross the collider

        if (wallHited.bounds.extents.x < wallHited.bounds.extents.y)
        {
            if (Vector3.Distance(new Vector3(enterPosition.x, 0, 0), new Vector3(exitPosition.x, 0, 0)) >= (wallHited.bounds.extents.x * 2))
            {
                crosses++;
            }
        }
        else {
            if (Vector3.Distance(new Vector3(0, enterPosition.y, 0), new Vector3(0, exitPosition.y, 0)) >= (wallHited.bounds.extents.y * 2))
            {
                crosses++;
            }
        }
    }

    void ResetTheSprite()
    {
        hitSprite.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        rendi.color = firstColor;
        hitSprite.SetActive(false);
    }
    #endregion
}
