using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : InteractableObject,IDamageable
{
    public int maxHP;
    public int currentHP;
    public Dialogueable dialogueable;


    #region 生命周期函数
    private void Awake()
    {
        dialogueable = GetComponent<Dialogueable>();
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
        //测试对话系统
        if (GUILayout.Button("StartDialogue"))
        {
            dialogueable.StartDialogue();
        }
        if (GUILayout.Button("StopDialogue"))
        {
            dialogueable.StopDialogue();
        }
        if(GUILayout.Button("ResetDialogue"))
        {
            dialogueable.ResetDialogue();
        }
    }
    #endregion

    public override void Init()
    {
        currentHP = maxHP;
    }




    public void GetDamage(int damage)
    {
        currentHP -= damage;
    }
}
