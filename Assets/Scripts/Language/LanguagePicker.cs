using UnityEngine;

public class LanguagePicker : MonoBehaviour
{
    public static string BasicTextRoute()
    {
        string baseLenguage = "";

        if (PlayerPrefs.GetInt(Keys.DeviceLenguage) == 0)
        {
            if (Application.systemLanguage == SystemLanguage.Spanish)
            {
                baseLenguage = Keys.Language_Spanish;
            }
            else
            {
                baseLenguage = Keys.Language_English;
            }
        }
        else
        {
            Languages selectedLanguage = (Languages)PlayerPrefs.GetInt(Keys.Selected_Language);
            if (selectedLanguage == Languages.Spanish)
            {
                baseLenguage = Keys.Language_Spanish;
            }
            else
            {
                baseLenguage = Keys.Language_English;
            }
        }
        return $"Texts/{baseLenguage}/";
    }

    public static string BasicAudioRoute()
    {
        string baseLenguage = "";

        if (PlayerPrefs.GetInt(Keys.DeviceLenguage) == 0)
        {
            if (Application.systemLanguage == SystemLanguage.Spanish)
            {
                baseLenguage = Keys.Language_Spanish;
            }
            else
            {
                baseLenguage = Keys.Language_English;
            }
        }
        else
        {
            Languages selectedLanguage = (Languages)PlayerPrefs.GetInt(Keys.Selected_Language);
            if (selectedLanguage == Languages.Spanish)
            {
                baseLenguage = Keys.Language_Spanish;
            }
            else
            {
                baseLenguage = Keys.Language_English;
            }
        }
        return $"Audios/{baseLenguage}/";
    }

    public enum Languages { English, Spanish};


}
