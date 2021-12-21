using System.Collections;
using System.Collections.Generic;

public class TowiDictionary
{
    public static Dictionary<string, int> AvatarNames = new Dictionary<string, int> { { "", 0 }, { "koala", 0 }, { "tucan", 1 }, { "mono", 2 }, { "jaguar", 3 }, { "tortuga", 4 } };

    public static Dictionary<string, string> CharacterNames = new Dictionary<string, string> { { "koala", "TOWI" }, { "tucan", "TUKI" }, { "mono", "MOKI" }, { "jaguar", "KALI" }, { "tortuga", "TELO" } };

    public static Dictionary<string, string> ColorHexs = new Dictionary<string, string> { { "activeGreen", "#3665F1" }, { "deactivated", "#36bdc8" }, {"activeOrange", "#E89B10" }, {"activeYellow", "#E89B10" } };
}
