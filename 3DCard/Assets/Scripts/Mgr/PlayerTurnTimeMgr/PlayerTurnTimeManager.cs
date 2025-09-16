using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 管理回合结束时的所有异步任务，并在所有任务完成后触发一个事件。
/// </summary>
public class TurnCompletionControl
{
    private int pendingTasks = 0;
    private bool isEndingTurn = false;

    // 当所有任务都完成后触发的事件
    public UnityEvent OnAllTasksCompleted = new UnityEvent();

    /// <summary>
    /// 通知管理器：我有一个新任务要开始。
    /// </summary>
    public void TaskStarted()
    {
        pendingTasks++;
    }

    /// <summary>
    /// 通知管理器：我的任务已完成。
    /// </summary>
    public void TaskCompleted()
    {
        pendingTasks = Mathf.Max(0, pendingTasks - 1);
        CheckAllTasksCompleted();
    }

    /// <summary>
    /// 启动回合结束流程，准备开始计时和等待。
    /// </summary>
    public void StartTurnEndProcess()
    {
        if (isEndingTurn) return;
        isEndingTurn = true;

        CheckAllTasksCompleted();
    }

    private void CheckAllTasksCompleted()
    {
        if (isEndingTurn && pendingTasks <= 0)
        {
            isEndingTurn = false;
            OnAllTasksCompleted.Invoke();
        }
    }
}

/// <summary>
/// 管理玩家回合的时间和生命周期。
/// </summary>
public class PlayerTurnTimeManager : SingletonMono<PlayerTurnTimeManager>
{
    [SerializeField] private float maxTurnTime = 100f;
    [SerializeField] private float currentTurnTime = 100f;

    private bool isPlayerTurn = false;

    // 负责回合结束时的同步管理
    public TurnCompletionControl turnCompletionControl = new TurnCompletionControl();

    // **新增事件：在回合结束流程开始时触发**
    public UnityEvent OnPlayerEndTurn = new UnityEvent();

    // **公开事件：在所有任务完成后触发**
    public UnityEvent OnPlayerEndTurnListenerEnd => turnCompletionControl.OnAllTasksCompleted;

    public bool IsPlayerTurn => isPlayerTurn;

    private void Start()
    {
        // 游戏开始时，自动开始玩家回合
        StartPlayerTurn();

        // **订阅自己的回合结束事件，以开始新回合**
        OnPlayerEndTurnListenerEnd.AddListener(StartPlayerTurn);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            // **模拟玩家按下空格键结束回合**
            ForceEndTurn();
        }
    }

    /// <summary>
    /// 减少回合时间，并检查是否小于等于0以强制结束回合。
    /// </summary>
    public void TickReduceTurnTime(float reduceTurnTime)
    {
        if (!isPlayerTurn) return;

        currentTurnTime -= reduceTurnTime;
        if (currentTurnTime <= 0)
        {
            ForceEndTurn();
        }
    }

    /// <summary>
    /// 强制结束玩家回合，并启动同步流程。
    /// </summary>
    public void ForceEndTurn()
    {
        if (!isPlayerTurn) return;

        isPlayerTurn = false;
        //Debug.Log("玩家回合结束！正在通知所有侦听者...");

        // **触发回合结束的开始事件**
        OnPlayerEndTurn.Invoke();

        // **立即启动计数器，防止没有任何侦听者导致事件不触发**
        turnCompletionControl.StartTurnEndProcess();
    }

    /// <summary>
    /// 启动玩家回合。
    /// </summary>
    public void StartPlayerTurn()
    {
        if (isPlayerTurn) return;

        isPlayerTurn = true;
        currentTurnTime = maxTurnTime;
        //Debug.Log("玩家回合开始！");
    }
}