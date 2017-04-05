using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    [Header("Options Menu Items:")]

    public Text playerNameText;
    public InputField playerNameField;
        
    public Slider mainVolSlider;
    public Text mainVolText;

    public Slider musicVolSlider;
    public Text musicVolText;

    public Slider effectsVolSlider;
    public Text effectsVolText;

    public Toggle beepToggle;

    public Text graphicsQuality;
    public Text graphicsAA;

    public Toggle vSyncToggle;
    public Toggle fpsCounter;

    [Header("Menu GameObjects:")]
    public GameObject PausedMenu;
    public GameObject PausedMenuBG;
    public GameObject MainMenuFolder;
    public GameObject SPMenuFolder;

    public GameObject OptionsFolder;
    public GameObject OptionsMain;
    public GameObject OptionsAudio;
    public Button OptionsMainBtn;
    public GameObject OptionsVideo;

    public GameObject loadingScreenMain;
    public Canvas InGameCanvas;

    //public static string PlayerName;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (OptionsAudio.activeSelf)
        {
            mainVolText.text = GameInfo.MainVol.ToString();
            musicVolText.text = GameInfo.MusicVol.ToString();
            effectsVolText.text = GameInfo.EffectsVol.ToString(); 
        }

        #region LOADINGSCREEN

        if (Application.isLoadingLevel && loadingScreenMain != null)
        {
            loadingScreenMain.SetActive(true);
        }
        else if (loadingScreenMain != null && loadingScreenMain.activeSelf == true)
        {
            loadingScreenMain.SetActive(false);
        }

        #endregion

        #region enable-disable_pauseMenu

        if (GameInfo.GameMode == "SinglePlayer")
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !GameInfo.GamePaused)
            {
                PausedMenu.SetActive(true);
                PausedMenuBG.SetActive(true);

                GameInfo.GamePaused = true;

                if (InGameCanvas == null)
                {
                    GameObject tmp = GameObject.FindGameObjectWithTag("InGameCanvasObj");
                    InGameCanvas = tmp.GetComponent<Canvas>();
                    InGameCanvas.enabled = false;
                }
                else
                {
                    InGameCanvas.enabled = false;
                }
                
                Time.timeScale = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && GameInfo.GamePaused)
            {
                BtnOptions(false);
                PausedMenu.SetActive(false);
                PausedMenuBG.SetActive(false);

                GameInfo.GamePaused = false;
                InGameCanvas.enabled = true;
                Time.timeScale = 1;

                GameInfo.WriteFile();
                GameInfo.ApplySettings();
            }
        }

        #endregion


    }
    /*
        paused menu buttons
    */

    public void ResumeGame()
    {
        PausedMenu.SetActive(false);
        PausedMenuBG.SetActive(false);

        GameInfo.GamePaused = false;
        InGameCanvas.enabled = true;
        Time.timeScale = 1;

        ClickSound();
    }

    public void QuitGame()
    {
        PausedMenu.SetActive(false);
        GameInfo.GamePaused = false;
        Time.timeScale = 1;
        GameInfo.GameMode = "";

        Application.LoadLevel(0);

        ClickSound();
    }



    /*
        Main Menu methods  
    */

    public void BtnSinglePlayer()
    {
        Debug.Log("BtnSinglePlayer " + Time.time);
        ClickSound();
        //Application.LoadLevel(1);
    }

    public void BtnMultiPLayer()
    {
        Debug.Log("BtnMultiPLayer " + Time.time);
        ClickSound();
    }

    public void BtnOptions(bool _enable)
    {
        OptionsFolder.SetActive(_enable);
       
        // from main menu
        if (!GameInfo.GamePaused)
        {
            OptionsMainBtn.interactable = true;
            OptionsMain.SetActive(_enable);

            MainMenuFolder.SetActive(true);
        }
        //from paused menu
        else
        {
            OptionsMainBtn.interactable = false;
            OptionsMain.SetActive(false);

            OptionsVideo.SetActive(true);
            PausedMenu.SetActive(!_enable);
        }

        

        Debug.Log("BtnOptions " + Time.time);
        ClickSound();
    }

    public void BtnQuit()
    {
        Debug.Log("BtnQuit " + Time.time);
        ClickSound();
        Application.Quit();
    }
    

    /*Options Menu methods*/

    public void PlayerNameEditEnd()
    {
        GameInfo.PlayerName = playerNameField.text;
        playerNameText.text = GameInfo.PlayerName;
    }

    public void BtnBack()
    {
        GameInfo.WriteFile();
        GameInfo.ApplySettings();
        ClickSound();
    }

    // back button in singleplayer menu
    public void BtnBackSP()
    {
        ClickSound();
    }

    public void BtnPlaySP()
    {
        SPMenuFolder.SetActive(false);

        ClickSound();
        Application.LoadLevel(1);
    }


    /*Audio settings*/

    /* Set volume level from slider */
    /* Called by UI */
    public void SliderMainVol()
    {
        GameInfo.MainVol = mainVolSlider.value;
    }

    public void SliderMusicVol()
    {
        GameInfo.MusicVol = musicVolSlider.value;
    }
    
    public void SliderEffectsVol()
    {
        GameInfo.EffectsVol = effectsVolSlider.value;
    }
    /* Toggle menu beep */
    public void ToggleBeep()
    {
        ClickSound();
        if (beepToggle.isOn)
            {GameInfo.MenuBeep = true;}
        else
            {GameInfo.MenuBeep = false;}
    }

    /* Set main volume slider and clicksound volume level */
    public void MainVolume(float _value)
    {
        gameObject.GetComponent<AudioSource>().volume = _value / 100;
        mainVolSlider.value = _value;
    }

    /* Set music volume slider */
    public void MusicVolume(float _value)
    {
        musicVolSlider.value = _value;
    }

    /* Set effects volume slider */
    public void EffectsVolume(float _value)
    {
        effectsVolSlider.value = _value;
    }

    /* Video options */
    /* General quality */
    public void SetGraphics(float _value)
    {
        ClickSound();
        if (_value < 0) //decrease quality
        {
            QualitySettings.DecreaseLevel();
        }
        else // increase quality
        {
            QualitySettings.IncreaseLevel();
        }
        //re-set AA to override default value for graphic level settings
        QualitySettings.antiAliasing = GameInfo.GraphicsAA;
        // Set text value for current level
        string[] names = QualitySettings.names;
        graphicsQuality.text = names[QualitySettings.GetQualityLevel()].ToString();
    }

    /* Set AA level */
    public void SetAA(int _value)
    {
        ClickSound();

        if (_value > 0 && GameInfo.GraphicsAA < 8) //increase value
        {
            if (GameInfo.GraphicsAA != 0)
            {
                GameInfo.GraphicsAA = GameInfo.GraphicsAA * 2; 
            }
            else
            {
                GameInfo.GraphicsAA = 2;
            }
        }
        else if(_value < 0 && GameInfo.GraphicsAA > 0)//decrease value
        {
            if(GameInfo.GraphicsAA == 4 || GameInfo.GraphicsAA == 8)
            {
                GameInfo.GraphicsAA = GameInfo.GraphicsAA / 2;
            }
            else
            {
                GameInfo.GraphicsAA = 0;
            }
        }
        QualitySettings.antiAliasing = GameInfo.GraphicsAA;
        graphicsAA.text = GameInfo.GraphicsAA.ToString() + "X";
    }

    /* toggle V-Sync */
    public void ToggleVSync()
    {
        ClickSound();
        if (vSyncToggle.isOn)
        {
            GameInfo.GraphicsVSync = 1;
        }
        else
        {
            GameInfo.GraphicsVSync = 0;
        }
        QualitySettings.vSyncCount = GameInfo.GraphicsVSync;
    }

    /* toggle fps counter */
    public void FpsCounter()
    {
        ClickSound();
        if(fpsCounter.isOn)
        {
            GameInfo.FpsCounter = 1;
        }
        else
        {
            GameInfo.FpsCounter = 0;
        }
    }
    
    /* general methods */
    /* play menu beep sound */
    public void ClickSound()
    {
        if (GameInfo.MenuBeep)
        {
            gameObject.GetComponent<AudioSource>().Play(); 
        }
    }
}

