using UnityEngine;
using System.Collections;

public class FadeInOut : MonoBehaviour
{
	GameObject plane;
	public bool fade = false;
	public Material mat;
	public Color col;
	
	float timeFade;
	
	public string fadeState;
	
	// Use this for initialization
	void Start ()
	{
		plane = transform.Find("FadePlane").gameObject;
		mat = plane.transform.GetComponent<MeshRenderer>().materials[0];
		col = mat.color;
		plane.SetActive(true);
//		FadeFunc(true, 1f);
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(fade)
		{

			switch(fadeState){
				
			case "FadeIn":
				if(mat.color.a >= 0f)
				{
				
//					Debug.Log("fading in");
					col.a -= (1 / timeFade) * Time.deltaTime;
					mat.color = col;
				}
				else
				{
					col.a = 0f;
					mat.color = col;
//					Debug.Log("aplha " + col.a);
					fade = false;
					plane.SetActive(false);
				}
				break;
			case "FadeOut":
				if(mat.color.a <= 1f)
				{
//					Debug.Log("fading out");
					col.a += (1 / timeFade) * Time.deltaTime;
					mat.color = col;	
				}
				else
				{
					col.a = 1f;
					mat.color = col;
//					Debug.Log("aplha " + col.a);
					
					fade = false;
				}
				
				break;
			}
			
		}
	}
	
	public void FadeFunc(bool fadeIn, float timeOfFade)
	{
		if(fadeIn)
		{
			fadeState = "FadeIn";	
		}
		else
		{
			plane.SetActive(true);
			fadeState = "FadeOut";
		}
		fade = true;
		timeFade = timeOfFade;
	}
}
