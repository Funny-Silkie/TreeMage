/**
 * Chromium内臓のショートカットを無効化し，アプリ独自のものに切り替えます。
 * @param page 呼び出し元のページオブジェクト
 */
export function bypassShortcuts(page: DotNetRazorComponent): void {
    document.addEventListener("keydown", e => onKeyDown(e, page));
}

/**
 * キーが押下された際に実行されます。
 * @param event イベント情報
 * @param page 呼び出し元のページオブジェクト
 */
function onKeyDown(event: KeyboardEvent, page: DotNetRazorComponent): void {
    // Ctrl+N
    if (event.ctrlKey && event.code == "KeyN") {
        event.preventDefault();

        page.invokeMethodAsync("CreateNew");
    }
}

/**
 * SVGのtextのサイズを取得します。
 * @param id 検索するSVG要素のID
 * @returns idに対応するSVG textのサイズ，存在しない場合はNaN
 */
export function getTextSize(id: string): [number, number] {
    console.log(id);

    const element: ChildNode | null = document.getElementById(id);
    if (element == null || !(element instanceof SVGTextElement)) return [NaN, NaN];

    const bBox: DOMRect = element.getBBox();
    return [bBox.width, bBox.height];
}

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
