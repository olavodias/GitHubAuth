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
    private static readonly IGitHubJwt Jwt = new GitHubJwtWithRS256("sample_key.pem", 123456);
    private static readonly AppAuthenticator AppAuthenticator = new(Jwt);

    private static readonly HttpClient MockedClient = new(new FakeGitHubMessageHandler())
    {
        BaseAddress = new Uri("http://localhost")
    };

    [TestMethod]
    public void TestAuthenticateAsApp()
    {
        AppAuthenticator.GetClient = () =>
        {
            return MockedClient;
        };

        AppAuthenticator.AuthenticateAsApp().GetAwaiter().GetResult();

        Assert.IsTrue(false);

    }

}

