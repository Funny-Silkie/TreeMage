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
 * .NETのオブジェクトを表します。
 */
interface DotNetObject {
    /**
     * 指定したメソッドを実行します。
     * @param name メソッド名
     * @param args 引数
     */
    invokeMethodAsync(name: string, ...args: any[]): Promise<void>;
}
