using UnityEngine;
using XNode;

[CreateNodeMenu("StartNode")]
[NodeWidth(200)]

[NodeTint(255,255,60)]//Node颜色
public class StartNode : Node
{
    [Output] public Empty output;
    
    public AudioClip typeSound;
    public override object GetValue(NodePort port)
    {
        return null;
    }

    public Node MoveNext(out Current current)
    {
        NodePort exitPort = GetOutputPort("output");

        if (exitPort.IsConnected==false)
        {
            current = Current.Start;
            return this;
        }

        Node temp = exitPort.Connection.node;
        
        DialogNode dNode = temp as DialogNode;
        if (dNode!=null)
        {
            current = Current.Dialog;
            return dNode;
        }

        current = Current.Start;
        
        SelectNode sNode = temp as SelectNode;
        if (sNode!=null)
        {
            current = Current.Select;
            return sNode;
        }

        return temp;
    }
}