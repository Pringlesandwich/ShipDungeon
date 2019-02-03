using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LayoutController : MonoBehaviour {

    //private GridType gridType = new GridType();

    public GameObject testFloor;
    public GameObject testWall;

    public bool iterate = false;

    struct GridSpace {

        public float x, z;

        public GridType gsSpaceType;

        public bool isNorth;
        public bool isEast;
        public bool isSouth;
        public bool isWest;

        public GridSpace(float _x, float _z, GridType gt) : this()
        {
            x = _x;
            z = _z;
            gsSpaceType = gt;

            isNorth = false;
            isEast = false;
            isSouth = false;
            isWest = false;

        }

        public void setCompass(bool N, bool E, bool S, bool W)
        {
            isNorth = N;
            isEast = E;
            isSouth = S;
            isWest = W;
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
            SetFloorTiles(_rooms);
        }
    }

    public void SetFloorTiles(List<DungeonRoom> _dungeonRooms)
    {
        bool isNorth;
        bool isSouth;
        bool isEast;
        bool isWest;

        float xStart;
        float zStart;

        GridSpace gs;

        foreach (var i in _dungeonRooms)
        {
            xStart = i.transform.position.x - ((i.sizeX / 2) + 0.5f);
            zStart = i.transform.position.z - ((i.sizeZ / 2) + 0.5f);
            var zRestart = zStart;

            isNorth = false;
            isSouth = false;
            isEast = false;
            isWest = false;

            for (var x = 0; x < i.sizeX; x++)
            {
                xStart = xStart + 1;
                zStart = zRestart;

                //this isnt a great method....
                if(x == 0)
                {
                    isNorth = false;
                    isEast = true;
                    isSouth = false;                 
                    isWest = false;
                }
                else if(x == i.sizeX - 1)
                {
                    isNorth = false;
                    isEast = false;
                    isSouth = false;
                    isWest = true;
                }
                else
                {
                    isNorth = false;
                    isEast = false;
                    isSouth = false;
                    isWest = false;
                }

                for (var z = 0; z < i.sizeZ; z++)
                {
                    if (z == 0)
                    {
                        isNorth = false;
                        //isEast = false;
                        isSouth = true;
                        //isWest = false;
                    }
                    else if (z == i.sizeZ - 1)
                    {
                        isNorth = true;
                        //isEast = false;
                        isSouth = false;
                        //isWest = false;
                    }
                    else
                    {
                        isNorth = false;
                        //isEast = false;
                        isSouth = false;
                       // isWest = false;
                    }

                    zStart = zStart + 1;
                    gs = new GridSpace(xStart, zStart, GridType.floor);
                    gs.setCompass(isNorth, isEast, isSouth, isWest);
                    theGrid.Add(gs);
                }
            }
        }
        LayFloors();
    }

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

    private void LayFloors()
    {
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
            
            if (g.isNorth)
            {
                //createWall = true;             
                if (!(theGrid.Any(x => x.x == g.x && x.z == g.z + 1)))
                {
                    wallPos = new Vector3(g.x, 1.25f, g.z + 1);
                    var newWall = Instantiate(testWall, wallPos, Quaternion.identity);
                    newWall.transform.parent = gridHousing.transform;
                }
            }
            if(g.isEast)
            {
                //createWall = true;
                if(!(theGrid.Any(x => x.x == g.x - 1 && x.z == g.z)))
                {
                    wallPos = new Vector3(g.x - 1, 1.25f, g.z);
                    var newWall = Instantiate(testWall, wallPos, Quaternion.identity);
                    newWall.transform.parent = gridHousing.transform;
                }
            }
            if (g.isSouth)
            {
                //createWall = true;
                if (!(theGrid.Any(x => x.x == g.x && x.z == g.z - 1)))
                {
                    wallPos = new Vector3(g.x, 1.25f, g.z - 1);
                    var newWall = Instantiate(testWall, wallPos, Quaternion.identity);
                    newWall.transform.parent = gridHousing.transform;
                }
            }
            if (g.isWest)
            {
                //createWall = true;
                if (!(theGrid.Any(x => x.x == g.x + 1 && x.z == g.z)))
                {
                    wallPos = new Vector3(g.x + 1, 1.25f, g.z);
                    var newWall = Instantiate(testWall, wallPos, Quaternion.identity);
                    newWall.transform.parent = gridHousing.transform;
                }
            }

        }
    }

}
