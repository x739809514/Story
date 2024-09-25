using XNode;

[CreateNodeMenu("EndNode")]
[NodeTint(4, 0, 150)]
public class EndNode : Node
{
    [Input] public Empty input;

    public override object GetValue(NodePort port)
    {
        return null;
    }
}