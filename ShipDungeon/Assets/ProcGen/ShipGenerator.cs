using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipGenerator : MonoBehaviour {


    public int sizeX;
    public int sizeY;

    private int currentX;
    private int currentY;

    public int genSpacing;

    public GameObject Void;
    public GameObject Floor;

    public int minRoomSize;
    public int maxRoomSize;

    public float waitTime;

    
    class Room
    {
        public int x, y;
        public string type;

        public Room(int deltaX, int deltaY, string deltaType)
        {
            x = deltaX;
            y = deltaY;
            type = deltaType;
        }
    }

    List<Room> rooms = new List<Room>();

    // Use this for initialization
    void Start () {
        MakeShip();
       
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("NEW MAP!!!");
            GameObject[] planes = GameObject.FindGameObjectsWithTag("Floor");
            foreach(var x in planes)
            {
                Destroy(x);
            }
            MakeShip();            
        }
    }

    public void MakeShip()
    {
        //create base ship
        CreateBase();

        //spawn parts
        AddRooms();

        CreateRooms();
        CreateRooms();
        CreateRooms();
    }

    public void CreateBase()
    {
        Room newRoom = new Room(0, 0, "");

        for (int Y = 0; Y < sizeY; Y++)
        {
            for (int X = 0; X < sizeX; X++)
            {
                newRoom = new Room(X * genSpacing, Y * genSpacing, "Void");
                rooms.Add(newRoom);
            }
        }
    }


    public void CreateRooms()
    {
        //add a random room of size

        //get a position between 0 and max room size - maxRoomSize
        int posX = Random.Range(0, sizeX - maxRoomSize);
        int posY = Random.Range(0, sizeY - maxRoomSize);

        int roomSizeX = Random.Range(minRoomSize, maxRoomSize);
        int roomSizeY = Random.Range(minRoomSize, maxRoomSize);

        for (int Y = posY; Y < posY + roomSizeY; Y++)
        {
            for (int X = posX; X < posX + roomSizeX; X++)
            {
                // rooms.FindIndex(x => x.x == X && x.y == Y);
                Instantiate(Floor, new Vector3(X * genSpacing, 0.1f, Y * genSpacing), Quaternion.identity);
            }
        }


    }



    //public void AddRooms(Vector2 _pos, string _roomType)
    public void AddRooms()
    {
        //StartCoroutine(placeRooms());
        //Debug.Log("pos: " + _pos + ", Type: " + _roomType);
        //StartCoroutine(placeRoom(_pos));

        foreach (var room in rooms)
        {
            //yield return new WaitForSeconds(waitTime);
            if (room.type == "Void")
            {
                Instantiate(Void, new Vector3(room.x, 0, room.y), Quaternion.identity);
            }
            else if (room.type == "Room")
            {
                Instantiate(Floor, new Vector3(room.x, 0, room.y), Quaternion.identity);
            }
        }
    }

    IEnumerator placeRooms()
    {
        foreach (var room in rooms)
        {
            yield return new WaitForSeconds(waitTime);
            if (room.type == "Void")
            {
                Instantiate(Void, new Vector3(room.x, 0, room.y), Quaternion.identity);
            }
            else if (room.type == "Room")
            {
                Instantiate(Floor, new Vector3(room.x, 0, room.y), Quaternion.identity);
            }
        }
    }



    //Vector2 pos = new Vector2(0, 0);
    //foreach (var room in rooms)
    //{
    //    pos.x = room.x;
    //    pos.y = room.y;
    //    //AddRoom(pos, room.type);
    //    StartCoroutine(placeRoom(pos));
    //}
}
