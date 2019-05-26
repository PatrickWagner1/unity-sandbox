using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonControls : MonoBehaviour {

    // Basic behaviour variables
    public float movementSpeed = 1F;
    public float rotationSpeed = 10F;

    // Max Rotation Values
    public float minX = -360F, maxX = 360F, minY = -90, maxY = 90;
    public float xRot = 0, yRot = 0;
    public bool useGravity = false;

    private GameObject physicalParent;
    private Rigidbody physicalParentBody;

    // Use this for initialization
    void Start () {
        // Set gravity vector
        Physics.gravity = new Vector3(0, -9.81F, 0);

        // Get the physical body of the camera which is represented by a capsule
        physicalParent = this.transform.parent.gameObject;

        // Give the capsule a rigidbody for the appliance of phyiscs
        physicalParentBody = physicalParent.AddComponent<Rigidbody>();

        // Standard mode without gravity
        physicalParentBody.useGravity = false;

        // Switch of wind
        physicalParentBody.angularDrag = 0;

        // Stop physics in x and z direction for the camera's body so  there isnt any unwanted movement except for gravity
        physicalParentBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Lock the cursor in the middle of the screen to achieve a fps handling
        if (Cursor.lockState != CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.Locked;

    }
	
	// Update is called once per frame
	void Update () {
        MouseController();
        KeyboardController();
	}

    void MouseController()
    {

        // Clamp is used tp apply limits to either side of the rotation's value
        // Save the mouse movements of the single axis into variables
        xRot += Mathf.Clamp(rotationSpeed * Input.GetAxis("Mouse X"), minX, maxX);
        yRot -= Mathf.Clamp(rotationSpeed * Input.GetAxis("Mouse Y"), minY, maxY);

        yRot = Mathf.Clamp(yRot, -90, 90);

        // Get a euler quanternion object, which saves a transformation without the z-axis.
        // This is used so the camera doesnt start to tilt and stays in some form parallel to the ground like in common fps games.
        Quaternion targetRot = Quaternion.Euler(new Vector3(yRot, xRot, 0.0F));

        // Apply the tranformation with the euler object. 
        transform.rotation = physicalParent.transform.rotation = targetRot; 
        
        //Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime);  <-- alternative rotation. not so direct but smoother.

    }

    // Function handeling different Keyevents.
    void KeyboardController()
    {
        // Free the mouse for debugging
        if(Input.GetKey(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;

        // Standard movement keys and the movement applied when pressing them
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            physicalParent.transform.localPosition += transform.right * Time.deltaTime * movementSpeed;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            physicalParent.transform.localPosition -= transform.right * Time.deltaTime * movementSpeed;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            physicalParent.transform.localPosition += transform.forward * Time.deltaTime * movementSpeed;

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            physicalParent.transform.localPosition -= transform.forward * Time.deltaTime * movementSpeed;

        // Gravity toggle for a different experience
        if (Input.GetKeyDown(KeyCode.G))
            ToggleGravity();

        // Sink or elevate vertically to the floor for a minecraft style movement
        if (Input.GetKey(KeyCode.Space))
            physicalParent.transform.localPosition += new Vector3(0, 1, 0) * Time.deltaTime * movementSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
            physicalParent.transform.localPosition -= new Vector3(0, 1, 0) * Time.deltaTime * movementSpeed;
    }

    // Function handling the basing gravity toggle
    void ToggleGravity()
    {
        if (useGravity)
        {
            // Set of gravity of the camera's phyiscal body and stops all movement imideatly 
            physicalParentBody.useGravity = false;
            physicalParentBody.velocity = Vector3.zero;
            useGravity = false;
        }
        else if (!useGravity)
        {
            physicalParentBody.useGravity = true;
            useGravity = true;
        }
            
    }






































    /*
    void KeyboardController()
    {
        return 0;
    }
    */

    /*
yaw += speedH * Input.GetAxis("Mouse X");
pitch -= speedV * Input.GetAxis("Mouse Y");

transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
print("Current");
print(_currentPos);
Vector3 rotation = new Vector3();
rotation.y = Mathf.Clamp(Input.GetAxis("Mouse X"), minX, maxX);
rotation.x = Mathf.Clamp(Input.GetAxis("Mouse Y"), minY, maxY);
print(rotation);
Quaternion targetPos = Quaternion.Euler(rotation);
print("Target");
print(targetPos);
transform.rotation = Quaternion.LookRotation(rotation);
_currentPos = targetPos;
*/
}
