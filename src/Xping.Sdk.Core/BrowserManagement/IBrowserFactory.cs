/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using Microsoft.Playwright;

namespace Xping.Sdk.Core.BrowserManagement;

public interface IBrowserFactory
{
    Task<IBrowser> GetBrowserAsync(string browserType, BrowserTypeLaunchOptions? launchOptions);
    Task CloseBrowserAsync(IBrowser browser);
}
