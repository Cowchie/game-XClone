namespace StateTree{

// The signature of a function which specializes a tree to one of its branches.
public delegate TreeBranch<TreeS> GetBranch<TreeS>(TreeS tree);

// The interface from which all tree branches derive. 
public interface TreeBranch<TreeS>{
    public void CenterOn(TreeBranch<TreeS> prev);
    public TreeBranch<TreeS> DoUpdate(TreeS tree);
}

public static class GenericTreeMethods{

    public static TreeBranch<TreeS> UpdateTree<TreeS>(
        TreeBranch<TreeS> prev_branch,  
        TreeS tree
    ){
        var curr_branch = prev_branch.DoUpdate(tree);
        if (curr_branch != prev_branch){
            curr_branch.CenterOn(prev_branch);
        }

        return curr_branch;
    }
    
}

// A branch which cycles through an array of branches
public class CycleBranches<TreeS> : TreeBranch<TreeS>{
    private GetBranch<TreeS>[] getsBranches;
    private int current_index;

    public CycleBranches(params GetBranch<TreeS>[] get_branch_array){
        getsBranches = get_branch_array;
        current_index = -1;
    }

    void TreeBranch<TreeS>.CenterOn(TreeBranch<TreeS> prev){
        current_index++;
        current_index = current_index < getsBranches.Length ? current_index : 0;
    }

    TreeBranch<TreeS> TreeBranch<TreeS>.DoUpdate(TreeS tree){
        return (getsBranches[current_index])(tree);
    }
}

// The signature of a function which is called everytime we center on CallbacksThenDoNext.
public delegate void WhenCenteredOn<TreeS>(
        TreeBranch<TreeS>    previous, 
        TreeBranch<TreeS>    current
    );
// A branch which calls a list of callbacks everytime it is centered on and then immediately moves on to another branch.
public class DoNext<TreeS> : TreeBranch<TreeS>{
    public event WhenCenteredOn<TreeS> OnCenterOn;

    private GetBranch<TreeS> next;
    public DoNext(GetBranch<TreeS> next_branch){
        next = next_branch;
    }

    void TreeBranch<TreeS>.CenterOn(TreeBranch<TreeS> prev){
        OnCenterOn?.Invoke(prev, this);
    }

    TreeBranch<TreeS> TreeBranch<TreeS>.DoUpdate(TreeS tree){
        return next(tree);
    }
}

// Subscribes to a callback and then will wait for that callback to be called before moving on to next.
public class DelayDoNext<TreeS> : TreeBranch<TreeS>{
    private GetBranch<TreeS> next;
    public DelayDoNext(
        out System.Action callback, 
        GetBranch<TreeS> next_branch
    ){
        callback = set_flag;
        next = next_branch;
    }

    private bool flag;
    void set_flag(){
        flag = true;
    }

    void TreeBranch<TreeS>.CenterOn(TreeBranch<TreeS> prev)
    {
        flag = false;
    }
    TreeBranch<TreeS> TreeBranch<TreeS>.DoUpdate(TreeS tree)
    {
        return flag ? next(tree) : this;
    }
}

}