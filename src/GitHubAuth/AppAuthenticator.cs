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
using System.Threading.Tasks;
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
    /// A list containing all the installations the GitHub App has access to
    /// </summary>
    public List<long> Installations { get; init; }

    /// <summary>
    /// An internal object to be used when locking the Installation Token Dictionary
    /// </summary>
    private readonly object _installationTokenLock = new();

    /// <summary>
    /// A dictionary containing the Installation Tokens
    /// </summary>
    public Dictionary<long, AccessToken> InstallationTokens { get; init; }

    /// <summary>
    /// The JSON Web Token to be used for the GitHub REST API
    /// </summary>
    private IGitHubJwt Jwt { get; init; }

    /// <summary>
    /// A function to retrieve the Api Client
    /// </summary>
    /// <remarks>To prevent issues with DNS changes, or Socket Exceptions, use this function to retrieve the client.</remarks>
    public Func<HttpClient>? GetClient { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppAuthenticator"/> class
    /// </summary>
    public AppAuthenticator(IGitHubJwt jwt)
    {
        Installations = new();
        InstallationTokens = new();
        Jwt = jwt;
    }

    /// <inheritdoc/>
    public async void Authenticate()
    {
        await AuthenticateAsApp(); // Calling Autheticate(null) would be the same and make the code, perhaps, more concise. However, for performance reasons, it's unecessary to call the same code twice.
    }

    /// <inheritdoc/>
    public async void Authenticate(params object[] args)
    {
        if (args is null)
        {
            await AuthenticateAsApp();
            return;
        }

        if (args.Length == 1 && args[0] is long appInstallationId)
        {
            AuthenticateAsAppInstallation(appInstallationId);
            return;
        }

        // Input is invalid
        throw new ArgumentException("Arguments for authentication are invalid. Expect a null array or an array with only one object of type long");
    }

    /// <summary>
    /// Authenticate to GitHub as an App
    /// </summary>
    /// <remarks>The authentication as an App will call the API and retrieve all valid Installation ID's for the App</remarks>
    /// <exception cref="NullReferenceException">Thrown when there is no implementation for the <see cref="GetClient"/> property</exception>
    public async Task AuthenticateAsApp()
    {
        // Ensure the GetClient property is implemented
        if (GetClient is null) throw new NullReferenceException($"\"{nameof(GetClient)}\" is not implemented");

        // Retrieve the client
        var client = GetClient();

        // The authentication as an app needs a JWT
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Jwt.Token);

        // Attempt to retrieve the installations
        var response = await client.GetAsync("app/installations");
        response.EnsureSuccessStatusCode();

        await AuthenticateAsApp_ProcessAPIResponseContent(response.Content);
    }

    internal async Task AuthenticateAsApp_ProcessAPIResponseContent(HttpContent content)
    {
        var result = await content.ReadFromJsonAsync<List<AppInstallation>>() ?? throw new InvalidOperationException("Unable to parse results from GET request");

        lock (_installationsLock)
        {
            Installations.Clear();

            foreach (var appInstallation in result)
            {
                Installations.Add(appInstallation.ID);
            }
        }

    }

    private void AuthenticateAsAppInstallation(long appInstallationId)
    {
        // Ensure the GetClient property is implemented
        if (GetClient is null) throw new NullReferenceException($"\"{nameof(GetClient)}\" is not implemented");

        // The authentication as an app installation needs a token
        lock (_installationTokenLock)
        {
            InstallationTokens.TryGetValue(appInstallationId, out var accessToken);

            if ((accessToken is not null && accessToken.ExpiresAt < DateTime.UtcNow.AddMinutes(2)) ||
                (accessToken is null))
            {
                // Token needs to be generated

                // A token is generated by making a POST request to the https://api.github.com/app/installations/$$installation_id$$/access_tokens. The JWT must be passed in the header.
                //ApiClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Jwt.Token);

                //ApiClient.PostAsync("A", new )

            }
        }



    }
}
