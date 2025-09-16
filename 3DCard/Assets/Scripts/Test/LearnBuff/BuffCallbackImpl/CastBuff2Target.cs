#region

//文件创建者：Egg
//创建时间：04-03 01:43

#endregion

using UnityEngine;

namespace LearnBuff
{
    public sealed class CastBuff2Target : BuffCallback
    {
        public readonly string BuffId;
        
        public CastBuff2Target(string buffId)
        {
            BuffId = buffId;
        }
        public override void Apply(BuffRunTimeInfo info, params object[] customParams)
        {
            var target      = info.Target;
            var buffManager = target.GetComponent<BuffManager>();
            buffManager.AddBuff(BuffId, info.Creator);
            Debug.Log($"{info.BuffData.Id} 施加了一个Buff {BuffId}");
        }
    }
}