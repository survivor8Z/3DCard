using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;
using UnityHFSM.Visualization;

public class EnemyBase : InteractableObject,IDamageable
{
    public BuffHandler buffHandler;
    public ChaPropertyInfo chaInfo;
    //状态机
    StateMachine<E_State, E_StateEvent> fsm = new();
    private Animator fsmAnimator;
    private void Awake()
    {
        fsmAnimator = GetComponent<Animator>();
        buffHandler = GetComponent<BuffHandler>();
        chaInfo = GetComponent<ChaPropertyInfo>();
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

        //属性初始化
        chaInfo.Init();

        //状态机初始化
        var waitState = new WaitState<EnemyBase>(this, needsExitTime: false);
        var attackState = new AttackState<EnemyBase>(this, needsExitTime: false);
        var deathState = new DeathState<EnemyBase>(this, needsExitTime: false);

        fsm.AddState(E_State.WaitState, waitState);
        fsm.AddState(E_State.AttackState, attackState);

        fsm.AddTriggerTransitionFromAny(E_StateEvent.ToDeath, E_State.DeathState);
        fsm.AddTwoWayTransition(E_State.WaitState, E_State.AttackState, condition =>
        {
            return (MapMgr.Instance.WorldPosToRoomWorldCoorBig(transform.position)
                 == MapMgr.Instance.WorldPosToRoomWorldCoorBig(Player.Instance.transform.position));//在同一房间内
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
        fsm.OnLogic();

#if UNITY_EDITOR
        HfsmAnimatorGraph.PreviewStateMachineInAnimator(fsm, fsmAnimator);
#endif
    }


    public void GetDamage(DamageInfo damageInfo)
    {
        buffHandler.TriggerCustom(E_BuffCallBackType.BeforeGetDamage);
        switch(damageInfo.damageType )
        {
            case E_DamageType.Normal:
                int damge = damageInfo.damageValue - chaInfo.curDef;
                int toReducetHpDamage = damge - chaInfo.curArmor;
                if (toReducetHpDamage > 0)
                {
                    chaInfo.curArmor = 0;
                    int realDamage = toReducetHpDamage ;
                    chaInfo.curHp -= realDamage;
                    buffHandler.TriggerCustom(E_BuffCallBackType.AfterGetDamage);
                    if (chaInfo.curHp <= 0)
                    {
                        chaInfo.curHp = 0;
                        //Die
                        Debug.Log(this.gameObject.name + " Die");
                    }
                }
                else
                {
                    chaInfo.curArmor -= damge;
                }
                break;
            case E_DamageType.Puncture:
                int damage = damageInfo.damageValue - chaInfo.curDef;
                chaInfo.curHp -= damage;
                buffHandler.TriggerCustom(E_BuffCallBackType.AfterGetDamage);
                if (chaInfo.curHp <= 0)
                {
                    chaInfo.curHp = 0;
                    //Die
                    Debug.Log(this.gameObject.name + " Die");
                }
                break;
        }
        
    }
    #region 事件监听
    /// <summary>
    /// 玩家回合结束时调用
    /// </summary>
    public void OnEnemyStartTurn()
    {
        fsm.OnAction(E_StateEvent.Do);
    }
    #endregion
}
