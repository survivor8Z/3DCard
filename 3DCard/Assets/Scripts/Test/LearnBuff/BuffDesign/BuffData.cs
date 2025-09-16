#region

//文件创建者：Egg
//创建时间：04-03 11:18

#endregion

using System.Collections.Generic;
using UnityEngine;

namespace LearnBuff
{
    public sealed class BuffData
    {
        //通用信息
        public string Id;
        public string Desc;
        public Sprite Icon;

        public int   MaxStack = 1;  //最大堆叠上限
        public bool  IsForever;     //策略模式控制参数，是否为永久Buff，控制Buff是否需要更新持续时间
        public float Duration;      //持续时间
        public float TickTime = -1; //Tick间隔

        public BuffUpdateEnum BuffUpdateEnum; //策略模式控制参数，控制Buff如何更新
        public BuffRemoveEnum BuffRemoveEnum; //策略模式控制参数，控制Buff如何移除

        public bool TriggerTickOnCreate; //策略模式控制参数，是否在OnCreate的时候也执行一次OnTick

        public List<BuffModule> BuffModules = new(); //Buff模块

        public List<string> Tags = new();//自定义标签
        
        public bool ExistBuffModule(string callbackName)
        {
            return BuffModules.Exists(buff => buff.CallbackName == callbackName);
        }

        public bool ExistTag(string tag)
        {
            return Tags.Contains(tag);
        }
        
        public void TriggerBuffModule(string callbackName, BuffRunTimeInfo runTimeInfo, params object[] paramList)
        {
            runTimeInfo.TriggerBuffAction(callbackName);
            
            foreach (var buffModule in BuffModules)
            {
                if (buffModule.CallbackName == callbackName)
                {
                    buffModule.Callback?.Apply(runTimeInfo, paramList);
                }
            }
        }
    }
}