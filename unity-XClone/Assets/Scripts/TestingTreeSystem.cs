using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using StateTree;

struct Tree{
    public CycleBranches<Tree> cycleBetweenAandB;
    public CallbacksThenDoNext<Tree> branchA;
    public CallbacksThenDoNext<Tree> afterBranchA;
    public CallbacksThenDoNext<Tree> branchB;
    public CallbacksThenDoNext<Tree> branchC;
}


public class TestingTreeSystem : MonoBehaviour
{
    Tree tree;
    TreeBranch<Tree> current_branch;

    // Start is called before the first frame update
    void Start()
    {
        tree = new Tree();
        tree.cycleBetweenAandB = new CycleBranches<Tree>(
                s => s.branchA, 
                s => s.branchB, 
                s => s.branchC
            );
        tree.branchA        = new CallbacksThenDoNext<Tree>
            (s => s.afterBranchA);
        tree.afterBranchA   = new CallbacksThenDoNext<Tree>
            (s => s.cycleBetweenAandB);
        tree.branchB        = new CallbacksThenDoNext<Tree>
            (s => s.cycleBetweenAandB);
        tree.branchC        = new CallbacksThenDoNext<Tree>
            (s => s.branchC);
        
        tree.branchA.OnCenterOn += when_A;
        tree.afterBranchA.OnCenterOn += when_after_A;
        tree.branchB.OnCenterOn += when_B;
        tree.branchC.OnCenterOn += when_C;
    }

    // Update is called once per frame
    void Update()
    {
        if (current_branch is null){
            current_branch = tree.cycleBetweenAandB;
            current_branch.CenterOn(tree.cycleBetweenAandB);
        }
        var prev_branch = current_branch;
        current_branch = current_branch.DoUpdate(tree);
        if (current_branch != prev_branch){
            current_branch.CenterOn(prev_branch);
        }
    }

    private void when_A(TreeBranch<Tree> prev, TreeBranch<Tree> curr){
        Debug.Log("We just switched into A");
        Debug.Log("     Came from:      " + prev);
        Debug.Log("     Currently on:   " + curr);
    }

    private void when_after_A(TreeBranch<Tree> prev, TreeBranch<Tree> curr){
        Debug.Log("We just switched into AfterA");
        Debug.Log("     Came from:      " + prev);
        Debug.Log("     Currently on:   " + curr);
    }

    private void when_B(TreeBranch<Tree> prev, TreeBranch<Tree> curr){
        Debug.Log("We just switched into B");
        Debug.Log("     Came from:      " + prev);
        Debug.Log("     Currently on:   " + curr);
    }

    private void when_C(TreeBranch<Tree> prev, TreeBranch<Tree> curr){
        Debug.Log("We just switched into C");
        Debug.Log("     Came from:      " + prev);
        Debug.Log("     Currently on:   " + curr);
    }
}
