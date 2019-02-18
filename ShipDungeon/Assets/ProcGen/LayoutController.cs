using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class LayoutController : MonoBehaviour {

    //private GridType gridType = new GridType();

    public GameObject testFloor;
    public GameObject testWall;
    public GameObject testDoor;

    public bool iterate = false;

    struct GridSpace {

        public int ID;

        public float x, z;

        public GridType gsSpaceType;

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
            xStart = i.transform.position.x - ((i.sizeX / 2) + 0.5f);
            zStart = i.transform.position.z - ((i.sizeZ / 2) + 0.5f);
            var zRestart = zStart;

            wallNorth = false;
            wallSouth = false;
            wallEast = false;
            wallWest = false;

            for (var x = 0; x < i.sizeX; x++)
            {
                xStart = xStart + 1;
                zStart = zRestart;

                //this isnt a great method....
                if(x == 0)
                {
                    wallNorth = false;
                    wallEast = true;
                    wallSouth = false;                 
                    wallWest = false;
                }
                else if(x == i.sizeX - 1)
                {
                    wallNorth = false;
                    wallEast = false;
                    wallSouth = false;
                    wallWest = true;
                }
                else
                {
                    wallNorth = false;
                    wallEast = false;
                    wallSouth = false;
                    wallWest = false;
                }

                for (var z = 0; z < i.sizeZ; z++)
                {
                    if (z == 0)
                    {
                        wallNorth = false;
                        //wallEast = false;
                        wallSouth = true;
                        //wallWest = false;
                    }
                    else if (z == i.sizeZ - 1)
                    {
                        wallNorth = true;
                        //wallEast = false;
                        wallSouth = false;
                        //wallWest = false;
                    }
                    else
                    {
                        wallNorth = false;
                        //wallEast = false;
                        wallSouth = false;
                       // wallWest = false;
                    }

                    id++;

                    zStart = zStart + 1;
                    gs = new GridSpace(id, xStart, zStart, GridType.floor);
                    gs.setWalls(wallNorth, wallEast, wallSouth, wallWest);
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


        LayGrid();
    } 

    //corridor builder
    public void DigCorridor(DungeonRoom Room1, DungeonRoom Room2)
    {
        //if(from x in theGrid where x.x == 1 && x.z == 2 select x).First() { }
        //Debug.Log("DIG CORRIDOR");


        var mainRoom = Room1;
        var targetRoom = Room2;






        Vector3 mainPos = mainRoom.transform.position;
        Vector3 targetPos = targetRoom.transform.position;

        bool targetNorth = targetPos.z >= mainPos.z;
        bool targetEast = targetPos.x >= mainPos.x;
        bool isCollidingNorth = false;
        bool isCollidingEast = false;
        bool isCollidingSouth = false;
        bool isCollidingWest = false;

        bool needsCorridor = false;

        //if (targetNorth)
        //{
        //    Debug.Log("main: " + mainRoom.ID + " , pos:" + (mainPos.z + (mainRoom.sizeZ / 2)));// + " , " + mainPos.x);
        //    Debug.Log("target: " + targetRoom.ID + " , pos: " + (targetPos.z - (mainRoom.sizeZ / 2)));// + " , " + targetPos.x);
        //}


        //north colision
        if (targetNorth && ((mainPos.z + ((mainRoom.sizeZ / 2)) == (targetPos.z - (targetRoom.sizeZ / 2)))))
        {
            isCollidingNorth = true;
        }
        else if (targetNorth && (mainPos.z - ((mainRoom.sizeZ / 2)) == (targetPos.z + (targetRoom.sizeZ / 2))))
        {
            isCollidingSouth = true;
        }
        
        //east collision
        if (targetEast && ((mainPos.x + ((mainRoom.sizeX / 2)) == (targetPos.x - (targetRoom.sizeX / 2)))))
        {
            isCollidingEast = true;
        }
        else if (targetEast && (mainPos.x - ((mainRoom.sizeX / 2)) == (targetPos.x + (targetRoom.sizeX / 2))))
        {
            isCollidingWest = true;
        }

        //if (!isCollidingNorth && !isCollidingEast && !isCollidingSouth && !isCollidingWest)
        //{
        //    needsCorridor = true;
        //}
        //else
        //{

        int start;
        int finish;

        //this else if will make it so it prioritises in this order
        if (isCollidingNorth)
        {

            //find a mutual point and make that gridspace have a doorNorth true
            if (targetEast)
            {
                start = Mathf.RoundToInt(targetPos.x - (targetRoom.sizeX / 2));
                finish = Mathf.RoundToInt(mainPos.x + (mainRoom.sizeX / 2));
                //Debug.Log("AADSDA " + (finish - start));
                //test
                if (finish > start)
                {
                    var doorPos = (Random.Range(start, finish) + 0.5f);
                    var zPos = mainPos.z + (((mainRoom.sizeZ / 2) - 1) + 0.5f);
                    //Debug.Log("Door: " + doorPos + " , zPos: " + zPos);
                    var doorSpace = (from x in theGrid where x.x == doorPos && x.z == zPos select x).First();
                    doorSpace.doorNorth = true;

                    theGrid.Remove((from x in theGrid where x.x == doorPos && x.z == zPos select x).First());
                    theGrid.Add(doorSpace);

                    //Debug.Log("DOOR NORTH");
                }
            }
            else
            {
                start = Mathf.RoundToInt(mainPos.x - (mainRoom.sizeX / 2));
                finish = Mathf.RoundToInt(targetPos.x + (targetRoom.sizeX / 2));
                //Debug.Log("AADSDA " + (finish - start));
                //test
                if (finish > start)
                {
                    var doorPos = (Random.Range(start, finish) + 0.5f);
                    var zPos = mainPos.z + (((mainRoom.sizeZ / 2) - 1) + 0.5f);
                    //var gridID = (from x in theGrid where x.x == doorPos && x.z == zPos select x.ID).First();

                    var doorSpace = (from x in theGrid where x.x == doorPos && x.z == zPos select x).First();
                    doorSpace.doorNorth = true;

                    theGrid.Remove((from x in theGrid where x.x == doorPos && x.z == zPos select x).First());
                    theGrid.Add(doorSpace);
                    //doorSpace.doorNorth = true;               
                    //Debug.Log("DOOR NORTH");
                }
            }
        }

        if (isCollidingEast)
        {
            Debug.Log("CollEast");

            //find a mutual point and make that gridspace have a doorNorth true
            if (targetNorth)
            {
                Debug.Log("TargetNorth");
                start = Mathf.RoundToInt(mainPos.z + (mainRoom.sizeZ / 2));
                finish = Mathf.RoundToInt(targetPos.z - (targetRoom.sizeZ / 2));
                if (start > finish)
                {
                    var doorPos = (Random.Range(start, finish) + 0.5f);
                    var xPos = mainPos.x + (((mainRoom.sizeX / 2) + 0) + 0.5f);
                    var doorSpace = (from x in theGrid where x.x == xPos && x.z == doorPos select x).First();
                    doorSpace.doorEast = true;
                    theGrid.Remove((from x in theGrid where x.x == xPos && x.z == doorPos select x).First());
                    theGrid.Add(doorSpace);
                }
            }
            else
            {
                Debug.Log("TargetSouth");              
                start = Mathf.RoundToInt(targetPos.z + (targetRoom.sizeZ / 2));
                finish = Mathf.RoundToInt(mainPos.z - (mainRoom.sizeZ / 2));
                if (start > finish)
                {
                    var doorPos = (Random.Range(start, finish) + 0.5f);
                    var xPos = mainPos.x + (((mainRoom.sizeX / 2) + 0) + 0.5f);
                    var doorSpace = (from x in theGrid where x.x == xPos && x.z == doorPos select x).First();
                    doorSpace.doorEast = true;
                    theGrid.Remove((from x in theGrid where x.x == xPos && x.z == doorPos select x).First());
                    theGrid.Add(doorSpace);
                }
            }
        }
        //else if (isCollidingSouth)
        //{

        //}
        //else if (isCollidingWest)
        //{

        //}
        else
        {
            needsCorridor = true;
        }
        //}


    }




    //THESE BELOW WILL BE REMOVED AND DONE ELSEWHERE


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

    private void LayGrid()
    {

        //TODO - add a door check in here that overrides wall, and more checks that look for problems



        var rot = Quaternion.Euler(0, 90, 0);

        Vector3 wallPos = new Vector3(0, 0, 0);

        var gridHousing = Instantiate(new GameObject(), new Vector3(0, 0, 0), Quaternion.identity);
        foreach (var g in theGrid)
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

                wallPos = new Vector3(g.x - 0.5f, 1.25f, g.z);
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
            if (g.wallSouth)
            {
                //createWall = true;
                if (!(theGrid.Any(x => x.x == g.x && x.z == g.z - 1)))
                {
                    wallPos = new Vector3(g.x, 1.25f, g.z - 0.5f);
                    var newWall = Instantiate(testWall, wallPos, Quaternion.identity);
                    newWall.transform.parent = gridHousing.transform;
                }
            }
            if (g.wallWest)
            {
                //createWall = true;
                if (!(theGrid.Any(x => x.x == g.x + 1 && x.z == g.z)))
                {
                    wallPos = new Vector3(g.x + 0.5f, 1.25f, g.z);
                    var newWall = Instantiate(testWall, wallPos, rot);
                    newWall.transform.parent = gridHousing.transform;
                }
            }

        }
    }

}
