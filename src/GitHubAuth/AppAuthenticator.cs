// *****************************************************************************
// AppAuthenticator.cs
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
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using GitHubAuth.Jwt;

namespace GitHubAuth;

/// <summary>
/// A class to authenticate to GitHub as an App or an App Installation
/// </summary>
public sealed class AppAuthenticator: IAuthenticator
{
    /// <summary>
    /// The URL to the GitHub REST API
    /// </summary>
    public const string API_URL = "https://api.github.com";

    private readonly object _installationsLock = new();

    /// <summary>
    /// The internal list containing all the installations the GitHub App has access to
    /// </summary>
    internal List<long> InstallationsInternalList { get; init; } = new();

    /// <summary>
    /// A read-only list of Installations
    /// </summary>
    public IReadOnlyList<long> Installations
    {
        get
        {
            return InstallationsInternalList.AsReadOnly();
        }
    }

    /// <summary>
    /// An internal object to be used when locking the Installation Token Dictionary
    /// </summary>
    private readonly object _installationTokenLock = new();

    /// <summary>
    /// The internal dictionary containing the Installation Tokens
    /// </summary>
    internal Dictionary<long, AccessToken> InstallationTokensInternalDictionary { get; init; } = new();

    /// <summary>
    /// A read-only dictionary of Installation Tokens
    /// </summary>
    public IReadOnlyDictionary<long, AccessToken> InstallationTokens
    {
        get
        {
            return new System.Collections.ObjectModel.ReadOnlyDictionary<long, AccessToken>(InstallationTokensInternalDictionary);
        }
    }

    /// <summary>
    /// The JSON Web Token to be used for the GitHub REST API
    /// </summary>
    public IGitHubJwt Jwt { get; init; }

