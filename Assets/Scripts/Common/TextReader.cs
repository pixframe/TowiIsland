using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TextReader : MonoBehaviour {

    public static string[] commonStrings;
    public static string[] addableStrings;
    public static string[] beforeStrings;

    public static string[] TextsToShow(TextAsset asset)
    {
        String[] listToReturn;

        listToReturn = asset.text.Split(new string [] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < listToReturn.Length; i++)
        {
            listToReturn[i] = listToReturn[i].Replace("\\n", "\n");
        }

        return listToReturn;
    }

    public static void FillCommon(TextAsset asset)
    {
        if (commonStrings == null)
        {
            commonStrings = asset.text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }
    }

    public static void FillAddables(TextAsset asset)
    {
        if (addableStrings == null)
        {
            addableStrings = asset.text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }
    }

    public static void FillBefore(TextAsset asset)
    {
        if (beforeStrings == null)
        {
            beforeStrings = asset.text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }
    }

    public static string AddStrings(int index, string firstString)
    {
        //here we will add some strings to reuse some reusable strings
        // 0  vamos a intentarlo, 1 volvamos a intentarlo, 2 muy bien
        string lineToReturn = (firstString + "\n" + addableStrings[index]);

        return lineToReturn;
    }

    public static string AddBeforeStrings(int index, string firstString)
    {
        //here will add some string before the original string
        string lineToReturn = (beforeStrings[index] + " " + firstString);

        return lineToReturn;
    }
}
