#region

//文件创建者：Egg
//创建时间：04-03 01:27

#endregion

using System.Collections.Generic;

namespace LearnBuff
{
    //用于储存所有Buff配置数据，可自行实现自己的配置方式，这只是最简单的做法
    public static class BuffModel
    {
        private static readonly Dictionary<string, BuffData> _buffDatas = new()
        {
            {
                BuffConstant.BuffName.POISON, new BuffData
                {
                    Id                  = BuffConstant.BuffName.POISON,
                    Desc                = "中毒效果，最大层数为5层，每隔一段时间受到相当于层数的伤害",
                    Icon                = null, //自行实现
                    MaxStack            = 5,
                    IsForever           = false,
                    Duration            = 1f,
                    TickTime            = 1f,
                    BuffUpdateEnum      = BuffUpdateEnum.ReplaceAndAddStack,
                    BuffRemoveEnum      = BuffRemoveEnum.Reduce, //持续时间到后，降低层数，并刷新时间
                    TriggerTickOnCreate = false,
                    BuffModules = new List<BuffModule>
                    {
                        new()
                        {
                            CallbackName = BuffConstant.BuffCallback.ON_TICK,
                            Callback     = new CastDamageByStack(1f) //基础伤害为1
                        }
                    },
                    Tags = new List<string>
                    {
                        BuffConstant.BuffTag.DEBUFF,
                        BuffConstant.BuffTag.VISIBLE_IN_UI
                    }
                }
            },
            {
                BuffConstant.BuffName.CAST_POISON, new BuffData
                {
                    Id                  = BuffConstant.BuffName.CAST_POISON,
                    Desc                = "施毒Buff，玩家不可见，是毒气区域施加给玩家的Buff，离开毒气区域清楚",
                    Icon                = null,
                    MaxStack            = 1,
                    IsForever           = true,
                    Duration            = 0,
                    TickTime            = 0.5f,
                    BuffUpdateEnum      = BuffUpdateEnum.KeepAndAddStack,
                    BuffRemoveEnum      = BuffRemoveEnum.Clear,
                    TriggerTickOnCreate = false,
                    BuffModules = new List<BuffModule>
                    {
                        new()
                        {
                            CallbackName = BuffConstant.BuffCallback.ON_TICK,
                            Callback     = new CastBuff2Target(BuffConstant.BuffName.POISON)
                        }
                    },
                    Tags = new List<string>
                    {
                        BuffConstant.BuffTag.DEBUFF,
                        BuffConstant.BuffTag.AREA_BUFF
                    }
                }
            }
            //...etc,自行实现其他BuffData
        };

        public static BuffData GetBuffData(string buffId) => _buffDatas.GetValueOrDefault(buffId, null);
    }
}