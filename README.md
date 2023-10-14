# GitHub Authentication

The GitHub Authentication is a library that provides the [fastest way](#benchmarks) to generate a JWT (JSON Web Token) to be used when calling the GitHub REST API.

This library has no dependencies on any external libraries. It is compatible with `net6.0` and beyond.

## Usage

The JWT used for authenticate with the GitHub REST API needs information, which is encrypted using the `SHA256` hash algorithm, and signed using `RS256`.

The simplest way to generate a token is:

```cs
var jwt = new GitHubJwt("path/to/pem_file", "123456");
var token = jwt.Token;
```

Everytime you call the `Token` property, it will evaluate if the token needs renewal. If it does, then it will automatically renew it.

### Header

The header should have the algorithm and the type of token.

```json
{
  "typ": "JWT",
  "alg": "RS256"
}
``

### Payload

The payload should contain the following claims:

```json
{
  "iat": 1651363200,
  "exp": 1651363800,
  "iss": "123456"
}
```

| Claim | Description |
| ----- | ----------- |
| `iat` | The time when the token was issued at |
| `exp` | The time when the token expires. Usually a token for a GitHub REST API cannot last more than 10 minutes. |
| `iss` | The ID of the GitHub Application to authenticate |

> The time is calculated using the number of seconds since January 1st 1970 (also known as *Epoch*).

## Benchmarks

The method to generate the JWT in this library is very simplified and optimized. Comparing it with another Nuget package yielded the following results:

| Method              | Mean     | Error     | StdDev    | Rank | Gen0    | Gen1   | Allocated |
|-------------------- |---------:|----------:|----------:|-----:|--------:|-------:|----------:|
| TokenWithGitHubAuth | 1.921 ms | 0.0081 ms | 0.0075 ms |    1 |  3.9063 |      - |  29.83 KB |
| TokenWithGitHubJwt  | 2.514 ms | 0.0164 ms | 0.0146 ms |    2 | 70.3125 | 3.9063 | 329.65 KB |
