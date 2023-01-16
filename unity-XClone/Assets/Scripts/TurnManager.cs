using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using StateTree;

public struct TurnTree{
    public DoNext<TurnTree>         StartBattle;
    public CycleBranches<TurnTree>  BetweenTurns;
    public DoNext<TurnTree>         StartPlayerTurn;
    public DelayDoNext<TurnTree>    WaitForEndPlayerTurn;
    public DoNext<TurnTree>         EndPlayerTurn;
}

public class TurnManager : MonoBehaviour
{
    public PlayerInputActions input;

    private TurnTree tree;
    TreeBranch<TurnTree> current_turn;

    // Start is called before the first frame update
    void Start(){
        // Initializes the inputs
        input = new PlayerInputActions();
        input.TacticalCombat.Enable();

        // Initializes the tree
        tree = new TurnTree();

        // Branch which triggers at the start of the battle.
        tree.StartBattle = new DoNext<TurnTree>(
            s => s.BetweenTurns
        );
        tree.StartBattle.OnCenterOn += 
            LogOnCenterOn("Start Battle!");

        // Branch which cycles between the turns.
        tree.BetweenTurns = new CycleBranches<TurnTree>(
            s => s.StartPlayerTurn
        );

        // Branch which triggers at the start of the player's turn.
        tree.StartPlayerTurn = new DoNext<TurnTree>(
            s => s.WaitForEndPlayerTurn
        );
        tree.StartPlayerTurn.OnCenterOn += 
            LogOnCenterOn("     Start Player Turn! Please choose an action...");

        // Branch which waits for the player to press the end turn button.
        tree.WaitForEndPlayerTurn = new DelayDoNext<TurnTree>(
            out var player_end_turn_callback, 
            s => s.EndPlayerTurn
        );
        input.TacticalCombat.EndTurn.performed += 
            (c => player_end_turn_callback());

        // Branch which triggers at the end of the player's turn.
        tree.EndPlayerTurn = new DoNext<TurnTree>(
            s => s.BetweenTurns
        );
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
