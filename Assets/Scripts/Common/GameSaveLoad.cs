using UnityEngine; 
using System.Collections; 
using System.Xml; 
using System.Xml.Serialization; 
using System.IO; 
using System.Text;

public class GameSaveLoad: MonoBehaviour
{ 


	// An example where the encoding can be found is at 
	// http://www.eggheadcafe.com/articles/system.xml.xmlserialization.asp 
	// We will just use the KISS method and cheat a little and use 
	// the examples from the web page since they are fully described 
	bool _ShouldSave, _ShouldLoad, _SwitchSave, _SwitchLoad;
	string _FileLocation, _FileName;
	public GameObject _Player;
	public object configuration;
	string _data;
	Vector3 VPosition;
	SessionManager sessionMng;
	public enum game
	{
		treasure,
		soundTree,
		whereIsTheBall,
		jungleDrawing,
		river,
		shadowGame,
		introGames,
		login,
		island,
		store,
		age,
		selection
	};
	game gameFile;
	
	// When the EGO is instansiated the Start will trigger 
	// so we setup our initial values for our local members 
	void Awake ()
	{ 
		// Where we want to save and load to and from 
		_FileLocation = Application.dataPath; 
		sessionMng=GetComponent<SessionManager> ();
		// we need soemthing to store the information into 
//				configuration = new WhereIsTheBallConfiguration(); 
//		CreateTestFile(game.shadowGame);
	}
	
	public void Load (game target)
	{
		TextAsset textAsset;
		switch (target) {
		case game.treasure:
			gameFile = game.treasure;
			//if(sessionMng.activeKid.xmlTesoro!="")
			//{
			//	_data=sessionMng.activeKid.xmlTesoro;
			//}else
			//{
				textAsset = (TextAsset) Resources.Load("ConfigTreasure");  
				_data=textAsset.text;
			//}
			//_FileName = "ConfigTreasure.haq";
			break;
		case game.soundTree:
			gameFile = game.soundTree;
			//if(sessionMng.activeKid.xmlArbolMusical!="")
			//{
			//	_data=sessionMng.activeKid.xmlArbolMusical;
			//}else
			//{
				textAsset = (TextAsset) Resources.Load("ConfigSoundTree");  
				_data=textAsset.text;
			//}
			//_FileName = "ConfigSoundTree.haq";
			break;
		case game.whereIsTheBall:
			gameFile = game.whereIsTheBall;
			//if(sessionMng.activeKid.xmlDondeQuedoLaBolita!="")
			//{
			//	_data=sessionMng.activeKid.xmlDondeQuedoLaBolita;
			//}else
			//{
				textAsset = (TextAsset) Resources.Load("ConfigWhereIsTheBall");  
				_data=textAsset.text;
			//}
			//_FileName = "ConfigWhereIsTheBall.haq";
			break;
		case game.jungleDrawing:
			gameFile = game.jungleDrawing;
			//if(sessionMng.activeKid.xmlArenaMagica!="")
			//{
			//	_data=sessionMng.activeKid.xmlArenaMagica;
			//}else
			//{
				textAsset = (TextAsset) Resources.Load("ConfigJungleDrawing");  
				_data=textAsset.text;
			//}
			//_FileName = "ConfigJungleDrawing.haq";
			break;
		case game.river:
			gameFile = game.river;
			//if(sessionMng.activeKid.xmlRio!="")
			//{
			//	_data=sessionMng.activeKid.xmlRio;
			//}else
			//{
				textAsset = (TextAsset) Resources.Load("ConfigRiver");  
				_data=textAsset.text;
			//}
			//_FileName = "ConfigJungleDrawing.haq";
			break;
		case game.shadowGame:
			gameFile = game.shadowGame;
			//if(sessionMng.activeKid.xmlSombras!="")
			//{
			//	_data=sessionMng.activeKid.xmlSombras;
			//}else
			//{
				textAsset = (TextAsset)Resources.Load ("ConfigShadowGame");
				_data = textAsset.text;
			//}
			break;
		case game.introGames:
			gameFile = game.introGames;
			textAsset = (TextAsset)Resources.Load ("ConfigIntroGames");
			_data = textAsset.text;
			break;
		}
		//LoadXML (); 
		if (_data.ToString () != "") { 
			configuration = DeserializeObject (_data); 
		}
	}
	
