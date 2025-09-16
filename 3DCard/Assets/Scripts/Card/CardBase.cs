using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardBase : SerializedMonoBehaviour
{
    public CardSO cardSO;
    /// <summary>
    /// 放入对象池时重置状态
    /// </summary>
    public virtual void ClearToInit()
    {

    }
}
