// *****************************************************************************
// GitHubJwtHeader.cs
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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GitHubAuth.Jwt;

/// <summary>
/// Represents the header of a JSON Web Token to be used by GitHub Apps
/// </summary>
public sealed class GitHubJwtHeader
{

    /// <summary>
    /// The Algorithm to be used for the signature
    /// </summary>
    [JsonPropertyName("alg")]
    [JsonPropertyOrder(2)]
    public string Algorithm { get; }

    /// <summary>
    /// The type of the token
    /// </summary>
    [JsonPropertyName("typ")]
    [JsonPropertyOrder(1)]
    public string Type { get; } = "JWT";

    /// <summary>
    /// Initializes a new instance of the <see cref="GitHubJwtHeader"/> class
    /// </summary>
    internal GitHubJwtHeader(string algorithm)
    {
        Algorithm = algorithm;
    }

    /// <summary>
    /// Serializes the object into a JSON format
    /// </summary>
    /// <returns>A string containing the serialized object</returns>
    public string ToJSON()
    {
        return JsonSerializer.Serialize(this);
    }
}

