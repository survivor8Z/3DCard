using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : InteractableObject,IDamageable
{
    public int maxHP;
    public int currentHP;


    #region 生命周期函数
    private void OnEnable()
    {
        Init();
    }
    #endregion

    public void Init()
    {
        currentHP = maxHP;
    }




    public void GetDamage(int damage)
    {
        currentHP -= damage;
    }
}
