using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoKey : MonoBehaviour {

    public static int numberOfDemoKeys = 0;
    public TextAsset commonAsset;
    public TextAsset addableAsset;
    public TextAsset beforeAssets;
	// Use this for initialization
	void Start () {
        TextReader.FillAddables(addableAsset);
        TextReader.FillBefore(beforeAssets);
        TextReader.FillCommon(commonAsset);

        numberOfDemoKeys++;
        if (DemoKey.numberOfDemoKeys < 2)
        {
            DontDestroyOnLoad(this.gameObject);
        }
        else {
            Destroy(this.gameObject);
        }

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
