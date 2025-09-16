using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : InteractableObject,IDamageable
{
    public ChaPropertyInfo chaInfo;
    public Dialogueable dialogueable;
    public BuffHandler buffHandler;
    


    #region 生命周期函数
    private void Awake()
    {
        buffHandler = GetComponent<BuffHandler>();
        dialogueable = GetComponent<Dialogueable>();
        chaInfo = GetComponent<ChaPropertyInfo>();
    }
    private void OnEnable()
    {
        Init();
    }
    private void Update()
    {
        ////测试对话系统
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    dialogueable.StartDialogue();
        //}

    }
    private void OnGUI()
    {
        ////测试对话系统
        //if (GUILayout.Button("StartDialogue"))
        //{
        //    dialogueable.StartDialogue();
        //}
        //if (GUILayout.Button("StopDialogue"))
        //{
        //    dialogueable.StopDialogue();
        //}
        //if(GUILayout.Button("ResetDialogue"))
        //{
        //    dialogueable.ResetDialogue();
        //}
    }
    #endregion

    public override void Init()
    {
        base.Init();
        chaInfo.Init();
    }




    public void GetDamage(DamageInfo damageInfo)
    {
        buffHandler.TriggerCustom(E_BuffCallBackType.BeforeGetDamage,damageInfo);
        switch (damageInfo.damageType)
        {
            case E_DamageType.Normal:
                int damge = damageInfo.damageValue - chaInfo.curDef;
                if (damge < 0)
                    damge = 0;
                int toReducetHpDamage = damge - chaInfo.curArmor;
                if (toReducetHpDamage > 0)
                {
                    chaInfo.curArmor = 0;
                    int realDamage = toReducetHpDamage;
                    chaInfo.curHp -= realDamage;
                    buffHandler.TriggerCustom(E_BuffCallBackType.AfterGetDamage,damageInfo);
                    if (chaInfo.curHp <= 0)
                    {
                        chaInfo.curHp = 0;
                        //Die
                        Debug.Log(this.gameObject.name + " Die");
                    }
                }
                else
                {
                    chaInfo.curArmor -= damge;
                }
                break;
            case E_DamageType.Puncture:
                int damage = damageInfo.damageValue - chaInfo.curDef;
                chaInfo.curHp -= damage;
                buffHandler.TriggerCustom(E_BuffCallBackType.AfterGetDamage);
                if (chaInfo.curHp <= 0)
                {
                    chaInfo.curHp = 0;
                    //Die
                    Debug.Log(this.gameObject.name + " Die");
                }
                break;
        }
    }
}
