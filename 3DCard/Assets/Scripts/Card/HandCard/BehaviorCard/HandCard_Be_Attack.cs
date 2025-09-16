using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// cardSO中,values[0]是花费,values[1]是伤害值
/// </summary>
public class HandCard_Be_Attack : HandCard_Behavior, IAttack
{
    public int costValue => cardSO.values[0];
    public int attackValue => cardSO.values[1];
    public override void TryCardDragPlay(IInteractable pointInteractableObject)
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
        //if (handCardDeck.player.playerMove.inSceneObj == null)
        //{
            
        //}
        //else if(handCardDeck.player.playerMove.inSceneObj is Table)
        //{
        //    switch (pointInteractableObject)
        //    {
        //        case Table://如果是桌子拖到桌面
        //            PlaceCard();
        //            break;
        //        case TableCardBase://如果是拖到桌牌上
        //            PlaceToTableCard(pointInteractableObject as TableCardBase);
        //            break;
        //    }
        //}
        

        base.TryCardDragPlay(pointInteractableObject);
    }


    public void SingleAttack(IDamageable damageable)
    {
        Debug.Log("SingleAttack");
        DamageInfo damageInfo
            = new DamageInfo(this, damageable, attackValue+Player.Instance.chaInfo.curStr);
        Player.Instance.buffHandler.TriggerCustom(E_BuffCallBackType.BeforeDoDamage,damageInfo);
        damageable.GetDamage(damageInfo);
        Player.Instance.buffHandler.TriggerCustom(E_BuffCallBackType.AfterDoDamage, damageInfo);//一般是用来移除buff的
    }

    public void Attack(IDamageable damageable)
    {
        SingleAttack(damageable);
    }

    //public void AttackWithAddDamage(IDamageable damageable, IAddDamage addDamage)
    //{
    //    damageable.GetDamage(1 + addDamage.AddDamage);
    //}
}
