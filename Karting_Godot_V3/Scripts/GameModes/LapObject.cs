using Godot;
using System;
using System.Runtime.Remoting.Messaging;

public class LapObject : Area, IDisability
{
    private bool isActive = true;
    public bool IsActive //interface field
    {
        get => isActive;
        set => isActive = value;
    }

    DisabilityManager disabilityManager;

    [Export(hintString: "Which game mode are you playing?")]
    public GameMode gameMode;

    /* [Tooltip("The amount of time the pickup gives in secs")]
    public float TimeGained;

    [Tooltip("Layers to trigger with")]
    public LayerMask layerMask;

    [Tooltip("The point at which the collect VFX is spawned")]
    public Transform CollectVFXSpawnPoint;

    [Header("Sounds")]

    [Tooltip("Sound played when receiving damages")]
    public AudioClip CollectSound; */

    [Export(hintString: "Is this the first/last lap object?")]
    public bool finishLap;
    [Export]
    public Godot.Collections.Array<NodePath> lapCheckpoints;
    public bool lapOverNextPass;

    public override void _Ready()
    {
        disabilityManager = (DisabilityManager) GetTree().GetRoot().GetNode<Node>(GameConstants.disabilityManagerPath);
        Connect("body_entered", this, "OnBodyEntered");
        OnEnable(); // In Unity OnEnable is also called after awake

        Register(); // in Unity called in Start(), which comes after onEnable
    }

    public void OnBodyEntered(Node Body)
    {
        if (Body.Name != "ArcadeKart")
            return;

        Objective.OnUnregisterPickup?.Invoke(this);
        if(!finishLap)
        {
            ((MeshInstance) this.GetChild(0)).Visible = false;
        }
        else
        {
            foreach (NodePath lapCheckpoint in lapCheckpoints)
            {
                ((MeshInstance) GetNode<Node>(lapCheckpoint).GetChild(0)).Visible = true;
            }
        }
    }

    protected void Register()
    {
        Objective.OnRegisterPickup?.Invoke(this);
    }

    public void OnEnable()
    {
        lapOverNextPass = false;
    }

    public void OnDisable()
    {

    }
}
