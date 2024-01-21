using Godot;
using System;
using System.Collections;
using System.Diagnostics;

public enum GameState{Play, Won, Lost}

public class GameFlowManager : Node
{
    // Parameters

    [Export(hintString: "Duration of the fade-to-black at the end of the game")]
    public float endSceneLoadDelay = 3f;
    // The canvas group of the fade-to-black screen
    /* [Export]
    public CanvasGroup endGameFadeCanvasGroup; */

    // Win

    [Export(hintString: "This string has to be the name of the scene you want to load when winning")]
    public string winSceneName = "WinScene";
    [Export(hintString: "Duration of delay before the fade-to-black, if winning")]
    public float delayBeforeFadeToBlack = 4f;
    [Export(hintString: "Duration of delay before the win message")]
    public float delayBeforeWinMessage = 2f;
    [Export(hintString: "Sound played on win")]
    public AudioStreamSample victorySound;
    [Export(hintString: "Prefab for the win game message")]
    public NodePath winDisplayMessagePath;
    public DisplayMessage winDisplayMessage;
    /* [Export] //TODO: playback the race countdown
    public PlayableDirector raceCountdownTrigger; */

    // Lose

    [Export(hintString: "This string has to be the name of the scene you want to load when losing")]
    public string loseSceneName = "LoseScene";
    [Export(hintString: "Prefab for the lose game message")]
    public NodePath loseDisplayMessagePath;
    public DisplayMessage loseDisplayMessage;

    public GameState gameState { get; private set; }

    // DONT NEED public bool autoFindKarts = true; // ONLY 1 KART in scene
    [Export(hintString: "Vehicle body player kart")]
    public NodePath kartPath;
    private ArcadeKartVehicleBody playerKart;

    // DONT NEED: ArcadeKart[] karts;
    [Export(hintString: "Objective Manager node")]
    public NodePath objectiveManagerPath;
    ObjectiveManager m_ObjectiveManager;

    [Export(hintString: "Time Manager node")]
    public NodePath timeManagerPath;
    TimeManager m_TimeManager;
    float m_TimeLoadEndGameScene;
    string m_SceneToLoad;
    float elapsedTimeBeforeEndScene = 0;

    DisabilityManager disabilityManager;

    [Export(hintString: "no delay until race starts if true")]
    public bool debugMode;


    // Racecountdown stuff
    [Signal]
    public delegate void start_race_countdown();

    [Export]
    public NodePath RacecountdownNodePath;
    private RaceCountdown racecountdown;
    private string countdownSignalName = "start_race_countdown";

    public override void _Ready()
    {
        disabilityManager = (DisabilityManager) GetTree().Root.GetNode<Node>(GameConstants.disabilityManagerPath);

        playerKart = GetNode<ArcadeKartVehicleBody>(kartPath);

        m_ObjectiveManager = GetNode<ObjectiveManager>(objectiveManagerPath);

        m_TimeManager = GetNode<TimeManager>(timeManagerPath);

        // connect to gdscript for countdown
        racecountdown = GetNode<RaceCountdown>(RacecountdownNodePath);
        //this.Connect(countdownSignalName, racecountdown, "OnTriggerRaceCountdown"); for connecting to gdscript

        // AudioUtility.SetMasterVolume(1); TODO:
        AudioServer.SetBusVolumeDb(0, 0.0f);

        winDisplayMessage = GetNode<DisplayMessage>(winDisplayMessagePath);
        loseDisplayMessage = GetNode<DisplayMessage>(loseDisplayMessagePath);
        disabilityManager.Disable(winDisplayMessage);
        disabilityManager.Disable(loseDisplayMessage);

        if (!debugMode)
        {
            m_TimeManager.StopRace();
            playerKart.SetCanMove(false);

            // run race countdown animation
            ShowRaceCountdownAnimation();

            // async methods for animations
            ShowObjectivesRoutine();
            CountdownThenStartRaceRoutine();
        }
    }

    async void CountdownThenStartRaceRoutine()
    {
        await ToSignal(GetTree().CreateTimer(3f), "timeout");
        StartRace();
    }

    void StartRace() {
        playerKart.SetCanMove(true);
        m_TimeManager.StartRace();
    }

