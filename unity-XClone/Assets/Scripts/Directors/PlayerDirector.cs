using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerDirector : MonoBehaviour{
    public PlayerInputActions input;

    public Transform CameraFacingDirection;
    public Transform CameraOrbitPoint;

    public float MoveSpeed;
    public float RotateSpeed;

    private LayerMask map_layer;
    private LayerMask not_crowd_layer;


    // Awake is called before Start is called before the first frame update
    void Awake(){
        // Initializes the inputs
        input = new PlayerInputActions();
        input.TacticalCombat.Enable();

        // Initializes the physics layers
        map_layer = LayerMask.GetMask("Map") + LayerMask.GetMask("Blockers");
        not_crowd_layer = -1 - LayerMask.GetMask("Crowd");
    }

    // Update is called once per frame
    void Update(){
        Vector2 input_dir 
            = input.TacticalCombat.CameraMove.ReadValue<Vector2>();
        Vector3 move_dir 
            = input_dir.x*CameraFacingDirection.right + 
                input_dir.y*CameraFacingDirection.forward;

        // Linear motion camera movement
        CameraOrbitPoint.Translate(
            move_dir.normalized*MoveSpeed*Time.deltaTime, 
            Space.World
        );
    
        bool rot_right = input.TacticalCombat.CameraRotateRight.IsPressed();
        bool rot_left = input.TacticalCombat.CameraRotateLeft.IsPressed();

        float rot_direction = 0f;
        if (rot_right && !rot_left)
            rot_direction = -1f;
        if (!rot_right && rot_left)
            rot_direction = 1f;

        CameraFacingDirection.Rotate(
            transform.up, 
            rot_direction*RotateSpeed*Time.deltaTime, 
            Space.World
        );
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
    public delegate bool RaycastConditions(
        Ray ray,
        out RaycastHit hit
    );

    public void SetPickPosition(
        RaycastConditions raycast_conditions, 
        Action a
    ){
        input.TacticalCombat.ClickPosition.canceled += (cc => {
            Ray mouse_ray = Camera.main.ScreenPointToRay(input.TacticalCombat.MousePosition.ReadValue<Vector2>());
            if (!raycast_conditions(mouse_ray, out RaycastHit hit))
                return;
            a();
        });
    }

    [SerializeField]
    private Vector3 selected_position;
    public bool RaycastToGround(Ray ray, out RaycastHit hit){
        if (!Physics.Raycast(ray, out hit, 50f, map_layer))
            return false;
        if (hit.transform.tag != "GroundPlane")
            return false;
        selected_position = hit.point;
        return true;
    }

    private UnitMoveToPositionStraightLine asd;
    public bool RaycastToUnit(Ray ray, out RaycastHit hit){
        if (!Physics.Raycast(ray, out hit, 50f, not_crowd_layer))
            return false;
        var unit_vis 
            = hit.transform.GetComponent<UnitMoveToPositionStraightLine>();
        if (unit_vis == null)
            return false;
        asd = unit_vis;
        return true;
    }

    public void SetCancel(Action a){
        input.TacticalCombat.Cancel.performed += (cc => a());
    }

    public void SetEndTurn(Action a){
        input.TacticalCombat.EndTurn.performed += (cc => a());
    }
}
