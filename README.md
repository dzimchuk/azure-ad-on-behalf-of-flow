# azure-ad-on-behalf-of-flow

A sample solution demonstrating how you can configure your ASP.NET Core 5 applications and services to use [On-Behalf-Of](https://tools.ietf.org/html/draft-ietf-oauth-token-exchange-02) flow when you need to carry the user identity across multiple service boundaries. The solution contains a web application (TestApp) calling a web API service (TestServiceA) which in turns calls another backing service (TestServiceB). All calls are done under the signed in user's security context.

The solution uses Microsoft identity platform (Azure AD) as its identity provider.

Please find more details in the accompanying [blog post](https://dzimchuk.net/using-the-on-behalf-of-flow-in-your-aspnet-core-services-protected-by-azure-ad/).

# Configuration

## Web App (TestApp)

```
"TestApp": {
  "Authentication": {
    "AzureAd": {
      "Instance": "e.g. https://login.microsoftonline.com/",
      "TenantId": "",
      "ClientId": "",
      "ClientSecret": "",
      "CallbackPath": "/signin-oidc",
      "SignedOutCallbackPath": "/signout-oidc",
    }
  },
  "TestServiceA": {
    "BaseUrl": "https://localhost:5001",
    "Scopes": "api://[ClientId of TestServiceA, e.g. 2ec40e65-ba09-4853-bcde-bcb60029e596]/access_as_user"
  }
}
```

## TestServiceA

```
"TestServiceA": {
  "Authentication": {
    "AzureAd": {
      "Instance": "e.g. https://login.microsoftonline.com/",
      "TenantId": "",
      "ClientId": "",
      "ClientSecret": ""
    }
  },
  "TestServiceB": {
    "BaseUrl": "https://localhost:5002",
    "Scopes": "api://[ClientId of TestServiceB, e.g. 2ec40e65-ba09-4853-bcde-bcb60029e596]/access_as_user"
  }
}
```

## TestServiceB

```
"TestServiceB": {
  "Authentication": {
    "AzureAd": {
      "Instance": "e.g. https://login.microsoftonline.com/",
      "TenantId": "",
	  "ClientId": ""
    }
  }
}
```
