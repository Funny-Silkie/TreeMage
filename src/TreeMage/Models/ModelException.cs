namespace TreeMage.Models
{
    /// <summary>
    /// Model処理上の例外を表します。
    /// </summary>
    public sealed class ModelException : Exception
    {
        public ModelException(string? message) : base(message)
        {
        }

        public ModelException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
