using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayerForMenus : MonoBehaviour
{
	// Use this for initialization
	void Awake ()
    {
        if (FindObjectsOfType<AudioPlayerForMenus>().Length > 1)
        {
            Destroy(this.gameObject);
        }
	}
}
