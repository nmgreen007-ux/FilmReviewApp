# AI Summary Service Setup Guide

## Overview

The AI Summary Service generates AI-powered summaries for films using Azure OpenAI via the Azure AI Foundry platform. This service provides concise, engaging descriptions of films based on their plot summaries.

## Prerequisites

- An Azure subscription
- Access to [Azure AI Foundry portal](https://ai.azure.com)
- Sufficient quota to deploy an Azure OpenAI model in your chosen region

## Azure Setup

### Step 1: Create an Azure AI Foundry Resource

1. Sign in to the [Azure AI Foundry portal](https://ai.azure.com) using your Azure credentials
2. From the home page, select **+ Create project**
3. Choose or create a **Foundry resource** (this replaces the older "Azure OpenAI" Cognitive Services resource)
4. Provide a resource name, select your **subscription**, **resource group**, and **region**
5. Select **Create** and wait for the resource and project to be provisioned

> **Note:** If you have an existing Azure OpenAI resource, you can upgrade it to a Foundry resource directly from the Azure portal. On your resource's overview page, look for the banner *"Want to try the latest industry models and Agents?"* and follow the upgrade steps. Your existing endpoint, API keys, and configurations are preserved.

### Step 2: Deploy a Model

1. In the Foundry portal, navigate to your project
2. In the left-hand menu, select **Models + endpoints** (under *My assets*)
3. Select **+ Deploy model** and choose `gpt-5-nano`
4. Give the deployment a name — this becomes your **Deployment ID** used in configuration
5. Set an appropriate **Tokens per minute** rate limit and select **Deploy**
6. Once provisioning completes, the deployment status will show as **Succeeded**

### Step 3: Retrieve Your Endpoint and API Key

1. In your Foundry project, go to **Management centre** in the left-hand navigation
2. Under your project, select **Go to project**, then view the **Endpoints and keys** section on the project overview page
3. Note the **Azure OpenAI in Foundry models** endpoint URL and one of the API keys — you will need these for configuration

## Application Configuration

Add your Azure AI Foundry credentials to `appsettings.json`:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://<your-foundry-resource>.openai.azure.com/",
    "ApiKey": "<your-api-key>",
    "DeploymentId": "gpt-5-nano"
  }
}
```

For local development, use user secrets instead of storing credentials in source control:

```bash
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://<your-foundry-resource>.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:ApiKey" "<your-api-key>"
dotnet user-secrets set "AzureOpenAI:DeploymentId" "gpt-5-nano"
```

## Security Considerations

- Never commit API keys to version control
- Use **Azure Key Vault** for production environments
- Where possible, use **managed identities** instead of API keys — the Foundry portal supports this natively
- Monitor API usage regularly for unexpected patterns or cost spikes

## Troubleshooting

| Issue | Solution |
|-------|----------|
| "Azure OpenAI configuration is not set" | Add credentials to `appsettings.json` or user secrets |
| 401 Unauthorized | Verify your API key is correct and has not expired |
| 404 Not Found | Verify the endpoint URL and deployment name match exactly |
| Timeout errors | Check network connectivity and Azure service health |
| Rate limit errors | Implement exponential backoff retry logic, or increase your TPM quota in the Foundry portal |

## Resources

- [Azure AI Foundry Portal](https://ai.azure.com)
- [Azure AI Foundry Documentation](https://learn.microsoft.com/en-us/azure/ai-foundry/)
- [Upgrading Azure OpenAI to Foundry](https://learn.microsoft.com/en-us/azure/ai-foundry/how-to/upgrade-azure-openai)
- [Azure OpenAI Service Documentation](https://learn.microsoft.com/en-us/azure/ai-services/openai/overview)
