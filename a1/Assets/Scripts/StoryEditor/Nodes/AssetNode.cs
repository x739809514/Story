using XNode;

[CreateNodeMenu("AssetNode")]
public class AssetNode : Node
{
    public DialogAssets dialogAssets;
    
    public override object GetValue(NodePort port)
    {
        return null;
    }
}