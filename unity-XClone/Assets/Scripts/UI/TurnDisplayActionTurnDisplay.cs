using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnDisplayActionTurnDisplay : MonoBehaviour{

    TextMeshProUGUI txt;

    // Start is called before the first frame update
    void Start(){
        txt = GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log(txt);
        TurnManager.tree.StartBattle.OnCenterOn += set_text("...");
        TurnManager.tree.PlayerStartTurn.OnCenterOn += set_text("Turn Start!");
        TurnManager.tree.PlayerStartChooseActions.OnCenterOn += set_text("Choose an action");
        TurnManager.tree.PlayerStartChoosePosition.OnCenterOn += set_text("Choose a position");
        TurnManager.tree.PlayerEndTurn.OnCenterOn += set_text("...");
    }

    private StateTree.WhenCenteredOn<TurnTree> set_text(string s){
        return (_a, _b) => txt.text = s;
    }
}
