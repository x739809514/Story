using System.Collections.Generic;
using UnityEngine;

public enum Current
{
    Null,
    Start,
    Dialog,
    Select,
}

public enum ChatType
{
    Null,
    
    /// <summary>
    /// 普通对话模式
    /// </summary>
    Normal,
    
    /// <summary>
    /// 事件模式，可以用于对话中触发一些事件
    /// </summary>
    CEvent,
    
    /// <summary>
    /// 选项模式，在对话播放完之后弹出选项框，必须要位于chatlist末尾
    /// </summary>
    Option,
    
    /// <summary>
    /// 跳转下一段对话，必须要位于chatlist末尾
    /// </summary>
    GoNext
}

[System.Serializable]
public class SinglePersonChat
{
    public int name;
    public string content;
    [SerializeField]
    public List<string> dialogs;
    public ChatType chatType;
    public Sprite icon;
}