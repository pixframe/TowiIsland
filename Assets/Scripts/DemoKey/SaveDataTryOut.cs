using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Boomlagoon.JSON;
using UnityEngine;
using UnityEngine.UI;

public class SaveDataTryOut : MonoBehaviour {

    public Button saver;
    public InputField field;
    public InputField field2;
    public Text doneIt;
    public Text debuger;
    public Text pathDebug;

    JSONObject item;
    JSONObject item2;

    // Use this for initialization
    void Start()
    {
        saver.onClick.AddListener(SaveData);
        item = new JSONObject();
        item2 = new JSONObject();
    }

    void SaveData()
    {
        SetTextInJSON();
        CreateText();
    }

    void SetTextInJSON()
    {
        item.Add("TextToSave", field.text);
        item2.Add("TextRaw", field2.text);
        Debug.Log("this data is saved" + item.ToString());
    }

    void CreateText()
    {
        doneIt.text = "Done it";
        //path of the file
        string path = Application.persistentDataPath + "/emergencysave.txt";

        doneIt.text = "Done it 2";
        pathDebug.text = path;

        doneIt.text = "Done it 3";

        doneIt.text = "Done it 4";
        //Content of file
        string content = item.ToString();
        string content2 = item2.ToString();
        Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] bytes = texture.EncodeToPNG();
        string content3 = BitConverter.ToString(bytes);
        doneIt.text = "Done it 5";

        File.WriteAllText(path, content);
        File.AppendAllText(path, "\n");
        File.AppendAllText(path, content2);
        Debug.Log(content3);

        doneIt.text = "Done it 6";
        Debug.Log(File.ReadAllText(path));
        debuger.text = File.ReadAllText(path);

        doneIt.text = "Done it 7";
        field.text = null;
    }
}
