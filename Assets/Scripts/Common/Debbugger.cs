using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debbugger : MonoBehaviour {

    Text debuggerText;
    string final = "";

    public void Debbugg(string textToDebug, string color)
    {
        if (debuggerText == null)
        {
            debuggerText = GetComponent<Text>();
        }
        final += "<color=" + color + ">" + textToDebug + "</color>\n";
        debuggerText.text = final;
    }
}
