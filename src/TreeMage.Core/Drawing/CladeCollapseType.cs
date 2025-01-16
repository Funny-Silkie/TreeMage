namespace TreeMage.Core.Drawing
{
    /// <summary>
    /// クレード折り畳みのタイプを表します。
    /// </summary>
    public enum CladeCollapseType
    {
        /// <summary>
        /// 上が最大枝長・下が最小枝長の三角形
        /// </summary>
        TopMax,

        /// <summary>
        /// 上が最小枝長・下が最大枝長の三角形
        /// </summary>
        BottomMax,

        /// <summary>
        /// 上下ともに最大枝長の三角形
        /// </summary>
        AllMax,

        /// <summary>
        /// 定数幅の三角形
        /// </summary>
        Constant,
    }
}
