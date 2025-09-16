using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCard_Be_Defend : HandCard_Behavior
{
	public override void TryCardDragPlay(IInteractable pointInteractableObject)
    {
        base.TryCardDragPlay(pointInteractableObject);
    }

    public override void TryCardSelectedPlay(IInteractable pointInteractableObject)
    {
        base.TryCardSelectedPlay(pointInteractableObject);
    }
}
