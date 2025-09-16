using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public Player player;
    //手牌创建Entity相关
    public Transform toCreateEntityTransform;

    //手牌
    public HandCardDeck handCardDeck;
    //射线检测
    [SerializeField] private GameObject hitObject;
    public Vector3 hitPosition;//射线检测点,基本是用来生成Entity
    public IInteractable pointInteractableObject;//不包含桌牌等卡牌
    public IInteractable pointCard;//包含桌牌,手牌不包含,手牌不继承IInteractable,通过桌牌的UGUI事件赋值
    public LayerMask interactableLayerMask;
    public Vector3 MouseWorldPosition => Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
    public Vector2 MouseViewPortPosition => new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);

    public float PlayableMouseY;
    #region 生命周期函数
    private void Update()
    {
        UpdatePointInteractableObject();

        if (Input.GetMouseButtonUp(0))
        {
            Interact();
        }else if (Input.GetMouseButtonDown(1))
        {
            handCardDeck.isCancel = true;
            if (handCardDeck.selectedCard is HandCard_Entity handCard_Entity)
            {
                handCard_Entity.DelPreViewEntity();
            }
            handCardDeck.ResetCardState();
            
        }
        else if(Input.GetMouseButtonDown(0))
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

    #endregion
    public void Interact()
    {


        //在桌子视角
        if (player.playerMove.inSceneObj is Table table)
        {
            if (pointCard != null)//将手牌直接拖拽到桌牌
            {
                if (handCardDeck.dragedCard != null)
                {

                    if(!PlayerTurnTimeManager.Instance.IsPlayerTurn)
                    {
                        handCardDeck.ResetCardState();
                        return;
                    }

                    if (MouseViewPortPosition.y < PlayableMouseY)
                    {
                        handCardDeck.ResetCardState();
                        return;
                    }

                    


                    handCardDeck.dragedCard.TryCardDragPlay(pointCard);
                    return;
                }
            }
            
        }


        //手牌相关

        ////组合打出,不搞了
        //if (handCardDeck.dragedCard != null && handCardDeck.selectedCard != null)
        //{
        //    //if (MouseViewPortPosition.y < PlayableMouseY)
        //    //{
        //    //    handCardDeck.ResetCardState();
        //    //    return;
        //    //}
        //    if (!PlayerTurnTimeManager.Instance.IsPlayerTurn)
        //    {
        //        handCardDeck.ResetCardState();
        //        return;
        //    }

        //    handCardDeck.TryCardCombinationPlay(pointInteractableObject);
        //    return;
        //}
        //拖拽打出
        if (handCardDeck.dragedCard != null)
        {
            if (!PlayerTurnTimeManager.Instance.IsPlayerTurn)
            {
                handCardDeck.ResetCardState();
                return;
            }
            //if (MouseViewPortPosition.y < PlayableMouseY)
            //{
            //    handCardDeck.ResetCardState();
            //    return;
            //}

            handCardDeck.dragedCard.TryCardDragPlay(pointInteractableObject);
            return;
        }
        //选择打出
        if (handCardDeck.selectedCard != null )
        {
            if (!PlayerTurnTimeManager.Instance.IsPlayerTurn)
            {
                handCardDeck.ResetCardState();
                return;
            }
            if (handCardDeck.hoveredCard == null)
            {
                handCardDeck.selectedCard.TryCardSelectedPlay(pointInteractableObject);
            }
            else
            {
                return;
            }
            
            return;
        }

        

        //下面是没有选中以及拖拽卡牌时
        if (pointInteractableObject != null)
        {
            if (!PlayerTurnTimeManager.Instance.IsPlayerTurn)
            {
                FailInteract();
                return;
            }
            //if(pointInteractableObject is TableCardMenuDeck tableCardDeck)
            //{
            //    //显示牌堆界面
            //}
            //print("下面是没有选中以及拖拽卡牌");
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
            
            

            if (pointInteractableObject is Table table)
            {
                table.UpdateTablePoint(hit.point + Vector3.up);
            }
            if (pointInteractableObject is Floor floor)
            {
                hitPosition = hit.point;
            }

            if (hitObject!=null&&hit.collider.gameObject == hitObject)
            {
                //相同的就不更新
                return; 
            }
            hitObject = hit.collider.gameObject;
            pointInteractableObject = hitObject.GetComponent<InteractableObject>() as IInteractable;
            
        }
        else
        {
            hitObject = null;
            pointInteractableObject = null;
        }
    }

    public void FailInteract()
    {
        //播放音效等等
    }
}
