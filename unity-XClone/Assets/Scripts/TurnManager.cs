using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using StateTree;

public struct TurnTree{
    public CallDoNext<TurnTree>     StartBattle;
    public CycleBranches<TurnTree>  BetweenTurns;
    public CallDoNext<TurnTree>     PlayerStartTurn;
    public DelayDoNext<TurnTree>    DelayPlayerUnitMove;
    public CallDoNext<TurnTree>     PlayerStartUnitMove;
    public DelayDoNext<TurnTree>    DelayPlayerEndTurn;
    public FirstOfNext<TurnTree>    FirstOfPlayerActions;
    public CallDoNext<TurnTree>     PlayerEndTurn;
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

{ // Branch which triggers at the start of the battle.
        tree.StartBattle = new CallDoNext<TurnTree>(
            s => s.BetweenTurns
        );
        tree.StartBattle.OnCenterOn += 
            LogOnCenterOn("Start Battle!");
}
{ // Branch which cycles between the turns.
        tree.BetweenTurns = new CycleBranches<TurnTree>(
            s => s.PlayerStartTurn
        );
}
{ // Branch which triggers at the start of the player's turn.
        tree.PlayerStartTurn = new CallDoNext<TurnTree>(
            s => s.FirstOfPlayerActions
        );
        tree.PlayerStartTurn.OnCenterOn += 
            LogOnCenterOn("     Start Player Turn! Please choose an action...");
}
{ // Branch which waits for the player to press the button to press unit move.
        tree.DelayPlayerUnitMove = new DelayDoNext<TurnTree>(
            out var player_unit_move_callback, 
            s => s.PlayerStartUnitMove
        );
        input.TacticalCombat.UnitMove.performed += 
            (c => player_unit_move_callback());
}
{ // Branch which triggers when player chooses to move
        tree.PlayerStartUnitMove = new CallDoNext<TurnTree>(
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
            s => s.DelayPlayerUnitMove, 
            s => s.DelayPlayerEndTurn
        );
}
{ // Branch which triggers at the end of the player's turn.
        tree.PlayerEndTurn = new CallDoNext<TurnTree>(
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
            current_turn.CenterOn(tree.StartBattle, tree);
        }

        var prev_branch = current_turn;
        current_turn = current_turn.DoUpdate(tree);
        if (current_turn != prev_branch){
            current_turn.CenterOn(prev_branch, tree);
        }
    }

    private WhenCenteredOn<TurnTree> LogOnCenterOn(
        string s
    ){
        return (_a, _b) => Debug.Log(s);
    }
}
