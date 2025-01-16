/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using Xping.Sdk.Core.Models;
using Xping.Sdk.Shared;

namespace Xping.Sdk.Core.Session;

/// <summary>
/// Builder class for creating instances of <see cref="TestStep"/>.
/// </summary>
public class TestStepBuilder
{
    private string _name = string.Empty;
    private int _testComponentIteration;
    private TimeSpan _duration;
    private TestStepType _type;
    private TestStepResult _result;
    private PropertyBag? _propertyBag;
    private string? _errorMessage;

    /// <summary>
    /// Sets the name of the test step.
    /// </summary>
    /// <param name="name">The name of the test step.</param>
    /// <returns>The current instance of <see cref="TestStepBuilder"/>.</returns>
    public TestStepBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the iteration count of the test component.
    /// </summary>
    /// <param name="testComponentIteration">The iteration count of the test component.</param>
    /// <returns>The current instance of <see cref="TestStepBuilder"/>.</returns>
    public TestStepBuilder WithTestComponentIteration(int testComponentIteration)
    {
        _testComponentIteration = testComponentIteration;
        return this;
    }

    /// <summary>
    /// Sets the duration of the test step.
    /// </summary>
    /// <param name="duration">The duration of the test step.</param>
    /// <returns>The current instance of <see cref="TestStepBuilder"/>.</returns>
    public TestStepBuilder WithDuration(TimeSpan duration)
    {
        _duration = duration;
        return this;
    }

    /// <summary>
    /// Sets the type of the test step.
    /// </summary>
    /// <param name="type">The type of the test step.</param>
    /// <returns>The current instance of <see cref="TestStepBuilder"/>.</returns>
    public TestStepBuilder WithType(TestStepType type)
    {
        _type = type;
        return this;
    }

    /// <summary>
    /// Sets the result of the test step.
    /// </summary>
    /// <param name="result">The result of the test step.</param>
    /// <returns>The current instance of <see cref="TestStepBuilder"/>.</returns>
    public TestStepBuilder WithResult(TestStepResult result)
    {
        _result = result;
        return this;
    }

    /// <summary>
    /// Sets the property bag of the test step.
    /// </summary>
    /// <param name="propertyBag">The property bag of the test step.</param>
    /// <returns>The current instance of <see cref="TestStepBuilder"/>.</returns>
    public TestStepBuilder WithPropertyBag(PropertyBag propertyBag)
    {
        _propertyBag = propertyBag;
        return this;
    }

    /// <summary>
    /// Sets the error message of the test step.
    /// </summary>
    /// <param name="errorMessage">The error message of the test step.</param>
    /// <returns>The current instance of <see cref="TestStepBuilder"/>.</returns>
    public TestStepBuilder WithErrorMessage(string? errorMessage)
    {
        _errorMessage = errorMessage;
        return this;
    }

    /// <summary>
    /// Builds and returns a new instance of <see cref="TestStep"/> with the configured properties.
    /// </summary>
    /// <returns>A new instance of <see cref="TestStep"/>.</returns>
    public TestStep Build()
    {
        return new TestStep
        {
            Name = _name.RequireNotNullOrWhiteSpace(nameof(WithName)),
            StartDate = DateTime.UtcNow,
            TestComponentIteration = _testComponentIteration,
            Duration = _duration,
            Type = _type,
            Result = _result,
            PropertyBag = _propertyBag,
            ErrorMessage = _errorMessage
        };
    }
}
