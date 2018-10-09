using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shower : MonoBehaviour {

    public GameObject showerManager;
    public GameObject displayManager;
    public LayerMask mask;
    public Button goBackButton;
    public Button exitButton;
    public Slider slidy;

    bool rotateTheShower = false;
    Camera cam;

    int sibilingNumber = 0;
    float speedOfRotate;

	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
        goBackButton.onClick.AddListener(SelectionTime);
        SelectionTime();
        exitButton.onClick.AddListener(ExitTheApp);
	}
	
	// Update is called once per frame
	void Update () {
        if (!rotateTheShower)
        {
            SelectAnObject();
        }
        else {
            speedOfRotate = slidy.value;
            RotateTheShoweMate();
        }
	}

    void SelectAnObject() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            {
                if (hit.transform != null) {
                    sibilingNumber = hit.transform.GetSiblingIndex();
                    ShowerTime();
                }
            }
        }
    }

    void SelectionTime() {
        displayManager.SetActive(true);
        cam.orthographic = true;
        showerManager.transform.GetChild(sibilingNumber).gameObject.SetActive(false);
        goBackButton.gameObject.SetActive(false);
        slidy.gameObject.SetActive(false);
        rotateTheShower = false;
    }

    void ShowerTime() {
        displayManager.SetActive(false);
        cam.orthographic = false;
        showerManager.transform.GetChild(sibilingNumber).gameObject.SetActive(true);
        goBackButton.gameObject.SetActive(true);
        slidy.gameObject.SetActive(true);
        rotateTheShower = true;
    }

    void RotateTheShoweMate() {
        showerManager.transform.Rotate(Vector3.up, speedOfRotate * Time.deltaTime);
    }

    void ExitTheApp() {
        Application.Quit();
    }
}
