using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Prims {

    private Hashtable vertexTable = new Hashtable();

    private List<VertexNode> allNodes;
    private List<Edge> allEdges;

    private List<Edge> edgesInTree = new List<Edge>();
    private List<VertexNode> nodesInTree = new List<VertexNode>();

    private List<Edge> finalPrimList = new List<Edge>();

    public void Update()
    {
        //int count = 0;
        bool flipFlop = false;
        //int count = 0;
        foreach (Edge aEdge in edgesInTree)
        {
            if (flipFlop) // stops double lines being drawn!
            {
                //count++;
                aEdge.drawEdge();
                aEdge.makeIsPrims();
                finalPrimList.Add(aEdge); // did this to simplify logic
            }
            flipFlop = !flipFlop;
        }
        //Debug.Log("QSASADQQQQQQ " + count);

        var toRemove = allEdges.Where(x => x.getIsPrims() == false);

        foreach (var x in toRemove)
        {
            x.stopDraw();
        }


        //Debug.Log("JHASDKAJSHDA: " + count);
    }

    public void setUpPrims(List<VertexNode> _verticies, List<Edge> _edges)
    {
        allNodes = _verticies;
        allEdges = _edges;

        foreach (Edge aEdge in _edges)
        {

            if (!vertexTable.ContainsKey(aEdge.getNode0()))
            {
                List<VertexNode> temp = new List<VertexNode>();
                temp.Add(aEdge.getNode1());
                vertexTable.Add(aEdge.getNode0(), temp);
            }
            else
            {
                List<VertexNode> temp = (List<VertexNode>)vertexTable[aEdge.getNode0()];

                if (!temp.Contains(aEdge.getNode1()))
                {
                    temp.Add(aEdge.getNode1());
                    vertexTable[aEdge.getNode0()] = temp;
                }

            }

            if (!vertexTable.ContainsKey(aEdge.getNode1()))
            {
                List<VertexNode> temp = new List<VertexNode>();
                temp.Add(aEdge.getNode0());
                vertexTable.Add(aEdge.getNode1(), temp);
            }
            else
            {
                List<VertexNode> temp = (List<VertexNode>)vertexTable[aEdge.getNode1()];

                if (!temp.Contains(aEdge.getNode0()))
                {
                    temp.Add(aEdge.getNode0());
                    vertexTable[aEdge.getNode1()] = temp;
                }
            }

        }

        startPrims();


        //RNG - keep random edges

        //List<Edge> poolList = new List<Edge>();
        //foreach (Edge edges in allEdges)
        //{
        //    if (!edgesInTree.Contains(edges))
        //    {
        //        poolList.Add(edges);
        //    }
        //}
        //int perc = 0;// (poolList.Count * 10) / 100;
        //for (int i = 0; i < perc; i++)
        //{
        //    int index = Random.Range(0, poolList.Count);
        //    edgesInTree.Add(poolList[index]);
        //    poolList.RemoveAt(index);
        //}
    }

    private void startPrims()
    {
        int count = Random.Range(0, allNodes.Count);

        VertexNode theNode = allNodes[count];
        nodesInTree.Add(theNode);
        findNext();
    }

    private void findNext()
    {

        VertexNode oldNode = null;
        VertexNode closesNode = null;
        float closesDistance = 0;

        foreach (VertexNode aNode1 in nodesInTree)
        {

            List<VertexNode> connectedNodes = (List<VertexNode>)vertexTable[aNode1];

            foreach (VertexNode aNode in connectedNodes)
            {
                if (!nodesInTree.Contains(aNode))
                {
                    //float tempDst = Vector2.Distance(aNode.getParentCell().transform.position, aNode1.getParentCell().transform.position);
                    float tempDst = Vector3.Distance(aNode.getParentCell().transform.position, aNode1.getParentCell().transform.position);
                    if (closesNode != null)
                    {
                        if (tempDst < closesDistance)
                        {
                            closesDistance = tempDst;
                            closesNode = aNode;
                            oldNode = aNode1;
                        }
                    }
                    else
                    {
                        closesNode = aNode;
                        closesDistance = tempDst;
                        oldNode = aNode1;
                    }
                }
            }
        }

        nodesInTree.Add(closesNode);

        foreach (Edge aEdge in allEdges)
        {
            if (aEdge.edgeContainsVertex(oldNode) && aEdge.edgeContainsVertex(closesNode))
            {
                aEdge.setDrawColor(new Color(0, 255, 0, 255));
                edgesInTree.Add(aEdge);
            }
        }

        if (nodesInTree.Count == allNodes.Count)
        {
            return;
        }
        else
        {
            findNext();
        }
    }

    public void stopEdgeDraw()
    {
        GameObject[] allLines = (GameObject[])GameObject.FindGameObjectsWithTag("Lines");

        foreach (GameObject aLine in allLines)
        {
            GameObject.Destroy(aLine);
        }
    }

    public List<Edge> getConnections()
    {
        return edgesInTree;
    }

    public List<Edge> getFinalConnections()
    {
        return finalPrimList;
    }
}
