using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using UnityEngine.SceneManagement;


public class LayoutController : MonoBehaviour {

    //private GridType gridType = new GridType();

    public GameObject testFloor;
    public GameObject testWall;
    public GameObject testDoor;
    public GameObject testBlock;

    public GameObject Floor;
    public GameObject Wall;
    public GameObject Door;
    public GameObject Block;

    public bool iterate = false;

    struct GridSpace {

        public int ID;

        public int roomID;

        public float x, z;

        public GridType gsSpaceType;

        public bool hasObject;

        public bool wallNorth;
        public bool wallEast;
        public bool wallSouth;
        public bool wallWest;

        public bool doorNorth;    
        public bool doorEast;
        public bool doorSouth;
        public bool doorWest;

        public GridSpace(int id, float _x, float _z, GridType gt) : this()
        {
            ID = id;

            x = _x;
            z = _z;
            gsSpaceType = gt;

            hasObject = false;

            wallNorth = false;
            wallEast = false;
            wallSouth = false;
            wallWest = false;

            doorNorth = false;
            doorEast = false;
            doorSouth = false;
            doorWest = false;

        }

        public void setWalls(bool N, bool E, bool S, bool W)
        {
            wallNorth = N;
            wallEast = E;
            wallSouth = S;
            wallWest = W;
        }
    }

    List<GridSpace> theGrid = new List<GridSpace>();
	
    public void setRooms(List<DungeonRoom> _rooms)
    {
        if (iterate)
        {
            StartCoroutine(genFloorsTestIterate(_rooms));
        }
        else
        {
            SetGrid(_rooms);
        }
    }

    public void SetGrid(List<DungeonRoom> _dungeonRooms)
    {
        bool wallNorth;
        bool wallSouth;
        bool wallEast;
        bool wallWest;

        float xStart;
        float zStart;

        GridSpace gs; // I want this to be a list of data that can be used to create a map

        int id = 0;

        foreach (var i in _dungeonRooms)
        {
            // start on + 0.5 instead of minus as it will + 1 at start
            xStart = i.transform.position.x - ((i.sizeX / 2) + 0.5f); 
            zStart = i.transform.position.z - ((i.sizeZ / 2) + 0.5f);
            var zRestart = zStart;

            wallNorth = false;
            wallSouth = false;
            wallEast = false;
            wallWest = false;

            for (var x = 0; x < i.sizeX; x++)
            {           
                zStart = zRestart;
                //can be refractured into some sort of bool class
                if(x == 0)
                {
                    wallNorth = false;
                    wallEast = false;
                    wallSouth = false;                 
                    wallWest = true;
                }
                else if(x == i.sizeX - 1)
                {
                    wallNorth = false;
                    wallEast = true;
                    wallSouth = false;
                    wallWest = false;
                }
                else
                {
                    wallNorth = false;
                    wallEast = false;
                    wallSouth = false;
                    wallWest = false;
                }

                xStart = xStart + 1;

                for (var z = 0; z < i.sizeZ; z++)
                {
                    if (z == 0)
                    {
                        wallNorth = false;
                        wallSouth = true;
                    }
                    else if (z == i.sizeZ - 1)
                    {
                        wallNorth = true;
                        wallSouth = false;
                    }
                    else
                    {
                        wallNorth = false;
                        wallSouth = false;
                    }

                    id++;
 
                    zStart = zStart + 1;
                    gs = new GridSpace(id, xStart, zStart, GridType.floor);
                    gs.setWalls(wallNorth, wallEast, wallSouth, wallWest);
                    gs.roomID = i.ID;
                    

                    int test = Random.Range(0, 12);
                    if (test < 2)
                    {
                        gs.hasObject = true;
                    }
                    theGrid.Add(gs);
                }
            }
        }

        foreach (var i in _dungeonRooms)
        {
            //if this room has any branches, dig a corridor to each one.
            var branchList = i.GetBranchList();
            if (branchList != null && branchList.Count > 0)
            {
                for (var j = 0; j < branchList.Count; j++)
                {
                    DigCorridor(i, branchList[j].GetComponent<DungeonRoom>());
                }
            }
        }
        //LayTestGrid();

        LayGrid();
    } 

