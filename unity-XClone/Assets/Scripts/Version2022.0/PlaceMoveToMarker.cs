using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceMoveToMarker : MonoBehaviour
{

    public GameObject marker;

    void OnEnable(){
        SimpleMouseInteract.PlayerSelectMoveToPosition += place_marker;
    }

    void OnDisable(){
        SimpleMouseInteract.PlayerSelectMoveToPosition -= place_marker;
    }
    // Update is called once per frame
    void Update()
    {
        
    }


    private void place_marker(GridPoint point, Vector3 position){
        marker.transform.position = position;
        marker.SetActive(true);
    }
}
