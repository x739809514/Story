using UnityEngine;
using XNode;

[CreateNodeMenu("BackgroundNode")]
[NodeWidth(300)]
[NodeTint(99, 79, 54)]
public class BackGroundNode : Node
{
    [Input] public Empty input;
    [Output] public Empty output;
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

        if (exitPort.IsConnected == false)
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
        else if (n is AnimNode animNode)
        {
            temp = animNode;
            current = Current.Animation;
        }
        else if (n is BackGroundNode backGroundNode)
        {
            temp = backGroundNode;
            current = Current.Background;
        }
        else if (n is AudioNode audioNode)
        {
            temp = audioNode;
            current = Current.Audio;
        }
        else if (n is EndNode endNode)
        {
            temp = endNode;
            current = Current.End;
        }
        else
        {
            Debug.LogError("BackgroundNode only can connect with dialogNode, animNode, backGroundNode and audioNode");
        }

        return temp;
    }
}