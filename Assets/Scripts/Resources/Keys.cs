using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

public class Keys : MonoBehaviour {

    //public static string Api_Web_Key = "https://towi.nyx.mx:2443/";
    //public static string Api_Web_Key = "http://35.232.245.131/";
    public static string Api_Web_Key = "https://towi-testing.nyx.mx/";
    public static string Try_Connection_Key = "api/levels/connection/";
    public static string First_Try = "frstTry";
    public static string Level_Of_Try = "lvlFTry";

    public static string Buy_IAP = "BuIAP";
    public const int Number_Of_Games = 6;

    public static string Active_User_Key = "activeUser";
    public static string Purchase_Key = "purchased";
    public static string Subscription_Purchased_Key = "subscriptionPurchased";
    public static string Trial_Day_Key = "TrialDay";
    public static string Flash_Probe_Num = "flash";
    public static string Easy_Evaluation = "evaluation";
    public static string Play_Anim = "PA";
    public static string Emergency_Save = "Em";
    public static string Version_Last_Season = "VeSe";
    public static string Logged_In = "Logeds";
    public static string Logged_Session = "logedses";

    public static string Last_Play_Time = "ltTime";
    public static string Last_Fetch_Time = "lsFtT";
    public static string Last_Time_Were = "lsWere";

    //Games Strings
    public static readonly string[] Game_Names = new string[7]{
        "ArbolMusical",
        "ArenaMagica",
        "Tesoro",
        "DondeQuedoLaBolita",
        "Rio",
        "JuegoDeSombras",
        "Helados"
    };

    //Lava Game
    public static string Lava_Difficulty = "LaDi";
    public static string Lava_Level = "LaLe";
    public static string Lava_First = "LaFi";

    //Bird Game
    public static string Bird_Difficulty = "BiDi";
    public static string Bird_Level = "BiLe";
    public static string Bird_First = "BiFi";

    //River Game
    public static string River_Difficulty = "RiDi";
    public static string River_Level = "RiLe";
    public static string River_First = "RiFi";

    //Monkey Game
    public static string Monkey_Difficulty = "MoDi";
    public static string Monkey_Level = "MoLe";
    public static string Monkey_First = "MoFi";

    //Treasure Game
    public static string Treasure_Difficulty = "TeDi";
    public static string Treasure_Level = "TeLe";
    public static string Treasure_First = "TeFi";

    //Sand Game
    public static string Sand_General_Level_Int = "SaGe";
    public static string Sand_Fill_Level_Int = "SaFi";
    public static string Sand_Complete_Level_Int = "SaCo";
    public static string Sand_Identify_Level_Int = "SaId";
    public static string Sand_First = "SaFi";

    //Icecream Game
    public static string Icecream_Level = "IcLe";
    public static string Icecream_First = "IcFi";

    //Lenguages
    public static string DeviceLenguage = "dvl";
    public static string Language_English = "en";
    public static string Language_Spanish = "es";
    public static string Selected_Language = "sln";

    //Funnel
    public static string Funnel_Games = "funGam";

    //Data Save
    public const string Game_To_Save = "_game_data.json";
    public const string Evaluation_To_Save = "_evaluation_data.json";
    public const string Evaluations_Saved = "evaSav";
    public const string Games_Saved = "gamSav";
}
