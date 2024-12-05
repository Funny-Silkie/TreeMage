using System.ComponentModel;
using System.Runtime.Serialization;

namespace TreeViewer.Core.Trees.Parsers
{
    /// <summary>
    /// 系統樹のフォーマットが無効である旨の例外を表します。
    /// </summary>
    [Serializable]
    public class TreeFormatException : Exception
    {
        /// <summary>
        /// <see cref="TreeFormatException"/>の新しいインスタンスを初期化します。
        /// </summary>
        public TreeFormatException()
        {
        }

        /// <summary>
        /// <see cref="TreeFormatException"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        public TreeFormatException(string? message) : base(message)
        {
        }

        /// <summary>
        /// <see cref="TreeFormatException"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <param name="inner">内部例外</param>
        public TreeFormatException(string? message, Exception? inner) : base(message, inner)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Binary serialization is obsolete")]
        protected TreeFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
