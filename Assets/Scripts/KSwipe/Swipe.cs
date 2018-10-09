using UnityEngine;
using System.Collections;

public class Swipe : MonoBehaviour {

    private bool tap, swipeLeft, swipeRight;
    private Vector2 startTouch, swipeDelta;
    private bool isDragging = false;
	// Update is called once per frame
	private void Update ()
    {
        swipeLeft = swipeRight = false;
        tap = false;
        #region Standalone Inputs
        if (Input.GetMouseButtonDown(0))
        {
            tap = true;
            isDragging = true;
            startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;            
            Reset();
        }

        #endregion
        
        #region Mobile Inputs
        if (Input.touches.Length != 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                tap = true;
                isDragging = true;
                startTouch = Input.touches[0].position;
            }
            else  if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                isDragging = false;               
                Reset();
            }
               
        }
        #endregion

        //Distancia
        swipeDelta = Vector2.zero;
        if(isDragging)
        {
            if (Input.touches.Length > 0)
                swipeDelta = Input.touches[0].position - startTouch;
            else if (Input.GetMouseButton(0))
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
        }

        if(swipeDelta.magnitude > 100)
        {
            float x = swipeDelta.x;
            //Lefts or Right
            if (x < 0)
            {
                swipeLeft = true;
               
            }
               
            else
            {
                swipeRight = true;
                
            }             

            Reset();                
        }
    }

    private void Reset()
    {
        startTouch = swipeDelta = Vector2.zero;
        tap = false;
        isDragging = false;
    }
    public Vector2 SwipeDelta { get { return swipeDelta; } }
    public bool SwipeLeft { get { return swipeLeft; } }
    public bool SwipeRight { get { return swipeRight; } }
    public bool Tap { get { return tap; } }
}
