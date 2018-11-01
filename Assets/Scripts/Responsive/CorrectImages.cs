using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectImages : MonoBehaviour
{
    void Start()
    {
        //Find all the images inside this canvas and add them into a list
        List<GameObject> images = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            images.Add(transform.GetChild(i).gameObject);
        }

        //determine wich is the aspect ratio
        float widht = Screen.width;
        float height = Screen.height;

        float aspectRatio = widht / height;

        //Determine the action thanks to the aspect ratio
        if (aspectRatio < 1.65f)
        {
            //Iterate trough all the images and select wich ones will be active and wich ones not
            for (int i = 0; i < images.Count; i++)
            {
                if (i < images.Count / 2)
                {
                    images[i].SetActive(true);
                }
                else
                {
                    images[i].SetActive(false);
                    Destroy(images[i]);
                }
            }
        }
        else
        {
            //Iterate trough all the images and select wich ones will be active and wich ones not
            for (int i = 0; i < images.Count; i++)
            {
                if (i < images.Count / 2)
                {
                    images[i].SetActive(false);
                    Destroy(images[i]);
                }
                else
                {
                    images[i].SetActive(true);
                }
            }
        }
    }
}
