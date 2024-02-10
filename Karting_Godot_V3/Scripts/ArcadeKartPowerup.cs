using Godot;
using System;

public class ArcadeKartPowerup : Area
{
    public ArcadeKartVehicleBody.StatPowerup boostStats = new ArcadeKartVehicleBody.StatPowerup
    {
        MaxTime = 3f,
        modifiers = new ArcadeKartVehicleBody.Stats
        {
            TopSpeed = 10f,
            Acceleration = 10f,
            AccelerationCurve = 0.2f,
            Braking = 0f,
            ReverseAcceleration = 0f,
            ReverseSpeed = 0f,
            Steer = 0f,
            CoastingDrag = 0f,
            Grip = 0f,
            AddedGravity = 0f,
        }
    };

    public bool isCoolingDown { get; private set; }
    public float lastActivatedTimestamp { get; private set; }

    public float cooldown = 5f;

    public bool disableGameObjectWhenActivated;
    /* public UnityEvent onPowerupActivated;
    public UnityEvent onPowerupFinishCooldown;

    private TrailRenderer[] laserTrails; */

    [Signal]
    public delegate void enableLaserTrails();
    private string enableLaserTrailSignalName = "enableLaserTrails";
    [Export]
    public NodePath LaserTrailManagerPath;

    public override void _Ready()
    {
        Connect("body_entered", this, "OnTriggerEnter");
        lastActivatedTimestamp = -9999f;

        //connect to gdscript for laser trail plugin
        Spatial laserTrailManager = GetNode<Spatial>(LaserTrailManagerPath);
        Connect(enableLaserTrailSignalName, laserTrailManager, "enableLaserTrails");
    }

    public override void _Process(float delta)
    {
        if (isCoolingDown)
        {
            if (HelperFunctions.GetTime() - lastActivatedTimestamp > cooldown)
            {
                isCoolingDown = false;
                //onPowerupFinishCooldown();
            }
        }
    }

    private void OnTriggerEnter(Node Body)
    {
        if (isCoolingDown) return;
        if (Body.Name != "ArcadeKart")
            return;
        var kart = Body as ArcadeKartVehicleBody;

        lastActivatedTimestamp = HelperFunctions.GetTime();
        // reset elapsed time if this boost has already been used before
        if(this.boostStats.ElapsedTime > this.boostStats.MaxTime)
        {
            this.boostStats.ElapsedTime = 0f;
        }
        kart.AddPowerup(this.boostStats);
        //onPowerupActivated.Invoke();
        isCoolingDown = true;

        //call gd script to activate laser trails
        EmitSignal(enableLaserTrailSignalName);

        //if (disableGameObjectWhenActivated) this.gameObject.SetActive(false);

        // enableLaserTrails(kart);
    }


}
