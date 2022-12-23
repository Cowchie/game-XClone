using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoveToPositionStraightLine : MonoBehaviour
{
    public Action<bool> IndicateSelected;

    public float MaxMoveDistance;
    public float MoveSpeed;
    public float RotateSpeed;


    public bool IsSelected = false;

    private bool is_animating = false;

    private Vector3 initial_position;
    private Vector3 destination;

    void Start(){
        initial_position = transform.position;
        destination = transform.position;
    }


    public void SelectDestination(Vector3 new_destination){
        if (is_animating)
            return;

        if ((new_destination - transform.position).sqrMagnitude >= MaxMoveDistance*MaxMoveDistance){
            return;
        }
        initial_position = transform.position;
        destination = new_destination;

        StartCoroutine(RotateToFacePath());
    }


    private IEnumerator RotateToFacePath(){        
        Vector3 direction = (destination - initial_position).normalized;
        float angle = Vector3.SignedAngle(transform.forward, direction, transform.up);

        int number_steps = 100;
        float time_step = Mathf.Abs(angle)/(RotateSpeed*number_steps);

        is_animating = true;
        IndicateSelected?.Invoke(false);

        float angle_per_step = Mathf.Sign(angle)*RotateSpeed*time_step;
        for (int i = 0; i < number_steps; i++)
        {
            yield return new WaitForSeconds(time_step);
            transform.Rotate(transform.up, angle_per_step, Space.World);
        }

        is_animating = false;
        StartCoroutine(FollowStraightLinePath());
    }


    private IEnumerator FollowStraightLinePath(){
        Vector3 direction = destination - initial_position;
        float distance = direction.magnitude;
        int number_steps = 100;
        float time_step = distance/(MoveSpeed*number_steps);

        is_animating = true;

        Vector3 disp_per_step = direction/distance*MoveSpeed*time_step;
        for (int i = 0; i < number_steps; i++)
        {
            yield return new WaitForSeconds(time_step);
            transform.Translate(disp_per_step, Space.World);
        }

        is_animating = false;
        if (IsSelected)
            IndicateSelected?.Invoke(true);
    }
}
