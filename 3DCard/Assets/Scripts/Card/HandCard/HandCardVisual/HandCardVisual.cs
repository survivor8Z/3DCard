using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HandCardVisual : MonoBehaviour
{
    [SerializeField] private CurveParameters curveParameters;
    [SerializeField] private HandCardBase theHandCard;
    [SerializeField] private float k
        => theHandCard.index / (theHandCard.handCardDeck.CurrentHandCardCount + 1f);// 0-1֮���ֵ,��ʾ�����������е�λ��
    private RectTransform theRectTransform;


    [SerializeField]private int originalIndex=>theHandCard.index;
    private SortingGroup sortingGroup;
    private void Awake()
    {
        theRectTransform = GetComponent<RectTransform>();
        sortingGroup = GetComponent<SortingGroup>();
    }
    private void Start()
    {
        //originalIndex = transform.GetSiblingIndex();
        sortingGroup.sortingOrder = theHandCard.index;

        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardHovered, OnHandCardHovered);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardHoveredExit, OnHandCardHoveredExit);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardSelected, OnHandCardClick);
    }
    private void Update()
    {
        SetPosition();
        SetRotation();
    }


    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardHovered, OnHandCardHovered);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardHoveredExit, OnHandCardHoveredExit);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_HandCardSelected, OnHandCardClick);


        tween.Kill();
        transform.DOKill();
    }

    private void SetRotation()
    {
        if(theHandCard.isSelected)
        {
            //������Ʊ�ѡ��,����ת
            theRectTransform.localRotation = Quaternion.identity;
            transform.LookAt(2 * transform.position - Camera.main.transform.position, Camera.main.transform.up);
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

    
    Vector3 velocity;
    private void SetPosition()
    {

        if (theHandCard.isSelected)
        {
            return;
        }
        theRectTransform.position = Vector3.SmoothDamp(
            theRectTransform.position,
            theHandCard.slotRectTrans.position
            + Camera.main.transform.up * curveParameters.positioning.Evaluate(k) * curveParameters.positioningInfluence,
            ref velocity,
            0.1f
        );
    }


    //�¼���Ӧ
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
            //if (!theHandCard.isSelected)
            //{
            //    transform.SetParent(theHandCard.handCardDeck.transform, true);
            //    return;
            //}
            //transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
            //transform.SetParent(theHandCard.handCardDeck.handCardSelectedToPos, true);
            //transform.DOLocalMove(Vector3.zero,
            //    0.2f
            //).SetEase(Ease.OutBack);
            //if (theHandCard.isDragging)
            //{
            //    return;
            //}
            if (theHandCard.isSelected)
            {
                transform.DOMove(theHandCard.selectedToPos.position,0.2f).SetEase(Ease.OutBack);
                transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
                //TODO:�����InteractableSceneObj��,���������ת
            }
        }
    }
}
