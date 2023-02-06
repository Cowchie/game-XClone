using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerDirector : MonoBehaviour
{
    public PlayerInputActions input;

    private LayerMask map_layer;
    // Awake is called before Start is called before the first frame update
    void Awake(){
        // Initializes the inputs
        input = new PlayerInputActions();
        input.TacticalCombat.Enable();

        // Initializes the physics layers
        map_layer = LayerMask.GetMask("Map") + LayerMask.GetMask("Blockers");
    }

    // Update is called once per frame
    void Update(){
        
    }

    public void SetActionKey(Action a, int index){
        KeyAtIndex(input.TacticalCombat, index).performed += (cc => a());
    }

    private static InputAction KeyAtIndex(
        PlayerInputActions.TacticalCombatActions tactical_combat, 
        int index
    ){
        switch (index){
            case 0: return tactical_combat.Key0;
            case 1: return tactical_combat.Key1;
            case 2: return tactical_combat.Key2;
            case 3: return tactical_combat.Key3;
            case 4: return tactical_combat.Key4;
            case 5: return tactical_combat.Key5;
            case 6: return tactical_combat.Key6;
            case 7: return tactical_combat.Key7;
            case 8: return tactical_combat.Key8;
            case 9: return tactical_combat.Key9;
            case 10: return tactical_combat.Key10;
            default: throw new ArgumentOutOfRangeException("Need to input value in range 0 to 11");
        }
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