    void ShowRaceCountdownAnimation() {
        //EmitSignal(countdownSignalName); for c# to gdscript interaction
        racecountdown.CallDeferred("TriggerRaceCountdown"); // call when node has finished loading
    }

    async void ShowObjectivesRoutine() {
        while(m_ObjectiveManager.Objectives.Count == 0)
        {
            await ToSignal(GetTree(), "idle_frame");
        }

        await ToSignal(GetTree().CreateTimer(0.2f), "timeout");

        for(int i = 0; i < m_ObjectiveManager.Objectives.Count; i++)
        {
            if(m_ObjectiveManager.Objectives[i].displayMessage != null)
            {
                m_ObjectiveManager.Objectives[i].displayMessage.Display();
            }
            await ToSignal(GetTree().CreateTimer(1f), "timeout");
        }
    }

    /* Unity way of async functions

        /* IEnumerator CountdownThenStartRaceRoutine() {
            yield return new WaitForSeconds(3f);
            StartRace();
        }
        IEnumerator ShowObjectivesRoutine() {
            while (m_ObjectiveManager.Objectives.Count == 0)
                yield return null;
            yield return new WaitForSecondsRealtime(0.2f);
            for (int i = 0; i < m_ObjectiveManager.Objectives.Count; i++)
            {
               if (m_ObjectiveManager.Objectives[i].displayMessage)m_ObjectiveManager.Objectives[i].displayMessage.Display();
               yield return new WaitForSecondsRealtime(1f);
            }
        } */

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (gameState != GameState.Play)
        {
            elapsedTimeBeforeEndScene += delta;
            if(elapsedTimeBeforeEndScene >= endSceneLoadDelay)
            {

                float timeRatio = 1 - (m_TimeLoadEndGameScene - HelperFunctions.GetTime()) / endSceneLoadDelay;

                //TODO:
                //endGameFadeCanvasGroup.alpha = timeRatio;

                float volumeRatio = Mathf.Abs(timeRatio);
                float volume = Mathf.Clamp(1 - volumeRatio, 0, 1);
                AudioServer.SetBusVolumeDb(0, GD.Linear2Db(volume));

                // See if it's time to load the end scene (after the delay)
                if (HelperFunctions.GetTime() >= m_TimeLoadEndGameScene)
                {
                    GetTree().ChangeScene("Scenes/Menues/"+m_SceneToLoad+"/"+m_SceneToLoad+".tscn");
                    gameState = GameState.Play;
                }
            }
        }
        else
        {
            if (m_ObjectiveManager.AreAllObjectivesCompleted())
                EndGame(true);

            if (m_TimeManager.IsFinite && m_TimeManager.IsOver)
                EndGame(false);
        }
    }

    void EndGame(bool win)
    {
        // unlocks the cursor before leaving the scene, to be able to click buttons
        Input.MouseMode = Input.MouseModeEnum.Visible;

        m_TimeManager.StopRace();

        // Remember that we need to load the appropriate end scene after a delay
        gameState = win ? GameState.Won : GameState.Lost;

        //endGameFadeCanvasGroup.gameObject.SetActive(true); TODO:

        if (win)
        {
            m_SceneToLoad = winSceneName;
            m_TimeLoadEndGameScene = HelperFunctions.GetTime() + endSceneLoadDelay + delayBeforeFadeToBlack;

            // TODO: play a sound on win
            AudioStreamPlayer victorySoundPlayer = new AudioStreamPlayer();
            victorySoundPlayer.Stream = victorySound;
            victorySoundPlayer.Autoplay = false;
            victorySoundPlayer.Bus = "HUDVictory";
            victorySoundPlayer.Play();
            GetTree().Root.AddChild(victorySoundPlayer);

            // create a game message
            winDisplayMessage.delayBeforeShowing = delayBeforeWinMessage;
            disabilityManager.Enable(winDisplayMessage);
        }
        else
        {
            m_SceneToLoad = loseSceneName;
            m_TimeLoadEndGameScene = HelperFunctions.GetTime() + endSceneLoadDelay + delayBeforeFadeToBlack;

            // create a game message
            loseDisplayMessage.delayBeforeShowing = delayBeforeWinMessage;
            disabilityManager.Enable(loseDisplayMessage);
        }
    }
}
