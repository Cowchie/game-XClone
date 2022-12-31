using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum UnitAffiliation{
    Player, Hostile
}

public struct Unit{
    public string Name;
    public UnitAffiliation Affiliation;

    // If this is 0, the unit has not been selected. If this is non-negative, say 2, this unit was the 2nd one selected.
    public int SelectedNumber;
    public Vector3 Position;
    public float StartOfTurnActionPoints;
    public float ActionPoints;

    public float MoveDistancePerActionPoint;
}

public class TacticalGameplay : MonoBehaviour
{

    public static event Action<Dictionary<GridPoint, float>> OnUnitPossibleMovesCalculated;

    //TODO: This is a bad way to do this kind of stuff. I should fix it.
    public PathfindingGrid Pathfinding;
    public SimpleMouseInteract MouseInteract;

    private List<Unit> units;

    // Start is called before the first frame update
    void Start(){
        units = new List<Unit>();
        Unit dummy = new Unit();
        dummy.Name = "dummy";
        dummy.Affiliation = UnitAffiliation.Player;
        dummy.SelectedNumber = 1;
        dummy.Position = new Vector3(25f,0f,25f);
        dummy.StartOfTurnActionPoints = 2f;
        dummy.ActionPoints = 2f;
        dummy.MoveDistancePerActionPoint = 9f;

        units.Add(dummy);
    }

    void OnEnable(){
        SimpleMouseInteract.PlayerSelectMoveToPosition += select_position_to_move_selected_unit_to;
    }

    void OnDisable(){
        SimpleMouseInteract.PlayerSelectMoveToPosition -= select_position_to_move_selected_unit_to;
    }


    private Unit selected_unit;
    private Dictionary<GridPoint, GridPoint> selected_unit_came_froms;
    private Dictionary<GridPoint, float> selected_unit_path_costs;


    // Update is called once per frame
    void Update(){
        if (Input.GetKeyDown(KeyCode.Space)){
            //TODO: Figure out how turns are going to work and run this code when the unit is selected.
            // Start of turn we want to find the grid points that we can move to.
            selected_unit = units.OrderByDescending(u => u.SelectedNumber).First();
            if (selected_unit.SelectedNumber < 1)
                return;

            (selected_unit_came_froms, selected_unit_path_costs) = Pathfinding.FindAllPossibleMoves(selected_unit.Position, selected_unit.MoveDistancePerActionPoint);

            //TODO: Separate this into a separate visuals class which displays this information.
            // Debug.Log("The points that "+selected.Name+" can visit are: ");
            OnUnitPossibleMovesCalculated?.Invoke(selected_unit_path_costs);   
        }
    }

    private void select_position_to_move_selected_unit_to(GridPoint finalGridPoint, Vector3 position){
        var path = Pathfinding.ReconstructPath(selected_unit_came_froms, finalGridPoint);
        float path_length = selected_unit_path_costs[finalGridPoint];

        selected_unit.ActionPoints -= path_length;

        while(path.Count > 0){
            selected_unit.Position = path.Pop().Position;
            Debug.Log("The unit " + selected_unit.Name + " is at the position " + selected_unit.Position);
        }
    }
}
