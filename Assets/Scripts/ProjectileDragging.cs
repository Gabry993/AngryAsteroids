using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Here we have all the logic to aim and fire (the asteroid) and to control the rocket
*/
public class ProjectileDragging : MonoBehaviour {

    public float maxStretch = 0.0f; //max stretch for the slingshot band (exposed to unity)
    public LineRenderer catapultLineFront; //half slingshot sprite
    public LineRenderer catapultLineBack;   //half slingshot sprite. We have them divided so the asteroid can be in the middle for a nice graphic results.

    private SpringJoint2D spring;
    private Transform catapult;
    private Ray rayToMouse; //misleading name. We followed the tutorial which was to drag the asteroid with the mouse, then adapted to our case. This is ray to asteroid position now.
    private Ray leftCatapultToProjectile;
    private float maxStretchSqr;
    private float circleRadius;
    private float speedScale = 1f;
    private float rad = 1.5f;
    private float angle;
    private bool aiming; //true if we are aiming
    private bool charging;  //true if we are charging
    private float bla;
    private Vector3 direction;
    private Vector2 prevVelocity;
    public float aimingMovement;
    private float prevHapkitPosition;
    private int frameToPos = 0;
    private string movingState = "not moving";
    private string previousState = "not moving";

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
        aiming = true;  //we start from aiming
        charging = false;
        aimingMovement = 0.2f;
        bla = 0.2f;
        angle = 4.7123f;//(float)Math.Atan2(transform.position.y - catapult.position.y, transform.position.x - catapult.position.x);
        //we set the asteroid in a sensible initial position
        transform.position = new Vector3(catapult.position.x + (float)Math.Sin(angle) * rad, catapult.position.y + (float)Math.Cos(angle) * rad, 0);
    }
	
	// Update is called once per frame
	void Update () {
        if (Globals.Hapkit) //if we are playing with hapkit
        {
            if (aiming) //and we are aiming
            {
                Aim();  //aim
                if (!charging) //if we are not chargin, moves the asteroid on a circle accordingly to hapkit position. The values are scaled to cover a sensible circunference
                {
                    angle = 4.12f + ((Globals.HapkitPosition + 0.045f) * 34.88f);
                    transform.position = new Vector3(catapult.position.x + (float)Math.Sin(angle) * rad, catapult.position.y + (float)Math.Cos(angle) * rad, 0);
                }
                if (charging) //if we are charging, move the asteroid on the line from its current pos and the "center" of the catapult
                {
                    // TODO make sure that catapult-transform.pos are never 0. Use clamp or something
                    //if (Vector3.Distance(catapult.position,transform.position)> aimingMovement + 0.01)
                    transform.position = catapult.position + (this.direction) * 35f*(Globals.HapkitPosition-0.045f);
                }
                Dragging();
                if (Input.GetKeyDown("c")) //if we press c we enter charging state
                {
                    //Globals.HapkitState = 1;
                    charging = true;
                    direction = Vector3.Normalize(catapult.position - transform.position);
                    SerialInputManager.SetState(1); //set charging state on the hapkit
                }
                if (Input.GetKeyDown("f") && !Globals.ReleaseToFire) //if release to fire is disabled,press f to fire
                    Fire();
                if (charging && Globals.ReleaseToFire && frameToPos>=150) //if we release to fire is enable, try to detect the event of releasing the hapkit handle.
                //frameToPos is used to let the asteroid move when we go into charging mode (thus enabling the spring) without triggering a wrong release event.
                {
                    float pos_avg = 0f;
                    float acc_avg = 0f;
                    float vel_avg = 0f;
                    foreach (var triple in Globals.ToFireQueue)
                    {
                        pos_avg += triple.position;
                        acc_avg += triple.acceleration* triple.acceleration;
                        vel_avg += triple.velocity*triple.velocity;
                    }
                    pos_avg /= (float)Globals.ToFireQueue.Count;
                    acc_avg = (float)Math.Sqrt(acc_avg) / (float)Globals.ToFireQueue.Count;
                    vel_avg = (float)Math.Sqrt(vel_avg) / (float)Globals.ToFireQueue.Count;
                    //print(pos_avg);
                   // print(acc_avg);
                    /*
                        that part was just for debugging. It's nice as it realizes in real time with perfect precision wheter tha hapkit is moving or not.
                        Well, we are looking at the speed,so it'spretty easy, but the responsiveness of the thing is quite good.
                    */
                    if (vel_avg < 0.001)
                    {
                        movingState = "not moving";
                    }
                    else
                        movingState = "moving";
                    //print(movingState + " " + vel_avg);
                    //print(acc_avg);
                    /*
                        we simply look at the square avg of the acceleration to detect the release event.
                        It's important to detect it as soon as it happens, as we want to fire exploining the full power of the spring.
                        If we detect the event too late, the force applied to the asteroid by the spring would be less then expected.
                        Sometimes this misbehaves, but it's only a simple try which works well enough most of the time and was really appreciate during live demos.
                    */
                    if (acc_avg > 0.15)
                    {
                        //print("released");
                        Fire();
                    }
                    else
                        print("hold");
                    previousState = movingState;
                }
                else if (charging && Globals.ReleaseToFire && frameToPos < 200)
                {
                    frameToPos++;
                    print(frameToPos);
                }
                /*if (charging)
                {
                    float pos_avg = 0f;
                    float acc_avg = 0f;
                    foreach (var couple in Globals.ToFireQueue)
                    {
                        pos_avg += couple.position;
                        acc_avg += couple.acceleration;
                    }
                    pos_avg /= (float)Globals.ToFireQueue.Count;
                    acc_avg /= (float)Globals.ToFireQueue.Count;
                    //print(pos_avg);
                    print(acc_avg);
                    if (acc_avg > -0.4f)
                    {
                        Fire();
                    }
                }*/
            }
        }
        else {  //without hapkit the whole thing is keyboard driven. We will not comment this as is not relevant.
            if (aiming)
            {
                Aim();
                if (Input.GetKeyDown("i") && !charging)
                {
                    angle += 0.1f * speedScale;
                    transform.position = new Vector3(catapult.position.x + (float)Math.Sin(angle) * rad, catapult.position.y + (float)Math.Cos(angle) * rad, 0);
                    /*
                    //print(Globals.HapkitPosition);
                    angle = 4.12f + (Globals.HapkitPosition*34.88f);
                    transform.position = new Vector3(catapult.position.x + (float)Math.Sin(angle) * rad, catapult.position.y + (float) Math.Cos(angle) * rad, 0);
                    prevHapkitPosition = Globals.HapkitPosition;
                    //transform.position = transform.position + new Vector3(0.0f, aimingMovement, 0.0f);
                */
                }
                if (Input.GetKeyDown("k") && !charging)
                {
                    angle -= 0.1f * speedScale;
                    transform.position = new Vector3(catapult.position.x + (float)Math.Sin(angle) * rad, catapult.position.y + (float)Math.Cos(angle) * rad, 0);
                    transform.position = transform.position + new Vector3(0.0f, -aimingMovement, 0.0f); ;
                }
                /*

                if (1<0 && !charging)
                {
                    angle -= Globals.HapkitPosition * speedScale;
                    transform.position = new Vector3(catapult.position.x + (float)Math.Sin(angle) * rad, catapult.position.y + (float)Math.Cos(angle) * rad, 0);
                    transform.position = transform.position + new Vector3(0.0f, -aimingMovement, 0.0f); ;
                }*/
                /*
                if (charging)
                    if ((transform.position - catapult.position).sqrMagnitude < maxStretchSqr)
                    {
                        //transform.position = catapult.position + Vector3.Normalize(transform.position - catapult.position) * (Globals.HapkitPosition + 0.045f);

                             transform.position = transform.position + new Vector3(catapult.position.x - Globals.HapkitPosition + 0.045f, 0.0f, 0.0f);
                    }
                      */

                /*
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
                }*/
                //print(aimingMovement);      
                if (Input.GetKeyDown("j") && charging)
                {
                    //aimingMovement -= 0.1f;
                    //if (aimingMovement * aimingMovement < maxStretchSqr)  
                    //Debug.Log(catapult.position);
                    //Debug.Log(transform.position);
                    //Debug.Log(transform.position - catapult.position);


                    // TODO make sure that catapult-transform.pos are never 0. Use clamp or something
                    //if (Vector3.Distance(catapult.position,transform.position)> aimingMovement + 0.01)
                    transform.position = transform.position + (this.direction) * -(aimingMovement);


                }
                if (Input.GetKeyDown("l") && charging)
                {
                    // aimingMovement += 0.1f;
                    // if (aimingMovement * aimingMovement < maxStretchSqr)
                    //transform.position = catapult.position + Vector3.Normalize(transform.position - catapult.position) * (aimingMovement);
                    // if (Vector3.Distance(catapult.position, transform.position) > aimingMovement + 0.01)
                    transform.position = transform.position + (this.direction) * (aimingMovement);

                }
                //transform.position = transform.position + new Vector3(aimingMovement, 0.0f, 0.0f);


                //   if ((transform.position - catapult.position).sqrMagnitude < maxStretchSqr)//to avoid moving on the circle while charging at the max




                Dragging();
                if (Input.GetKeyDown("c"))
                {
                    //Globals.HapkitState = 1;
                    charging = true;
                    direction = Vector3.Normalize(catapult.position - transform.position);
                }
                if (Input.GetKeyDown("f"))
                    Fire();
            }
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

    private void Aim() //while aiming we must be sure that the spring is disabled
    {
        spring.enabled = false;
        aiming = true;
    }

    private void Fire()
    {
        SerialInputManager.SetState(0);
        spring.enabled = true;
        GetComponent<Rigidbody2D>().isKinematic = false;
        aiming = false;
    }
    void Dragging() //keeps updating the asteroid position
    {
        Vector3 asteroidPosition = transform.position;//Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 catapultToMouse = asteroidPosition - catapult.position;
        if (catapultToMouse.sqrMagnitude > maxStretchSqr)
        {
            //rayToMouse.direction = catapultToMouse;
            //asteroidPosition = rayToMouse.GetPoint(maxStretch);
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
