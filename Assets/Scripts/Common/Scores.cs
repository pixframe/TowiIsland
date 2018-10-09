using UnityEngine;
using System.Collections;

public class Scores : MonoBehaviour
{
	public GameSaveLoad.game gameKey;
	public float tempScore;
	public float scoreSum;
	public int prevCorMult;

	public int score = 100;
	public bool finalScore = false;
	public int scoreCounter;
	public float scoreCountTimer;
	public float scoreCountTime = 2;
	public float scoreAdd;
	public string scoreString;

	public GUIStyle scoreStyle;

	public int kiwiMilestone = 500;
	public int scoreMilestone;
	public int extraKiwis;

	public AudioSource scoreSound;


	ShadowGameMain shadowScript;
	WhereIsTheBallLogic whereScript;
	Painter paintScript;
	MainRiver riverScript;
	SoundTree birdsScript;
	TreasureLogic treasureScript;

	// Use this for initialization
	void Start ()
	{
		scoreCountTimer = scoreCountTime;
		switch(gameKey){
		case GameSaveLoad.game.jungleDrawing:
			paintScript = GetComponent<Painter>();
			break;
		case GameSaveLoad.game.river:
			riverScript = GetComponent<MainRiver>();
			break;
		case GameSaveLoad.game.shadowGame:
			shadowScript = GetComponent<ShadowGameMain>();
			break;
		case GameSaveLoad.game.soundTree:
			birdsScript = GetComponent<SoundTree>();
			break;
		case GameSaveLoad.game.treasure:
			treasureScript = GetComponent<TreasureLogic>();
			break;
		case GameSaveLoad.game.whereIsTheBall:
			whereScript = GetComponent<WhereIsTheBallLogic>();
			break;
		}
	}

	// Update is called once per frame
	void Update ()
	{

	}
	public void ScoreAddValue()
	{
		scoreAdd = tempScore / (scoreCountTime * 60);
		finalScore = true;
	}
	public void ScoreCounter()
	{
		scoreCountTimer -= Time.deltaTime;
		if(scoreCountTimer >= 0)
		{
			scoreCounter +=  (int)scoreAdd;
			scoreMilestone += (int)scoreAdd;
			scoreString = scoreCounter.ToString();
		}
		else
		{
			int s = (int)scoreSum;
			scoreString = s.ToString();
            float t = scoreSum / kiwiMilestone;
            scoreCounter =(int)scoreSum;
			if(t != extraKiwis)
			{
				extraKiwis = (int)t;

			}
		}
	}
	public void GuiExtraKiwisDisplay()
	{
		if(scoreMilestone >= kiwiMilestone)
		{
			scoreSound.Play();
			extraKiwis++;
			scoreMilestone = 0;
		}
	}
	public int TempScoreSum()
	{
		float t;
		switch(gameKey){
		case GameSaveLoad.game.jungleDrawing:
			t = 0;
            if (paintScript.pickTimer >= 1)
			{
                t = score * paintScript.pickTimer;
				float m = (float)prevCorMult * score;
				t += m;
			}
			else
			{
				t = score;
				float m = (float)prevCorMult * score;
				t += m;
			}
			scoreSum += t;
			tempScore = t;			
			break;
		case GameSaveLoad.game.river:
            t = 0;
            if (riverScript.pickTimer >= 1)
			{
                t = score * riverScript.pickTimer;
				float m = (float)prevCorMult * score;
				t += m;
			}
			else
			{
				t = score;
				float m = (float)prevCorMult * score;
				t += m;
			}
			scoreSum += t;
			tempScore = t;			
			break;
		case GameSaveLoad.game.shadowGame:
			t = 0;
			if(shadowScript.pickTimer >= 1)
			{
				t = score * shadowScript.pickTimer;
				float m = (float)prevCorMult * score;
				t += m;
			}
			else
			{
				t = score;
				float m = (float)prevCorMult * score;
				t += m;
			}
			scoreSum += t;
			tempScore = t;
			break;
		case GameSaveLoad.game.soundTree:
            t = 0;
            if (birdsScript.GetPickTimer() >= 1)
			{
                t = score * birdsScript.GetPickTimer();
				float m = (float)prevCorMult * score;
				t += m;
			}
			else
			{
				t = score;
				float m = (float)prevCorMult * score;
				t += m;
			}
			scoreSum += t;
			tempScore = t;
			break;
		case GameSaveLoad.game.treasure:
            t = 0;
			t = score;
			float mult = (float)prevCorMult * score;
            t += mult;			
			scoreSum += t;
			tempScore = t;
			break;
		case GameSaveLoad.game.whereIsTheBall:
			t = 0;
			if(whereScript.pickTimer >= 1)
			{
				t = score * whereScript.pickTimer;
				float m = (float)prevCorMult * score;
				t += m;
			}
			else
			{
				t = score;
				float m = (float)prevCorMult * score;
				t += m;
			}
			scoreSum += t;
			tempScore = t;
			break;
		}
        return (int)tempScore;
	}
    public int GetExtraKiwis()
    {
        return (int)(scoreSum/kiwiMilestone);
    }
//	public float ScoreCalculator(float )
//	{
//
//	}
}
