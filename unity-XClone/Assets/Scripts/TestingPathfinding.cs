using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingPathfinding : MonoBehaviour
{
    public PathfindingGrid grid;

    // Start is called before the first frame update
    void OnEnable()
    {
        int count;
        var path = grid.GeneratePath(new Vector3(17f,0f,19f), new Vector3(16f,0f,29f), out count);
        Debug.Log(count);
        if (path == null){
            Debug.Log("AAAAAAAAAAAAAAHHHHHHHHHHHHHHH!!!!!!!!!");
            return;
        }
        while(path.Count > 0)
            Debug.Log(path.Pop().Position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
