using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FPSGameManager : MonoBehaviour {

    private bool run = false;
    public float runTimer;
    public PlayerController pc;
    public Text countdown;

	// Use this for initialization
	void Start () {
        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {
		

        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("TestScene");
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SceneManager.LoadScene("TestScene");
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (run)
        {
            runTimer -= Time.deltaTime;
            float displayTime = runTimer;
            int fuck = Mathf.RoundToInt(displayTime);
            countdown.text = "RUN BACK TO START! " + fuck;
            if (runTimer < 0)
            {
                pc.Kill();
                runTimer = 0;
            }
        }



    }

    public void Run()
    {
        //start timer
        run = true;


    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && run == true)
        {

            run = false;

            countdown.text = "YOU A WINNER!!!!!!!!!!!!!!!!!!!!!!!!";

        }
        else
        {
           
        }
    }



}
