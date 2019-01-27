using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertToLayout {


    //public ConvertToLayout()
    //{
    //    //constructor??
    //}

    private List<GameObject> roomList = new List<GameObject>();

    //private VertexNode startNode;

    private GameObject target;
    private Edge startEdge;

    private List<Edge> edgeList;

    public ConvertToLayout(List<VertexNode> _roomList, GameObject _startRoom, Prims thePrimController)
    {

        target = _startRoom;

        //List<Edge> test = thePrimController.getConnections();
        //List<Edge> toDelete = new List<Edge>();

        List<Edge> primsList = new List<Edge>();

        //Debug.Log("AHTESTING " + test.Count);

        foreach (Edge i in thePrimController.getConnections())
        {
            if ((i.getNode0().getVertexPosition() != Vector3.zero && i.getNode1().getVertexPosition() != Vector3.zero) && i.getIsPrims() == true)
            {
                primsList.Add(i);
            }
        }

        Debug.Log("PrimList    " + primsList.Count);

        foreach (VertexNode aNode in _roomList)
        {
            //aNode.getParentCell().AddComponent<DungeonRoom>();
            roomList.Add(aNode.getParentCell());
        }
        //Debug.Log("room list Size: " + roomList.Count);

        //int count = 0;



        edgeList = thePrimController.getConnections();


        //transfer the connection data into rooms so each room knows who it is connected too
        foreach (Edge aEdge in primsList)
        {
            if (target == aEdge.getNode0().getParentCell())
            {
                //Debug.Log("HALLELUJA!!!");
                startEdge = aEdge;
            }
           // aEdge.getNode1().getParentCell();

            //I want to do this a different way
            //starting at the furthest away, get a list of rooms, give each a "child" room that is off its branch

            //end room is the one furthest on the branch


            //aEdge.getNode0().getParentCell().GetComponent<scRoom>().addConnection(aEdge.getNode1().getParentCell());
            //aEdge.getNode1().getParentCell().GetComponent<scRoom>().addConnection(aEdge.getNode0().getParentCell());
        }


        int bugCount = 0;
        try
        {
            //startEdge.getNode1().getParentCell().transform.parent = target.transform;
        }
        catch
        {
            bugCount++;
            Debug.Log("Something Bombed! " + bugCount);
        }

        bool complete = false;

        for (var i = -1; i < roomList.Count; i++)
        {
            //Debug.Log("for loop " + i);
            foreach (Edge aEdge in primsList)
            {
                if (target == aEdge.getNode0().getParentCell())
                {

                    Debug.Log("for loop " + i);
                    //int bugCount = 0;
                    try
                    {
                        startEdge.getNode0().getParentCell().transform.parent = target.transform;
                        target = startEdge.getNode1().getParentCell();
                    }
                    catch
                    {
                        bugCount++;
                        Debug.Log("Something Bombed!");
                    }
                }
            }
        }

    }

}
