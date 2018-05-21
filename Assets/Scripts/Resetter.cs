using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Resetter : MonoBehaviour {

    public Rigidbody2D projectile;
    public Rigidbody2D asteroid;
    public Rigidbody2D rocket;

    public float resetSpeed = 0.025f;

    private float resetSpeedSqr;
    private SpringJoint2D spring;
    private int frameAfterExplosion = 0;

    // Use this for initialization
    void Start () {
        resetSpeedSqr = resetSpeed * resetSpeed;
        spring = projectile.GetComponent<SpringJoint2D>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
        if (Globals.Mode == 0)
        {
            projectile = asteroid;
        }
        if (Globals.Mode == 1)
        {
            projectile = rocket;
        }
        if (Globals.Mode == 0 && spring == null && projectile.velocity.sqrMagnitude < resetSpeedSqr)
        {
            Reset();
        }
        if (Globals.Mode == 1 && Globals.Exploded)
        {
            frameAfterExplosion++;
            if (frameAfterExplosion > 500)
                Reset();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
       if (other.GetComponent<Rigidbody2D>() == projectile)
        {
            Reset();
        } 
    }
    private void Reset()
    {

        //Application.LoadLevel(Application.loadedLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
}
