using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class TestLogic : MonoBehaviour
{
    List<TestSpawner> spawners;
    public List<RouteResult> results;
    public List<TestScore> userResults;
    public GameObject spawnerRef;
    RoutesManager manager;
    RoutesManager.Route route;
    public ScoreEffects scoreEffects;
    public int found = 0;
    public Texture pearlTexture;
    float scale = 1;
    public Texture backgroundTexture;
    public GUIStyle textHeader;
    public GUIStyle textScore;
    public GUIStyle textFinish;
    public GUIStyle textSync;
    public GUIStyle textPause;
    public GUIStyle textTableHeader;
    public GUIStyle detailButton;
    public GUIStyle startButton;
    public GUIStyle endButton;
    public GUIStyle syncButton;
    public Texture scoreBoxTexture;
    string nameLabel = "";
    string ageLabel = "";
    string sexLabel = "";
    int routeIdx = 1;
    public BotControlScript avatarControl;
    public GameObject avatar;
    Vector3 startingPosition;
    Vector2 scrollPos = Vector2.zero;
    int detailIdx = 0;
    ProgressHandler saveHandler;

    public enum TestStates { Name, Scores, ScoreDetail, Test, Pause, End};

    public TestStates state = TestStates.Scores;
    // Use this for initialization
    void Start()
    {
        scale = (float)Screen.height / (float)768;
        manager = GetComponent<RoutesManager>();
        results = new List<RouteResult>();
        spawners = new List<TestSpawner>();
        userResults = new List<TestScore>();
        startingPosition = avatar.transform.position;
        textHeader.fontSize = (int)((float)textHeader.fontSize * scale);
        textScore.fontSize = (int)((float)textScore.fontSize * scale);
        textFinish.fontSize = (int)((float)textFinish.fontSize * scale);
        detailButton.fontSize = (int)((float)detailButton.fontSize * scale);
        startButton.fontSize = (int)((float)startButton.fontSize * scale);
        endButton.fontSize = (int)((float)endButton.fontSize * scale);
        syncButton.fontSize = (int)((float)syncButton.fontSize * scale);
        textPause.fontSize = (int)((float)textPause.fontSize * scale);
        textTableHeader.fontSize = (int)((float)textTableHeader.fontSize * scale);
        saveHandler = (ProgressHandler)GetComponent(typeof(ProgressHandler));

        LoadSession();
    }

    void LoadRoute(int id)
    {
        route = manager.GetRoute(id);
        spawners.Clear();
        for (int i = 0; i < route.nodes.Count; i++)
        {
            Ray floorTestRay = new Ray(route.nodes[i].position + (Vector3.up * 5), Vector3.down);
            RaycastHit hitInfo;
            Physics.Raycast(floorTestRay, out hitInfo, 20);
            GameObject temp = (GameObject)GameObject.Instantiate(spawnerRef, route.nodes[i].position+ new Vector3(0, hitInfo.point.y, 0), new Quaternion(0, 0, 0, 0));
            TestSpawner tempSpawner = temp.GetComponent<TestSpawner>();
            tempSpawner.order = route.nodes[i].order;
            spawners.Add(tempSpawner);
        }
    }
    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case TestStates.Test:
                if (route == null)
                {
                    LoadRoute(routeIdx);
                }
                if(found>=route.nodes.Count)
                {
                    StopAvatar();
                    userResults.Add(new TestScore(nameLabel, ageLabel, sexLabel, routeIdx, results));
                    SaveSession();
                    results = new List<RouteResult>();

                    found = 0;

                    state = TestStates.End;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (Input.mousePosition.x > Screen.width * 0.85f && Input.mousePosition.y > Screen.height * 0.85f)
                    {
                        StopAvatar();
                        state = TestStates.Pause;
                    }
                }
                break;
            case TestStates.End:
                if(Input.GetMouseButtonUp(0))
                {
                    if(Input.mousePosition.x>Screen.width*0.85f&&Input.mousePosition.y>Screen.height*0.85f)
                    {
                        foreach(TestSpawner spawner in spawners)
                        {
                            spawner.Spawn();
                        }
                        state = TestStates.Scores;
                    }
                }
                break;
        }
    }

    void OnGUI()
    {
        switch(state)
        {
            case TestStates.Scores:
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundTexture);

			if (GUI.Button(new Rect(40 * scale, 40 * scale, 200 * scale, 50 * scale), "Salir", startButton))
			{
				Application.Quit();
			}

			if(GUI.Button(new Rect(Screen.width - 220 * scale, Screen.height - 400 * scale, 200 * scale, 100 * scale), "Nueva prueba", startButton))
                {
                    nameLabel = "";
                    ageLabel = "";
                    sexLabel = "";
                    TestInteraction tempInteraction = avatar.GetComponent<TestInteraction>();
                    tempInteraction.glow.transform.position = new Vector3(-900, -900 - 900);
                    tempInteraction.Reset();
                    avatar.transform.position = startingPosition;
                    avatar.transform.rotation = Quaternion.identity;
                    StopAvatar();
                    state = TestStates.Name;
                }
                if (userResults.Count > 0)
                {
                    GUI.Label(new Rect(Screen.width * 0.5f - 100 * scale, 30 * scale, 200 * scale, 50 * scale), "Resultados", textHeader);
                    scrollPos = GUI.BeginScrollView(new Rect(Screen.width * 0.5f - 400 * scale, 90 * scale, 840 * scale, Screen.height - 200 * scale), scrollPos, new Rect(0, 0, 800 * scale, userResults.Count * 210 * scale));
                    for (int i = 0; i < userResults.Count; i++)
                    {
                        GUI.BeginGroup(new Rect(0, i * 210 * scale, 800 * scale, 200 * scale));
                        GUI.DrawTexture(new Rect(0, 0, 800 * scale, 200 * scale),scoreBoxTexture);
                        GUI.Label(new Rect(20 * scale, 20 * scale, 600 * scale, 30 * scale), userResults[i].name+"-"+ userResults[i].age+"-"+ userResults[i].sex, textScore);
                        if(!userResults[i].synced)
                        {
                            GUI.Label(new Rect(580 * scale, 10 * scale, 200 * scale, 30 * scale), "No sincronizado", textSync);
                        }
                        GUI.Label(new Rect(20 * scale, 70 * scale, 150 * scale, 30 * scale), "Fecha:", textScore);
                        GUI.Label(new Rect(160 * scale, 70 * scale, 400 * scale, 30 * scale), userResults[i].date.ToString(), textScore);
                        GUI.Label(new Rect(20 * scale, 110 * scale, 150 * scale, 30 * scale), "Tiempo de la prueba:", textScore);
                        TimeSpan time = TimeSpan.FromSeconds(userResults[i].totalTime);
                        GUI.Label(new Rect(190 * scale, 125 * scale, 200 * scale, 30 * scale), string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                    time.Hours,
                                    time.Minutes,
                                    time.Seconds), textScore);
                        if(GUI.Button(new Rect(680 * scale, 120 * scale, 100 * scale, 50 * scale), "Detalle", detailButton))
                        {
                            detailIdx = i;
                            state = TestStates.ScoreDetail;
                        }
                        GUI.EndGroup();
                    }
                    GUI.EndScrollView();
                    if (GUI.Button(new Rect(Screen.width*0.5f-100*scale, Screen.height-90*scale, 200 * scale, 60 * scale), "Sincronizar", syncButton))
                    {
                        Sync();
                    }
                }
                else
                {
                    GUI.Label(new Rect(Screen.width*0.5f-100*scale, Screen.height*0.5f-25*scale, 200 * scale, 50 * scale), "No hay resultados almacenados", textHeader);
                }
                break;
            case TestStates.ScoreDetail:
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundTexture);
                if (userResults[detailIdx].results.Count > 0)
                {
                    textHeader.alignment = TextAnchor.MiddleLeft;
                    GUI.Label(new Rect(Screen.width * 0.5f - 280 * scale, 30 * scale, 500 * scale, 50 * scale), "Prueba de "+ userResults[detailIdx] .name + "-" + userResults[detailIdx].age + "-" + userResults[detailIdx].sex + " - Detalle", textHeader);
                    textHeader.alignment = TextAnchor.MiddleCenter;

                    GUI.BeginGroup(new Rect(Screen.width * 0.5f - 400 * scale, 70 * scale, 840 * scale, 60 * scale));
                    GUI.Label(new Rect(20 * scale, 25 * scale, 100 * scale, 30 * scale), "Cono", textTableHeader);
                    GUI.Label(new Rect(120 * scale, 25 * scale, 100 * scale, 30 * scale), "Tiempo", textTableHeader);
                    GUI.Label(new Rect(270 * scale, 25 * scale, 100 * scale, 30 * scale), "Distancia", textTableHeader);
                    GUI.Label(new Rect(430 * scale, 25 * scale, 100 * scale, 30 * scale), "Estado", textTableHeader);
                    GUI.EndGroup();

                    scrollPos = GUI.BeginScrollView(new Rect(Screen.width * 0.5f - 400 * scale, 130 * scale, 840 * scale, Screen.height - 230 * scale), scrollPos, new Rect(0, 0, 800 * scale, userResults[detailIdx].results.Count * 50 * scale));
                    for (int i = 0; i < userResults[detailIdx].results.Count; i++)
                    {
                        GUI.BeginGroup(new Rect(0, i * 50 * scale, 800 * scale, 40 * scale));
                        GUI.DrawTexture(new Rect(0, 0, 800 * scale, 40 * scale), scoreBoxTexture);
                        GUI.Label(new Rect(20 * scale, 10 * scale, 100 * scale, 30 * scale), userResults[detailIdx].results[i].order.ToString(), textScore);
                        GUI.Label(new Rect(120 * scale, 10 * scale, 100 * scale, 30 * scale), userResults[detailIdx].results[i].elapsedTime + "s", textScore);
                        GUI.Label(new Rect(270 * scale, 10 * scale, 100 * scale, 30 * scale), userResults[detailIdx].results[i].distance+"m", textScore);
                        GUI.Label(new Rect(430 * scale, 10 * scale, 200 * scale, 30 * scale), userResults[detailIdx].results[i].currentState == Treasure.state.Empty?"Vacio":"Ocupado", textScore);
                        GUI.EndGroup();
                    }
                    GUI.EndScrollView();
                    if (GUI.Button(new Rect(Screen.width * 0.5f - 400 * scale, 35 * scale, 100 * scale, 40 * scale), "Volver", detailButton))
                    {
                        state = TestStates.Scores;
                    }
                    if (GUI.Button(new Rect(Screen.width * 0.5f - 70 * scale, Screen.height - 80 * scale, 100 * scale, 40 * scale), "Eliminar", endButton))
                    {
                        userResults.RemoveAt(detailIdx);
                        SaveSession();
                        state = TestStates.Scores;
                    }
                }
                else
                {
					if (GUI.Button(new Rect(Screen.width * 0.5f - 400 * scale, 35 * scale, 100 * scale, 40 * scale), "Volver", detailButton))
					{
						state = TestStates.Scores;
					}
                    GUI.Label(new Rect(Screen.width * 0.5f - 100 * scale, Screen.height * 0.5f - 25 * scale, 200 * scale, 50 * scale), "No se agarro ninguna concha en esta prueba", textHeader);
					
					if (GUI.Button(new Rect(Screen.width * 0.5f - 70 * scale, Screen.height - 80 * scale, 100 * scale, 40 * scale), "Eliminar", endButton))
					{
						userResults.RemoveAt(detailIdx);
						SaveSession();
						state = TestStates.Scores;
					}
                }
                break;
            case TestStates.Name:
                textScore.alignment = TextAnchor.MiddleLeft;
                GUI.Label(new Rect(Screen.width * 0.5f - 180 * scale, Screen.height * 0.5f - 110 * scale, 150 * scale, 50 * scale), "Nombre", textScore);
                nameLabel = GUI.TextField(new Rect(Screen.width * 0.5f - 50 * scale, Screen.height * 0.5f - 110 * scale, 200 * scale, 50 * scale), nameLabel);
                GUI.Label(new Rect(Screen.width * 0.5f - 180 * scale, Screen.height * 0.5f - 55 * scale, 150 * scale, 50 * scale), "Edad", textScore);
                ageLabel = GUI.TextField(new Rect(Screen.width * 0.5f - 50 * scale, Screen.height * 0.5f - 55 * scale, 200 * scale, 50 * scale), ageLabel);
                GUI.Label(new Rect(Screen.width * 0.5f - 180 * scale, Screen.height * 0.5f - 0 * scale, 150 * scale, 50 * scale), "Sexo", textScore);
                sexLabel = GUI.TextField(new Rect(Screen.width * 0.5f - 50 * scale, Screen.height * 0.5f - 0 * scale, 200 * scale, 50 * scale), sexLabel);
                textScore.alignment = TextAnchor.UpperLeft;
                if (GUI.Button(new Rect(Screen.width*0.5f+30*scale,Screen.height*0.5f + 60 * scale, 120*scale,50*scale),"Empezar",startButton) && nameLabel != "" && ageLabel != "" && sexLabel != "")
                {
                    avatarControl.overrideMovement = false;
                    state = TestStates.Test;
                }
                break;
            case TestStates.Test:
                GUI.BeginGroup(new Rect(20 * scale, 20 * scale, 500 * scale, 100 * scale));
                for (int i = 0; i < found; i++)
                {
                    GUI.DrawTexture(new Rect(45 * (i % 10) * scale, 45 * (int)(i / 10) * scale, 40 * scale, 40 * scale), pearlTexture);
                }
                GUI.EndGroup();
                break;
            case TestStates.End:
                GUI.Label(new Rect(Screen.width * 0.5f - 100 * scale, Screen.height * 0.5f - 25 * scale, 200 * scale, 50 * scale), "¡Felicidades, terminaste el juego!",textFinish);
                break;
            case TestStates.Pause:
                GUI.Label(new Rect(Screen.width * 0.5f - 100 * scale, Screen.height*0.5f-100*scale, 200 * scale, 50 * scale), "Pausa", textPause);
                if(GUI.Button(new Rect(Screen.width*0.5f-210*scale,Screen.height*0.5f-30*scale,200*scale,60*scale),"Continuar",detailButton))
                {
                    avatarControl.overrideMovement = false;
                    state = TestStates.Test;
                }
                if (GUI.Button(new Rect(Screen.width * 0.5f + 10 * scale, Screen.height * 0.5f - 30 * scale, 200 * scale, 60 * scale), "Terminar", endButton))
                {
                    StopAvatar();
                    userResults.Add(new TestScore(nameLabel, ageLabel, sexLabel, routeIdx, results));
                    SaveSession();
                    results = new List<RouteResult>();
                    found = 0;

                    foreach (TestSpawner spawner in spawners)
                    {
                        spawner.Spawn();
                    }

                    state = TestStates.Scores;
                }
                break;
        }
        
    }

    void StopAvatar()
    {
        avatarControl.overrideMovement = true;
        avatarControl.h = 0;
        avatarControl.v = 0;
    }

    public void PrintResults()
    {
        for (int i = 0; i < results.Count; i++)
        {
            Debug.Log("Order: " + results[i].order + ",State: " + results[i].currentState + ",Time: " + results[i].elapsedTime + ",Distance: " + results[i].distance);
        }
    }

    void SaveLevelProgress(TestScore testResult)
    {
        for(int i = 0; i< testResult.results.Count; i++)
        {
            saveHandler.AddLevelData("cono", testResult.results[i].order);
            saveHandler.AddLevelData("distance", testResult.results[i].distance);
            saveHandler.AddLevelData("time", (int)testResult.results[i].elapsedTime);
            saveHandler.AddLevelData("state", testResult.results[i].currentState== Treasure.state.Empty?"Vacio":"Ocupado");
            saveHandler.AddLevelData("order", i);
            //saveHandler.SetLevel();
        }

        StartCoroutine(SaveProgress(testResult));
    }
    IEnumerator SaveProgress(TestScore testResult)
    {
        saveHandler.createSaveBlockConos("PruebaConos", testResult.name, testResult.age,testResult.sex,testResult.routeIdx,testResult.date);
        saveHandler.AddLevelsToBlock();
        bool success = false;
        yield return StartCoroutine(saveHandler.PostProgressConos(value => success = value));
        testResult.synced = success;
        SaveSession();
        Debug.Log(saveHandler.ToString());
    }

    public void Sync()
    {
        for(int i=0;i<userResults.Count;i++)
        {
            if (!userResults[i].synced)
            {
                SaveLevelProgress(userResults[i]);
            }
        }
    }

    public void SaveSession()
    {
        Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
        BinaryFormatter b = new BinaryFormatter();
        //Create an in memory stream
        MemoryStream m = new MemoryStream();
        //Save the scores
        b.Serialize(m, userResults);
        //Add it to player prefs
        PlayerPrefs.SetString("SearchTest", Convert.ToBase64String(m.GetBuffer()));
    }

    void LoadSession()
    {
        //Get the data
        string data = PlayerPrefs.GetString("SearchTest");
        //If not blank then load it
        if (!string.IsNullOrEmpty(data))
        {
            Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
            //Binary formatter for loading back
            BinaryFormatter b = new BinaryFormatter();
            //Create a memory stream with the data
            MemoryStream m = new MemoryStream(Convert.FromBase64String(data));
            //Load back the scoress
            userResults = (List<TestScore>)b.Deserialize(m);
        }
    }

    [System.Serializable]
    public class RouteResult
    {
        public int order;
        public Treasure.state currentState;
        public float distance;
        public float elapsedTime;
        public RouteResult(int ord, Treasure.state resultState, float t, float d)
        {
            order = ord;
            currentState = resultState;
            distance = d;
            elapsedTime = t;
        }
    }

    [System.Serializable]
    public class TestScore
    {
        public string name;
        public string age;
        public string sex;
        public List<RouteResult> results;
        public int routeIdx;
        public float totalTime;
        public DateTime date;
        public bool synced;

        public TestScore(string name, string age, string sex, int routeIdx, List<RouteResult> results)
        {
            this.name = name;
            this.age = age;
            this.sex = sex;
            this.routeIdx = routeIdx;
            this.results = results;
			if (results.Count > 0)
			{
				totalTime = results[results.Count - 1].elapsedTime;
			}else{
				totalTime = 0;
			}
            
            date = DateTime.Now;
            synced = false;
        }
    }
}