	public void CreateTestFile (game target)
	{
		switch (target) {
		case game.treasure:
			gameFile = game.treasure;
			_FileName = "ConfigTreasure.haq";
			_data=SerializeObject(new LevelConfiguration());
			break;
		case game.soundTree:
			gameFile = game.soundTree;
			_FileName = "ConfigSoundTree.haq";
			_data=SerializeObject(new SoundTreeConfiguration());
			break;
		case game.whereIsTheBall:
			gameFile = game.whereIsTheBall;
			_FileName = "ConfigWhereIsTheBall.haq";
			_data=SerializeObject(new WhereIsTheBallConfiguration());
			break;
		case game.jungleDrawing:
			gameFile = game.jungleDrawing;
			_FileName = "ConfigJungleDrawing.haq";
			_data=SerializeObject(new JungleDrawingConfiguration());
			break;
		case game.river:
			gameFile = game.river;
			_FileName = "ConfigRiver.haq";
			_data=SerializeObject(new RiverConfiguration());
			break;
		case game.shadowGame:
			gameFile = game.shadowGame;
			_FileName = "ConfigShadowGame.xml";
			_data = SerializeObject (new ShadowGameConfiguration ());
			break;
		case game.introGames:
			gameFile = game.introGames;
			_FileName = "ConfigIntroGames.xml";
			_data = SerializeObject (new IntroGamesConfiguration ());
			break;
		}
		CreateXML ();
	}
	/* The following metods came from the referenced URL */ 
	string UTF8ByteArrayToString (byte[] characters)
	{      
		UTF8Encoding encoding = new UTF8Encoding (); 
		string constructedString = encoding.GetString (characters); 
		return (constructedString); 
	}
	
	byte[] StringToUTF8ByteArray (string pXmlString)
	{ 
		UTF8Encoding encoding = new UTF8Encoding (); 
		byte[] byteArray = encoding.GetBytes (pXmlString); 
		return byteArray; 
	} 
	
	// Here we serialize our UserData object of myData 
	string SerializeObject (object pObject)
	{ 
		string XmlizedString = null; 
		MemoryStream memoryStream = new MemoryStream (); 
		XmlSerializer xs; 
		switch(gameFile){
		case game.treasure:
			xs = new XmlSerializer (typeof(LevelConfiguration)); 
			break;
		case game.soundTree:
			xs = new XmlSerializer (typeof(SoundTreeConfiguration)); 
			break;
		case game.whereIsTheBall:
			xs = new XmlSerializer (typeof(WhereIsTheBallConfiguration));
			break;
		case game.jungleDrawing:
			xs = new XmlSerializer (typeof(JungleDrawingConfiguration));
			break;
		case game.river:
			xs = new XmlSerializer (typeof(RiverConfiguration));
			break;
		case game.shadowGame:
			xs = new XmlSerializer (typeof(ShadowGameConfiguration));
			break;
		case game.introGames:
			xs = new XmlSerializer (typeof(IntroGamesConfiguration));
			break;
		default:
			xs=new XmlSerializer(typeof(int));
			break;
		}
		XmlTextWriter xmlTextWriter = new XmlTextWriter (memoryStream, Encoding.UTF8); 
		xs.Serialize (xmlTextWriter, pObject); 
		memoryStream = (MemoryStream)xmlTextWriter.BaseStream; 
		XmlizedString = UTF8ByteArrayToString (memoryStream.ToArray ()); 
		return XmlizedString; 
	} 
	
	// Here we deserialize it back into its original form 
	object DeserializeObject (string pXmlizedString)
	{ 
		XmlSerializer xs;
		switch(gameFile){
		case game.treasure:
			xs = new XmlSerializer (typeof(LevelConfiguration)); 
			break;
		case game.soundTree:
			xs = new XmlSerializer (typeof(SoundTreeConfiguration)); 
			break;
		case game.whereIsTheBall:
			xs = new XmlSerializer (typeof(WhereIsTheBallConfiguration));
			break;
		case game.jungleDrawing:
			xs = new XmlSerializer (typeof(JungleDrawingConfiguration));
			break;
		case game.river:
			xs = new XmlSerializer (typeof(RiverConfiguration));
			break;
		case game.shadowGame:
			xs = new XmlSerializer (typeof(ShadowGameConfiguration));
			break;
		case game.introGames:
			xs = new XmlSerializer (typeof(IntroGamesConfiguration));
			break;
		default:
			xs=new XmlSerializer(typeof(int));
			break;
		}
		MemoryStream memoryStream = new MemoryStream (StringToUTF8ByteArray (pXmlizedString)); 
		//XmlTextWriter xmlTextWriter = new XmlTextWriter (memoryStream, Encoding.UTF8); 
		return xs.Deserialize (memoryStream); 
	} 
	
