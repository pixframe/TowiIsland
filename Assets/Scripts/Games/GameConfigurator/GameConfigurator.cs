using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfigurator
{
    /*for more information of how this works check the manual of traning*/
    /// <summary>
    /// Here we will calculate the number of the nest and birds to be used, and the banks of sounds were the sounds come from.
    /// The rules of here are 9 levels in the first difficulty and gets up by three every difficulty, have 4 difficulties
    /// </summary>
    /// <param name="difficulty"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static List<int> SoundThreeConfig(int difficulty, int level)
    {
        //nest are easy calculated the maximum number of nest are 5
        int nest = difficulty + 2;

        //Thi sis used to determine the number of birds that will be in an assay
        int levelAcomodation = level % 3;
        int birds;

        switch (levelAcomodation)
        {
            case 0:
                birds = Random.Range(1, 3);
                break;
            case 1:
                birds = Random.Range(3, 5);
                break;
            case 2:
                birds = 5;
                break;
            default:
                birds = 5;
                break;
        }

        //We create a sort of clssification were every difficult level its divided by 3 
        //to determine from wich bank the birds will get teir sounds
        float bank = level / 3;

        int bankForGet;

        switch (difficulty)
        {
            case 0:
                if (bank < 1)
                {
                    bankForGet = 0;
                }
                else if (bank >= 1 && bank < 2)
                {
                    bankForGet = 3;
                }
                else
                {
                    bankForGet = 5;
                }
                break;
            case 1:
                if (bank < 1)
                {
                    bankForGet = 0;
                }
                else if (bank >= 1 && bank < 2)
                {
                    bankForGet = 2;
                }
                else if (bank >= 2 && bank < 3)
                {
                    bankForGet = 3;
                }
                else
                {
                    bankForGet = 5;
                }
                break;
            case 2:
                if (bank < 1)
                {
                    bankForGet = 0;
                }
                else if (bank >= 1 && bank < 2)
                {
                    bankForGet = 2;
                }
                else if (bank >= 2 && bank < 3)
                {
                    bankForGet = 3;
                }
                else if (bank >= 3 && bank < 4)
                {
                    bankForGet = 4;
                }
                else
                {
                    bankForGet = 5;
                }
                break;
            case 3:
                if (bank < 1)
                {
                    bankForGet = 0;
                }
                else if (bank >= 1 && bank < 2)
                {
                    bankForGet = 1;
                }
                else if (bank >= 2 && bank < 3)
                {
                    bankForGet = 2;
                }
                else if (bank >= 3 && bank < 4)
                {
                    bankForGet = 3;
                }
                else if (bank >= 4 && bank < 5)
                {
                    bankForGet = 4;
                }
                else
                {
                    bankForGet = 5;
                }
                break;
            default:
                bankForGet = 0;
                break;
        }

        return new List<int>() { nest, birds, bankForGet };
    }

    /// <summary>
    /// return the amount of monkeys to show, the time and movements the monkeys will have, and how many objects they will lokong for 
    /// </summary>
    /// <param name="difficulty"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static int[] MonkeyGameConfig(int difficulty, int level)
    {
        //this will set the number of monkeys
        int monkeys = 0;
        if (level < 10)
        {
            monkeys = 3;
        }
        else if (level >= 10 && level < 20)
        {
            monkeys = 4;
        }
        else if (level >= 20) {
            monkeys = 5;
        }

        //this will set the time of the movements
        int[] times = new int[] { 10, 12, 14, 16, 18 };
        int time = times[level % 5];

        //this will set the number of movements in the game
        int[] movements = new int[] { 5, 6, 7, 8, 10, 7, 8, 9, 10, 12 };
        int numOfMovements = movements[level % 10];

        int find = 1;
        if (difficulty > 0)
        {
            find = level % 10;
            if (find < 5)
            {
                find = 1;
            }
            else
            {
                find = 2;
            }
        }

        return new int[] { monkeys, time, numOfMovements, find };
    }

    public static int LavaGameConfig(int level) {
        int product = level % 9;
        int movementData = 0;
        if (product < 3)
        {
            movementData = 0;
        }
        else if (product >= 3 && product < 6)
        {
            movementData = 1;
        }
        else
        {
            movementData = 2;
        }
        return movementData;
    }

    /// <summary>
    /// Returns the amount of targets, distractors, and hide objects
    /// </summary>
    /// <param name="difficulty"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static List<int> TreasureGameConfigSimple(int difficulty, int level) {
        List<int> returner = new List<int>();
        int objects = difficulty + 3;
        int distractors = difficulty;
        int hide = 0;
        switch (difficulty) {
            case 0:
                hide = 0;
                break;
            case 1:
                hide = 1;
                break;
            case 2:
                if (level < 3)
                {
                    hide = 1;
                }
                else
                {
                    hide = 2;
                }
                break;
            case 3:
                hide = 2;
                break;
            case 4:
                hide = 3;
                break;
            default:
                if (level < 3)
                {
                    hide = 3;
                }
                else
                {
                    hide = 4;
                }
                break;
        }
        returner = new List<int> { objects, distractors, hide };
        return returner;
    }

    /// <summary>
    /// return the type of qualifications the objects will have
    /// </summary>
    /// <param name="difficulty"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static List<List<int>> TreasureGameConfigDouble(int difficulty, int level) {
        List<int> li0 = new List<int>();
        List<int> li1 = new List<int>();
        List<int> li2 = new List<int>();
        List<int> li3 = new List<int>();
        List<int> li4 = new List<int>();
        List<List<int>> retuner = new List<List<int>>();
        switch (difficulty)
        {
            case 0:
                switch (level)
                {
                    case 0:
                        li0 = new List<int> { 0, 0 };
                        li1 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1 };
                        return retuner;
                    case 1:
                        li0 = new List<int> { 0, 1 };
                        li1 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1 };
                        return retuner;
                    case 2:
                        li0 = new List<int> { 0, 1, 2 };
                        retuner = new List<List<int>> { li0 };
                        return retuner;
                    case 3:
                        li0 = new List<int> { 0, 0 };
                        li1 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1 };
                        return retuner;
                    case 4:
                        li0 = new List<int> { 0, 1 };
                        li1 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1 };
                        return retuner;
                    default:
                        li0 = new List<int> { 0 };
                        li1 = new List<int> { 0 };
                        li2 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1, li2 };
                        return retuner;
                }
            case 1:
                switch (level)
                {
                    case 0:
                        li0 = new List<int> { 0, 0, 0 };
                        li1 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1 };
                        return retuner;
                    case 1:
                        li0 = new List<int> { 0, 0 };
                        li1 = new List<int> { 0, 0 };
                        retuner = new List<List<int>> { li0, li1 };
                        return retuner;
                    case 2:
                        li0 = new List<int> { 0, 0 };
                        li1 = new List<int> { 0, 1 };
                        retuner = new List<List<int>> { li0, li1 };
                        return retuner;
                    case 3:
                        li0 = new List<int> { 0, 1 };
                        li1 = new List<int> { 0, 1 };
                        retuner = new List<List<int>> { li0, li1 };
                        return retuner;
                    case 4:
                        li0 = new List<int> { 0, 0 };
                        li1 = new List<int> { 0 };
                        li2 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1, li2 };
                        return retuner;
                    default:
                        li0 = new List<int> { 0, 1 };
                        li1 = new List<int> { 0 };
                        li2 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1, li2 };
                        return retuner;
                }
            case 2:
                switch (level)
                {
                    case 0:
                        li0 = new List<int> { 0, 0, 1, 2 };
                        li1 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1 };
                        return retuner;
                    case 1:
                        li0 = new List<int> { 0, 0, 1 };
                        li1 = new List<int> { 0 };
                        li2 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1, li2 };
                        return retuner;
                    case 2:
                        li0 = new List<int> { 0, 1, 2 };
                        li1 = new List<int> { 0 };
                        li2 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1, li2 };
                        return retuner;
                    case 3:
                        li0 = new List<int> { 0, 0 };
                        li1 = new List<int> { 0 };
                        li2 = new List<int> { 0 };
                        li3 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1, li2, li3 };
                        return retuner;
                    case 4:
                        li0 = new List<int> { 0, 0 };
                        li1 = new List<int> { 0 };
                        li2 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1, li2 };
                        return retuner;
                    default:
                        li0 = new List<int> { 0, 1 };
                        li1 = new List<int> { 0 };
                        li2 = new List<int> { 0 };
                        li3 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1, li2, li3 };
                        return retuner;
                }
            case 3:
                switch (level)
                {
                    case 0:
                        li0 = new List<int> { 0, 0, 1, 2 };
                        li1 = new List<int> { 0 };
                        li2 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1, li2 };
                        return retuner;
                    case 1:
                        li0 = new List<int> { 0, 0, 1, 2 };
                        li1 = new List<int> { 0 };
                        li2 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1, li2 };
                        return retuner;
                    case 2:
                        li0 = new List<int> { 0, 0 };
                        li1 = new List<int> { 0, 0 };
                        li2 = new List<int> { 0 };
                        li3 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1, li2, li3 };
                        return retuner;
                    case 3:
                        li0 = new List<int> { 0, 0 };
                        li1 = new List<int> { 0, 1 };
                        li2 = new List<int> { 0, 1 };
                        retuner = new List<List<int>> { li0, li1, li2 };
                        return retuner;
                    case 4:
                        li0 = new List<int> { 0, 0 };
                        li1 = new List<int> { 0, 1 };
                        li2 = new List<int> { 0 };
                        li3 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1, li2, li3 };
                        return retuner;
                    default:
                        li0 = new List<int> { 0, 0 };
                        li1 = new List<int> { 0, 1 };
                        li2 = new List<int> { 0 };
                        li3 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1, li2, li3 };
                        return retuner;
                }
            case 4:
                switch (level)
                {
                    case 0:
                        li0 = new List<int> { 0, 0, 1, 1 };
                        li1 = new List<int> { 0, 0 };
                        li2 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1, li2 };
                        return retuner;
                    case 1:
                        li0 = new List<int> { 0, 0, 0 };
                        li1 = new List<int> { 0, 0 };
                        li2 = new List<int> { 0, 1 };
                        retuner = new List<List<int>> { li0, li1, li2 };
                        return retuner;
                    case 2:
                        li0 = new List<int> { 0, 0, 0 };
                        li1 = new List<int> { 0, 1 };
                        li2 = new List<int> { 0, 1 };
                        retuner = new List<List<int>> { li0, li1, li2 };
                        return retuner;
                    case 3:
                        li0 = new List<int> { 0, 0, 0 };
                        li1 = new List<int> { 0, 1 };
                        li2 = new List<int> { 0 };
                        li3 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1, li2, li3 };
                        return retuner;
                    case 4:
                        li0 = new List<int> { 0, 0, 1 };
                        li1 = new List<int> { 0, 1 };
                        li2 = new List<int> { 0 };
                        li3 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1, li2, li3 };
                        return retuner;
                    default:
                        li0 = new List<int> { 0, 0 };
                        li1 = new List<int> { 0, 1 };
                        li2 = new List<int> { 0, 1 };
                        li3 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1, li2, li3 };
                        return retuner;
                }
            default:
                switch (level)
                {
                    case 0:
                        li0 = new List<int> { 0, 0, 1, 1 };
                        li1 = new List<int> { 0, 1 };
                        li2 = new List<int> { 0, 1 };
                        retuner = new List<List<int>> { li0, li1, li2 };
                        return retuner;
                    case 1:
                        li0 = new List<int> { 0, 0, 1 };
                        li1 = new List<int> { 0, 0, 1 };
                        li2 = new List<int> { 0, 1 };
                        retuner = new List<List<int>> { li0, li1, li2 };
                        return retuner;
                    case 2:
                        li0 = new List<int> { 0, 0, 1, 1 };
                        li1 = new List<int> { 0, 1 };
                        li2 = new List<int> { 0 };
                        li3 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1, li2, li3 };
                        return retuner;
                    case 3:
                        li0 = new List<int> { 0, 0 };
                        li1 = new List<int> { 0, 0 };
                        li2 = new List<int> { 0, 1 };
                        li3 = new List<int> { 0, 1 };
                        retuner = new List<List<int>> { li0, li1, li2, li3 };
                        return retuner;
                    case 4:
                        li0 = new List<int> { 0, 0, 1 };
                        li1 = new List<int> { 0, 1 };
                        li2 = new List<int> { 0 };
                        li3 = new List<int> { 0 };
                        li4 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1, li2, li3, li4 };
                        return retuner;
                    default:
                        li0 = new List<int> { 0, 0 };
                        li1 = new List<int> { 0, 1 };
                        li2 = new List<int> { 0, 1 };
                        li3 = new List<int> { 0 };
                        li4 = new List<int> { 0 };
                        retuner = new List<List<int>> { li0, li1, li2, li3, li4 };
                        return retuner;
                }
        }
    }

    /// <summary>
    /// Return direction of ordering, speed of sorting, and conditions
    /// </summary>
    /// <param name="difficulty"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static int[] RiverConfig(int difficulty, int level)
    {
        //This will set the direction of ordering the objects if its 0 refers a normal handel if its one referes a reverse 
        int direction;
        //this will set the speed of movement of the objects insede the river
        int speed;
        int[] directionList;
        int[] speedList;
        //this will handle the conditions of the objects that will be traeted differently
        //the conditions are 0 specific, 1 after ,2 if
        int[] conditionOne;
        //this will set all the condition of a second objects if its needed to set
        int[] conditionTwo;
        //This will handle what actions should do whit specific objects
        //The conditions are 0 let it go, 1 reverse
        int[] actionsToDoOne;

        if (difficulty == 0)
        {
            directionList = new int[] { 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1 };
            speedList = new int[] { 3, 3, 3, 3, 3, 3, 2, 2, 3, 3, 2, 2, 3, 3, 2, 1, 3, 2 };
        }
        else
        {
            directionList = new int[] { 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1 };
            speedList = new int[] { 2, 1, 3, 2, 2, 1, 3, 2, 2, 1, 3, 2, 2, 1, 3, 2, 1, 1 };
        }

        direction = directionList[level];
        speed = speedList[level];

        if (difficulty < 1 && level < 14)
        {
            if (level == 0 || level == 3)
            {
                return new int[] { direction, speed };
            }
            else
            {
                conditionOne = new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2 };
                actionsToDoOne = new int[] { 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 };
                return new int[] { direction, speed, conditionOne[level], actionsToDoOne[level] };
            }
        }
        else
        {
            int action1 = 0;
            int acttion2 = 1;

            if (difficulty < 1)
            {
                int condition1 = 0;
                int condition2 = 0;
                return new int[] { direction, speed, condition1, action1, condition2, acttion2 };
            }
            else
            {
                conditionOne = new int[] { 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 1, 1, 1, 1, 2, 2 };
                conditionTwo = new int[] { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2 };
                return new int[] { direction, speed, conditionOne[level], action1, conditionTwo[level], acttion2 };
            }
        }

    }

    public static int[] SandConfig(int level)
    {
        /*the categories are ordered by hability to develop 
        0 = motor Habilities
        1 = identify Habilities
        2 = clousure Habilities
        3 = random Hability
        4 = deficit hability
         */

        if (level < 4)
        {
            return new int[] { 0, 0, 0, 3, 3 };
        }
        if (level < 8)
        {
            return new int[] { 0, 0, 1, 3, 3 };
        }
        if (level < 12)
        {
            return new int[] { 0, 0, 1, 2, 3, 3 };
        }
        if (level < 16)
        {
            return new int[] { 0, 0, 1, 2, 4 };
        }

        return new int[] { 0, 0, 0, 3, 3 };
    }

}
