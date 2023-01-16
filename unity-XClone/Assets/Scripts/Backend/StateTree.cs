namespace StateTree{

// The signature of a function which specializes a tree to one of its branches.
public delegate TreeBranch<TreeS> GetBranch<TreeS>(TreeS tree);

// The interface from which all tree branches derive. 
public interface TreeBranch<TreeS>{
    public void CenterOn(GetBranch<TreeS> prev, TreeS tree);
    public TreeBranch<TreeS> DoUpdate(TreeS tree);
}

// A branch which cycles through an array of branches
public class CycleBranches<TreeS> : TreeBranch<TreeS>{
    private GetBranch<TreeS>[] getsBranches;
    private int current_index;

    public CycleBranches(params GetBranch<TreeS>[] get_branch_array){
        getsBranches = get_branch_array;
        current_index = -1;
    }

    void TreeBranch<TreeS>.CenterOn(GetBranch<TreeS> prev, TreeS tree){
        current_index++;
        current_index = current_index < getsBranches.Length ? current_index : 0;
    }

    TreeBranch<TreeS> TreeBranch<TreeS>.DoUpdate(TreeS tree){
        return (getsBranches[current_index])(tree);
    }
}

// The signature of a function which is called everytime we center on CallbacksThenDoNext.
public delegate void WhenCenteredOn<TreeS>(
        GetBranch<TreeS>    previous, 
        GetBranch<TreeS>    current,
        TreeS               tree
    );
// A branch which calls a list of callbacks everytime it is centered on and then immediately moves on to another branch.
public class CallbacksThenDoNext<TreeS> : TreeBranch<TreeS>{
    public event WhenCenteredOn<TreeS> OnCenterOn;

    private GetBranch<TreeS> get_this;
    private GetBranch<TreeS> get_next;
    public CallbacksThenDoNext(
        GetBranch<TreeS> this_branch, 
        GetBranch<TreeS> next_branch
    ){
        get_this = this_branch;
        get_next = next_branch;
    }

    void TreeBranch<TreeS>.CenterOn(GetBranch<TreeS> prev, TreeS tree){
        OnCenterOn?.Invoke(prev, get_this, tree);
    }

    TreeBranch<TreeS> TreeBranch<TreeS>.DoUpdate(TreeS tree){
        return get_next(tree);
    }
}

}