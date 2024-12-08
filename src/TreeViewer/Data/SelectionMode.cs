namespace TreeViewer.Data
{
    /// <summary>
    /// 選択対象を表します。
    /// </summary>
    public enum SelectionMode
    {
        /// <summary>
        /// 枝単体
        /// </summary>
        Node,

        /// <summary>
        /// クレード全体
        /// </summary>
        Clade,

        /// <summary>
        /// 系統名
        /// </summary>
        Taxa,
    }
}
