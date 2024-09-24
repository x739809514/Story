using UnityEngine;
using UnityEngine.Serialization;
using XNode;

[CreateNodeMenu("BackgroundNode")]
[NodeWidth(300)]
[NodeTint(99,79,54)]
public class BackGroundNode : Node
{
    [Input] public Empty input;
    [Output] public Empty output;
    public AudioClip bgm;
    public Sprite img;

    public override object GetValue(NodePort port)
    {
        return null;
    }

    public Node MoveNext(out Current current)
    {
        Node temp = this;
        current = Current.Background;
        NodePort exitPort = GetOutputPort("output");

        if (exitPort.IsConnected==false)
        {
            current = Current.Background;
            return temp;
        }

        var n = exitPort.Connection.node;
        if (n is DialogNode dialogNode)
        {
            temp = dialogNode;
            current = Current.Dialog;
        }

        if (n is SelectNode selectNode)
        {
            temp = selectNode;
            current = Current.Select;
        }

        if (n is BackGroundNode backGroundNode)
        {
            temp = backGroundNode;
            current = Current.Background;
        }
        
        return temp;
    }
}