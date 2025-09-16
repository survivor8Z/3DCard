#region

//文件创建者：Egg
//创建时间：04-03 11:24

#endregion

using System;

namespace LearnBuff
{
    //用于支持自定义传入委托
    public interface IBuffAction
    {
        void RegisterBuffAction(string callbackName, Action action);

        void TriggerBuffAction(string callbackName);
    }
}