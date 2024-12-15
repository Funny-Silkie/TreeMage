/**
 * .NETのRazorコンポーネントを表します。
 */
interface DotNetRazorComponent {
    /**
     * 指定したメソッドを実行します。
     * @param name メソッド名
     * @param args 引数
     */
    invokeMethodAsync(name: string, ...args: any[]): Promise<void>;
}
