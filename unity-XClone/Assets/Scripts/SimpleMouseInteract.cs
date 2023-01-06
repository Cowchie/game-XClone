using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMouseInteract : MonoBehaviour
{
    public static event Action<Unit, Action<Stack<GridPoint>>> PlayerSelectUnit;
    public static event Action<GridPoint, Vector3> PlayerSelectMoveToPosition;

    public TacticalOverlayUI OverlayUI;
    
    private LayerMask map_layer;
    private LayerMask not_crowd_layer;

    void OnEnable(){
        TacticalGameplay.OnUnitPossibleMovesCalculated += set_possible_unit_move_to_area;
    }

    void OnDisable(){
        TacticalGameplay.OnUnitPossibleMovesCalculated -= set_possible_unit_move_to_area;
    }


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
                    var point = find_grid_point_for_selected_unit_move(hit.point);
                    // Debug.Log("Clicked on: " + hit.point);
                    // Debug.Log("Nearest grid point is: " + point.Position);
                    if (point != null)
                        PlayerSelectMoveToPosition?.Invoke(point, hit.point);
                }
            }
        }

        if (Input.GetMouseButtonUp(0)){
            Ray mouse_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouse_ray, out RaycastHit hit, 50f, not_crowd_layer)){
                // Check to see if we hit a unit.
                var new_unit = hit.transform.GetComponent<UnitMoveToPositionStraightLine>();

                if (new_unit != null){
                    PlayerSelectUnit?.Invoke(new_unit.GetUnit, new_unit.ChooseMovePath);
                }
                if (new_unit != null){
                    Debug.Log("What the hell do I do here? I'm actually so confused right now...");
                }
            }
        }
    }

    private List<GridPoint> points_to_move_near;
    //TODO: I dunno what to do about this.
    private float grid_radius = 0.5f;
    private void set_possible_unit_move_to_area(Dictionary<GridPoint, float> path_costs){
        points_to_move_near = new List<GridPoint>(path_costs.Keys);
    }

    private GridPoint find_grid_point_for_selected_unit_move(Vector3 position){
        foreach (var point in points_to_move_near){
            if ((point.Position-position).sqrMagnitude < grid_radius*grid_radius)
                return point;
        }
        return null;
    }
}