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
            if (InstantiateVisuals){
                var node = Instantiate(NodePrefab, pos, Quaternion.identity, transform);
                node.GetComponent<GridPointVisualizing>().SetMoveCost(move_rate_for_fill(grid_points[x,z].Fill));
            }
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

    private float move_rate_for_fill(SpaceFill fill){
        switch (fill){
            case SpaceFill.None:        return 1f;
            case SpaceFill.Impassable:  return 0f;
            case SpaceFill.Crowd:       return 0.5f;
            default:                    return 0f;
        }
    }

    public Stack<GridPoint> ReconstructPath(
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

    private float box_car_distance(Vector3 v){
        return Mathf.Abs(v.x)+Mathf.Abs(v.y)+Mathf.Abs(v.z);
    }

    private float distance_heuristic(GridPoint point, Vector3 final_position){
        return (point.Position-final_position).magnitude;
    }

    // Finds all of the shortest paths of length <= distance.
    public (Dictionary<GridPoint, GridPoint>, Dictionary<GridPoint, float>) FindAllPossibleMoves(Vector3 initialPosition, float maxDistance){
        GridPoint start = nearest_grid_point(initialPosition);

        var open_set = new List<GridPoint>();
        open_set.Add(start);

        var came_from = new Dictionary<GridPoint, GridPoint>();

        var g_score = new Dictionary<GridPoint, float>();
        g_score[start] = 0f;
        

        while (open_set.Count > 0){
            // We should use a priority queue at some point.
            GridPoint current = open_set.OrderBy(point => g_score[point]).First();

            // Debug.Log("Currently at " + current.Position);
            open_set.Remove(current);
            foreach (GridPoint nhbr in current.Neighbors){
                if (nhbr == null)
                    continue;
                if (move_rate_for_fill(nhbr.Fill) == 0f)
                    continue;
                // Debug.Log("     Neighbor at " + nhbr.Position);
                float tentative_g_score = g_score[current] + 1f/move_rate_for_fill(nhbr.Fill);
                // If this path is too far, continue;
                if (tentative_g_score > maxDistance)
                    continue;
                // If the nhbr does not have infinite distance and we already have a better score, continue;
                if (g_score.ContainsKey(nhbr) && 
                    tentative_g_score >= g_score[nhbr]
                )
                    continue;
                
                came_from[nhbr] = current;
                g_score[nhbr] = tentative_g_score;
                // f_score = f_score.OrderBy(pair=>pair.Value).ToDictionary(pair=>pair.Key, pair=>pair.Value);
                if (!open_set.Contains(nhbr))
                    open_set.Add(nhbr);
            }
        }

        // If we have reached here, we can return the came_from dictionary and also the g_score? 
        return (came_from, g_score);
    }

    // Finds the fasted path between initailPosition and finalPosition
    public Stack<GridPoint> GeneratePath(Vector3 initialPosition, Vector3 finalPosition){
        GridPoint start = nearest_grid_point(initialPosition);
        GridPoint goal = nearest_grid_point(finalPosition);

        var open_set = new List<GridPoint>();
        open_set.Add(start);

        var came_from = new Dictionary<GridPoint, GridPoint>();

        var g_score = new Dictionary<GridPoint, float>();
        g_score[start] = 0f;
        
        var f_score = new Dictionary<GridPoint, float>();
        f_score[start] = distance_heuristic(start, finalPosition);

        while (open_set.Count > 0){
            // We should use a priority queue at some point.
            GridPoint current = open_set.OrderBy(point => f_score[point]).First();
            if (current == goal)
                return ReconstructPath(came_from, current);
            
            // Debug.Log("Currently at " + current.Position);
            open_set.Remove(current);
            foreach (GridPoint nhbr in current.Neighbors){
                if (nhbr == null)
                    continue;
                if (move_rate_for_fill(nhbr.Fill) == 0f)
                    continue;
                // Debug.Log("     Neighbor at " + nhbr.Position);
                float tentative_g_score = g_score[current] + 1f/move_rate_for_fill(nhbr.Fill);
                if (g_score.ContainsKey(nhbr) && 
                    tentative_g_score >= g_score[nhbr]
                )
                    continue;
                
                came_from[nhbr] = current;
                g_score[nhbr] = tentative_g_score;
                f_score[nhbr] = tentative_g_score+distance_heuristic(nhbr, finalPosition);
                // f_score = f_score.OrderBy(pair=>pair.Value).ToDictionary(pair=>pair.Key, pair=>pair.Value);
                if (!open_set.Contains(nhbr))
                    open_set.Add(nhbr);
            }
        }

        // If we have reached here, we have broken the thing!
        return null;
    }
}
