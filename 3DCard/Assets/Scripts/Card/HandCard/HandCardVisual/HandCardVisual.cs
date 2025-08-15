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
        => theHandCard.index / (theHandCard.handCardDeck.CurrentHandCardCount + 1f);// 0-1֮���ֵ,��ʾ�����������е�λ��
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
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardSelected, OnHandCardClick);//����¼�����handcardDeck�д�������ٴ���
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

        //���Ƴ��������
        //Vector3 direction = Camera.main.transform.position - transform.position;
        transform.LookAt(2 * transform.position - Camera.main.transform.position, Camera.main.transform.up);

        //������Բ������
        theRectTransform.localRotation *= Quaternion.Euler(
            0,
            0,
            curveParameters.rotation.Evaluate(k) * curveParameters.rotationInfluence
        );



    }

    /// <summary>
    /// ֻ������ƽ��λ�ø����
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


    #region �¼���Ӧ
    private Tween tween;
    private void OnHandCardHovered(int index)
    {
        if(theHandCard.isSelected)
        {
            //������Ʊ�ѡ��,������
            return;
        }
        //����
        if (tween != null && tween.IsActive() && tween.IsPlaying())
        {
            tween.Kill();
        }
        
        tween = theRectTransform.DOScale(
            //index - theHandCard.index��ʾ����hovered�Ŀ��Ƶı�������
            Vector3.one * curveParameters.scaler.Evaluate(Mathf.Abs(index - theHandCard.index)/(0f+theHandCard.handCardDeck.CurrentHandCardCount))
                * curveParameters.scalerInfluence,
            0.4f
        ).SetEase(Ease.OutBack);

        if(theHandCard.index == index)
        {
            //��hovered�Ŀ��Ʒŵ����ϲ�
            transform.SetAsLastSibling();
        }
        


    }
    private void OnHandCardHoveredExit(int index)
    {
        if (theHandCard.isSelected)
        {
            //������Ʊ�ѡ��,������
            return;
        }
        //����
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
                //�����InteractableSceneObj��,���������ת
                //TODO:��Ϊ��������ƻ������Բ�Ҫˮƽ����
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
            //������Ʊ�ѡ��,������
            return;
        }
        //����
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
            //������Ʊ�ѡ��,������
            return;
        }
        //����
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

    //��Ϊ���е����ƶ�����������¼�,������Ҫ�Ƚ�index
    private void OnTranslateToTableCard(HandCardBase theHandCardBase)
    {
        if (theHandCardBase != theHandCard) return;
        this.enabled = false;
    }
    #endregion
}
