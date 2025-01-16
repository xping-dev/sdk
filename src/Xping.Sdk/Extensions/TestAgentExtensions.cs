/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Xping.Sdk.Actions;
using Xping.Sdk.Actions.Configurations;
using Xping.Sdk.Core;
using Xping.Sdk.Core.BrowserManagement;
using Xping.Sdk.Core.Configuration;
using Xping.Sdk.Core.Extensions;
using Xping.Sdk.Shared;
using Xping.Sdk.Validations.Content.Html;
using Xping.Sdk.Validations.Content.Page;
using Xping.Sdk.Validations.HttpResponse;
using HttpClientHandler = Xping.Sdk.Actions.HttpClientHandler;

namespace Xping.Sdk.Extensions;

/// <summary>
/// Provides extension methods for the TestAgent class to enhance its functionality.
/// </summary>
public static class TestAgentExtensions
{
    /// <summary>
    /// Configures the TestAgent to perform DNS lookups.
    /// </summary>
    /// <param name="testAgent">The instance of TestAgent to configure.</param>
    public static TestAgent UseDnsLookup(this TestAgent testAgent)
    {
        return testAgent.RequireNotNull(nameof(testAgent)).Update(new DnsLookup());
    }

    /// <summary>
    /// Configures the TestAgent to check IP address accessibility using ping.
    /// </summary>
    /// <param name="testAgent">The instance of TestAgent to configure.</param>
    /// <param name="options">Optional configuration settings for the ping operation.</param>
    public static TestAgent UseIPAddressAccessibilityCheck(
        this TestAgent testAgent, Action<PingConfiguration>? options = null)
    {
        var configuration = new PingConfiguration();
        options?.Invoke(configuration);

        return testAgent.RequireNotNull(nameof(testAgent)).Update(new IPAddressAccessibilityCheck(configuration));
    }

    /// <summary>
    /// Configures the TestAgent to send requests using an HttpClient.
    /// </summary>
    /// <param name="testAgent">The instance of TestAgent to configure.</param>
    /// <param name="options">Optional configuration settings for the HttpClient.</param>
    public static TestAgent UseHttpClient(this TestAgent testAgent, Action<HttpClientOptions>? options = null)
    {
        ArgumentNullException.ThrowIfNull(testAgent, nameof(testAgent));

        var configuration = new HttpClientOptions();
        options?.Invoke(configuration);

        return testAgent.Update(new HttpClientHandler(configuration));
    }

    /// <summary>
    /// Configures the TestAgent to simulate browser requests.
    /// </summary>
    /// <param name="testAgent">The instance of TestAgent to configure.</param>
    /// <param name="browserManager"></param>
    /// <param name="configureOptions">Optional configuration settings for the browser simulation.</param>
    public static TestAgent UseBrowser(
        this TestAgent testAgent, 
        BrowserManager? browserManager = null,
        Action<BrowserOptions>? configureOptions = null)
    {
        ArgumentNullException.ThrowIfNull(testAgent, nameof(testAgent));

        var options = new BrowserOptions();
        configureOptions?.Invoke(options);
        browserManager ??= testAgent.ServiceProvider.GetRequiredService<BrowserManager>();

        return testAgent.Update(new BrowserHandler(browserManager, options));
    }

    /// <summary>
    /// Configures the TestAgent with a http response validator.
    /// </summary>
    /// <param name="testAgent">The instance of TestAgent to configure.</param>
    /// <param name="response">The validation logic to use on the HTTP response.</param>
    /// <returns></returns>
    public static TestAgent UseHttpValidation(this TestAgent testAgent, Action<IHttpResponse> response)
    {
        return testAgent.RequireNotNull(nameof(testAgent)).Add(new HttpResponseValidator(response));
    }

    /// <summary>
    /// Configures the TestAgent with a page content validator.
    /// </summary>
    /// <param name="testAgent">The instance of TestAgent to configure.</param>
    /// <param name="page">The validation logic to use on the Page object.</param>
    public static TestAgent UsePageValidation(this TestAgent testAgent, Func<IPage, Task> page)
    {
        return testAgent.RequireNotNull(nameof(testAgent)).Add(new PageContentValidator(page));
    }

    /// <summary>
    /// Configures the TestAgent with a html content validator.
    /// </summary>
    /// <param name="testAgent">The instance of TestAgent to configure.</param>
    /// <param name="htmlContent">The validation logic to use on the Html content.</param>
    public static TestAgent UseHtmlValidation(this TestAgent testAgent, Action<IHtmlContent> htmlContent)
    {
        return testAgent.RequireNotNull(nameof(testAgent)).Add(new HtmlContentValidator(htmlContent));
    }
}
