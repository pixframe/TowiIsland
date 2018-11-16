using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraIslandShop : MonoBehaviour {

    bool canMoveCamera;
    Camera cameraOfTheIsland;

    Vector2? oldTouchPosition;
    Vector2 oldTouchVector;
    Vector2 firstTouchPrevPos;
    Vector2 secondTouchPrevPos;

    float oldTouchDistance;
    float touchesPrevPosDifference;
    float touchesCurPosDifference;
    float zoomModifier;
    float zoomModifierSpeed = 0.1f;

    Vector3 originalPos; 

    // Use this for initialization
    void Start ()
    {
        cameraOfTheIsland = GetComponent<Camera>();
        originalPos = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        MoveCamera();
        
	}

    //This is used to move the camera
    void MoveCamera()
    {
        if (canMoveCamera)
        {
            if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                //We checks if thres any touch
                //if its not we erase all data 
                if (Input.touchCount == 0)
                {
                    oldTouchPosition = null;
                }
                //If theres one touch
                else if (Input.touchCount == 1)
                {
                    if (oldTouchPosition == null)
                    {
                        oldTouchPosition = Input.GetTouch(0).position;
                    }
                    else
                    {
                        var newTouchPosition = Input.GetTouch(0).position;

                        transform.position += transform.TransformDirection((Vector3)((oldTouchPosition - newTouchPosition) * cameraOfTheIsland.orthographicSize / cameraOfTheIsland.pixelHeight * 2f));
                        var pos = transform.position;
                        pos.x = Mathf.Clamp(transform.position.x, -30f, 16f);
                        pos.y = Mathf.Clamp(transform.position.y, 45.5f, 54f);
                        transform.position = pos;
                        oldTouchPosition = newTouchPosition;
                    }
                }
                else
                {
                    if (Input.touchCount == 2)
                    {
                        var firstTouch = Input.GetTouch(0);
                        var secondTouch = Input.GetTouch(1);

                        firstTouchPrevPos = firstTouch.position - firstTouch.deltaPosition;
                        secondTouchPrevPos = secondTouch.position - secondTouch.deltaPosition;

                        touchesPrevPosDifference = (firstTouchPrevPos - secondTouchPrevPos).magnitude;
                        touchesCurPosDifference = (firstTouch.position - secondTouch.position).magnitude;

                        zoomModifier = (firstTouch.deltaPosition - secondTouch.deltaPosition).magnitude * zoomModifierSpeed;

                        if (touchesPrevPosDifference > touchesCurPosDifference)
                        {
                            cameraOfTheIsland.fieldOfView += zoomModifier;
                        }
                        if (touchesPrevPosDifference < touchesCurPosDifference)
                        {
                            cameraOfTheIsland.fieldOfView -= zoomModifier;
                        }
                    }

                    cameraOfTheIsland.fieldOfView = Mathf.Clamp(cameraOfTheIsland.fieldOfView, 5f, 40f);
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0))
                {
                    oldTouchPosition = null;
                }
                if (Input.GetMouseButton(0))
                {
                    if (oldTouchPosition == null)
                    {
                        oldTouchPosition = Input.mousePosition;
                    }
                    else
                    {
                        var newTouchPosition = Input.mousePosition;

                        transform.position += transform.TransformDirection((Vector3)((oldTouchPosition - newTouchPosition) * cameraOfTheIsland.orthographicSize / cameraOfTheIsland.pixelHeight * 2f));

                        var pos = transform.position;
                        pos.x = Mathf.Clamp(transform.position.x, -30f, 16f);
                        pos.y = Mathf.Clamp(transform.position.y, 45.5f, 54f);
                        pos.z = -24f;
                        transform.position = pos;

                        oldTouchPosition = newTouchPosition;
                    }
                }
                if (Input.mouseScrollDelta.y != 0)
                {
                    Debug.Log("Should zoom");
                    float zoomMovement = Input.mouseScrollDelta.y;
                    Debug.Log(zoomMovement);

                    zoomModifier = zoomMovement * zoomModifierSpeed * 10f;

                    cameraOfTheIsland.fieldOfView -= zoomModifier;

                    cameraOfTheIsland.fieldOfView = Mathf.Clamp(cameraOfTheIsland.fieldOfView, 5f, 40f);
                }

            }
        }
    }

    void ZoomTheCamera()
    {

    }

    //This is used to enable the movement of the camera
    public bool EnableMoveCamera()
    {
        if (canMoveCamera)
        {
            canMoveCamera = false;
            transform.position = originalPos;
        }
        else
        {
            canMoveCamera = true;
        }

        return canMoveCamera;
    }
}