	// Finally our save and load methods for the file itself 
	void CreateXML ()
	{ 
		StreamWriter writer; 
		FileInfo t = new FileInfo (_FileLocation + "\\" + _FileName); 
		if (!t.Exists) { 
			writer = t.CreateText (); 
		} else { 
			t.Delete();
			writer = t.CreateText (); 
		} 
		writer.Write (_data); 
		writer.Close (); 
		Debug.Log ("File written."); 
	}
	
	void LoadXML ()
	{ 
		StreamReader r = File.OpenText (_FileLocation + "\\" + _FileName); 
		string _info = r.ReadToEnd (); 
		r.Close (); 
		_data = _info; 
		//		Debug.Log ("File Read"); 
	} 
} 


public class LevelConfiguration
{ 
	// We have to define a default instance of the structure 
	public int music;
	public int sound;
	public Level[] levels; 
	// Default constructor doesn't really do anything at the moment 
	public LevelConfiguration ()
	{
		levels = new Level[3];
		levels [0].subLevels = new SubLevel[2];
		levels [0].subLevels [0].availableObjects = new string[2]{"objeto1","objeto2"};
		//levels[0].subLevels[0].minObjectsToLook=1;
		//levels[0].subLevels[0].maxObjectsToLook=1;
		levels [0].subLevels [0].minObjectsQuantity = 1;
		levels [0].subLevels [0].maxObjectsQuantity = 2;
		levels [0].subLevels [1].availableObjects = new string[2]{"objeto3","objeto4"};
		//levels[0].subLevels[1].minObjectsToLook=1;
		//levels[0].subLevels[1].maxObjectsToLook=2;
		levels [0].subLevels [1].minObjectsQuantity = 1;
		levels [0].subLevels [1].maxObjectsQuantity = 3;
		levels [1].subLevels = new SubLevel[1];
		levels [1].subLevels [0].availableObjects = new string[2]{"objeto21","objeto22"};
		//levels[1].subLevels[0].minObjectsToLook=2;
		//levels[1].subLevels[0].maxObjectsToLook=2;
		levels [1].subLevels [0].minObjectsQuantity = 1;
		levels [1].subLevels [0].maxObjectsQuantity = 3;
		levels [2].subLevels = new SubLevel[1];
		levels [2].subLevels [0].availableObjects = new string[2]{"objeto31","objeto32"};
		//levels[2].subLevels[0].minObjectsToLook=2;
		//levels[2].subLevels[0].maxObjectsToLook=3;
		levels [2].subLevels [0].minObjectsQuantity = 2;
		levels [2].subLevels [0].maxObjectsQuantity = 3;
	} 
	
	public struct Level
	{ 
		public SubLevel[] subLevels;
	}
	// Anything we want to store in the XML file, we define it here 
	public struct SubLevel
	{ 
		//public int minObjectsToLook;
		//public int maxObjectsToLook;
		public int minObjectsQuantity;
		public int maxObjectsQuantity;
		public string[]availableObjects;
		public string[]availableCategories;
		public string[] search;
		public string[] distractors;
	}
}

public class SoundTreeConfiguration
{ 
	public int music;
	public int sound;
	public SoundCategory[] categories;
	public TreeLevel[]levels;
	public SoundTreeConfiguration ()
	{
		categories=new SoundCategory[2];
		categories[0].sounds=new string[]{"Motocicleta","Coche","Camion","Tren"};
		categories[1].sounds=new string[]{"Licuadora","Aspiradora","Radio"};
		levels=new TreeLevel[2];
		levels[0].subLevels=new TreeSubLevel[3];
		levels[1].subLevels=new TreeSubLevel[2];
		levels[0].subLevels[0].birds=1;
		levels[0].subLevels[0].nests=1;
		levels[0].subLevels[0].category=new int[]{0};
		levels[0].subLevels[1].birds=2;
		levels[0].subLevels[1].nests=2;
		levels[0].subLevels[1].category=new int[]{0,1};
		levels[0].subLevels[2].birds=3;
		levels[0].subLevels[2].nests=3;
		levels[0].subLevels[2].category=new int[]{0,1,0};
		levels[1].subLevels[0].birds=3;
		levels[1].subLevels[0].nests=3;
		levels[1].subLevels[0].category=new int[]{1,1,0};
		levels[1].subLevels[1].birds=4;
		levels[1].subLevels[1].nests=3;
		levels[1].subLevels[1].category=new int[]{1,1,0};
		
	}
	
	public struct TreeLevel
	{ 
		public TreeSubLevel[] subLevels;
	}
	public struct TreeSubLevel
	{ 
		public int birds;
		public int nests;
		public int[] category;
	}
	public struct SoundCategory
	{ 
		public string[] sounds;
	}
}

