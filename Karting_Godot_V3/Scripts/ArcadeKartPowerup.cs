using Godot;
using System;

public class ArcadeKartPowerup : Area
{
    public ArcadeKartVehicleBody.StatPowerup boostStats = new ArcadeKartVehicleBody.StatPowerup
    {
        MaxTime = 5f,
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

    public override void _Ready()
    {
        Connect("body_entered", this, "OnTriggerEnter");
        lastActivatedTimestamp = -9999f;
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

        //if (disableGameObjectWhenActivated) this.gameObject.SetActive(false);

        // enableLaserTrails(kart);
    }

    /* private void enableLaserTrails(ArcadeKart kart)
    {
        var wheels = kart.transform.Find("Wheels");
        if (wheels)
        {
            this.laserTrails = new TrailRenderer[2];
            // get wheels with laser trail
            var wheelRearLeft = wheels.transform.Find("WheelRearLeft");
            if (wheelRearLeft)
            {
                var laserTrail = wheelRearLeft.GetChild(0);
                if (laserTrail)
                {
                    this.laserTrails[0] = laserTrail.GetComponent<TrailRenderer>();
                    this.laserTrails[0].enabled = true;
                }
                    
            }

            var wheelRearRight = wheels.transform.Find("WheelRearRight");
            if (wheelRearRight)
            {
                var laserTrail = wheelRearRight.GetChild(0);
                if (laserTrail)
                {
                    this.laserTrails[1] = laserTrail.GetComponent<TrailRenderer>();
                    this.laserTrails[1].enabled = true;
                }
            }

            Invoke("disableLaserTrails", boostStats.MaxTime);
        }
    }

    private void disableLaserTrails()
    {
        foreach(var trailRenderer in this.laserTrails)
        {
            trailRenderer.enabled = false;
        }
    } */
}
