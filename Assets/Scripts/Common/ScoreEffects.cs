using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreEffects : MonoBehaviour {

	public GUIStyle scoreStyle;
	public float fadeSpeed=0.2f;
	public float speed=1;
	public float fadeWait=1;

	public float fadeWaitObj=1;
	public float fadeSpeedObj=0.2f;
	public float speedObj=1;

	List<Score> activeScores;
	List<NewObj> activeNewObj;
	public float testTime=2;
	public Camera cam;
	Transform effects;
	//List<ParticleEmitter> particles;
	float testCurrTime;
	float scale;
	public AudioClip scoreSound;
	public AudioClip errorSound;
	public Texture redColor;
	public Texture cubeTexture;
	public float errormarginWidth=10;
	public float errorTime=1;
	float currectErrorTime=0;
	bool showError=false;
	// Use this for initialization
	void Start () {
		scale = (float)Screen.height / (float)768;
		scoreStyle.fontSize=(int)((float)scoreStyle.fontSize*scale);
		testCurrTime = testTime;
		activeScores = new List<Score> ();
		activeNewObj = new List<NewObj> ();
		effects=transform.Find("Effects");
		/*particles = new List<ParticleEmitter> ();
		if(effects)
		{
			for(int i=0;i<effects.childCount;i++)
			{
				ParticleEmitter tempP=effects.GetChild(i).GetComponent<ParticleEmitter>();
				if(tempP)
					particles.Add(tempP);
			}
			effects.transform.position=cam.transform.position+cam.transform.forward;
		}*/
	}
	
	// Update is called once per frame
	void Update () {
		/*testCurrTime -= Time.deltaTime;
		if(testCurrTime<=0)
		{
			testCurrTime=testTime;
			DisplayNewObject(1);
		}*/
		for(int i=0;i<activeScores.Count;i++)
		{
			if(activeScores[i].fadeWait<=0)
				activeScores[i].alpha-=activeScores[i].fadeSpeed;
			else
				activeScores[i].fadeWait-=Time.deltaTime;
			activeScores[i].yOffset-=activeScores[i].moveSpeed;
			if(activeScores[i].alpha<=0)
				activeScores[i].delete=true;
		}
		for(int i=0;i<activeNewObj.Count;i++)
		{
			if(activeNewObj[i].fadeIn)
			{
				activeNewObj[i].alpha+=activeNewObj[i].fadeSpeed;
				if(activeNewObj[i].alpha>=1)
				{
					activeNewObj[i].alpha=1;
					activeNewObj[i].fadeIn=false;
				}
			}else{
				if(activeNewObj[i].fadeWait<=0)
					activeNewObj[i].alpha-=activeNewObj[i].fadeSpeed;
				else
					activeNewObj[i].fadeWait-=Time.deltaTime;
				if(activeNewObj[i].alpha<=0)
					activeNewObj[i].delete=true;
			}
			activeNewObj[i].yOffset-=activeNewObj[i].moveSpeed;
		}
		for(int i=0;i<activeScores.Count;i++)
		{
			if(activeScores[i].delete)
			{
				activeScores.RemoveAt(i--);
			}
		}
		for(int i=0;i<activeNewObj.Count;i++)
		{
			if(activeNewObj[i].delete)
			{
				activeNewObj.RemoveAt(i--);
			}
		}
	}

	public void DisplayScore(int value)
	{
		AudioSource.PlayClipAtPoint (scoreSound, cam.transform.position,0.1f);
		activeScores.Add (new Score (value, fadeWait, fadeSpeed, speed));
		//for(int i=0;i<particles.Count;i++)
			//particles[i].Emit();
	}
	public void DisplayScore(int value,bool sound)
	{
		if(sound)
			AudioSource.PlayClipAtPoint (scoreSound, cam.transform.position,0.1f);
		activeScores.Add (new Score (value, fadeWait, fadeSpeed, speed));
		//for(int i=0;i<particles.Count;i++)
			//particles[i].Emit();
	}
	public void DisplayError()
	{
		AudioSource.PlayClipAtPoint (errorSound, cam.transform.position,0.1f);
		currectErrorTime = errorTime;
		showError = true;
	}
	public void DisplayError(bool sound)
	{
		if(sound)
			AudioSource.PlayClipAtPoint (errorSound, cam.transform.position,0.1f);
		currectErrorTime = errorTime;
		showError = true;
	}
    public void DisplayNewObject(bool positive, int value, Texture preview)
	{
		AudioSource.PlayClipAtPoint (positive?scoreSound:errorSound, cam.transform.position,0.1f);
		activeNewObj.Add (new NewObj (value, fadeWaitObj, fadeSpeedObj, speedObj, preview));
	}
	public void DisplayNewObject(bool positive, int value,bool sound, Texture preview)
	{
		if(sound)
			AudioSource.PlayClipAtPoint (positive ? scoreSound : errorSound, cam.transform.position,0.1f);
		activeNewObj.Add (new NewObj (value, fadeWaitObj, fadeSpeedObj, speedObj, preview));
	}
    public void DisplayNewObject(bool positive, string value, bool sound, Texture preview)
    {
        if (sound)
            AudioSource.PlayClipAtPoint(positive ? scoreSound : errorSound, cam.transform.position, 0.1f);
        activeNewObj.Add(new NewObj(value, fadeWaitObj, fadeSpeedObj, speedObj, preview));
    }

    void OnGUI()
	{
        GUI.depth = -1;
		for(int i=0;i<activeScores.Count;i++)
		{	
			GUI.color=new Color(1,1,1,activeScores[i].alpha);
			scoreStyle.normal.textColor=new Color(0.5f,0.5f,0.5f);
			GUI.Label(new Rect(Screen.width*0.5f-198*scale,Screen.height*0.5f-198*scale-activeScores[i].yOffset,400*scale,400*scale),"+"+activeScores[i].value.ToString(),scoreStyle);
			scoreStyle.normal.textColor=new Color(1f,1f,1f);
			GUI.Label(new Rect(Screen.width*0.5f-200*scale,Screen.height*0.5f-200*scale-activeScores[i].yOffset,400*scale,400*scale),"+"+activeScores[i].value.ToString(),scoreStyle);
			GUI.color=new Color(1,1,1,1);
		}
		for(int i=0;i<activeNewObj.Count;i++)
		{	
			GUI.color=new Color(1,1,1,activeNewObj[i].alpha);
            if (activeNewObj[i].text == "")
            {
                GUI.DrawTexture(new Rect(Screen.width * 0.5f - 20 * scale, Screen.height * 0.4f - 50 * scale - activeNewObj[i].yOffset, 100 * scale, 100 * scale), activeNewObj[i].customTexture != null ? activeNewObj[i].customTexture : cubeTexture);
            }

            string label = "";
            if(activeNewObj[i].text =="")
            {
                label = "+" + activeNewObj[i].value.ToString();
            }
            else
            {
                label = activeNewObj[i].text;
            }

            scoreStyle.normal.textColor=new Color(0.5f,0.5f,0.5f);
			GUI.Label(new Rect(Screen.width*0.5f-268*scale,Screen.height*0.4f-198*scale-activeNewObj[i].yOffset,400*scale,400*scale),label,scoreStyle);
			scoreStyle.normal.textColor=new Color(1f,1f,1f);
			GUI.Label(new Rect(Screen.width*0.5f-270*scale,Screen.height*0.4f-200*scale-activeNewObj[i].yOffset,400*scale,400*scale),label,scoreStyle);
			GUI.color=new Color(1,1,1,1);
		}
		if(showError)
		{
			currectErrorTime-=Time.deltaTime;
			GUI.DrawTexture(new Rect(0,0,errormarginWidth*scale,Screen.height),redColor);
			GUI.DrawTexture(new Rect(0,0,Screen.width,errormarginWidth*scale),redColor);
			GUI.DrawTexture(new Rect(Screen.width-errormarginWidth*scale,0,errormarginWidth*scale,Screen.height),redColor);
			GUI.DrawTexture(new Rect(0,Screen.height-errormarginWidth*scale,Screen.width,errormarginWidth*scale),redColor);
			if(currectErrorTime<=0)
				showError=false;
		}
	}

	class Score
	{
		public int value;
		public float alpha;
		public float fadeWait;
		public float fadeSpeed;
		public float moveSpeed;
		public float yOffset;
		public bool delete;
		public Score(int score, float wait, float fSpeed,float mSpeed)
		{
			value=score;
			alpha=1;
			fadeWait=wait;
			fadeSpeed=fSpeed;
			moveSpeed=mSpeed;
			yOffset=0;
			delete=false;
		}
	}
	class NewObj
	{
		public int value;
		public float alpha;
		public float fadeWait;
		public float fadeSpeed;
		public float moveSpeed;
		public float yOffset;
		public bool fadeIn;
		public bool delete;
        public string text;
        public Texture customTexture;
		public NewObj(int score, float wait, float fSpeed,float mSpeed,Texture preview)
		{
            text = "";
			value=score;
			alpha=0;
			fadeWait=wait;
			fadeSpeed=fSpeed;
			moveSpeed=mSpeed;
			yOffset=0;
			fadeIn=true;
			delete=false;
            customTexture = preview;
        }
        public NewObj(string message, float wait, float fSpeed, float mSpeed, Texture preview)
        {
            text = message;
            alpha = 0;
            fadeWait = wait;
            fadeSpeed = fSpeed;
            moveSpeed = mSpeed;
            yOffset = 0;
            fadeIn = true;
            delete = false;
            customTexture = preview;
        }
    }
}
