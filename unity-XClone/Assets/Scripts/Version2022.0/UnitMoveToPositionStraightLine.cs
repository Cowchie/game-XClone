using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoveToPositionStraightLine : MonoBehaviour
{
    private OldUnit my_unit;
    public OldUnit GetUnit{
        get {return my_unit;}
    }

    public event Action<bool> IndicateSelected;

    public float MaxMoveDistance;
    public float MoveSpeed;
    public float RotateSpeed;


    private bool IsSelected = false;

    private bool is_animating = false;

    void Start(){}

    private Stack<GridPoint> move_path;

    public void ChooseMovePath(Stack<GridPoint> path){
        move_path = path;
        move_to_grid_point_line(path.Pop());
    }

    private void move_to_grid_point_line(GridPoint point){
        if (is_animating)
            return;

        StartCoroutine(rotate_then_move_to_destination(point, move_to_grid_point_line));
    }
    private IEnumerator rotate_then_move_to_destination(GridPoint destination_point, Action<GridPoint> finished){
        Vector3 initial_position = transform.position;
        Vector3 destination = destination_point.Position; 
        Vector3 displacement = destination - initial_position;
        float distance = displacement.magnitude;
        Vector3 direction = displacement/distance;
        float angle = Vector3.SignedAngle(transform.forward, direction, transform.up);
        
        float min_rot_distance = 0.1f;
        float min_move_distance = 0.01f;

        int number_steps = Mathf.CeilToInt(angle/min_rot_distance);
        float time_step = Mathf.Abs(angle)/(RotateSpeed*number_steps);


        is_animating = true;
        IndicateSelected?.Invoke(false);

        float angle_per_step = Mathf.Sign(angle)*RotateSpeed*time_step;
        for (int i = 0; i < number_steps; i++)
        {
            yield return new WaitForSeconds(time_step);
            transform.Rotate(transform.up, angle_per_step, Space.World);
        }

        number_steps = Mathf.CeilToInt(distance/min_move_distance);
        time_step = distance/(MoveSpeed*number_steps);

        Vector3 disp_per_step = direction/distance*MoveSpeed*time_step;
        for (int i = 0; i < number_steps; i++)
        {
            yield return new WaitForSeconds(time_step);
            transform.Translate(disp_per_step, Space.World);
        }

        if (move_path.Count > 0){
            finished(move_path.Pop());
        }
        else{
            is_animating = false;
            if (IsSelected)
                IndicateSelected?.Invoke(true);
        }
    }


    public OldUnit SelectMe(out Action deselect_action){
        IsSelected = true;
        IndicateSelected?.Invoke(true);
        deselect_action = deselect_me;
        return my_unit;
    }

    private void deselect_me(){
        IsSelected = false;
        IndicateSelected?.Invoke(false);
    }
}
