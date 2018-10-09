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

public class TouchLogic : MonoBehaviour 
{
	public static int currTouch = 0;//so other scripts can know what touch is currently on screen
	private Ray ray;//this will be the ray that we cast from our touch into the scene
	private RaycastHit rayHitInfo = new RaycastHit();//return the info of the object that was hit by the ray
	[HideInInspector]
	public int touch2Watch = 64;
	[HideInInspector]
	public bool mouseDown=false;
	public GUITexture target;
	
	void Update () 
	{
		//executes this code for current touch (i) on screen
		if(target != null && (target.HitTest(Input.mousePosition)))
		{
			//if current touch hits our guitexture, run this code
			if (Input.GetMouseButtonDown (0)) 
			{
				//need to send message b/c function is not present in this script
				//OnTouchBegan();
				this.SendMessage("OnTouchBegan");
			}
			/*if (Input.GetMouseButtonUp (0))
			{
				//OnTouchEnded();
				this.SendMessage("OnTouchEnded");
			}
			if (Input.GetMouseButton(0))
			{
				//OnTouchMoved();
				this.SendMessage("OnTouchMoved");
			}*/
		}
		/*if (Input.GetMouseButtonDown (0)) {
			this.SendMessage("OnTouchBeganAnywhere");
		}*/
		if (Input.GetMouseButtonUp (0)) {
			this.SendMessage("OnTouchEndedAnywhere");
		}
		if (Input.GetMouseButton(0)) {
			this.SendMessage ("OnTouchMovedAnywhere");
		}
	}
}