using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaAdjuster : MonoBehaviour {

    //We get the rect transform that will be modified
    RectTransform Panel;
    //This ist the last know safe area
    Rect LastSafeArea = new Rect(0, 0, 0, 0);

    KeyCode KeySafeArea = KeyCode.A;
    public enum SimDevice { None, iPhoneX }
    public static SimDevice Sim = SimDevice.None;
    Rect[] NSA_iPhoneX = new Rect[]
    {
            new Rect (0f, 102f / 2436f, 1f, 2202f / 2436f),  // Portrait
            new Rect (132f / 2436f, 63f / 1125f, 2172f / 2436f, 1062f / 1125f)  // Landscape
    };

    void Awake()
    {
        //We get the rect component attach to the game object
        Panel = GetComponent<RectTransform>();
        //We used this method to update the safe area of the device
        Refresh();
    }

    void Refresh()
    {
        //We get the safe area for the  device 
        Rect safeArea = GetSafeArea();

        //We compare if the safe area its different than that we create a new safe area
        if (safeArea != LastSafeArea)
        {
            //This is the mothod we used to avoid notch's
            ApplySafeArea(safeArea);
        }
    }

    //This is the safe area of the screen
    Rect GetSafeArea()
    {
        Rect safeArea = Screen.safeArea;

        if (Application.isEditor && Sim != SimDevice.None)
        {
            Rect nsa = new Rect(0, 0, Screen.width, Screen.height);

            switch (Sim)
            {
                case SimDevice.iPhoneX:
                    if (Screen.height > Screen.width)  // Portrait
                        nsa = NSA_iPhoneX[0];
                    else  // Landscape
                        nsa = NSA_iPhoneX[1];
                    break;
                default:
                    break;
            }

            safeArea = new Rect(Screen.width * nsa.x, Screen.height * nsa.y, Screen.width * nsa.width, Screen.height * nsa.height);
        }

        return safeArea;
    }


    //This method is the one that we use to resize the canvas of election to fit properly
    void ApplySafeArea(Rect r)
    {
        //We change the last safe area to a new safe area 
        LastSafeArea = r;

        // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
        Vector2 anchorMin = r.position;
        Vector2 anchorMax = r.position + r.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        Panel.anchorMin = anchorMin;
        Panel.anchorMax = anchorMax;

        Debug.LogFormat("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
            name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
    }
}
