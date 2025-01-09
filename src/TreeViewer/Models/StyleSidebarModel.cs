namespace TreeViewer.Models
{
    /// <summary>
    /// スタイル編集サイドバー用のModelのクラスです。
    /// </summary>
    public class StyleSidebarModel : ModelBase
    {
        private readonly MainModel mainModel;

        /// <summary>
        /// <see cref="StyleSidebarModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public StyleSidebarModel(MainModel mainModel)
        {
            this.mainModel = mainModel;
        }
    }
}
