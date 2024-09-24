using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateNodeMenu("OptionNode")]
[NodeWidth(400)]
[NodeTint(88,43,142)]
public class SelectNode : Node
{
    [Input] public Empty input;
    [Output(dynamicPortList =true )]
    public List<string> selections;

    public override object GetValue(NodePort port)
    {
        return null;
    }

    public Node MoveNext(int index, out Current current)
    {
        Node temp = this;
        current = Current.Select;
        foreach (var output in DynamicOutputs)
        {
            //Debug.Log(output.fieldName);
            if (output.fieldName == "selections " + index)
            {
                if (output.IsConnected)
                {
                    var nextNode = output.Connection.node;
                    if (nextNode is DialogNode dialogNode)
                    {
                        temp = dialogNode;
                        current = Current.Dialog;
                    }
                    else if (nextNode is SelectNode selectNode)
                    {
                        temp = selectNode;
                        current = Current.Select;
                    }
                    else if (nextNode is BackGroundNode backGroundNode)
                    {
                        temp = backGroundNode;
                        current = Current.Background;
                    }
                }
            }
        }

        return temp;
    }
}