using Godot;
using System;
using System.Collections;

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
    /* //TODO: play sound when victory
    [Export(hintString: "Sound played on win")]
    public AudioClip victorySound; */
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


    public override void _Ready()
    {
        disabilityManager = (DisabilityManager) GetTree().Root.GetNode<Node>(GameConstants.disabilityManagerPath);

        playerKart = GetNode<ArcadeKartVehicleBody>(kartPath);

        m_ObjectiveManager = GetNode<ObjectiveManager>(objectiveManagerPath);

        m_TimeManager = GetNode<TimeManager>(timeManagerPath);

        //AudioUtility.SetMasterVolume(1); TODO:

        winDisplayMessage = GetNode<DisplayMessage>(winDisplayMessagePath);
        loseDisplayMessage = GetNode<DisplayMessage>(winDisplayMessagePath);
        disabilityManager.Disable(winDisplayMessage);
        disabilityManager.Disable(loseDisplayMessage);

        m_TimeManager.StopRace();
        playerKart.SetCanMove(false);

        //run race countdown animation
        ShowRaceCountdownAnimation();

    }

/*     void Start()
    {
        

        m_ObjectiveManager = FindObjectOfType<ObjectiveManager>();
        DebugUtility.HandleErrorIfNullFindObject<ObjectiveManager, GameFlowManager>(m_ObjectiveManager, this);

        m_TimeManager = FindObjectOfType<TimeManager>();
        DebugUtility.HandleErrorIfNullFindObject<TimeManager, GameFlowManager>(m_TimeManager, this);

        AudioUtility.SetMasterVolume(1);

        winDisplayMessage.gameObject.SetActive(false);
        loseDisplayMessage.gameObject.SetActive(false);

        m_TimeManager.StopRace();
        foreach (ArcadeKart k in karts)
        {
            k.SetCanMove(false);
        }

        //run race countdown animation
        ShowRaceCountdownAnimation();
        StartCoroutine(ShowObjectivesRoutine());

        StartCoroutine(CountdownThenStartRaceRoutine());
    }

    IEnumerator CountdownThenStartRaceRoutine() {
        yield return new WaitForSeconds(3f);
        StartRace();
    }

    void StartRace() {
        foreach (ArcadeKart k in karts)
        {
            k.SetCanMove(true);
        }
        m_TimeManager.StartRace();
    }*/

    void ShowRaceCountdownAnimation() {
        //raceCountdownTrigger.Play(); //TODO:
    }

    /* IEnumerator ShowObjectivesRoutine() {
        while (m_ObjectiveManager.Objectives.Count == 0)
            yield return null;
        yield return new WaitForSecondsRealtime(0.2f);
        for (int i = 0; i < m_ObjectiveManager.Objectives.Count; i++)
        {
           if (m_ObjectiveManager.Objectives[i].displayMessage)m_ObjectiveManager.Objectives[i].displayMessage.Display();
           yield return new WaitForSecondsRealtime(1f);
        }
    } */

/*
    void Update()
    {

        if (gameState != GameState.Play)
        {
            elapsedTimeBeforeEndScene += Time.deltaTime;
            if(elapsedTimeBeforeEndScene >= endSceneLoadDelay)
            {

                float timeRatio = 1 - (m_TimeLoadEndGameScene - Time.time) / endSceneLoadDelay;
                endGameFadeCanvasGroup.alpha = timeRatio;

                float volumeRatio = Mathf.Abs(timeRatio);
                float volume = Mathf.Clamp(1 - volumeRatio, 0, 1);
                AudioUtility.SetMasterVolume(volume);

                // See if it's time to load the end scene (after the delay)
                if (Time.time >= m_TimeLoadEndGameScene)
                {
                    SceneManager.LoadScene(m_SceneToLoad);
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        m_TimeManager.StopRace();

        // Remember that we need to load the appropriate end scene after a delay
        gameState = win ? GameState.Won : GameState.Lost;
        endGameFadeCanvasGroup.gameObject.SetActive(true);
        if (win)
        {
            m_SceneToLoad = winSceneName;
            m_TimeLoadEndGameScene = Time.time + endSceneLoadDelay + delayBeforeFadeToBlack;

            // play a sound on win
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = victorySound;
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = AudioUtility.GetAudioGroup(AudioUtility.AudioGroups.HUDVictory);
            audioSource.PlayScheduled(AudioSettings.dspTime + delayBeforeWinMessage);

            // create a game message
            winDisplayMessage.delayBeforeShowing = delayBeforeWinMessage;
            winDisplayMessage.gameObject.SetActive(true);
        }
        else
        {
            m_SceneToLoad = loseSceneName;
            m_TimeLoadEndGameScene = Time.time + endSceneLoadDelay + delayBeforeFadeToBlack;

            // create a game message
            loseDisplayMessage.delayBeforeShowing = delayBeforeWinMessage;
            loseDisplayMessage.gameObject.SetActive(true);
        }
    } */
}
