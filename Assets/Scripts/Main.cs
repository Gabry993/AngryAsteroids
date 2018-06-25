using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Main class. Herewe control the mode of the game: can be asteroid/slingshot or rocket
*/
public class Main : MonoBehaviour {

    public GameObject asteroid;
    public GameObject rocket;
    public GameObject catapult;
    public static int mode; //0 is asteroid, 1 is rocket

    // Use this for initialization
    void Start () {
        Globals.Mode = 0;   //default game is asteroid and slingshot
        //Globals.HapkitState = 0;
        SerialInputManager.SetState(0); //needed to set the state on the hapkit
        Globals.Exploded = false;
        rocket.SetActive(false);
        Globals.Hapkit = true;  //here we decide if we are playing with hapkit. Maybe we could expose that to unity
        Globals.ReleaseToFire = true;   //here we decide if we are using release to fire function. Maybe we could expose that to unity
	}
	    
	// Update is called once per frame
	void Update () {
        /*
            Press p to enter rocket mode
        */
        if (Input.GetKeyDown("p"))
        {
            Globals.Mode = 1;
            //Globals.HapkitState = 2;
            SerialInputManager.SetState(2); //needed to set the state on the hapkit
            asteroid.SetActive(false);
            catapult.SetActive(false);
            rocket.SetActive(true);
        }
        /*
            Press o to enter asteroid mode
        */
        if (Input.GetKeyDown("o"))
        {
            Globals.Mode = 0;
            //Globals.HapkitState = 0;
            SerialInputManager.SetState(0); //needed to set the state on the hapkit
            asteroid.SetActive(true);
            catapult.SetActive(true);
            rocket.SetActive(false);
        }

    }
}
