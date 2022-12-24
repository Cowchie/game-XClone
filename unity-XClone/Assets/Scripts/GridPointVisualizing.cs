using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPointVisualizing : MonoBehaviour
{
    public void SetMoveCost(float cost){
        GetComponent<Renderer>().material.SetFloat("_MoveCost", cost);
    }
}
