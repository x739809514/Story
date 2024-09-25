using UnityEngine;
using XNode;

[CreateNodeMenu("AnimNode")]
[NodeWidth(250)]
[NodeTint(34, 126, 114)]
public class AnimNode : Node
{
    [Input] public Empty input;
    [Output] public Empty output;
    public AnimationClip clip;

    public override object GetValue(NodePort port)
    {
        return null;
    }

    public Node MoveNext(out Current current)
    {
        Node temp = this;
        current = Current.Animation;

        var exitPort = GetOutputPort("output");
        if (exitPort.IsConnected)
        {
            var nextNode = exitPort.Connection.node;
            if (nextNode is DialogNode dialogNode)
            {
                temp = dialogNode;
                current = Current.Dialog;
            }
            else if (nextNode is SelectNode)
            {
                Debug.LogError("selectNode's previous must be dialogNode");
            }
            else if (nextNode is BackGroundNode backGroundNode)
            {
                temp = backGroundNode;
                current = Current.Background;
            }
            else if (nextNode is AudioNode audioNode)
            {
                temp = audioNode;
                current = Current.Audio;
            }
            else if (nextNode is AnimNode animNode)
            {
                temp = animNode;
                current = Current.Animation;
            }
            else if (nextNode is EndNode endNode)
            {
                temp = endNode;
                current = Current.End;
            }
        }

        return temp;
    }
}