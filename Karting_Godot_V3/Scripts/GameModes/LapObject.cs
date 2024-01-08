using Godot;
using System;

public class LapObject : Area, IDisability
{
    public bool isActive
    {
        get => isActive;
        set => isActive = value;
    }

    [Export]
    DisabilityManager disabilityManager;

    [Export(hintString: "Is this the first/last lap object?")]
    public bool finishLap;
    [Export]
    public Godot.Collections.Array<NodePath> lapCheckpoints;
    public bool lapOverNextPass;
    public override void _Ready()
    {
        Connect("body_entered", this, "OnBodyEntered");
        lapOverNextPass = false;
    }

    public void OnBodyEntered(Node Body)
    {
        disabilityManager = (DisabilityManager) GetTree().GetRoot().GetNode<Node>("RaceScene/DisabilityManager");
        if (Body.Name != "ArcadeKart")
            return;

        // TODO: Objective.OnUnregisterPickup?.Invoke(this);
        if(!finishLap)
        {
            ((MeshInstance) this.GetChild(0)).Visible = false;
            disabilityManager.Disable(this);
        }
        else
        {
            foreach (NodePath lapCheckpoint in lapCheckpoints)
            {
                ((MeshInstance) GetNode<Node>(lapCheckpoint).GetChild(0)).Visible = true;
                disabilityManager.Enable(GetNode<Node>(lapCheckpoint));
            }
        }
    }

    public void OnEnable()
    {
        GD.Print("Enabled LapObject");
    }
    
    public void OnDisable()
    {
        GD.Print("Disabled LapObject");
    }
}
