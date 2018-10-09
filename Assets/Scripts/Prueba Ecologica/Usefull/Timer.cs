using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{
	float curTimer = 10000f;

	public bool TimerFunc(float time)
	{

		if(curTimer == 10000f)
		{
			curTimer = time;
		}
		curTimer -= Time.deltaTime;
		if(curTimer <= 0f)
		{
			curTimer = 10000f;
			return true;
		}
		else
		{
			return false;
		}

	}
}
