using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XNode;
using XNodeEditor;

[CustomNodeEditor(typeof(DialogNode))]
public class DialogNodeEditor : NodeEditor
{
    private DialogNode dialogNode;
    private StoryGraph storyGraph;
    private DialogAssets dAssets;
    private readonly string fieldName = "chatList";
    private SerializedProperty iconProperty;

    private void DrawList(ReorderableList list)
    {
        if (dialogNode == null)
        {
            dialogNode = target as DialogNode;
        }

        if (storyGraph == null)
        {
            storyGraph = window.graph as StoryGraph;
        }

        dAssets = storyGraph.GetAssets();
        if (dAssets != null)
        {
            foreach (var person in dAssets.dialogPersons)
            {
                dialogNode.singlePersonName.Add(person.name);
            }
        }

        SerializedProperty arrayData = serializedObject.FindProperty(fieldName);
        bool hasArrayData = arrayData != null && arrayData.isArray;

        // 绘制列表头
        list.drawHeaderCallback = (Rect rect) =>
        {
            string title = "Dialog List";
            GUI.Label(rect, title);
        };

        list.drawElementCallback = ((rect, index, active, focused) =>
        {
            Node node = serializedObject.targetObject as Node;
            NodePort port = node.GetPort(fieldName + " " + index);

            // 渲染动态节点
            EditorGUI.LabelField(rect, "");
            if (port != null && (dialogNode.chatList[index].chatType == ChatType.Option ||
                                 dialogNode.chatList[index].chatType == ChatType.GoNext))
            {
                Vector2 pos = rect.position + (port.IsOutput ? new Vector2(rect.width + 6, 0) : new Vector2(-36, 0));
                NodeEditorGUILayout.PortField(pos, port);
            }


#region 定义每个元素的位置

            Rect position = rect;
            position.height = EditorGUIUtility.singleLineHeight;

            SerializedProperty itemData = arrayData.GetArrayElementAtIndex(index);
            itemData.isExpanded = false;
            //Debug.Log($"Element {index} Height: {EditorGUI.GetPropertyHeight(itemData)} | Type: {itemData.propertyType} | IsExpanded: {itemData.isExpanded}");
            
            // CEvent
            if (dialogNode.chatList[index].chatType == ChatType.CEvent)
            {
                Rect typeRect = new Rect(position)
                {
                    width = 130,
                    y = position.y + 5,
                    height = 20
                };

                Rect methodNameRect = new Rect(typeRect)
                {
                    width = position.width - 140,
                    x = position.x + 140,
                    height = 20
                };

                SerializedProperty content_Timeproperty = itemData.FindPropertyRelative("content");
                SerializedProperty typeProperty = itemData.FindPropertyRelative("chatType");


                content_Timeproperty.stringValue = EditorGUI.TextArea(methodNameRect, content_Timeproperty.stringValue);
                var ty = (ChatType)EditorGUI.EnumPopup(typeRect, dialogNode.chatList[index].chatType);

                typeProperty.enumValueIndex = (int)ty;
            }
            else if (dialogNode.chatList[index].chatType == ChatType.Option)
            {
                Rect typeRect = new Rect(position)
                {
                    width = 130,
                    y = position.y + 5,
                    height = 20
                };
                SerializedProperty typeProperty = itemData.FindPropertyRelative("chatType");
                typeProperty.enumValueIndex = (int)(ChatType)EditorGUI.EnumPopup(typeRect, dialogNode.chatList[index].chatType);
            }
            else
            {
                Rect iconRect = new Rect(position)
                {
                    width = 50,
                    height = 50,
                    y=position.y+8
                };

                Rect nameTypeRect = new Rect(position)
                {
                    width = position.width - 270,
                    x = position.x + 60,
                    y=position.y+8
                };

                Rect typeRect = new Rect(nameTypeRect)
                {
                    width = position.width - 270,
                    y = nameTypeRect.y + EditorGUIUtility.singleLineHeight + 4,
                    height = 20
                };
                Rect posRect = new Rect(typeRect)
                {
                    width = position.width - 270,
                    y = typeRect.y + EditorGUIUtility.singleLineHeight + 4,
                    height = 20
                };
                
                Rect contentRect = new Rect(position)
                {
                    width = position.width - 140,
                    height = 64,
                    x = position.x + 140,
                    y=position.y+8
                };
                iconProperty = itemData.FindPropertyRelative("icon");
                SerializedProperty nameProperty = itemData.FindPropertyRelative("name");
                SerializedProperty contentProperty = itemData.FindPropertyRelative("content");
                SerializedProperty typeProperty = itemData.FindPropertyRelative("chatType");
                SerializedProperty posProperty = itemData.FindPropertyRelative("pos");

                iconProperty.objectReferenceValue =
                    EditorGUI.ObjectField(iconRect, iconProperty.objectReferenceValue, typeof(Sprite), false);


                SinglePersonChat temp = dialogNode.chatList[index];

                nameProperty.intValue = EditorGUI.Popup(nameTypeRect, temp.name, dialogNode.singlePersonName.ToArray());
                
                typeProperty.enumValueIndex = (int)(ChatType)EditorGUI.EnumPopup(typeRect, temp.chatType);
                posProperty.enumValueIndex = (int)(RolePos)EditorGUI.EnumPopup(posRect, temp.pos);

                contentProperty.stringValue = EditorGUI.TextArea(contentRect, contentProperty.stringValue);
            }

#endregion
        });
        
        list.elementHeightCallback =(int index) => {
            //Debug.Log("CCCCCCCCCCCCCCCCC");
            if (hasArrayData)
            {
                if (arrayData.arraySize <= index)
                {
                    return EditorGUIUtility.singleLineHeight ;
                }
                SerializedProperty itemData = arrayData.GetArrayElementAtIndex(index);

                if (dialogNode.chatList[index].chatType == ChatType.CEvent || dialogNode.chatList[index].chatType == ChatType.Option)
                {
                    return EditorGUI.GetPropertyHeight(itemData) + 12;
                }

                //return EditorGUIUtility.singleLineHeight + 36;
                return EditorGUI.GetPropertyHeight(itemData)+54;
            }
            else return EditorGUIUtility.singleLineHeight;
        };
    }

    public override void OnBodyGUI()
    {
        if (dialogNode==null)
        {
            dialogNode = target as DialogNode;
        }
        
        foreach (XNode.NodePort Port in target.Ports)
        {
            if (NodeEditorGUILayout.IsDynamicPortListPort(Port)) continue;
            NodeEditorGUILayout.PortField(Port);
        }

        bool forceReset = false;
            
        if (dAssets != null)
        {
            dAssets = storyGraph.GetAssets();
            bool isNameEqual = true;
            if (dialogNode.singlePersonName.Count == dAssets.dialogPersons.Count)
            {
                for (int i = 0; i < dAssets.dialogPersons.Count; i++)
                {
                    if (dialogNode.singlePersonName[i] != dAssets.dialogPersons[i].name)
                    {
                        isNameEqual = false;
                        dialogNode.singlePersonName[i] = dAssets.dialogPersons[i].name;
                    }
                }

                if (!isNameEqual)
                {
                       
                    forceReset = true;
                }
            }
            else
            {
                dialogNode.singlePersonName.Clear();

                forceReset = true;
            }
        }
            
        NodeEditorGUILayout.DynamicPortList(fieldName,typeof(SinglePersonChat),serializedObject,NodePort.IO.Output,Node.ConnectionType.Override,Node.TypeConstraint.None,DrawList,forceReset);
    }
}