/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xping.Sdk.Core;
using Xping.Sdk.Core.Session;
using Microsoft.Extensions.Logging;
using Xping.Sdk.Extensions;
using Xping.Sdk.Core.Extensions;

namespace Xping.Sdk.IntegrationTests.TestFixtures;

public static class TestFixtureProvider
{
    private static readonly Lazy<IHost> HostInstance = new(valueFactory: CreateHost, isThreadSafe: true);

    public static IServiceProvider[] ServiceProvider()
    {
        return [HostInstance.Value.Services];
    }

    private static IHost CreateHost()
    {
        var builder = Host.CreateDefaultBuilder();
        builder.ConfigureServices(services =>
        {
            services.AddSingleton(implementationInstance: Mock.Of<IProgress<TestStep>>());
            services.AddHttpClientFactory();
            services.AddBrowserClientFactory();
            services.AddTestAgent(
                name: "HttpClient", builder: (TestAgent agent) =>
                {
                    agent.UseDnsLookup()
                         .UseIPAddressAccessibilityCheck()
                         .UseHttpClient();
                });
            services.AddTestAgent(
                name: "BrowserClient", builder: (TestAgent agent) =>
                {
                    agent.UseDnsLookup()
                         .UseIPAddressAccessibilityCheck()
                         .UseBrowser();
                });
        });
        builder.ConfigureLogging(logging =>
        {
            logging.AddFilter(typeof(HttpClient).FullName, LogLevel.Warning);
        });

        var host = builder.Build();

        return host;
    }
}
