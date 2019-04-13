struct GridSpace
{

    public int ID;

    public int roomID;

    public float x, z;

    //GridType is its own Enum
    public GridType gsSpaceType;

    public bool hasObject;

    public bool hasSmallEnemy;
    public bool hasLargeEnemy;

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

        hasSmallEnemy = false;
        hasLargeEnemy = false;

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

    public void setHasSmallEnemy(bool delta)
    {
        hasSmallEnemy = delta;
    }

    public void setHasLargeEnemy(bool delta)
    {
        hasLargeEnemy = delta;
    }
}