#region

//文件创建者：Egg
//创建时间：04-03 11:23

#endregion

using System;
using System.Collections.Generic;
using UnityEngine;

namespace LearnBuff
{
    //Buff运行时数据
    public sealed class BuffRunTimeInfo : IBuffTicker, IBuffAction
    {
        public BuffData   BuffData;//Buff配置数据
        public GameObject Creator;
        public GameObject Target;
        public float      DurationTimer => ((IBuffTicker)this).DurationTimer;
        public float      TickTimer     => ((IBuffTicker)this).TickTimer;
        public int        CurStack      => ((IBuffTicker)this).CurStack;
        //为了避免外界意外修改DurationTimer等值（一般情况下只能由BuffManager管理），使用接口的显示实现降低访问权限
        float IBuffTicker.DurationTimer { get; set; }
        float IBuffTicker.TickTimer     { get; set; }
        int IBuffTicker.  CurStack      { get; set; } = 1;
        private Dictionary<string, Action> _buffActions { get; } = new();
        
        //支持自定义运行时委托注册，提高灵活性
        public void RegisterBuffAction(string callbackName, Action action)
        {
            if (!_buffActions.TryAdd(callbackName, action))
            {
                _buffActions[callbackName] += action;
            }
        }
        

        public void TriggerBuffAction(string callbackName)
        {
            _buffActions.TryGetValue(callbackName, out var action);
            action?.Invoke();
        }
    }
}