using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCard_Be_Attack : HandCard_Behavior, IAttack
{


    public override void TryCardDragPlay(IInteractable pointInteractableObject)
    {
        if (handCardDeck.player.playerMove.inSceneObj == null)
        {
            switch (pointInteractableObject)
            {
                case IDamageable damageable:
                    SingleAttack(damageable);
                    break;

                default:
                    FailDragPlay();
                    break;
            }
        }
        else if(handCardDeck.player.playerMove.inSceneObj is Table)
        {
            switch (pointInteractableObject)
            {
                case Table://如果是桌子拖到桌面
                    PlaceCard();
                    break;
                case TableCardBase://如果是拖到桌牌上
                    PlaceToTableCard(pointInteractableObject as TableCardBase);
                    break;
            }
        }
        

        base.TryCardDragPlay(pointInteractableObject);
    }


    public void SingleAttack(IDamageable damageable)
    {
        Debug.Log("SingleAttack");
        //固定伤害
        damageable.GetDamage(1);
    }

    public void Attack(IDamageable damageable)
    {
        SingleAttack(damageable);
    }

    public void AttackWithAddDamage(IDamageable damageable, IAddDamage addDamage)
    {
        damageable.GetDamage(1 + addDamage.AddDamage);
    }
}
