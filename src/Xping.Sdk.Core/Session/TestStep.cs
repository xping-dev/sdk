/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using System.Runtime.Serialization;
using Xping.Sdk.Core.Models;
using Xping.Sdk.Shared;

namespace Xping.Sdk.Core.Session;

/// <summary>
/// This record represents a step in a test execution. It provides a set of properties that can be used to store 
/// information about the step, such as its name, start date, duration, result, and error message.
/// </summary>
/// <remarks>
/// This record can be serialized and its state can be saved using serializers that support the ISerializable interface.
/// </remarks>
[Serializable]
public sealed record TestStep : ISerializable
{
    private readonly string _name = null!;

    /// <summary>
    /// Gets the name of the test step.
    /// </summary>
    public required string Name
    {
        get => _name;
        init => _name = value.RequireNotNullOrWhiteSpace(nameof(Name));
    }

    /// <summary>
    /// Gets the iteration count of the test component that created this test step.
    /// </summary>
    /// <value>
    /// The number of test steps previously created by the same test component during the test execution, indicating 
    /// the order of this test step in the sequence of iterations.
    /// </value>
    public required int TestComponentIteration { get; init; }

    /// <summary>
    /// Gets the start date of the test step.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> object that represents the start time of the test step.
    /// </value>
    public required DateTime StartDate { get; init; }

    /// <summary>
    /// Gets or sets the duration of the test step.
    /// </summary>
    /// <value>
    /// A <see cref="TimeSpan"/> object that represents the duration of the test step.
    /// </value>
    public required TimeSpan Duration { get; init; }

    /// <summary>
    /// Gets or sets the type of the test step.
    /// </summary>
    /// <value>
    /// A <see cref="TestStepType"/> enumeration value that indicates the type of the test step.
    /// </value>
    public required TestStepType Type { get; init; }

    /// <summary>
    /// Gets or sets the result of the test step.
    /// </summary>
    /// <value>
    /// A <see cref="TestStepResult"/> enumeration value that indicates the result of the test step.
    /// </value>
    public required TestStepResult Result { get; init; }

    /// <summary>
    /// Gets or sets the property bag of this test step. This property bag should contain data related to this specific 
    /// test step execution. On serialization, this data is also serialized, so this property bag should rather include 
    /// built-in types rather than complex objects which won't be (de)serialized.
    /// </summary>
    public PropertyBag? PropertyBag { get; init; }

    /// <summary>
    /// Gets or sets the error message of the test step.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Parameterless constructor for serialization.
    /// </summary>
    public TestStep()
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="TestStep"/> class with serialized data.
    /// </summary>
    /// <param name="info">
    /// The <see cref="SerializationInfo"/> that holds the serialized object data about the test step.
    /// </param>
    /// <param name="context">
    /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
    /// </param>
    public TestStep(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));

        _name = (string)info.GetValue(nameof(Name), typeof(string)).RequireNotNull(nameof(Name));
        StartDate = (DateTime)info.GetValue(nameof(StartDate), typeof(DateTime)).RequireNotNull(nameof(StartDate));
        TestComponentIteration = info.GetInt32(nameof(TestComponentIteration));
        Duration = (TimeSpan)info.GetValue(nameof(Duration), typeof(TimeSpan)).RequireNotNull(nameof(Duration));
        Type = Enum.Parse<TestStepType>((string)info
            .GetValue(nameof(Type), typeof(string))
            .RequireNotNull(nameof(Type)));
        Result = Enum.Parse<TestStepResult>((string)info
            .GetValue(nameof(Result), typeof(string))
            .RequireNotNull(nameof(Result)));
        PropertyBag = (PropertyBag?)info.GetValue(
                name: nameof(PropertyBag),
                type: typeof(PropertyBag));
        ErrorMessage = info.GetValue(nameof(ErrorMessage), typeof(string)) as string;
    }

    /// <summary>
    /// Returns a string that represents the current TestStep object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        string msg = $"{StartDate} " +
            $"({Duration.GetFormattedTime()}) " +
            $"[{Type}]: " +
            $"{Name} " +
            $"{Result.GetDisplayName()}.";

        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            msg += $" {ErrorMessage}.";
        }

        return msg;
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(Name), Name, typeof(string));
        info.AddValue(nameof(StartDate), StartDate, typeof(DateTime));
        info.AddValue(nameof(TestComponentIteration), TestComponentIteration, typeof(int));
        info.AddValue(nameof(Duration), Duration, typeof(TimeSpan));
        info.AddValue(nameof(Type), Type.ToString(), typeof(string));
        info.AddValue(nameof(Result), Result.ToString(), typeof(string));
        info.AddValue(nameof(PropertyBag), PropertyBag, typeof(PropertyBag));
        info.AddValue(nameof(ErrorMessage), ErrorMessage, typeof(string));
    }
}
