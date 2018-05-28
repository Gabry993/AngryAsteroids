using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFollow : MonoBehaviour {

    public Transform projectile;
    public Transform asteroid;
    public Transform rocket;
    public Transform farLeft;
    public Transform farRight;
    public Transform farDown;
    public Transform farUp;
    public Transform secondCameraPlane;


    // Update is called once per frame
    void LateUpdate()
    {
        secondCameraPlane.position = new Vector3(transform.position.x + 14, transform.position.y + 6.5f, 0);
    }
    void Update () {
        //
        if (projectile == null)
            return;
        if (Globals.Mode == 1)
            projectile = rocket;
        if (Globals.Mode == 0)
            projectile = asteroid;
        Vector3 newPosition = transform.position;
        newPosition.x = projectile.position.x;
        if (Globals.Mode == 1)
        {
            newPosition.y = projectile.position.y;
            newPosition.y = Mathf.Clamp(newPosition.y, farDown.position.y, farUp.position.y);
        }
        newPosition.x = Mathf.Clamp(newPosition.x, farLeft.position.x, farRight.position.x);
        
        transform.position = newPosition;
	}
}
