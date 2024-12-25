namespace TreeViewer.Data
{
    /// <summary>
    /// <see cref="CladeId"/>におけるサフィックスを取得します。
    /// </summary>
    public enum CladeIdSuffix
    {
        /// <summary>
        /// サフィックスなし
        /// </summary>
        None,

        /// <summary>
        /// 枝
        /// </summary>
        Branch,

        /// <summary>
        /// 結節点
        /// </summary>
        Node,

        /// <summary>
        /// 葉
        /// </summary>
        Leaf,
    }
}
