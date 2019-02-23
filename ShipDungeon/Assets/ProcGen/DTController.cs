using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DTController {

    private bool isDone = false;

    //All the triangles in the triangulation
    private List<Triangle> triangleList = new List<Triangle>();

    //Verticies that still need to be added to the triangulations
    private List<VertexNode> toAddList = new List<VertexNode>();

    //the current verticie that is being added to the triangulation
    private VertexNode nextNode = null;

    //Edges that have become possibly unDelaunay due to the insertion of another verticie
    private List<Edge> dirtyEdges = new List<Edge>();

    //controls if click control should be allowed
    private bool doStep = false;
    private bool canClick;
    ////controls if the algorithum should animate the process step by step
    private bool animate = false;
    ////time between steps
    private float animateTime = 0.5f;
    private float animateTimer = 0;
    //current stage the algorithum is at (used for step and animate control)
    int stage = 0;

    //the omega triangle created at start of triangulation
    private Triangle rootTriangle; 

    //the triangle the "nextNode" is inside of
    private Triangle inTriangle;

    private List<Edge> finalTriangulation = new List<Edge>(); // NOT USED SO FAR!!!!!!

    //List of cells that have been turned into rooms
    //private List<VertexNode> roomList = new List<VertexNode>();

    //NEEDED??????
    public DTController()
    {

    }

    // Update is called once per frame
    public void Update () {

        //logic here controls the different playback modes the algorithum can be executed in
        if (doStep)
        {
            
        }
        else
        {
            if (!animate)
            {
                while (toAddList.Count > 0)
                {
                    addVertexToTriangulation();
                }
                trigDone();
            }
        }

        if (animate)
        {
            if (animateTimer < animateTime)
            {
                animateTimer += 1 * Time.deltaTime;
            }
            else
            {
                if (toAddList.Count > 0)
                {
                    addVertexToTriangulation();
                }
                else
                {
                    if (stage != 0)
                    {
                        addVertexToTriangulation();
                        trigDone();
                    }
                }
                animateTimer %= animateTime;
            }
        }

        drawTriangles();

    }

    private void drawTriangles()
    {
        //Debug.Log("drawTriangle");
        foreach (Triangle aTri in triangleList)
        {
            //Debug.Log("!11111111!!!1");
            aTri.drawTriangle();
        }
    }

    public bool getDTDone()
    {
        return isDone;
    }

    public void setDTDone(bool delta)
    {
        isDone = delta;
    }

    public void clearList()
    {
        toAddList.Clear();
    }

    //Handles set up of triangulation
    public void setupTriangulation(List<VertexNode> _roomList)
    {
        //Debug.Log(_roomList.Count);
        //puts all verticies into the toDo list
        foreach (VertexNode aNode in _roomList)
        {
            toAddList.Add(aNode);
        }

        //creates three artificial verticies for the omega triangle
        VertexNode node0 = new VertexNode(0, 250, null);      
        VertexNode node1 = new VertexNode(-250, -200, null);
        VertexNode node2 = new VertexNode(250, -200, null);

        //creates the omega triangle
        rootTriangle = new Triangle(new Edge(node0, node1), new Edge(node0, node2), new Edge(node1, node2));

        //adds the omega triangle to the triangle list
        triangleList.Add(rootTriangle);
    }



    //Adds a verticies to the triangulation
    private void addVertexToTriangulation()
    {
        //check what mode the triangulation is running in
        if (stage == 0 || (!doStep && !animate))
        {
            //Debug.Log("Stage 0");
            //Find a Random verticie from the todo list
            int choice = Random.Range(0, toAddList.Count);

            //Change the color of all other verticies
            foreach (VertexNode aNode in toAddList)
            {
                aNode.getParentCell().GetComponent<Renderer>().material.color = new Color(255, 255, 255, 255);
            }

            //set next node to selected verticies
            nextNode = toAddList[choice];

            nextNode.getParentCell().GetComponent<Renderer>().material.color = new Color(0, 0, 255, 255);

            //Debug.Log("Remove");

            //remove selected verticies from todo list
            toAddList.Remove(nextNode);

            if (doStep || animate)
            {
                stage++;
                return;
            }
        }

        if (stage == 1 || (!doStep && !animate))
        {
            //Debug.Log("Stage 1");
            //stores triangles created during the loop to be appended to main list after loop
            List<Triangle> tempTriList = new List<Triangle>();

            //All edges are clean at this point. Remove any that may be left over from previous loop
            dirtyEdges.Clear();

            float count = -1;
            foreach (Triangle aTri in triangleList)
            {
                //Debug.Log("triangleList: " + triangleList.Count); 
                List<Edge> triEdges = aTri.getEdges();
                count++;
                //Debug.Log("Check");
                //Find which triangle the current vertex being add is located within

                //trun all postions into false vector2's
                Vector2 A = new Vector2(nextNode.getVertexPosition().x, nextNode.getVertexPosition().z);
                Vector2 a = new Vector2(triEdges[0].getNode0().getVertexPosition().x, triEdges[0].getNode0().getVertexPosition().z);
                Vector2 b = new Vector2(triEdges[0].getNode1().getVertexPosition().x, triEdges[0].getNode1().getVertexPosition().z);
                Vector2 c = new Vector2(triEdges[1].getNode1().getVertexPosition().x, triEdges[1].getNode1().getVertexPosition().z);

                //if (LineIntersector.PointInTraingle(
                //    nextNode.getVertexPosition(),
                //    triEdges[0].getNode0().getVertexPosition(),
                //    triEdges[0].getNode1().getVertexPosition(),
                //    triEdges[1].getNode1().getVertexPosition()))
                if (LineIntersector.PointInTraingle(A, a, b, c))
                {
                    //Debug.Log("true");
                    //cache the triangle we are in so we can delete it after loop
                    inTriangle = aTri;

                    //create three new triangles from each edge of the triangle vertex is in to the new vertex
                    foreach (Edge aEdge in aTri.getEdges())
                    {
                        Triangle nTri1 = new Triangle(new Edge(nextNode, aEdge.getNode0()),
                                        new Edge(nextNode, aEdge.getNode1()),
                                        new Edge(aEdge.getNode1(), aEdge.getNode0()));

                        //cache created triangles so we can add to list after loop
                        tempTriList.Add(nTri1);

                        //mark the edges of the old triangle as dirty
                        dirtyEdges.Add(new Edge(aEdge.getNode0(), aEdge.getNode1()));

                    }

                    break;
                }
            }

            //add the three new triangles to the triangle list
            foreach (Triangle aTri in tempTriList)
            {
                //Debug.Log("ADSADASDASD");
                triangleList.Add(aTri);
            }

            //delete the old triangle that the vertex was inside of
            if (inTriangle != null)
            {
                triangleList.Remove(inTriangle);
                inTriangle.stopDraw();
                inTriangle = null;
            }

            if (doStep || animate)
            {
                stage++;
                return;
            }
        }

        if (stage == 2 || !doStep)
        {
            //Debug.Log("Stage 2");
            //recursively check the dirty edges to make sure they are still delaunay
            checkEdges(dirtyEdges);
        }

    }


    private void checkEdges(List<Edge> _list)
    {
        //stores if a flip occured for mode control
        bool didFlip = false;

        //the current dirty edge
        if (_list.Count == 0)
        {
            stage = 0;
            if (animate || doStep)
            {
                if (toAddList.Count > 0)
                {
                    addVertexToTriangulation();
                }
            }
            return;
        }

        //get the next edge in the dirty list
        Edge currentEdge = _list[0];

        Triangle[] connectedTris = new Triangle[2];
        int index = 0;


        foreach (Triangle aTri in triangleList)
        {
            if (aTri.checkTriangleContainsEdge(currentEdge))
            {
                connectedTris[index] = aTri;
                index++;
            }
        }


        //in first case (omega triangle) this will = 1 so dont flip
        if (index == 2)
        {
            //stores the two verticies from both triangles that arnt on the shared edge
            VertexNode[] uniqueNodes = new VertexNode[2];
            int index1 = 0;

            //loop through the connected triangles and there edges. Checking for a vertex that isnt in the edge
            for (int i = 0; i < connectedTris.Length; i++)
            {
                foreach (Edge aEdge in connectedTris[i].getEdges())
                {
                    if (!currentEdge.edgeContainsVertex(aEdge.getNode0()))
                    {
                        uniqueNodes[index1] = aEdge.getNode0();
                        index1++;
                        break;
                    }

                    if (!currentEdge.edgeContainsVertex(aEdge.getNode1()))
                    {
                        uniqueNodes[index1] = aEdge.getNode1();
                        index1++;
                        break;
                    }
                }
            }

            //find the angles of the two unique verticies
            float angle0 = calculateVertexAngle(uniqueNodes[0].getVertexPosition(),
                                                currentEdge.getNode0().getVertexPosition(),
                                                currentEdge.getNode1().getVertexPosition());

            float angle1 = calculateVertexAngle(uniqueNodes[1].getVertexPosition(),
                                                currentEdge.getNode0().getVertexPosition(),
                                                currentEdge.getNode1().getVertexPosition());

            //Check if the target Edge needs flipping
            if (angle0 + angle1 > 180)
            {
                didFlip = true;

                //create the new edge after flipped
                Edge flippedEdge = new Edge(uniqueNodes[0], uniqueNodes[1]);

                //store the edges of both triangles in the Quad
                Edge[] firstTriEdges = new Edge[3];
                Edge[] secondTriEdges = new Edge[3];

                VertexNode sharedNode0;
                VertexNode sharedNode1;

                //set the shared nodes on the shared edge
                sharedNode0 = currentEdge.getNode0();
                sharedNode1 = currentEdge.getNode1();

                //construct a new triangle to update old triangle after flip
                firstTriEdges[0] = new Edge(uniqueNodes[0], sharedNode0);
                firstTriEdges[1] = new Edge(sharedNode0, uniqueNodes[1]);
                firstTriEdges[2] = flippedEdge;

                //construct a new triangle to update the other old triangle after flip
                secondTriEdges[0] = new Edge(uniqueNodes[1], sharedNode1);
                secondTriEdges[1] = new Edge(sharedNode1, uniqueNodes[0]);
                secondTriEdges[2] = flippedEdge;

                //update the edges of the triangles involved in the flip
                connectedTris[0].setEdges(firstTriEdges[0], firstTriEdges[1], firstTriEdges[2]);
                connectedTris[1].setEdges(secondTriEdges[0], secondTriEdges[1], secondTriEdges[2]);


                //Adds all edges to be potentially dirty. This is bad and should only add the edges that *could* be dirty
                foreach (Edge eEdge in connectedTris[0].getEdges())
                {
                    _list.Add(eEdge);
                }

                foreach (Edge eEdge in connectedTris[1].getEdges())
                {
                    _list.Add(eEdge);
                }

                //also add new edge to dirty list
                _list.Add(flippedEdge);
            }
        }

        //remove the current edge from the dirty list
        _list.Remove(currentEdge);

        if (doStep || animate)
        {
            if (!didFlip)
            {
                checkEdges(_list);
            }
        }
        else
        {
            checkEdges(_list);
        }
    }


    //private float calculateVertexAngle(Vector2 _target, Vector2 _shared0, Vector2 _shared1) // MIGHT NEED TO CHANGE TO VECTOR3 FOR FLAT!!!!!!!!!!!!!!!!!
    //{
    //    float length0 = Vector2.Distance(_target, _shared0);
    //    float length1 = Vector2.Distance(_shared0, _shared1);
    //    float length2 = Vector2.Distance(_shared1, _target);

    //    return Mathf.Acos(((length0 * length0) + (length2 * length2) - (length1 * length1)) / (2 * length0 * length2)) * Mathf.Rad2Deg;
    //}

    private float calculateVertexAngle(Vector3 _target, Vector3 _shared0, Vector3 _shared1)
    {
        float length0 = Vector3.Distance(_target, _shared0);
        float length1 = Vector3.Distance(_shared0, _shared1);
        float length2 = Vector3.Distance(_shared1, _target);

        return Mathf.Acos(
            (
            (length0 * length0) + (length2 * length2) - (length1 * length1)
            )
            / (2 * length0 * length2)
            )
            * Mathf.Rad2Deg; //WHAT DOES THIS DO???????
    }


    private void trigDone()
    {
        //Debug.Log("DONE!");
        isDone = true;
        constructFinal();
    }


    //Construct a list of all the edges actually in the triangulation
    private void constructFinal()
    {
        foreach (Triangle aTriangle in triangleList)
        {
            foreach (Edge aEdge in aTriangle.getEdges())
            {
                //stop edges connecting to the omega triangle to be added to the final list
                if (aEdge.getNode0().getParentCell() != null && aEdge.getNode1().getParentCell() != null)
                {
                    finalTriangulation.Add(aEdge);
                }
               
                //aEdge.stopDraw();
            }
        }
    }

    public List<Edge> getTriangulation()
    {
        return finalTriangulation;
    }

}
