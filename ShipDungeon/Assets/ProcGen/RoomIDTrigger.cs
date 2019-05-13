using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomIDTrigger : MonoBehaviour {


    public LayoutController layoutController;

    public List<int> roomIDs = new List<int>();

    public void SetLayoutControllerReference(LayoutController lc)
    {
        layoutController = lc;
        //Debug.Log("DONE!");
    }

    public void addRoomID(int ID)
    {
        roomIDs.Add(ID);
    }

    public void TriggerAllIDs()
    {
        //Debug.Log("HERE NOW");
        foreach(var i in roomIDs)
        {
            layoutController.SetEnemyAgroByRoomID(i);
        }
    }

}
