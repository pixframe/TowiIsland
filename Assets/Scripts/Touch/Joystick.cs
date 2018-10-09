/*/
* Script by Devin Curry
	* www.Devination.com
		* www.youtube.com/user/curryboy001
		* Please like and subscribe if you found my tutorials helpful :D
			* Google+ Community: https://plus.google.com/communities/108584850180626452949
				* Twitter: https://twitter.com/Devination3D
				* Facebook Page: https://www.facebook.com/unity3Dtutorialsbydevin
				/*/
using UnityEngine;
using System.Collections;

public class Joystick : TouchLogicBU
{
	public enum JoystickType {Movement, LookRotation, SkyColor};
	public JoystickType joystickType;
	public Transform player = null;
	BotControlScript controller=null;
	public float playerSpeed = 2f, maxJoyDelta = 200f, rotateSpeed = 100.0f;
	private Vector3 oJoyPos, joyDelta;
	private Transform joyTrans = null;
	private float pitch = 0.0f,
	yaw = 0.0f;
	float scale=1;
	//[NEW]//cache initial rotation of player so pitch and yaw don't reset to 0 before rotating
	private Vector3 oRotation;
	void Start () 
	{
		scale = (float)Screen.height / (float)768;
		joyTrans = this.transform;
		oJoyPos = joyTrans.position;
		GetComponent<GUITexture>().pixelInset = new Rect (GetComponent<GUITexture>().pixelInset.x * scale, GetComponent<GUITexture>().pixelInset.y * scale,
		                               GetComponent<GUITexture>().pixelInset.width * scale, GetComponent<GUITexture>().pixelInset.height * scale);
		maxJoyDelta *= scale;
		GUI.depth = 0;
		//NEW//cache original rotation of player so pitch and yaw don't reset to 0 before rotating
	}
	
	void OnTouchBegan()
	{
		touch2Watch = TouchLogicBU.currTouch;
	}
	
	void OnTouchMovedAnywhere()
	{
		if(TouchLogicBU.currTouch == touch2Watch)
		{
			//move the joystick
			joyTrans.position = MoveJoyStick();
			ApplyDeltaJoy();
		}
	}
	
	void OnTouchStayed()
	{
		if(TouchLogicBU.currTouch == touch2Watch)
		{
			ApplyDeltaJoy();
		}
	}
	
	void OnTouchEndedAnywhere()
	{
		if(TouchLogicBU.currTouch == touch2Watch || Input.touches.Length <= 0)
		{
			touch2Watch = 64;
			if(controller)
			{
				controller.h=0;
				controller.v = 0;
			}
				//move the joystick back to its orig position
			joyTrans.position = oJoyPos;
		}
	}
	
	void ApplyDeltaJoy()
	{
		switch(joystickType)
		{
		case JoystickType.Movement:
			if(controller)
			{
				controller.h=joyDelta.x;
				controller.v=joyDelta.z;
			}
			break;
		case JoystickType.LookRotation:
			pitch -= Input.GetTouch(touch2Watch).deltaPosition.y * rotateSpeed * Time.deltaTime;
			yaw += Input.GetTouch(touch2Watch).deltaPosition.x * rotateSpeed * Time.deltaTime;
			
			//limit so we dont do backflips
			pitch = Mathf.Clamp(pitch, -80, 80);
			
			//do the rotations of our camera 
			player.eulerAngles += new Vector3 ( pitch, yaw, 0.0f);
			break;
		case JoystickType.SkyColor:
			Camera.main.backgroundColor = new Color(joyDelta.x, joyDelta.z, joyDelta.x*joyDelta.z);
			break;
			
		}
	}
	
	Vector3 MoveJoyStick()
	{
		//convert the touch position to a % of the screen to move our joystick
		float x = Input.GetTouch (touch2Watch).position.x / Screen.width,
		y = Input.GetTouch (touch2Watch).position.y / Screen.height;
		//combine the floats into a single Vector3 and limit the delta distance
		Vector3 position = new Vector3 (Mathf.Clamp(x, oJoyPos.x - (float)maxJoyDelta/(float)Screen.width, oJoyPos.x + (float)maxJoyDelta/(float)Screen.width),
		                                Mathf.Clamp(y, oJoyPos.y - (float)maxJoyDelta/(float)Screen.height, oJoyPos.y + (float)maxJoyDelta/(float)Screen.height),0);
		//joyDelta used for moving the player
		joyDelta = new Vector3(position.x - oJoyPos.x, 0, position.y - oJoyPos.y).normalized;
		//Debug.Log (joyDelta);
		//position used for moving the joystick
		return position;
	}
	
	void Update()
	{
		if(Input.touches.Length <= 0)
		{
			touch2Watch = 64;
			if(controller)
			{
				controller.h=0;
				controller.v = 0;
			}
			//move the joystick back to its orig position
			joyTrans.position = oJoyPos;
		}
		if (player) {
			if(!controller)
			{
				controller=player.GetComponent<BotControlScript>();
				oRotation = player.eulerAngles;
				pitch = oRotation.x;
				yaw = oRotation.y;
			}else
			{

			}
		}else{
			GameObject temp= GameObject.Find("Character");
			if(temp)
				player=temp.transform;
		}
	}
}