# 開発者向け情報 <!-- omit in toc -->

- [要求](#要求)
- [ビルド](#ビルド)
  - [プロジェクト全体](#プロジェクト全体)
  - [GUIアプリバイナリ生成](#guiアプリバイナリ生成)
  - [テスト実行](#テスト実行)
  - [デバッグ](#デバッグ)


## 要求
**開発を行う際に**必要なものです。

- .NET SDK: .NET9, .NET8 & .NET6
- npm: バージョン16.17.1以上
- ElectronNET.CLI

## ビルド

### プロジェクト全体

`src` ディレクトリで以下のコマンドを実行
```sh
dotnet build
```

### GUIアプリバイナリ生成

`src/TreeMage` ディレクトリで以下のコマンドを実行
```sh
electronize build /target win # win-x64
electronize build /target linux # linux-x64
electronize build /custom "linux-arm64;linux" /electron-arch arm64 # linux-arm64
electronize build /custom "osx-arm64;osx" /electron-arch arm64 # osx-arm64
```

### テスト実行

`src` ディレクトリで以下のコマンドを実行
```sh
dotnet test -c TEST
```

### デバッグ

`src/TreeMage` ディレクトリで以下のコマンドを実行
※ VisualStudioのデバッグでも代替可能
```sh
electronize start
```
