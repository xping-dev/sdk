/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using System.Diagnostics;
using Xping.Sdk.Shared;

namespace Xping.Sdk.Core.Instrumentation;

/// <summary>
/// This class provides a set utilities that can be used to log data related to code execution time.
/// </summary>
/// <example>
/// <code>
/// using (var log = InstrumentationTimer())
/// {
///     // start of the code for which to measure execution time
///     // end of the code
///     Console.WriteLine($"Code started at {log.StartTime} and took {log.ElapsedTime.TotalMilliseconds} [ms] to run. 
/// }
/// </code>
/// </example>
public sealed class InstrumentationTimer : IDisposable
{
    private bool _isDisposed;
    private readonly ThreadLocal<Stopwatch> _threadLocalStopwatch;
    private readonly Action<InstrumentationTimer>? _callback;
    private DateTime _startTime = DateTime.Today;
    private readonly string _name;

    /// <summary>
    /// Initializes a new instance of the InstrumentationTimer class.
    /// </summary>
    /// <param name="name">The name of the instrumentation timer.</param>
    /// <param name="callback">An optional callback action that will be invoked when the instance is disposed.</param>
    /// <param name="startStopwatch">A flag indicating whether to start the stopwatch automatically.</param>
    public InstrumentationTimer(string name, Action<InstrumentationTimer>? callback = null, bool startStopwatch = true)
    {
        _name = name;
        _callback = callback;
        _threadLocalStopwatch = new(valueFactory: () => new Stopwatch());

        if (startStopwatch)
        {
            Restart();
        }
    }

    /// <summary>
    /// Gets the name of the instrumentation timer.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Gets the total elapsed time measured by the current instaince, in milliseconds.
    /// </summary>
    public long ElapsedMilliseconds => ThreadLocalStopwatch.ElapsedMilliseconds;

    /// <summary>
    /// Gets the total elapsed time measured by the current instance, in timer ticks.
    /// </summary>
    public long ElapsedTicks => ThreadLocalStopwatch.ElapsedTicks;

    /// <summary>
    /// Gets the total elapsed time measured by the current instance.
    /// </summary>
    public TimeSpan ElapsedTime => ThreadLocalStopwatch.Elapsed;

    /// <summary>
    /// Gets the time that the associated stopwatch was started.
    /// </summary>
    public DateTime StartTime => _startTime;

    /// <summary>
    /// Gets a value indicating whether the stopwatch timer is running.
    /// </summary>
    public bool IsRunning => ThreadLocalStopwatch.IsRunning;

    /// <summary>
    /// Stops time measurement, resets the elapsed time to zero and start time to current time and starts measuring.
    /// </summary>
    public void Restart()
    {
        _startTime = DateTime.UtcNow;
        ThreadLocalStopwatch.Restart();
    }

    /// <summary>
    /// Releases the resources used by the InstrumentationTimer instance.
    /// </summary>
    /// <remarks>
    /// This method stops the stopwatch and invokes the callback action if provided.
    /// </remarks>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (disposing)
        {
            ThreadLocalStopwatch.Stop();
            _callback?.Invoke(this);
            _threadLocalStopwatch.Dispose();
        }

        _isDisposed = true;
    }

    private Stopwatch ThreadLocalStopwatch =>
        _threadLocalStopwatch.Value.RequireNotNull(nameof(_threadLocalStopwatch));
}
