#region

//文件创建者：Egg
//创建时间：04-03 11:20

#endregion

namespace LearnBuff
{
    public abstract class BuffCallback
    {
        public abstract void Apply(BuffRunTimeInfo info, params object[] customParams);
    }
}