/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

namespace Xping.Sdk.Core.Services.Internals;

internal class TestSessionBuilderFactory : ITestSessionBuilderFactory
{
    public ITestSessionBuilder CreateSessionBuilder()
    {
        return new TestSessionBuilder();
    }
}
