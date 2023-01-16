using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using StateTree;

public struct TurnTree{
    public CallbacksThenDoNext<TurnTree>    StartBattle;
    public CycleBranches<TurnTree>          BetweenTurns;
    public CallbacksThenDoNext<TurnTree>    StartPlayerTurn;
    public CallbacksThenDoNext<TurnTree>    EndPlayerTurn;
}

public class TurnManager : MonoBehaviour
{
    private TurnTree tree;

    TreeBranch<TurnTree> current_turn;

    // Start is called before the first frame update
    void Start(){
        tree = new TurnTree();

        tree.StartBattle = new CallbacksThenDoNext<TurnTree>(
            s => s.BetweenTurns
        );

        tree.BetweenTurns = new CycleBranches<TurnTree>(
            s => s.StartPlayerTurn
        );

        tree.StartPlayerTurn = new CallbacksThenDoNext<TurnTree>(
            s => s.EndPlayerTurn
        );

        tree.EndPlayerTurn = new CallbacksThenDoNext<TurnTree>(
            s => s.BetweenTurns
        );

        tree.StartBattle.OnCenterOn += 
            LogOnCenterOn("Start Battle!");
        tree.StartPlayerTurn.OnCenterOn += 
            LogOnCenterOn("     Start Player Turn! Please choose an action...");
        tree.EndPlayerTurn.OnCenterOn += 
            LogOnCenterOn("     End Player Turn");
    }

    // Update is called once per frame
    void Update(){
        if (current_turn is null){
            current_turn = tree.StartBattle;
            current_turn.CenterOn(tree.StartBattle);
        }

        var prev_branch = current_turn;
        current_turn = current_turn.DoUpdate(tree);
        if (current_turn != prev_branch){
            current_turn.CenterOn(prev_branch);
        }
    }

    private WhenCenteredOn<TurnTree> LogOnCenterOn(
        string s
    ){
        return (a, b) => Debug.Log(s);
    }
}
