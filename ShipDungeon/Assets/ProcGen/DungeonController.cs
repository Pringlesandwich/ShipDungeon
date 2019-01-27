using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonController : MonoBehaviour {

    bool isFinished = false;

    //the Delaunay Triangulation controller 
    //(Contains incremental Algorithum for construcing a Delaunay Triangulation of a set of verticies)
    private DTController theDTController = new DTController();
    private bool DTFinished = false;

    private Prims thePrimController = new Prims();
    private bool PrimFinished = false;

    ////List of all cells created at in start
    //private ArrayList cellList = new ArrayList();

    //List of cells that have been turned into rooms
    private List<VertexNode> roomList = new List<VertexNode>();

    // Use this for initialization
    void Start () {
        //theDTController.Start();

        GameObject[] GO = GameObject.FindGameObjectsWithTag("Gen");
        foreach (var g in GO)
        {
            VertexNode thisNode = new VertexNode(g.transform.position.x, g.transform.position.z, g.gameObject);
            //cellList.Add(thisNode);
            roomList.Add(thisNode);
        }

        //toAddList = roomList;
        //setupTriangulation(roomList);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFinished)
        {

            //turn large cells into rooms;
            //setRooms();

            //initalize the triangulation
            theDTController.setupTriangulation(roomList);

            isFinished = true;
        }
        else
        {
            if (!DTFinished)
            {
                if (!theDTController.getDTDone())
                {
                    //Debug.Log(theDTController.getDTDone() + " " + Time.deltaTime);
                    theDTController.Update();
                }
                else
                {
                    //Debug.Log("ASDASDASDADS");
                    DTFinished = true;
                    thePrimController.setUpPrims(roomList, theDTController.getTriangulation());
                }
            }
            else
            {
                if(!PrimFinished)
                {
                    thePrimController.stopEdgeDraw();

                    thePrimController.Update();
                    //PrimFinished = true;
                }
            }
        }
    }


}
