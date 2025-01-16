/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using Xping.Sdk.Core.Configuration;
using Xping.Sdk.Core.Instrumentation;
using Xping.Sdk.Core.Session;

namespace Xping.Sdk.Core.Services.Internals;

internal class TestContextFactory(
    IInstrumentationFactory instrumentationFactory,
    IProgress<TestStep>? progress) : ITestContextFactory
{
    public TestContext CreateInstrumentedContext(
        Uri url,
        TestSettings? settings,
        ITestSessionBuilder sessionBuilder)
    {
        return new TestContext(url, settings ?? new TestSettings(), sessionBuilder, instrumentationFactory, progress);
    }
}