    //corridor builder
    public void DigCorridor(DungeonRoom Room1, DungeonRoom Room2)
    {
        DungeonRoom mainRoom;
        DungeonRoom targetRoom;

        Vector3 deltaVector = (Room2.transform.position - Room1.transform.position);
        Vector3 deltaDirection = new Vector3(1, 0, 1);
        var angle = Vector3.Angle(deltaDirection, deltaVector);

        if(angle < 90)
        {
            mainRoom = Room1;
            targetRoom = Room2;
        }
        else
        {
            mainRoom = Room2;
            targetRoom = Room1;
        }

        var mainRoomPos = mainRoom.transform.position;
        var targetRoomPos = targetRoom.transform.position;

        // get a gs in room1 
        var MainGs = new Vector3(mainRoomPos.x + 0.5f, 0, mainRoomPos.z + 0.5f);

        Vector3 isNorthCheck = (targetRoom.transform.position - mainRoom.transform.position);

        bool isNorth = (isNorthCheck.z > isNorthCheck.x) ? true : false;

        //Debug.Log("MainID: " + mainRoom.ID + " , TargetID: " + targetRoom.ID + " , North: " + isNorth);


        if (isNorth)
        {
            //find smalles value east
            var mainEast = mainRoomPos.x + ((mainRoom.sizeX / 2) - 0.5f);
            var targetEast = targetRoomPos.x + ((targetRoom.sizeX / 2) - 0.5f);
            float deltaEast = 0;
            if(mainEast < targetEast)
            {
                deltaEast = mainRoomPos.x + ((mainRoom.sizeX / 2) - 0.5f);
            }
            else
            {
                deltaEast = targetRoomPos.x + ((targetRoom.sizeX / 2) - 0.5f);
            }
          
            //find the smallest value west
            var mainWest = mainRoomPos.x - ((mainRoom.sizeX / 2) - 0.5f);     
            var targetWest = targetRoomPos.x - ((targetRoom.sizeX / 2) - 0.5f);
            float deltaWest = 0;
            if (mainWest > targetWest)
            {
                deltaWest = mainRoomPos.x - ((mainRoom.sizeX / 2) - 0.5f);
            }
            else
            {
                deltaWest = targetRoomPos.x - ((targetRoom.sizeX / 2) - 0.5f);
            }

            bool isCornerPiece = false;
            if(mainEast < targetWest || mainWest > targetEast)
            {
                isCornerPiece = true;
            }

            int start = Mathf.RoundToInt(deltaEast - 0.5f);
            int finish = Mathf.RoundToInt(deltaWest - 0.5f);

            var doorPos = (Random.Range(start, finish) + 0.5f);
            var zPos = mainRoomPos.z + ((mainRoom.sizeZ / 2) - 0.5f);

            if (!isCornerPiece)
            {
                var doorSpace = (from x in theGrid where x.x == doorPos && x.z == zPos select x).First();
                doorSpace.doorNorth = true;
                theGrid.Remove((from x in theGrid where x.x == doorPos && x.z == zPos select x).First());
                theGrid.Add(doorSpace);

                bool isDigging = false;

                //walk up and place corridors until you hit the other room, then change that to a no wall, if a corridor was made
                if (zPos < (targetRoomPos.z - (targetRoom.sizeZ / 2)))
                {
                    zPos = zPos + 1;
                    while (zPos < (targetRoomPos.z - (targetRoom.sizeZ / 2) + 1.5f)) // 1.5 so that it hits the other side in this while loop
                    {
                        GridSpace gs;
                        if (zPos < (targetRoomPos.z - (targetRoom.sizeZ / 2)))
                        {
                            //NEED TO SORT ID - maybe make id global and ++ it here too?
                            gs = new GridSpace(666, doorPos, zPos, GridType.floor);
                            gs.setWalls(false, true, false, true);
                            theGrid.Add(gs);
                            zPos = zPos + 1;
                            isDigging = true;

                        }
                        else if (isDigging)
                        {
                            //change one above
                            var southDoor = (from x in theGrid where x.x == doorPos && x.z == zPos select x).First();
                            southDoor.doorSouth = true;
                            //Debug.Log("ASDSDAAAAAA");
                            theGrid.Remove((from x in theGrid where x.x == doorPos && x.z == zPos select x).First());
                            theGrid.Add(southDoor);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                Debug.Log("CORNER PIECE ON ID: " + mainRoom.ID);
            }
        }
        // IS EAST
        else
        {
            //find smalles value north
            var mainNorth = mainRoomPos.z + ((mainRoom.sizeZ / 2) - 0.5f);
            var targetNorth = targetRoomPos.z + ((targetRoom.sizeZ / 2) - 0.5f);
            float deltaNorth = 0;
            if (mainNorth < targetNorth)
            {
                deltaNorth = mainRoomPos.z + ((mainRoom.sizeZ / 2) - 0.5f);
            }
            else
            {
                deltaNorth = targetRoomPos.z + ((targetRoom.sizeZ / 2) - 0.5f);
            }

            //find the smallest value west
            var mainSouth = mainRoomPos.z - ((mainRoom.sizeZ / 2) - 0.5f);
            var targetSouth = targetRoomPos.z - ((targetRoom.sizeZ / 2) - 0.5f);
            float deltaSouth = 0;
            if (mainSouth > targetSouth)
            {
                deltaSouth = mainRoomPos.z - ((mainRoom.sizeZ / 2) - 0.5f);
            }
            else
            {
                deltaSouth = targetRoomPos.z - ((targetRoom.sizeZ / 2) - 0.5f);
            }

            bool isCornerPiece = false;
            if (mainNorth < targetSouth || mainSouth > targetNorth)
            {
                Debug.Log("EAST TO WEST CORNER PIECE!!!");
                isCornerPiece = true;
            }

            int start = Mathf.RoundToInt(deltaNorth - 0.5f);
            int finish = Mathf.RoundToInt(deltaSouth - 0.5f);

            var doorPos = (Random.Range(start, finish) + 0.5f);
            var xPos = mainRoomPos.x + ((mainRoom.sizeX / 2) - 0.5f);

            if (!isCornerPiece)
            {
                var doorSpace = (from x in theGrid where x.x == xPos && x.z == doorPos select x).First();
                doorSpace.doorEast = true;
                theGrid.Remove((from x in theGrid where x.x == xPos && x.z == doorPos select x).First());
                theGrid.Add(doorSpace);

                bool isDigging = false;

                //walk up and place corridors until you hit the other room, then change that to a no wall, if a corridor was made
                if (xPos < (targetRoomPos.x - (targetRoom.sizeX / 2)))
                {
                    xPos = xPos + 1;
                    while (xPos < (targetRoomPos.x - (targetRoom.sizeX / 2) + 1.5f)) // 1.5 so that it hits the other side in this while loop
                    {
                        GridSpace gs;
                        if (xPos < (targetRoomPos.x - (targetRoom.sizeX / 2)))
                        {
                            //NEED TO SORT ID - maybe make id global and ++ it here too?
                            gs = new GridSpace(666, xPos, doorPos, GridType.floor);
                            gs.setWalls(true, false, true, false);
                            theGrid.Add(gs);
                            xPos = xPos + 1;
                            isDigging = true;

                        }
                        else if (isDigging)
                        {
                            //change one above
                            var westDoor = (from x in theGrid where x.x == xPos && x.z == doorPos select x).First();
                            westDoor.doorWest = true;
                            //Debug.Log("ASDSDAAAAAA");
                            theGrid.Remove((from x in theGrid where x.x == xPos && x.z == doorPos select x).First());
                            theGrid.Add(westDoor);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                Debug.Log("CORNER PIECE ON ID: " + mainRoom.ID);

                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
                
            }
        }
        bool old = false;
        if (old)
        {


        //    DungeonRoom mainRoom;
        //    DungeonRoom targetRoom;

        //    Vector3 Room1Pos = Room1.transform.position;
        //    Vector3 Room2Pos = Room2.transform.position;


        //    //get main room - this will half the code I have to do later!

        //    //north








        //    mainRoom = Room1;
        //    targetRoom = Room2;

        //    Vector3 mainPos = mainRoom.transform.position;
        //    Vector3 targetPos = targetRoom.transform.position;

        //    bool targetNorth = targetPos.z >= mainPos.z;
        //    bool targetEast = targetPos.x >= mainPos.x;

        //    bool isCollidingNorth = false;
        //    bool isCollidingEast = false;

        //    bool isCollidingSouth = false;
        //    bool isCollidingWest = false;

        //    bool needsCorridor = false;


        //    //if (targetNorth)
        //    //{
        //    //    Debug.Log("main: " + mainRoom.ID + " , pos:" + (mainPos.z + (mainRoom.sizeZ / 2)));// + " , " + mainPos.x);
        //    //    Debug.Log("target: " + targetRoom.ID + " , pos: " + (targetPos.z - (mainRoom.sizeZ / 2)));// + " , " + targetPos.x);
        //    //}



        //    //north colision
        //    if (targetNorth && ((mainPos.z + ((mainRoom.sizeZ / 2)) == (targetPos.z - (targetRoom.sizeZ / 2)))))
        //    {
        //        isCollidingNorth = true;
        //    }
        //    else if (targetNorth && (mainPos.z - ((mainRoom.sizeZ / 2)) == (targetPos.z + (targetRoom.sizeZ / 2))))
        //    {
        //        isCollidingSouth = true;
        //    }



        //    //east collision
        //    if (targetEast && ((mainPos.x + ((mainRoom.sizeX / 2)) == (targetPos.x - (targetRoom.sizeX / 2)))))
        //    {
        //        isCollidingEast = true;
        //    }
        //    else if (targetEast && (mainPos.x - ((mainRoom.sizeX / 2)) == (targetPos.x + (targetRoom.sizeX / 2))))
        //    {
        //        isCollidingWest = true;
        //    }

        //    //if (!isCollidingNorth && !isCollidingEast && !isCollidingSouth && !isCollidingWest)
        //    //{
        //    //    needsCorridor = true;
        //    //}
        //    //else
        //    //{

        //    int start;
        //    int finish;

        //    //this else if will make it so it prioritises in this order
        //    if (isCollidingNorth)
        //    {

        //        //find a mutual point and make that gridspace have a doorNorth true
        //        if (targetEast)
        //        {
        //            start = Mathf.RoundToInt(targetPos.x - (targetRoom.sizeX / 2));
        //            finish = Mathf.RoundToInt(mainPos.x + (mainRoom.sizeX / 2));
        //            //Debug.Log("AADSDA " + (finish - start));
        //            //test
        //            if (finish > start)
        //            {
        //                var doorPos = (Random.Range(start, finish) + 0.5f);
        //                var zPos = mainPos.z + (((mainRoom.sizeZ / 2) - 1) + 0.5f);
        //                //Debug.Log("Door: " + doorPos + " , zPos: " + zPos);
        //                var doorSpace = (from x in theGrid where x.x == doorPos && x.z == zPos select x).First();
        //                doorSpace.doorNorth = true;

        //                theGrid.Remove((from x in theGrid where x.x == doorPos && x.z == zPos select x).First());
        //                theGrid.Add(doorSpace);

        //                //Debug.Log("DOOR NORTH");
        //            }
        //        }
        //        else
        //        {
        //            start = Mathf.RoundToInt(mainPos.x - (mainRoom.sizeX / 2));
        //            finish = Mathf.RoundToInt(targetPos.x + (targetRoom.sizeX / 2));
        //            //Debug.Log("AADSDA " + (finish - start));
        //            //test
        //            if (finish > start)
        //            {
        //                var doorPos = (Random.Range(start, finish) + 0.5f);
        //                var zPos = mainPos.z + (((mainRoom.sizeZ / 2) - 1) + 0.5f);
        //                //var gridID = (from x in theGrid where x.x == doorPos && x.z == zPos select x.ID).First();

        //                var doorSpace = (from x in theGrid where x.x == doorPos && x.z == zPos select x).First();
        //                doorSpace.doorNorth = true;

        //                theGrid.Remove((from x in theGrid where x.x == doorPos && x.z == zPos select x).First());
        //                theGrid.Add(doorSpace);
        //                //doorSpace.doorNorth = true;               
        //                //Debug.Log("DOOR NORTH");
        //            }
        //        }
        //    }

        //    if (isCollidingEast)
        //    {
        //        Debug.Log("CollEast");

        //        //find a mutual point and make that gridspace have a doorNorth true
        //        if (targetNorth)
        //        {
        //            Debug.Log("TargetNorth");
        //            start = Mathf.RoundToInt(mainPos.z + (mainRoom.sizeZ / 2));
        //            finish = Mathf.RoundToInt(targetPos.z - (targetRoom.sizeZ / 2));
        //            if (start > finish)
        //            {
        //                var doorPos = (Random.Range(start, finish) + 0.5f);
        //                var xPos = mainPos.x + (((mainRoom.sizeX / 2) + 0) + 0.5f);
        //                var doorSpace = (from x in theGrid where x.x == xPos && x.z == doorPos select x).First();
        //                doorSpace.doorEast = true;
        //                theGrid.Remove((from x in theGrid where x.x == xPos && x.z == doorPos select x).First());
        //                theGrid.Add(doorSpace);
        //            }
        //        }
        //        else
        //        {
        //            Debug.Log("TargetSouth");
        //            start = Mathf.RoundToInt(targetPos.z + (targetRoom.sizeZ / 2));
        //            finish = Mathf.RoundToInt(mainPos.z - (mainRoom.sizeZ / 2));
        //            if (start > finish)
        //            {
        //                var doorPos = (Random.Range(start, finish) + 0.5f);
        //                var xPos = mainPos.x + (((mainRoom.sizeX / 2) + 0) + 0.5f);
        //                var doorSpace = (from x in theGrid where x.x == xPos && x.z == doorPos select x).First();
        //                doorSpace.doorEast = true;
        //                theGrid.Remove((from x in theGrid where x.x == xPos && x.z == doorPos select x).First());
        //                theGrid.Add(doorSpace);
        //            }
        //        }
        //    }
        //    //else if (isCollidingSouth)
        //    //{

        //    //}
        //    //else if (isCollidingWest)
        //    //{

        //    //}
        //    else
        //    {
        //        needsCorridor = true;
        //    }
        //    //}
        }

    }

    //THESE BELOW WILL BE REMOVED AND DONE ELSEWHERE

    //THIS IS OLD AND WONT WORK
    IEnumerator genFloorsTestIterate(List<DungeonRoom> _dungeonRooms)
    {
        //bool isFirstX;
        //bool isFirstZ;
        //bool isLastX;
        //bool isLastZ;

        float xStart;
        float zStart;

        var gridHousing = Instantiate(new GameObject(), new Vector3(0,0,0), Quaternion.identity);

        //GridSpace gs;

        foreach (var i in _dungeonRooms)
        {
            xStart = i.transform.position.x - ((i.sizeX / 2) + 0.5f);
            zStart = i.transform.position.z - ((i.sizeZ / 2) + 0.5f);
            var zRestart = zStart;
            for (var x = 0; x < i.sizeX; x++)
            {
                xStart = xStart + 1;
                zStart = zRestart;
                for (var z = 0; z < i.sizeZ; z++)
                {
                    zStart = zStart + 1;

                    //gs = new GridSpace(Mathf.RoundToInt(xStart), Mathf.RoundToInt(zStart), GridType.floor);

                    var newFloor = Instantiate(testFloor, new Vector3(xStart, .75f, zStart), Quaternion.identity);
                    newFloor.transform.parent = gridHousing.transform;

                    yield return new WaitForSeconds(0.00f);
                }
            }           
        }
    }

    //used
    private void LayTestGrid()
    {
        var rot = Quaternion.Euler(0, 90, 0);
        Vector3 wallPos = new Vector3(0, 0, 0);

        var gridHousing = Instantiate(new GameObject(), new Vector3(0, 0, 0), Quaternion.identity);
        foreach (var g in theGrid)
        {

            bool complete = true;

            if (g.hasObject && (!g.doorNorth && !g.doorEast && !g.doorSouth && !g.doorWest))
            {
                //check if door below or to left
                if ((theGrid.Any(x => x.x == g.x && x.z == g.z - 1 && x.wallNorth)) || (theGrid.Any(x => x.x == g.x - 1 && x.z == g.z && x.wallEast)))
                {
                    Debug.Log("DOOR IN WAY!!!!");
                }
                else
                {
                    //LAY OBJECT HERE!!!
                    var newBlock = Instantiate(testBlock, new Vector3(g.x, 1.25f, g.z), Quaternion.identity);
                    newBlock.transform.parent = gridHousing.transform;
                    //complete = false;
                }
            }
            else
            {

            }
            if (complete)
            { 
                if (g.gsSpaceType == GridType.floor)
                {
                    var newFloor = Instantiate(testFloor, new Vector3(g.x, .75f, g.z), Quaternion.identity);
                    newFloor.transform.parent = gridHousing.transform;
                }

                //bool createWall = false;
                //TEST
                if (g.wallNorth || g.doorNorth)
                {
                    wallPos = new Vector3(g.x, 1.25f, g.z + 0.5f);
                    if (g.doorNorth)
                    {
                        var newWall = Instantiate(testDoor, wallPos, Quaternion.identity);
                        newWall.transform.parent = gridHousing.transform;
                    }
                    else
                    {
                        var newWall = Instantiate(testWall, wallPos, Quaternion.identity);
                        newWall.transform.parent = gridHousing.transform;
                    }
                }

                if (g.wallEast || g.doorEast)
                {

                    wallPos = new Vector3(g.x + 0.5f, 1.25f, g.z);
                    if (g.doorEast)
                    {
                        var newWall = Instantiate(testDoor, wallPos, rot);
                        newWall.transform.parent = gridHousing.transform;
                    }
                    else
                    {
                        var newWall = Instantiate(testWall, wallPos, rot);
                        newWall.transform.parent = gridHousing.transform;
                    }

                }
                if (g.doorSouth || g.wallSouth)
                {
                    //createWall = true;
                    if (!(theGrid.Any(x => x.x == g.x && x.z == g.z - 1 && x.wallNorth)))
                    {
                        wallPos = new Vector3(g.x, 1.25f, g.z - 0.5f);
                        //Debug.Log(g.doorSouth);
                        if (g.doorSouth)
                        {
                            var newWall = Instantiate(testDoor, wallPos, Quaternion.identity);
                            newWall.transform.parent = gridHousing.transform;
                        }
                        else
                        {
                            var newWall = Instantiate(testWall, wallPos, Quaternion.identity);
                            newWall.transform.parent = gridHousing.transform;
                        }
                    }
                }
                if (g.doorWest || g.wallWest)
                {
                    //createWall = true;
                    if (!(theGrid.Any(x => x.x == g.x - 1 && x.z == g.z && x.wallEast)))
                    {
                        wallPos = new Vector3(g.x - 0.5f, 1.25f, g.z);
                        if (g.doorWest)
                        {
                            var newWall = Instantiate(testDoor, wallPos, rot);
                            newWall.transform.parent = gridHousing.transform;
                        }
                        else
                        {
                            var newWall = Instantiate(testWall, wallPos, rot);
                            newWall.transform.parent = gridHousing.transform;
                        }
                    }
                }
            }
        }
    }


    private void LayGrid()
    {
        var rot = Quaternion.Euler(0, 90, 0);
        Vector3 wallPos = new Vector3(0, 0, 0);
        Vector3 doorPos = new Vector3(0, 0, 0);

        var gridHousing = Instantiate(new GameObject(), new Vector3(0, 0, 0), Quaternion.identity);
        gridHousing.tag = "AllShipMesh";
        foreach (var g in theGrid)
        {
            var X = g.x * 5;
            var Z = g.z * 5;

            if (g.hasObject && (!g.doorNorth && !g.doorEast && !g.doorSouth && !g.doorWest))
            {
                //check if door below or to left
                if (!(theGrid.Any(x => x.x == g.x && x.z == g.z - 1 && x.wallNorth)) 
                    && !(theGrid.Any(x => x.x == g.x - 1 && x.z == g.z && x.wallEast))
                    )
                {
                    //LAY OBJECT HERE!!!
                    var newBlock = Instantiate(Block, new Vector3(X, 2.5f, Z), Quaternion.identity);
                    newBlock.transform.parent = gridHousing.transform;
                    //complete = false;
                }
            }
            if (g.gsSpaceType == GridType.floor)
            {
                var newFloor = Instantiate(Floor, new Vector3(X, 0, Z), Quaternion.identity);
                newFloor.transform.parent = gridHousing.transform;
            }

            //bool createWall = false;
            //TEST
            if (g.wallNorth || g.doorNorth)
            {
                wallPos = new Vector3(X, 2.5f, Z + 2.5f);
                doorPos = new Vector3(X, 0, Z + 2.5f);
                if (g.doorNorth)
                {
                    var newWall = Instantiate(Door, doorPos, Quaternion.identity);
                    newWall.transform.parent = gridHousing.transform;
                }
                else
                {
                    var newWall = Instantiate(Wall, wallPos, Quaternion.identity);
                    newWall.transform.parent = gridHousing.transform;
                }
            }

            if (g.wallEast || g.doorEast)
            {

                wallPos = new Vector3(X + 2.5f, 2.5f, Z);
                doorPos = new Vector3(X + 2.5f, 0, Z);
                if (g.doorEast)
                {
                    var newWall = Instantiate(Door, doorPos, rot);
                    newWall.transform.parent = gridHousing.transform;
                }
                else
                {
                    var newWall = Instantiate(Wall, wallPos, rot);
                    newWall.transform.parent = gridHousing.transform;
                }

            }
            if (g.doorSouth || g.wallSouth)
            {
                //createWall = true;
                if (!(theGrid.Any(x => x.x == g.x && x.z == g.z - 1 && x.wallNorth)))
                {
                    wallPos = new Vector3(X, 2.5f, Z - 2.5f);
                    doorPos = new Vector3(X, 0, Z - 2.5f);
                    //Debug.Log(g.doorSouth);
                    if (g.doorSouth)
                    {
                        var newWall = Instantiate(Door, doorPos, Quaternion.identity);
                        newWall.transform.parent = gridHousing.transform;
                    }
                    else
                    {
                        var newWall = Instantiate(Wall, wallPos, Quaternion.identity);
                        newWall.transform.parent = gridHousing.transform;
                    }
                }
            }
            if (g.doorWest || g.wallWest)
            {
                //createWall = true;
                if (!(theGrid.Any(x => x.x == g.x - 1 && x.z == g.z && x.wallEast)))
                {
                    wallPos = new Vector3(X - 2.5f, 2.5f, Z);
                    doorPos = new Vector3(X - 2.5f, 0, Z);
                    if (g.doorWest)
                    {
                        var newWall = Instantiate(Door, doorPos, rot);
                        newWall.transform.parent = gridHousing.transform;
                    }
                    else
                    {
                        var newWall = Instantiate(Wall, wallPos, rot);
                        newWall.transform.parent = gridHousing.transform;
                    }
                }
            }

        }
    }
}
