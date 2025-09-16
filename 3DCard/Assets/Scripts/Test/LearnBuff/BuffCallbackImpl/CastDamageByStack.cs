#region

//文件创建者：Egg
//创建时间：04-03 11:35

#endregion

using UnityEngine;

namespace LearnBuff
{
    //通过构造函数传入BuffConfigParam，跟Action的闭包本质是相同的，都是捕获外部变量形成一个类
    public sealed class CastDamageByStack : BuffCallback
    {
        public readonly float BaseValue;

        public CastDamageByStack(float baseValue)
        {
            BaseValue = baseValue;
        }
        
        public override void Apply(BuffRunTimeInfo info, params object[] customParams)
        {
            //没有具体实现，给出可能的示例代码
            // var target = info.Target;
            // var healthController = target.GetComponent<HealthController>();
            // healthController.Damage(info.CurStack * BaseValue);
            Debug.Log($"造成伤害 {info.CurStack * BaseValue}");
        }
    }
}