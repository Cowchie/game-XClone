using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    //TODO: This is a bad way to do this kind of stuff. I should fix it.
    public PathfindingGrid Pathfinding;

    public GameObject GameObjectForPreviewigSpaces;

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
        // Start of turn we want to find the grid points that we can move to.
        Unit selected = units.OrderBy(u => -u.SelectedNumber).First();
        if (selected.SelectedNumber < 1)
            return;
        
        (var came_froms, var path_costs) = Pathfinding.FindAllPossibleMoves(selected.Position, selected.MoveDistancePerActionPoint);

        Debug.Log("The points that "+selected.Name+" can visit are: ");
        foreach (var point in path_costs.Keys){
            var preview = Instantiate(GameObjectForPreviewigSpaces, point.Position, Quaternion.identity);
            preview.GetComponentInChildren<TextMeshPro>().text = path_costs[point].ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
