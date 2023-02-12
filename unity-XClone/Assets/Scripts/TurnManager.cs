using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using StateTree;

public struct TurnTree{
    public UnitActionList           PlayerSelectedUnitActions;

    public CallDoNext<TurnTree>     StartBattle;
    public CycleBranches<TurnTree>  BetweenTurns;

    // Player Stuff
    public CallDoNext<TurnTree>     PlayerStartTurn;

    public DelayDoNext<TurnTree>    PlayerDelayCancel;
    public CallDoNext<TurnTree>     PlayerCancel;
    
    public CallDoNext<TurnTree>     PlayerStartChooseActions;
    public FirstOfNext<TurnTree>    PlayerFirstOfActions;
    public DelayDoNext<TurnTree>[]  PlayerDelayChooseUnitAction;
    public CallDoNext<TurnTree>[]   PlayerEndChooseUnitAction;
    
    public FirstOfNext<TurnTree>    PlayerFirstOfPickPosition;

    public CallDoNext<TurnTree>     PlayerStartChoosePosition;
    public DelayDoNext<TurnTree>    PlayerDelayChoosePosition;
    public CallDoNext<TurnTree>     PlayerEndChoosePosition;


    public DelayDoNext<TurnTree>    PlayerDelayEndTurn;
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

{ // Create a sample selected unit.
    GetBranch<TurnTree>[] get_branches = new GetBranch<TurnTree>[11];
    get_branches[0] = s => s.PlayerStartChooseActions;
    get_branches[1] = s => s.PlayerStartChoosePosition;
    for (int j = 2; j < get_branches.Length; j++){
        get_branches[j] = s => s.PlayerStartChooseActions;
    }
    tree.PlayerSelectedUnitActions = new UnitActionList(get_branches);
}

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

// Player's turn
{ // Branch which triggers at the start of the player's turn.
        tree.PlayerStartTurn = new CallDoNext<TurnTree>(
            s => s.PlayerStartChooseActions
        );
        tree.PlayerStartTurn.OnCenterOn += 
            LogOnCenterOn("Start Player Turn!");
}

{ // Branch which waits for the player to cancel a selection.
        tree.PlayerDelayCancel = new DelayDoNext<TurnTree>(
            out var player_cancel_callback, 
            s => s.PlayerCancel
        );
        Director.SetCancel(player_cancel_callback);
}
{ // Branch which triggers when the player chooses to cancel a selection.
        tree.PlayerCancel = new CallDoNext<TurnTree>(
            s => s.PlayerStartChooseActions
        );
        tree.PlayerCancel.OnCenterOn += 
            LogOnCenterOn("     Cancel Move");
}

    tree.PlayerDelayChooseUnitAction    = new DelayDoNext<TurnTree>[11];
    tree.PlayerEndChooseUnitAction      = new CallDoNext<TurnTree>[11];
{ // Branch which triggers at the start of the choose action step
        tree.PlayerStartChooseActions = new CallDoNext<TurnTree>(
            s => s.PlayerFirstOfActions
        );
        tree.PlayerStartChooseActions.OnCenterOn += 
            LogOnCenterOn("     Please choose an action...");
}
{ // Branch which waits for the first action the player chooses.
    GetBranch<TurnTree>[] get_branches = new GetBranch<TurnTree>[12];
    for (int j = 0; j < tree.PlayerDelayChooseUnitAction.Length; j++){
        // TODO: This is dumb.
        int k = j;
        get_branches[k] = s => s.PlayerDelayChooseUnitAction[k];
    }
    get_branches[11] = s => s.PlayerDelayEndTurn;

        tree.PlayerFirstOfActions = new FirstOfNext<TurnTree>(
            get_branches
        );
}
{ // Branch which waits for the player to choose action i.
    for (int i = 0; i < tree.PlayerDelayChooseUnitAction.Length; i++){
        // TODO: This is dumb.
        int k = i;
        tree.PlayerDelayChooseUnitAction[k] = new DelayDoNext<TurnTree>(
            out var player_choose_action_callback, 
            s => s.PlayerEndChooseUnitAction[k]
        );
        Director.SetActionKey(player_choose_action_callback, i);
    }
}
{ // Branch which triggers when the player has chosen an action.
    for (int i = 0; i < tree.PlayerEndChooseUnitAction.Length; i++){
        int k = i;
        tree.PlayerEndChooseUnitAction[i] = new CallDoNext<TurnTree>(
            tree.PlayerSelectedUnitActions.ActionArray[k]
        );

        tree.PlayerEndChooseUnitAction[k].OnCenterOn += LogOnCenterOn("Action " + k.ToString());
    }
}

{ // Branch which waits for the first position the player chooses.
        tree.PlayerFirstOfPickPosition = new FirstOfNext<TurnTree>(
            s => s.PlayerDelayChoosePosition, 
            s => s.PlayerDelayCancel,
            s => s.PlayerDelayEndTurn
        );
}

{ // Branch which triggers when player needs to choose a position.
        tree.PlayerStartChoosePosition = new CallDoNext<TurnTree>(
            s => s.PlayerFirstOfPickPosition
        );
        tree.PlayerStartChoosePosition.OnCenterOn += 
            LogOnCenterOn("     Choose a place for the unit to move!");
}
{ // Branch which waits for the player to select a position.
        tree.PlayerDelayChoosePosition = new DelayDoNext<TurnTree>(
            out var player_pick_position_callback, 
            s => s.PlayerEndChoosePosition
        );
        Director.SetPickPosition(
            Director.RaycastToGround, 
            player_pick_position_callback
        );
}
{ // Branch which triggers when the player choses a position.
        tree.PlayerEndChoosePosition = new CallDoNext<TurnTree>(
            s => s.PlayerStartChooseActions
        );
        tree.PlayerEndChoosePosition.OnCenterOn += 
            LogOnCenterOn("     Player picked a position!");
}

{ // Branch which waits for the player to press the end turn button.
        tree.PlayerDelayEndTurn = new DelayDoNext<TurnTree>(
            out var player_end_turn_callback, 
            s => s.PlayerEndTurn
        );
        Director.SetEndTurn(player_end_turn_callback);
}
{ // Branch which triggers at the end of the player's turn.
        tree.PlayerEndTurn = new CallDoNext<TurnTree>(
            s => s.BetweenTurns
        );
        tree.PlayerEndTurn.OnCenterOn += 
            LogOnCenterOn("End Player Turn");
}

// Enemy's turn
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
