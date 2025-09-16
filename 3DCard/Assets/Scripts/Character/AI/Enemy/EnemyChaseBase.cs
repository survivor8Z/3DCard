using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityHFSM;
using UnityHFSM.Visualization;  // Import the animator graph feature.

public class EnemyChaseBase: InteractableObject
{
    public Player player;
    StateMachine<E_State, E_StateEvent> fsm = new();
    private Animator fsmAnimator;

    private void Awake()
    {
        fsmAnimator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        Init();
        PlayerTurnTimeManager.Instance.OnPlayerEndTurn.AddListener(OnEnemyStartTurn);
    }
    private void OnDisable()
    {
        PlayerTurnTimeManager.Instance.OnPlayerEndTurn.RemoveListener(OnEnemyStartTurn);
    }
    public override void Init()
    {
        base.Init();

        player = Player.Instance;


        var waitState = new WaitState<EnemyChaseBase>(this,needsExitTime: false);
        var chaseState = new ChaseState<EnemyChaseBase>(this,needsExitTime: false);
        var attackState = new AttackState<EnemyChaseBase>(this,needsExitTime: false);
        var deathState = new DeathState<EnemyChaseBase>(this,needsExitTime: false);

        fsm.AddState(E_State.WaitState,waitState);
        fsm.AddState(E_State.ChaseState, chaseState);
        fsm.AddState(E_State.AttackState, attackState);

        fsm.AddTriggerTransitionFromAny(E_StateEvent.ToDeath, E_State.DeathState);
        fsm.AddTwoWayTransition(E_State.WaitState, E_State.ChaseState, condition =>
        {
            return (MapMgr.Instance.WorldPosToRoomWorldCoorBig(transform.position)
                == MapMgr.Instance.WorldPosToRoomWorldCoorBig(player.transform.position));//在同一房间内
        });
        fsm.AddTwoWayTransition(E_State.ChaseState, E_State.AttackState, condition =>
        {
            return (MapMgr.Instance.WorldPosToWorldCoor(transform.position)
                -MapMgr.Instance.WorldPosToWorldCoor(player.transform.position)).magnitude==1;//相邻格子
        });

       
        fsm.SetStartState(E_State.WaitState);
        fsm.Init();

#if UNITY_EDITOR
        HfsmAnimatorGraph.CreateAnimatorFromStateMachine(
            fsm,
            outputFolderPath: "Assets/Scripts/AI/Enemy/DebugAnimators",
            animatorName: "TestGraph.controller"
        );
#endif  
    }

    private void Update()
    {
        
        
#if UNITY_EDITOR
        HfsmAnimatorGraph.PreviewStateMachineInAnimator(fsm, fsmAnimator);
#endif
    }


    #region 事件监听
    /// <summary>
    /// 玩家回合结束时调用
    /// </summary>
    public void OnEnemyStartTurn()
    {
        fsm.OnLogic();
       
        fsm.OnAction(E_StateEvent.Do);
    }
    #endregion
}
