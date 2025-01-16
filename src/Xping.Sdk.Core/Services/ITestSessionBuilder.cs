/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using Xping.Sdk.Core.Models;
using Xping.Sdk.Core.Session;

namespace Xping.Sdk.Core.Services;

/// <summary>
/// Interface for building a test session with various configurations.
/// </summary>
public interface ITestSessionBuilder
{
    /// <summary>
    /// Configures the builder with a URL.
    /// </summary>
    /// <param name="url">The URL to be used in the test session.</param>
    /// <returns>The current instance of the builder.</returns>
    ITestSessionBuilder BuildWith(Uri url);

    /// <summary>
    /// Configures the builder with a start date.
    /// </summary>
    /// <param name="startDate">The start date of the test session.</param>
    /// <returns>The current instance of the builder.</returns>
    ITestSessionBuilder BuildWith(DateTime startDate);

    /// <summary>
    /// Configures the builder with a test agent and an error.
    /// </summary>
    /// <param name="err">The error to be associated with the test session.</param>
    /// <returns>The current instance of the builder.</returns>
    ITestSessionBuilder BuildWith(Error err);

    /// <summary>
    /// Configures the builder with a test step.
    /// </summary>
    /// <param name="testStep">The test step to be included in the test session.</param>
    /// <returns>The current instance of the builder.</returns>
    ITestSessionBuilder BuildWith(TestStep testStep);

    /// <summary>
    /// Configures the builder with an upload token.
    /// </summary>
    /// <param name="uploadToken">The upload token to be used in the test session.</param>
    /// <returns>The current instance of the builder.</returns>
    ITestSessionBuilder BuildWith(Guid uploadToken);

    /// <summary>
    /// Builds and returns the configured test session.
    /// </summary>
    /// <returns>The configured test session.</returns>
    TestSession Build();
}
