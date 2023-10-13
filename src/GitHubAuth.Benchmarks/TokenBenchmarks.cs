// *****************************************************************************
// TokenBenchmarks.cs
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

#pragma warning disable CA1822 // Mark members as static

using System;
using BenchmarkDotNet.Attributes;

namespace GitHubAuth.Benchmarks;

[MemoryDiagnoser]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class TokenBenchmarks
{

    public const string PRIVATE_KEY_FILE = "sample_key.pem";


    [Benchmark]
    public void TokenWithGitHubAuth()
    {
        var jwt = new GitHubAuth.Jwt.GitHubJwt(PRIVATE_KEY_FILE, "123456");
        _ = jwt.Token;
    }


    [Benchmark]
    public void TokenWithGitHubJwt()
    {
        // Code extracted from:
        // https://octokitnet.readthedocs.io/en/latest/github-apps/#additional-notes

        var generator = new GitHubJwt.GitHubJwtFactory(
            new GitHubJwt.FilePrivateKeySource(PRIVATE_KEY_FILE),
            new GitHubJwt.GitHubJwtFactoryOptions
            {
                AppIntegrationId = 123456,
                ExpirationSeconds = 600
            });

        _ = generator.CreateEncodedJwtToken();
    }

}

#pragma warning restore CA1822 // Mark members as static
