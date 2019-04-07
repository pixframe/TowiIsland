using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoKey : MonoBehaviour {

    public static int numberOfDemoKeys = 0;
    public TextAsset commonAsset;
    public TextAsset addableAsset;
    public TextAsset beforeAssets;

    int difficulty;

    int levelA;
    int levelB;
    int levelC;

    bool isFLISActive = true;
    bool isLevelSet = false;
	// Use this for initialization
	void Start ()
    {
        TextReader.FillAddables(addableAsset);
        TextReader.FillBefore(beforeAssets);
        TextReader.FillCommon(commonAsset);

        numberOfDemoKeys++;
        if (DemoKey.numberOfDemoKeys < 2)
        {
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        Debug.Log("is FLIS active? " + isFLISActive);
    }

    public void SetDifficulty(int difficultyInput)
    {
        difficulty = Mathf.Clamp(difficultyInput, 0, 2);
        Debug.Log("difficulty is" + difficulty);
    }

    public int GetDifficulty()
    {
        return difficulty;
    }

    public void ChangeFLIS()
    {
        isFLISActive = !isFLISActive;
        Debug.Log("is FLIS active? " + isFLISActive);
    }

    public bool IsFLISOn()
    {
        return isFLISActive;
    }

    public bool IsLevelSetSpecially()
    {
        return isLevelSet;
    }

    public void ResetSpecial()
    {
        isLevelSet = false;
    }

    public void SetSpecialLevels(int A, int B, int C)
    {
        levelA = A;
        levelB = B;
        levelC = C;
        isLevelSet = true;
    }

    public int GetLevelA()
    {
        return levelA;
    }

    public int GetLevelB()
    {
        return levelB;
    }

    public int GetLevelC()
    {
        return levelC;
    }
}
