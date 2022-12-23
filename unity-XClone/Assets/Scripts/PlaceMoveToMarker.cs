using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceMoveToMarker : MonoBehaviour
{
    void OnEnable(){
        SimpleMouseMovement.PlayerSelectPosition += place_marker;
    }

    void OnDisable(){
        SimpleMouseMovement.PlayerSelectPosition -= place_marker;
    }
    // Update is called once per frame
    void Update()
    {
        
    }


    private void place_marker(Vector3 position){
        Debug.Log("Marker is at " + position);
    }
}
