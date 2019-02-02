using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeleeHitBox : MonoBehaviour {

    public List<GameObject> targets = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        //if not player and has damage thing
        if (other.GetComponent<DamageHandler>() && other.gameObject.tag != "Player")
        {
            Debug.Log("Added: " + other.gameObject.name);
            targets.Add(other.gameObject);
        }
        //add to list
    }

    private void OnTriggerExit(Collider other)
    {
        //if in list then remove from list
        if(targets.Any(x => x.gameObject == other.gameObject))
        {
            //Debug.Log("Removed: " + other.gameObject.name);
            targets.Remove(other.gameObject);
        }
    }

    public List<GameObject> GetTargets()
    {
        return targets;
    }

}
