#region

//文件创建者：Egg
//创建时间：04-03 11:24

#endregion

namespace LearnBuff
{
    //用于限制给BuffManager控制的变量被外界访问（参阅C#接口的显示实现）
    public interface IBuffTicker
    {
        float DurationTimer { get; set; }
        float TickTimer     { get; set; }
        int   CurStack      { get; set; }
    }
}