using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMouseMovement : MonoBehaviour
{
    public static event Action<Vector3> PlayerSelectPosition;

    
    private LayerMask map_layer;

    // Start is called before the first frame update
    void Start()
    {
        map_layer = LayerMask.GetMask("MapLayer");
    }

    Vector3 destination;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(1)){
            Ray mouse_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Check to see if we have hit something that blocks mouse click.
            if (Physics.Raycast(mouse_ray, out RaycastHit hit, 50f, map_layer)){
                // Check to see if that thing was the ground.
                if (hit.transform.tag == "GroundPlane"){
                    // Debug.Log("Hit the ground at position " + hit.point);
                    PlayerSelectPosition?.Invoke(new Vector3(hit.point.x,0f,hit.point.z));
                }
            }
        }
    }
}