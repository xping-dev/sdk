/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using System.Globalization;
using Microsoft.Playwright;

namespace Xping.Sdk.Core.BrowserManagement.Internals;

internal class BrowserFactory(IPlaywright playwright) : IBrowserFactory
{
    public async Task<IBrowser> GetBrowserAsync(string browserType, BrowserTypeLaunchOptions? launchOptions)
    {
        return browserType.ToLower(CultureInfo.InvariantCulture) switch
        {
            BrowserType.Chromium => await playwright.Chromium.LaunchAsync(launchOptions),
            BrowserType.Firefox => await playwright.Firefox.LaunchAsync(launchOptions),
            BrowserType.Webkit => await playwright.Webkit.LaunchAsync(launchOptions),
            _ => throw new ArgumentException("Unsupported browser type", nameof(browserType))
        };
    }

    public async Task CloseBrowserAsync(IBrowser browser)
    {
        await browser.CloseAsync();
    }
}
