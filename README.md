# azure-ad-on-behalf-of-flow

A sample solution demonstrating how you can configure your ASP.NET Core applications and services to use [OAuth 2.0 Token Exchange](https://tools.ietf.org/html/draft-ietf-oauth-token-exchange-02) (On-Behalf-Of) flow when you need to carry the user identity across multiple service boundaries. The solution contains a web application (TestApp) calling a web API service (TestServiceA) which in turns calls another backing service (TestServiceB). All calls are done under the signed in user's security context.

The solution uses Azure AD as its identity provider.

# Configuration

## Web App (TestApp)

```
"TestApp": {
  "Authentication": {
    "AzureAd": {
      "Instance": "e.g. https://login.microsoftonline.com/",
      "TenantId": "e.g. <your domain>.onmicrosoft.com>",
      "ClientId": "",
      "ClientSecret": "",
      "PostLogoutRedirectUri": "https://localhost:44349/"
    }
  },
  "TestService": {
    "BaseUrl": "https://localhost:44339/",
    "Resource": "https://devunleashed.onmicrosoft.com/TestServiceA"
  }
}
```

## TestServiceA

```
"TestServiceA": {
  "Authentication": {
    "AzureAd": {
      "Instance": "e.g. https://login.microsoftonline.com/",
      "TenantId": "e.g. <your domain>.onmicrosoft.com>",
      "Audience": "https://devunleashed.onmicrosoft.com/TestServiceA",
      "ClientId": "",
      "ClientSecret": ""
    }
  },
  "DownstreamService": {
    "BaseUrl": "https://localhost:44355/",
    "Resource": "https://devunleashed.onmicrosoft.com/TestServiceB"
  }
}
```

## TestServiceB

```
"TestServiceB": {
  "Authentication": {
    "AzureAd": {
      "Instance": "e.g. https://login.microsoftonline.com/",
      "TenantId": "e.g. <your domain>.onmicrosoft.com>",
      "Audience": "https://devunleashed.onmicrosoft.com/TestServiceB"
    }
  }
}
```