using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Debug = UnityEngine.Debug;

public class StorySystem : MonoBehaviour
{
    // Fields
    public static StorySystem Instance;
    public StoryGraph graph;
    private State curState;
    private Current current;
    private Node curNode;
    private int curDialogIndex; // 当前播放到第几段对话

    private float chatSpeed = 0.15f;
    private WaitForSeconds chatTimer;
    private bool auto; // 自动播放
    private bool toNext; // 点击显示完成对话内容

    // 对外API
    public Action moveToDialogEndHandler;
    public Action<SinglePersonChat, string> updateDialogHandler;
    public Action<List<string>> updateOptionsHandler;
    public Action<Sprite> updateBackgroundHandler;


#region life cycle

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }

        Instance = this;
    }

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
                case Current.Background:
                    PlayBackGroundNode();
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        EventHandle.RemoveEvent<SinglePersonChat, string>(EventName.EvtDialogExecute, UpdateDialog);
        EventHandle.RemoveEvent<int>(EventName.EvtOptionClick, OnOptionsClick);
        EventHandle.RemoveEvent(EventName.EvtDialogClick, OnDialogClick);
    }

#endregion


#region run nodes

    private void PlayStartNode()
    {
        if (curNode is StartNode startNode)
        {
            if (startNode.typeSound != null)
            {
                // Add music
                GameManager.Instance.audioManager.AddTypeMusic("type", startNode.typeSound);
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

        if (index == dialogNode.chatList.Count - 1)
        {
            curDialogIndex = 0;
        }
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
        updateDialogHandler?.Invoke(detail, personName);
        if (type == ChatType.Normal)
        {
            curDialogIndex++;
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
        updateOptionsHandler?.Invoke(options);

        // pause, waiting for players' choice
        curState = State.Pause;
    }

    private void OnOptionsClick(int index)
    {
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

    private void PlayBackGroundNode()
    {
        if (curNode is BackGroundNode backGroundNode)
        {
            updateBackgroundHandler?.Invoke(backGroundNode.img);
            // execute next node
            curNode = backGroundNode.MoveNext(out current);
        }
        else
        {
            curState = State.Pause;
            Debug.LogError("curNode is not BackGroundNode, curNode is " + curNode.name + " current is " +
                           nameof(current));
        }
    }

#endregion


#region Method

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

        curDialogIndex = 0;
        curState = State.Play;
    }

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
            moveToDialogEndHandler?.Invoke();
            toNext = true;
        }
    }

#endregion
}

public enum State
{
    Play,
    Pause
}