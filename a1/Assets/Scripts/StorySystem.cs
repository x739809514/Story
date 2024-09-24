using System;
using System.Collections;
using UnityEngine;
using XNode;
using Debug = UnityEngine.Debug;

public class StorySystem : MonoBehaviour
{
    // Fields
    public Panel_Story panelStory;
    public StoryGraph graph;
    private State curState;
    private Current current;
    private Node curNode;
    private int curDialogIndex; // 当前播放到第几段对话

    private float chatSpeed = 0.15f;
    private WaitForSeconds chatTimer;
    private bool auto; // 自动播放
    private bool toNext; // 点击显示完成对话内容

    public Action MoveToDialogEndHandler;
    public Action<SinglePersonChat, string> UpdateDialogHanler;
    public Action<int> UpdateOptionsHandler;

    private void Start()
    {
        EventHandle.AddEvent<SinglePersonChat, string>(EventName.EvtDialogExecute, UpdateDialog);
        EventHandle.AddEvent<int>(EventName.EvtOptionClick, OnOptionsClick);
        EventHandle.AddEvent(EventName.EvtDialogClick, OnDialogClick);
        InitNode();
    }

    private void Update()
    {
        if (curState == State.Play)
        {
            switch (current)
            {
                case Current.Start:
                    PlayStartNode();
                    break;
                case Current.Dialog:
                    PlayDialogNode(curDialogIndex);
                    break;
                case Current.Select:
                    PlaySelectNode();
                    break;
            }
        }
    }

    private void InitNode()
    {
        foreach (var node in graph.nodes)
        {
            if (node is StartNode n)
            {
                curNode = n;
                current = Current.Start;
            }
        }

        curState = State.Play;
    }


#region run nodes

    private void PlayStartNode()
    {
        if (curNode is StartNode startNode)
        {
            if (startNode.typeSound != null)
            {
                // Add music
                GameManager.Instance.audioManager.AddTypeMusic(startNode.typeSound);
            }

            // execute next node
            curNode = startNode.MoveNext(out current);
        }
        else
        {
            curState = State.Pause;
            Debug.LogError("curNode is not startNode, curNode is " + curNode.name + " current is " + nameof(current));
        }
    }

    private void PlayDialogNode(int index)
    {
        var dialogNode = curNode as DialogNode;
        if (dialogNode == null)
        {
            curState = State.Pause;
            Debug.LogError("curNode is not DialogNode, curNode is " + curNode.name + " current is " + nameof(current));
            return;
        }
        
        curNode = dialogNode.MoveNext(index, out current);
    }

    private void UpdateDialog(SinglePersonChat detail, string personName)
    {
        toNext = false;
        // 暂停状态，处理对话播放
        curState = State.Pause;
        // 开始一个计时器，计时结束后回到播放状态
        chatTimer = new WaitForSeconds(chatSpeed * detail.content.Length);
        StartCoroutine(SetDialogTimer());

        var type = detail.chatType;
        if (type == ChatType.CEvent)
        {
            curDialogIndex++;
            return;
        }

        // 处理UI部分
        panelStory.UpdateDialog(detail, personName);

        switch (type)
        {
            case ChatType.Normal:
                curDialogIndex++;
                break;
            case ChatType.Option:
            case ChatType.GoNext:
                curDialogIndex = 0;
                break;
        }
    }

    private void PlaySelectNode()
    {
        var selectNode = curNode as SelectNode;
        if (selectNode == null)
        {
            curState = State.Pause;
            Debug.LogError("curNode is not SelectNode, curNode is " + curNode.name + " current is " + nameof(current));
            return;
        }

        var options = selectNode.selections;
        panelStory.UpdateOptions(options);

        // pause, waiting for players' choice
        curState = State.Pause;
    }

    private void OnOptionsClick(int index)
    {
        Debug.Log("进来了");
        // move to next node
        if (curNode is SelectNode selectNode)
        {
            curDialogIndex = 0;
            curNode = selectNode.MoveNext(index, out current);
            curState = State.Play;
        }
        else
        {
            Debug.LogError("curNode is not SelectNode, curNode is " + curNode.name + " current is " + nameof(current));
        }
    }

#endregion


#region Method

    IEnumerator SetDialogTimer()
    {
        yield return chatTimer;
        // auto
        if (auto)
        {
            curState = State.Play;
        }
        else
        {
            toNext = true;
        }

        yield return null;
    }

    private void OnDialogClick()
    {
        if (toNext)
        {
            curState = State.Play;
        }
        else
        {
            MoveToDialogEndHandler?.Invoke();
            toNext = true;
        }
    }

#endregion


    private void OnDestroy()
    {
        EventHandle.RemoveEvent<SinglePersonChat, string>(EventName.EvtDialogExecute, UpdateDialog);
        EventHandle.RemoveEvent<int>(EventName.EvtOptionClick, OnOptionsClick);
        EventHandle.RemoveEvent(EventName.EvtDialogClick, OnDialogClick);
    }
}

public enum State
{
    Play,
    Pause
}