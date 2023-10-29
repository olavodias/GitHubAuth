// *****************************************************************************
// AccessToken.cs
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
using System.Text.Json.Serialization;

namespace GitHubAuth;

/// <summary>
/// Represents an Access Token containing the token, the expiration date, and permissions
/// </summary>
public sealed class AccessToken
{
	/// <summary>
	/// The token
	/// </summary>
	[JsonPropertyName("token")]
	public string? Token { get; set; }

	/// <summary>
	/// The date and time the token expires
	/// </summary>
	[JsonPropertyName("expires_at")]
	public DateTime? ExpiresAt { get; set; }

	/// <summary>
	/// A dictionary containing the permissions of this token
	/// </summary>
	[JsonPropertyName("permissions")]
	public Dictionary<string, string>? Permissions { get; set; }

	/// <summary>
	/// The repository selection
	/// </summary>
	[JsonPropertyName("repository_selection")]
	public string? RepositorySelection { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="AccessToken"/> class
	/// </summary>
	public AccessToken()
	{
	}
}

