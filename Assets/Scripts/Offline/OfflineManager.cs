using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class OfflineManager : MonoBehaviour
{
    

    public static List<int> Create_Levels()
    {
        List<int> numberOfGame = new List<int>();
        for (int i = 0; i < Keys.Number_Of_Games; i++)
        {
            int x = i;
            numberOfGame.Add(x);
        }

        int levelsToPlay = UnityEngine.Random.Range(2, 5);

        List<int> levelsToCreate = new List<int>();
        for (int i = 0; i < levelsToPlay; i++)
        {
            int randomMission = UnityEngine.Random.Range(0, numberOfGame.Count);
            levelsToCreate.Add(numberOfGame[randomMission]);
            numberOfGame.RemoveAt(randomMission);
        }

        PlayerPrefs.SetString(Keys.Last_Play_Time, DateTime.Now.ToString(System.Globalization.DateTimeFormatInfo.InvariantInfo));

        return levelsToCreate;
    }
}
