using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class HandCardVisual : MonoBehaviour
{
    [HideInInspector] public CurveParameters curveParameters;
    private HandCardBase theHandCard;
    [SerializeField] private float k
        => theHandCard.index / (theHandCard.handCardDeck.CurrentHandCardCount + 1f);// 0-1之间的值,表示卡牌在手牌中的位置
    private RectTransform theRectTransform;


    [SerializeField]private int originalIndex=>theHandCard.index;
    private SortingGroup sortingGroup;

    private void Awake()
    {
        theHandCard = GetComponent<HandCardBase>();
        theRectTransform = GetComponent<RectTransform>();
        sortingGroup = GetComponent<SortingGroup>();
    }
    private void Start()
    {
        sortingGroup.sortingOrder = theHandCard.index;
    }
    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardHovered, OnHandCardHovered);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardHoveredExit, OnHandCardHoveredExit);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardSelected, OnHandCardClick);//这个事件是在handcardDeck中处理完后再触发
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardStartDrag, OnHandCardStartDrag);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardEndDrag, OnHandCardEndDrag);

        EventCenter.Instance.AddEventListener<HandCardBase>(E_EventType.E_HandCardToTableCard, OnTranslateToTableCard);
    }
    private void Update()
    {
        SetPosition();
        SetRotation();
    }

    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardHovered, OnHandCardHovered);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardHoveredExit, OnHandCardHoveredExit);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardSelected, OnHandCardClick);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardStartDrag, OnHandCardStartDrag);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardEndDrag, OnHandCardEndDrag);

        EventCenter.Instance.RemoveEventListener<HandCardBase>(E_EventType.E_HandCardToTableCard, OnTranslateToTableCard);

        tween.Kill();
        transform.DOKill();
    }

    private void OnDestroy()
    {
        
    }

    private void SetRotation()
    {
        if(theHandCard.isSelected)
        {
            if (theHandCard.handCardDeck.player.playerMove.inSceneObj != null) return;
            theRectTransform.localRotation = Quaternion.identity;
            transform.LookAt(2 * transform.position - Camera.main.transform.position, Camera.main.transform.up);
            return;
        }

        if(theHandCard.isDragging&&theHandCard.cardSO.cardType == E_CardType.E_Entity && theHandCard.handCardDeck.player.playerMove.inSceneObj is Table table)
        {
            transform.rotation = theHandCard.handCardDeck.player.playerMove.inSceneObj.pickPoint.rotation;
            return;
        }

        //卡牌朝向摄像机
        //Vector3 direction = Camera.main.transform.position - transform.position;
        transform.LookAt(2 * transform.position - Camera.main.transform.position, Camera.main.transform.up);

        //卡牌沿圆形排列
        theRectTransform.localRotation *= Quaternion.Euler(
            0,
            0,
            curveParameters.rotation.Evaluate(k) * curveParameters.rotationInfluence
        );



    }

    /// <summary>
    /// 只是用来平滑位置跟随的
    /// </summary>
    public Vector3 followVelocity;
    private void SetPosition()
    {

        if (theHandCard.isSelected)
        {
            return;
        }

        if(theHandCard.isDragging&&theHandCard.cardSO.cardType == E_CardType.E_Entity && theHandCard.handCardDeck.player.playerMove.inSceneObj is Table table)
        {
            theRectTransform.position = Vector3.SmoothDamp(
            theRectTransform.position,
            table.dragPoint.position,
            ref followVelocity,
            0.1f
        );

            return;
        }

        theRectTransform.position = Vector3.SmoothDamp(
            theRectTransform.position,
            theHandCard.slotRectTrans.position
            + Camera.main.transform.up * curveParameters.positioning.Evaluate(k) * curveParameters.positioningInfluence,
            ref followVelocity,
            0.1f
        );
    }


    #region 事件响应
    private Tween tween;
    private void OnHandCardHovered(int index)
    {
        if(theHandCard.isSelected)
        {
            //如果卡牌被选中,则不缩放
            return;
        }
        //缩放
        if (tween != null && tween.IsActive() && tween.IsPlaying())
        {
            tween.Kill();
        }
        
        tween = theRectTransform.DOScale(
            //index - theHandCard.index表示距离hovered的卡牌的比例距离
            Vector3.one * curveParameters.scaler.Evaluate(Mathf.Abs(index - theHandCard.index)/(0f+theHandCard.handCardDeck.CurrentHandCardCount))
                * curveParameters.scalerInfluence,
            0.4f
        ).SetEase(Ease.OutBack);

        if(theHandCard.index == index)
        {
            //将hovered的卡牌放到最上层
            transform.SetAsLastSibling();
        }
        


    }
    private void OnHandCardHoveredExit(int index)
    {
        if (theHandCard.isSelected)
        {
            //如果卡牌被选中,则不缩放
            return;
        }
        //缩放
        if (tween != null && tween.IsActive() && tween.IsPlaying())
        {
            tween.Kill();
        }
        tween = theRectTransform.DOScale(
            Vector3.one,
            0.2f
        ).SetEase(Ease.OutBack);

        transform.SetSiblingIndex(originalIndex);

    }
    private void OnHandCardClick(int index)
    {
        if(tween != null && tween.IsActive() && tween.IsPlaying())
        {
            tween.Kill();
        }
        
        if (index == theHandCard.index)
        {
            if (theHandCard.isSelected)
            {
                transform.DOMove(theHandCard.selectedToPos.position,0.2f).SetEase(Ease.OutBack);
                transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
                //如果在InteractableSceneObj中,还需调整旋转
                //TODO:因为不想和桌牌混淆所以不要水平放置
                if (theHandCard.handCardDeck.player.playerMove.inSceneObj != null)
                {
                    transform.DORotateQuaternion(
                        theHandCard.handCardDeck.player.playerMove.inSceneObj.pickPoint.rotation,
                        0.2f
                    ).SetEase(Ease.OutBack);
                }

            }
        }
    }

    private void OnHandCardStartDrag(int index)
    {
        if (theHandCard.isSelected)
        {
            //如果卡牌被选中,则不缩放
            return;
        }
        //缩放
        if (tween != null && tween.IsActive() && tween.IsPlaying())
        {
            tween.Kill();
        }
        tween = theRectTransform.DOScale(
            Vector3.one,
            0.2f
        ).SetEase(Ease.OutBack);
        transform.SetSiblingIndex(originalIndex);
    }
    private void OnHandCardEndDrag(int index)
    {
        if (theHandCard.isSelected)
        {
            //如果卡牌被选中,则不缩放
            return;
        }
        //缩放
        if (tween != null && tween.IsActive() && tween.IsPlaying())
        {
            tween.Kill();
        }
        tween = theRectTransform.DOScale(
            Vector3.one,
            0.2f
        ).SetEase(Ease.OutBack);
        transform.SetSiblingIndex(originalIndex);
    }

    //因为所有的手牌都监听了这个事件,还是需要比较index
    private void OnTranslateToTableCard(HandCardBase theHandCardBase)
    {
        if (theHandCardBase != theHandCard) return;
        this.enabled = false;
    }
    #endregion
}
