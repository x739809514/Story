using UnityEngine;
using XNode;

[CreateNodeMenu("StartNode")]
[NodeWidth(200)]

[NodeTint(152,15,32)]//Node颜色
public class StartNode : Node
{
    [Output] public Empty output;
    
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
        
        BackGroundNode bNode = temp as BackGroundNode;
        if (bNode!=null)
        {
            current = Current.Background;
            return bNode;
        }
        
        AnimNode animNode = temp as AnimNode;
        if (animNode !=null)
        {
            current = Current.Animation;
            return animNode;
        }
        
        EndNode endNode = temp as EndNode;
        if (endNode!= null)
        {
            current = Current.End;
            return endNode;
        }

        return temp;
    }
}