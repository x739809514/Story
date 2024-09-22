using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogNode : Node
{
    public List<SinglePersonChat> chatList;

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

                var bb = chatList[i].dialogs;
                var head = chatList[i].icon;
                var character = chatList[i].name;
                EventHandle.DispatchEvent(EventName.EvtDialogExecute, cType, bb, head, character);
                return temp;
            }
        }

        return temp;
    }
}