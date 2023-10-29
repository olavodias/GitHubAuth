# GitHub Authentication .NET

[![nuget](https://img.shields.io/nuget/v/GitHubAuth.svg)](https://www.nuget.org/packages/GitHubAuth/) 
![GitHub release](https://img.shields.io/github/release/olavodias/GitHubAuth.svg)
![NuGet](https://img.shields.io/nuget/dt/GitHubAuth.svg)
![license](https://img.shields.io/github/license/olavodias/GitHubAuth.svg)

The GitHub Authentication is a library that provides classes to facilitate the authentication process to the GitHub REST API.

It contains the [fastest way](#benchmarks) to generate a JWT (JSON Web Token), which is a pre-requisite when authenticating to the GitHub REST API as an Application or an Application Installation.

This library has no dependencies. It is compatible with `net6.0` and beyond.

Refer to the [API Documentation](https://olavodias.github.io/GitHubApps) to have a better understanding of each class and how to use it.

## Generating a JWT (JSON Web Token)

In order to generate a JWT, the first thing necessary is a private key. You can generate the private key on GitHub. Check [this document](https://docs.github.com/en/apps/creating-github-apps/authenticating-with-a-github-app/managing-private-keys-for-github-apps#generating-private-keys) for more information.

The token will be in a file with the `PEM` extension. Store it in a location that your software can access.

> For security reasons, **never store the PEM file in the repository**, even if it is a private repository. Also, never store it in a configuration file.

The simplest way to generate a token is:

```cs
// Assuming the application ID is 123456 (you can obtain your application ID in GitHub)
var jwt = new GitHubJwtWithRS256("path/to/pem_file.pem", "123456");
var token = jwt.Token;
```

The `Token` property returns the token to be used in the Request Header. Everytime you call the `Token` property, it will evaluate if the token needs renewal. If it does, then it will automatically renew it.

The JWT used for authenticate with the GitHub REST API needs a header and a payload, which is encrypted using the `SHA256` hash algorithm, and signed using `RS256`.

The token is comprised of a header, a payload, and the signature. Both the header and payload are encoded with `Base64Url`.

The formula would then be `Base64Url(header).Base64Url(payload).signature`.

### Header

The header should have the algorithm and the type of token.

```json
{
  "typ": "JWT",
  "alg": "RS256"
}
```

The header is represented by the `GitHubJwtHeader` class.

### Payload

The payload should contain the following claims:

```json
{
  "iat": 1651363200,
  "exp": 1651363800,
  "iss": "123456"
}
```

| Claim | Description | Property Name |
| ----- | ----------- | ------------- |
| `iat` | The time when the token was issued at | `GitHubJwtPayload.IssuedAt` |
| `exp` | The time when the token expires. Usually a token for a GitHub REST API cannot last more than 10 minutes. | `GitHubJwtPayload.ExpiresAt` |
| `iss` | The ID of the GitHub Application to authenticate | `GitHubJwtPayload.Issuer` |

> The time is calculated using the number of seconds since January 1st 1970 (also known as *Epoch*).

The payload is represented by the `GitHubJwtPayload` class. The fields `IssuedAt` and `ExpiresAt` are stored as `DateTime` fields, however, when the JSON is generated, the system automatically converts the `DateTime` value into the proper numeric value, as expected by the API.

## Authenticating with the AppAuthenticator

This library contains the class `AppAuthenticator` to facilitate the authentication process. It exposes methods to perform the authentication process and retrieve the necessary authentication data..

| Method | Purpose |
| ------ | ------- |
| `GetToken()` | This method will return the JWT to be used to authenticate as a GitHub App |
| `GetToken<T>(T input)` | This method will authenticate as a GitHub App Installation, based on the given input, and return the token |

Users can retrieve the token any time by calling the `GetToken<T>(T input)` method. It automatically renews the token if necessary.

```cs
// Assuming the application ID is 123456 (you can obtain your application ID in GitHub)
var jwt = new GitHubJwtWithRS256("path/to/pem_file.pem", "123456");
var authenticator = new AppAuthenticator(jwt);

// Assuming the App Installation ID is 350000
var appInstallationId = 350000;
var authData = authenticator.GetToken(appInstallationId);

Console.WriteLine(authData.Token);  // this will print the token
Console.WriteLine(authData.Mode);   // this will print wether to use 'Token' or 'Bearer'
```

## Benchmarks

The method to generate the JWT in this library is very simplified and optimized. Comparing it with another Nuget package yielded the following results:

| Method              | Mean     | Error     | StdDev    | Rank | Gen0    | Gen1   | Allocated |
|-------------------- |---------:|----------:|----------:|-----:|--------:|-------:|----------:|
| TokenWithGitHubAuth | 1.921 ms | 0.0081 ms | 0.0075 ms |    1 |  3.9063 |      - |  29.83 KB |
| TokenWithGitHubJwt  | 2.514 ms | 0.0164 ms | 0.0146 ms |    2 | 70.3125 | 3.9063 | 329.65 KB |

> The comparison was made using the `BenchmarkDotNet` library