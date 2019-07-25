using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NeedAudioMenu : MonoBehaviour
{
    public void SetHideButtonFunction(UnityEngine.Events.UnityAction action)
    {
        var hideButton = transform.Find("Hide Button").GetComponent<Button>();
        var textOfObject = transform.Find("Need Audio Text").GetComponent<Text>();
        textOfObject.text = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Menus/NeedAudio").text;
        hideButton.onClick.AddListener(action);
    }
}
