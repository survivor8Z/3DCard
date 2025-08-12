using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��attack�����˺�
/// </summary>
public interface IAddDamage
{
    public int AddDamage
    {
        get;
    }
}
/// <summary>
/// ���Ǻ�����,Ϊ���ʺϵ�ǰ�̳е����,���ھ�ֻ����InteractableObject�̳�������ӿ�
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
/// ���Ա�����,�������
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
/// ����ת��������
/// </summary>
public interface ICardable
{
    public CardSO cardSO
    {
        get;
    }
    public void ToCard();
}