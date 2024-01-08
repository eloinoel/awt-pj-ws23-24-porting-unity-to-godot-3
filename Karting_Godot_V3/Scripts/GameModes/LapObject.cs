using Godot;
using System;
using System.Runtime.Remoting.Messaging;

public class LapObject : Area, IDisability
{
    private bool isActive;
    public bool IsActive //interface field
    {
        get => isActive;
        set => isActive = value;
    }

    DisabilityManager disabilityManager;

    [Export(hintString: "Is this the first/last lap object?")]
    public bool finishLap;
    [Export]
    public Godot.Collections.Array<NodePath> lapCheckpoints;
    public bool lapOverNextPass;


    LapObject lapdance;
    public override void _Ready()
    {
        Connect("body_entered", this, "OnBodyEntered");
        lapOverNextPass = false;

        disabilityManager = (DisabilityManager) GetTree().GetRoot().GetNode<Node>("RaceScene/DisabilityManager");
/*         if(this.Name == "LapCheckpoint2")
        {
            lapdance = (LapObject) GetParent().GetNode("LapCheckpoint");
            disabilityManager.Disable(lapdance);
        } */
    }

/*     float timer = 0f;
    bool doOnce = true;
    public override void _Process(float delta)
    {
        base._Process(delta);
        timer += delta;
        if(timer >= 2 && doOnce && this.Name == "LapCheckpoint2")
        {
            GD.Print(this.Name + ", doOnce: " + doOnce);
            disabilityManager.Enable(lapdance);
            doOnce = false;
        }
    } */

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

    public void OnEnable()
    {
        GD.Print("Enabled LapObject");
    }

    public void OnDisable()
    {
        GD.Print("Disabled LapObject");
    }
}
