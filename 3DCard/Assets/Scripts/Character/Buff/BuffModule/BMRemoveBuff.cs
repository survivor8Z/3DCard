using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BMRemoveBuff : BuffModuleBase
{
    public override void Apply(BuffInfo buffInfo, params object[] customInfo)
    {
        BuffHandler buffHandler = buffInfo.target.GetComponent<BuffHandler>();
        if(buffHandler != null)
        {
            buffHandler.RemoveBuff(buffInfo.buffData.id);
        }
    }
}
