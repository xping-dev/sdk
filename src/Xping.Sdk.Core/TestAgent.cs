/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xping.Sdk.Core.Components;
using Xping.Sdk.Core.Configuration;
using Xping.Sdk.Core.Extensions;
using Xping.Sdk.Core.Services;
using Xping.Sdk.Core.Session;
using static Xping.Sdk.Core.Models.Errors;

namespace Xping.Sdk.Core;

/// <summary>
/// The TestAgent class is the main class that performs the testing logic of the Xping SDK. It runs test components, 
/// for example action or validation steps, such as DnsLookup, IPAddressAccessibilityCheck etc., using the HTTP client 
/// and the headless browser. It also creates a test session object that summarizes the outcome of the test operations.
/// </summary>
/// <remarks>
/// The TestAgent class performs the core testing logic of the Xping SDK. It has two methods that can execute test 
/// components that have been added to a container: <see cref="RunAsync(Uri, TestSettings, CancellationToken)"/> and 
/// <see cref="ProbeAsync(Uri, TestSettings, CancellationToken)"/>. The former executes the test components and 
/// creates a test session that summarizes the test operations. The latter serves as a quick check to ensure that the 
/// test components are properly configured and do not cause any errors. The TestAgent class collects various data 
/// related to the test execution. It constructs a <see cref="TestSession"/> object that represents the outcome of the 
/// test operations, which can be serialized, analyzed, or compared. The TestAgent class can be configured with various 
/// settings, such as the timeout, using the <see cref="TestSettings"/> class.
/// <para>
/// <note type="important">
/// Please note that the TestAgent class is designed to be used with a dependency injection system and should not be 
/// instantiated by the user directly. Instead, the user should register the TestAgent class in the dependency injection 
/// container using one of the supported methods, such as the 
/// <see cref="DependencyInjectionExtension.AddTestAgent(IServiceCollection)"/> extension method for 
/// the IServiceCollection interface. This way, the TestAgent class can be resolved and injected into other classes that 
/// depend on it.
/// </note>
/// <example>
/// <code>
/// Host.CreateDefaultBuilder(args)
///     .ConfigureServices((services) =>
///     {
///         services.AddHttpClientFactory();
///         services.AddTestAgent(options =>
///         {
///             options.ApiKey = "--- Your API Key ---";
///         });
///     });
/// </code>
/// </example>
/// </para>
/// </remarks>
public class TestAgent : IDisposable
{
    private bool _disposedValue;
    private Guid _uploadToken;

    private readonly IServiceProvider _serviceProvider;
    private readonly ITestSessionUploader _sessionUploader;
    private readonly ITestSessionBuilderFactory _sessionBuilderFactory;
    private readonly ITestContextFactory _contextFactory;

    // Lazy initialization of a shared pipeline container that is thread-safe. This ensures that only a single instance
    // of Pipeline is created and shared across all threads, and it is only created when it is first accessed and
    // property InstantiatePerThread is disabled.
    private readonly Lazy<Pipeline> _sharedContainer = new(valueFactory: () =>
    {
        return new Pipeline($"Pipeline:Shared");
    }, isThreadSafe: true);

    // ThreadLocal is used to provide a separate instance of the pipeline container for each thread.
    private readonly ThreadLocal<Pipeline> _container = new(valueFactory: () =>
    {
        // Initialize the container for each thread.
        return new Pipeline($"Pipeline:Thread[{Thread.CurrentThread.Name}]:#{Environment.CurrentManagedThreadId}");
    });

