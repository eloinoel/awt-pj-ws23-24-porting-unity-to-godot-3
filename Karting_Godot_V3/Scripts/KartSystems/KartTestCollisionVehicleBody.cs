using Godot;
using System;

public class KartTestCollisionVehicleBody : VehicleBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect("body_entered", this, "OnCollisionEnter");
        Connect("body_exited", this, "OnCollisionExit");
    }

    private void OnCollisionEnter(Node body)
    {

    }

    private void OnCollisionExit(Node body)
    {

    }
}
