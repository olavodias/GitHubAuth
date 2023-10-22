// *****************************************************************************
// FakeGitHubMessageHandler.cs
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
using System.Text;

namespace GitHubAuth.Testing;

/// <summary>
/// A fake client to mock a real Git Hub API client
/// </summary>
public sealed class FakeGitHubMessageHandler: HttpMessageHandler
{
    private static readonly Task<HttpResponseMessage> ResponseStatusInternalServerError = Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError));
    private static readonly Task<HttpResponseMessage> ResponseStatusNotFound = Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.NotFound));

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Method == HttpMethod.Get)
        {
            if (request.RequestUri is null)
                return ResponseStatusInternalServerError;

            // Check the Segments of the URI and build a file
            // Example: segments is an array
            //   arr[0] = "/"
            //   arr[1] = "app"
            //   arr[2] = "installation"
            //
            // Results in Responses/App_Installation.json
            var fileName = Path.Combine("Responses", FileFromSegments(request.RequestUri.Segments));
            var fileContents = File.ReadAllText(fileName);

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            var content = new StringContent(fileContents, Encoding.UTF8, "application/json");

            response.Content = content;

            return Task.FromResult(response);
        }

        return ResponseStatusNotFound;
    }

    private static string FileFromSegments(string[] segments)
    {
        var sb = new StringBuilder(256);

        foreach (var segment in from s
                                in segments
                                select s.TrimEnd('/')                               )
        {
            if (segment.Length > 0)
            {
                sb.Append(CapitalizeFirstLetter(segment.ToLower()));
                sb.Append('_');
            }
        }

        sb.Remove(sb.Length - 1, 1);

        sb.Append(".json");

        return sb.ToString();
    }


    private static string CapitalizeFirstLetter(string input)
    {
        if (input.Length == 0) return input;
        return char.ToUpper(input[0]) + input[1..];
    }
    
}

