using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdsController : MonoBehaviour {

    Animator anim;
    BirdsSingingManager manager;
    ParticleSystem singingParticles;
    GameObject singingNote;
    AudioManager audioManager;


    Vector3 initialPosition;
    Vector3 goalPosition;
    Vector3 standingPosition;
    Vector3 mousePos;
    bool inPosition = true;
    bool firstSing = true;
    bool flightBack = false;
    bool isWellPlaced = false;

    float speedToGo = 0.03f;
    int numberOfSong;

    // Use this for initialization
    void Start () {
        audioManager = FindObjectOfType<AudioManager>();
        singingParticles = transform.Find("Note Particle System").GetComponent<ParticleSystem>();
        singingNote = transform.Find("Note").gameObject;
        anim = GetComponent<Animator>();
        manager = FindObjectOfType<BirdsSingingManager>();
        singingNote.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (!inPosition) {
            GoToTheNewPosition();
        }
        if (flightBack)
        {
            FlightToStandingPosition();
        }
	}

    //this will move a bird if its not in te correct position
    void GoToTheNewPosition() {
        transform.position = Vector3.Lerp(transform.position, goalPosition, speedToGo);
        if (Vector3.Distance(transform.position, goalPosition) < 0.1f) {
            transform.position = goalPosition;
            inPosition = true;
            standingPosition = transform.position;
            singingNote.SetActive(true);
        }
    }

    //this will set a new direction for the bird
    public void SetANewDirection(Vector3 positionToGo) {
        inPosition = false;
        goalPosition = positionToGo;
    }

    public void SetNewBranch(Vector3 newPosition)
    {
        goalPosition = newPosition;
    }

    public void GoToNewSpot()
    {
        inPosition = false;
    }

    public void TeleportTheBird()
    {
        firstSing = true;
        Vector3 newPos = initialPosition;
        initialPosition.y -= 5;
        transform.position = initialPosition;
        isWellPlaced = false;
    }

    //This one will save the song number of the bird inside of him
    public void LearnASongNumber(int songNumber)
    {
        numberOfSong = songNumber;
    }

    public int SingASongNumber() {
        return numberOfSong;
    }

    public void BirdMissTheNest()
    {
        flightBack = true; 
    }

    public void BirdIsWellSet()
    {
        isWellPlaced = true;
    }

    void SingNow()
    {
        manager.BirdSing(numberOfSong);
        manager.PairOrNot();
        singingParticles.gameObject.SetActive(true);
        singingParticles.Play();
        singingNote.SetActive(false);
        Invoke("StopTheSing", audioManager.ClipDuration());
        firstSing = false;
    }

    void StopTheSing()
    {
        singingParticles.Stop();
        singingParticles.gameObject.SetActive(false);
    }

    void FlightToStandingPosition()
    {
        transform.position = Vector3.Lerp(transform.position, standingPosition, speedToGo);
        if (Vector3.Distance(transform.position, standingPosition) < 0.1f)
        {
            transform.position = goalPosition;
            flightBack = false;
        }
    }

    public void TeleportToStandingPos()
    {
        transform.position = goalPosition;
    }

    //This will tell which song the bird will be play

    void OnMouseDown()
    {
        if (manager.phase == BirdsSingingManager.GamePhase.Game && !isWellPlaced)
        {
            if (!firstSing)
            {
                manager.SelectTheBird(gameObject);
                flightBack = false;
                mousePos = Input.mousePosition;
            }
        }
    }

    void OnMouseUp()
    {
        if (manager.phase == BirdsSingingManager.GamePhase.Game && !isWellPlaced)
        {
            if (firstSing)
            {
                SingNow();
            }
            else
            {
                if (Vector3.Distance(mousePos, Input.mousePosition) < 0.1f)
                {
                    SingNow();
                }
            }
        }
    }
}
