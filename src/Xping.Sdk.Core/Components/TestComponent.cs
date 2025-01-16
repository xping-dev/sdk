/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using Xping.Sdk.Core.Session;
using Xping.Sdk.Shared;

namespace Xping.Sdk.Core.Components;

/// <summary>
/// This abstract class is designed to execute an action or validate test operation that is defined by the derived 
/// class.
/// </summary>
public abstract class TestComponent
{
    /// <summary>
    /// Initializes new instance of the TestComponent class.
    /// </summary>
    /// <param name="name">Name of the test component.</param>
    /// <param name="type">Type of the test component.</param>
    protected TestComponent(string name, TestStepType type)
    {
        Name = name.RequireNotNullOrEmpty(nameof(name));
        Type = type;
    }

    /// <summary>
    /// Gets component name.
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Gets a test component type.
    /// </summary>
    public TestStepType Type { get; }

    /// <summary>
    /// This method performs the test operation asynchronously. It is an abstract method that must be implemented 
    /// by the subclass.
    /// </summary>
    /// <param name="context">A <see cref="TestContext"/> object that represents the test context.</param>
    /// <param name="cancellationToken">An optional CancellationToken object that can be used to cancel the 
    /// this operation.</param>
    /// <returns>
    /// A <see cref="Task{TestStepResult}"/> that represents the asynchronous operation. The task result 
    /// contains a <see cref="TestStepResult"/> enumeration value that indicates the result of the test component.
    /// </returns>
    public abstract Task<TestStepResult> HandleAsync(
        TestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new instance of the TestComponent class to the current object.
    /// </summary>
    /// <param name="component">The TestComponent instance to add.</param>
    public void AddComponent(TestComponent component) => GetComposite()?.AddComponent(component);

    /// <summary>
    /// Removes the specified instance of the TestComponent class from the current object.
    /// </summary>
    /// <param name="component">The TestComponent instance to remove.</param>
    /// <returns>
    /// <c>true</c> if component is successfully removed; otherwise, <c>false</c>. This method also returns <c>false</c>
    /// when component was not found.
    /// </returns>
    public bool RemoveComponent(TestComponent component) => GetComposite()?.RemoveComponent(component) ?? false;

    /// <summary>
    /// Gets a read-only collection of the child TestComponent instances of the current object.
    /// </summary>
    public IReadOnlyCollection<TestComponent> Components => GetComposite()?.Components ?? [];

    internal virtual CompositeTests? GetComposite() => null;
}
