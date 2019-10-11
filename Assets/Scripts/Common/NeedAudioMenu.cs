using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NeedAudioMenu : MonoBehaviour
{
    public void SetHideButtonFunction(UnityEngine.Events.UnityAction action)
    {
        var hideButton = transform.Find("Hide Button").GetComponent<Button>();
        var textOfObject = transform.Find("Need Audio Text").GetComponent<TextMeshProUGUI>();
        textOfObject.text = Resources.Load<TextAsset>($"{LanguagePicker.BasicTextRoute()}Menus/NeedAudio").text;
        hideButton.onClick.AddListener(action);
    }
}
