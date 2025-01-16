/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

namespace Xping.Sdk.Core.Instrumentation;

/// <summary>
/// Provides a factory interface for creating instances of <see cref="InstrumentationTimer"/>.
/// </summary>
public interface IInstrumentationFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="InstrumentationTimer"/> with the specified name.
    /// </summary>
    /// <param name="name">The name of the timer to create.</param>
    /// <param name="callback">An optional callback action that will be invoked when the timer is disposed.</param>
    /// <param name="startStopwatch">A flag indicating whether to start the stopwatch automatically.</param>
    /// <returns>A new instance of <see cref="InstrumentationTimer"/>.</returns>
    InstrumentationTimer CreateTimer(
        string name,
        Action<InstrumentationTimer>? callback = null,
        bool startStopwatch = true);
}
