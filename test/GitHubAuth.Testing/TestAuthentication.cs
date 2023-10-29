// *****************************************************************************
// TestAuthentication.cs
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
using GitHubAuth.Jwt;

namespace GitHubAuth.Testing;

[TestClass]
public class TestAuthentication
{
    private static readonly long ApplicationID = 123456;
    private static readonly IGitHubJwt Jwt = new GitHubJwtWithRS256("sample_key.pem", ApplicationID);
    private static readonly AppAuthenticator AppAuthenticator = new(Jwt);

    private static readonly HttpClient MockedClient = new(new FakeGitHubMessageHandler())
    {
        BaseAddress = new Uri("http://localhost")
    };

    [TestInitialize]
    public void TestInitialize()
    {
        AppAuthenticator.GetClient = () =>
        {
            return MockedClient;
        };
    }

    [TestMethod]
    public void TestGetTokenForAppInstallation()
    {
        // Authenticates with a valid app installation (40000000)
        long appInstallationId = 40000000;
        var authData = AppAuthenticator.GetToken(appInstallationId);

        Assert.IsTrue(AppAuthenticator.InstallationTokensInternalDictionary.ContainsKey(appInstallationId));
        Assert.IsTrue(AppAuthenticator.InstallationTokens.ContainsKey(appInstallationId));

        Assert.AreEqual("ghs_v5xXNXICdEtmQgW4nIX3RwptMsV0r402BEql", authData.Token);

        // Authenticates with a valid app installation (40000001)
        appInstallationId = 40000001;
        authData = AppAuthenticator.GetToken(appInstallationId);

        Assert.IsTrue(AppAuthenticator.InstallationTokensInternalDictionary.ContainsKey(appInstallationId));
        Assert.IsTrue(AppAuthenticator.InstallationTokens.ContainsKey(appInstallationId));

        Assert.AreEqual("shs_v6xXNXICdEtmQgW4nYX3NwptMsV0x402BEwl", authData.Token);

        // Try to authenticate with an invalid app installation
        Assert.ThrowsException<ArgumentException>(() => AppAuthenticator.GetToken<long>(123456));
    }

}

