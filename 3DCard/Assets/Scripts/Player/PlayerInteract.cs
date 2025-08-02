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
        if (handCardDeck.dragedCard != null && handCardDeck.selectedCard != null)
        {
            handCardDeck.CardCombinationPlay();
            return;
        }
        if (handCardDeck.dragedCard != null)
        {
            handCardDeck.dragedCard.CardDragPlay();
            return;
        }
        if (handCardDeck.hoveredCard == null && handCardDeck.selectedCard != null && handCardDeck.selectedCard.CanSelectedPlayCard())
        {
            handCardDeck.selectedCard.CardSelectedPlay();
            return;
        }
        //下面是没有选中以及拖拽卡牌时
        if (pointInteractableObject != null)
        {
            if(pointInteractableObject is TableCardDeck tableCardDeck)
            {
                //TODO:显示牌堆界面
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
            if (hitObject!=null||hit.collider.gameObject == hitObject)
            {
                return; 
            }
            hitObject = hit.collider.gameObject;
            InteractableObject interactableObject = hit.collider.GetComponent<InteractableObject>();
        }
        else
        {
            hitObject = null;
            pointInteractableObject = null;
        }
    }

    
}
