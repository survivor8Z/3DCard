using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandCardDeckVisual : MonoBehaviour
{
    //用于控制手牌堆的隐藏(降到一定程度)
    public bool isHide = false;
    public Transform HandCardDeckHideTransform;


    //用于得到场景与摄像机状态
    public PlayerMove playerMove;
    public Vector2 MousePosViewPort=>handCardDeck.mousePositionViewport;
    public Transform firstVCStateSlotsTransform, otherVCStateSlotsTransform;
    public Transform firstVCStatePickPointTransform, otherVCStatePickPointTransform;
    public Transform handCardSelectedToPos;

    //用于控制手牌升降
    [SerializeField]private HandCardDeck handCardDeck;
    [SerializeField]private Transform handCardDeckUpMax, handCardDeckDownMax;
    [SerializeField]private float minY;
    [SerializeField]private float maxY;
    [SerializeField]private float k = 1;

    private void Awake()
    {
        handCardDeck = GetComponent<HandCardDeck>();
    }
    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener<InteractableSceneObj>(E_EventType.E_PlayerEnterInteractableSceneObj, OnPlayerEnterInteractableSceneObj);
        EventCenter.Instance.AddEventListener<InteractableSceneObj>(E_EventType.E_PlayerExitInteractableSceneObj, OnPlayerExitInteractableSceneObj);

        //EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardStartDrag, OnHandCardStartDrag);
        //EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardEndDrag, OnHandCardEndDrag);
    }
    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener<InteractableSceneObj>(E_EventType.E_PlayerEnterInteractableSceneObj, OnPlayerEnterInteractableSceneObj);
        EventCenter.Instance.RemoveEventListener<InteractableSceneObj>(E_EventType.E_PlayerExitInteractableSceneObj, OnPlayerExitInteractableSceneObj);

        //EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardStartDrag, OnHandCardStartDrag);
        //EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardEndDrag, OnHandCardEndDrag);
    }
    private void Update()
    {
        HideToggle();
        SetDeckPos();
    }
    private void HideToggle()
    {
        if(playerMove.inSceneObj != null)
        {
            isHide = false;
            return;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            isHide = !isHide;
        }
    }
    //根据鼠标位置设置手牌整体升降,通过设置Slots间接控制实现缓动
    private void SetDeckPos()
    {
        if (isHide)
        {
            handCardDeck.slots.transform.position = HandCardDeckHideTransform.position;
            return;
        }


        if (playerMove.inSceneObj!=null)
        {
            handCardDeck.slots.transform.position = otherVCStateSlotsTransform.position;
            return;
        }

        handCardDeck.slots.transform.position = firstVCStateSlotsTransform.position;
        //在有选中的时候
        if (handCardDeck.hoveredCard != null&&handCardDeck.dragedCard==null)
        {
            return;
        }
        if (MousePosViewPort.y > maxY) k = 1;
        else if (MousePosViewPort.y > minY) k = Mathf.Lerp(0, 1, (MousePosViewPort.y - minY) / (maxY - minY));
        else k = 0;
        handCardDeck.slots.transform.position = Vector3.Lerp(handCardDeckDownMax.position, handCardDeckUpMax.position, k);
    }

    //事件响应
    public void OnPlayerEnterInteractableSceneObj(InteractableSceneObj interactableSceneObj)
    {
        handCardSelectedToPos = interactableSceneObj.pickPoint;
    }
    public void OnPlayerExitInteractableSceneObj(InteractableSceneObj interactableSceneObj)
    {
        handCardSelectedToPos = firstVCStatePickPointTransform;
    }
    #region InputSystem
    public void OnHideOrShowHandCardDeck(InputAction.CallbackContext context)
    {
        float axisValue = context.ReadValue<float>();

        if (axisValue > 0)
        {
            isHide = false;
        }
        else if (axisValue < 0)
        {
            isHide = true;
        }
    }
    #endregion
}
