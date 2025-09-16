using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour,IDamageable
{
    private static Player _instance;
    public static Player Instance
    {
        get
        {
            return _instance;
        }
    }

    [HideInInspector]public PlayerInteract playerInteract;
    [HideInInspector]public PlayerMove playerMove;
    [HideInInspector]public PlayerTurnAround playerTurn;
    [HideInInspector]public CameraController cameraController;
    public BuffHandler buffHandler;
    public BuffView buffView;
    public ChaPropertyInfo chaInfo;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
            return;
        }
        _instance = this;
        playerInteract = GetComponent<PlayerInteract>();
        playerMove = GetComponent<PlayerMove>();
        playerTurn = GetComponent<PlayerTurnAround>();
        cameraController = GetComponent<CameraController>();
        buffHandler = GetComponent<BuffHandler>();
        buffView = GetComponent<BuffView>();
        chaInfo = GetComponent<ChaPropertyInfo>();
    }
    private void OnEnable()
    {
        Init();
    }
    private void Update()
    {
        //设置当前房间
        MapMgr.Instance.currentRoom 
            = MapMgr.Instance.roomsDict[MapMgr.Instance.WorldPosToRoomWorldCoorBig(transform.position)];

        //test
        if (Input.GetKeyDown(KeyCode.O))
        {
            MapMgr.Instance.currentRoom.door.ToggleDoor();
        }
        
    }

    private void OnGUI()
    {
        if (_instance != null)
        {
            //右上角按钮
            if (GUI.Button(new Rect(Screen.width - 110, 10, 100, 30), "增加力量10点"))
            {
                buffHandler.AddBuff("Strength", gameObject, 10);
                
            }
            if(GUI.Button(new Rect(Screen.width - 110, 50, 100, 30), "减少力量10点"))
            {
                //填入buff的id
                buffHandler.ReduceBuff(101,10);
            }
        }
    }
    public void Init()
    {
        chaInfo.Init();
    }
    public void GetDamage(DamageInfo damageInfo)
    {
        buffHandler.TriggerCustom(E_BuffCallBackType.BeforeGetDamage);
        switch (damageInfo.damageType)
        {
            case E_DamageType.Normal:
                int damge = damageInfo.damageValue - chaInfo.curDef;
                int toReducetHpDamage = damge - chaInfo.curArmor;
                if (toReducetHpDamage > 0)
                {
                    chaInfo.curArmor = 0;
                    int realDamage = toReducetHpDamage;
                    chaInfo.curHp -= realDamage;
                    buffHandler.TriggerCustom(E_BuffCallBackType.AfterGetDamage);
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
