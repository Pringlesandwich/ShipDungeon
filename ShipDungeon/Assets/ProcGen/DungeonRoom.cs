using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : MonoBehaviour {

    GameObject[] GO;
    //public int strength;

    //public bool isColliding;

    public int ID = 0;

    public int sizeX = 0;
    public int sizeZ = 0;

    public bool isMain = false;

    public bool edgesComplete = false;

    public List<GameObject> branches = new List<GameObject>();

    //hold locked door data

    //hold key data

    //add a branch from ConvertToLayout, this will allow corridor creation as well as locked doors potentially
    public void AddBranch(GameObject g)
    {
        branches.Add(g);
    }




	// Use this for initialization
	void Start () {
        //GO = GameObject.FindGameObjectsWithTag("Gen");  
        //var a = this.GetComponent<BoxCollider>();




    }
	
    public void AddTitles(int x, int z)
    {



        

    }

    public void CreateLines()
    {
        GO = GameObject.FindGameObjectsWithTag("Gen");
        foreach(var g in GO)
        {
            if(g != this.gameObject)
            {

            }
        }

    }


	// Update is called once per frame
	//void Update () {
 //       //foreach (GameObject a in GO)
 //       //{
 //       //    if (a != this)
 //       //    {
 //       //        if (this.GetComponent<MeshCollider>().bounds.Intersects(a.GetComponent<MeshCollider>().bounds))
 //       //        {
 //       //            Vector3 direction = transform.position - a.transform.position;
 //       //            direction.Normalize();
 //       //            transform.position = new Vector3(Mathf.Round(transform.position.x + (direction.x * strength)), transform.position.y, Mathf.Round(transform.position.z + (direction.z * strength)));

 //       //        }
 //       //    }
 //       //}
 //   }




}