    /// <summary>
    /// A function to retrieve the Api Client
    /// </summary>
    /// <remarks>To prevent issues with DNS changes, or Socket Exceptions, use this function to retrieve the client.</remarks>
    public Func<HttpClient>? GetClient { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppAuthenticator"/> class
    /// </summary>
    /// <param name="privateKeyFileName">The path to the PEM file</param>
    /// <param name="appId">The GitHub Application ID</param>
    public AppAuthenticator(string privateKeyFileName, long appId)
    {
        Jwt = new GitHubJwtWithRS256(privateKeyFileName, appId);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppAuthenticator"/> class
    /// </summary>
    /// <param name="jwt">The JWT to be used by the Authenticator</param>
    public AppAuthenticator(IGitHubJwt jwt)
    {
        Jwt = jwt;
    }

    /// <summary>
    /// Returns the JWT to be used to authenticate as a GitHub App
    /// </summary>
    /// <returns>An object containing the components necessary to perform authentication</returns>
    public AuthenticationData GetToken()
    {
        if (Jwt.Token is null)
            throw new NullReferenceException("The system could not generate a valid JWT Token");

        return new AuthenticationData(AuthenticationTokenType.AppToken, Jwt.Token);
    }

    /// <inheritdoc/>
    /// <remarks>The input should be the Application Installation ID, which is a <see cref="long"/> field. A value of type <see cref="string"/> will be converted to long.</remarks>
    /// <exception cref="ArgumentException">Thrown when the input value is not acceptable</exception>
    public AuthenticationData GetToken<T>(T input)
    {
        if (input is long longInput)
            return GetTokenForApplicationInstallationID(longInput);

        if (input is int intInput)
            return GetTokenForApplicationInstallationID((long)intInput);

        if (input is byte byteInput)
            return GetTokenForApplicationInstallationID((long)byteInput);

        if (input is string stringInput)
        {
            // Attempt to convert the string to long
            if (!long.TryParse(stringInput, out longInput))
                throw new ArgumentException($"The value \"{stringInput}\" could not be converted to a number");

            return GetTokenForApplicationInstallationID(longInput);
        }

        throw new ArgumentException($"The input \"{input}\" has an invalid value");
    }

    /// <summary>
    /// Returns the token to be used when authentication as an Application Installation
    /// </summary>
    /// <param name="applicationInstallationID">The Application Installation ID</param>
    /// <returns></returns>
    /// <exception cref="MissingMemberException">Thrown when there is no implementation for the <see cref="GetClient"/> property</exception>
    /// <exception cref="ArgumentException">Thrown when the Application Installation ID is not valid for the GitHub App</exception>
    /// <exception cref="NullReferenceException">Thrown when the Token has a null value</exception>
    private AuthenticationData GetTokenForApplicationInstallationID(long applicationInstallationID)
    {
        // Ensure the GetClient property is implemented
        if (GetClient is null) throw new MissingMemberException($"\"{nameof(GetClient)}\" is not implemented", nameof(GetClient));

        // Ensure the installation is valid for the app
        if (!InstallationsInternalList.Contains(applicationInstallationID))
        {
            GetApplicationsIDForApp();
            if (!InstallationsInternalList.Contains(applicationInstallationID))
                throw new ArgumentException($"Installation '{applicationInstallationID}' is not valid for GitHub App '{Jwt.AppId}'", nameof(applicationInstallationID));
        }

        // The authentication as an app installation needs a token
        lock (_installationTokenLock)
        {
            InstallationTokensInternalDictionary.TryGetValue(applicationInstallationID, out var accessToken);

            if ((accessToken is not null && accessToken.ExpiresAt < DateTime.UtcNow.AddMinutes(2)) ||
                (accessToken is null))
            {
                // Token needs to be generated

                // A token is generated by making a POST request to the https://api.github.com/app/installations/$$installation_id$$/access_tokens. The JWT must be passed in the header.
                var client = GetClient();

                // The authentication as an app needs a JWT
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Jwt.Token);
                var response = client.PostAsync($"app/installations/{applicationInstallationID}/access_tokens", null).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                    throw new ArgumentException($"Unable to generate an access token for \"{applicationInstallationID}\"");

                // Process the response into a valid AccessToken
                accessToken = response.Content.ReadFromJsonAsync<AccessToken>().GetAwaiter().GetResult() ?? throw new NullReferenceException("Unable to parse results from POST request");
                InstallationTokensInternalDictionary.Add(applicationInstallationID, accessToken);
            }

            if (accessToken.Token is null)
                throw new NullReferenceException($"The access token for the app installation \"{applicationInstallationID}\" is null");

            return new AuthenticationData(AuthenticationTokenType.AppInstallationToken, accessToken.Token);
        }
    }

    /// <summary>
    /// Get the Application IDs valid for the given GitHub App
    /// </summary>
    /// <remarks>The authentication as an App will call the API and retrieve all valid Installation ID's for the App</remarks>
    /// <exception cref="NullReferenceException">Thrown when the system could not parse the results of the GET request</exception>
    /// <exception cref="MissingMemberException">Thrown when there is no implementation for the <see cref="GetClient"/> property</exception>
    private void GetApplicationsIDForApp()
    {
        // Ensure the GetClient property is implemented
        if (GetClient is null) throw new MissingMemberException($"\"{nameof(GetClient)}\" is not implemented", nameof(GetClient));

        // Retrieve the client
        var client = GetClient();

        // The authentication as an app needs a JWT
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Jwt.Token);

        // Attempt to retrieve the installations
        var response = client.GetAsync("app/installations").GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();

        var listOfAppInstallations = response.Content.ReadFromJsonAsync<List<AppInstallation>>().GetAwaiter().GetResult() ?? throw new NullReferenceException("Unable to parse results from GET request");

        // Recreate the list with the new application installations
        lock (_installationsLock)
        {
            InstallationsInternalList.Clear();

            foreach (var appInstallation in listOfAppInstallations)
            {
                InstallationsInternalList.Add(appInstallation.ID);
            }
        }
    }
}
