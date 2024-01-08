using Godot;
using System;

public enum GameState{Play, Won, Lost}

public class GameFlowManager : Node
{
    // Parameters

    // Duration of the fade-to-black at the end of the game
    [Export]
    public float endSceneLoadDelay = 3f;
    // The canvas group of the fade-to-black screen
    /* [Export]
    public CanvasGroup endGameFadeCanvasGroup; */

    // Win

    // This string has to be the name of the scene you want to load when winning
    [Export]
    public string winSceneName = "WinScene";
    // Duration of delay before the fade-to-black, if winning
    [Export]
    public float delayBeforeFadeToBlack = 4f;
    // Duration of delay before the win message
    [Export]
    public float delayBeforeWinMessage = 2f;
    // Sound played on win
    /* [Export] //TODO: play sound when victory
    public AudioClip victorySound; */
    // Prefab for the win game message
    [Export]
    public DisplayMessage winDisplayMessage;
    /* [Export] //TODO: playback the race countdown
    public PlayableDirector raceCountdownTrigger; */

    // Lose

    // This string has to be the name of the scene you want to load when losing
    [Export]
    public string loseSceneName = "LoseScene";
    // Prefab for the lose game message
    [Export]
    public DisplayMessage loseDisplayMessage;


    public GameState gameState { get; private set; }

    public bool autoFindKarts = true;
    public ArcadeKart playerKart;

    ArcadeKart[] karts;
    ObjectiveManager m_ObjectiveManager;
    TimeManager m_TimeManager;
    float m_TimeLoadEndGameScene;
    string m_SceneToLoad;
    float elapsedTimeBeforeEndScene = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
