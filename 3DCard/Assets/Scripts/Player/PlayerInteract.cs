using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public Player player;
    //����
    public HandCardDeck handCardDeck;
    //���߼��
    [SerializeField] private GameObject hitObject;
    public InteractableObject pointInteractableObject;
    public LayerMask interactableLayerMask;
    public Vector3 MouseWorldPosition => Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
    public Vector2 MouseViewPortPosition => new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);

    public float PlayableMouseY;
    #region �������ں���
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

    #endregion
    public void Interact()
    {
        int pointInteractableObjectID = pointInteractableObject==null?0: pointInteractableObject.id;

        //�������ӽ�
        if (player.playerMove.inSceneObj is Table table)
        {
            if (table.tableCardsControl.currentDragCard != null) return;
            
        }



        //�������

        //��ϴ��
        if (handCardDeck.dragedCard != null && handCardDeck.selectedCard != null)
        {
            if (MouseViewPortPosition.y < PlayableMouseY)
            {

                return;
            }

            handCardDeck.TryCardCombinationPlay(new CardCombinationPlayID(handCardDeck.dragedCard.cardSO.cardID,
                                                                            handCardDeck.selectedCard.cardSO.cardID,
                                                                            pointInteractableObjectID));
            return;
        }
        //��ק���
        if (handCardDeck.dragedCard != null)
        {
            if (MouseViewPortPosition.y < PlayableMouseY)
            {

                return;
            }

            handCardDeck.dragedCard.TryCardDragPlay(new CardDragedPlayID(handCardDeck.dragedCard.cardSO.cardID,
                                                                            pointInteractableObjectID));
            return;
        }
        //ѡ����
        if (handCardDeck.hoveredCard == null && handCardDeck.selectedCard != null 
            )
        {
            if (MouseViewPortPosition.y < PlayableMouseY)
            {

                return;
            }

            handCardDeck.selectedCard.TryCardSelectedPlay(new CardSelectedPlayID(handCardDeck.selectedCard.cardSO.cardID,
                                                                                    pointInteractableObjectID));
            return;
        }

        

        //������û��ѡ���Լ���ק����ʱ
        if (pointInteractableObject != null)
        {
            if(pointInteractableObject is TableCardMenuDeck tableCardDeck)
            {
                //��ʾ�ƶѽ���
            }
            
        }
        
    }


    //����ָ��������,���߼��
    public void UpdatePointInteractableObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, (MouseWorldPosition-Camera.main.transform.position).normalized, out hit, 100f,interactableLayerMask))
        {
            //����debug
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);

            if(pointInteractableObject is Table table)
            {
                table.UpdateTablePoint(hit.point + Vector3.up);
            }

            if (hitObject!=null&&hit.collider.gameObject == hitObject)
            {
                //��ͬ�ľͲ�����
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
    
}
