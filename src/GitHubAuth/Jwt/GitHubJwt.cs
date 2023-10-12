// *****************************************************************************
// GitHubJwt.cs
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

#pragma warning disable IDE0074 // Use compound assignment

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GitHubAuth.Jwt;

/// <summary>
/// Represents the JSON Web Token used by GitHub to authenticate
/// </summary>
/// <remarks>
/// A token is composed of:
/// <code>
/// var encodedHeader = Base64UrlEncode(header) + "." + Base64UrlEncode(payload)
/// SHA256(encodedHeader)
/// Sign(encodedHeader, privateKey)
/// </code>
/// </remarks>
public sealed class GitHubJwt
{
	/// <summary>
	/// The default algorithm used to generate a GitHub JWT
	/// </summary>
	public const string ALGORITHM = "RS256";

	/// <summary>
	/// The header of the JWT
	/// </summary>
	public GitHubJwtHeader Header { get; set; } = new();
	/// <summary>
	/// The payload of the JWT
	/// </summary>
	public GitHubJwtPayload Payload { get; set; } = new();

	/// <summary>
	/// Internal variable to store the token
	/// </summary>
	private string? _token;

	/// <summary>
	/// The JWT Token
	/// </summary>
	/// <remarks>Returns null if the system is unable to generate the token. Automatically renews the token when it's near expiration.</remarks>
	public string? Token
	{
		get
		{
			// Automatically renew the token if it is expired or null
			if (_token is null || DateTime.UtcNow > Payload.RenewalDateTime)
			{
                Payload.IssuedAt = DateTime.UtcNow.AddSeconds(-GitHubJwtPayload.CLOCK_DRIFT_SECONDS);
				try
				{
                    _token = GenerateToken();
                }
				catch
				{
					_token = null;
				}
            }

			return _token;
		}
	}

	/// <summary>
	/// The path to the private key PEM file
	/// </summary>
	private readonly string PrivateKeyFileName;

	/// <summary>
	/// The contents of the private kew
	/// </summary>
	public string? PrivateKey
	{
		get
		{
			return _privateKey;
		}
		internal set
		{
			_privateKey = value;
			_privateKeyBytes = _privateKey is null ? null : Convert.FromBase64String(_privateKey);
		}
	}
    private string? _privateKey;
	private byte[]? _privateKeyBytes;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="GitHubJwt"/> class
	/// </summary>
	/// <param name="privateKeyFileName">The path to the private key PEM file</param>
	/// <param name="appId">The ID of the GitHub App</param>
	/// <exception cref="NullReferenceException">Thrown when the system could not read the file</exception>
	public GitHubJwt(string privateKeyFileName, string appId)
	{
		PrivateKeyFileName = privateKeyFileName;
        Payload.Issuer = appId;
        Payload.OnPropertyChanged = (s) => _token = null;
	}

	/// <summary>
	/// Reads the private key
	/// </summary>
	/// <remarks>This function expects the file to only contains the private key. PEM files can contain more information, but this function will assume there is only a private key on it</remarks>
	/// <returns>The contents of the private key file or null if the PEM file does not have a private key</returns>
	/// <exception cref="System.IO.FileNotFoundException">Thrown when the private key file could not be found</exception>
	internal string? ReadPrivateKey()
	{
        if (!System.IO.File.Exists(PrivateKeyFileName))
            throw new System.IO.FileNotFoundException("Unable to locate private keyfile", PrivateKeyFileName);

		// Initializes the string builder to store the private key contents (1625 = 65 characters * 25 lines)
		var sbPrivateKey = new StringBuilder(1625);

		// Initializes a string builder to check the comments
		var sbComments = new StringBuilder(65);

		// Control whether the contents is inside a comment (preceeded by 5 '-')
		var isComment = false;

		// The number of dashes (every 5 dashes flips the sequence)
		byte countOfDashes = 0;

        // The file starts with -----BEGIN RSA PRIVATE KEY-----
        // The file ends with -----END RSA PRIVATE KEY-----

        using var sr = new StreamReader(new FileStream(PrivateKeyFileName, FileMode.Open, FileAccess.Read));

		int c;
        while ((c = sr.Read()) >= 0)
        {
            switch (c)
            {
                case '-': // Dashes control the comment flags

					// Dashes are part of the comment section
                    countOfDashes++;

                    if (countOfDashes == 5)
                    {
                        // Flips tag open
                        isComment = !isComment;
						
						if (isComment)
						{
							// When starting a comment, clear the builder
                            sbComments.Clear();
						}
						else if (!isComment && !sbComments.ToString().EndsWith("PRIVATE KEY"))
						{
							// When closing a comment, if it does not end with PRIVATE KEY, clear the private key builder
							sbPrivateKey.Clear();
						}

                        // Reset Count of dashes
                        countOfDashes = 0;
                    }

                    break;
				
				case ' ':	// Blanks or white spaces are ignored, unless the tag is open
				case '\n':  // Line breaks
				case '\t':	// Tabs

					if (isComment) sbComments.Append((char)c);
					break;

                default: // The contents of the private key file or the comments

					if (isComment)
					{
                        // Append comments
                        sbComments.Append((char)c);
					}
					else
					{
						// Append to the Private Key
                        sbPrivateKey.Append((char)c);
					}

                    break;

            }
        }

		// Return the private key contents
		return sbComments.ToString().EndsWith("PRIVATE KEY") ? sbPrivateKey.ToString() : null;
    }

	/// <summary>
	/// Generates a token based on the <see cref="Header"/> and <see cref="Payload"/>
	/// </summary>
	/// <returns>A string containing the JWT</returns>
    internal string GenerateToken()
	{
        if (PrivateKey is null)
			PrivateKey = ReadPrivateKey() ?? throw new NullReferenceException("Unable to read Private Key from file");

        var sbToken = new StringBuilder(500); // Token usually have less than that

		sbToken.Append(Base64UrlEncode(Header.ToJSON()));
		sbToken.Append('.');
		sbToken.Append(Base64UrlEncode(Payload.ToJSON()));

		var bytesToEncrypt = Encoding.UTF8.GetBytes(sbToken.ToString());

        using var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(_privateKeyBytes, out _);

        var encryptedData = rsa.SignData(bytesToEncrypt, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
		
		sbToken.Append('.');
        sbToken.Append(Convert.ToBase64String(encryptedData).TrimEnd('=').Replace('+', '-').Replace('/', '_'));

		return sbToken.ToString();
	}

    /// <summary>
    /// Encodes the text into Base64
    /// </summary>
    /// <param name="text">The text to be encoded</param>
    /// <returns>The text encoded</returns>
    internal static string Base64UrlEncode(string text)
    {
        var textInBytes = System.Text.Encoding.ASCII.GetBytes(text);
		return System.Convert.ToBase64String(textInBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }


	internal static string Base64UrlDecode(string text)
	{
		var incoming = text.Replace('_', '/').Replace('-', '+');
		switch (incoming.Length % 4)
		{
			case 2: incoming += "=="; break;
			case 3: incoming += "="; break;
		}

		var bytes = Convert.FromBase64String(incoming);
		return System.Text.Encoding.ASCII.GetString(bytes);
	}
}

#pragma warning restore IDE0074 // Use compound assignment
