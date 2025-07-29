using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    void OnMove(InputValue value)
    {
        Move(value.Get<float>() > 0);
    }
    void Move(bool isForward)
    {
        Debug.Log(isForward);
        Vector3 to = transform.position + transform.forward * (isForward ? 1 : -1);   
        transform.DOMove(to, 0.2f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            Debug.Log("ÒÆ¶¯Íê³É");
        });
    }
}
