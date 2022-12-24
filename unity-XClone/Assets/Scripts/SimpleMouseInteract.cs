using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMouseInteract : MonoBehaviour
{
    public static event Action<Vector3> PlayerSelectPosition;

    
    private LayerMask map_layer;
    private LayerMask not_crowd_layer;
    
    private UnitMoveToPositionStraightLine selected_unit;

    // Start is called before the first frame update
    void Start(){
        map_layer = LayerMask.GetMask("Map") + LayerMask.GetMask("Blockers");
        not_crowd_layer = -1 - LayerMask.GetMask("Crowd");
    }

    // Update is called once per frame
    void Update(){

        if (Input.GetMouseButtonUp(1)){
            Ray mouse_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Check to see if we have hit something that blocks mouse click.
            if (Physics.Raycast(mouse_ray, out RaycastHit hit, 50f, map_layer)){
                // Check to see if that thing was the ground.
                if (hit.transform.tag == "GroundPlane"){
                    PlayerSelectPosition?.Invoke(new Vector3(hit.point.x, hit.transform.position.y, hit.point.z));
                    selected_unit?.SelectDestination(new Vector3(hit.point.x, hit.transform.position.y, hit.point.z));
                }
            }
        }

        if (Input.GetMouseButtonUp(0)){
            Ray mouse_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouse_ray, out RaycastHit hit, 50f, not_crowd_layer)){
                // Check to see if we hit a unit.
                var new_unit = hit.transform.GetComponent<UnitMoveToPositionStraightLine>();

                if (selected_unit != null){
                    selected_unit.IndicateSelected?.Invoke(false);
                    selected_unit.IsSelected = false;
                }
                selected_unit = null;
                if (new_unit != null){
                    selected_unit = new_unit;
                    selected_unit.IsSelected = true;
                    selected_unit.IndicateSelected?.Invoke(true);
                }
            }
        }
    }
}