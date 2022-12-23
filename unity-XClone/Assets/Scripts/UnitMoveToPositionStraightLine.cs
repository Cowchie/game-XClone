using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoveToPositionStraightLine : MonoBehaviour
{
    public float MaxMoveDistance;

    public float TimeToMove;

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
        if ((destination-transform.position).sqrMagnitude < (destination-initial_position).sqrMagnitude*0.001f){
            destination = transform.position;
            initial_position = destination;
            return;
        }
        transform.Translate((destination-initial_position)*Time.deltaTime/TimeToMove);
    }


    private void SelectDestination(Vector3 new_destination){
        if ((new_destination - transform.position).sqrMagnitude >= MaxMoveDistance*MaxMoveDistance){
            Debug.LogError("Cannot Move that far!");
            return;
        }
        initial_position = transform.position;
        destination = new_destination;
    }
}
