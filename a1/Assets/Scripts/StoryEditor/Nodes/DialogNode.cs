using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateNodeMenu("DialogNode")]
[NodeWidth(400)]
[NodeTint(73,236,209)]
public class DialogNode : Node
{
    [Input]
    public Empty input;
    
    public List<SinglePersonChat> chatList = new List<SinglePersonChat>();
    public List<string> singlePersonName = new List<string>();

    public override object GetValue(NodePort port)
    {
        return null;
    }

    public Node MoveNext(int chatItemID)
    {
        Node temp = this;
        for (int i = 0; i < chatList.Count; i++)
        {
            if (i == chatItemID)
            {
                ChatType cType = ChatType.Null;
                switch (chatList[i].chatType)
                {
                    case ChatType.Normal:
                        cType = ChatType.Normal;
                        break;
                    case ChatType.CEvent:
                        cType = ChatType.CEvent;
                        var content = chatList[i].content;
                        EventHandle.DispatchEvent(EventName.EvtDialog, content);
                        break;
                    case ChatType.Option:
                        cType = ChatType.Option;
                        foreach (var port in DynamicPorts)
                        {
                            if (port.fieldName == "chatList" + " " + i.ToString())
                            {
                                if (port.IsConnected)
                                {
                                    if (port.Connection.node is SelectNode node)
                                    {
                                        temp = node;
                                    }
                                    else
                                    {
                                        Debug.LogError(name + "的结尾有option，只能连接selectNode");
                                    }
                                }

                                return temp;
                            }
                        }

                        break;
                    case ChatType.GoNext:
                        cType = ChatType.GoNext;
                        foreach (var port in DynamicPorts)
                        {
                            if (port.fieldName == "chatList" + " " + i.ToString())
                            {
                                if (port.IsConnected)
                                {
                                    if (temp is DialogNode node)
                                    {
                                        temp = node;
                                    }
                                }

                                return temp;
                            }
                        }

                        break;
                }

                var bb = chatList[i].content;
                var head = chatList[i].icon;
                var character = singlePersonName[chatList[i].name];
                EventHandle.DispatchEvent(EventName.EvtDialogExecute, cType, bb, head, character);
                return temp;
            }
        }

        return temp;
    }
}