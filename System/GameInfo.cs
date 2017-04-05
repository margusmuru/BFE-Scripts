using UnityEngine;
using System.Collections;
using System;
using System.IO;
using UnityEngine.UI;

public class GameInfo : MonoBehaviour {

    public MainMenu mainMenu;

    public static ILevelMaster levelMaster;
    public static ILevelMaster levelMasterAI;

    private static string filePath;
    private  bool firstLoad = true;
    private  int myLifeTime = 0;
	
	public static string GameMode = "";
	public static string LevelOption = "Conquest";
    public static bool SinglePlayer = true;

    public static bool GameEnded = false;
    public static bool GamePaused = false;
	public static bool LevelReady = false;
		
	public static string PlayerName = "";
	public static string PlayerTag = "Team1";
	public static string PlayerUnit = "Friendly";
	public static int GameDifficulty = 2;
	public static  bool MenuBeep = true;

	public static float MainVol = 100;
	public static float MusicVol = 100;
	public static float EffectsVol = 100;
	public static int GraphicsQuality = 2;
	public static int GraphicsAA = 2;
	public static int GraphicsVSync = 0;
    public static int FpsCounter = 1;

    //HUD FPS
    public float updateInterval = 0.5F;

    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval
    public Text fpsValue;

    //faction
    public Text factionText;
    public static bool FastBuild = true;
    public Text fastBuildText;

	void Awake() 
	{
		/*
         * always make sure there is only one GameInfo file in the scene
         */
        DontDestroyOnLoad(transform.gameObject);
		GameObject[] allGI = GameObject.FindGameObjectsWithTag("GameInfo");
		if(allGI.Length > 1)
		{
			foreach(GameObject theGI in allGI) 
			{
				if(theGI.GetComponent<GameInfo>().myLifeTime > myLifeTime)
				{Destroy(gameObject);}
			}
		}
		myLifeTime = 1;

        levelMaster = GameObject.FindGameObjectWithTag("LevelMaster").GetComponent<ILevelMaster>();
        if (SinglePlayer)
        {
            levelMaster.PlayerTag = "Team1";
        }
        levelMasterAI = GameObject.FindGameObjectWithTag("LevelMasterAI").GetComponent<ILevelMaster>();
        if (SinglePlayer)
        {
            levelMasterAI.PlayerTag = "Team2";
        }
    }

	void Start()
	{
		// Set settings file filepath
        filePath= Application.persistentDataPath+"/settings.txt";
//		if(writeFile){WriteFile();}else {ReadFile();}
        ReadFile();
		StartCoroutine(DelayedStart());
        //HUD FPS
        timeleft = updateInterval;

        // gamemode
        GameMode = "SinglePlayer";
	}

    void Update()
    {
        #region FPS
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            float fps = accum / frames;
            string format = System.String.Format("{0:F2} FPS", fps);
            if (FpsCounter==1)
            {
                fpsValue.text = format;
                if (fps < 30)
                    fpsValue.color = Color.yellow;
                else
                    if (fps < 10)
                        fpsValue.color = Color.red;
                    else
                        fpsValue.color = Color.green;
            }
            else
            {
                fpsValue.text = "";
            }
        
            //	DebugConsole.Log(format,level);
            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
        #endregion

    }

	IEnumerator DelayedStart()
	{
        yield return new WaitForSeconds(1);
        //ReadFile();
        yield return new WaitForSeconds(2);
		firstLoad=false;
	}

