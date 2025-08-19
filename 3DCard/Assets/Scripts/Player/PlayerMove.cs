using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public RoomBase theRoom => MapMgr.Instance.currentRoom;
    public Vector2Int toWorldCoor;
    public bool isToSceneObj = false;
    public InteractableSceneObj toSceneObj = null;
    public InteractableSceneObj inSceneObj = null;

    // --- ��ʱ���ͼ��ʱ�� ---
    private float lastMoveTime;
    private const float MOVE_INTERVAL = 0.15f; // �ƶ��������λ����
    #region �������ں���
    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener<InteractableSceneObj>(E_EventType.E_PlayerEnterInteractableSceneObjFront, OnPlayerEnterInteractableSceneObjFront);
        EventCenter.Instance.AddEventListener<InteractableSceneObj>(E_EventType.E_PlayerExitInteractableSceneObjFront, OnPlayerExitInteractableSceneObjFront);
    }

    private void Start()
    {
        toWorldCoor = MapMgr.Instance.WorldPosToWorldCoor(transform.position);
    }
    private void Update()
    {
        // ֻ�е�isFrontMove��isBackMoveΪ�棬�������ƶ����ʱ��ʱ��ִ���ƶ�
        if (isFrontMove && Time.time - lastMoveTime >= MOVE_INTERVAL)
        {
            MoveInternal(true);
            lastMoveTime = Time.time; // ���¼�ʱ��
        }
        else if (isBackMove && Time.time - lastMoveTime >= MOVE_INTERVAL)
        {
            MoveInternal(false);
            lastMoveTime = Time.time; // ���¼�ʱ��
        }
    }
    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener<InteractableSceneObj>(E_EventType.E_PlayerEnterInteractableSceneObjFront, OnPlayerEnterInteractableSceneObjFront);
        EventCenter.Instance.RemoveEventListener<InteractableSceneObj>(E_EventType.E_PlayerExitInteractableSceneObjFront, OnPlayerExitInteractableSceneObjFront);
    }
    #endregion

    void Move()
    {
        if (theRoom.unWalkableWorldCoor.Contains(toWorldCoor))
        {
            toWorldCoor = MapMgr.Instance.WorldPosToWorldCoor(transform.position);
            return;
        }
        transform.DOMove(MapMgr.Instance.WorldCoorToWorldPos(toWorldCoor), 0.2f).SetEase(Ease.OutCubic);
    }

    public void OnPlayerEnterInteractableSceneObjFront(InteractableSceneObj toSceneObj)
    {
        isToSceneObj = true;
        this.toSceneObj = toSceneObj;
    }

    public void OnPlayerExitInteractableSceneObjFront(InteractableSceneObj toSceneObj)
    {
        isToSceneObj = false;
        if (this.toSceneObj == toSceneObj)
            this.toSceneObj = null;
    }
    public void OnMove(InputValue value)
    {
        // ��ȡ����ֵ�����簴���ĸ���ֵ
        float moveValue = value.Get<float>();
        bool isForward = moveValue > 0;
        MoveInternal(isForward);
    }

    private bool isFrontMove = false;
    private bool isBackMove = false;
    public void OnHoldMoveFront(InputAction.CallbackContext context)
    {
        // ����Ƿ��� Canceled �׶�
        if (context.canceled)
        {
            isFrontMove = false;
        }
        else if (context.performed)
        {
            isFrontMove = true;
            isBackMove = false;
        }
    }
    public void OnHoldMoveBack(InputAction.CallbackContext context)
    {
        // ����Ƿ��� Canceled �׶�
        if (context.canceled)
        {
            isBackMove = false;
        }
        else if (context.performed)
        {
            isBackMove = true;
            isFrontMove = false;
            
        }
    }
    private void MoveInternal(bool isForward)
    {
        if (inSceneObj != null)
        {
            if (isForward)
            {
                inSceneObj.Enter();
            }
            else
            {
                inSceneObj.Exit();
                if (toSceneObj.EnterLevel == 0)
                {
                    inSceneObj = null;
                }
            }
            return;
        }

        if (isToSceneObj)
        {
            bool isFaceToSceneObj = Vector3.Dot((toSceneObj.transform.position - transform.position).normalized, transform.forward) > 0.7;
            if (isFaceToSceneObj)
            {
                if (isForward)
                {
                    toSceneObj.Enter();
                    inSceneObj = toSceneObj;
                }
                else
                {
                    toWorldCoor = MapMgr.Instance.WorldPosToWorldCoor(transform.position) - MapMgr.Instance.WorldPosToWorldCoor(transform.forward);
                    Move();
                }
            }
            else
            {
                toWorldCoor = MapMgr.Instance.WorldPosToWorldCoor(transform.position) + (isForward ? 1 : -1) * MapMgr.Instance.WorldPosToWorldCoor(transform.forward);
                Move();
            }
            return;
        }

        toWorldCoor = MapMgr.Instance.WorldPosToWorldCoor(transform.position) + (isForward ? 1 : -1) * MapMgr.Instance.WorldPosToWorldCoor(transform.forward);
        Move();
    }
}