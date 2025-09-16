using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BMModDef", menuName = "Buff/Module/BMModDef")]
public class BMModDef : BuffModuleBase
{
    public override void Apply(BuffInfo buffInfo, params object[] customInfo)
    {
        ChaPropertyInfo targetChaInfo = buffInfo.target.GetComponent<ChaPropertyInfo>();
        if (targetChaInfo != null)
        {
            targetChaInfo.curDef = targetChaInfo.chaData.defaultDex + buffInfo.CurStack;
        }
    }
}
