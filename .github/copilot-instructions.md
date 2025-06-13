# C# 開発のためのカスタムインストラクション
- このリポジトリ（https://github.com/MamoruKuroda/Build25-BRK231）をローカル環境で再現し、C# コードを書く際のガイドラインです。
- C:\VSCode - Work\Build25-BRK231-ExternalID\Build25-BRK231\docs\readme.md を参照して、Microsoft Entra External ID および Azure AI Foundry に関する設定を実装内で定義してください。

## 1. 開発環境のセットアップ
- .NET 8.0 SDK 以上をインストールしてください。
- 推奨エディタ: Visual Studio 2022 または Visual Studio Code（C# 拡張機能を有効化）。
- リポジトリをクローン後、`dotnet restore` で依存関係を復元してください。

## 2. コーディング規約
- ファイル名、クラス名、メソッド名は PascalCase を使用してください。
- 変数名、引数名は camelCase を使用してください。
- 1 ファイル 1 クラスを原則としてください。
- コメントは日本語で簡潔に記述してください。

## 3. プロジェクト構成
- ソースコードは `src/` ディレクトリ配下に配置してください。
- 設定ファイル（例: appsettings.json）は `src/AgentAPI`、`src/WebApp`、`src/WoodgroveGroceriesApi` 直下に配置してください。2、3、4 についてはコードの確認や修正が必要と思われる。

## 4. ビルド・実行・テスト
- ビルド: `dotnet build`
- 実行: `dotnet run --project src/プロジェクト名`
- テスト: `dotnet test`

## 5. その他
- 依存パッケージの追加は `dotnet add package パッケージ名` を利用してください。
- コードの自動整形は `dotnet format` を推奨します。
- プルリクエスト時は必ずテストが通ることを確認してください。
- C:\VSCode - Work\Build25-BRK231-ExternalID\Build25-BRK231\docs\readme.md を参照して Microsoft Entra External ID および Azure AI Foundry に関する設定を実装内で定義すること

---
このガイドラインに従って、C# プロジェクトをローカルで再現・開発してください。