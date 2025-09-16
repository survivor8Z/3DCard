using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_DamageType
{
    Normal,//∆’Õ®…À∫¶
    Rebound,//∑¥µØ
    Puncture,//¥©¥Ã
}

public class DamageInfo
{
    public IAttack attacker;
    public IDamageable receiver;
    public int damageValue;
    public E_DamageType damageType;
    public IAddDamage addDamage;
    public DamageInfo()
    {
        attacker = null;
        receiver = null;
        damageValue = 0;
        damageType = E_DamageType.Normal;
        addDamage = null;
    }
    public DamageInfo(IAttack attacker, IDamageable receiver, int damageValue, E_DamageType damageType=E_DamageType.Normal, IAddDamage addDamage = null)
    {
        this.attacker = attacker;
        this.receiver = receiver;
        this.damageType = damageType;
        this.damageValue = damageValue;
        this.addDamage = addDamage;
    }
}
