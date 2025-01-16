/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using Xping.Sdk.Core.Configuration;
using Xping.Sdk.Core.Session;

namespace Xping.Sdk.Core.Services;

/// <summary>
/// Represents a factory for creating test context objects.
/// </summary>
public interface ITestContextFactory
{
    /// <summary>
    /// Creates a new test context object with the specified session builder.
    /// </summary>
    /// <remarks>
    /// The session builder instance must be provided by the TestAgent to ensure that the same object is used to build 
    /// the test session in a single run.
    /// </remarks>
    TestContext CreateInstrumentedContext(
        Uri url,
        TestSettings? settings,
        ITestSessionBuilder sessionBuilder);
}
