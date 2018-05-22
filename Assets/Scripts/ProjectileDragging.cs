using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDragging : MonoBehaviour {

    public float maxStretch = 3.0f;
    public LineRenderer catapultLineFront;
    public LineRenderer catapultLineBack;

    private SpringJoint2D spring;
    private Transform catapult;
    private Ray rayToMouse;
    private Ray leftCatapultToProjectile;
    private float maxStretchSqr;
    private float circleRadius;
    private float speedScale = 1f;
    private float rad = 1.5f;
    private float angle;
    private bool aiming;
    private bool charging;
    private Vector2 prevVelocity;
    public float aimingMovement;
    private float prevHapkitPosition;

    private void Awake()
    {
        spring = GetComponent<SpringJoint2D>();
        catapult = spring.connectedBody.transform;
    }
    // Use this for initialization
    void Start () {
        LineRendererSetup();
        rayToMouse = new Ray(catapult.position, Vector3.zero);
        leftCatapultToProjectile = new Ray(catapultLineFront.transform.position, Vector3.zero);
        maxStretchSqr = maxStretch * maxStretch;
        CircleCollider2D circle = GetComponent<Collider2D>() as CircleCollider2D;
        circleRadius = circle.radius;
        aiming = true;
        charging = false;
        aimingMovement = 0.2f;
        angle = 4.7123f;//(float)Math.Atan2(transform.position.y - catapult.position.y, transform.position.x - catapult.position.x);
        transform.position = new Vector3(catapult.position.x + (float)Math.Sin(angle) * rad, catapult.position.y + (float)Math.Cos(angle) * rad, 0);
    }
	
	// Update is called once per frame
	void Update () {
        if (aiming)
        {
            Aim();
            if (!charging)
            {
                //print(Globals.HapkitPosition);
                angle = 3f + (Globals.HapkitPosition*17.84f);
                transform.position = new Vector3(catapult.position.x + (float)Math.Sin(angle) * rad, catapult.position.y + (float) Math.Cos(angle) * rad, 0);
                prevHapkitPosition = Globals.HapkitPosition;
                //transform.position = transform.position + new Vector3(0.0f, aimingMovement, 0.0f);
            }
            /*
            if (1<0 && !charging)
            {
                angle -= Globals.HapkitPosition * speedScale;
                transform.position = new Vector3(catapult.position.x + (float)Math.Sin(angle) * rad, catapult.position.y + (float)Math.Cos(angle) * rad, 0);
                transform.position = transform.position + new Vector3(0.0f, -aimingMovement, 0.0f); ;
            }*/
                
            if (charging)
                if (prevHapkitPosition< Globals.HapkitPosition && (transform.position - catapult.position).sqrMagnitude < maxStretchSqr)//to avoid moving on the circle while charging at the max
                {
                    transform.position = transform.position + new Vector3(-aimingMovement, 0.0f, 0.0f);
                    prevHapkitPosition = Globals.HapkitPosition;
                }
            if (charging)
                if (prevHapkitPosition > Globals.HapkitPosition)//to avoid moving on the circle while charging at the max
                {
                    transform.position = transform.position + new Vector3(aimingMovement, 0.0f, 0.0f);
                    prevHapkitPosition = Globals.HapkitPosition;
                }
            Dragging();
            if (Input.GetKeyDown("c"))
                charging = true;
            if (Input.GetKeyDown("f"))
                Fire();
           
        }
        if (spring != null)
        {
            if (!GetComponent<Rigidbody2D>().isKinematic && prevVelocity.sqrMagnitude > GetComponent<Rigidbody2D>().velocity.sqrMagnitude)
            {
                Destroy(spring);
                GetComponent<Rigidbody2D>().velocity = prevVelocity;
            }
            if (!aiming)
            { prevVelocity = GetComponent<Rigidbody2D>().velocity; }

            LineRendererUpdate();
        }
        else
        {
            catapultLineFront.enabled = false;
            catapultLineBack.enabled = false;
        }

	}

    void LineRendererSetup()
    {
        catapultLineFront.SetPosition(0, catapultLineFront.transform.position);
        catapultLineBack.SetPosition(0, catapultLineBack.transform.position);

        catapultLineFront.sortingLayerName = "ground";
        catapultLineBack.sortingLayerName = "ground";

        catapultLineFront.sortingOrder = 3;
        catapultLineBack.sortingOrder = 1;
    }

    private void Aim()
    {
        spring.enabled = false;
        aiming = true;
    }

    private void Fire()
    {
        spring.enabled = true;
        GetComponent<Rigidbody2D>().isKinematic = false;
        aiming = false;
    }
    void Dragging()
    {
        Vector3 asteroidPosition = transform.position;//Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 catapultToMouse = asteroidPosition - catapult.position;
        if (catapultToMouse.sqrMagnitude > maxStretchSqr)
        {
            rayToMouse.direction = catapultToMouse;
            asteroidPosition = rayToMouse.GetPoint(maxStretch);
        }

        asteroidPosition.z = 0f;
        transform.position = asteroidPosition;
    }

    void LineRendererUpdate()
    {
        Vector2 catapultToProjectile = transform.position - catapultLineFront.transform.position;
        leftCatapultToProjectile.direction = catapultToProjectile;
        Vector3 holdPoint = leftCatapultToProjectile.GetPoint(catapultToProjectile.magnitude + circleRadius);
        catapultLineFront.SetPosition(1, holdPoint);
        catapultLineBack.SetPosition(1, holdPoint);
    }
}
