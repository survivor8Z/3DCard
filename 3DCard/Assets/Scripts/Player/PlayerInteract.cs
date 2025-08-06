using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{

    //手牌
    public HandCardDeck handCardDeck;
    //射线检测
    [SerializeField] private GameObject hitObject;
    public InteractableObject pointInteractableObject;
    public LayerMask interactableLayerMask;
    public Vector3 MouseWorldPosition => Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
    private void Update()
    {
        UpdatePointInteractableObject();

        if (Input.GetMouseButtonUp(0))
        {
            Interact();
        }else if (Input.GetMouseButtonDown(1))
        {
            handCardDeck.isCancel = true;
            handCardDeck.ResetCardState();
        }else if(Input.GetMouseButtonDown(0))
        {
            handCardDeck.isCancel = false;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "InteractableSceneFront")
        {
            EventCenter.Instance.EventTrigger(E_EventType.E_PlayerEnterInteractableSceneObjFront, other.GetComponentInParent<InteractableSceneObj>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "InteractableSceneFront")
        {
            EventCenter.Instance.EventTrigger(E_EventType.E_PlayerExitInteractableSceneObjFront, other.GetComponent<InteractableSceneObj>());
        }
    }


    public void Interact()
    {
        int pointInteractableObjectID = pointInteractableObject==null?0: pointInteractableObject.id;

        if (handCardDeck.dragedCard != null && handCardDeck.selectedCard != null)
        {
            handCardDeck.TryCardCombinationPlay(new CardCombinationPlayID(handCardDeck.dragedCard.cardSO.cardID,
                                                                          handCardDeck.selectedCard.cardSO.cardID,
                                                                          pointInteractableObjectID));
            return;
        }
        if (handCardDeck.dragedCard != null)
        {
            handCardDeck.dragedCard.TryCardDragPlay(new CardDragedPlayID(handCardDeck.dragedCard.cardSO.cardID,
                                                                         pointInteractableObjectID));
            return;
        }
        if (handCardDeck.hoveredCard == null && handCardDeck.selectedCard != null&&handCardDeck.hoveredCard==null)
        {
            handCardDeck.selectedCard.TryCardSelectedPlay(new CardSelectedPlayID(handCardDeck.selectedCard.cardSO.cardID,
                                                                                 pointInteractableObjectID));
            return;
        }
        //下面是没有选中以及拖拽卡牌时
        if (pointInteractableObject != null)
        {
            if(pointInteractableObject is TableCardDeck tableCardDeck)
            {
                //显示牌堆界面
            }
            
        }
        
    }


    //更新指到的物体,射线检测
    public void UpdatePointInteractableObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, (MouseWorldPosition-Camera.main.transform.position).normalized, out hit, 100f,interactableLayerMask))
        {
            //画线debug
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);

            if(pointInteractableObject is Table table)
            {
                table.UpdateTablePoint(hit.point + Vector3.up);
            }

            if (hitObject!=null&&hit.collider.gameObject == hitObject)
            {
                //相同的就不更新
                return; 
            }
            hitObject = hit.collider.gameObject;
            pointInteractableObject = hitObject.GetComponent<InteractableObject>();
            
        }
        else
        {
            hitObject = null;
            pointInteractableObject = null;
        }
    }
    //public void Test()
    //{
    //    handCardDeck.dragedCard.DragedFuncDic.Add(new CardDragedPlayID(0, 0), 1);
    //}
    
}
