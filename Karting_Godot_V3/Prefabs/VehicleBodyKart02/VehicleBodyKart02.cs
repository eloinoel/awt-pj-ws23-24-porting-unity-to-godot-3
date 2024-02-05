using Godot;
using System;

public class VehicleBodyKart02 : VehicleBody
{
    const float MAX_STEER = 0.8f;
    const float ENGINE_POWER = 300f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        Steering = Mathf.MoveToward(Steering, Input.GetAxis("right", "left") * MAX_STEER, delta * 2.5f);
        EngineForce = Input.GetAxis("backward", "forward") * ENGINE_POWER;

        //Video 7:16 https://www.youtube.com/watch?v=5m7nBj98rx4&t=590s, Fix Kart first
        GD.Print("Steering: " + Steering + ", EngineForce: " + EngineForce);
    }
}
