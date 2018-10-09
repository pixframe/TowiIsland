using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaitingLogic : MonoBehaviour
{
	PEMainLogic mainLogicScript;
	PackLogic packScript;
	Timer timerScript;
	string[] fNUm;
    string[] tutorialStrings;
    public int tutorialError = -1;
	public string state;
	string correctFlight = "KW10";
	int num = 0;
    int tutorialIdx = 0;
    public bool tutorialDone = false;
	public float timeBetweenCalls = 2f;
	public int correct;
	public int incorrect;
	public int missed;
	bool finished = false;
	bool click = false;
	public AudioClip[] sounds;
	AudioSource player;
	float timerBetweenCalls;
	bool firstPlay = false;
    public Texture mouseFeedbackTexture;
    public float feedbackTime;
    public float feedbackScaleRate;
    public float opacityRate;
    List<MouseFeedback> feedbackList;
    float scale = 1;
    
	// Use this for initialization
	void Start ()
	{
		mainLogicScript = GameObject.FindGameObjectWithTag("Main").GetComponent<PEMainLogic>();
		timerScript = GameObject.FindGameObjectWithTag("Main").GetComponent<Timer>();
		packScript = GameObject.Find("Pack").GetComponent<PackLogic>();
		player = GetComponent<AudioSource>();
		state = "Intro";
		timerBetweenCalls = timeBetweenCalls;
        feedbackList = new List<MouseFeedback>();
        scale = Screen.height / 768f;
        tutorialStrings = new string[3] { "KZ45","KW02","RQ33"};
	}

	// Update is called once per frame
	void Update ()
	{
		if(fNUm == null)
		{
			fNUm = mainLogicScript.configuration.miniGame.waitingRoom.flights;
		}
		else
		{
			if(mainLogicScript.miniGame == "WaitingRoom")
			switch(state){
			case "Intro":
				break;
            case "Tutorial":
                if (!tutorialDone)
                {
                    if (tutorialIdx >= tutorialStrings.Length)
                    {
                        tutorialDone = true;
                        break;
                    }

                    timerBetweenCalls -= Time.deltaTime;

                    if (!firstPlay)
                    {
                        firstPlay = true;
                        player.clip = sounds[tutorialIdx+30];

                        if (!player.isPlaying)
                        {
                            player.Play();
                        }
                    }

                    if (timerBetweenCalls <= 0)
                    {
                        timerBetweenCalls = timeBetweenCalls;

                        if (tutorialStrings[tutorialIdx].Contains("KW") && !click)
                        {
                            tutorialError = 0;
                            tutorialDone = true;
                            break;
                            //missed++;
                            //Debug.Log("missed " + missed);
                        }

                        tutorialIdx++;
                        click = false;

                        if (tutorialIdx < tutorialStrings.Length)
                        {
                            player.clip = sounds[tutorialIdx+30];

                            if (!player.isPlaying)
                            {
                                player.Play();
                            }
                        }
                    }
					if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown("space")) && !click)
                    {
                        feedbackList.Add(new MouseFeedback(Input.mousePosition, 1, feedbackScaleRate, feedbackTime, opacityRate));
                        click = true;

                        if (tutorialStrings[tutorialIdx].Contains("KW") /*&& fNUm[num] != "KW10"*/)
                        {
                            Debug.Log("Tutcorrect " + correct);
                        }
                        else
                        {
                            Debug.Log("Tutincorrect " + incorrect);
                            tutorialError = 1;
                            tutorialDone = true;
                            break;
                        }
                        if (tutorialIdx == tutorialStrings.Length-1 && !tutorialDone)
                        {
                            tutorialDone = true;
                            break;
                        }
					}else if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown("space")) && click)
                    {
                        tutorialError = 2;
                        tutorialDone = true;
                        break;
                    }
                }
                break;
			case "GiveNum":
