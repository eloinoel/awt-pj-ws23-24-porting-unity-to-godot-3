using Godot;
using System;
using System.Collections.Generic;

public enum GameMode
{
    TimeLimit, Crash, Laps
}

public abstract class Objective : Node, IDisability
{
    [Export(hintString: "Which game mode are you playing?")]
    public GameMode gameMode;

    protected int m_PickupTotal;

    [Export(hintString: "Name of the target object the player will collect/crash/complete for this objective")]
    public string targetName;

    [Export(hintString: "Short text explaining the objective that will be shown on screen")]
    public string title;

    [Export(hintString: "Short text explaining the objective that will be shown on screen")]
    public string description;

    [Export(hintString: "Whether the objective is required to win or not")]
    public bool isOptional;

    [Export(hintString: "Delay before the objective becomes visible")]
    public float delayVisible;


    //Requirements

    [Export(hintString: "Does the objective have a time limit?")]
    public bool isTimed;

    [Export(hintString: "If there is a time limit, how long in secs?")]
    public int totalTimeInSecs;
    public bool isCompleted { get; protected set; }
    public bool isBlocking() => !(isOptional || isCompleted);

//TODO:
    // public UnityAction<UnityActionUpdateObjective> onUpdateObjective;

    //[Export(hintString: "Handle HUD notifications")] // TODO: Check if protected can be exported
    protected NotificationHUDManager m_NotificationHUDManager;
    [Export(hintString: "Handle HUD objectives")]
    protected NodePath m_ObjectiveHUDManagerPath;
    protected ObjectiveHUDManager m_ObjectiveHUDManager;

    public static Action<LapObject> OnRegisterPickup;
    public static Action<LapObject> OnUnregisterPickup;

    [Export(hintString: "show objective at start of game")]
    public DisplayMessage displayMessage; //TODO: implement

    private List<LapObject> pickups = new List<LapObject>();

    public List<LapObject> Pickups => pickups;
    public int NumberOfPickupsTotal { get; private set; }
    public int NumberOfPickupsRemaining => Pickups.Count;
    private bool isActive = true;
    public bool IsActive
    {
        get => isActive;
        set => isActive = value;
    }

    public int NumberOfActivePickupsRemaining()
    {
        int total = 0;
        for (int i = 0; i < Pickups.Count; i++)
        {
            if (Pickups[i].IsActive) total++;
        }

        return total;
    }

    protected abstract void ReachCheckpoint(int remaining);

    public override void _Ready()
    {
        base._Ready();
        m_ObjectiveHUDManager = GetNode<ObjectiveHUDManager>(m_ObjectiveHUDManagerPath);
        OnEnable(); // In Unity OnEnable is also called after Awake
    }

    public void OnEnable()
    {
        OnRegisterPickup += RegisterPickup;
        OnUnregisterPickup += UnregisterPickup;
    }

    protected void Register()
    {
        // add this objective to the list contained in the objective manager
        ObjectiveManager.RegisterObjective(this);

        // NOTE: We dont find object by types here anymore, because we simply export ObjectiveHUDManager and NotificationHUDManager and set them through the editor
        // register this objective in the ObjectiveHUDManager
/*         m_ObjectiveHUDManager = FindObjectOfType<ObjectiveHUDManager>();
        DebugUtility.HandleErrorIfNullFindObject<ObjectiveHUDManager, Objective>(m_ObjectiveHUDManager, this); */
        m_ObjectiveHUDManager.RegisterObjective(this); // TODO: implement
        GD.Print("Register Objective");

        // register this objective in the NotificationHUDManager
/*         m_NotificationHUDManager = FindObjectOfType<NotificationHUDManager>();
        DebugUtility.HandleErrorIfNullFindObject<NotificationHUDManager, Objective>(m_NotificationHUDManager, this); */
        //m_NotificationHUDManager.RegisterObjective(this);
    }

    public void UpdateObjective(string descriptionText, string counterText, string notificationText)
    {
        // TODO: onUpdateObjective?.Invoke(new UnityActionUpdateObjective(this, descriptionText, counterText, false, notificationText));
    }

    public void CompleteObjective(string descriptionText, string counterText, string notificationText)
    {
        isCompleted = true;
        UpdateObjective(descriptionText, counterText, notificationText);

        // unregister this objective form both HUD managers
        m_ObjectiveHUDManager.UnregisterObjective(this);
        m_NotificationHUDManager.UnregisterObjective(this);
    }

    public virtual string GetUpdatedCounterAmount()
    {
        return "";
    }

    public void RegisterPickup(LapObject pickup)
    {
        if (pickup.gameMode != gameMode) return;

        Pickups.Add(pickup);

        NumberOfPickupsTotal++;
    }

    public void UnregisterPickup(LapObject pickupCollected)
    {
        if (pickupCollected.gameMode != gameMode) return;

        // removes the pickup from the list, so that we can keep track of how many are left on the map
        if (pickupCollected.gameMode == GameMode.Laps)
        {
            pickupCollected.IsActive = false;

            LapObject lapObject = (LapObject) pickupCollected;

            if (!lapObject.finishLap) return;

            if (!lapObject.lapOverNextPass)
            {
                // TODO: TimeDisplay.OnUpdateLap();
                lapObject.lapOverNextPass = true;
                return;
            }

            if (NumberOfActivePickupsRemaining() != 0) return;

            ReachCheckpoint(0);
            ResetPickups();
            // TODO: TimeDisplay.OnUpdateLap();

        }
        else
        {
            ReachCheckpoint(NumberOfPickupsRemaining - 1);
            Pickups.Remove(pickupCollected);
            /* TODO: if (gameMode == GameMode.Laps)
                KartGame.Track.TimeDisplay.OnUpdateLap(); */
        }
    }

    public void ResetPickups()
    {
        for (int i = 0; i < Pickups.Count; i++)
        {
            Pickups[i].IsActive = true;
        }
    }

    public void OnDisable()
    {
        OnRegisterPickup -= RegisterPickup;
        OnUnregisterPickup -= UnregisterPickup;
    }

}

/* public class UnityActionUpdateObjective
{
    public Objective objective;
    public string descriptionText;
    public string counterText;
    public bool isComplete;
    public string notificationText;

    public UnityActionUpdateObjective(Objective objective, string descriptionText, string counterText, bool isComplete, string notificationText)
    {
        this.objective = objective;
        this.descriptionText = descriptionText;
        this.counterText = counterText;
        this.isComplete = isComplete;
        this.notificationText = notificationText;
    }
} */
