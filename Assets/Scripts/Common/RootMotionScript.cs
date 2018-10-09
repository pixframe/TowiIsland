using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class RootMotionScript : MonoBehaviour {

	float jumpWait=0.3f;
	bool doJump=false;
	bool jumped=false;
	private AnimatorStateInfo currentBaseState;
	static int locoState = Animator.StringToHash("Base Layer.Locomotion");
	static int backState = Animator.StringToHash("Base Layer.WalkBack");
	static int jumpState = Animator.StringToHash("Base Layer.Jump");
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnAnimatorMove()
	{
        currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
        Vector3 newPosition = transform.position;
        //newPosition.x += animator.GetFloat("Speed") *100* Time.deltaTime;
        //newPosition.z += animator.GetFloat("Direction") *100* Time.deltaTime;                                 
        if (currentBaseState.fullPathHash == locoState || currentBaseState.fullPathHash == jumpState)
        {
            transform.Translate(Vector3.forward * anim.GetFloat("Speed") * 7 * Time.deltaTime);
            transform.Rotate(Vector3.up * Time.deltaTime * 100 * anim.GetFloat("Direction") * anim.GetFloat("Speed"));
        }
        else if (currentBaseState.fullPathHash == backState)
        {
            transform.Translate(Vector3.forward * anim.GetFloat("Speed") * 4 * Time.deltaTime);
            transform.Rotate(Vector3.up * Time.deltaTime * 100 * anim.GetFloat("Direction") * ((anim.GetFloat("Speed") < 0) ? -1 : 1) * anim.GetFloat("Speed"));
        }
        if (anim.GetBool("Jump"))
        {
            if (!jumped)
            {
                jumped = true;
                doJump = true;
            }
        }
        else
        {
            jumped = false;
        }
        if (anim.GetBool("Turn"))
        {
            //transform.Translate(Vector3.forward*1* Time.deltaTime);
            transform.Rotate(Vector3.up * Time.deltaTime * 100 * anim.GetFloat("Direction"));
        }
    }
	void Update(){
		if (doJump) {
			jumpWait-=Time.deltaTime;
			if(jumpWait<=0){
				transform.GetComponent<Rigidbody>().AddForce(Vector3.up*7000);
				doJump=false;
				jumpWait=0.3f;
			}
		}
	}

}