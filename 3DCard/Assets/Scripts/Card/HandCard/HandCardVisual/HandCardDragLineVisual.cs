using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HandCardDragLineVisual : MonoBehaviour
{
    [SerializeField] private CurveParameters curveParameters;
    [SerializeField] private HandCardBase theHandCard;
    [SerializeField]private float k
        => theHandCard.index / (theHandCard.handCardDeck.CurrentHandCardCount + 1f);// 0-1֮���ֵ,��ʾ�����������е�λ��
    private RectTransform theRectTransform;


    [SerializeField] private int originalIndex => theHandCard.index;
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

        tween.Kill();
    }

    private void SetRotation()
    {

        //���Ƴ��������
        Vector3 direction = Camera.main.transform.position - transform.position;
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
        //theRectTransform.position = theHandCard.slotRectTrans.position
        //    +Camera.main.transform.up * curveParameters.positioning.Evaluate(k) * curveParameters.positioningInfluence;



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
        //����
        if (tween != null && tween.IsActive() && tween.IsPlaying())
        {
            tween.Kill();
        }

        tween = theRectTransform.DOScale(
            //index - theHandCard.index��ʾ����hovered�Ŀ��Ƶı�������
            Vector3.one * curveParameters.scaler.Evaluate(Mathf.Abs(index - theHandCard.index) / (0f + theHandCard.handCardDeck.CurrentHandCardCount))
                * curveParameters.scalerInfluence,
            0.4f
        ).SetEase(Ease.OutBack);

        if (theHandCard.index == index)
        {
            //��hovered�Ŀ��Ʒŵ����ϲ�
            transform.SetAsLastSibling();
        }



    }
    private void OnHandCardHoveredExit(int index)
    {
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

    private void OnHandCardDrag()
    {

    }
}
