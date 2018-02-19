using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretRotation : MonoBehaviour {

    private Rigidbody rb;
    private LineRenderer line;
    public float camRayLength = 200f;
    public int floorMask;

    //public Transform fireTransform;

    // Use this for initialization
    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>();
        floorMask = LayerMask.GetMask("Floor");
    }
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        Rotate();
	}

    void Rotate()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit floorHit;

        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
        {
            Vector3 playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0f;

            Quaternion newRotation = Quaternion.LookRotation(playerToMouse.normalized);
            rb.MoveRotation(newRotation);

            line.SetPosition(0, this.transform.position);
            line.SetPosition(1, floorHit.point);
            
        }
        
        //fireTransform.rotation = Quaternion.Euler(new Vector3(fireTransform.rotation.x, this.transform.rotation.y, fireTransform.transform.rotation.z));

    }
}
