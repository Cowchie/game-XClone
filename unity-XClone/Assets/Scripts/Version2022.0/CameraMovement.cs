using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform CameraFacingDirection;

    public float MoveSpeed;
    public float RotateSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 input = 
            CameraFacingDirection.right*Input.GetAxis("Horizontal") +
            CameraFacingDirection.forward*Input.GetAxis("Vertical");

        // Linear motion camera movement
        transform.Translate(input.normalized*MoveSpeed*Time.deltaTime, Space.World);
    
        bool rot_right = Input.GetKey(KeyCode.E);
        bool rot_left = Input.GetKey(KeyCode.Q);

        float rot_direction = 0f;
        if (rot_right && !rot_left)
            rot_direction = -1f;
        if (!rot_right && rot_left)
            rot_direction = 1f;

        CameraFacingDirection.Rotate(transform.up, rot_direction*RotateSpeed*Time.deltaTime, Space.World);
    }
}