using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnDisplay : MonoBehaviour
{
    TextMeshProUGUI txt;
    

    // Start is called before the first frame update
    void Start(){
        txt = GetComponentInChildren<TextMeshProUGUI>();
        TurnManager.tree.StartBattle.OnCenterOn += set_text("Start Battle!");
        TurnManager.tree.PlayerStartTurn.OnCenterOn += set_text("Player Turn");
        TurnManager.tree.EnemyStartTurn.OnCenterOn += set_text("Enemy Turn");
    }

    private StateTree.WhenCenteredOn<TurnTree> set_text(string s){
        return (_a, _b) => txt.text = s;
    }
}
