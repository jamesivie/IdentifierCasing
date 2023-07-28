using System;
using System.Collections.Generic;
using System.Linq;

namespace IdentifierCasing.Utility;

/// <summary>
/// A static class that holds extensions to the system <see cref="Enum"/> class.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Enumerates all the values of an enum.
    /// </summary>
    /// <typeparam name="T">The enum to enumerate values for.</typeparam>
    /// <returns>An enumeration of all possible enum values.</returns>
    public static IEnumerable<T> EnumUniqueValues<T>() where T : struct
    {
        if (!typeof(T).IsEnum)
        {
            throw new InvalidOperationException();
        }
        Array values = Enum.GetValues(typeof(T));
        for (int offset = 0; offset < values.Length; ++offset)
        {
            T x = (T)values.GetValue(offset)!;  // Enum.GetValues better not return an array with nulls init!
            if (offset < 1 || !Enum.Equals(values.GetValue(offset - 1), x)) yield return x;
        }
    }
}
