using Godot;
using System;

//TODO: delete this at the end if dont need for testing anymore
public class KartTestCollisionVehicleBody : VehicleBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("loaded collision test kart");
        Connect("body_entered", this, "_OnCollisionEnter");
        Connect("body_exited", this, "_OnCollisionExit");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    private void _OnCollisionEnter(Node body)
    {
        GD.Print("Collision detected");
    }

    /* PREV: void OnCollisionExit(Collision collision) => m_HasCollision = false; */
    private void _OnCollisionExit(Node body)
    {
        GD.Print("Exited detected collision");
    }
}
