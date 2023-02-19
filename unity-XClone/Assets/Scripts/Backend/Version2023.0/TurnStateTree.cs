using System;
using System.Collections;
using System.Collections.Generic;

using StateTree;



public class UnitActionList{
    public GetBranch<TurnTree>[] ActionArray;

    public UnitActionList(params GetBranch<TurnTree>[] actions){
        ActionArray = actions;
    }
}