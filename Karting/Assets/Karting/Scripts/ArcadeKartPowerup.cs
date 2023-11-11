using KartGame.KartSystems;
using UnityEngine;
using UnityEngine.Events;

public class ArcadeKartPowerup : MonoBehaviour
{

    public ArcadeKart.StatPowerup boostStats = new ArcadeKart.StatPowerup
    {
        MaxTime = 5
    };

    public bool isCoolingDown { get; private set; }
    public float lastActivatedTimestamp { get; private set; }

    public float cooldown = 5f;

    public bool disableGameObjectWhenActivated;
    public UnityEvent onPowerupActivated;
    public UnityEvent onPowerupFinishCooldown;

    private TrailRenderer[] laserTrails;

    private void Awake()
    {
        lastActivatedTimestamp = -9999f;
    }


    private void Update()
    {
        if (isCoolingDown)
        {

            if (Time.time - lastActivatedTimestamp > cooldown)
            {
                //finished cooldown!
                isCoolingDown = false;
                onPowerupFinishCooldown.Invoke();
            }

        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (isCoolingDown) return;

        var rb = other.attachedRigidbody;
        if (rb)
        {
            var kart = rb.GetComponent<ArcadeKart>();

            if (kart)
            {
                lastActivatedTimestamp = Time.time;
                kart.AddPowerup(this.boostStats);
                onPowerupActivated.Invoke();
                isCoolingDown = true;

                if (disableGameObjectWhenActivated) this.gameObject.SetActive(false);

                enableLaserTrails(kart);
            }
        }
    }

    private void enableLaserTrails(ArcadeKart kart)
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
    }

}
