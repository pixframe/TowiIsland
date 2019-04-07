using UnityEngine;
using System.Collections;
using Boomlagoon.JSON;
using System;

public class LevelSync : MonoBehaviour {
	public string postURL;
	SessionManager sessionMng;
	Interface main;
	public int TopArbolMusical=5;
	public int TopRio=3;
	public int TopArenaMagica=4;
	public int TopMonkey=5;
	public int TopSombras=5;
	public int TopTesoro=2;
	// Use this for initialization
	void Start ()
	{
		sessionMng = GetComponent<SessionManager> ();
		main = GetComponent<Interface> ();
		SyncLevels ();
	}
	void SyncLevels()
	{
		if(!Login.local)
			StartCoroutine(PostSyncLevels());
	}
	IEnumerator PostSyncLevels()
	{
		string post_url = postURL;
		DateTime today = DateTime.Now;

		WWWForm form = new WWWForm();
        form.AddField("userKey", sessionMng.activeKid.userkey);
		form.AddField("cid", sessionMng.activeKid.id);
		form.AddField("date", String.Format ("{0:0000}-{1:00}-{2:00}",today.Year, today.Month, today.Day));
		//WWW hs_post = new WWW(post_url);
		WWW hs_post = new WWW(post_url,form);
		yield return hs_post;
		
		if (hs_post.error == null) {
			JSONObject response= JSONObject.Parse(hs_post.text);
			Debug.Log(response["code"].Str);
			if(response["code"].Str=="200"){

				if(sessionMng.activeKid.dontSyncArbolMusical==0)
				{
					sessionMng.activeKid.birdsDifficulty=int.Parse(response["arbolMusicalLevel"].Str);
					sessionMng.activeKid.birdsLevel=int.Parse(response["arbolMusicalSublevel"].Str);
				}
				if(sessionMng.activeKid.dontSyncRio==0)
				{
					sessionMng.activeKid.riverDifficulty=int.Parse(response["rioLevel"].Str);
					sessionMng.activeKid.riverLevel=int.Parse(response["rioSublevel"].Str);
				}
				if(sessionMng.activeKid.dontSyncArenaMagica==0)
				{
					sessionMng.activeKid.sandDifficulty=int.Parse(response["arenaMagicaLevel"].Str);
					sessionMng.activeKid.sandLevel=int.Parse(response["arenaMagicaSublevel"].Str);
				}
				if(sessionMng.activeKid.dontSyncDondeQuedoLaBolita==0)
				{
					sessionMng.activeKid.monkeyDifficulty=int.Parse(response["monkeyLevel"].Str);
					sessionMng.activeKid.monkeyLevel=int.Parse(response["monkeySublevel"].Str);
				}
				if(sessionMng.activeKid.dontSyncSombras==0)
				{
					sessionMng.activeKid.lavaDifficulty=int.Parse(response["sombrasLevel"].Str);
					sessionMng.activeKid.lavaLevel=int.Parse(response["sombrasSublevel"].Str);
				}
				if(sessionMng.activeKid.dontSyncTesoro==0)
				{
					sessionMng.activeKid.treasureDifficulty=int.Parse(response["tesoroLevel"].Str);
					sessionMng.activeKid.treasureLevel=int.Parse(response["tesoroSublevel"].Str);
				}

				sessionMng.activeKid.playedBird=int.Parse(response["arbolToday"].Str); 
				sessionMng.activeKid.playedRiver=int.Parse(response["rioToday"].Str);	
				sessionMng.activeKid.playedSand=int.Parse(response["arenaToday"].Str); 
				sessionMng.activeKid.playedMonkey=int.Parse(response["monkeyToday"].Str); 
				sessionMng.activeKid.playedLava=int.Parse(response["sombrasToday"].Str); 
				sessionMng.activeKid.playedTreasure=int.Parse(response["tesoroToday"].Str);

				if(sessionMng.activeKid.playedBird>=TopArbolMusical)
					sessionMng.activeKid.blockedArbolMusical=1;
				if(sessionMng.activeKid.playedRiver>=TopRio)
					sessionMng.activeKid.blockedRio=1;
				if(sessionMng.activeKid.playedSand>=TopArenaMagica)
					sessionMng.activeKid.blockedArenaMagica=1;
				if(sessionMng.activeKid.playedMonkey>=TopMonkey)
					sessionMng.activeKid.blockedDondeQuedoLaBolita=1;
				if(sessionMng.activeKid.playedLava>=TopSombras)
					sessionMng.activeKid.blockedSombras=1;
				if(sessionMng.activeKid.playedTreasure>=TopTesoro)
					sessionMng.activeKid.blockedTesoro=1;

				sessionMng.SaveSession();

				main.UpdateBlocked();
			}
			Debug.Log (hs_post.text);
		} else {
			Debug.Log(hs_post.error);
		}
	}
}
