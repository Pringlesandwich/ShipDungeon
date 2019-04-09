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
    public GameObject FloorMain;
    public GameObject Wall;
    public GameObject Door;
    public GameObject Block;

    public GameObject SpawnRoom;

    public bool iterate = false;


    //TODO - move this to its own class/struct
    struct GridSpace {

        public int ID;

        public int roomID;

        public float x, z;

        //GridType is its own Enum
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

        public bool entranceNorth;
        public bool entranceEast;
        public bool entranceSouth;
        public bool entranceWest;

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

            entranceNorth = false;
            entranceEast = false;
            entranceSouth = false;
            entranceWest = false;

        }

        public void setWalls(bool N, bool E, bool S, bool W)
        {
            wallNorth = N;
            wallEast = E;
            wallSouth = S;
            wallWest = W;
        }

        public void setGridType(GridType gt)
        {
            gsSpaceType = gt;
        }

        public void setHasObject(bool delta)
        {
            hasObject = delta;
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
                    

                    //PROTOTYPE Random generation of object in map! BAD!
                    //int test = Random.Range(0, 12);
                    //if (test < 2)
                    //{
                    //    gs.hasObject = true;
                    //}


                    theGrid.Add(gs);
                }
            }
        }

        // set corridors
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

        //room obstacles
        foreach (var i in _dungeonRooms)
        {
            EnsureMainCorridors(i);
        }

        // inner corridor for object placement
        foreach (var i in _dungeonRooms)
        {
            AddInnerCorridors(i);
        }

        //place room objects
        foreach (var i in _dungeonRooms)
        {
            AddRoomObjects(i);
        }

        




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
                            gs = new GridSpace(666, doorPos, zPos, GridType.corridor);
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

                //TODO - reset
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
                            gs = new GridSpace(666, xPos, doorPos, GridType.corridor);
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


    public void EnsureMainCorridors(DungeonRoom room)
    {
        //foreach space on grid in room, get list of ones near that are doors
        var xPos = room.transform.position.x - ((room.sizeX / 2) + 0.5f);
        var zPos = room.transform.position.z - ((room.sizeZ / 2) + 0.5f);
        var zRestart = zPos;

        //go through each gridspace and see if it is a door, make sure the other side of that door is a mainspace
        for (var x = 0; x < room.sizeX; x++)
        {
            zPos = zRestart;
            xPos = xPos + 1;
            for (var z = 0; z < room.sizeZ; z++)
            {
                zPos = zPos + 1;
                //if a part in the grid has a door, add it to the new list
                if ((theGrid.Any((grid => grid.x == xPos && grid.z == zPos 
                && (grid.doorEast || grid.doorNorth || grid.doorSouth || grid.doorWest)))))        
                {
                   
                    var newGrid = (from g in theGrid where g.x == xPos && g.z == zPos select g).First();
                    newGrid.setGridType(GridType.floorMain);
                    theGrid.Remove((from g in theGrid where g.x == xPos && g.z == zPos select g).First());
                    theGrid.Add(newGrid);

                    //doorSpaces.Add(newGrid);

                    var newXPos = xPos + 1;
                    var newZPos = zPos + 1;

                    //see if there is a grid space above or right and replace floor
                    if ((theGrid.Any((grid => grid.x == xPos && grid.z == newZPos && grid.gsSpaceType != GridType.corridor)))
                        && newGrid.doorNorth )
                    {
                        var otherGrid = (from g in theGrid where g.x == xPos && g.z == newZPos select g).First();
                        otherGrid.setGridType(GridType.floorMain);
                        theGrid.Remove((from g in theGrid where g.x == xPos && g.z == newZPos select g).First());
                        theGrid.Add(otherGrid);
                        //doorSpaces.Add(otherGrid);
                    }
                    if ((theGrid.Any((grid => grid.x == newXPos && grid.z == zPos && grid.gsSpaceType != GridType.corridor))) 
                        && newGrid.doorEast)
                    {
                        var otherGrid = (from g in theGrid where g.x == newXPos && g.z == zPos select g).First();
                        otherGrid.setGridType(GridType.floorMain);
                        theGrid.Remove((from g in theGrid where g.x == newXPos && g.z == zPos select g).First());
                        theGrid.Add(otherGrid);
                        //doorSpaces.Add(otherGrid);
                    }
                }
            }
        } 
        
        //if this room is the start room then ensure a path to the spawn point and set it on grid
        //then add a grid next to it with a special gridtype
        if(room.isStartRoom)
        {
            //choose a side to do this to
            var x = room.transform.position.x;
            var z = room.transform.position.z;

            //get positive of values
            var deltaX = x > 0 ? x : x * -1;
            var deltaZ = z > 0 ? z : z * -1;

            //var xPos = room.transform.position.x - ((room.sizeX / 2) + 0.5f);
            //var zPos = room.transform.position.z - ((room.sizeZ / 2) + 0.5f);

            int pos = 0;
            float useX = 0;
            float useZ = 0;
            bool isEast = false;
            bool isWest = false;
            bool isNorth = false;
            bool isSouth = false;

            //use east or west wall
            if(deltaX > deltaZ)
            {
                if(x > 0) //east
                {
                    pos = Random.Range(1, room.sizeZ);
                    useX = room.transform.position.x + ((room.sizeX / 2) + 0.5f);
                    useZ = (room.transform.position.z - ((room.sizeZ / 2) + 0.5f)) + pos;
                    isEast = true;
                }
                else //west
                {
                    pos = Random.Range(1, room.sizeZ);
                    useX = room.transform.position.x - ((room.sizeX / 2) + 0.5f);
                    useZ = (room.transform.position.z - ((room.sizeZ / 2) + 0.5f)) + pos;
                    isWest = true;
                }
            }
            //use north of south wall
            else
            {
                if(z > 0) //north
                {
                    pos = Random.Range(1, room.sizeX);
                    useX = (room.transform.position.x - ((room.sizeX / 2) + 0.5f)) + pos;
                    useZ = room.transform.position.z + ((room.sizeZ / 2) + 0.5f);
                    isNorth = true;
                }
                else //south
                {
                    pos = Random.Range(1, room.sizeX);
                    useX = (room.transform.position.x - ((room.sizeX / 2) + 0.5f)) + pos;
                    useZ = room.transform.position.z - ((room.sizeZ / 2) + 0.5f);
                    isSouth = true;
                }
            }

            GridSpace gs = new GridSpace(666, useX, useZ, GridType.spawnRoom);
            if (isEast)
            {
                gs.entranceWest = true;
                useX = useX - 1;
                Debug.Log("East");
            }
            else if (isWest)
            {
                gs.entranceEast = true;
                useX = useX + 1;
                Debug.Log("West");
            }
            else if (isNorth)
            {
                gs.entranceSouth = true;
                useZ = useZ - 1;
                Debug.Log("North");
            }
            else if (isSouth)
            {
                gs.entranceNorth = true;
                useZ = useZ + 1;
                Debug.Log("South");
            }

            theGrid.Add(gs);

            //set the floor next to it as main so a path will be made to it
            var otherGrid = (from g in theGrid where g.x == useX && g.z == useZ select g).First();
            otherGrid.setGridType(GridType.floorMain);
            if (isEast) { otherGrid.wallEast = false; }
            else if (isWest) { otherGrid.wallWest = false; }
            else if (isNorth) { otherGrid.wallNorth = false; }
            else if (isSouth) { otherGrid.wallSouth = false; }
            theGrid.Remove((from g in theGrid where g.x == useX && g.z == useZ select g).First());
            theGrid.Add(otherGrid);

        }
    }


    //link each door to each and change its GridType to floormain so that obstacles cannot be added to it
    public void AddInnerCorridors(DungeonRoom room)
    {
        //link each door
        try
        {
            List<GridSpace> doorSpaces = (from g in theGrid
                                          where g.roomID == room.ID
                                          && g.gsSpaceType == GridType.floorMain
                                          select g).ToList();

            var mainSpace = doorSpaces.First();
            foreach (var space in doorSpaces)
            {

                //Debug.Log("LINK ROOM SPACES!");
                //Debug.Log("Space ID: " + space.ID + " , MainID: " + mainSpace.ID);

                int moves = 0;

                if (space.ID != mainSpace.ID) //dont go from first to first
                {
                    //go from mainspace to space
                    GridSpace walker = mainSpace;
                    bool running = true;
                    bool xDone = false;
                    bool zDone = false;

                    bool moveX = false;

                    float tempPos;

                    if (walker.doorEast || walker.doorWest)
                    {
                        //need to move x first
                        moveX = true;
                    }

                    while (running)
                    {
                        // X 
                        if (!xDone && moveX)
                        {
                            if (walker.x < space.x)
                            {
                                moves++;
                                tempPos = space.x;

                                //then walk right
                                walker = new GridSpace(walker.ID, walker.x + 1, walker.z, walker.gsSpaceType);

                                var deltaGrid = (from g in theGrid where g.x == walker.x && g.z == walker.z select g).First();
                                deltaGrid.setGridType(GridType.floorMain);
                                theGrid.Remove((from g in theGrid where g.x == deltaGrid.x && g.z == deltaGrid.z select g).First());
                                theGrid.Add(deltaGrid);

                                if (walker.x == tempPos)
                                {
                                    moveX = false;
                                    xDone = true;
                                }
                            }
                            else if (walker.x > space.x)
                            {
                                moves++;
                                tempPos = space.x;

                                //then walk left
                                walker = new GridSpace(walker.ID, walker.x - 1, walker.z, walker.gsSpaceType);

                                var deltaGrid = (from g in theGrid where g.x == walker.x && g.z == walker.z select g).First();
                                deltaGrid.setGridType(GridType.floorMain);
                                theGrid.Remove((from g in theGrid where g.x == deltaGrid.x && g.z == deltaGrid.z select g).First());
                                theGrid.Add(deltaGrid);

                                if (walker.x == tempPos)
                                {
                                    moveX = false;
                                    xDone = true;
                                }
                            }
                            else
                            {
                                moveX = false;
                                xDone = true;
                            }
                        }

                        // Z 
                        if (!zDone && !moveX)
                        {
                            if (walker.z < space.z)
                            {
                                moves++;
                                tempPos = space.z;
                                //then walk up
                                walker = new GridSpace(walker.ID, walker.x, walker.z + 1, walker.gsSpaceType);

                                var deltaGrid = (from g in theGrid where g.x == walker.x && g.z == walker.z select g).First();
                                deltaGrid.setGridType(GridType.floorMain);
                                theGrid.Remove((from g in theGrid where g.x == deltaGrid.x && g.z == deltaGrid.z select g).First());
                                theGrid.Add(deltaGrid);

                                if (walker.z == tempPos)
                                {
                                    moveX = true;
                                    zDone = true;
                                }
                            }
                            else if (walker.z > space.z)
                            {
                                moves++;
                                tempPos = space.z;
                                //then walk down
                                walker = new GridSpace(walker.ID, walker.x, walker.z - 1, walker.gsSpaceType);

                                var deltaGrid = (from g in theGrid where g.x == walker.x && g.z == walker.z select g).First();
                                deltaGrid.setGridType(GridType.floorMain);
                                theGrid.Remove((from g in theGrid where g.x == deltaGrid.x && g.z == deltaGrid.z select g).First());
                                theGrid.Add(deltaGrid);

                                if (walker.z == tempPos)
                                {
                                    moveX = true;
                                    zDone = true;
                                }
                            }
                            else
                            {
                                moveX = true; // 
                                zDone = true;
                            }
                        }

                        if (xDone && zDone)
                        {
                            running = false;
                            //break;
                        }

                    }
                }
            }
        }
        catch { }
    }


    //try and place random set block types in a room x number of times
    public void AddRoomObjects(DungeonRoom room)
    {
        int targetPercent = Mathf.RoundToInt((room.sizeX * room.sizeZ) * 0.3f); // percent

        int blockCount = 0;

        float deltaX = 0;
        float deltaZ = 0;

        float roomXMin;
        float roomXMax;
        float roomZMin;
        float roomZMax;

        roomXMin = room.transform.position.x - ((room.sizeX / 2) - 0.5f);
        roomZMin = room.transform.position.z - ((room.sizeZ / 2) - 0.5f);
        roomXMax = roomXMin + room.sizeX;
        roomZMax = roomZMin + room.sizeZ;

        //do x times, or until room has a percentage of floors changed
        for (var i = 0; i < 15; i++)
        {
            //get random space within room 
            int randomX = Random.Range(0, room.sizeX);
            int randomZ = Random.Range(0, room.sizeZ);
            deltaX = roomXMin + randomX;
            deltaZ = roomZMin + randomZ;

            //get a random shape (0-3)
            switch (Random.Range(0, 3))
            {
                case 0: // 1x1
                    if(CanPlaceObject(deltaX, deltaZ))
                    {
                        SetHasObject(deltaX, deltaZ);
                        blockCount++;
                    }
                    break;
                case 1: // 1x2
                    if (CanPlaceObject(deltaX, deltaZ) && CanPlaceObject(deltaX, deltaZ + 1))
                    {
                        SetHasObject(deltaX, deltaZ);
                        SetHasObject(deltaX, deltaZ + 1);
                        blockCount++;
                        blockCount++;
                    }
                    break;
                case 2: // 2x1
                    if (CanPlaceObject(deltaX, deltaZ) && CanPlaceObject(deltaX + 1, deltaZ))
                    {
                        SetHasObject(deltaX, deltaZ);
                        SetHasObject(deltaX + 1, deltaZ);
                        blockCount++;
                        blockCount++;
                    }
                    break;
                case 3: // 2x2
                    if (CanPlaceObject(deltaX, deltaZ) && CanPlaceObject(deltaX + 1, deltaZ) 
                        && CanPlaceObject(deltaX, deltaZ + 1) && CanPlaceObject(deltaX + 1, deltaZ + 1))
                    {
                        SetHasObject(deltaX, deltaZ);
                        SetHasObject(deltaX + 1, deltaZ);
                        SetHasObject(deltaX, deltaZ + 1);
                        SetHasObject(deltaX + 1, deltaZ + 1);
                        blockCount++;
                        blockCount++;
                        blockCount++;
                        blockCount++;
                    }
                    break;
            }

            if (blockCount >= targetPercent)
            {
                break;
            }
        }
    }


    private bool CanPlaceObject(float x, float z)
    {
        try
        {
            var Grid = (from g in theGrid where g.x == x && g.z == z select g).First();
            return Grid.gsSpaceType == GridType.floorMain ? false : true;
        }
        catch
        {
            return false; // return false if not exist so wont try an place
        }
    }


    private void SetHasObject(float x, float z)
    {
        var deltaGrid = (from g in theGrid where g.x == x && g.z == z select g).First();
        deltaGrid.setHasObject(true);
        theGrid.Remove((from g in theGrid where g.x == x && g.z == z select g).First());
        theGrid.Add(deltaGrid);
    }



    //THESE BELOW WILL BE REMOVED AND DONE ELSEWHERE?????????

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

    // not used
    //private void LayTestGrid()
    //{
    //    var rot = Quaternion.Euler(0, 90, 0);
    //    Vector3 wallPos = new Vector3(0, 0, 0);

    //    var gridHousing = Instantiate(new GameObject(), new Vector3(0, 0, 0), Quaternion.identity);
    //    foreach (var g in theGrid)
    //    {

    //        bool complete = true;

    //        if (g.hasObject && (!g.doorNorth && !g.doorEast && !g.doorSouth && !g.doorWest))
    //        {
    //            //check if door below or to left
    //            if ((theGrid.Any(x => x.x == g.x && x.z == g.z - 1 && x.wallNorth)) || (theGrid.Any(x => x.x == g.x - 1 && x.z == g.z && x.wallEast)))
    //            {
    //                Debug.Log("DOOR IN WAY!!!!");
    //            }
    //            else
    //            {
    //                //LAY OBJECT HERE!!!
    //                var newBlock = Instantiate(testBlock, new Vector3(g.x, 1.25f, g.z), Quaternion.identity);
    //                newBlock.transform.parent = gridHousing.transform;
    //                //complete = false;
    //            }
    //        }
    //        else
    //        {

    //        }
    //        if (complete)
    //        { 
    //            if (g.gsSpaceType == GridType.floor)
    //            {
    //                var newFloor = Instantiate(testFloor, new Vector3(g.x, .75f, g.z), Quaternion.identity);
    //                newFloor.transform.parent = gridHousing.transform;
    //            }

    //            //bool createWall = false;
    //            //TEST
    //            if (g.wallNorth || g.doorNorth)
    //            {
    //                wallPos = new Vector3(g.x, 1.25f, g.z + 0.5f);
    //                if (g.doorNorth)
    //                {
    //                    var newWall = Instantiate(testDoor, wallPos, Quaternion.identity);
    //                    newWall.transform.parent = gridHousing.transform;
    //                }
    //                else
    //                {
    //                    var newWall = Instantiate(testWall, wallPos, Quaternion.identity);
    //                    newWall.transform.parent = gridHousing.transform;
    //                }
    //            }

    //            if (g.wallEast || g.doorEast)
    //            {

    //                wallPos = new Vector3(g.x + 0.5f, 1.25f, g.z);
    //                if (g.doorEast)
    //                {
    //                    var newWall = Instantiate(testDoor, wallPos, rot);
    //                    newWall.transform.parent = gridHousing.transform;
    //                }
    //                else
    //                {
    //                    var newWall = Instantiate(testWall, wallPos, rot);
    //                    newWall.transform.parent = gridHousing.transform;
    //                }

    //            }
    //            if (g.doorSouth || g.wallSouth)
    //            {
    //                //createWall = true;
    //                if (!(theGrid.Any(x => x.x == g.x && x.z == g.z - 1 && x.wallNorth)))
    //                {
    //                    wallPos = new Vector3(g.x, 1.25f, g.z - 0.5f);
    //                    //Debug.Log(g.doorSouth);
    //                    if (g.doorSouth)
    //                    {
    //                        var newWall = Instantiate(testDoor, wallPos, Quaternion.identity);
    //                        newWall.transform.parent = gridHousing.transform;
    //                    }
    //                    else
    //                    {
    //                        var newWall = Instantiate(testWall, wallPos, Quaternion.identity);
    //                        newWall.transform.parent = gridHousing.transform;
    //                    }
    //                }
    //            }
    //            if (g.doorWest || g.wallWest)
    //            {
    //                //createWall = true;
    //                if (!(theGrid.Any(x => x.x == g.x - 1 && x.z == g.z && x.wallEast)))
    //                {
    //                    wallPos = new Vector3(g.x - 0.5f, 1.25f, g.z);
    //                    if (g.doorWest)
    //                    {
    //                        var newWall = Instantiate(testDoor, wallPos, rot);
    //                        newWall.transform.parent = gridHousing.transform;
    //                    }
    //                    else
    //                    {
    //                        var newWall = Instantiate(testWall, wallPos, rot);
    //                        newWall.transform.parent = gridHousing.transform;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    //used

    private void LayGrid()
    {
        var rot = Quaternion.Euler(0, 90, 0);
        Vector3 wallPos = new Vector3(0, 0, 0);
        Vector3 doorPos = new Vector3(0, 0, 0);

        Quaternion ceilingQuat = Quaternion.Euler(180f, 0, 0); //for ceiling

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
            //floor
            if (g.gsSpaceType == GridType.floor || g.gsSpaceType == GridType.floorMain
                || g.gsSpaceType == GridType.floorDoor || g.gsSpaceType == GridType.corridor)
            {
                var newFloor = Instantiate(Floor, new Vector3(X, 0, Z), Quaternion.identity);
                newFloor.transform.parent = gridHousing.transform;

                var newCeiling = Instantiate(Floor, new Vector3(X, 5, Z), ceilingQuat);
                newCeiling.transform.parent = gridHousing.transform;

                // DEBUG
                //if (g.gsSpaceType == GridType.floor || g.gsSpaceType == GridType.corridor)
                //{
                //var newFloor = Instantiate(Floor, new Vector3(X, 0, Z), Quaternion.identity);
                //newFloor.transform.parent = gridHousing.transform;
                //}
                //else
                //{
                //    var newFloor = Instantiate(FloorMain, new Vector3(X, 0, Z), Quaternion.identity);
                //    newFloor.transform.parent = gridHousing.transform;
                //}          
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
                    //newWall.transform.parent = gridHousing.transform;
                }
                else
                {
                    var newWall = Instantiate(Wall, wallPos, Quaternion.identity);
                    //newWall.transform.parent = gridHousing.transform;
                }
            }

            if (g.wallEast || g.doorEast)
            {

                wallPos = new Vector3(X + 2.5f, 2.5f, Z);
                doorPos = new Vector3(X + 2.5f, 0, Z);
                if (g.doorEast)
                {
                    var newWall = Instantiate(Door, doorPos, rot);
                    //newWall.transform.parent = gridHousing.transform;
                }
                else
                {
                    var newWall = Instantiate(Wall, wallPos, rot);
                    //newWall.transform.parent = gridHousing.transform;
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
                        //newWall.transform.parent = gridHousing.transform;
                    }
                    else
                    {
                        var newWall = Instantiate(Wall, wallPos, Quaternion.identity);
                        //newWall.transform.parent = gridHousing.transform;
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
                        //newWall.transform.parent = gridHousing.transform;
                    }
                    else
                    {
                        var newWall = Instantiate(Wall, wallPos, rot);
                        //newWall.transform.parent = gridHousing.transform;
                    }
                }
            }

            //spawn room
            if (g.gsSpaceType == GridType.spawnRoom)
            {
                var spawnRoom = new GameObject();

                //SpawnRoom
                if (g.entranceEast)
                {
                    Quaternion quat = Quaternion.Euler(0, 90f, 0);
                    spawnRoom = Instantiate(SpawnRoom, new Vector3(X, 0, Z), quat);
                }
                else if(g.entranceWest)
                {
                    Quaternion quat = Quaternion.Euler(0, 270f, 0);
                    spawnRoom = Instantiate(SpawnRoom, new Vector3(X, 0, Z), quat);
                }
                else if(g.entranceNorth)
                {
                    Quaternion quat = Quaternion.Euler(0, 0, 0);
                    spawnRoom = Instantiate(SpawnRoom, new Vector3(X, 0, Z), quat);
                }
                else if( g.entranceSouth)
                {
                    Quaternion quat = Quaternion.Euler(0, 180f, 0);
                    spawnRoom = Instantiate(SpawnRoom, new Vector3(X, 0, Z), quat);
                }
                //spawnRoom.transform.parent = gridHousing.transform;
            }

        }
    }
}
