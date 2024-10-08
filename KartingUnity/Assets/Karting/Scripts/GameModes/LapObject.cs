﻿using UnityEngine;

/// <summary>
/// This class inherits from TargetObject and represents a LapObject.
/// </summary>
public class LapObject : TargetObject
{
    [Header("LapObject")]
    [Tooltip("Is this the first/last lap object?")]
    public bool finishLap;

    [HideInInspector]
    public bool lapOverNextPass;

    void Start() {
        Register();
    }

    void OnEnable()
    {
        lapOverNextPass = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!((layerMask.value & 1 << other.gameObject.layer) > 0 && other.CompareTag("Player")))
            return;

        Objective.OnUnregisterPickup?.Invoke(this);

        if(this.tag == "Lap")
        {
            ((MeshRenderer) this.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>()).enabled = false;
        }
        else if(this.tag == "Finish")
        {
            //Debug.Log("Finish");
            // make all lap objects visible again
            GameObject[] lapObjects = GameObject.FindGameObjectsWithTag("Lap");

            foreach (GameObject lapObj in lapObjects)
            {
                if(lapObj.tag == "Lap")
                {
                    //Debug.Log("Checkpoint");
                    ((MeshRenderer)lapObj.transform.GetChild(0).GetComponent<MeshRenderer>()).enabled = true;
                }
            }
        }
    }
}
