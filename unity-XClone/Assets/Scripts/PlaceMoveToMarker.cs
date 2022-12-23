using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceMoveToMarker : MonoBehaviour
{

    public GameObject marker;

    void OnEnable(){
        SimpleMouseInteract.PlayerSelectPosition += place_marker;
    }

    void OnDisable(){
        SimpleMouseInteract.PlayerSelectPosition -= place_marker;
    }
    // Update is called once per frame
    void Update()
    {
        
    }


    private void place_marker(Vector3 position){
        marker.transform.position = position;
        marker.SetActive(true);
    }
}