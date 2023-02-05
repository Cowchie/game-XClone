using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBar : MonoBehaviour{
    public GameObject ActionVisPrefab;

    // Start is called before the first frame update
    void Start(){
        
    }

    public void ShowActions((string, System.Action)[] titles_and_actions){
        for (int i = 0; i < titles_and_actions.Length; i++){
            var vis = Instantiate(ActionVisPrefab, Vector3.zero, Quaternion.identity, transform);
            var (s, a) = titles_and_actions[i];
            vis.GetComponent<ActionBarActionVis>().SetTextAndAction(s, a, i);
        }
    }
}
