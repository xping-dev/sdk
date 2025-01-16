/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using Xping.Sdk.Core.Components;
using Xping.Sdk.Core.Configuration;
using Xping.Sdk.Core.Instrumentation;
using Xping.Sdk.Core.Models;
using Xping.Sdk.Core.Services;
using Xping.Sdk.Shared;

namespace Xping.Sdk.Core.Session;

/// <summary>
/// The TestContext class is responsible for maintaining the state of the test execution. It encapsulates the test 
/// session builder, test component, instrumentationFactory log, and progress reporter.
/// </summary>
public class TestContext
{
    /// <summary>
    /// Initializes new instance of the TestContext class.
    /// </summary>
    /// <param name="url">A Uri object that represents the URL of the page being validated.</param>
    /// <param name="settings">The test settings for a test execution.</param>
    /// <param name="sessionBuilder">The session builder used to create test sessions.</param>
    /// <param name="instrumentationFactory">
    /// The instrumentation factory timer associated with the current context to record test execution details.
    /// </param>
    /// <param name="progress">Optional progress reporter for tracking test execution progress.</param>
    public TestContext(
        Uri url,
        TestSettings settings,
        ITestSessionBuilder sessionBuilder,
        IInstrumentationFactory instrumentationFactory,
        IProgress<TestStep>? progress = null)
    {
        Url = url;
        Settings = settings;
        SessionBuilder = sessionBuilder.RequireNotNull(nameof(sessionBuilder));
        InstrumentationFactory = instrumentationFactory.RequireNotNull(nameof(instrumentationFactory));
        Progress = progress;
        PropertyBag = new();
    }

    /// <summary>
    /// Gets a Uri object that represents the URL of the page being validated.
    /// </summary>
    public Uri Url { get; }

    /// <summary>
    /// Gets the test settings for a test execution.
    /// </summary>
    public TestSettings Settings { get; }

    /// <summary>
    /// Gets an instance of the `ITestSessionBuilder` interface that is used to build test sessions.
    /// </summary>
    public ITestSessionBuilder SessionBuilder { get; }

    /// <summary>
    /// Gets the instrumentationFactory timer associated with the current context to record test execution details.
    /// </summary>
    public IInstrumentationFactory InstrumentationFactory { get; }

    /// <summary>
    /// Gets an optional object that can be used to report progress updates for the current operation.
    /// </summary>
    public IProgress<TestStep>? Progress { get; }

    /// <summary>
    /// Gets the test component associated with the current context that executes an action or validate test operation.
    /// </summary>
    public TestComponent CurrentComponent { get; private set; } = null!;

    /// <summary>
    /// Gets the property bag used to share data between different test operations.
    /// In the execution pipeline, test steps may depend on each other and require shared data. For instance, the 
    /// DnsLookupComponent might share its IP addresses, or the BrowserClient might store page information.
    /// Note that the data stored in this context will not be serialized and will be disposed of after the pipeline 
    /// execution.
    /// </summary>
    public PropertyBag PropertyBag { get; }

    /// <summary>
    /// Updates the TestContext with the currently executing TestComponent and resets the instrumentationFactory timer.
    /// This method should be called before executing a new TestComponent to ensure accurate timing and state tracking.
    /// </summary>
    /// <param name="newComponent">The new TestComponent that is about to be executed.</param>
    public void UpdateExecutionContext(TestComponent newComponent)
    {
        // Update the currently executing TestComponent
        this.CurrentComponent = newComponent.RequireNotNull(nameof(newComponent));
    }
}
