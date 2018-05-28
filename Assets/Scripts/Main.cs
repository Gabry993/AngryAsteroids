using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

    public GameObject asteroid;
    public GameObject rocket;
    public GameObject catapult;
    public static int mode; //0 is catapult, 1 is rocket

    // Use this for initialization
    void Start () {
        Globals.Mode = 0;
        //Globals.HapkitState = 0;
        SerialInputManager.SetState(0);
        Globals.Exploded = false;
        rocket.SetActive(false);
        Globals.Hapkit = true;
        Globals.ReleaseToFire = false;
	}
	    
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("n")){
            
            //Globals.HapkitState = 1;

        }

        if (Input.GetKeyDown("p"))
        {
            Globals.Mode = 1;
            //Globals.HapkitState = 2;
            SerialInputManager.SetState(2);
            asteroid.SetActive(false);
            catapult.SetActive(false);
            rocket.SetActive(true);
        }
        if (Input.GetKeyDown("o"))
        {
            Globals.Mode = 0;
            //Globals.HapkitState = 0;
            SerialInputManager.SetState(0);
            asteroid.SetActive(true);
            catapult.SetActive(true);
            rocket.SetActive(false);
        }

    }
}
