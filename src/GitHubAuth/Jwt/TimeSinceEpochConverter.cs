// *****************************************************************************
// TimeSinceEpochConverter.cs
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
using GitHubAuth.Extensions;

namespace GitHubAuth.Jwt;

/// <summary>
/// Converts a DateTime into a time in seconds since epoch
/// </summary>
public sealed class TimeSinceEpochConverter : JsonConverter<DateTime>
{
    /// <inheritdoc/>
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var contents = reader.GetInt64();
        var result = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(contents);

        return result;
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ToSecondsSinceEpoch());
    }
}

