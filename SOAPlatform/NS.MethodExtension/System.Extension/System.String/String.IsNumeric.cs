// Copyright (c) 2015 ZZZ Projects. All rights reserved
// Licensed under MIT License (MIT) (https://github.com/zzzprojects/Z.ExtensionMethods)
// Website: http://www.zzzprojects.com/
// Feedback / Feature Requests / Issues : http://zzzprojects.uservoice.com/forums/283927
// All ZZZ Projects products: Entity Framework Extensions / Bulk Operations / Extension Methods /Icon Library

using System.Text.RegularExpressions;

public static partial class Extensions
{
    /// <summary>
    ///     A string extension method that query if '@this' is numeric.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>true if numeric, false if not.</returns>
    public static bool IsNumeric(this string @this)
    {
        return !Regex.IsMatch(@this, "[^0-9]");
    }
}