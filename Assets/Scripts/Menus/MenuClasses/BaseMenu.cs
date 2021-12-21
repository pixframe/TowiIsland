using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMenu
{
    public GameObject gameObject;

    public void Initialization (GameObject baseGameObject)
    {
        gameObject = baseGameObject;
    }

    public void SetActive(bool status)
    {
        gameObject.SetActive(status);
    }
}
