using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDistanceIndicator : MonoBehaviour
{
    public MeshRenderer MyRenderer;

    void OnEnable(){
        transform.parent.GetComponent<UnitMoveToPositionStraightLine>().IndicateSelected += turn_on;
    }

    void OnDisable(){
        transform.parent.GetComponent<UnitMoveToPositionStraightLine>().IndicateSelected -= turn_on;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void turn_on(bool active){
        MyRenderer.enabled = active;
    }
}
