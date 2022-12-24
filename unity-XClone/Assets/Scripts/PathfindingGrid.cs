using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingGrid : MonoBehaviour
{
    public bool InstantiateVisuals = false;

    public float MapWidth;
    public float MapHeight;

    public float PointRadius;
    public float GridSpacing;

    public GameObject NodePrefab;

    private LayerMask blockers_layer;
    private LayerMask crowd_layer;

    private GridPoint[,] grid_points;


    // Start is called before the first frame update
    void Start()
    {
        blockers_layer = LayerMask.GetMask("Blockers");
        crowd_layer = LayerMask.GetMask("Crowd");

        grid_points = new GridPoint[Mathf.CeilToInt(MapWidth/GridSpacing),Mathf.CeilToInt(MapHeight/GridSpacing)];

        // DO NOT INSTANTIATE ALL OF THESE OBJECTS!!!!!!
        for (int x = 0; x < MapWidth/GridSpacing; x++){
        for (int z = 0; z < MapHeight/GridSpacing; z++){
            Vector3 pos = new Vector3(x*GridSpacing, 0f, z*GridSpacing);
            if (Physics.CheckSphere(pos, PointRadius, blockers_layer)){
                grid_points[x,z] = new GridPoint(pos, SpaceFill.Impassable);
            }
            else if (Physics.CheckSphere(pos, PointRadius, crowd_layer)){
                grid_points[x,z] = new GridPoint(pos, SpaceFill.Crowd);
            }
            else {
                grid_points[x,z] = new GridPoint(pos);
            }
            if (x > 0){
                grid_points[x-1,z].Neighbors[0] = grid_points[x,z];
                grid_points[x,z].Neighbors[2] = grid_points[x-1,z];
            }
            if (z > 0){
                grid_points[x,z-1].Neighbors[1] = grid_points[x,z];
                grid_points[x,z].Neighbors[3] = grid_points[x,z-1];
            }
            if (InstantiateVisuals)
                Instantiate(NodePrefab, pos, Quaternion.identity, transform);
        }}
    }

    // Update is called once per frame
    void Update(){
        foreach (var renderer in GetComponentsInChildren<MeshRenderer>()){
            renderer.enabled = InstantiateVisuals;
        }
    }

    private GridPoint nearest_grid_point(Vector3 position){
        int x_index = Mathf.FloorToInt(Mathf.Clamp(position.x, 0f, MapWidth)/GridSpacing);
        int z_index = Mathf.FloorToInt(Mathf.Clamp(position.z, 0f, MapHeight)/GridSpacing);

        return grid_points[x_index,z_index];
    }

    private float cost_for_fill(SpaceFill fill){
        switch (fill){
            case SpaceFill.None:        return 1f;
            case SpaceFill.Impassable:  return 0f;
            case SpaceFill.Crowd:       return 0.5f;
            default:                    return 0f;
        }
    }

    private Stack<GridPoint> reconstruct_path(
        Dictionary<GridPoint, GridPoint> came_from, 
        GridPoint current
    ){
        var path = new Stack<GridPoint>();
        path.Push(current);
        while (came_from.ContainsKey(current)){
            current = came_from[current];
            path.Push(current);
        }
        return path;
    }

    private float distance_heuristic(Vector3 initialPosition, GridPoint point){
        return (initialPosition-point.Position).magnitude;
    }

    public Stack<GridPoint> GeneratePath(Vector3 initialPosition, Vector3 finalPosition, out int count){
        GridPoint start = nearest_grid_point(initialPosition);
        GridPoint goal = nearest_grid_point(finalPosition);

        var open_set = new List<GridPoint>();
        open_set.Add(start);

        var came_from = new Dictionary<GridPoint, GridPoint>();

        var g_score = new Dictionary<GridPoint, float>();
        g_score.Add(start, 0f);
        
        var f_score = new Dictionary<GridPoint, float>();
        f_score.Add(start, distance_heuristic(initialPosition, start));

        count = 0;
        while (open_set.Count > 0){
            count++;
            // Assume that f_score is sorted from lowest value to heighest value.
            GridPoint current = open_set.OrderBy(point => f_score[point]).First();
            if (current == goal)
                return reconstruct_path(came_from, current);
            
            // Debug.Log("Currently at " + current.Position);
            open_set.Remove(current);
            foreach (GridPoint nhbr in current.Neighbors){
                if (nhbr == null)
                    continue;
                if (cost_for_fill(nhbr.Fill) == 0f)
                    continue;
                // Debug.Log("     Neighbor at " + nhbr.Position);
                float tentative_g_score = g_score[current] + 1f/cost_for_fill(nhbr.Fill);
                if (
                    !g_score.ContainsKey(nhbr) || 
                    tentative_g_score < g_score[nhbr]
                ){
                    came_from[nhbr] = current;
                    g_score[nhbr] = tentative_g_score;
                    f_score[nhbr] = tentative_g_score+distance_heuristic(initialPosition, nhbr);
                    f_score = f_score.OrderBy(pair=>pair.Value).ToDictionary(pair=>pair.Key, pair=>pair.Value);
                    if (!open_set.Contains(nhbr))
                        open_set.Add(nhbr);
                }
            }
        }

        // If we have reached here, we have broken the thing!
        return null;
    }
}
