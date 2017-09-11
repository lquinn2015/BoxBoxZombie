using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {


    Rigidbody myRigidbody;
    Vector3 velocity;


	// Use this for initialization
	void Start () {
        myRigidbody = this.GetComponent<Rigidbody>();
	}

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;

    }

    public void LookAt(Vector3 point)
    {
        Vector3 levelPlanePoint = new Vector3(point.x, transform.position.y, point.z);
        transform.LookAt(levelPlanePoint);
    }

    void FixedUpdate()
    {
        myRigidbody.MovePosition(myRigidbody.position + velocity*Time.fixedDeltaTime);
    }
}
