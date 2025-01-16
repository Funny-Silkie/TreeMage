namespace TreeMage.Data
{
    /// <summary>
    /// ツリーの編集モードを表します。
    /// </summary>
    public enum TreeEditMode
    {
        /// <summary>
        /// 通常モード
        /// </summary>
        Select,

        /// <summary>
        /// Rerootモード
        /// </summary>
        Reroot,

        /// <summary>
        /// 枝の入れ替えモード
        /// </summary>
        Swap,

        /// <summary>
        /// サブツリー取得
        /// </summary>
        Subtree,
    }
}