//				Debug.Log("lenght" + fNUm.Length);
				if(num >= fNUm.Length)
				{
					state = "Done";
					break;
				}
				timerBetweenCalls -= Time.deltaTime;
				if(!firstPlay)
				{
					firstPlay = true;
					player.clip = sounds[num];
					
					if(!player.isPlaying)
					{
						player.Play();
					}
				}

				if(timerBetweenCalls <= 0)
				{
					timerBetweenCalls = timeBetweenCalls;
					if(num != 0)
					{
						if(fNUm[num].Contains("KW") && !click)
						{
							missed++;
							Debug.Log("missed "+missed);
						}
					}
					num++;
					click = false;
					if(num < fNUm.Length)
					{
						player.clip = sounds[num];
						
						if(!player.isPlaying)
						{
							player.Play();
						}

					}
				}
				if((Input.GetMouseButtonDown(0) || Input.GetKeyDown("space")) && !click)
				{
                    feedbackList.Add(new MouseFeedback(Input.mousePosition, 1, feedbackScaleRate, feedbackTime,opacityRate));
					click = true;

					if(/*fNUm[num] != correctFlight && */fNUm[num].Contains("KW") /*&& fNUm[num] != "KW10"*/)
					{
						Debug.Log("correct "+correct);
						correct++;
					}
					else/* if(fNUm[num] != "KW10")*/
					{
						Debug.Log("incorrect "+incorrect);
						incorrect++;
					}
                    if (num == fNUm.Length-1 && !finished)
                    {
                        finished = true;
                        //							Debug.Log("correct finished");
                        //correct++;
                        state = "Done";

                    }
                }
				else if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown("space")) && click)
                {
                    feedbackList.Add(new MouseFeedback(Input.mousePosition, 1, feedbackScaleRate, feedbackTime, opacityRate));
                    Debug.Log("incorrect " + incorrect);
                    incorrect++;
                }
				/*else if(Input.GetMouseButtonDown(0))
				{
					if(click)
					{
						if(fNUm[num] == correctFlight && !finished)
						{
							finished = true;
//							Debug.Log("correct finished");
							correct++;
							state = "Done";

						}
					}
				}*/

				break;
			case "Done":
				mainLogicScript.curGameFinished = true;
				break;
			
			}
		}
        for(int i=0;i<feedbackList.Count;i++)
        {
            feedbackList[i].Update();
        }
        feedbackList.RemoveAll(a => a.destroy);
	}

    void OnGUI()
    {
        for (int i = 0; i < feedbackList.Count; i++)
        {
            GUI.color=new Color(1, 1, 1, feedbackList[i].opacity);
            GUI.DrawTexture(new Rect(feedbackList[i].feedBackPos.x - feedbackList[i].feedbackSize * 0.6f * scale, Screen.height- feedbackList[i].feedBackPos.y - feedbackList[i].feedbackSize * 0.6f * scale, feedbackList[i].feedbackSize * scale, feedbackList[i].feedbackSize * scale), mouseFeedbackTexture);
            GUI.color = new Color(1, 1, 1, 1);
            feedbackList[i].Update();
        }
    }

    public void StartTutorial()
    {
        click = false;
        tutorialDone = false;
        tutorialIdx=0;
        tutorialError = -1;
        timerBetweenCalls = timeBetweenCalls;
        firstPlay = false;
        state = "Tutorial";
    }

    public void StartGame()
    {
        click = false;
        timerBetweenCalls = timeBetweenCalls;
        firstPlay = false;
        state = "GiveNum";
    }

    public class MouseFeedback
    {
        public Vector2 feedBackPos;
        public float feedbackSize;
        public float currentfeedbackTime;
        public bool destroy;
        public float opacity;
        float scaleR;
        float opacityR;
        public MouseFeedback(Vector2 pos,float size, float scaleRate, float time,float opacityRate)
        {
            feedBackPos = pos;
            feedbackSize = size;
            currentfeedbackTime = time;
            scaleR=scaleRate;
            opacity = 1;
            opacityR = opacityRate;
            destroy = false;
        }
        public void Update()
        {
            feedbackSize+=scaleR*Time.deltaTime;
            opacity -= opacityR * Time.deltaTime;
            opacity = Mathf.Max(opacity, 0);
            currentfeedbackTime -= Time.deltaTime;
            if (currentfeedbackTime <= 0)
                destroy = true;
        }
    }
}
