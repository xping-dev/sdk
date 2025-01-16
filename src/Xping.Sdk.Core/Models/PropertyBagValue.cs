/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using System.Runtime.Serialization;

namespace Xping.Sdk.Core.Models;

/// <summary>
/// Represents a serializable property bag value.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
[Serializable]
public sealed class PropertyBagValue<TValue> : ISerializable
{
    /// <summary>
    /// Gets or sets the value of the serializable property bag value.
    /// </summary>
    public TValue? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyBagValue{TValue}"/> class with the specified value.
    /// </summary>
    /// <param name="value">The value of the serializable property bag value.</param>
    public PropertyBagValue(TValue value)
    {
        Value = value;
    }

    /// <summary>
    /// Parameterless constructor for serialization.
    /// </summary>
    public PropertyBagValue()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyBagValue{TValue}"/> class with serialized data.
    /// </summary>
    /// <param name="info">
    /// The <see cref="SerializationInfo"/> that holds the serialized object data about the 
    /// exception being thrown.
    /// </param>
    /// <param name="context">
    /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
    /// </param>
    public PropertyBagValue(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));

        Value = (TValue?)info.GetValue(nameof(Value), typeof(TValue));
    }

    /// <summary>
    /// Populates a <see cref="SerializationInfo"/> with the data needed to serialize the target object.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(Value), Value);
    }
}
