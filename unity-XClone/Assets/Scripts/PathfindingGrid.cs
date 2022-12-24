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

    private List<GridPoint> grid_points = new List<GridPoint>();


    private LayerMask blockers_layer;

    public GameObject NodePrefab;

    // Start is called before the first frame update
    void Start()
    {
        blockers_layer = LayerMask.GetMask("Blockers");

        // DO NOT INSTANTIATE ALL OF THESE OBJECTS!!!!!!
        for (int x = 0; x < MapWidth/GridSpacing; x++){
        for (int z = 0; z < MapHeight/GridSpacing; z++){
            Vector3 pos = new Vector3(x*GridSpacing, 0f, z*GridSpacing);
            if (Physics.CheckSphere(pos, PointRadius, blockers_layer)){
                // Debug.LogError("Cannot place node at " + pos);
                continue;
            }
            grid_points.Add(new GridPoint(pos));
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
}
