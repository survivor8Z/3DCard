using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 给attack增加伤害
/// </summary>
public interface IAddDamage
{
    public int AddDamage
    {
        get;
    }
}
/// <summary>
/// 不是很适用,为了适合当前继承的设计,现在就只是让InteractableObject继承了这个接口
/// </summary>
public interface IInteractable
{
    public int InteractableID
    {
        get;
    }
    public void React();
}

/// <summary>
/// 可以被攻击,计算承伤
/// </summary>
public interface IDamageable
{
    public void GetDamage(int damage);
}


public interface IAttack
{
    public void Attack(IDamageable damageable);

    public void AttackWithAddDamage(IDamageable damageable,IAddDamage addDamage);
}

/// <summary>
/// 可以转换成手牌
/// </summary>
public interface ICardable
{
    public CardSO cardSO
    {
        get;
    }
    public void ToCard();
}