	void ReadFile()
	{
        string[] names;

		try {
			var sr = new StreamReader(filePath);
			string input = "";
			while (true) 
			{
				input = sr.ReadLine();
				if (input == null) {break;}
				names=input.Split("="[0]);

				if(names[0] == "GameDifficulty")
                {
                    GameDifficulty=System.Int32.Parse(names[1]);
                }
                if (names[0] == "PlayerName") 
                {
                    //Apply to local
                    PlayerName = names[1];
                    //Apply to main menu if available
                    GameObject _tmpMainMenu = GameObject.FindGameObjectWithTag("MainMenu");
                    if (_tmpMainMenu != null)
                    {
                        _tmpMainMenu.GetComponent<MainMenu>().playerNameField.text = PlayerName; 
                    }
                }
				if(names[0] == "MenuBeep")
                {
                    if(System.Int32.Parse(names[1]) == 0)
                        {MenuBeep=false;}
                    else{MenuBeep=true;}
                }
				if(names[0] == "MainVol")
                { 
                    MainVol = int.Parse(names[1]); 
                }
				if(names[0] == "MusicVol")
                {
                    MusicVol = System.Int32.Parse(names[1]);
                }
                if(names[0] == "EffectsVol") 
                {
                    EffectsVol = System.Int32.Parse(names[1]); 
                }
				if(names[0] == "GraphicsQuality")
                {
                    GraphicsQuality = System.Int32.Parse(names[1]);
                }
				if(names[0] == "GraphicsAA")
                {
                    GraphicsAA = System.Int32.Parse(names[1]);
                }
				if(names[0] == "GraphicsVSync")
                {
                    GraphicsVSync = System.Int32.Parse(names[1]);
                }
                if (names[0] == "FpsCounter")
                {
                    FpsCounter = System.Int32.Parse(names[1]);
                }
			}
			sr.Close ();
			Debug.Log("settings file read");
            
		}
		catch (Exception e)
        {
            print(e); 
            Debug.Log("Writing a new settings file");
            WriteFile();
        }
		ApplySettings();
	}

	public static void WriteFile()
	{
		var sw = new StreamWriter(filePath);
		sw.WriteLine("GameDifficulty=" + GameDifficulty);
		sw.WriteLine("PlayerName=" + PlayerName);
		if(MenuBeep){sw.WriteLine("MenuBeep=1");}else{sw.WriteLine("MenuBeep=0");}
		sw.WriteLine("MainVol=" + MainVol);	
		sw.WriteLine("MusicVol=" + MusicVol);
        sw.WriteLine("EffectsVol=" + EffectsVol);	
		sw.WriteLine("GraphicsQuality=" + GraphicsQuality);	
		sw.WriteLine("GraphicsAA=" + GraphicsAA);	
		sw.WriteLine("GraphicsVSync=" + GraphicsVSync);
        sw.WriteLine("FpsCounter=" + FpsCounter);
		sw.Flush();
		sw.Close();
		Debug.Log("settings file saved");
	}

	public static void ApplySettings()
	{
        
        GameObject _tmp = GameObject.FindGameObjectWithTag("MainMenu");

        if (_tmp != null)
        {
            MainMenu _mainMenu = GameObject.FindGameObjectWithTag("MainMenu").GetComponent<MainMenu>();
            //Set volume levels
            _mainMenu.MainVolume(MainVol);
            _mainMenu.MusicVolume(MusicVol);
            _mainMenu.EffectsVolume(EffectsVol);
            if (MenuBeep)
            {
                _mainMenu.beepToggle.isOn = true;
            }
            else
            {
                _mainMenu.beepToggle.isOn = false;
            }

            //set main graphics quality
            QualitySettings.SetQualityLevel(GraphicsQuality);
            _mainMenu.SetGraphics(0); // value "0" calls just the text value update

            //set Antialiasing
            QualitySettings.antiAliasing = GraphicsAA;
            _mainMenu.SetAA(0); //value "0" calls just the text value update

            //set V-Sync
            QualitySettings.vSyncCount = GraphicsVSync;
            if (GraphicsVSync == 1)
            {
                _mainMenu.vSyncToggle.isOn = true;
            }
            else
            {
                _mainMenu.vSyncToggle.isOn = false;
            }

            /*fps counter*/
            if (FpsCounter == 1)
            {
                _mainMenu.fpsCounter.isOn = true;
            }
            else
            {
                _mainMenu.fpsCounter.isOn = false;
            } 
        }
	}

    public void ChangeFaction()
    {
        if (PlayerUnit == "Friendly")
        {
            PlayerUnit = "Enemy";
        }
        else if (PlayerUnit == "Enemy")
        {
            PlayerUnit = "Friendly";
        }
        factionText.text = PlayerUnit;
    }

    public void ChangeFastBuild()
    {
        if (FastBuild)
        {
            FastBuild = false;
        }
        else
        {
            FastBuild = true;
        }
        fastBuildText.text = "FB = " + FastBuild.ToString();
    }

    public static ILevelMaster GetLevelMaster(String faction)
    {
        if (SinglePlayer)
        {
            if (faction.Equals("Friendly"))
            {
                return levelMaster;
            }
            else
            {
                return levelMasterAI;
            }
        }
        else
        {
            return null;
        }
    }


}
