#region

//文件创建者：Egg
//创建时间：04-03 11:19

#endregion

namespace LearnBuff
{
    //Buff更新的策略模式控制符
    public enum BuffUpdateEnum
    {
        //增加持续时间
        AddTime,
        //刷新持续时间，增加层数
        ReplaceAndAddStack,
        //保留持续时间，增加层数
        KeepAndAddStack
    }
}