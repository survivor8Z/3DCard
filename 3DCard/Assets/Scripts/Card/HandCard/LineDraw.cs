using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDraw : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Vector3 startPoint;
    private Vector3 endPoint;
    [SerializeField]private HandCardDeck handCardDeck;
    [SerializeField]private float lineWidth = 0.01f;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        DrawLine();
    }

    private void DrawLine()
    {
        if (handCardDeck.dragedCard!=null 
            && handCardDeck.dragedCard.cardSO.cardType == E_CardType.E_Entity 
            && handCardDeck.dragedCard.handCardDeck.player.playerMove.inSceneObj is Table)
        {
            return;
        }
            if (handCardDeck.dragedCard == null)
        {
            lineRenderer.positionCount = 0; // No line to draw
            return;
        }
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.SetPosition(0, handCardDeck.dragedCard.transform.position);
        lineRenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,1f)));

    }

    private void OnDestroy()
    {
        
    }
}
