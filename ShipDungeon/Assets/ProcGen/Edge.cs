//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Edge {

    private VertexNode node0;
    private VertexNode node1;

    private Color theDrawColor = new Color(255, 0, 0, 1);
    private LineRenderer theLine;

    private bool isPrims = false;

    public Edge(VertexNode _n0, VertexNode _n1)
    {
        node0 = _n0;
        node1 = _n1;
        theLine = new GameObject().AddComponent<LineRenderer>();
        theLine.material = new Material(Shader.Find("Particles/Additive"));
        theLine.name = "EdgeLine";
        theLine.tag = "Lines";
        //theLine.startColor = theDrawColor;
        //theLine.endColor = theDrawColor;
    }

    public VertexNode getNode0()
    {
        return node0;
    }

    public VertexNode getNode1()
    {
        return node1;
    }

    public bool checkSame(Edge _aEdge)
    {
        if ((node0 == _aEdge.getNode0() || node0 == _aEdge.getNode1()) &&
              (node1 == _aEdge.getNode0() || node1 == _aEdge.getNode1()))
        {
            return true;
        }

        return false;
    }

    public bool edgeContainsVertex(VertexNode _aNode)
    {
        if (node0 == _aNode || node1 == _aNode)
        {
            return true;
        }

        return false;
    }

    public void drawEdge()
    {
        if (node0.getParentCell() != null && node1.getParentCell() != null)
        {
            if (theLine == null)
            {
                theLine = new GameObject().AddComponent<LineRenderer>();
                theLine.name = "EdgeLine";
                theLine.material = new Material(Shader.Find("Particles/Additive"));
                theLine.tag = "Lines";
            }

            //theLine.SetWidth(0.7f, 0.7f);
            theLine.startWidth = 0.7f;
            theLine.endWidth = 0.7f;

            //theLine.renderer.material.color = theDrawColor;
            //theLine.SetColors(theDrawColor, theDrawColor);
            theLine.startColor = theDrawColor;
            theLine.endColor = theDrawColor;

            //theLine.SetVertexCount(2);
            theLine.positionCount = 2;

            theLine.SetPosition(0, new Vector3(node0.getVertexPosition().x, 3, node0.getVertexPosition().z));
            theLine.SetPosition(1, new Vector3(node1.getVertexPosition().x, 3, node1.getVertexPosition().z));

            //theLine.SetPosition(0, new Vector3(node0.getVertexPosition().x, node0.getVertexPosition().y, -3));
            //theLine.SetPosition(1, new Vector3(node1.getVertexPosition().x, node1.getVertexPosition().y, -3));
        }
    }

    public void setDrawColor(Color _theColor)
    {
        theDrawColor = new Color(_theColor.r, _theColor.g, _theColor.b, 1);
        if (theLine != null)
        {
            //theLine.renderer.material.color = theDrawColor;
            theLine.material = new Material(Shader.Find("Particles/Additive"));
            //isPrims = true;
        }
    }

    public bool getIsPrims()
    {
        //if(theDrawColor.r == 0 && theDrawColor.g == 255 && theDrawColor.b == 0)
        //{
        //    return true;
        //}
        //else { return false; }
        return isPrims;
    }

    public void makeIsPrims()
    {
        isPrims = true;
    }

    public void stopDraw()
    {
        if (theLine != null)
        {
            GameObject.Destroy(theLine.gameObject);
        }
    }
}
