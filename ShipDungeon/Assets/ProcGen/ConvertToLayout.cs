using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ConvertToLayout {

    public Material startMat;

    private List<GameObject> roomList = new List<GameObject>();

    private GameObject target;

    private Edge currentEdge;

    private List<Edge> edgeList;

    List<Edge> expendedPrimList = new List<Edge>();

    private bool isNode0;

    List<Edge> primsList = new List<Edge>();

    List<GameObject> stepRoomList = new List<GameObject>();

    public ConvertToLayout(List<VertexNode> _roomList, GameObject _startRoom, Prims thePrimController)
    {
        startMat =  new Material(Shader.Find("Particles/Additive"));
        startMat.color = Color.red;

        target = _startRoom;

        target.GetComponent<Renderer>().material = startMat;

        primsList = thePrimController.getFinalConnections();

        //get a list of physical rooms, not needed so far?????
        foreach (VertexNode aNode in _roomList)
        {
            roomList.Add(aNode.getParentCell());
        }

        //find the first edge that
        currentEdge = (from x in primsList
                     where x.getNode0().getParentCell() == target 
                     || 
                     x.getNode1().getParentCell() == target
                     select x).First();

        stepRoomList.Add(target);

        int step = -1;

        // loop through this many rooms, we dont need to know which room is which, just the amount
        for (int i = 0; i < roomList.Count; i++)
        {
            step++;
            ConnectRoom(stepRoomList[step]);
        }

    }

    //find and make branches to each dugeon room
    private void ConnectRoom(GameObject _targetRoom)
    {
        GameObject currentRoom = _targetRoom;
        GameObject targetRoom;

        List<Edge> allConnections = (from x in primsList
                                  where (x.getNode0().getParentCell() == currentRoom ||
                                  x.getNode1().getParentCell() == currentRoom)
                                  select x).ToList();

        var usableConnections = allConnections.Except(expendedPrimList);

        List<GameObject> targetRooms = new List<GameObject>();

        //foreach usable edge, find what its connected too, but dont go back up the branch
        foreach(var edge in usableConnections)
        {
            //find the node that isnt a on target
            isNode0 = edge.getNode0().getParentCell() == currentRoom ? true : false;
            if (isNode0)
            {
                targetRoom = edge.getNode1().getParentCell();
                targetRooms.Add(targetRoom);
                stepRoomList.Add(targetRoom);
            }
            else
            {
                targetRoom = edge.getNode0().getParentCell();
                targetRooms.Add(targetRoom);
                stepRoomList.Add(targetRoom);
            }
            expendedPrimList.Add(edge);
        }

        var a = currentRoom.GetComponent<DungeonRoom>();

        // add the parent of the other end to targetRooms
        foreach (var b in targetRooms)
        {
            a.AddBranch(b);
        }
    }

    public void MakeMap()
    {


        //********Make Corridors********//

        //go through each room
        foreach (var room in roomList)
        {
            var currentRoom = room.GetComponent<DungeonRoom>();

            //for each branch
            foreach (var target in currentRoom.GetBranchList())
            {
                //see if they have a common axis 
                //x - left/right
                //z - up/down
                    //make that part of this room have a door in that axis
                

                //else
                //create
                
            
            }
        }

    }


}











//transfer the connection data into rooms so each room knows who it is connected too
//foreach (Edge aEdge in primsList)
//{
//    if (target == aEdge.getNode0().getParentCell() || target == aEdge.getNode1().getParentCell())
//    {
//        //Debug.Log("HALLELUJA!!!");
//        startEdge = aEdge;
//        break;
//    }
//   // aEdge.getNode1().getParentCell();
//    //I want to do this a different way
//    //starting at the furthest away, get a list of rooms, give each a "child" room that is off its branch
//    //end room is the one furthest on the branch

//    //aEdge.getNode0().getParentCell().GetComponent<scRoom>().addConnection(aEdge.getNode1().getParentCell());
//    //aEdge.getNode1().getParentCell().GetComponent<scRoom>().addConnection(aEdge.getNode0().getParentCell());
//}

//int bugCount = 0;
//try
//{
//    //startEdge.getNode1().getParentCell().transform.parent = target.transform;
//}
//catch
//{
//    bugCount++;
//    Debug.Log("Something Bombed! " + bugCount);
//}
//bool complete = false;
//for (var i = -1; i < roomList.Count; i++)
//{
//    //Debug.Log("for loop " + i);
//    foreach (Edge aEdge in primsList)
//    {
//        if (target == aEdge.getNode0().getParentCell() || target == aEdge.getNode1().getParentCell())
//        {
//            //Debug.Log("for loop " + i);
//            //int bugCount = 0;
//            try
//            {
//                startEdge.getNode0().getParentCell().transform.parent = target.transform;
//                target = startEdge.getNode1().getParentCell();
//            }
//            catch
//            {
//                //bugCount++;
//                Debug.Log("Something Bombed!");
//            }
//        }
//    }
//}
