using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateNodeMenu("AudioNode")]
[NodeWidth(400)]
public class AudioNode : Node
{
    [Input] public Empty input;
    [Output] public Empty output;

    public List<SoundAsset> soundLists;

    public override object GetValue(NodePort port)
    {
        return null;
    }

    public Node MoveNext(out Current current)
    {
        Node temp = this;
        current = Current.Audio;

        var exitNode = GetOutputPort("output");
        if (exitNode.IsConnected)
        {
            var nextNode = exitNode.Connection.node;
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
        }

        return temp;
    }
}