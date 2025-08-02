using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slots : MonoBehaviour
{
    public List<Slot> slotsList;
    private HandCardDeck handCardDeck;
    private RectTransform theRectTransform;
    private float width=>handCardDeck.CurrentHandCardCount*140;
    [SerializeField]float hoveredAddWidth = 0f;
    [SerializeField]AnimationCurve widthCurve;
    [SerializeField]float widthInfluence;

    private void Awake()
    {
        theRectTransform = GetComponent<RectTransform>();
        handCardDeck = GetComponentInParent<HandCardDeck>();
    }
    private void Start()
    {
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardHovered, OnHandCardHovered);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_HandCardHoveredExit, OnHandCardHoveredExit);
    }
    private void Update()
    {
        SetWidth();
    }
    private void SetWidth()
    {
        theRectTransform.sizeDelta 
            = new Vector2(width+hoveredAddWidth
            , theRectTransform.sizeDelta.y);
    }
    public void ResetSlots()
    {
        for(int i=0;i<slotsList.Count;i++)
        {
            slotsList[i].transform.SetSiblingIndex(i);
        }
    }

    //ÊÂ¼þÏìÓ¦

    private void OnHandCardHovered(int index)
    {
        if(handCardDeck.selectedCard!=null && handCardDeck.selectedCard.index == index)
        {
            hoveredAddWidth = 0f;
            return;
        }
        hoveredAddWidth = 100f;
    }
    private void OnHandCardHoveredExit(int index)
    {
        hoveredAddWidth = 0;
    }

}

