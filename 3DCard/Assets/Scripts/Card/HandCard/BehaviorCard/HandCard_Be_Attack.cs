using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCard_Be_Attack : HandCardBase,IAttack
{


    public override void TryCardDragPlay(IInteractable pointInteractableObject)
    {
        switch(pointInteractableObject)
        {
            case IDamageable damageable:
                SingleAttack(damageable);
                break;

            default:
                FailDragPlay();
                break;
        }

        base.TryCardDragPlay(pointInteractableObject);
    }


    public void SingleAttack(IDamageable damageable)
    {
        Debug.Log("SingleAttack");
        //πÃ∂®…À∫¶
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
