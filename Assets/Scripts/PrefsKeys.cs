using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefsKeys {

    static string NextScene = null;

    public static string GetNextScene() {
        return NextScene;
    }

    public static void SetNextScene(string nextScene) {
        NextScene = nextScene;
    }
}
