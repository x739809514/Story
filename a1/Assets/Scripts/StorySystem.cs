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
    public List<StoryGraph> graphList;
    private State curState;
    private Current current;
    private Node curNode;
    private int curDialogIndex; // 当前播放到第几段对话
    private int curGraphIndex;

    private float chatSpeed = 0.15f;
    private WaitForSeconds chatTimer;
    private bool auto; // 自动播放
    private bool toNext; // 点击显示完成对话内容

    // 对外API
    public Action moveToDialogEndHandler;
    public Action<SinglePersonChat, string> updateDialogHandler;
    public Action<List<string>> updateOptionsHandler;
    public Action<Sprite> updateBackgroundHandler;
    public Action<AnimationClip> updateAnimationHandler;


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
        EventHandle.AddEvent(EventName.EvtAnimStopClick, OnAnimStopClick);
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
                case Current.Audio:
                    PlayAudioNode();
                    break;
                case Current.Animation:
                    PlayAnimNode();
                    break;
                case Current.End:
                    PlayEndNode();
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        EventHandle.RemoveEvent<SinglePersonChat, string>(EventName.EvtDialogExecute, UpdateDialog);
        EventHandle.RemoveEvent<int>(EventName.EvtOptionClick, OnOptionsClick);
        EventHandle.RemoveEvent(EventName.EvtDialogClick, OnDialogClick);
        EventHandle.RemoveEvent(EventName.EvtAnimStopClick, OnAnimStopClick);
    }

#endregion


#region run nodes

    private void PlayStartNode()
    {
        if (curNode is StartNode startNode)
        {
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

    private void PlayAudioNode()
    {
        curState = State.Pause;
        if (curNode is AudioNode audioNode)
        {
            for (int i = 0; i < audioNode.soundLists.Count; i++)
            {
                var sound = audioNode.soundLists[i];
                if (sound.clip != null)
                {
                    GameManager.audioManager.AddTypeMusic(sound.name, sound.clip, sound.play);
                }
                else
                {
                    Debug.LogError($"Audio Clip {i} is empty");
                }
            }

            curNode = audioNode.MoveNext(out current);
            curState = State.Play;
        }
        else
        {
            Debug.LogError("curNode is not AudioNode, curNode is " + curNode.name + " current is " +
                           nameof(current));
        }
    }

    private void PlayAnimNode()
    {
        if (curNode is AnimNode animNode)
        {
            updateAnimationHandler?.Invoke(animNode.clip);
            curState = State.Pause;
            curNode = animNode.MoveNext(out current);
        }
        else
        {
            Debug.LogError("curNode is not AnimNode, curNode is " + curNode.name + " current is " +
                           nameof(current));
        }
    }

    private void PlayEndNode()
    {
        curGraphIndex++;
        curState = State.Pause;
        InitNode();
    }

#endregion


#region Method

    private void InitNode()
    {
        if (curGraphIndex >= graphList.Count)
        {
            Debug.Log("It's end");
            return;
        }

        var graph = graphList[curGraphIndex];
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

    private void OnAnimStopClick()
    {
        curState = State.Play;
    }

#endregion
}

public enum State
{
    Play,
    Pause
}