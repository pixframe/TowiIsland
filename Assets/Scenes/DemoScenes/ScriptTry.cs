using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptTry : MonoBehaviour
{
    public HeaderToJson headerToJson;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(headerToJson.GetData());
        GetComponent<Button>().onClick.AddListener(() =>
        {
            headerToJson.SetData("Pablo", 4);
            Debug.Log(headerToJson.GetData());
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
