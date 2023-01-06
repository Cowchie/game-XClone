using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingPathfinding : MonoBehaviour
{
    public PathfindingGrid grid;

    // Start is called before the first frame update
    void OnEnable(){
        var path = grid.GeneratePath(new Vector3(0f,0f,0f), new Vector3(49f,0f,49f));
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
