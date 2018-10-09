using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnableDisable : MonoBehaviour {

    Image i;
    Button button;
    DemoKey key;

	// Use this for initialization
	void Start () {
        button = GetComponent<Button>();
        i = GetComponent<Image>();
        key = FindObjectOfType<DemoKey>();
        button.onClick.AddListener(DemoKeyControl);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void DemoKeyControl() {
        if (key.isActiveAndEnabled)
        {
            key.gameObject.SetActive(false);
            i.color = Color.red;
        }
        else {
            key.gameObject.SetActive(true);
            i.color = Color.green;
        }

    }
}
