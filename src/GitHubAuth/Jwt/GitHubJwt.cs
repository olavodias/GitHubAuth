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
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GitHubAuth.Jwt;

/// <summary>
/// Represents the JSON Web Token used by GitHub to authenticate
/// </summary>
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
	/// The path to the private key PEM file
	/// </summary>
	private readonly string PrivateKeyFileName;
	/// <summary>
	/// The contents of the private kew
	/// </summary>
	private readonly string PrivateKey;

	/// <summary>
	/// Initializes a new instance of the <see cref="GitHubJwt"/> class
	/// </summary>
	/// <param name="privateKeyFileName">The path to the private key PEM file</param>
	/// <exception cref="NullReferenceException">Thrown when the system could not read the file</exception>
	public GitHubJwt(string privateKeyFileName)
	{
		PrivateKeyFileName = privateKeyFileName;
		PrivateKey = ReadPrivateKey() ?? throw new NullReferenceException("Unable to read Private Key from file");
	}

	/// <summary>
	/// Gets a string containing the JWT. This function will automatically generate a new token if the time expired
	/// </summary>
	/// <returns>A string containing the JWT</returns>
	public string Get()
	{
		//TODO: Check if it is expired or not
		return GenerateNewToken();
	}

	/// <summary>
	/// Reads the private key
	/// </summary>
	/// <remarks>This function expects the file to only contains the private key. PEM files can contain more information, but this function will assume there is only a private key on it</remarks>
	/// <returns>The contents of the private key file or null if the PEM file does not have a private key</returns>
	/// <exception cref="System.IO.FileNotFoundException">Thrown when the private key file could not be found</exception>
	private string? ReadPrivateKey()
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

				case ' ': // Blanks or white spaces are ignored, unless the tag is open

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
	/// Generates a new token
	/// </summary>
	/// <returns>A string containing the JWT</returns>
    private string GenerateNewToken()
	{
		var sb = new StringBuilder(400);

		sb.Append(Base64Encode(Header.ToJSON()));
		sb.Append('.');
		sb.Append(Base64Encode(Payload.ToJSON()));

		var bytes = Encoding.UTF8.GetBytes(sb.ToString());

		return Convert.ToBase64String(EncryptData(bytes));
	}

	/// <summary>
	/// Encondes the text into Base64
	/// </summary>
	/// <param name="text">The text to be encoded</param>
	/// <returns>The text encoded</returns>
	private static string Base64Encode(string text)
	{
		var textInBytes = System.Text.Encoding.UTF8.GetBytes(text);
		return System.Convert.ToBase64String(textInBytes);
	}

	/// <summary>
	/// Encrypt the data using the key defined at <see cref="PrivateKey"/>
	/// </summary>
	/// <param name="bytes">The array of bytes to be encrypted</param>
	/// <returns>An array of bytes containing the encrypted data</returns>
	private byte[] EncryptData(byte[] bytes)
	{
        using var rsa = new RSACryptoServiceProvider();
        rsa.FromXmlString(PrivateKey);
        var encryptedData = rsa.Encrypt(bytes, true);
        return encryptedData;
    }

}

