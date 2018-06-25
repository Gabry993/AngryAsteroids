using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Script to make the camera follow the projectile
    For the asteroid we only move the camera horizontally, for the rocket also vertically
*/
public class ProjectileFollow : MonoBehaviour {

    public Transform projectile;
    public Transform asteroid;
    public Transform rocket;
    public Transform farLeft;
    public Transform farRight;
    public Transform farDown;
    public Transform farUp;
    public Transform secondCameraPlane;


    /*
        I'm not sure about when this thing gets called, but it works to keep the secondary camera in the right position
    */
    void LateUpdate()
    {
        secondCameraPlane.position = new Vector3(transform.position.x + 10, transform.position.y + 6.5f, 0);
    }

    void Update () {
        if (projectile == null)
            return;
        if (Globals.Mode == 1)  //rocket mode
            projectile = rocket;
        if (Globals.Mode == 0)  //asteroid mode
            projectile = asteroid;
        Vector3 newPosition = transform.position;
        newPosition.x = projectile.position.x;  //in both mode we move the camera on the x axis (horizontally)

        //in rocket mode we move the camera also on the y axis (vertically)
        if (Globals.Mode == 1)
        {
            newPosition.y = projectile.position.y;
            newPosition.y = Mathf.Clamp(newPosition.y, farDown.position.y, farUp.position.y);   //to stop the camera and prevent it from going outside game boundaries
        }
        newPosition.x = Mathf.Clamp(newPosition.x, farLeft.position.x, farRight.position.x);    //to stop the camera and prevent it from going outside game boundaries
        
        transform.position = newPosition;   //set the new position for the camera
	}
}
