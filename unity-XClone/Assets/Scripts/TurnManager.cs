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
    public DelayDoNext<TurnTree>    DelayPlayerPickPosition;
    public CallDoNext<TurnTree>     PlayerPickedPosition;
    public DelayDoNext<TurnTree>    DelayPlayerCancelMove;
    public CallDoNext<TurnTree>     PlayerCancelMove;
    public FirstOfNext<TurnTree>    FirstOfPlayerPickPositionOrCancel;
    public DelayDoNext<TurnTree>    DelayPlayerEndTurn;
    public FirstOfNext<TurnTree>    FirstOfPlayerActions;
    public CallDoNext<TurnTree>     PlayerEndTurn;
}

public class TurnManager : MonoBehaviour
{
    public PlayerDirector Director;

    private TurnTree tree;
    TreeBranch<TurnTree> current_turn;

    // Start is called before the first frame update
    void Start(){
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
        Director.SetUnitMove(player_unit_move_callback);
}
{ // Branch which triggers when player chooses to move
        tree.PlayerStartUnitMove = new CallDoNext<TurnTree>(
            s => s.FirstOfPlayerPickPositionOrCancel
        );
        tree.PlayerStartUnitMove.OnCenterOn += 
            LogOnCenterOn("     Choose a place for the unit to move!");
}
{ // Branch which waits for the player to select a position.
        tree.DelayPlayerPickPosition = new DelayDoNext<TurnTree>(
            out var player_pick_position_callback, 
            s => s.PlayerPickedPosition
        );
        Director.SetPickPosition(player_pick_position_callback);
}
{ // Branch which triggers when the player choses a position.
        tree.PlayerPickedPosition = new CallDoNext<TurnTree>(
            s => s.PlayerStartUnitMove
        );
        tree.PlayerPickedPosition.OnCenterOn += 
            LogOnCenterOn("     Player picked a position!");
}
{ // Branch which waits for the player to cancel the current move.
        tree.DelayPlayerCancelMove = new DelayDoNext<TurnTree>(
            out var player_cancel_callback, 
            s => s.PlayerCancelMove
        );
        Director.SetCancel(player_cancel_callback);
}
{ // Branch which triggers when the player chooses to cancel the move
        tree.PlayerCancelMove = new CallDoNext<TurnTree>(
            s => s.FirstOfPlayerActions
        );
        tree.PlayerCancelMove.OnCenterOn += 
            LogOnCenterOn("     Cancel Move");
}
{ // Branch which waits for the player to either select a position, cancel, or end turn
        tree.FirstOfPlayerPickPositionOrCancel = new FirstOfNext<TurnTree>(
            s => s.DelayPlayerPickPosition, 
            s => s.DelayPlayerCancelMove,
            s => s.DelayPlayerEndTurn
        );
}
{ // Branch which waits for the player to press the end turn button.
        tree.DelayPlayerEndTurn = new DelayDoNext<TurnTree>(
            out var player_end_turn_callback, 
            s => s.PlayerEndTurn
        );
        Director.SetEndTurn(player_end_turn_callback);
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
            // Debug.Log(current_turn);
            current_turn.CenterOn(prev_branch, tree);
        }
    }

    private WhenCenteredOn<TurnTree> LogOnCenterOn(
        string s
    ){
        return (_a, _b) => Debug.Log(s);
    }
}
