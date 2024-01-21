using Godot;
using System;

public class ObjectiveCompleteLaps : Objective
{
    [Export(hintString: "How many laps should the player complete before the game is over?")]
    public int lapsToComplete = 3;

    [Export(hintString: "Start sending notification about remaining laps when this amount of laps is left")]
    public int notificationLapsRemainingThreshold = 1;

    [Export(hintString: "timeDisplay prefab path")]
    public NodePath timeDisplayNodePath;
    private TimeDisplay timeDisplay;

    public int currentLap { get; private set; }

    public override void _Ready()
    {
        // ---- Awake ----
        base._Ready();
        currentLap = 0;

        // set a title and description specific for this type of objective, if it hasn't one
        if (string.IsNullOrEmpty(title))
            title = $"Complete {lapsToComplete} {targetName}s";

        timeDisplay = GetNode<TimeDisplay>(timeDisplayNodePath);

        // ---- Start ----
        Start();
    }

    private async void Start()
    {
        TimeManager.OnSetTime(totalTimeInSecs, isTimed, gameMode);
        timeDisplay.CallDeferred("SetLaps", lapsToComplete);
        //TimeDisplay.OnSetLaps(lapsToComplete);
        await ToSignal(GetTree(), "idle_frame");
        Register();
    }

    protected override void ReachCheckpoint(int remaining)
    {

        if (isCompleted)
            return;

        currentLap++;

        int targetRemaining = lapsToComplete - currentLap;

        // update the objective text according to how many enemies remain to kill
        if (targetRemaining == 0)
        {
            CompleteObjective(string.Empty, GetUpdatedCounterAmount(),
                "Objective complete: " + title);
        }
        else if (targetRemaining == 1)
        {
            string notificationText = notificationLapsRemainingThreshold >= targetRemaining
                ? "One " + targetName + " left"
                : string.Empty;
            UpdateObjective(string.Empty, GetUpdatedCounterAmount(), notificationText);
        }
        else if (targetRemaining > 1)
        {
            // create a notification text if needed, if it stays empty, the notification will not be created
            string notificationText = notificationLapsRemainingThreshold >= targetRemaining
                ? targetRemaining + " " + targetName + "s to collect left"
                : string.Empty;

            UpdateObjective(string.Empty, GetUpdatedCounterAmount(), notificationText);
        }

    }

    public override string GetUpdatedCounterAmount()
    {
        return currentLap + " / " + lapsToComplete;
    }
}
