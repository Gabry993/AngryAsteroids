using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
    We use this class to reset the game, in 3 case:
    1- The user press "R"
    2- The projectile (asteroid or rocket) goes out of boundaries
    3- The rocket explodes/ the asteroid stops moving for a while (after it has been shot)
*/
public class Resetter : MonoBehaviour {

    public Rigidbody2D projectile;
    public Rigidbody2D asteroid;
    public Rigidbody2D rocket;

    public float resetSpeed = 0.025f;

    private float resetSpeedSqr;
    private SpringJoint2D spring; //when we shoot the asteroid, the spring object is destroyed. We check that to see if the asteroid was shot
    private int frameAfterExplosion = 0; //counter to keep the game running enough to see the explosion animation before resetting

    // Use this for initialization
    void Start () {
        resetSpeedSqr = resetSpeed * resetSpeed;
        spring = projectile.GetComponent<SpringJoint2D>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.R)) //press R to reset
        {
            Reset();
        }
        if (Globals.Mode == 0) //if we are in mode 0 (default), the asteroid is the projectile
        {
            projectile = asteroid;
        }
        if (Globals.Mode == 1) //if we are in mode 1, the rocket is the projectile
        {
            projectile = rocket;
        }
        if (Globals.Mode == 0 && spring == null && projectile.velocity.sqrMagnitude < resetSpeedSqr) //in mode 0, reset when the speed of the asteroid is too low after it has been shot (so, the spring object is null)
        {
            Reset();
        }
        if (Globals.Mode == 1 && Globals.Exploded) //if the rocket is exploded, we count (thus wait) 200 frames to see the animation and 70 before stopping the vibration on the Hapkit
        {
            frameAfterExplosion++;
           
            if (frameAfterExplosion == 70)
                SerialInputManager.SetState(0);
            if (frameAfterExplosion > 200)
                Reset();
        }
    }

    /*
        If the projectile exits the game boundaries, we reset
    */
    private void OnTriggerExit2D(Collider2D other)
    {
       if (other.GetComponent<Rigidbody2D>() == projectile)
        {
            Reset();
        } 
    }
    private void Reset()
    {

        //Application.LoadLevel(Application.loadedLevel); deprecated
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
}
