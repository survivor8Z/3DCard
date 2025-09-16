using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BMModDex", menuName = "Buff/Module/BMModDex")]
public class BMModDex : BuffModuleBase
{
    public override void Apply(BuffInfo buffInfo, params object[] customInfo)
    {
        ChaPropertyInfo targetChaInfo = buffInfo.target.GetComponent<ChaPropertyInfo>();
        if (targetChaInfo != null)
        {
            targetChaInfo.curDex = targetChaInfo.chaData.defaultDex+buffInfo.CurStack;
        }
    }
}
