/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

namespace Xping.Sdk.Core.Instrumentation.Internals;

internal class InstrumentationFactory : IInstrumentationFactory
{
    public InstrumentationTimer CreateTimer(
        string name,
        Action<InstrumentationTimer>? callback = null,
        bool startStopwatch = true)
    {
        return new InstrumentationTimer(name, callback, startStopwatch);
    }
}