public class WhereIsTheBallConfiguration
{
	//Default instance
	public int music;
	public int sound;
	public WhereIsTheBallLevel[] levels; 
	
	public WhereIsTheBallConfiguration()
	{
		//number of levels
		levels =new WhereIsTheBallLevel[2];
		//number of sublevels for each level
		levels[0].subLevels=new WhereIsTheBallSubLevel[2];
		levels[1].subLevels=new WhereIsTheBallSubLevel[1];
		//for each sublevel add the objects required
		levels[0].subLevels[0].monkeys=2;
		levels[0].subLevels[0].objectNum=1;
		levels[0].subLevels[0].movementNum=10;
		levels[0].subLevels[0].time=2;
		levels[0].subLevels[0].instructions="FindOne";
		levels[0].subLevels[1].monkeys=3;
		levels[0].subLevels[1].objectNum=1;
		levels[0].subLevels[1].movementNum=13;
		levels[0].subLevels[1].time=4;
		levels[0].subLevels[1].instructions="FindOne";
		levels[1].subLevels[0].monkeys=4;
		levels[1].subLevels[0].objectNum=2;
		levels[1].subLevels[0].movementNum=20;
		levels[1].subLevels[0].time=5;
		levels[1].subLevels[0].instructions="FindOneOfTwo";
		
		
	}
	
	public struct WhereIsTheBallLevel
	{
		public WhereIsTheBallSubLevel[] subLevels;
	}
	public struct WhereIsTheBallSubLevel
	{
		//Objects the game needs 
		public int monkeys;//number of monkeys in the level
		public int objectNum;//number of objects in the level
		public int movementNum;//number of movements in the level
		public float time;//speed of the level
		public string instructions;//how many objects the have to find
	}
}
public class JungleDrawingConfiguration
{ 
	public int music;
	public int sound;
	public DrawingLevel[]levels;
	public JungleDrawingConfiguration ()
	{
		levels=new DrawingLevel[2];
		levels[0].subLevels=new DrawingSubLevel[2];
		levels[1].subLevels=new DrawingSubLevel[1];
		levels[0].subLevels[0].availableDrawings=new string[]{"LadyBug","Triangle"};
		levels[0].subLevels[0].activity="Single";
		levels[0].subLevels[1].availableDrawings=new string[]{"Triangle"};
		levels[0].subLevels[1].activity="Single";
		levels[1].subLevels[0].availableDrawings=new string[]{"LadyBug"};
		levels[1].subLevels[0].activity="Single";
	}
	
	public struct DrawingLevel
	{ 
		public DrawingSubLevel[] subLevels;
	}
	public struct DrawingSubLevel
	{ 
		public string[] availableDrawings;
		public string activity;
	}
}

public class RiverConfiguration
{ 
	public int music;
	public int sound;
	public RiverLevel[]levels;
	public RiverConfiguration ()
	{
		/*levels = new RiverLevel[2];
		levels [0].subLevels = new RiverSubLevel[2];
		levels [1].subLevels = new RiverSubLevel[2];
		levels [0].subLevels [0] = new RiverSubLevel ();
		levels [0].subLevels [0].reverse = false;
		levels [0].subLevels [0].availableObjects=new string[]{"object1","object2"};
		levels [0].subLevels [0].reverseObjects = null;
		levels [0].subLevels [0].neutralObjects = null;
		levels [0].subLevels [0].specialLeave = null;
		levels [0].subLevels [0].specialReverse = null;
		levels [0].subLevels [1] = new RiverSubLevel ();
		levels [0].subLevels [1].reverse = true;
		levels [0].subLevels [1].availableObjects=new string[]{"object3","object4"};
		levels [0].subLevels [1].reverseObjects = null;
		levels [0].subLevels [1].neutralObjects = null;
		levels [0].subLevels [1].specialLeave = null;
		levels [0].subLevels [1].specialReverse = null;
		levels [1].subLevels [0] = new RiverSubLevel ();
		levels [1].subLevels [0].reverse = false;
		levels [1].subLevels [0].availableObjects=new string[]{"object5","object6"};
		levels [1].subLevels [0].reverseObjects = null;
		levels [1].subLevels [0].neutralObjects = new string[]{"object5"};
		levels [1].subLevels [0].specialLeave = null;
		levels [1].subLevels [0].specialReverse = null;
		levels [1].subLevels [1] = new RiverSubLevel ();
		levels [1].subLevels [1].reverse = true;
		levels [1].subLevels [1].availableObjects=new string[]{"object7","object8"};
		levels [1].subLevels [1].reverseObjects = null;
		levels [1].subLevels [1].neutralObjects = null;
		levels [1].subLevels [1].specialLeave = null;
		levels [1].subLevels [1].specialReverse = null;*/
	}
	
