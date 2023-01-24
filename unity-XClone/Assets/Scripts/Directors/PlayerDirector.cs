using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerDirector : MonoBehaviour
{
    public PlayerInputActions input;

    private LayerMask map_layer;
    // Start is called before the first frame update
    void Start(){
        // Initializes the inputs
        input = new PlayerInputActions();
        input.TacticalCombat.Enable();

        // Initializes the physics layers
        map_layer = LayerMask.GetMask("Map") + LayerMask.GetMask("Blockers");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUnitMove(Action a){
        input.TacticalCombat.UnitMove.performed += (cc => a());
    }

    // TODO: Update this to eventually get all of the checking information from somewhere else, maybe pass in a function which returns a flag?
    [SerializeField]
    private Vector3 selected_position;
    public void SetPickPosition(Action a){
        input.TacticalCombat.PickPosition.canceled += (cc => {
            Ray mouse_ray = Camera.main.ScreenPointToRay(input.TacticalCombat.MousePosition.ReadValue<Vector2>());
            if (!Physics.Raycast(mouse_ray, out RaycastHit hit, 50f, map_layer))
                return;
            if (hit.transform.tag != "GroundPlane")
                return;
            selected_position = hit.point;
            a();
        });
    }

    public void SetCancel(Action a){
        input.TacticalCombat.Cancel.performed += (cc => a());
    }

    public void SetEndTurn(Action a){
        input.TacticalCombat.EndTurn.performed += (cc => a());
    }
}
