using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;


public enum E_StateEvent
{
    Do,
    ToChase,
    ToAttack,
    ToDeath,
}

public enum E_State
{
    WaitState,
    ChaseState,
    AttackState,
    DeathState,
}

public class WaitState<TAI> : ActionState<E_State,E_StateEvent>
{
    private TAI theAI;
    public WaitState(TAI ai,bool needsExitTime, bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
        theAI = ai;
    }
    public override void OnEnter()
    {
        
    }
    public override void OnLogic()
    {
        
    }
    public override void OnExit()
    {

    }
}
public class ChaseState<TAI> : ActionState<E_State, E_StateEvent> where TAI: InteractableObject
{
    private TAI theAI;
    private Tween chaseTween;
    public ChaseState(TAI ai,bool needsExitTime, bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
        theAI = ai;
        AddAction(E_StateEvent.Do, MoveATile);
    }
    private void MoveATile()
    {
        Debug.Log("Enemy Move a tile towards player!");
        PlayerTurnTimeManager.Instance.turnCompletionControl.TaskStarted();
        Vector2Int nextStep = AStarPathFinding.FindPathNextStep(
            MapMgr.Instance.WorldPosToWorldCoor(theAI.transform.position),
            MapMgr.Instance.WorldPosToWorldCoor(Player.Instance.transform.position));

        if (nextStep != MapMgr.Instance.WorldPosToWorldCoor(theAI.transform.position))
        {
            MapMgr.Instance.TheObstacleMoveTo(theAI, nextStep);
            if(chaseTween != null && chaseTween.IsActive())
            {
                chaseTween.Kill();
            }
            chaseTween = theAI.transform.DOMove(
                MapMgr.Instance.WorldCoorToWorldPos(nextStep),
                0.2f).SetEase(Ease.OutCubic)
                    .OnComplete(() =>
                    {
                        PlayerTurnTimeManager.Instance.turnCompletionControl.TaskCompleted();
                    });
        }
        else
        {
            Debug.Log("enemy cannot move");
            PlayerTurnTimeManager.Instance.turnCompletionControl.TaskCompleted();
        }
        
    }
    private IEnumerator MoveATileCoroutine(Vector2Int nextStep)
    {
        //≤ª”√dotweenª∫∂Ø
        while(theAI.transform.position != MapMgr.Instance.WorldCoorToWorldPos(nextStep))
        {
            theAI.transform.position = Vector3.MoveTowards(
                theAI.transform.position,
                MapMgr.Instance.WorldCoorToWorldPos(nextStep),
                0.1f);
            yield return null;
        }
        PlayerTurnTimeManager.Instance.turnCompletionControl.TaskCompleted();
    }
    public override void OnEnter()
    {

    }
    public override void OnLogic()
    {

    }
    public override void OnExit()
    {

    }
    
}
public class AttackState<TAI> : ActionState<E_State, E_StateEvent>
{
    private TAI theAI;
    public AttackState(TAI ai, bool needsExitTime, bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
        theAI = ai;
        AddAction(E_StateEvent.Do,Attack);
    }
    private void Attack()
    {
        PlayerTurnTimeManager.Instance.turnCompletionControl.TaskStarted();
        Debug.Log("Enemy Attack!");
        PlayerTurnTimeManager.Instance.turnCompletionControl.TaskCompleted();
    }
    public override void OnEnter()
    {

    }
    public override void OnLogic()
    {

    }
    public override void OnExit()
    {

    }
}
public class DeathState<TAI> : ActionState<E_State, E_StateEvent>
{
    private TAI theAI;
    public DeathState(TAI ai, bool needsExitTime, bool isGhostState = false) : base(needsExitTime, isGhostState)
    {
        theAI = ai;
    }
    public override void OnEnter()
    {

    }
    public override void OnLogic()
    {

    }
    public override void OnExit()
    {

    }
}

