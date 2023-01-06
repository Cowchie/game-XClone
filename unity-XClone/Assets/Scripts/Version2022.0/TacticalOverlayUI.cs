using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TacticalOverlayUI : MonoBehaviour
{
    public float VisualTolerance = 1f;

    public GameObject GameObjectForPreviewingMoveSpaces;


    private Queue<GameObject> MoveSpacePreviewsQueued;
    private List<GameObject> MoveSpacePreviewsVisible;

    private void OnEnable(){
        TacticalGameplay.OnUnitPossibleMovesCalculated += view_possible_moves;
    }
    private void OnDisable(){
        TacticalGameplay.OnUnitPossibleMovesCalculated -= view_possible_moves;
    }

    // Start is called before the first frame update
    void Start()
    {
        MoveSpacePreviewsQueued = new Queue<GameObject>();
        MoveSpacePreviewsVisible = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // private float sup_norm(Vector3 a){
    //     return Mathf.Max(Mathf.Abs(a.x), Mathf.Abs(a.y),Mathf.Abs(a.z));
    // }

    private void view_possible_moves(Dictionary<GridPoint, float> path_costs){
        foreach (var go in MoveSpacePreviewsVisible){
            go.SetActive(false);
            MoveSpacePreviewsQueued.Enqueue(go);
        }
        MoveSpacePreviewsVisible.Clear();
        foreach (var point in path_costs.Keys){
            GameObject preview;
            if (MoveSpacePreviewsQueued.Count > 0){
                preview = MoveSpacePreviewsQueued.Dequeue();
                preview.transform.position = point.Position;
                preview.transform.rotation = Quaternion.identity;
                preview.SetActive(true);
            }
            else
                preview = Instantiate(GameObjectForPreviewingMoveSpaces, point.Position, Quaternion.identity);
            
            preview.GetComponentInChildren<TextMeshPro>().text = path_costs[point].ToString();
            MoveSpacePreviewsVisible.Add(preview);
        }
    }
}
