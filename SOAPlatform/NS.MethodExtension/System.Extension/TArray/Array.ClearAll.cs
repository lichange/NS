// Copyright (c) 2015 ZZZ Projects. All rights reserved
// Licensed under MIT License (MIT) (https://github.com/zzzprojects/Z.ExtensionMethods)
// Website: http://www.zzzprojects.com/
// Feedback / Feature Requests / Issues : http://zzzprojects.uservoice.com/forums/283927
// All ZZZ Projects products: Entity Framework Extensions / Bulk Operations / Extension Methods /Icon Library

using System;

public static partial class Extensions
{
    /// <summary>
    ///     A T[] extension method that clears all described by @this.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// ###
    /// <returns>.</returns>
    public static void ClearAll<T>(this T[] @this)
    {
        Array.Clear(@this, 0, @this.Length);
    }
}