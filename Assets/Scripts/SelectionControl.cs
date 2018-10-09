using UnityEngine;
using System.Collections;

// Require these components when using this script
[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Rigidbody))]
public class SelectionControl : MonoBehaviour
{
	[System.NonSerialized]					
	public float lookWeight;					// the amount to transition when using head look
	float originalZ=0;
	public float animSpeed = 1.5f;				// a public setting for overall animator animation speed
	public float lookSmoother = 3f;				// a smoothing setting for camera motion
	public bool useCurves;						// a setting for teaching purposes to show use of curves
	float speed=0;
	public bool getClose=false;
	public bool getFar=false;
	bool selected=false;
	SpotlightControl spotlight;
	SelectorController mainControl;
	private Animator anim;							// a reference to the animator on the character
	private AnimatorStateInfo currentBaseState;			// a reference to the current state of the animator, used for base layer
	private CapsuleCollider col;					// a reference to the capsule collider of the character

	void Start ()
	{
		mainControl = GameObject.Find ("Main").GetComponent<SelectorController> ();
		spotlight=GameObject.Find("Spotlight").GetComponent<SpotlightControl>();
		// initialising reference variables
		anim = GetComponent<Animator>();
		anim.ForceStateNormalizedTime(Random.Range(0.0f, 1.0f));
		col = GetComponent<CapsuleCollider>();
		originalZ = transform.position.z;
	}
	
	void OnMouseOver() {
		spotlight.target.x=transform.position.x;
		spotlight.target.z=transform.position.z;
		if(Input.GetMouseButtonDown(0)&&!selected&&!mainControl.target){
			mainControl.target=transform;
			getClose=true;
			getFar=false;
			selected=true;
			spotlight.turnOff=true;
		}
		//Debug.Log("Sobre Objeto");
	}
	public void resetSpotlight(){
		selected = false;
		getFar = true;
		spotlight.turnOn=true;
	}

	void Update(){
		if (getFar) {
			if(transform.position.z<originalZ){
				speed=-1;
			}else{
				getFar=false;
				speed=0;
			}
		}if (getClose) {
			if (transform.position.z > -6) {
				speed = 1;
			} else {
				getClose = false;
				speed = 0;
			}
		}
	}
	void FixedUpdate ()
	{
		anim.SetFloat("Speed", speed);							// set our animator's float parameter 'Speed' equal to the vertical input axis				
		anim.speed = animSpeed;								// set the speed of our animator to the public variable 'animSpeed'
		anim.SetLookAtWeight(lookWeight);					// set the Look At Weight - amount to use look at IK vs using the head's animation
		currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	// set our currentState variable to the current state of the Base Layer (0) of animation
	}
}

