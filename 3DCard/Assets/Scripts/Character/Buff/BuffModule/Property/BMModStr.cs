using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BMModStr", menuName = "Buff/Module/BMModStr")]
public class BMModStr : BuffModuleBase
{
    public override void Apply(BuffInfo buffInfo, params object[] customInfo)
    {
        ChaPropertyInfo targetChaInfo = buffInfo.target.GetComponent<ChaPropertyInfo>();
        if (targetChaInfo != null)
        {
            targetChaInfo.curStr = targetChaInfo.chaData.defaultStr + buffInfo.CurStack;
            Debug.Log(buffInfo.CurStack);
        }
    }
}
