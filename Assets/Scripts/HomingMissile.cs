using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour {

    public Transform target;

    public float speed = 0f;
    public float rotateSpeed = 0f;

    public GameObject explosionEffect;

    private Rigidbody2D rb;

    public float Power;
    public float Radius;

    private float prevHapkitPosition;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
    }

    /*
	// Update is called once per frame
	void Update () {
        if (Globals.HapkitPosition>= 0f)
        {
            //print(Globals.HapkitPosition - 0.1f);
            rotateSpeed = 500f* (Globals.HapkitPosition);
            prevHapkitPosition = Globals.HapkitPosition;
        }
        if (Globals.HapkitPosition< 0f)
        {
            //print(Globals.HapkitPosition - 0.1f);
            rotateSpeed = 500f* (Globals.HapkitPosition );
            prevHapkitPosition = Globals.HapkitPosition;
        }
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
    */
    void Update()
    {
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
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag != "EditorOnly")
        {
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
