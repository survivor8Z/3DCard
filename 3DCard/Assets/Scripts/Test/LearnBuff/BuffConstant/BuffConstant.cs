#region

//文件创建者：Egg
//创建时间：04-03 11:19

#endregion

namespace LearnBuff
{
    public static class BuffConstant
    {
        public static class BuffCallback
        {
            public const string ON_TICK         = "OnTick";
            public const string ON_CREATE       = "OnCreate";
            public const string ON_REMOVE       = "OnRemove";
            public const string ON_ADD_STACK    = "OnAddStack";
            public const string ON_REDUCE_STACK = "OnReduceStack";

            // 业务相关的回调时机可以在这里补充
            // public const string ON_KILL = "OnKill";
            // public const string ON_BE_KILL = "OnBeKill";
            // public const string ON_HIT = "OnHit";
            // public const string ON_BE_HIT = "OnBeHit";
        }
        
        public static class BuffName
        {
            //施毒Buff
            public const string CAST_POISON = "CastPoison";
            //中毒Buff
            public const string POISON = "Poison";
            //施电Buff
            public const string CAST_ELECTRIC = "CastElectric";
            //电击Buff
            public const string ELECTRIC = "Electric";
            
            //...etc,建议每一个Buff都添加常量
        }
        
        public static class BuffTag
        {
            //是否在UI层可见
            public const string VISIBLE_IN_UI = "VisibleInUI";
            //是否是减益Buff
            public const string DEBUFF = "Debuff";
            //是否是增益Buff
            public const string BOOST_BUFF = "BoostBuff";
            //是否是区域性Buff（施毒和施电）
            public const string AREA_BUFF = "AreaBuff";
            
            // //战斗相关
            // public const string BATTLE_RELATED = "BattleRelated";
            // //资源相关
            // public const string RESOURCE_RELATED = "ResourceRelated";
            // ...etc tag的用法非常广泛，而且比较复杂
            // 比如设计一个提升所有战斗类Buff的Buff，你不能让'经验掉落加成'这个Buff也受到增益
            // 其中一个做法是通过tag筛选出所有含有BoostBuff和BattleRelated标签的Buff进行影响
        }
    }
}