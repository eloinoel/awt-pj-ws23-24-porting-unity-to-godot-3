using Godot;
using System;

public class LapObject : Area
{
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
        if (Body.Name != "ArcadeKart")
            return;

        // TODO: Objective.OnUnregisterPickup?.Invoke(this);
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
}
