using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : InteractableObject,IDamageable
{
    public int maxHP;
    public int currentHP;
    public Dialogueable dialogueable;


    #region �������ں���
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
        ////���ԶԻ�ϵͳ
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    dialogueable.StartDialogue();
        //}

    }
    private void OnGUI()
    {
        //���ԶԻ�ϵͳ
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
