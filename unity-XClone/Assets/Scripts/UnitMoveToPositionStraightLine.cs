using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoveToPositionStraightLine : MonoBehaviour
{
    public float MaxMoveDistance;
    public float MoveSpeed;


    private bool is_animating = false;

    private Vector3 initial_position;
    private Vector3 destination;

    void Start(){
        initial_position = transform.position;
        destination = transform.position;
    }

    void OnEnable(){
        SimpleMouseMovement.PlayerSelectPosition += SelectDestination;
    }

    void OnDisable(){
        SimpleMouseMovement.PlayerSelectPosition -= SelectDestination;
    }

    // Update is called once per frame
    void Update()
    {
    }


    private void SelectDestination(Vector3 new_destination){
        if (is_animating)
            return;

        if ((new_destination - transform.position).sqrMagnitude >= MaxMoveDistance*MaxMoveDistance){
            Debug.LogError("Cannot Move that far!");
            return;
        }
        initial_position = transform.position;
        destination = new_destination;

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
            transform.Translate(disp_per_step);
        }

        is_animating = false;
    }
}
