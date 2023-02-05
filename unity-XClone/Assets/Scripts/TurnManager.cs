using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using StateTree;

public struct UnitActionTree{
    public CallDoNext<TurnTree>     StartChooseAction;
    public DelayDoNext<TurnTree>    DelayChooseAction;
    public CallDoNext<TurnTree>     EndChooseAction;
}

public struct TurnTree{
    public CallDoNext<TurnTree>     StartBattle;
    public CycleBranches<TurnTree>  BetweenTurns;

    // Player Stuff
    public CallDoNext<TurnTree>     PlayerStartTurn;
    public CallDoNext<TurnTree>     PlayerStartChooseAction;
    public DelayDoNext<TurnTree>    DelayPlayerChooseAction;
    public CallDoNext<TurnTree>     PlayerStartUnitMove;
    public DelayDoNext<TurnTree>    PlayerDelayPickPosition;
    public CallDoNext<TurnTree>     PlayerPickedPosition;
    public DelayDoNext<TurnTree>    PlayerDelayCancelMove;
    public CallDoNext<TurnTree>     PlayerCancelMove;
    public FirstOfNext<TurnTree>    PlayerFirstOfPickPositionOrCancel;
    public DelayDoNext<TurnTree>    PlayerDelayEndTurn;
    public FirstOfNext<TurnTree>    PlayerFirstOfActions;
    public CallDoNext<TurnTree>     PlayerEndTurn;

    // Enemy AI stuff
    public CallDoNext<TurnTree>     EnemyStartTurn;
    public DelayDoNext<TurnTree>    EnemyDelayTurn;
    public CallDoNext<TurnTree>     EnemyEndTurn;
}

public class TurnManager : MonoBehaviour
{
    public PlayerDirector Director;

    public static TurnTree tree;
    TreeBranch<TurnTree> current_turn;

    // Awake is called before Start is called before the first frame update
    void Awake(){
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
            s => s.PlayerStartTurn,
            s => s.EnemyStartTurn
        );
}
{ // Branch which triggers at the start of the player's turn.
        tree.PlayerStartTurn = new CallDoNext<TurnTree>(
            s => s.PlayerStartChooseAction
        );
        tree.PlayerStartTurn.OnCenterOn += 
            LogOnCenterOn("Start Player Turn!");
}
{ // Branch which triggers at the start of the choose action step
        tree.PlayerStartChooseAction = new CallDoNext<TurnTree>(
            s => s.PlayerFirstOfActions
        );
        tree.PlayerStartChooseAction.OnCenterOn += 
            LogOnCenterOn("     Please choose an action...");
}
{ // Branch which waits for the player to choose an action.
        tree.DelayPlayerChooseAction = new DelayDoNext<TurnTree>(
            out var player_unit_move_callback, 
            s => s.PlayerStartUnitMove
        );
        Director.SetUnitMove(player_unit_move_callback);
}
{ // Branch which triggers when player chooses to move.
        tree.PlayerStartUnitMove = new CallDoNext<TurnTree>(
            s => s.PlayerFirstOfPickPositionOrCancel
        );
        tree.PlayerStartUnitMove.OnCenterOn += 
            LogOnCenterOn("     Choose a place for the unit to move!");
}
{ // Branch which waits for the player to select a position.
        tree.PlayerDelayPickPosition = new DelayDoNext<TurnTree>(
            out var player_pick_position_callback, 
            s => s.PlayerPickedPosition
        );
        Director.SetPickPosition(player_pick_position_callback);
}
{ // Branch which triggers when the player choses a position.
        tree.PlayerPickedPosition = new CallDoNext<TurnTree>(
            s => s.PlayerStartChooseAction
        );
        tree.PlayerPickedPosition.OnCenterOn += 
            LogOnCenterOn("     Player picked a position!");
}
{ // Branch which waits for the player to cancel the current move.
        tree.PlayerDelayCancelMove = new DelayDoNext<TurnTree>(
            out var player_cancel_callback, 
            s => s.PlayerCancelMove
        );
        Director.SetCancel(player_cancel_callback);
}
{ // Branch which triggers when the player chooses to cancel the move.
        tree.PlayerCancelMove = new CallDoNext<TurnTree>(
            s => s.PlayerStartChooseAction
        );
        tree.PlayerCancelMove.OnCenterOn += 
            LogOnCenterOn("     Cancel Move");
}
{ // Branch which waits for the player to either select a position, cancel, or end turn.
        tree.PlayerFirstOfPickPositionOrCancel = new FirstOfNext<TurnTree>(
            s => s.PlayerDelayPickPosition, 
            s => s.PlayerDelayCancelMove,
            s => s.PlayerDelayEndTurn
        );
}
{ // Branch which waits for the player to press the end turn button.
        tree.PlayerDelayEndTurn = new DelayDoNext<TurnTree>(
            out var player_end_turn_callback, 
            s => s.PlayerEndTurn
        );
        Director.SetEndTurn(player_end_turn_callback);
}
{ // Branch which waits for the player to take some action.
        tree.PlayerFirstOfActions = new FirstOfNext<TurnTree>(
            s => s.DelayPlayerChooseAction, 
            s => s.PlayerDelayEndTurn
        );
}
{ // Branch which triggers at the end of the player's turn.
        tree.PlayerEndTurn = new CallDoNext<TurnTree>(
            s => s.BetweenTurns
        );
        tree.PlayerEndTurn.OnCenterOn += 
            LogOnCenterOn("End Player Turn");
}
    
{ // Branch which triggers at the start of the enemy turn.
        tree.EnemyStartTurn = new CallDoNext<TurnTree>(
            s => s.EnemyDelayTurn
        );
        tree.EnemyStartTurn.OnCenterOn +=
            LogOnCenterOn("Start Enemy Turn!");
}
{ // Branch which simulates the enemy AI turn (does nothing for < 5 seconds)
        tree.EnemyDelayTurn = new DelayDoNext<TurnTree>(
            out var enemy_delay_turn_callback, 
            s => s.EnemyEndTurn
        );
        enemyDelayTurnCallback = enemy_delay_turn_callback;
}
{ // Branch which triggers at the end of the enemy turn.
        tree.EnemyEndTurn = new CallDoNext<TurnTree>(
            s => s.BetweenTurns
        );
        tree.EnemyEndTurn.OnCenterOn +=
            LogOnCenterOn("End Enemy Turn!");
}
    }

    private System.Action enemyDelayTurnCallback;
    float counter = 0f;

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

        if (counter > 5f){
            counter = 0f;
            enemyDelayTurnCallback?.Invoke();
        }
        counter += Time.deltaTime;
    }

    private WhenCenteredOn<TurnTree> LogOnCenterOn(
        string s
    ){
        return (_a, _b) => Debug.Log(s);
    }



}
