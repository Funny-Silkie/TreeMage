# クイックスタート <!-- omit in toc -->

- [ツリーの読み込み](#ツリーの読み込み)
- [樹形操作](#樹形操作)
  - [Select](#select)
  - [Reroot](#reroot)
  - [Swap](#swap)
  - [Subtree](#subtree)
- [表示メニュー](#表示メニュー)
  - [Layout](#layout)
  - [Tree](#tree)
  - [Search](#search)
  - [Leaf labels](#leaf-labels)
  - [Clade labels](#clade-labels)
  - [Node Values](#node-values)
  - [Branch values](#branch-values)
  - [Branch decorations](#branch-decorations)
  - [Scale bar](#scale-bar)
- [プロジェクトファイルの管理](#プロジェクトファイルの管理)
- [エクスポート](#エクスポート)


## ツリーの読み込み

メニューからの読み込みかファイルマネージャーからのdrag & dropでツリーを読み込むことができます。
メニューではFile→Import→Newickから読み込むことができます。

> [!IMPORTANT]
> 現段階ではNewickフォーマットのみ受け付けています。
> Nexus等の他フォーマットは今後実装予定です。

## 樹形操作

画面上部から編集モードを選択することができます。

- Select: リーフや枝を選択して編集することができます
- Reroot: リルートを行います
- Swap: 枝の入れ替えを行います
- Subtree: サブツリーを抜き出します

![Edit mode](images/edit_mode.png)

### Select

デフォルトのモードです。
枝やリーフの選択時に画面右のプロパティウィンドウで枝色などの情報を編集できます。

### Reroot
![Reroot](images/reroot.png)

白抜き四角（枝中）または黒塗り四角（結節点）をクリックすることでリルートすることができます。
白抜き四角をクリックするとrootedに，黒塗り四角をクリックするとunrootedな形でリルートできます。

### Swap
![Swap](images/swap.png)

結節点の黒塗り四角を選択すると上下の枝を交換できます。

### Subtree
![Subtree selection](images/subtree_1.png)

枝中の黒塗り四角を選択するとその枝から先を新たなツリーとして抜き出します。

以下が選択後のツリー
![Subtree result](images/subtree_2.png)

## 表示メニュー
![menu left](images/menu_left.png)
画面左のサイドバーから表示内容をカスタマイズすることができます。

### Layout
![Layout](images/menu_left_layout.png)

ツリー全体の表示方法について編集できます。
- Collapseボタン：枝選択時に押すと，選択中の枝を折りたたんで三角形の表示に変えられます。折りたたみ中の枝を選択中にもう一度押すと折りたたみを解除します
- OrderAボタン：枝の長さの昇順で枝の順番を並び替えます
- OrderDボタン：枝の長さの降順で枝の順番を並び替えます
- Collapse type：折りたたみの表示を変更します
  - TopMax：右上の頂点が最大の枝長に，右下の頂点が最小の枝長に対応します
  - BottomMax：右上の頂点が最小の枝長に，右下の頂点が最大の枝長に対応します
  - AllMax：右側の両頂点が最大の枝長に対応します
  - Constant：右側の両頂点が直下にある「Width」の枝長換算で配置されます

### Tree
![Tree](images/menu_left_tree.png)

ツリーの一般的な設定を編集できます。
- Scale X：横方向の拡大率
- Scale Y：縦方向の拡大率
- Branch thickness：枝の太さ
- Default branch length：枝長が未指定の場合に利用する枝長のダミー値

### Search
![Search](images/menu_left_search.png)

条件にマッチする系統を検索します。
- テキストボックス：検索文字列を入力します
- 虫眼鏡マーク：クリックで検索を行います
- Target：検索対象を選択します
  - Taxon：リーフ名
  - Supports：サポート値
- Ignore case：チェックを入れると大文字/小文字の区別を無視します
- Use regex：チェックを入れると正規表現で検索を行います

### Leaf labels
![Leaf labels](images/menu_left_leaf_labels.png)

リーフの表示を設定します。
- タイトル横のチェックボックス：表示の有無
- Font size：リーフのフォントサイズ

### Clade labels
![Clade labels](images/menu_left_clade_labels.png)

クレード名の表示を設定します。
- タイトル横のチェックボックス：表示の有無
- Font size：フォントサイズ
- Line thickness：クレード範囲を表す直線の太さ

### Node Values
![Node values](images/menu_left_node_values.png)

結節点の横の値の表示を設定します。

- タイトル横のチェックボックス：表示の有無
- Value: 表示する値
  - Supports：サポート値
  - BranchLength：枝長
- Font size：フォントサイズ

### Branch values
![Branch values](images/menu_left_branch_values.png)

枝の上に表示する値の表示を設定します。

- タイトル横のチェックボックス：表示の有無
- Value: 表示する値
  - Supports：サポート値
  - BranchLength：枝長
- Font size：フォントサイズ
- Hide pattern：指定した正規表現にマッチする値を非表示にします

### Branch decorations
![Branch decorations](images/menu_left_branch_decorations.png)

枝の装飾の表示を設定します。

- タイトル横のチェックボックス：表示の有無
- +ボタン：新たな装飾の設定を追加します。
- 装飾ごとの設定事項
  - Pattern：装飾を行う枝の値の条件（正規表現）
  - Shape：装飾の図形を選択します
    - Closed Circle：塗りつぶされた円
    - Open Circle：白抜きの円
    - Closed Rectangle：塗りつぶされた正方形
    - Open Rectangle：白抜きの正方形
  - Size：装飾のサイズ
  - Color：装飾の色
  - 目のボタン：表示/非表示を切り替えます
  - 赤いゴミ箱ボタン：設定を削除します

### Scale bar
![Scale bar](images/menu_left_scale_bar.png)

スケールバーの表示を設定します。

- タイトル横のチェックボックス：表示の有無
- Value: スケールバーとして表示する枝長の値
- Font size：枝長のフォントサイズ
- Bar thickness：バー部分の太さ

## プロジェクトファイルの管理
メニューのFile→SaveまたはCtrl+Sでこれまでの編集状況を，プロジェクトファイルとして保存することができます。
プロジェクトファイルは `.treeprj` の拡張子で表され，TreeMage独自のフォーマットとなっています。

> [!TIP]
> TREEPRJファイルの実体はJSONファイルをGZIP形式に圧縮したものとなっています。
> 自己責任とはなりますが，JSONファイルを編集してGZIP化することでTreeMage外で編集状況をいじることも可能です。

一度保存したプロジェクトファイルはメニューのFile→OpenまたはCtrl+O，ファイルマネージャーからのdrag & dropで読み込むことができます。

## エクスポート
TreeMageは編集した系統樹を，ツリーのファイルとしても，画像としても出力することができます。
メニューのFile→Exportからフォーマットを選択できます。
以下がサポートしているフォーマットです。

- Tree/Newick：Newickフォーマット（色などのスタイルの情報は保存できません）
- PNG
- SVG
- PDF

> [!IMPORTANT]
> 現状，PDF出力は技術的問題からレイヤー分け・グルーピングができていません。
> Adobe Illustrator等で編集を行いたい場合は**SVGとして出力**してから開くことを強く推奨します。
