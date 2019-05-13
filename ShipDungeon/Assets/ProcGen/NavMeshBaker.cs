using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour {

    //USE THIS SCRIPT LATER WHEN YOU MAY NEED SEVERAL NAV MESH COMPONENTS!!!


    //public List<NavMeshSurface> surfaces;

    //public void initialise()
    //{
    //    surfaces = new List<NavMeshSurface>();
    //}

    //public void addToSurfaceList(NavMeshSurface surface)
    //{
    //    surfaces.Add(surface);
    //}

    public void BakeNavMesh(NavMeshSurface surface)
    {
        surface.BuildNavMesh();
    }


}
