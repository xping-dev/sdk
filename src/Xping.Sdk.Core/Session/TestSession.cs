/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using Xping.Sdk.Core.Extensions;
using Xping.Sdk.Core.HttpClients;
using Xping.Sdk.Core.Models;
using Xping.Sdk.Shared;

namespace Xping.Sdk.Core.Session;

/// <summary>
/// Represents a test session that contains a collection of test steps and their results.
/// </summary>
/// <remarks>
/// A test session is a class that represents a test execution and its attributes. It consists of one or more test steps 
/// that execute different actions or validations on the URL, such as DNS lookup, HTTP request, HTML parsing, or 
/// headless browser interaction. A test session has a start date and a duration that indicate when and how long the 
/// testing took place. It also has a state that indicates the overall status of the test session, such as completed, 
/// not started, or declined. A test session can store various data related to the test operation in a 
/// <see cref="PropertyBag{TValue}"/>, which is a dictionary of key-value pairs of serializable objects. 
/// The property bag can contain data such as resolved IP addresses from DNS lookup, HTTP response headers, HTML 
/// content, or captured screenshots from the headless browsers.
/// A test session can be serialized and deserialized to and from stream, using the 
/// <see cref="Utilities.Serialization.TestSessionSerializer"/>. 
/// This enables the test session to be saved and loaded for further analysis and comparison, or transferred between 
/// different machines or applications.
/// </remarks>
[Serializable]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class TestSession : 
    IDisposable, IAsyncDisposable, ISerializable, IDeserializationCallback, IEquatable<TestSession>
{
    private bool _disposedValue;

    /// <summary>
    /// Gets the unique identifier of the test session.
    /// </summary>
    /// <value>
    /// A <see cref="Guid"/> value that represents the test session ID.
    /// </value>
    public Guid Id { get; }

    /// <summary>
    /// Gets the upload token for the test session that links the TestAgent's results to the project configured on 
    /// the server.
    /// </summary>
    public required Guid UploadToken { get; init; }

    /// <summary>
    /// A Uri object that represents the URL of the page being validated.
    /// </summary>
    public required Uri Url { get; init; }

    /// <summary>
    /// Gets the start date of the test session.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> object that represents the start time of the test session.
    /// </value>
    public required DateTime StartDate { get; init; }

    /// <summary>
    /// Returns a read-only collection of the test steps executed within current test session.
    /// </summary>
    public required IReadOnlyCollection<TestStep> Steps { get; init; }

    /// <summary>
    /// Gets the state of the test session.
    /// </summary>
    /// <value>
    /// A <see cref="TestSessionState"/> enum that represents the state of the current test session.
    /// </value>
    public required TestSessionState State { get; init; }

    /// <summary>
    /// Returns decline reason for the current test session.
    /// </summary>
    /// <remarks>
    /// A test session may be declined if it attempts to run a test component that needs a headless browser, but no 
    /// headless browser is available. Another cause of a declined test session is invalid data that blocks the test 
    /// execution.
    /// </remarks>
    public string? DeclineReason { get; init; }

    /// <summary>
    /// Returns a read-only collection of the failed test steps within current test session.
    /// </summary>
    public IReadOnlyCollection<TestStep> Failures =>
        Steps.Where(step => step.Result == TestStepResult.Failed).ToList().AsReadOnly();

    /// <summary>
    /// Returns a boolean value indicating whether the test session is valid or not.
    /// </summary>
    /// <remarks>
    /// Valid test session has all test steps completed successfully. Check <see cref="Failures"/> to get 
    /// failed test steps.
    /// </remarks>
    public bool IsValid => Steps.Count > 0 && !Steps.Any(step => step.Result == TestStepResult.Failed);

    /// <summary>
    /// Gets the total duration of all the steps in the test session.
    /// </summary>
    /// <value>
    /// A <see cref="TimeSpan"/> object that represents the sum of the durations of all the steps.
    /// </value>
    public TimeSpan Duration => Steps.Aggregate(TimeSpan.Zero, (elapsedTime, step) => elapsedTime + step.Duration);

    /// <summary>
    /// Initializes a new instance of the <see cref="TestSession"/> class.
    /// </summary>
    public TestSession()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestSession"/> class with serialized data.
    /// </summary>
    /// <param name="info">
    /// The <see cref="SerializationInfo"/> that holds the serialized object data about the test session.
    /// </param>
    /// <param name="context">
    /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
    /// </param>
    public TestSession(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));

        Id = (Guid)info.GetValue(nameof(Id), typeof(Guid)).RequireNotNull(nameof(Id));
        Url = (Uri)info.GetValue(nameof(Url), typeof(Uri)).RequireNotNull(nameof(Url));
        StartDate = (DateTime)info.GetValue(nameof(StartDate), typeof(DateTime)).RequireNotNull(nameof(StartDate));
        Steps = info.GetValue(nameof(Steps), typeof(TestStep[])) as TestStep[] ?? [];
        State = Enum.Parse<TestSessionState>(
            value: (string)info.GetValue(nameof(State), typeof(string)).RequireNotNull(nameof(State)));
        DeclineReason = info.GetValue(nameof(DeclineReason), typeof(string)) as string;
    }

    /// <summary>
    /// Returns a string that represents the current TestSession object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendFormat(
            CultureInfo.InvariantCulture,
            $"{StartDate} ({Duration.GetFormattedTime()}) " +
            $"Test session {State.GetDisplayName()} for {Url.AbsoluteUri}." +
            $"{Environment.NewLine}");
        sb.AppendFormat(
            CultureInfo.InvariantCulture,
            $"Total steps: {Steps.Count}, Success: {Steps.Count - Failures.Count}, Failures: {Failures.Count}" +
            $"{Environment.NewLine}{Environment.NewLine}");

        return sb.ToString();
    }

    /// <summary>
    /// Determines whether the current TestSession object is equal to a specified object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>
    /// <c>true</c> if the current object and obj are both TestSession objects and have the same id; otherwise, 
    /// <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as TestSession);
    }

    /// <summary>
    /// Determines whether the current TestSession object is equal to another TestSession object.
    /// </summary>
    /// <param name="other">The TestSession object to compare with the current object.</param>
    /// <returns><c>true</c>if the current object and other have the same id; otherwise, <c>false</c>.</returns>
    public bool Equals(TestSession? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Id == other.Id;
    }

    /// <summary>
    /// Determines whether two TestSession objects have the same id.
    /// </summary>
    /// <param name="lhs">The first TestSession object to compare.</param>
    /// <param name="rhs">The second TestSession object to compare.</param>
    /// <returns><c>true</c> if lhs and rhs have the same id; otherwise, <c>false</c>.</returns>
    public static bool operator ==(TestSession? lhs, TestSession? rhs)
    {
        if (lhs is null || rhs is null)
        {
            return Equals(lhs, rhs);
        }

        return lhs.Equals(rhs);
    }

    /// <summary>
    /// Determines whether two TestSession objects have different id.
    /// </summary>
    /// <param name="lhs">The first TestSession object to compare.</param>
    /// <param name="rhs">The second TestSession object to compare.</param>
    /// <returns><c>true</c> if lhs and rhs have different id; otherwise, <c>false</c>.</returns>
    public static bool operator !=(TestSession? lhs, TestSession? rhs)
    {
        if (lhs is null || rhs is null)
        {
            return !Equals(lhs, rhs);
        }

        return !lhs.Equals(rhs);
    }

    /// <summary>
    /// Returns the hash code for the current TestSession object.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
        // Calculate a hash code based on the same properties used in Equals
        return Id.GetHashCode();
    }

    /// <summary>
    /// Releases the resources stored in the TestSession.
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Asynchronously releases the resources stored in the TestSession.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(Id), Id, typeof(Guid));
        info.AddValue(nameof(Url), Url, typeof(Uri));
        info.AddValue(nameof(StartDate), StartDate, typeof(DateTime));
        info.AddValue(nameof(Steps), Steps.ToArray(), typeof(TestStep[]));
        info.AddValue(nameof(State), State.ToString(), typeof(string));
        info.AddValue(nameof(DeclineReason), DeclineReason, typeof(string));
    }

    void IDeserializationCallback.OnDeserialization(object? sender)
    {
        if (!Uri.TryCreate(Url.ToString(), UriKind.Absolute, out _))
        {
            throw new SerializationException($"The Url parameter is not a valid Uri: {Url}");
        }
    }

    private string GetDebuggerDisplay() =>
        $"{StartDate} ({Duration.GetFormattedTime()}), Steps: {Steps.Count}, Failures: {Failures.Count} ";

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                var httpResponseMessage = this.GetNonSerializablePropertyBagValue<HttpResponseMessage>
                    (PropertyBagKeys.HttpResponseMessage);
                httpResponseMessage?.Dispose();

                var browserResponseMessage = this.GetNonSerializablePropertyBagValue<BrowserResponseMessage>
                    (PropertyBagKeys.BrowserResponseMessage);
                browserResponseMessage?.Dispose();
            }

            _disposedValue = true;
        }
    }

    private async Task DisposeAsyncCore()
    {
        var browserResponseMessage = this.GetNonSerializablePropertyBagValue<BrowserResponseMessage>
            (PropertyBagKeys.BrowserResponseMessage);

        if (browserResponseMessage != null)
        {
            await browserResponseMessage.DisposeAsync().ConfigureAwait(false);
        }

        var httpResponseMessage = this.GetNonSerializablePropertyBagValue<HttpResponseMessage>
            (PropertyBagKeys.HttpResponseMessage);
        httpResponseMessage?.Dispose();
    }
}