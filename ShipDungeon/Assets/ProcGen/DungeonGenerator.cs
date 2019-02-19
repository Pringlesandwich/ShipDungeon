using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DungeonGenerator : MonoBehaviour {

    public float Radius;
    public float gridSpacing;

    public float minRoomSize;
    public float maxRoomSize;

    public int numberOfRooms;

    public int MaxAttempts;

    public GameObject floor;

    private GameObject[] GO;
    public int strength;

    public float waitTime;

    private bool moveRooms = false;

    public bool iterate;

    private Renderer renderer;
    public Material newMat;
    public Material startMat;

    public int strenght2;
    private bool DTTime = false;

    private DTController theDTController = new DTController();

    private Prims thePrimController = new Prims();
    private bool PrimFinished = false;

    private ConvertToLayout ConvertLayoutController;// = new ConvertToLayout();
    private bool LayoutFinished = false;

    //List of cells that have been turned into rooms
    private List<VertexNode> roomList = new List<VertexNode>();

    LineRenderer lineRender = new LineRenderer();

    bool isFinished = false;
    private bool DTFinished = false;

    private bool prepFinished = false;
    public LayoutController LayoutController;
    private List<DungeonRoom> dungeonRooms = new List<DungeonRoom>();

    //---------------------------------------//
    // SO FAR...
    //      - GenRooms()
    //      - SeperateRooms()
    //      - SetRooms()
    //  - the three above can be moved to a roomController????
    //      - DTTime = true;
    //      - Update()
    //          - theDTController.setupTriangulation(roomList);
    //          - theDTController.Update(); - this does the triangulation
    //          - thePrimController.setUpPrims(roomList, theDTController.getTriangulation()); - does the maths
    //          - thePrimController.Update(); - draws the edges
    //          - LayoutController = new ConvertToLayout(roomList, startRoom, thePrimController);
    //
    //---------------------------------------//




    // Use this for initialization
    void Start()
    {       
        //line.SetVertexCount(2);
        // instantiate all the rooms, make sure to do this in a 1 tile setting!!!
        // call Addtiles method with a random x and z value
        if (iterate)
        {
            StartCoroutine(GenRooms());
        }
        else
        {
            GenRoomslol();
            SeperateRoomslol();
            setRooms();
            DTTime = true;
        }     
    }

    private void setRooms()
    {
        //Debug.Log("SET ROOMS");
        foreach(var g in GO)
        {
            VertexNode thisNode = new VertexNode(g.transform.position.x, g.transform.position.z, g.gameObject);
            roomList.Add(thisNode);
        }
    }

    struct Branch
    {
        int ID;
        GameObject Main;
        List<GameObject> Connections;

        public void SetID(int id)
        {
            ID = id;
        }
    }

    List<Branch> branches = new List<Branch>();

    //not used
    private void DelaunayTriangulation()
    {
        Vector3 A = new Vector3();
        Vector3 B = new Vector3();
        int i = -1;
        foreach(var a in GO)
        {
            i++;
            Branch branch = new Branch();
            branch.SetID(i);
            foreach (var b in GO)
            {
                if (a != b)
                {
                    foreach (var c in GO)
                    {
                        if (b != c)
                        {
                            if (a != c)
                            {
                                //make a triangle with these 3

                                //get the center

                                //get distance to a

                                //foreach

                            }
                        }
                    }
                }
            }
            branches.Add(branch);
        }
        foreach(var b in branches)
        {
            //shoot a ray from it main to each connection
        }


        //for each GO

            //create a triangle with 2 other GOs



    }

    IEnumerator GenRooms()
    {
        for (int i = 0; i < numberOfRooms; i++)
        {
            Vector2 delta = GetRandomPointInCircle(Radius);
            int x = RoundToGrid(delta.x, gridSpacing);
            int y = RoundToGrid(delta.y, gridSpacing);
            delta = GetRandomRoomSize();
            var newFloor = Instantiate(floor, new Vector3(x, 0, y), Quaternion.identity);
            DungeonRoom DR = newFloor.GetComponent<DungeonRoom>();
            DR.ID = i + 1; // set id (from 0+1)
            DR.sizeX = Mathf.RoundToInt(delta.x * 2);
            DR.sizeZ = Mathf.RoundToInt(delta.y * 2);
            dungeonRooms.Add(DR);
            //newFloor.transform.localScale = new Vector3(((delta.x * gridSpacing) - 0.1f), 1, ((delta.y * gridSpacing) - 0.1f));
            //newFloor.transform.localScale = new Vector3((delta.x * gridSpacing), 1, (delta.y * gridSpacing));
            newFloor.transform.localScale = new Vector3((delta.x * 2) - 0.1f, 1, (delta.y * 2) - 0.1f);
            //Debug.Log("pos = " + delta.x + " : " + delta.y);
            yield return new WaitForSeconds(waitTime);
        }
        StartCoroutine(SeperateRooms());
    }

    private void SeperateRoomslol()
    {
        GO = GameObject.FindGameObjectsWithTag("Gen");
        for (int i = 0; i < MaxAttempts; i++)
        {
            int count = 0;
            foreach (GameObject a in GO)
            {            
                foreach (GameObject b in GO)
                {
                    if (a != b)
                    {
                        if (a.GetComponent<BoxCollider>().bounds.Intersects(b.GetComponent<BoxCollider>().bounds))
                        {
                            //get the GO furthest from the center
                            Vector3 A = new Vector3(0, 0, 0) + a.transform.position;
                            Vector3 B = new Vector3(0, 0, 0) + b.transform.position;
                            float distA = A.sqrMagnitude;
                            float distB = B.sqrMagnitude;

                            if (distB > distA)
                            {
                                count++;
                                Vector3 direction = (b.transform.position - a.transform.position) + (B / 2);
                                //Vector3 direction = (b.transform.position - a.transform.position);
                                direction.Normalize();
                                Vector3 moveDirection = FindMoveDiection(direction);
                                b.transform.position = new Vector3(
                                    b.transform.position.x + (moveDirection.x * 1.0f),
                                    b.transform.position.y,
                                    b.transform.position.z + (moveDirection.z * 1.0f)
                                    );
                            }
                            else
                            {
                                count++;
                                Vector3 direction = (a.transform.position - b.transform.position) + (A / 8);
                                //Vector3 direction = (a.transform.position - b.transform.position);
                                direction.Normalize();
                                Vector3 moveDirection = FindMoveDiection(direction);
                                a.transform.position = new Vector3(
                                    a.transform.position.x + (moveDirection.x * 1.0f),
                                    a.transform.position.y,
                                    a.transform.position.z + (moveDirection.z * 1.0f)
                                    );
                            }
                        }
                    }
                }               
            }
            if (count == 0)
            {
                Debug.Log("WOOOOOOOOOOOOOOOOOOOOOOOOOOO");
                break;
            }
        }
    }

    private void GenRoomslol()
    {
        for (int i = 0; i < numberOfRooms; i++)
        {
            Vector2 delta = GetRandomPointInCircle(Radius);
            int x = RoundToGrid(delta.x, gridSpacing);
            int y = RoundToGrid(delta.y, gridSpacing);
            delta = GetRandomRoomSize();
            var newFloor = Instantiate(floor, new Vector3(x, 0, y), Quaternion.identity);
            DungeonRoom DR = newFloor.GetComponent<DungeonRoom>();
            DR.ID = i +1; // set id (from 0+1)
            DR.sizeX = Mathf.RoundToInt(delta.x * 2);
            DR.sizeZ = Mathf.RoundToInt(delta.y * 2);
            dungeonRooms.Add(DR);
            //newFloor.transform.localScale = new Vector3(((delta.x * gridSpacing) - 0.1f), 1, ((delta.y * gridSpacing) - 0.1f));
            //newFloor.transform.localScale = new Vector3((delta.x * gridSpacing), 1, (delta.y * gridSpacing));
            newFloor.transform.localScale = new Vector3((delta.x * 2) - 0.1f, 1, (delta.y * 2) - 0.1f);
            //Debug.Log("pos = " + delta.x + " : " + delta.y);
        }
    }

    IEnumerator SeperateRooms()
    {
        GO = GameObject.FindGameObjectsWithTag("Gen");

        int count = 0;

        for (int i = 0; i < MaxAttempts; i++)
        {

            count = 0;

            foreach (GameObject a in GO)
            {
                foreach (GameObject b in GO)
                {
                    if (a != b)
                    {
                        if (a.GetComponent<BoxCollider>().bounds.Intersects(b.GetComponent<BoxCollider>().bounds))
                        {
                            //get the GO furthest from the center
                            Vector3 A = new Vector3(0, 0, 0) + a.transform.position;
                            Vector3 B = new Vector3(0, 0, 0) + b.transform.position;
                            float distA = A.sqrMagnitude;
                            float distB = B.sqrMagnitude;

                            if (distB > distA)
                            {
                                count++;
                                Vector3 direction = b.transform.position - a.transform.position;
                                direction.Normalize();
                                Vector3 moveDirection = FindMoveDiection(direction);
                                b.transform.position = new Vector3(
                                    b.transform.position.x + (moveDirection.x * 1.0f),
                                    b.transform.position.y,
                                    b.transform.position.z + (moveDirection.z * 1.0f)
                                    );
                            }
                            else
                            {
                                count++;
                                Vector3 direction = a.transform.position - b.transform.position;
                                direction.Normalize();
                                Vector3 moveDirection = FindMoveDiection(direction);
                                a.transform.position = new Vector3(
                                    a.transform.position.x + (moveDirection.x * 1.0f),
                                    a.transform.position.y,
                                    a.transform.position.z + (moveDirection.z * 1.0f)
                                    );
                            }
                            yield return new WaitForSeconds(waitTime);
                        }
                        
                    }
                }
            }
            if (count == 0)
            {
                Debug.Log("FINISHED SEPERATEING ROOMS - TODO - FIX BUG");
                break;
            }
        }
        if(count > 0)
        {
            //the map has not finished due to a collision conflict (bad programming)
            //restart the build!
        }

        setRooms();

        DTTime = true;

        //ColourMainRooms();
    }

    private Vector3 FindMoveDiection(Vector3 input)
    {
        //movement algorithm
        input.x = Mathf.RoundToInt(input.x);
        input.z = Mathf.RoundToInt(input.z);
        //Debug.Log("X: " + input.x + " Y: " + input.z);
        return input;
    }

    private void RemoveIntersecting()
    {
        int saftey = 5;
        int count = 10;
        int GOCount = GO.Length;
        float removeCount = 0;
        bool reset = false;
        List<GameObject> MarkForDestroy = new List<GameObject>();
        do
        {
            reset = false;
            count = 0;
            saftey--;        
            if (GO.Length != GOCount)
            {
                GO = GameObject.FindGameObjectsWithTag("Gen");
            }
            MarkForDestroy = new List<GameObject>();
            GOCount = GO.Length;
            foreach (GameObject a in GO)
            {
                foreach (GameObject b in GO)
                {
                    if (a != b)
                    {
                        if (a.GetComponent<BoxCollider>().bounds.Intersects(b.GetComponent<BoxCollider>().bounds))
                        {
                            count++;
                            removeCount++;
                            float sizeA = a.transform.localScale.x + a.transform.localScale.z;
                            float sizeB = b.transform.localScale.x + b.transform.localScale.z;
                            if (sizeA > sizeB)
                            {
                                MarkForDestroy.Add(b);
                                //Destroy(b);
                                reset = true;
                                //break;
                            }
                            else if (sizeB > sizeA)
                            {
                                MarkForDestroy.Add(a);
                                //Destroy(a);
                                reset = true;
                                //break;
                            }
                            else
                            {
                                MarkForDestroy.Add(b);
                                //Destroy(b);
                                reset = true;
                                //break;
                            }
                        }                    
                    }
                }
                if (reset)
                {
                    //break;
                }
            }
            //count = 0;
            foreach (var g in MarkForDestroy)
            {
                GameObject delta = g;
                //MarkForDestroy.Remove(g);
                //Destroy(delta);
                delta.SetActive(false);
            }
            MarkForDestroy.Clear();

        }
        while (count > 0 || saftey <= 0);
        //Debug.Log("saftey: " + saftey + " Count " + count);
        Debug.Log("Remove Count: " + removeCount);
    }

    private void ColourMainRooms()
    {
        float avgSize = 0;
        foreach(var g in GO)
        {
            avgSize += g.transform.localScale.x + g.transform.localScale.z;
        }
        avgSize = avgSize / GO.Length;
        foreach (var g in GO)
        {
            float deltaSize = g.transform.localScale.x + g.transform.localScale.z;
            if (deltaSize > avgSize * 1.0f) //1.15
            {
                try
                {
                    renderer = g.GetComponent<Renderer>();
                    renderer.material = newMat;
                    DungeonRoom DR = g.GetComponent<DungeonRoom>();
                    DR.isMain = true;
                }
                catch
                {
                    string oops = "oops";
                }

            }
        }
        //bool removeTest = true;
        //if(removeTest)
        //{
        //    foreach (var g in GO)
        //    {
        //        float distance = (new Vector3(0, 0, 0) - g.transform.position).magnitude;

        //        Debug.Log(distance);

        //        if (distance > 35 || distance < -35)
        //        {
        //            Destroy(g);
        //        }
        //    }
        //    //GameObject testing = GameObject.FindGameObjectWithTag("testing");
        //    //float distance2 = (new Vector3(0, 0, 0) - testing.transform.position).magnitude;
        //    //Debug.Log(distance2);
        //}
        GetStartRoom();
    }

    private GameObject GetStartRoom()
    {
        float furthestDistance = 0;
        float deltaDistance = 0;
        GameObject selected = new GameObject();
        foreach (var g in GO)
        {
            Vector3 A = new Vector3(0, 0, 0) + g.transform.position;
            deltaDistance = A.sqrMagnitude;
            if(deltaDistance > furthestDistance)
            {
                selected = g;
                furthestDistance = deltaDistance;
            }      
        }
        //renderer = selected.GetComponent<Renderer>();
        //renderer.material = startMat;
        return selected;
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.R))
        //{
        //    GenDungeon();
        //}

        //if(moveRooms)
        //{
        //    MoveRooms();
        //    moveRooms = false;
        //}

        if (DTTime)
        {
            if (!isFinished)
            {
                theDTController.setupTriangulation(roomList);

                isFinished = true;
            }
            else if (!DTFinished)
            {

                if (!theDTController.getDTDone())
                {
                    theDTController.Update();
                }
                else
                {
                    DTFinished = true;
                    thePrimController.setUpPrims(roomList, theDTController.getTriangulation());
                }
            }
            else if (!PrimFinished)
            {
                thePrimController.Update();
                PrimFinished = true;
            }
            else if (!LayoutFinished)
            {         
                GameObject startRoom = GetStartRoom(); // find start room (furthest from center)

                LayoutFinished = true;
                ConvertLayoutController = new ConvertToLayout(roomList, startRoom, thePrimController); // connecting all rooms in order

            }
            else if (!prepFinished)
            {
                prepFinished = true;
                LayoutController.SetGrid(dungeonRooms); //convert rooms to a grid
            }
        }
    }

    private float RoundUpToGridSize(float value)
    {
        //Debug.Log("in: " + value);
        int delta = Mathf.RoundToInt(value / gridSpacing);
        value = delta * gridSpacing;
        //Debug.Log("out: " + value);
        return value;
    }

    private void MoveRooms()
    {
        float doneCount = 0;
        doneCount = 100;
        bool die = false;
        if (die)
        {
            //bool doOnce = true;
            //if (doOnce)
            //{
            //    foreach (GameObject a in GO)
            //    {
            //        var rb = a.AddComponent<Rigidbody>();
            //        rb.constraints = RigidbodyConstraints.FreezePositionY
            //            | RigidbodyConstraints.FreezeRotation;
            //        rb.useGravity = false;
            //        rb.drag = 10;
            //        //rb.constraints = RigidbodyConstraints.FreezePositionX
            //    }
            //    foreach (GameObject a in GO)
            //    {
            //        a.transform.position = new Vector3(a.transform.position.x, a.transform.position.y - 0.5f, a.transform.position.z);
            //    }
            //    doOnce = false;
            //}
            //if (doneCount == 0)
            //{
            //    Debug.Log("DONE!!!!!!");
            //    moveRooms = false;
            //}
            //else if (doneCount <= Mathf.RoundToInt(numberOfRooms * 0.15f))
            //{
            //    //foreach(var a in GO)
            //    //{
            //    //    if (a.GetComponent<MeshCollider>().bounds.Intersects(GetComponent<MeshCollider>().bounds))
            //    //    {
            //    //        Destroy(a);
            //    //    }
            //    //}
            //}
        }

        // int count = 0;
        for (int i = 0; i < MaxAttempts; i++)
        {
            foreach (GameObject a in GO)
            {
                foreach (GameObject b in GO)
                {
                    if (a != b)
                    {
                        if (a.GetComponent<BoxCollider>().bounds.Intersects(b.GetComponent<BoxCollider>().bounds))
                        {
                            doneCount++;
                            Vector3 direction = a.transform.position - b.transform.position;
                            direction.Normalize();
                            //a.transform.position = new Vector3(
                            //    RoundUpToGridSize(a.transform.position.x + (direction.x * 1.0f)),
                            //    a.transform.position.y,
                            //    RoundUpToGridSize(a.transform.position.z + (direction.z * 1.0f))
                            //    );
                            //a.transform.position = new Vector3(
                            //    RoundUpToGridSize(a.transform.position.x + (direction.x * strength)),
                            //    a.transform.position.y,
                            //    RoundUpToGridSize(a.transform.position.z + (direction.z * strength))
                            //    );
                            a.transform.position = new Vector3(
                                Mathf.Round(a.transform.position.x + (direction.x * strength)),
                                a.transform.position.y,
                                Mathf.Round(a.transform.position.z + (direction.z * strength))
                                );
                            a.transform.position = new Vector3(
                                RoundUpToGridSize(a.transform.position.x),
                                a.transform.position.y,
                                RoundUpToGridSize(a.transform.position.z)
                                );
                        }
                    }
                }
            }
            if (doneCount == 0)
            {
                Debug.Log("HAYYYYYYYYYYY");
                moveRooms = false;
                break;
            }
        }
        float destroyCount = 0;
        foreach (GameObject a in GO)
        {
            try
            {
                foreach (GameObject b in GO)
                {
                    try
                    {
                        if (a != b)
                        {
                            if (a.GetComponent<BoxCollider>().bounds.Intersects(b.GetComponent<BoxCollider>().bounds))
                            {
                                destroyCount++;
                                //Destroy(b);
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }

            //check if they are colliding and delete if true
            //try
            //{
            //    var box = a.GetComponent<DungeonRoom>();
            //    if(box.isColliding)
            //    {
            //        Debug.Log("BIG SHACK!");
            //        Destroy(a);
            //    }
            //}
            //catch { }
        }
        
        Debug.Log(destroyCount);
    }

    private void GenDungeon()
    {
        //make this number of rooms
        for (int i = 0; i < numberOfRooms; i++)
        {
            Vector2 delta = GetRandomPointInCircle(Radius);
            int x = RoundToGrid(delta.x, gridSpacing);
            int y = RoundToGrid(delta.y, gridSpacing);
            delta = GetRandomRoomSize();
            var newFloor = Instantiate(floor, new Vector3(x, 0, y), Quaternion.identity);
            //newFloor.transform.localScale = new Vector3(((delta.x * gridSpacing) - 0.1f), 1, ((delta.y * gridSpacing) - 0.1f));
            //newFloor.transform.localScale = new Vector3((delta.x * gridSpacing), 1, (delta.y * gridSpacing));
            newFloor.transform.localScale = new Vector3((delta.x * 2), 1, (delta.y * 2));
            //Debug.Log("pos = " + delta.x + " : " + delta.y);
        }
        moveRooms = false;
    }

    private Vector2 GetRandomPointInCircle(float radius)
    {
        //for other methods, see notes at bottom!!!!!!!!!!!!!!!!!


        //circle
        //var angle = Random.value * (Mathf.PI * 2);
        //var radius2 = Mathf.Sqrt(Random.value) * radius;
        ////var x = (radius * Mathf.Cos(angle) - (radius / 2));
        ////var y = (radius * Mathf.Sin(angle) - (radius / 2));
        //var x = (radius2 * Mathf.Cos(angle));
        //var y = (radius2 * Mathf.Sin(angle));
        //Vector2 delta = new Vector2(x, y);
        ////Debug.Log(delta);

        //elipse
        var angle = Random.value * (Mathf.PI * 2);
        var radiusX = Mathf.Sqrt(Random.value) * radius;
        var radiusY = Mathf.Sqrt(Random.value) * radius * 2;
        //var x = (radius * Mathf.Cos(angle) - (radius / 2));
        //var y = (radius * Mathf.Sin(angle) - (radius / 2));
        var x = (radiusX * Mathf.Cos(angle));
        var y = (radiusY * Mathf.Sin(angle));
        Vector2 delta = new Vector2(x, y);


        return new Vector2(x, y);
    }

    private int RoundToGrid(float pos, float grid)
    {
        return Mathf.RoundToInt(Mathf.Floor((pos + grid - 1) * grid));
    }

    private Vector2 GetRandomRoomSize()
    {
        int x = Mathf.RoundToInt(Random.Range(minRoomSize, maxRoomSize));
        int y = Mathf.RoundToInt(Random.Range(minRoomSize, maxRoomSize));
        return new Vector2(x, y);
    }

    private void TriangulateAllRooms()
    {
        Vector3 A = new Vector3();
        Vector3 B = new Vector3();
        int count = -1;
        var points = new Vector3[200];

        lineRender = new LineRenderer();
        lineRender = this.gameObject.AddComponent<LineRenderer>();
        lineRender.startWidth = 0.5f;
        lineRender.endWidth = 0.5f;
        lineRender.positionCount = 200;

        foreach (var a in GO)
        {
            GameObject closestA = new GameObject();
            GameObject closestB = new GameObject();

            float firstClosest = 0;
            float secondClosest = 0;

            foreach (var b in GO)
            {
                if (a != b)
                {
                    A = a.transform.position;
                    B = b.transform.position;
                    A.y += 1.5f;
                    B.y += 1.5f;

                    float distance = (A + B).sqrMagnitude;
                    
                    if(distance < firstClosest)
                    {
                        closestA = b;
                        try { closestB = closestA; }
                        catch { }
                        secondClosest = firstClosest;
                        firstClosest = distance;
                    }
                    else if(distance < secondClosest)
                    {
                        closestB = b;
                        secondClosest = distance;
                    }
                }
            }
            //draw a line to the 2 closest!!
            Vector3 delta = new Vector3();
            delta = a.transform.position;
            delta.y += 1.5f;
            count++;
            lineRender.SetPosition(count, delta);
            delta = A;
            delta.y += 1.5f;
            count++;
            lineRender.SetPosition(count, delta);
            delta = a.transform.position;
            delta.y += 1.5f;
            count++;
            lineRender.SetPosition(count, delta);
            delta = B;
            delta.y += 1.5f;
            count++;
            lineRender.SetPosition(count, delta);
        }
        
    }

    //IEnumerator TriangulateAllRoomslol()
    //{
    //    Vector3 A = new Vector3();
    //    Vector3 B = new Vector3();
    //    int count = -1;
    //    var points = new Vector3[200];



    //    lineRender = new LineRenderer();
    //    lineRender = this.gameObject.AddComponent<LineRenderer>();
    //    lineRender.startWidth = 0.5f;
    //    lineRender.endWidth = 0.5f;
    //    lineRender.positionCount = 200;

    //    int roundCount = 0;

    //    foreach (var a in GO)
    //    {
    //        Debug.Log(roundCount);
    //        roundCount++;
    //        GameObject closestA = new GameObject();
    //        GameObject closestB = new GameObject();

    //        float firstClosest = 10000;
    //        float secondClosest = 1000000;

    //        foreach (var b in GO)
    //        {
    //            Debug.Log(roundCount);
    //            roundCount++;
    //            if (a != b)
    //            {
    //                A = a.transform.position;
    //                B = b.transform.position;

    //                float distance = (A - B).sqrMagnitude;

    //                if (distance < firstClosest)
    //                {
    //                    try { closestB = closestA; }
    //                    catch { }
    //                    closestA = b;    
    //                    secondClosest = firstClosest;
    //                    firstClosest = distance;
    //                }
    //                else if (distance < secondClosest)
    //                {
    //                    closestB = b;
    //                    secondClosest = distance;
    //                }
    //            }
    //        }
    //        //draw a line to the 2 closest!!
    //        Vector3 delta = new Vector3();
    //        delta = a.transform.position;
    //        delta.y -= 1.5f;
    //        count++;
    //        lineRender.SetPosition(count, delta);

    //        delta = new Vector3();
    //        delta = a.transform.position;
    //        delta.y += 1.5f;
    //        count++;
    //        lineRender.SetPosition(count, delta);

    //        delta = closestA.transform.position;
    //        delta.y += 1.5f;
    //        count++;
    //        lineRender.SetPosition(count, delta);

    //        delta = a.transform.position;
    //        delta.y += 1.5f;
    //        count++;
    //        lineRender.SetPosition(count, delta);

    //        delta = closestB.transform.position;
    //        delta.y += 1.5f;
    //        count++;
    //        lineRender.SetPosition(count, delta);

    //        delta = closestA.transform.position;
    //        delta.y += 1.5f;
    //        count++;
    //        lineRender.SetPosition(count, delta);

    //        delta = closestA.transform.position;
    //        delta.y -= 1.5f;
    //        count++;
    //        lineRender.SetPosition(count, delta);

    //        yield return new WaitForSeconds(waitTime);
    //    }
    //}

    //private void OnDrawGizmos(Vector3 from, Vector3 to)
    //{

    //    // Draws a blue line from this transform to the target
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawLine(from, to);
    //    Debug.Log("DONE!");
    //}


}





//NOTES---------------------------------------

//class Grid
//{
//    public int x, y;
//    //public string type;
//    public Grid(int deltaX, int deltaY)//, string deltaType)
//    {
//        x = deltaX;
//        y = deltaY;
//        //type = deltaType;
//    }
//}




//function getRandomPointInCircle(radius)
//    local t = 2 * math.pi * math.random()
//    local u = math.random() + math.random()
//    local r = nil
//    if u > 1 then r = 2 - u else r = u end
//    return radius* r*math.cos(t), radius* r*math.sin(t)
//end

//-- Now we can change the returned value from getRandomPointInCircle to:

//function getRandomPointInCircle(radius)
//  
//  return roundm(radius* r*math.cos(t), tile_size), 
//         roundm(radius* r*math.sin(t), tile_size)
//end