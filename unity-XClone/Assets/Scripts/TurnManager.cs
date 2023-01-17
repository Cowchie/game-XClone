using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using StateTree;

public struct TurnTree{
    public DoNext<TurnTree>         StartBattle;
    public CycleBranches<TurnTree>  BetweenTurns;
    public DoNext<TurnTree>         PlayerStartTurn;
    public DelayDoNext<TurnTree>    DelayPlayerUnitMove;
    public DoNext<TurnTree>         PlayerStartUnitMove;
    public DelayDoNext<TurnTree>    DelayPlayerEndTurn;
    public FirstOfNext<TurnTree>    FirstOfPlayerActions;
    public DoNext<TurnTree>         PlayerEndTurn;
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

{ // Branch which cycles between the turns.
        tree.BetweenTurns = new CycleBranches<TurnTree>(
            s => s.PlayerStartTurn
        );
}
{ // Branch which triggers at the start of the player's turn.
        tree.PlayerStartTurn = new DoNext<TurnTree>(
            s => s.FirstOfPlayerActions
        );
        tree.PlayerStartTurn.OnCenterOn += 
            LogOnCenterOn("     Start Player Turn! Please choose an action...");
}
{ // Branch which waits for the player to press the button to enter unit move mode.
        tree.DelayPlayerUnitMove = new DelayDoNext<TurnTree>(
            out var player_unit_move_callback, 
            s => s.PlayerStartUnitMove
        );
        input.TacticalCombat.UnitMove.performed += 
            (c => player_unit_move_callback());
}
{ // Branch which triggers when player chooses to move
        tree.PlayerStartUnitMove = new DoNext<TurnTree>(
            s => s.FirstOfPlayerActions
        );
        tree.PlayerStartUnitMove.OnCenterOn += 
            LogOnCenterOn("     Choose a place for the unit to move!");
}
{ // Branch which waits for the player to press the end turn button.
        tree.DelayPlayerEndTurn = new DelayDoNext<TurnTree>(
            out var player_end_turn_callback, 
            s => s.PlayerEndTurn
        );
        input.TacticalCombat.EndTurn.performed += 
            (c => player_end_turn_callback());
}
{ // Branch which waits for the player to take some action
        tree.FirstOfPlayerActions = new FirstOfNext<TurnTree>(
            tree.DelayPlayerUnitMove, 
            tree.DelayPlayerEndTurn
        );
}
{ // Branch which triggers at the end of the player's turn.
        tree.PlayerEndTurn = new DoNext<TurnTree>(
            s => s.BetweenTurns
        );
        tree.PlayerEndTurn.OnCenterOn += 
            LogOnCenterOn("     End Player Turn");
}
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
