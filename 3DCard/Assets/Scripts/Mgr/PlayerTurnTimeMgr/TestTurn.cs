using System.Collections;
using UnityEngine;

/// <summary>
/// 这是一个单位脚本，它在回合结束时需要执行一个动画。
/// </summary>
public class TestTurn : MonoBehaviour
{
    private void OnEnable()
    {
        // **只订阅回合结束的开始事件**
        PlayerTurnTimeManager.Instance.OnPlayerEndTurn.AddListener(OnTurnEndStarted);
    }

    private void OnDisable()
    {
        
        PlayerTurnTimeManager.Instance.OnPlayerEndTurn.RemoveListener(OnTurnEndStarted);
        
    }

    public void OnTurnEndStarted()
    {
        // **当收到回合结束的通知时，立即通知管理器任务开始**
        PlayerTurnTimeManager.Instance.turnCompletionControl.TaskStarted();

        // 开始协程执行异步任务
        StartCoroutine(PerformMyAction());
    }

    private IEnumerator PerformMyAction()
    {
        Debug.Log("单位开始执行异步任务...");
        yield return new WaitForSeconds(3f);
        Debug.Log("单位异步任务完成！");

        // 任务完成后，通知管理器
        PlayerTurnTimeManager.Instance.turnCompletionControl.TaskCompleted();
    }
}