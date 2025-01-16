/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using Microsoft.Playwright;
using Xping.Sdk.Core.BrowserManagement;
using Xping.Sdk.Core.Configuration;

namespace Xping.Sdk.Core.HttpClients.Internals;

internal class BrowserClientFactory(IPlaywright playwright) : IBrowserClientFactory
{
    public async Task<BrowserClient> CreateClientAsync(BrowserOptions configuration, TestSettings settings)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        playwright.Selectors.SetTestIdAttribute(settings.TestIdAttribute);

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = configuration.Headless,
        };

        return configuration.BrowserType switch
        {
            BrowserType.Webkit =>
                new BrowserClient(
                    browser: await playwright.Webkit.LaunchAsync(launchOptions).ConfigureAwait(false), configuration),

            BrowserType.Firefox =>
                new BrowserClient(
                    browser: await playwright.Firefox.LaunchAsync(launchOptions).ConfigureAwait(false), configuration),

            _ => new BrowserClient(
                browser: await playwright.Chromium.LaunchAsync(launchOptions).ConfigureAwait(false), configuration),
        };
    }
}
