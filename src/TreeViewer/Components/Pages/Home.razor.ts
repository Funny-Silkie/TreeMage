/**
 * イベント類の登録を行います。
 * @param page 呼び出し元のページオブジェクト
 */
export function registerEvents(page: DotNetRazorComponent): void {
    bypassShortcuts(page);

    // 未保存時の確認
    window.onbeforeunload = async (e: BeforeUnloadEvent) => {
        const canClose: boolean = await page.invokeMethodAsync<boolean>("VerifyCanClose");
        console.log(canClose);
        if (!canClose) e.preventDefault();
        else {
            window.onbeforeunload = null;
            window.close();
        }
    };

    // Drag & Drop
    document.addEventListener("dragover", (e: DragEvent) => {
        e.preventDefault();
        e.stopPropagation();
    });

    document.addEventListener("drop", (e: DragEvent) => {
        e.preventDefault();
        e.stopPropagation();

        const data: DataTransfer | null = e.dataTransfer;
        if (data == null || data.files.length == 0) return;

        const pathes: Array<string> = [];
        for (const current of data.files) pathes.push((current as any).path);

        page.invokeMethodAsync("FileDropped", pathes);
    });
}

/**
 * Chromium内蔵のショートカットを無効化し，アプリ独自のものに切り替えます。
 * @param page 呼び出し元のページオブジェクト
 */
function bypassShortcuts(page: DotNetRazorComponent): void {
    document.addEventListener("keydown", e => onKeyDown(e, page));
}

/**
 * キーが押下された際に実行されます。
 * @param event イベント情報
 * @param page 呼び出し元のページオブジェクト
 */
function onKeyDown(event: KeyboardEvent, page: DotNetRazorComponent): void {
    const target: HTMLElement = event.target as HTMLElement;
    const isInput: boolean = target.tagName == "INPUT";

    // Ctrl+N
    if (event.ctrlKey && event.code == "KeyN") {
        event.preventDefault();

        page.invokeMethodAsync("CreateNew");
        return;
    }
    // Ctrl+Shift+Z or Ctrl+Y
    if (event.ctrlKey && (event.code == "KeyY" || (event.shiftKey && event.code == "KeyZ"))) {
        event.preventDefault();

        if (!isInput) page.invokeMethodAsync("Redo");
        return;
    }
    // Ctrl+Z
    if (event.ctrlKey && event.code == "KeyZ") {
        event.preventDefault();

        if (!isInput) page.invokeMethodAsync("Undo");
        return;
    }

    // textboxなどではショートカットを潰さないようにするために対象の要素に応じて挙動を切り替え
    if (!isInput) {
        // Ctrl+A
        if (event.ctrlKey && event.code == "KeyA") {
            event.preventDefault();

            page.invokeMethodAsync("FocusAll");
            return;
        }
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
