using UnityEngine;
using System.Collections;
using System;

public class LanguageLoader : MonoBehaviour {

	public String[] levelStrings;
	public GameSaveLoad.game gameKey;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadGameLanguage(string lang)
	{
		TextAsset textObj = null;
		switch(gameKey)
		{
			case GameSaveLoad.game.login:
				textObj = (TextAsset)Resources.Load("Language/"+lang+"_login", typeof(TextAsset));
				levelStrings = textObj.text.Split(new string[] {"\n","\r\n"}, StringSplitOptions.RemoveEmptyEntries);
				for(int i=0;i<levelStrings.Length;i++)
				{
					levelStrings[i]=levelStrings[i].Replace("\\n","\n");
				}
			break;
			case GameSaveLoad.game.jungleDrawing:
				textObj = (TextAsset)Resources.Load("Language/"+lang+"_arenaMagica", typeof(TextAsset));
				levelStrings = textObj.text.Split(new string[] {"\n","\r\n"}, StringSplitOptions.RemoveEmptyEntries);
				for(int i=0;i<levelStrings.Length;i++)
				{
					levelStrings[i]=levelStrings[i].Replace("\\n","\n");
				}
			break;
			case GameSaveLoad.game.soundTree:
				textObj = (TextAsset)Resources.Load("Language/"+lang+"_arbolMusical", typeof(TextAsset));
				levelStrings = textObj.text.Split(new string[] {"\n","\r\n"}, StringSplitOptions.RemoveEmptyEntries);
				for(int i=0;i<levelStrings.Length;i++)
				{
					levelStrings[i]=levelStrings[i].Replace("\\n","\n");
				}
			break;
			case GameSaveLoad.game.river:
				textObj = (TextAsset)Resources.Load("Language/"+lang+"_rio", typeof(TextAsset));
				levelStrings = textObj.text.Split(new string[] {"\n","\r\n"}, StringSplitOptions.RemoveEmptyEntries);
				for(int i=0;i<levelStrings.Length;i++)
				{
					levelStrings[i]=levelStrings[i].Replace("\\n","\n");
				}
			break;
			case GameSaveLoad.game.whereIsTheBall:
				textObj = (TextAsset)Resources.Load("Language/"+lang+"_dondeQuedoLaBolita", typeof(TextAsset));
				levelStrings = textObj.text.Split(new string[] {"\n","\r\n"}, StringSplitOptions.RemoveEmptyEntries);
				for(int i=0;i<levelStrings.Length;i++)
				{
					levelStrings[i]=levelStrings[i].Replace("\\n","\n");
				}
			break;
			case GameSaveLoad.game.shadowGame:
				textObj = (TextAsset)Resources.Load("Language/"+lang+"_sombras", typeof(TextAsset));
				levelStrings = textObj.text.Split(new string[] {"\n","\r\n"}, StringSplitOptions.RemoveEmptyEntries);
				for(int i=0;i<levelStrings.Length;i++)
				{
					levelStrings[i]=levelStrings[i].Replace("\\n","\n");
				}
			break;
			case GameSaveLoad.game.treasure:
				textObj = (TextAsset)Resources.Load("Language/"+lang+"_recoleccionTesoro", typeof(TextAsset));
				levelStrings = textObj.text.Split(new string[] {"\n","\r\n"}, StringSplitOptions.RemoveEmptyEntries);
				for(int i=0;i<levelStrings.Length;i++)
				{
					levelStrings[i]=levelStrings[i].Replace("\\n","\n");
				}
			break;
			case GameSaveLoad.game.introGames:
				textObj = (TextAsset)Resources.Load("Language/"+lang+"_introGames", typeof(TextAsset));
				levelStrings = textObj.text.Split(new string[] {"\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);
				for(int i = 0; i < levelStrings.Length; i++)
				{
					levelStrings[i]=levelStrings[i].Replace("\\n", "\n");
				}
			break;
			case GameSaveLoad.game.island:
				textObj = (TextAsset)Resources.Load("Language/"+lang+"_archipielago", typeof(TextAsset));
				levelStrings = textObj.text.Split(new string[] {"\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);
				for(int i = 0; i < levelStrings.Length; i++)
				{
					levelStrings[i]=levelStrings[i].Replace("\\n", "\n");
				}
			break;
			case GameSaveLoad.game.store:
				textObj = (TextAsset)Resources.Load("Language/"+lang+"_tienda", typeof(TextAsset));
				levelStrings = textObj.text.Split(new string[] {"\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);
				for(int i = 0; i < levelStrings.Length; i++)
				{
					levelStrings[i]=levelStrings[i].Replace("\\n", "\n");
				}
			break;
			case GameSaveLoad.game.age:
				textObj = (TextAsset)Resources.Load("Language/"+lang+"_edad", typeof(TextAsset));
				levelStrings = textObj.text.Split(new string[] {"\n","\r\n"}, StringSplitOptions.RemoveEmptyEntries);
				for(int i=0;i<levelStrings.Length;i++)
				{
					levelStrings[i]=levelStrings[i].Replace("\\n","\n");
				}
			break;
			case GameSaveLoad.game.selection:
				textObj = (TextAsset)Resources.Load("Language/"+lang+"_seleccion", typeof(TextAsset));
				levelStrings = textObj.text.Split(new string[] {"\n","\r\n"}, StringSplitOptions.RemoveEmptyEntries);
				for(int i=0;i<levelStrings.Length;i++)
				{
					levelStrings[i]=levelStrings[i].Replace("\\n","\n");
				}
			break;
		}
	}
}
