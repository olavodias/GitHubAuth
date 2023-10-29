// *****************************************************************************
// IAuthenticator.cs
//
// Author:
//       Olavo Henrique Dias <olavodias@gmail.com>
//
// Copyright (c) 2023 Olavo Henrique Dias
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// *****************************************************************************
using System;
using System.Net.Http;

namespace GitHubAuth;

/// <summary>
/// Represents the methods that need to be implemented by a class that performs GitHub Authentication
/// </summary>
public interface IAuthenticator
{
    /// <summary>
    /// Returns the token to be used for authentication
    /// </summary>
    /// <returns>An object containing the components necessary to perform authentication</returns>
    public AuthenticationData GetToken();
    /// <summary>
    /// Returns the token to be used for authentication
    /// </summary>
    /// <param name="input">A parameter to be used when generating the token</param>
    /// <returns>An object containing the components necessary to perform authentication</returns>
    public AuthenticationData GetToken<T>(T input);
}

/// <summary>
/// Represents the response of the Authentication Process
/// </summary>
public struct AuthenticationData
{
    /// <summary>
    /// The type of the token
    /// </summary>
    public AuthenticationTokenType TokenType { get; private set; }
    /// <summary>
    /// The string containing the token
    /// </summary>
    public string Token { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationData"/> struct
    /// </summary>
    /// <param name="tokenType">The type of the token</param>
    /// <param name="token">A string containing the token</param>
    public AuthenticationData(AuthenticationTokenType tokenType, string token)
    {
        TokenType = tokenType;
        Token = token;
    }
}

/// <summary>
/// Represents an Authentication Token Type
/// </summary>
public struct AuthenticationTokenType
{
    /// <summary>
    /// A constant representing the Token mode
    /// </summary>
    public const string MODE_TOKEN = "Token";
    /// <summary>
    /// A constant representing the Bearer mode
    /// </summary>
    public const string MODE_BEARER = "Bearer";

    /// <summary>
    /// The ID of the Authentication Token Type
    /// </summary>
    public int ID { get; private set; }
    /// <summary>
    /// The mode of using the token
    /// </summary>
    public string Mode { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationTokenType"/> struct
    /// </summary>
    /// <param name="id"></param>
    /// <param name="mode"></param>
    public AuthenticationTokenType(int id, string mode)
    {
        ID = id;
        Mode = mode;
    }

    /// <summary>
    /// Represents a GitHub App Token, containing the JWT
    /// </summary>
    public static AuthenticationTokenType AppToken => new(10, MODE_BEARER);
    /// <summary>
    /// Represents a GitHub App Installation Token
    /// </summary>
    public static AuthenticationTokenType AppInstallationToken => new(20, MODE_TOKEN);
    /// <summary>
    /// Represents a GitHub Personal Access Token
    /// </summary>
    public static AuthenticationTokenType PersonalAccessToken => new(30, MODE_TOKEN);

}