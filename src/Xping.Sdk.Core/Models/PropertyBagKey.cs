/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

namespace Xping.Sdk.Core.Models;

/// <summary>
/// Represents a key for a property bag.
/// </summary>
[Serializable]
public sealed class PropertyBagKey
{
    /// <summary>
    /// Gets the key string.
    /// </summary>
    public string Key { get; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyBagKey"/> class with the specified key.
    /// </summary>
    /// <param name="key">The key string.</param>
    public PropertyBagKey(string key)
    {
        Key = key;
    }

    /// <summary>
    /// Parameterless constructor for serialization.
    /// </summary>
    public PropertyBagKey() { }
}
