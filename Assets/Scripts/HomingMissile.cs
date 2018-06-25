using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour {

    public Transform target;    //useless, can be removed safely

    public float speed = 0f;    //linear speed of the rocket
    public float rotateSpeed = 0f;  //rotation speed of the rocket

    public GameObject explosionEffect;  //input for unity: explosion object

    private Rigidbody2D rb; //rigid body of the rocket

    public float Power;     //power of the explosion
    public float Radius;    //radius of the explosion

    //private float prevHapkitPosition;   //last Hapkit position, used to detect changes in direction -- NOT NEEDED ANYMORE

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
    }


	// Update is called once per frame
	void Update () {
        //if we are playing with Hapkit
        if (Globals.Hapkit) {
            /*
                Change rotation speed according to HapkitPosition: the farther from the center, the quicker it rotates
            */
            if (Globals.HapkitPosition>= 0f || Globals.HapkitPosition< 0f)
            {
                rotateSpeed = 500f* (Globals.HapkitPosition);
                //prevHapkitPosition = Globals.HapkitPosition;
            }
            /*
                Shoot the rocket!
            */
            if (Input.GetKeyDown("up"))
            {
                rb.isKinematic = false;
                speed = 30f;
                rb.velocity = transform.right * speed;
            }
            /*
                Turn off rocket engine
            */
            if (Input.GetKeyDown("down"))
            {
                rotateSpeed = 0f;
                speed = 0f;
            }
            /*
                If speed is not 0, then move the rocket object
            */
            float rotateAmount = 5f;
            rb.angularVelocity = rotateAmount * rotateSpeed;
            if (speed > 0f)
                rb.velocity = transform.right * speed;
            else
                return;
        }
        else { //we are playing with the keyboard only
            if (Input.GetKeyDown("a"))
                rotateSpeed += 25f;
            if (Input.GetKeyDown("d"))
                rotateSpeed -= 25f;
            if (Input.GetKeyDown("up"))
            {
                rb.isKinematic = false;
                speed = 10f;
                rb.velocity = transform.right * speed;
            }
            if (Input.GetKeyDown("down"))
            {
                rotateSpeed = 0f;
                speed = 0f;
            }
            float rotateAmount = 2f;
            rb.angularVelocity = rotateAmount * rotateSpeed;
            if (speed > 0f)
                rb.velocity = transform.right * speed;
            else
                return;
            }
    }
   

    /*
        Method to explode!
    */
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag != "GameController") //otherwise we would explode immediately as we are inside the game border collider
        {
            SerialInputManager.SetState(3); //Change the state: Alert the hapkit that we want the vibration for the explosion
            /*
                There is not an easy/default way for unity to apply explosion force to 2d object.
                This is how we do it, using AddExplosion Force to add the force to all the object inside Radius
            */
            Vector3 explosionPos = transform.position;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, Radius);
            foreach (Collider2D hit in colliders)
            {
                Rigidbody2D rib = hit.GetComponent<Rigidbody2D>();
                if (rib != null)
                {
                    AddExplosionForce(rib, Power * 100, explosionPos, Radius);
                }
            }
            Instantiate(explosionEffect, transform.position, transform.rotation);
            Destroy(gameObject);
            Globals.Exploded = true;
        }
        else
            return;
    }

    /*
        Useful method to add explosion force to a 2d rigid body
    */
    public static void AddExplosionForce(Rigidbody2D body, float expForce, Vector3 expPosition, float expRadius)
    {
        var dir = (body.transform.position - expPosition);
        float calc = 1 - (dir.magnitude / expRadius);
        if (calc <= 0)
        {
            calc = 0;
        }
        
        body.AddForce(dir.normalized * expForce * calc);
    }
}
