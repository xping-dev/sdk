/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using Microsoft.Playwright;

namespace Xping.Sdk.Core.BrowserManagement.Internals;

internal class PageFactory : IPageFactory
{
    public async Task<IPage> GetPageAsync(IBrowserContext context)
    {
        return await context.NewPageAsync();
    }

    public async Task ClosePageAsync(IPage page)
    {
        await page.CloseAsync();
    }
}
