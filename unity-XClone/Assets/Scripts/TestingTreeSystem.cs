using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using StateTree;

namespace TestingStuff{

struct TestingTree{
    public CycleBranches<TestingTree> cycleBetweenAandB;
    public DoNext<TestingTree> branchA;
    public DoNext<TestingTree> afterBranchA;
    public DoNext<TestingTree> branchB;
    public DoNext<TestingTree> branchC;
}

public class TestingTreeSystem : MonoBehaviour
{
    TestingTree tree;
    TreeBranch<TestingTree> current_branch;

    // Start is called before the first frame update
    void Start()
    {
        tree = new TestingTree();
        tree.cycleBetweenAandB = new CycleBranches<TestingTree>(
                s => s.branchA, 
                s => s.branchB, 
                s => s.branchC
            );
        tree.branchA        = new DoNext<TestingTree>
            (s => s.afterBranchA);
        tree.afterBranchA   = new DoNext<TestingTree>
            (s => s.cycleBetweenAandB);
        tree.branchB        = new DoNext<TestingTree>
            (s => s.cycleBetweenAandB);
        tree.branchC        = new DoNext<TestingTree>
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

    private void when_A(TreeBranch<TestingTree> prev, TreeBranch<TestingTree> curr){
        Debug.Log("We just switched into A");
        Debug.Log("     Came from:      " + prev);
        Debug.Log("     Currently on:   " + curr);
    }

    private void when_after_A(TreeBranch<TestingTree> prev, TreeBranch<TestingTree> curr){
        Debug.Log("We just switched into AfterA");
        Debug.Log("     Came from:      " + prev);
        Debug.Log("     Currently on:   " + curr);
    }

    private void when_B(TreeBranch<TestingTree> prev, TreeBranch<TestingTree> curr){
        Debug.Log("We just switched into B");
        Debug.Log("     Came from:      " + prev);
        Debug.Log("     Currently on:   " + curr);
    }

    private void when_C(TreeBranch<TestingTree> prev, TreeBranch<TestingTree> curr){
        Debug.Log("We just switched into C");
        Debug.Log("     Came from:      " + prev);
        Debug.Log("     Currently on:   " + curr);
    }
}
}