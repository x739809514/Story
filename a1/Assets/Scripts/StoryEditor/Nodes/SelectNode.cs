using System.Collections.Generic;
using UnityEngine;
using XNode;

public class SelectNode : Node
{
    [SerializeField]
    public List<string> selections;
    public override object GetValue(NodePort port)
    {
        return null;
    }

    public Node MoveNext(int index,out Current current)
    {
        Node temp = this;
        
        current = Current.Select;
        return temp;
    }
}