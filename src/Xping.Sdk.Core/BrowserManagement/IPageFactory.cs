/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using Microsoft.Playwright;

namespace Xping.Sdk.Core.BrowserManagement;

public interface IPageFactory
{
    Task<IPage> GetPageAsync(IBrowserContext context);
    Task ClosePageAsync(IPage page);
}
