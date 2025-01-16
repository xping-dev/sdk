/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using Microsoft.Playwright;

namespace Xping.Sdk.Core.BrowserManagement.Internals;

internal class BrowserContextFactory : IBrowserContextFactory
{
    public async Task<IBrowserContext> GetBrowserContextAsync(
        IBrowser browser, BrowserNewContextOptions? options = null)
    {
        return await browser.NewContextAsync(options);
    }

    public async Task CloseBrowserContextAsync(IBrowserContext context)
    {
        await context.CloseAsync();
    }
}
