/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using Microsoft.Playwright;

namespace Xping.Sdk.Core.BrowserManagement;

public interface IBrowserContextFactory
{
    Task<IBrowserContext> GetBrowserContextAsync(IBrowser browser, BrowserNewContextOptions? options = null);
    Task CloseBrowserContextAsync(IBrowserContext context);
}
