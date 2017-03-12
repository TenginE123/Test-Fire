using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    //enums for setting the current gamestate

    public enum Mode { StartScreen, Settings, Game, Scores }
    public enum GameState { Start, Normal, Defense, GameOver, EnterScore }
    //Mode currentMode = Mode.StartScreen;

    //GameState gameState = Gamestate;

    public Mode Modes;
    public GameState GameMode;
    //width property?
    [SerializeField]
    private int width = 97; //width + 1 to account for pixel adjustment

    public bool isPaused = false;

    public int Width
    {
        get { return width; }
    }

    //universal speed modifier
    [SerializeField]
    private int scrollSpeed = 288;

    public int ScrollSpeed
    {
        get { return scrollSpeed; }
    }

    private void Awake()
    {
        base.Awake();//Initialisation procedures
        Application.targetFrameRate = 30;
    }

    private void OnApplicationFocus(bool focus)
    {
        //Use this to pause the game when the app is in the background
        //isPaused = !focus;
    }

    private void Update()
    {
        //place game update event call here
        
    }

    //Allow buttons and other objects to set the gamemode
    public void SetGameMode (string mode)
    {
        switch (mode)
        {
            case "Start":
                StartGame();
                break;
            case "Normal":
                SetGameNormal();
                break;
            case "Defense":
                SetGameDefense();
                break;
            case "GameOver":
                SetGameGameOver();
                break;
            case "Finish":
                FinishGame();
                break;
        }
    }

    private void StartGame ()
    {
        Modes = Mode.Game;
        GameMode = GameState.Start;
        //do the things
        AudioManager.Instance.PlayMain();
        //set score to 0


        //EventManager.Instance.Invoke("OnTouch", 1);
        Invoke("SetGameNormal", 1);
    }

    private void FinishGame()
    {
        Modes = Mode.StartScreen;
        GameMode = GameState.Start;
    }

    private void SetGameNormal()
    {
        GameMode = GameState.Normal;
    }

    private void SetGameDefense()
    {
        if(GameMode != GameState.Defense)   //ensure gamemode is set only once
        {
            GameMode = GameState.Defense;
            AudioManager.Instance.PlayAlert();
        }
    }

    private void SetGameGameOver()
    {
        if (GameMode != GameState.GameOver)   //ensure gamemode is set only once
        {
            GameMode = GameState.GameOver;
        }
    }

    public void RestartGame()
    {
        EventManager.Instance.CallRestart();
        SetGameMode("Start");
    }

    public void PauseGame()
    {
        isPaused = !isPaused;

        if (isPaused == true)
        {
            if (Time.timeScale != 0)
            {
                Time.timeScale = 0;
            }
        }
        else
        {
            if (Time.timeScale != 1)
            {
                Time.timeScale = 1;
            }
        }
    }
}