	public struct RiverLevel
	{ 
		public RiverSubLevel[] subLevels;
	}
	public struct RiverSubLevel
	{ 
		public int speed;
		public bool reverse;
		public int totalObjects;
		public string[] availableObjects;
		/*public string[] reverseObjects;
		public string[] neutralObjects;
		public string[] forceForest;
		public string[] forceBeach;
		public string[] specialLeave;
		public string[] specialReverse;*/
		public bool reverseObjects;
		public bool neutralObjects;
		public bool forceForest;
		public bool forceBeach;
		public bool specialLeave;
		public bool specialReverse;
	}
}
public class ShadowGameConfiguration
{
	public int music;
	public int sound;
	public ShadowLevel[]levels;
	
	public ShadowGameConfiguration ()
	{	
		levels = new ShadowLevel[4];
		levels [0].subLevels = new ShadowSubLevel[5];
		levels [1].subLevels = new ShadowSubLevel[5];
		levels [2].subLevels = new ShadowSubLevel[5];
		levels [3].subLevels = new ShadowSubLevel[5];
		
		//level
		levels [0].subLevels [0].objectOptions = new string[] {
			"CubetaFacil",
			"PalaFacil",
			"RastrilloFacil"
		};
		levels [0].subLevels [0].timeOfShadow = 4f;
		levels [0].subLevels [0].numOfOptions = 3;
		levels [0].subLevels [0].inverse = false;
		levels [0].subLevels[0].pickTime = 4f;
		
		levels [0].subLevels [1].objectOptions = new string[] {
			"PlayeraFacil",
			"ShortsFacil",
			"TenisFacil"
		};
		levels [0].subLevels [1].timeOfShadow = 4f;
		levels [0].subLevels [1].numOfOptions = 3;
		levels [0].subLevels [1].inverse = false;
		levels [0].subLevels[1].pickTime = 3f;
		
		levels [0].subLevels [2].objectOptions = new string[] {
			"JarraFacil",
			"TazaFacil",
			"VasoFacil"
		};
		levels [0].subLevels [2].timeOfShadow = 4f;
		levels [0].subLevels [2].numOfOptions = 3;
		levels [0].subLevels [2].inverse = false;
		levels [0].subLevels[2].pickTime = 4f;
		
		levels [0].subLevels [3].objectOptions = new string[] {
			"ManzanaFacil",
			"PeraFacil",
			"BananaFacil"
		};
		levels [0].subLevels [3].timeOfShadow = 4f;
		levels [0].subLevels [3].numOfOptions = 3;
		levels [0].subLevels [3].inverse = false;
		levels [0].subLevels[3].pickTime = 2f;
		
		levels [0].subLevels [4].objectOptions = new string[] {
			"RadioFacil",
			"LicuadoraFacil",
			"TelefonoFacil",
			"TelevisionFacil"
		};
		levels [0].subLevels [4].timeOfShadow = 4f;
		levels [0].subLevels [4].numOfOptions = 4;
		levels [0].subLevels [4].inverse = false;
		levels[0].subLevels[4].pickTime = 3f;
		
		
		//level
		levels [1].subLevels [0].objectOptions = new string[] {
			"AvionMedio",
			"AvionetaMedio",
			"HelicopteroMedio"
		};
		levels [1].subLevels [0].timeOfShadow = 2f;
		levels [1].subLevels [0].numOfOptions = 3;
		levels [1].subLevels [0].inverse = false;
		levels[1].subLevels[0].pickTime = 3f;
		
		levels [1].subLevels [1].objectOptions = new string[] {
			"LataMedio",
			"OllaMedio",
			"TamborMedio"
		};
		levels [1].subLevels [1].timeOfShadow = 2f;
		levels [1].subLevels [1].numOfOptions = 3;
		levels [1].subLevels [1].inverse = false;
		levels[1].subLevels[1].pickTime = 4f;
		
		levels [1].subLevels [2].objectOptions = new string[] {
			"CajaMedio",
			"DadoMedio",
			"RegaloMedio"
		};
		levels [1].subLevels [2].timeOfShadow = 2f;
		levels [1].subLevels [2].numOfOptions = 3;
		levels [1].subLevels [2].inverse = false;
		levels[1].subLevels[2].pickTime = 2f;
		
		levels [1].subLevels [3].objectOptions = new string[] {
			"GatoMedio",
			"PerroMedio",
			"TigreMedio"
		};
		levels [1].subLevels [3].timeOfShadow = 2f;
		levels [1].subLevels [3].numOfOptions = 3;
		levels [1].subLevels [3].inverse = false;
		levels[1].subLevels[3].pickTime = 3f;
		
		levels [1].subLevels [4].objectOptions = new string[] {
			"PlayeraMedio",
			"SacoMedio",
			"SudaderaMedio"
		};
		levels [1].subLevels [4].timeOfShadow = 2f;
		levels [1].subLevels [4].numOfOptions = 3;
		levels [1].subLevels [4].inverse = false;
		levels[1].subLevels[4].pickTime = 3f;
		
		
		//level
		levels [2].subLevels [0].objectOptions = new string[] {
			"CaraFacil",
			"ManoFacil",
			"PieFacil"
		};
		levels [2].subLevels [0].timeOfShadow = 3f;
		levels [2].subLevels [0].numOfOptions = 3;
		levels [2].subLevels [0].inverse = true;
		levels[2].subLevels[0].pickTime = 2f;
		
		levels [2].subLevels [1].objectOptions = new string[] {
			"BicicletaFacil",
			"PatinesFacil",
			"PatinetaFacil"
		};
		levels [2].subLevels [1].timeOfShadow = 3f;
		levels [2].subLevels [1].numOfOptions = 3;
		levels [2].subLevels [1].inverse = true;
		levels[2].subLevels[1].pickTime = 3f;
		
		levels [2].subLevels [2].objectOptions = new string[] {
			"BancoFacil",
			"CamaFacil",
			"MesaFacil",
			"SillaFacil"
		};
		levels [2].subLevels [2].timeOfShadow = 3f;
		levels [2].subLevels [2].numOfOptions = 4;
		levels [2].subLevels [2].inverse = true;
		levels[2].subLevels[2].pickTime = 3f;
		
		levels [2].subLevels [3].objectOptions = new string[] {
			"GorraFacil",
			"SombreroFacil",
			"SombreroCopaFacil"
		};
		levels [2].subLevels [3].timeOfShadow = 3f;
		levels [2].subLevels [3].numOfOptions = 3;
		levels [2].subLevels [3].inverse = true;
		levels[2].subLevels[3].pickTime = 2f;
		
		levels [2].subLevels [4].objectOptions = new string[] {
			"AvionFacil",
			"BarcoFacil",
			"CarroFacil",
			"HelicopteroFacil"
		};
		levels [2].subLevels [4].timeOfShadow = 3f;
		levels [2].subLevels [4].numOfOptions = 4;
		levels [2].subLevels [4].inverse = true;
		levels[2].subLevels[4].pickTime = 3f;
		
		
		//level
		levels [3].subLevels [0].objectOptions = new string[] {
			"Pinzas1Medio",
			"Pinzas2Medio",
			"Pinzas3Medio"
		};
		levels [3].subLevels [0].timeOfShadow = 2f;
		levels [3].subLevels [0].numOfOptions = 3;
		levels [3].subLevels [0].inverse = true;
		levels[3].subLevels[0].pickTime = 2f;
		
		levels [3].subLevels [1].objectOptions = new string[] {
			"BancaMedio",
			"EscritorioMedio",
			"MesaMedio"
		};
		levels [3].subLevels [1].timeOfShadow = 2f;
		levels [3].subLevels [1].numOfOptions = 3;
		levels [3].subLevels [1].inverse = true;
		levels[3].subLevels[1].pickTime = 3f;
		
		levels [3].subLevels [2].objectOptions = new string[] {
			"LaptopMedio",
			"PortaretratoMedio",
			"TelevisionMedio"
		};
		levels [3].subLevels [2].timeOfShadow = 2f;
		levels [3].subLevels [2].numOfOptions = 3;
		levels [3].subLevels [2].inverse = true;
		levels[3].subLevels[2].pickTime = 2f;
		
		levels [3].subLevels [3].objectOptions = new string[] {
			"DonaMedio",
			"RuedaMedio",
			"VolanteMedio"
		};
		levels [3].subLevels [3].timeOfShadow = 2f;
		levels [3].subLevels [3].numOfOptions = 3;
		levels [3].subLevels [3].inverse = true;
		levels[3].subLevels[3].pickTime = 3f;
		
		levels [3].subLevels [4].objectOptions = new string[] {
			"FlautaMedio",
			"TrombonMedio",
			"TrompetaMedio"
		};
		levels [3].subLevels [4].timeOfShadow = 2f;
		levels [3].subLevels [4].numOfOptions = 3;
		levels [3].subLevels [4].inverse = true;
		levels[3].subLevels[4].pickTime = 2f;
		
	}
	public struct ShadowLevel
	{
		public ShadowSubLevel[] subLevels;
	}
	
