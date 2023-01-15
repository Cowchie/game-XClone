namespace StateTree{

public interface TreeBranch{
    public void CenterOn(TreeBranch prev);
    public TreeBranch DoUpdate();
}

public class CycleBranches : TreeBranch{
    private TreeBranch[] branches;
    private int current_index;

    public CycleBranches(params TreeBranch[] branch_array){
        branches = branch_array;
        current_index = -1;
    }

    void TreeBranch.CenterOn(TreeBranch prev){
        current_index++;
        current_index = current_index < branches.Length ? current_index : 0;
    }

    TreeBranch TreeBranch.DoUpdate(){
        return branches[current_index];
    }
}

public delegate void WhenCenteredOnDelegate(
        TreeBranch previous, 
        TreeBranch current
    );
public class CallbacksThenDoNext : TreeBranch{
    public event WhenCenteredOnDelegate OnCenterOn;

    private TreeBranch next;
    public CallbacksThenDoNext(TreeBranch next_branch){
        next = next_branch;
    }

    void TreeBranch.CenterOn(TreeBranch prev){
        OnCenterOn?.Invoke(prev, this);
    }

    TreeBranch TreeBranch.DoUpdate(){
        return next;
    }
}

}