using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateNodeMenu("DialogNode")]
[NodeWidth(400)]
[NodeTint(0, 82, 176)]
public class DialogNode : Node
{
    [Input] public Empty input;

    public List<SinglePersonChat> chatList = new List<SinglePersonChat>();
    public List<string> singlePersonName = new List<string>();

    public override object GetValue(NodePort port)
    {
        return null;
    }

    public Node MoveNext(int chatItemID, out Current current)
    {
        Node temp = this;
        current = Current.Dialog;
        for (int i = 0; i < chatList.Count; i++)
        {
            if (i == chatItemID)
            {
                ChatType cType = ChatType.Null;
                switch (chatList[i].chatType)
                {
                    case ChatType.Normal:
                        cType = ChatType.Normal;
                        current = Current.Dialog;
                        break;
                    case ChatType.CEvent:
                        cType = ChatType.CEvent;
                        current = Current.Dialog;
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
                                        current = Current.Select;
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
                                    if (port.Connection.node is DialogNode node)
                                    {
                                        temp = node;
                                        current = Current.Dialog;
                                    }
                                    else if (port.Connection.node is BackGroundNode bNode)
                                    {
                                        temp = bNode;
                                        current = Current.Background;
                                    }else if (port.Connection.node is AudioNode aNode)
                                    {
                                        temp = aNode;
                                        current = Current.Audio;
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
                var pos = chatList[i].pos;
                EventHandle.DispatchEvent<SinglePersonChat,string>(EventName.EvtDialogExecute, chatList[i],character);
                return temp;
            }
        }

        return temp;
    }
}