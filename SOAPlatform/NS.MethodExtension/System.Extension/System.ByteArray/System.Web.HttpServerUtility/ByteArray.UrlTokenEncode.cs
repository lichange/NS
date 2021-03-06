// Copyright (c) 2015 ZZZ Projects. All rights reserved
// Licensed under MIT License (MIT) (https://github.com/zzzprojects/Z.ExtensionMethods)
// Website: http://www.zzzprojects.com/
// Feedback / Feature Requests / Issues : http://zzzprojects.uservoice.com/forums/283927
// All ZZZ Projects products: Entity Framework Extensions / Bulk Operations / Extension Methods /Icon Library

using System;

public static partial class Extensions
{
    /// <summary>
    ///     Encodes a byte array into its equivalent string representation using base 64 digits, which is usable for
    ///     transmission on the URL.
    /// </summary>
    /// <param name="input">The byte array to encode.</param>
    /// <returns>
    ///     The string containing the encoded token if the byte array length is greater than one; otherwise, an empty
    ///     string (&quot;&quot;).
    /// </returns>
    public static String UrlTokenEncode(this Byte[] input)
    {
        throw new Exception(".net standrad 2.0 已经与Server解耦，如需功能请从实际的具体Server中寻找替换方案");
        //return HttpServerUtility.UrlTokenEncode(input);
    }
}