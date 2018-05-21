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
        Globals.Exploded = false;
        rocket.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("p"))
        {
            Globals.Mode = 1;
            asteroid.SetActive(false);
            catapult.SetActive(false);
            rocket.SetActive(true);
        }
        if (Input.GetKeyDown("o"))
        {
            Globals.Mode = 0;
            asteroid.SetActive(true);
            catapult.SetActive(true);
            rocket.SetActive(false);
        }

    }
}
