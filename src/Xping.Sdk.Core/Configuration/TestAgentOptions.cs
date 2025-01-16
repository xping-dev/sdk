/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

namespace Xping.Sdk.Core.Configuration;

/// <summary>
/// Configuration options for the Test Agent.
/// </summary>
public class TestAgentOptions
{
    /// <summary>
    /// Gets or sets the API key used for authentication with Xping API.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the upload token that links the TestAgent's results to the project configured on the server.
    /// This token facilitates the upload of testing sessions to the server for further analysis. If set to <c>null</c>,
    /// no uploads will occur. Default is <c>null</c>. This property can be modified later in the TestAgent object,
    /// which will override the value set here.
    /// </summary>
    public string? UploadToken { get; set; }

    /// <summary>
    /// Controls whether the TestAgent's pipeline container object should be instantiated for each thread separately.
    /// When set to true, each thread will have its own instance of the pipeline container. Default is <c>true</c>.
    /// This property can be modified later in the TestAgent object, which will override the value set here.
    /// </summary>
    public bool InstantiatePerThread { get; set; } = true;
}
