/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using System.Net;

namespace Xping.Sdk.Core.Session.Collector;

/// <summary>
/// Interface for uploading test sessions to the xping.io server.
/// </summary>
/// <remarks>
/// The uploaded test session data is utilized to create a comprehensive dashboard, enabling further analysis and 
/// maintaining historical data for performance tracking and comparison.
/// </remarks>
public interface ITestSessionUploader
{
    /// <summary>
    /// Asynchronously uploads a test session to the xping.io server.
    /// </summary>
    /// <param name="testSession">The test session to upload.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous upload operation, containing the HTTP status code of the response.
    /// </returns>
    Task<HttpStatusCode> UploadAsync(TestSession testSession, CancellationToken cancellationToken = default);
}