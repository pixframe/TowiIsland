using UnityEngine;
using System.Collections;

public class CamComtrol : MonoBehaviour
{
	//script variables
	PEMainLogic logicScript;
	FadeInOut fadeScript;
	PackLogic packScritp;
	SelectionScript selecScript;



	//logic vars
	bool rotate;
	Vector3 secondPos;

	//control vars
	public float sensitivityX = 15f;
	public float sensitivityY = 15f;
	public float minimumX = -40f;
	public float maximumX = 40f;
	public float minimumY = -40f;
	public float maximumY = 40f;
	public float rotationY = 0f;
	public float rotationX = 0f;

	// Use this for initialization
	void Start ()
	{
		logicScript = GameObject.Find("Main").GetComponent<PEMainLogic>();
		fadeScript = Camera.main.GetComponent<FadeInOut>();
		packScritp = GameObject.Find("Pack").GetComponent<PackLogic>();
		selecScript = GameObject.Find("Main").GetComponent<SelectionScript>();

	}

	// Update is called once per frame
	void Update ()
	{
		if(logicScript.miniGame == "Pack" && !fadeScript.fade)
		{
			if(packScritp.gameState == "Controls")
			{
				if(packScritp.controlsOn)
				{
					//cam rotation in control explanation
					if(Input.GetMouseButtonUp(0))
					{
						if(rotate)
						{
							rotate = false;
							packScritp.controlsOn = false;
							packScritp.rotationFinished = true;
//							transform.rotation = Quaternion.identity;
							rotationX = 0f;
							rotationY = 0f;
						}
					}
					if(Input.GetMouseButtonDown(0) && !selecScript.SelectionFunc(Input.mousePosition))//mouse down and no object is selected
					{
						rotate = true;
					}
					
					if(rotate)
					{
						
//						rotationX += Input.GetAxis("Mouse X") * sensitivityX;
//						rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);
//						
//						rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
//						rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
//						
//						transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
					}
				}
			}
			else if(packScritp.gameState == "PlayerPick")
			{
				if(packScritp.controlsOn)
				{

					//cam rot possible in player pick phase
					if(Input.GetMouseButtonUp(0))
					{
					
						if(rotate)
						{
							rotate = false;
						}

					}
					if(Input.GetMouseButtonDown(0) && !selecScript.SelectionFunc(Input.mousePosition))//mouse down and no object is selected
					{

						rotate = true;
					}

					if(rotate)
					{

//						rotationX += Input.GetAxis("Mouse X") * sensitivityX;
//					
//						rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);
//						
//						rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
//						rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
//						
//						transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
					}
				}
			}
		}
	}
}