	public struct ShadowSubLevel
	{
		public string[] objectOptions;
		public float timeOfShadow;
		public int numOfOptions;
		public bool inverse;
		public float pickTime;
	}
	
}

public class IntroGamesConfiguration
{
	public MiniGames miniGame;
	
	public IntroGamesConfiguration ()
	{
		miniGame.packing.packingSublevel = new PackingSublevel[7];
		miniGame.packing.packingSublevel [0].elementsA = new string[] {
			"Alcancia",
			"Piano",
			"Carrito"
		};
		miniGame.packing.packingSublevel [1].elementsA = new string[] {
			"Carrito",
			"Pino",
			"Raqueta",
			"Tabla de surf"
		};
		miniGame.packing.packingSublevel [2].elementsA = new string[] {
			"Raqueta",
			"Piano",
			"Tambor",
			"Pino",
			"Alcancia"
		};
		miniGame.packing.packingSublevel [3].elementsA = new string[] {
			"Tambor",
			"Alcancia",
			"Carrito",
			"Sombrero",
			"Pantalon",
			"Raqueta"
		};
		miniGame.packing.packingSublevel [4].elementsA = new string[] {
			"Piano",
			"Alacancia",
			"Carrito",
			"Raqueta",
			"Pantalon",
			"Robot",
			"Tabla de surf"
		};
		miniGame.packing.packingSublevel [5].elementsA = new string[] {
			"Alcancia",
			"Tambor",
			"Robot",
			"Tabla de surf",
			"Raqueta",
			"Carrito",
			"Sombrero",
			"Piano"
		};
		miniGame.packing.packingSublevel [6].elementsA = new string[] {
			"Alcancia",
			"Piano",
			"Carrito",
			"Raqueta",
			"Pantalon",
			"Robot",
			"Tambor",
			"Alcancia",
			"Pino"
		};
		miniGame.packing.packingSublevel [0].elementsB = new string[] {
			"Alcancia",
			"Piano",
			"Carrito"
		};
		miniGame.packing.packingSublevel [1].elementsB = new string[] {
			"Carrito",
			"Pino",
			"Raqueta",
			"Tabla de surf"
		};
		miniGame.packing.packingSublevel [2].elementsB = new string[] {
			"Raqueta",
			"Piano",
			"Tambor",
			"Pino",
			"Alcancia"
		};
		miniGame.packing.packingSublevel [3].elementsB = new string[] {
			"Tambor",
			"Alcancia",
			"Carrito",
			"Sombrero",
			"Pantalon",
			"Raqueta"
		};
		miniGame.packing.packingSublevel [4].elementsB = new string[] {
			"Piano",
			"Alacancia",
			"Carrito",
			"Raqueta",
			"Pantalon",
			"Robot",
			"Tabla de surf"
		};
		miniGame.packing.packingSublevel [5].elementsB = new string[] {
			"Alcancia",
			"Tambor",
			"Robot",
			"Tabla de surf",
			"Raqueta",
			"Carrito",
			"Sombrero",
			"Piano"
		};
		miniGame.packing.packingSublevel [6].elementsB = new string[] {
			"Alcancia",
			"Piano",
			"Carrito",
			"Raqueta",
			"Pantalon",
			"Robot",
			"Tambor",
			"Alcancia",
			"Pino"
		};
		miniGame.packing.packingSublevel [0].reverseEleA = new string[] {
			"Aletas",
			"Globo Terraqueo"
		};
		miniGame.packing.packingSublevel [1].reverseEleA = new string[] {
			"Aletas",
			"Globo Terraqueo",
			"Lentes"
		};
		miniGame.packing.packingSublevel [2].reverseEleA = new string[] {
			"Aletas",
			"Globo Terraqueo",
			"Lentes",
			"Cubo de Rubik"
		};
		miniGame.packing.packingSublevel [3].reverseEleA = new string[] {
			"Aletas",
			"Globo Terraqueo",
			"Lentes",
			"Cubo de Rubik",
			"Tableta"
		};
		miniGame.packing.packingSublevel [4].reverseEleA = new string[] {
			"Aletas",
			"Globo Terraqueo",
			"Lentes",
			"Cubo de Rubik",
			"Tableta",
			"Linterna"
		};
		miniGame.packing.packingSublevel [5].reverseEleA = new string[] {
			"Aletas",
			"Globo Terraqueo",
			"Lentes",
			"Cubo de Rubik",
			"Tableta",
			"Linterna",
			"Camara"
		};
		miniGame.packing.packingSublevel [6].reverseEleA = new string[] {
			"Aletas",
			"Globo Terraqueo",
			"Lentes",
			"Cubo de Rubik",
			"Tableta",
			"Linterna",
			"Camara",
			"Tiro al blanco"
		};
		miniGame.packing.packingSublevel [0].reverseEleB = new string[] {
			"Aletas",
			"Globo Terraqueo"
		};
		miniGame.packing.packingSublevel [1].reverseEleB = new string[] {
			"Aletas",
			"Globo Terraqueo",
			"Lentes"
		};
		miniGame.packing.packingSublevel [2].reverseEleB = new string[] {
			"Aletas",
			"Globo Terraqueo",
			"Lentes",
			"Cubo de Rubik"
		};
		miniGame.packing.packingSublevel [3].reverseEleB = new string[] {
			"Aletas",
			"Globo Terraqueo",
			"Lentes",
			"Cubo de Rubik",
			"Tableta"
		};
		miniGame.packing.packingSublevel [4].reverseEleB = new string[] {
			"Aletas",
			"Globo Terraqueo",
			"Lentes",
			"Cubo de Rubik",
			"Tableta",
			"Linterna"
		};
		miniGame.packing.packingSublevel [5].reverseEleB = new string[] {
			"Aletas",
			"Globo Terraqueo",
			"Lentes",
			"Cubo de Rubik",
			"Tableta",
			"Linterna",
			"Camara"
		};
		miniGame.packing.packingSublevel [6].reverseEleB = new string[] {
			"Aletas",
			"Globo Terraqueo",
			"Lentes",
			"Cubo de Rubik",
			"Tableta",
			"Linterna",
			"Camara",
			"Tiro al blanco"
		};
		miniGame.waitingRoom.flights = new string[] {
			"KM08",
			"KL11",
			"WK20",
			"ZA55",
			"WJ05",
			"KB12",
			"KS09",
			"ML56",
			"KW07",
			"GH13",
			"AT15",
			"PL90",
			"XQ04",
			"KM09",
			"KW03",
			"KL12",
			"WK0",
			"WK10",
			"WJ06",
			"KB13",
			"KS10",
			"KW04",
			"ML57",
			"GH14",
			"AT16",
			"KW01",
			"PL91",
			"NB30",
			"CD55",
			"RU02",
			"KZ45",
			"KW02",
			"RQ33",
			"FU88",
			"ZO73",
			"ÑJ21",
			"YT98",
			"KS56",
			"FU89",
			"KW08",
			"ZO74",
			"ÑJ22",
			"YT99",
			"KS57",
			"KW05",
			"FU90",
			"ZO75",
			"ÑJ23",
			"KW11",
			"WK10",
			"RQ34",
			"FU89",
			"ZO74",
			"ÑJ22",
			"KW14",
			"NB31",
			"CD56",
			"RU03",
			"RQ35",
			"FU90",
			"ZO75",
			"YT99",
			"KS57",
			"FU90",
			"WJ07",
			"KB14",
			"KW10",
			"WK-10",
			"KW10"
		};
		miniGame.flyPlane.arrowColor = new string[] {
			"LeftUpCloud",
			"MiddleCloud",
			"RightDownCloud",
			"RightUpCloud",
			"MiddleCloud",
			"MiddleCloud",
			"LeftDownCloud",
			"MiddleCloud",
			"RightUpCloud",
			"MiddleCloud"
		};
		miniGame.flyPlane.arrowDirection = new string[] {
			"Right",
			"Up",
			"Right",
			"Down",
			"Left",
			"Down",
			"Right",
			"Up",
			"Left",
			"Right"
		};
		
	}
	
	public struct MiniGames
	{
		
		public Packing packing;
		public WaitingRoom waitingRoom;
		public FlyPlane flyPlane;
		
	}
	
	public struct Packing
	{
		public PackingSublevel[] packingSublevel;
	}
	
	public struct PackingSublevel
	{
		public string[] elementsA;
		public string[] elementsB;
		public string[] reverseEleA;
		public string[] reverseEleB;
	}
	
	public struct WaitingRoom
	{
		public string[] flights;
	}
	
	public struct FlyPlane
	{
		public string[] arrowColorTutorial;
		public string[] arrowDirectionTutorial;
		public string[] arrowColor;
		public string[] arrowDirection;
	}
	
}