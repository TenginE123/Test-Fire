using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager> {

    GameManager.Mode mode = GameManager.Instance.Modes;
    GameManager.GameState state = GameManager.Instance.GameMode;

    //Main Menu
    public Button startButton;
        Text startButtonText;

    //Settings Menu
        Toggle  musicToggle;
        Text    musicToggleText;
        public Toggle  sfxToggle;
        Text    sfxToggleText;
        Button removeAds;
        //Text removeAdsText;   //[included to allow to swap text if user has purchased ad removal]

    //GameUI
        public Button pauseButton;

    //Pause Menu
        Button pauseResume;
        Button pauseRestart;
        public Button pauseExit;

    //Game Over
        GameObject gameOver;
        Button goPlayAgain;
        Text goPlayAgainText;

    // Subscribe to an "Initialisation" event?
    void Start ()
    {
        //Main Menu Button Functions
        startButton = GameObject.Find("Start Button").GetComponent<Button>();
        startButtonText = GameObject.Find("Start Button Text").GetComponent<Text>();
        startButton.onClick.AddListener(CallStart); //use substitute functions to call singleton functions

        //Settings Menu Functions
        musicToggle = GameObject.Find("Music Toggle").GetComponent<Toggle>();           //Music Toggle
        musicToggleText = GameObject.Find("Music Toggle Text").GetComponent<Text>();
        musicToggle.onValueChanged.AddListener(MusicToggle);
        sfxToggle = GameObject.Find("SoundFX Toggle").GetComponent<Toggle>();           //SoundFX Toggle
        sfxToggleText = GameObject.Find("SoundFX Toggle Text").GetComponent<Text>();
        sfxToggle.onValueChanged.AddListener(SFXToggle);
        removeAds = GameObject.Find("Remove Ads").GetComponent<Button>();               //Remove Ads
        //removeAdsText = GameObject.Find("Remove Ads Text").GetComponent<Text>();
        removeAds.onClick.AddListener(RemoveAds);

        //Game UI Button Functions
        pauseButton = GameObject.Find("Pause Button").GetComponent<Button>();   //Pause
        pauseButton.onClick.AddListener(CallPauseGame);

        //Pause Menu Button Functions
        pauseResume = GameObject.Find("Resume").GetComponent<Button>();     //Resume
        pauseResume.onClick.AddListener(CallPauseResume);
        pauseRestart = GameObject.Find("Restart").GetComponent<Button>();   //Restart
        pauseRestart.onClick.AddListener(CallPauseRestart);
        pauseExit = GameObject.Find("Exit").GetComponent<Button>();         //Exit
        pauseExit.onClick.AddListener(CallPauseExit);

        //Game Over Screen Functions
        gameOver = GameObject.Find("Game Over");
        goPlayAgain = GameObject.Find("Restart Button").GetComponent<Button>();
        goPlayAgain.onClick.AddListener(CallGameExit);
        goPlayAgainText = GameObject.Find("Restart Button Text").GetComponent<Text>();

        //Disable Container Objects after initialisation
        gameOver.SetActive(false);
        GameObject.Find("Settings").SetActive(false);
        GameObject.Find("Pause Menu").SetActive(false); //Deactivate Pause Menu after initialisation
        GameObject.Find("Game UI").SetActive(false);    //Deactivate Game UI
    }


    private void Update()
    {
        mode = GameManager.Instance.Modes;
        state = GameManager.Instance.GameMode;
        //Toggle Start Button conditions
        if (mode == GameManager.Mode.StartScreen || mode == GameManager.Mode.Scores || state == GameManager.GameState.GameOver)
        {
            if (!IsInvoking("ToggleStartButton"))
            {
                Invoke("ToggleStartButton", 0.5f);
            }
        }
    }

    void ToggleStartButton() //Start Button Text flash function
    {
        //Flash Main Menu Start Button
        if (mode == GameManager.Mode.StartScreen)
        {
            startButtonText.gameObject.SetActive(!startButtonText.gameObject.activeSelf);
        }
        //Flash Game Over Restart Button
        if(state == GameManager.GameState.GameOver)
        {
            goPlayAgainText.gameObject.SetActive(!goPlayAgainText.gameObject.activeSelf);
        }
        //if HighScore Screen ? Flash hsRestart button
    }

    void MusicToggle(bool t)
    {
        if (t)
        {
            musicToggleText.text = "Music: ON";
        }
        else
        {
            musicToggleText.text = "Music: OFF";
        }
    }

    void SFXToggle(bool t)
    {
        if (t)
        {
            sfxToggleText.text = "Sound FX: ON";
        }
        else
        {
            sfxToggleText.text = "Sound FX: OFF";
        }
    }

    void RemoveAds()
    {
        //call IAP api funciton in GameManager
        Debug.Log("RemoveAds Called");
    }

    void CallStart()
    {
        GameManager.Instance.SetGameMode("Start");
    }

    void CallPauseGame()
    {
        GameManager.Instance.PauseGame();
        AudioManager.Instance.StopMain();
    }

    void CallPauseResume()
    {
        GameManager.Instance.PauseGame();
        AudioManager.Instance.PlayMain();
    }

    void CallPauseRestart()
    {
        GameManager.Instance.RestartGame();
        GameManager.Instance.PauseGame();
    }

    void CallPauseExit()
    {
        CallGameExit();
        GameManager.Instance.PauseGame();
    }

    void CallGameExit()
    {
        GameManager.Instance.SetGameMode("Finish");
    }

    //public function to hide a UI element
    public void Hide(string elemName)
    {
        switch (elemName)
        {
            case "GameUI":
                GameObject.Find("Game UI").SetActive(false);    //Deactivate Game UI
                break;
            default:
                Debug.Log("Error: UIManager - Unrecognised Hide Command.");
                break;
        }
    }

    public void GameOver()
    {
        //Set gameover ovject active
        gameOver.SetActive(true);
        
    }
}