    /// <summary>
    /// Controls whether the TestAgent's pipeline container object should be instantiated for each thread separately.
    /// When set to true, each thread will have its own instance of the pipeline container. Default is <c>true</c>.
    /// </summary>
    public bool InstantiatePerThread { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    public IServiceProvider ServiceProvider => _serviceProvider;

    /// <summary>
    /// Gets or sets the upload token that links the TestAgent's results to the project configured on the server.
    /// This token facilitates the upload of testing sessions to the server for further analysis. If set to <c>null</c>,
    /// no uploads will occur. Default is <c>null</c>.
    /// </summary>
    public string? UploadToken
    {
        get => _uploadToken.ToString();
        set => _uploadToken = !string.IsNullOrWhiteSpace(value) &&
                   Guid.TryParse(value, out Guid result) ? result : Guid.Empty;
    }

    /// <summary>
    /// Retrieves the <see cref="Pipeline"/> instance that serves as the execution container.
    /// </summary>
    /// <remarks>
    /// This property provides access to the appropriate <see cref="Pipeline"/> instance based on the current 
    /// configuration. If configured for per-thread instantiation, it returns a thread-specific <see cref="Pipeline"/> 
    /// instance, ensuring thread safety. In a shared configuration, it provides a common <see cref="Pipeline"/> 
    /// instance for all threads.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// This exception is thrown if a per-thread <see cref="Pipeline"/> instance is not available, which 
    /// could indicate an issue with its construction or an attempt to access it post-disposal.
    /// </exception>
    public Pipeline Container => InstantiatePerThread ?
        _container.Value ?? throw new InvalidOperationException(
            "The Pipeline instance for the current thread could not be retrieved. This may occur if the Pipeline " +
            "constructor threw an exception or if the ThreadLocal instance was accessed after disposal.") :
        _sharedContainer.Value;

    /// <summary>
    /// Initializes new instance of the TestAgent class. For internal use only.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="options"></param>
    /// <param name="sessionBuilderFactory"></param>
    /// <param name="sessionUploader"></param>
    /// <param name="contextFactory"></param>
    internal TestAgent(
        IServiceProvider serviceProvider,
        ITestSessionUploader sessionUploader,
        ITestSessionBuilderFactory sessionBuilderFactory,
        ITestContextFactory contextFactory,
        IOptions<TestAgentOptions>? options)
    {
        _serviceProvider = serviceProvider;
        _sessionUploader = sessionUploader;
        _sessionBuilderFactory = sessionBuilderFactory;
        _contextFactory = contextFactory;

        if (options?.Value != null)
        {
            UploadToken = options.Value.UploadToken;
            InstantiatePerThread = options.Value.InstantiatePerThread;
        }
    }

    /// <summary>
    /// This method executes the tests. After the tests operations are executed, it constructs a test session that 
    /// represents the outcome of the tests operations.
    /// </summary>
    /// <param name="url">A Uri object that represents the URL of the page being validated.</param>
    /// <param name="settings">
    /// Optional <see cref="TestSettings"/> object that contains the settings for the tests.
    /// </param>
    /// <param name="cancellationToken">
    /// An optional CancellationToken object that can be used to cancel the validation process.</param>
    /// <returns>
    /// Returns a Task&lt;TestSession&gt; object that represents the asynchronous outcome of testing operation.
    /// </returns>
    public async Task<TestSession> RunAsync(
        Uri url,
        TestSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(url);

        var sessionBuilder = _sessionBuilderFactory
            .CreateSessionBuilder()
            .BuildWith(_uploadToken)
            .BuildWith(url)
            .BuildWith(startDate: DateTime.UtcNow);

        var context = _contextFactory.CreateInstrumentedContext(url, settings, sessionBuilder);

        try
        {
            // Update context with currently executing component.
            context.UpdateExecutionContext(Container);

            // Execute test operation on the current thread's container or shared instance based on
            // `InstantiatePerThread` property configuration.
            await Container
                .HandleAsync(context, cancellationToken)
                .ConfigureAwait(false);

            // 
            context.Clean();
        }
        catch (Exception ex)
        {
            sessionBuilder.BuildWith(ExceptionError(ex));
        }

        TestSession testSession = sessionBuilder.Build();
        return testSession;
    }

    /// <summary>
    /// Cleans up test components from the container.
    /// </summary>
    /// <remarks>
    /// This method is called to clean up the test components. It ensures that the components collection is emptied, 
    /// preventing cross-contamination of state between tests. If <see cref="InstantiatePerThread"/> is enabled, this 
    /// method only cleans the container that has been instantiated on the calling thread.
    /// </remarks>
    public void Cleanup()
    {
        Container.Clear();
    }

    /// <summary>
    /// Releases the resources used by the TestAgent.
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the resources used by the TestAgent.
    /// </summary>
    /// <param name="disposing">A flag indicating whether to release the managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                foreach (var pipeline in _container.Values)
                {
                    pipeline.Clear();
                }

                if (_sharedContainer.IsValueCreated)
                {
                    _sharedContainer.Value.Clear();
                }

                _container.Dispose();
            }

            _disposedValue = true;
        }
    }
}
