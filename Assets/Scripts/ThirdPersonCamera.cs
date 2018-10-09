using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour
{
    Transform standarPos;
    bool follow = false;

    void Start()
	{
		// initialising references
		//standardPos = cameraRef.transform;
	}
	
	void Update ()
	{
        if (follow)
        {
            transform.position = standarPos.position;
            transform.forward = standarPos.forward;
        }	
	}

    public void SetFollowToTransfrom(Transform cameraPosition)
    {
        standarPos = cameraPosition;
        follow = true;
    }

    public void StopFollowing() {
        follow = false;
    }
}
