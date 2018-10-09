using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashProbes : MonoBehaviour {

    string nameOfKid;
    int ageOfKid;
    int arrowDifficulty;
    bool secondRow = false;

    public static int count = 0;

	// Use this for initialization
	void Start () {
        count++;
        if (count > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
	}

    public void SetData(string names, int ages)
    {
        nameOfKid = names;
        ageOfKid = ages;
    }

    public string GetName()
    {
        return name;
    }

    public int GetAge()
    {
        return ageOfKid;
    }

    public int ArrowData()
    {
        return arrowDifficulty;
    }

    public bool IsSecondRow()
    {
        return secondRow;
    }

    public void NewArrowScene()
    {
        secondRow = true;
        arrowDifficulty = 1;
    }

    public void ResetTheScene()
    {
        secondRow = false;
        arrowDifficulty = 0;
    }

    public void DestryTheScript()
    {
        Destroy(this);
    }
}
