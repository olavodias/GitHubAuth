﻿// *****************************************************************************
// DateTimeExtension.cs
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
namespace GitHubAuth.Extensions;

/// <summary>
/// Represents Extensions for the <see cref="DateTime"/> class
/// </summary>
public static class DateTimeExtension
{
    /// <summary>
    /// The number of ticks in January 1st 1970 at 00:00:00
    /// </summary>
    public const long TicksAtEpoch = 621355968000000000;

    /// <summary>
    /// A constant to define the epoch
    /// </summary>
    public static readonly DateTime Epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Convert the date/time into UTC seconds, since epoch
    /// </summary>
    /// <param name="dateTime">The date time to be converted</param>
    /// <returns>A value containing the UTC seconds since epoch (1970-01-01)</returns>
    public static long ToSecondsSinceEpoch(this DateTime dateTime)
    {
        return (dateTime.ToUniversalTime().Ticks - TicksAtEpoch) / TimeSpan.TicksPerSecond;
    }

}

