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
using GitHubAuth.Extensions;

namespace GitHubAuth.Jwt;

/// <summary>
/// Represents the Payload for a JSON Web Token
/// </summary>
public sealed class GitHubJwtPayload
{
    /// <summary>
    /// The maximum number of minutes to add to the token expiration date
    /// </summary>
    /// <remarks>The maximum time accepted by a GitHub Token is 10 minutes. Take into consideration the fact that the request take a few seconds to process.</remarks>
    public static readonly int MAX_TOKEN_MINUTES = 10;

    /// <summary>
    /// The number of seconds allowed by GitHub to be considered as "clock drift".
    /// </summary>
    public static readonly int CLOCK_DRIFT_SECONDS = 60;

    /// <summary>
    /// The time that the JWT was created. To protect against clock drift, we recommend that you set this 60 seconds in the past and ensure that your server's date and time is set accurately (for example, by using the Network Time Protocol).
    /// </summary>
    /// <remarks>When changing this property, it automatically updates <see cref="ExpiresAt"/>, by adding 8 minutes to the date and time set for <see cref="IssuedAt"/></remarks>
	[JsonPropertyName("iat")]
    [JsonConverter(typeof(TimeSinceEpochConverter))]
    public DateTime IssuedAt
    {
        get { return _issuedAt; }
        set
        {
            _issuedAt = value;
            OnPropertyChanged?.Invoke(nameof(IssuedAt));

            ExpiresAt = _issuedAt.AddMinutes(MAX_TOKEN_MINUTES);
        }
    }
    private DateTime _issuedAt = DateTimeExtension.Epoch;

    /// <summary>
    /// The expiration time of the JWT, after which it can't be used to request an installation token. The time must be no more than 10 minutes into the future.
    /// </summary>
    /// <remarks>This property is populated automatically when changing the <see cref="IssuedAt"/> property</remarks>
    [JsonPropertyName("exp")]
    [JsonConverter(typeof(TimeSinceEpochConverter))]
    public DateTime ExpiresAt
    {
        get { return _expiresAt; }
        private set
        {
            _expiresAt = value;
            OnPropertyChanged?.Invoke(nameof(ExpiresAt));
            RenewalDateTime = MAX_TOKEN_MINUTES > 2 ? _expiresAt.AddMinutes(-2) : _expiresAt;
        }
    }
    private DateTime _expiresAt = DateTimeExtension.Epoch.AddMinutes(MAX_TOKEN_MINUTES);

    /// <summary>
    /// The date/time when the token should be renewed. Usually it's two minutes prior to expiration.
    /// </summary>
    /// <remarks>This property is populated automatically when changing the <see cref="IssuedAt"/> property</remarks>
    [JsonIgnore]
    public DateTime RenewalDateTime
    {
        get { return _renewalDateTime; }
        private set
        {
            _renewalDateTime = value;
            OnPropertyChanged?.Invoke(nameof(RenewalDateTime));
        }
    }
    private DateTime _renewalDateTime = DateTime.MinValue;

    /// <summary>
    /// The ID of your GitHub App. This value is used to find the right public key to verify the signature of the JWT. You can find your app's ID with the GET /app REST API endpoint.
    /// </summary>
    [JsonPropertyName("iss")]
    public long Issuer
    {
        get { return _issuer; }
        set
        {
            _issuer = value;
            OnPropertyChanged?.Invoke(nameof(Issuer));
        }
    }
    private long _issuer;

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

    /// <summary>
    /// An action triggered when there is a property change
    /// </summary>
    [JsonIgnore]
    public Action<string>? OnPropertyChanged { get; set; }
}

