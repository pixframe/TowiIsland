using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OfflineManager : MonoBehaviour
{
    

    public static List<string> Create_Levels()
    {
        var namesOfGame = new List<string> { Keys.Bird_Game_Name, Keys.Lava_Game_Name, Keys.Monkey_Game_Name, Keys.River_Game_Name, Keys.Sand_Game_Name, Keys.Treasure_Game_Name };

        int levelsToPlay = Random.Range(2, 5);

        List<string> levelsToCreate = new List<string>();
        for (int i = 0; i < levelsToPlay; i++)
        {
            int randomMission = Random.Range(0, namesOfGame.Count);
            levelsToCreate.Add(namesOfGame[randomMission]);
            namesOfGame.RemoveAt(randomMission);
        }

        return levelsToCreate;
    }
}
