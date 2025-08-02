using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector; // ����Odin�����ռ�

public struct CombinationKey
{
    public int card1ID;
    public int card2ID;
    public CombinationKey(int card1ID, int card2ID)
    {
        this.card1ID = card1ID;
        this.card2ID = card2ID;
    }
}

    [CreateAssetMenu(fileName = "CardInteractSO", menuName = "CardInteractSO")]
public class CardInteractSO : ScriptableObject
{
    [ShowInInspector,DictionaryDrawerSettings(KeyLabel = "���", ValueLabel = "Ч��")] 
    public Dictionary<CombinationKey, HandCardBase> CardInteractions=new Dictionary<CombinationKey, HandCardBase>();


    //����ȫ�Ƿ���
    
}

