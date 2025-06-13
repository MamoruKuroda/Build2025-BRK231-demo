## 開発環境
- このソースコードは Github のリポジトリ [Build2025-BRK231](https://github.com/microsoft/Build25-BRK231) からクローンしたもの

## 目的
- Microsoft Entra External ID をつかい、BtoC向けのECサイトでチャットUIを用意し、ユーザーの入力つまりプロンプトに応じて動作するエージェントを構成する。
- エージェントは Azure AI Foundry でデプロイされた LLM のモデルのエンドポイントを利用する。
- Microsoft Entra External ID の技術的なドキュメントは [MSLearn - Microsoft Entra External ID Overview](https://learn.microsoft.com/ja-jp/entra/external-id/external-identities-overview)を参照すること。

## 環境情報
### Azure AI Foundry
- Endpoint: "https://`<deploymentname>`.openai.azure.com/openai/deployments/gpt-4.1/chat/completions?api-version=2025-01-01-preview"
- Api-Key: "`<Key Value>`"
- Model: "`<model name>`"
- Azure AI Foundry の設定は src\AgentAPI\OpenAIOptions.cs ハードコーディングするが必要に応じて Azure Key Vault や App Service の環境変数を利用すること

### Microsoft Entra External ID Info
- Tenant: `<Entra External ID のサブドメイン名>`.ciamlogin.com
- AgentAPI Client ID: `<Agent API の Client ID>`
- AgentAPI Secret: `<Agent API の Secret>`
- WoodgroveGroceries Client ID: `<WoodgroveGroceries の Client ID>`
- WoodgroveGroceries Secret: `<WoodgroveGroceries の Secret>`
- WebAPI Client ID: `<WebAPI の Client ID>`
- WebAPI Secret: `<WebAPI の Secret>`

### Configuration Steps
1. create external tenant
    1. create an extenal id tenant
2. App Registrations
    2. register an app for the Woodgrove Groceries API
        2. create client secret
    3. register an app for the Agent API
        2. create client secret
    4. register an app for the WebApp.
        2. create client secret
3. User Flows
    1. create extension fields
        2. date of birth
        3. dietary restrictions
        4. Name?? 
    1. register a SUSI flow for the Web App
        2. configure extension attributes for sign-up flow
4. Permissions
    1. expose apis / scopes in woodgrove groceries apis
    2. give permissions for the woodgrove apis to the Agent API
        3. Give admin consent to woodgrove groceries API in Agent App
        4. configure token to include extension attributes in woodgrove groceries API
    3. expose agent api permissions / scope in Agent AI
    4. configure Agent API to receive extension fields --> 
    5. give permissions to web app to consume Agent API permissions
        6. give admin consent 
        7. configure web app to receive extension attributes
5. configure appsettings in APIs to reflect ClientId / TenantID
