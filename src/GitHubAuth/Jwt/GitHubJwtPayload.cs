// *****************************************************************************
// GitHubJwtPayload.cs
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
using System.Text.Json.Serialization;

namespace GitHubAuth.Jwt;

/// <summary>
/// Represents the Payload for a JSON Web Token
/// </summary>
public sealed class GitHubJwtPayload
{
    /// <summary>
    /// The time that the JWT was created. To protect against clock drift, we recommend that you set this 60 seconds in the past and ensure that your server's date and time is set accurately (for example, by using the Network Time Protocol).
    /// </summary>
	[JsonPropertyName("iat")]
	public long IssuedAt { get; set; }
    /// <summary>
    /// The expiration time of the JWT, after which it can't be used to request an installation token. The time must be no more than 10 minutes into the future.
    /// </summary>
    [JsonPropertyName("exp")]
    public long ExpiresAt { get; set; }
    /// <summary>
    /// The ID of your GitHub App. This value is used to find the right public key to verify the signature of the JWT. You can find your app's ID with the GET /app REST API endpoint.
    /// </summary>
    [JsonPropertyName("iss")]
    public string? Issuer { get; set; }
    /// <summary>
    /// This should be RS256 since your JWT must be signed using the RS256 algorithm.
    /// </summary>
    [JsonPropertyName("alg")]
    public string Algorithm { get; set; } = GitHubJwt.ALGORITHM;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitHubJwtPayload"/> class
    /// </summary>
	internal GitHubJwtPayload()
	{
	}

    /// <summary>
    /// Serializes the object into a JSON format
    /// </summary>
    /// <returns>A string containing the serialized object</returns>
    public string ToJSON()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}

