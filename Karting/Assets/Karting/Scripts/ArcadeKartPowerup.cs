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

        // get the player object that hits the powerup
        // GameObject player = other.gameObject;
        // Debug.Log(player.name);


        // // get player 
        GameObject player = GameObject.FindWithTag("LaserTrail");
        player.gameObject.SetActive(true);

        // // get the children of the playerobject 
        // Transform[] children = player.GetComponentsInChildren<Transform>();
        // foreach (Transform child in children)
        // {
        //     Debug.Log(child.gameObject.name);
        //     if (child.gameObject.name == "LaserTrail")
        //     {
        //         // activate the trail renderer
        //         //child.getComponent<TrailRenderer>().enabled = true;
        //         Debug.Log("Laser Activated");
        //     }
        //     // //if the child is 'LaserTrail' activate the laser
        //     // if (child.GameObject == "LaserTrail")
        //     // {
        //     //     child.gameObject.SetActive(true);
        //     //     Debug.Log("Laser Activated");
        //     // }
        // }

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
            }
        }
    }

}
