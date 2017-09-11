using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof (PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity {


    public float movementSpeed = 5;
    PlayerController controller;
    Camera cam;
    GunController gunController;

	// Use this for initialization
	protected override void Start ()
    {
        base.Start();   
        controller = this.GetComponent<PlayerController>();
        cam = Camera.main;
        gunController = GetComponent<GunController>();
    }
	
	// Update is called once per frame
	void Update ()
    {

        //movement
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * movementSpeed;
        controller.Move(moveVelocity);


        //lookinput
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Debug.DrawLine(ray.origin, point, Color.red);

            controller.LookAt(point);
        }


        //weapon input
        if (Input.GetMouseButton(0))
        {
            gunController.Shoot();
        }
	}
}
