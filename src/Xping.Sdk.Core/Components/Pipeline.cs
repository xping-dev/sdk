/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using Xping.Sdk.Core.Session;

namespace Xping.Sdk.Core.Components;

/// <summary>
/// The Pipeline class is a concrete implementation of the <see cref="CompositeTests"/> class that is designed to run 
/// and manage the test components that have been added.
/// </summary>
public class Pipeline : CompositeTests
{
    /// <summary>
    /// The name of the test component that represents a pipeline of tests.
    /// </summary>
    /// <remarks>
    /// This constant is used to register the Pipeline class in the test framework.
    /// </remarks>
    public const string ComponentName = nameof(Pipeline);

    /// <summary>
    /// Initializes a new instance of the Pipeline class with the specified name and components.
    /// </summary>
    /// <param name="name">The optional name of the pipeline. If null, the ComponentName constant is used.</param>
    /// <param name="components">The test components that make up the pipeline.</param>
    /// <remarks>
    /// The Pipeline class inherits from the CompositeTests class and represents a sequence of tests that can be 
    /// executed as a single unit.
    /// </remarks>
    public Pipeline(
        string? name = null,
        params TestComponent[] components) : base(name ?? ComponentName)
    {
        if (components != null)
        {
            foreach (var component in components)
            {
                AddComponent(component);
            }
        }
    }

    /// <summary>
    /// This method is designed to perform the test components that have been included in the current object.
    /// </summary>
    /// <param name="context">A <see cref="TestContext"/> object that represents the test context.</param>
    /// <param name="cancellationToken">An optional CancellationToken object that can be used to cancel this operation.
    /// </param>
    public override async Task<TestStepResult> HandleAsync(
        TestContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        TestStepResult pipelineResult = TestStepResult.Succeeded;

        foreach (var component in Components)
        {
            using var tokenSource = new CancellationTokenSource(context.Settings.Timeout);

            // Update context with currently executing component.
            context.UpdateExecutionContext(component);

            var result = await component
                .HandleAsync(context, tokenSource.Token)
                .ConfigureAwait(false);

            if (result == TestStepResult.Failed)
            {
                pipelineResult = TestStepResult.Failed;

                // If the 'ContinueOnFailure' property is set to false and the component has failed, then break the loop.
                if (!context.Settings.ContinueOnFailure)
                {
                    break;
                }
            }
        }

        return pipelineResult;
    }
}
