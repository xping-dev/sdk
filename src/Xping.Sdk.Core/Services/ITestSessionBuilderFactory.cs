/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

namespace Xping.Sdk.Core.Services;

/// <summary>
/// Factory interface for creating instances of <see cref="ITestSessionBuilder"/>.
/// </summary>
/// <remarks>
/// This interface provides a method to create a new instance of a test session builder, which can be used to configure
/// and build a test session with various parameters such as URL, start date, test steps, and errors.
/// </remarks>
public interface ITestSessionBuilderFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="ITestSessionBuilder"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="ITestSessionBuilder"/>.</returns>
    ITestSessionBuilder CreateSessionBuilder();
}
