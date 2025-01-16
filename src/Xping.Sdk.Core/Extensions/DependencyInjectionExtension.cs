/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using Polly;
using Xping.Sdk.Core.BrowserManagement;
using Xping.Sdk.Core.BrowserManagement.Internals;
using Xping.Sdk.Core.Configuration;
using Xping.Sdk.Core.HttpClients;
using Xping.Sdk.Core.HttpClients.Internals;
using Xping.Sdk.Core.Services;
using Xping.Sdk.Core.Services.Internals;

namespace Xping.Sdk.Core.Extensions;

/// <summary>
/// Provides extension methods for the IServiceCollection interface to register test-related services.
/// </summary>
public static class DependencyInjectionExtension
{
    /// <summary>
    /// This extension method adds the IHttpClientFactory service and related services to service collection and 
    /// configures a named <see cref="HttpClient"/> clients. This is required if you want to make HTTP requests throught 
    /// <see cref="HttpClient"/> object.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">The <see cref="HttpClientFactoryOptions"/>.</param>
    /// <returns><see cref="IServiceCollection"/> object.</returns>
    public static IServiceCollection AddHttpClientFactory(
        this IServiceCollection services,
        Action<IServiceProvider, HttpClientFactoryOptions>? configuration = null)
    {
        var factoryConfiguration = new HttpClientFactoryOptions();
        configuration?.Invoke(services.BuildServiceProvider(), factoryConfiguration);

        services.AddHttpClient(HttpClientFactoryOptions.HttpClientWithNoRetryPolicy)
                .ConfigurePrimaryHttpMessageHandler(configureHandler =>
                {
                    return CreateSocketsHttpHandler(factoryConfiguration);
                });

        services.AddHttpClient(HttpClientFactoryOptions.HttpClientWithRetryPolicy)
                .ConfigurePrimaryHttpMessageHandler(configureHandler =>
                {
                    return CreateSocketsHttpHandler(factoryConfiguration);
                })
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(
                    sleepDurations: factoryConfiguration.SleepDurations))
                .AddTransientHttpErrorPolicy(builder => builder.CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: factoryConfiguration.HandledEventsAllowedBeforeBreaking,
                    durationOfBreak: factoryConfiguration.DurationOfBreak));

        return services;
    }

    /// <summary>
    /// Adds an IHttpClientFactory implemented by <see cref="WebApplicationHttpClientFactory"/> to the service 
    /// collection. This factory is specifically designed to align with system requirements for returning HttpClient 
    /// instances produced by WebApplicationFactory.
    /// </summary>
    /// <remarks>
    /// The IHttpClientFactory provides a central location for creating and configuring HttpClient instances.
    /// It ensures efficient reuse of HttpClient instances, manages their lifetime, and allows customization of their 
    /// behavior. The WebApplicationHttpClientFactory is tailored to work seamlessly with WebApplicationFactory and its 
    /// associated services.
    /// </remarks>
    /// <param name="services">The service collection to add the service to.</param>
    /// <param name="createClient">
    /// A function that creates an HttpClient instance, typically WebApplicationFactory.CreateClient method.
    /// </param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddTestServerHttpClientFactory(
        this IServiceCollection services,
        Func<WebApplicationFactoryClientOptions, HttpClient> createClient)
    {
        var factoryConfiguration = new HttpClientFactoryOptions();
        services.TryAddTransient<IHttpClientFactory>(implementationFactory:
            serviceProvider => new WebApplicationHttpClientFactory(createClient, factoryConfiguration));

        return services;
    }

    /// <summary>
    /// This extension method adds the necessary services to create headless browser.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <returns><see cref="IServiceCollection"/> object.</returns>
    public static IServiceCollection AddBrowserClientFactory(this IServiceCollection services)
    {
        services.AddSingleton(provider => Playwright.CreateAsync().Result);
        services.AddSingleton<IBrowserFactory, BrowserFactory>();
        services.AddSingleton<IBrowserContextFactory, BrowserContextFactory>();
        services.AddSingleton<IPageFactory, PageFactory>();
        services.AddTransient<BrowserManager>();

        return services;
    }

    /// <summary>
    /// Adds a named test agent to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the service to.</param>
    /// <param name="serviceKey">The service key.</param>
    /// <param name="configureOptions">An action that configures the the test agent options.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddTestAgent(
        this IServiceCollection services,
        string serviceKey,
        Action<TestAgentOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddHttpClient(name: TestSessionUploader.HttpClientName);
        services.TryAddTransient<ITestSessionUploader, TestSessionUploader>();
        services.TryAddTransient<ITestSessionBuilderFactory, TestSessionBuilderFactory>();
        services.TryAddTransient<ITestContextFactory, TestContextFactory>();
        services.TryAddKeyedTransient<TestAgent>(serviceKey);

        return services;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddTestAgent(
        this IServiceCollection services, 
        Action<TestAgentOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddHttpClient(name: TestSessionUploader.HttpClientName);
        services.TryAddTransient<ITestSessionUploader, TestSessionUploader>();
        services.TryAddTransient<ITestSessionBuilderFactory, TestSessionBuilderFactory>();
        services.TryAddTransient<ITestContextFactory, TestContextFactory>();
        services.TryAddTransient<TestAgent>();

        return services;
    }

    /// <summary>
    /// Checks if a specific service type is already registered in the service collection.
    /// </summary>
    /// <typeparam name="TService">The type of the service to check for registration.</typeparam>
    /// <param name="services">The IServiceCollection instance to search in.</param>
    /// <returns>
    /// True if the service type is registered; otherwise, false.
    /// </returns>
    public static bool IsServiceRegistered<TService>(this IServiceCollection services)
    {
        return services.Any(serviceDescriptor => serviceDescriptor.ServiceType == typeof(TService));
    }

    private static SocketsHttpHandler CreateSocketsHttpHandler(
        HttpClientFactoryOptions factoryConfiguration) => new()
        {
            PooledConnectionLifetime = factoryConfiguration.PooledConnectionLifetime,
            AutomaticDecompression = factoryConfiguration.AutomaticDecompression,
            AllowAutoRedirect = HttpClientFactoryOptions.HttpClientAutoRedirects,
            UseCookies = HttpClientFactoryOptions.HttpClientHandleCookies,
        };
}
