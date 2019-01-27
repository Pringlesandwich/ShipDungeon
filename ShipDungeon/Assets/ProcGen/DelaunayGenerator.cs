using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelaunayGenerator : MonoBehaviour {

    public GameObject[] GO;

    public GameObject spawn;

	// Use this for initialization
	void Start () {
        GO = GameObject.FindGameObjectsWithTag("Gen");

        //GenTriangles();

    }





   
    public class Vertex
    {
        public Vector3 position;

        //The outgoing halfedge (a halfedge that starts at this vertex)
        //Doesnt matter which edge we connect to it
        public HalfEdge halfEdge;

        //Which triangle is this vertex a part of?
        public Triangle triangle;

        //The previous and next vertex this vertex is attached to
        public Vertex prevVertex;
        public Vertex nextVertex;

        //Properties this vertex may have
        //Reflex is concave
        public bool isReflex;
        public bool isConvex;
        public bool isEar;

        public Vertex(Vector3 position)
        {
            this.position = position;
        }

        //Get 2d pos of this vertex
        public Vector2 GetPos2D_XZ()
        {
            Vector2 pos_2d_xz = new Vector2(position.x, position.z);

            return pos_2d_xz;
        }
    }

    public class HalfEdge
    {
        //The vertex the edge points to
        public Vertex v;

        //The face this edge is a part of
        public Triangle t;

        //The next edge
        public HalfEdge nextEdge;
        //The previous
        public HalfEdge prevEdge;
        //The edge going in the opposite direction
        public HalfEdge oppositeEdge;

        //This structure assumes we have a vertex class with a reference to a half edge going from that vertex
        //and a face (triangle) class with a reference to a half edge which is a part of this face 
        public HalfEdge(Vertex v)
        {
            this.v = v;
        }
    }

    public class Triangle
    {
        //Corners
        public Vertex v1;
        public Vertex v2;
        public Vertex v3;

        //If we are using the half edge mesh structure, we just need one half edge
        public HalfEdge halfEdge;

        public Triangle(Vertex v1, Vertex v2, Vertex v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }

        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            this.v1 = new Vertex(v1);
            this.v2 = new Vertex(v2);
            this.v3 = new Vertex(v3);
        }

        public Triangle(HalfEdge halfEdge)
        {
            this.halfEdge = halfEdge;
        }

        //Change orientation of triangle from cw -> ccw or ccw -> cw
        public void ChangeOrientation()
        {
            Vertex temp = this.v1;

            this.v1 = this.v2;

            this.v2 = temp;
        }
    }

    public class Edge
    {
        public Vertex v1;
        public Vertex v2;

        //Is this edge intersecting with another edge?
        public bool isIntersecting = false;

        public Edge(Vertex v1, Vertex v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public Edge(Vector3 v1, Vector3 v2)
        {
            this.v1 = new Vertex(v1);
            this.v2 = new Vertex(v2);
        }

        //Get vertex in 2d space (assuming x, z)
        public Vector2 GetVertex2D(Vertex v)
        {
            return new Vector2(v.position.x, v.position.z);
        }

        //Flip edge
        public void FlipEdge()
        {
            Vertex temp = v1;

            v1 = v2;

            v2 = temp;
        }
    }


    public class Plane
    {
        public Vector3 pos;

        public Vector3 normal;

        public Plane(Vector3 pos, Vector3 normal)
        {
            this.pos = pos;

            this.normal = normal;
        }
    }


























    //private void GenTriangles()
    //{
    //    Vector3 A = new Vector3();
    //    Vector3 B = new Vector3();
    //    Vector3 C = new Vector3();
    //    Vector3 Center = new Vector3();

    //    bool aGood = false;
    //    bool bGood = false;
    //    bool cGood = false;

    //    bool looping = true;


    //    foreach (var a in GO)
    //    {
    //        looping = a.GetComponent<DungeonRoom>().edgesComplete;
    //        A = a.transform.position;
    //        foreach (var b in GO)
    //        {
    //            B = b.transform.position;
    //            if (a != b)
    //            {
    //                foreach (var c in GO)
    //                {
    //                    C = c.transform.position;
    //                    if (!(b == c || a == c))
    //                    {
    //                        bool proceed = false;
    //                        //make a triangle with these 3 and get the center
    //                        Center = FindCircumcenter(A, B, C);
    //                        Instantiate(spawn, Center, Quaternion.identity);
    //                        //get distance to a
    //                        // the radius is the distance between center and any point
    //                        // distance(A, B) = length(A-B)
    //                        var radius = Vector3.Distance(Center, A);

    //                        //foreach that isnt part of the triangle
    //                        foreach (var test in GO)
    //                        {
    //                            if (test == a || test == b || test == c)
    //                            {
    //                                //do nothing
    //                            }
    //                            else
    //                            {
    //                                float distance = Vector3.Distance(test.transform.position, Center);
    //                                if (distance <= radius)
    //                                {
    //                                    //BAD TRIANGLE
    //                                    proceed = false;
    //                                    Debug.Log(proceed);
    //                                }
    //                                else
    //                                {
    //                                    looping = false;
    //                                    proceed = true;
    //                                    Debug.Log(proceed);
    //                                    //Instantiate(spawn, Center, Quaternion.identity);
    //                                }
    //                            }
    //                            if (!looping)
    //                                break;
    //                        }
    //                    }
    //                    if (!looping)
    //                        break;
    //                }
    //            }
    //            if (!looping)
    //                break;
    //        }
    //    }
    //}

    //private Vector3 FindCircumcenter(Vector3 A, Vector3 B, Vector3 C)
    //{

    //    // lines from a to b and a to c
    //    var AB = B - A;
    //    var AC = C - A;

    //    // perpendicular vector on triangle
    //    var N = Vector3.Normalize(Vector3.Cross(AB, AC));

    //    // find the points halfway on AB and AC
    //    var halfAB = A + AB * 0.5f;
    //    var halfAC = A + AC * 0.5f;

    //    // build vectors perpendicular to ab and ac
    //    var perpAB = Vector3.Cross(AB, N);
    //    var perpAC = Vector3.Cross(AC, N);

    //    // find intersection between the two lines
    //    // D: halfAB + t*perpAB
    //    // E: halfAC + s*perpAC
    //    var center = LineLineIntersection(halfAB, perpAB, halfAC, perpAC);

    //    return center;
    //}
    ///// <summary>
    ///// Calculates the intersection point between two lines, assuming that there is such a point.
    ///// </summary>
    ///// <param name="originD">The origin of the first line</param>
    ///// <param name="directionD">The direction of the first line.</param>
    ///// <param name="originE">The origin of the second line.</param>
    ///// <param name="directionE">The direction of the second line.</param>
    ///// <returns>The point at which the two lines intersect.</returns>
    //Vector3 LineLineIntersection(Vector3 originD, Vector3 directionD, Vector3 originE, Vector3 directionE)
    //{
    //    directionD.Normalize();
    //    directionE.Normalize();
    //    var N = Vector3.Cross(directionD, directionE);
    //    var SR = originD - originE;
    //    var absX = Mathf.Abs(N.x);
    //    var absY = Mathf.Abs(N.y);
    //    var absZ = Mathf.Abs(N.z);
    //    float t;
    //    if (absZ > absX && absZ > absY)
    //    {
    //        t = (SR.x * directionE.y - SR.y * directionE.x) / N.z;
    //    }
    //    else if (absX > absY)
    //    {
    //        t = (SR.y * directionE.z - SR.z * directionE.y) / N.x;
    //    }
    //    else
    //    {
    //        t = (SR.z * directionE.x - SR.x * directionE.z) / N.y;
    //    }
    //    return originD - t * directionD;
    //}

    ///// <summary>
    ///// Calculates the distance between a point and a line.
    ///// </summary>
    ///// <param name="P">The point.</param>
    ///// <param name="S">The origin of the line.</param>
    ///// <param name="D">The direction of the line.</param>
    ///// <returns>
    ///// The distance of the point to the line.
    ///// </returns>
    ////float PointLineDistance(Vector3 P, Vector3 S, Vector3 D)
    ////{
    ////    D.Normalize();
    ////    var SP = P - S;
    ////    return Vector3.Distance(SP, Vector3.Dot(SP, D) * D);
    ////}

}
