namespace TreeViewer.TestUtilities
{
    /// <summary>
    /// Tryメソッドを表します。
    /// </summary>
    /// <typeparam name="TArg">引数の型</typeparam>
    /// <typeparam name="TResult">結果の型</typeparam>
    /// <param name="arg">引数</param>
    /// <param name="result">変換結果</param>
    /// <returns>処理に成功したら<see langword="true"/>，それ以外で<see langword="false"/></returns>
    public delegate bool TryOperation<in TArg, TResult>(TArg arg, out TResult result)
        where TArg : allows ref struct;

    /// <summary>
    /// Tryメソッドを表します。
    /// </summary>
    /// <typeparam name="TArg1">引数の型1</typeparam>
    /// <typeparam name="TArg2">引数の型2</typeparam>
    /// <typeparam name="TResult">結果の型</typeparam>
    /// <param name="arg1">引数1</param>
    /// <param name="arg2">引数2</param>
    /// <param name="result">変換結果</param>
    /// <returns>処理に成功したら<see langword="true"/>，それ以外で<see langword="false"/></returns>
    public delegate bool TryOperation<in TArg1, in TArg2, TResult>(TArg1 arg1, TArg2 arg2, out TResult result)
        where TArg1 : allows ref struct
        where TArg2 : allows ref struct;
}
