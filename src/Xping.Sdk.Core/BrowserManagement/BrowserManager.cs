/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using Microsoft.Playwright;
using Xping.Sdk.Core.Configuration;

namespace Xping.Sdk.Core.BrowserManagement;

public class BrowserManager : IDisposable, IAsyncDisposable
{
    private readonly IPlaywright _playwright;
    private readonly IBrowserFactory _browserFactory;
    private readonly IBrowserContextFactory _contextFactory;
    private readonly IPageFactory _pageFactory;

    public IBrowser? Browser { get; private set; }
    public IBrowserContext? Context { get; private set; }
    public IPage? Page { get; private set; }
    public string BrowserType { get; set; } = "chromium";
    public BrowserTypeLaunchOptions LaunchOptions { get; set; } = new BrowserTypeLaunchOptions();
    public BrowserNewContextOptions ContextOptions { get; set; } = new BrowserNewContextOptions();

    public BrowserManager(
        IPlaywright playwright,
        IBrowserFactory browserFactory,
        IBrowserContextFactory contextFactory,
        IPageFactory pageFactory)
    {
        _playwright = playwright;
        _browserFactory = browserFactory;
        _contextFactory = contextFactory;
        _pageFactory = pageFactory;
    }

    public async Task<IPage> InitializeAsync(BrowserOptions browserOptions)
    {
        Browser ??= await _browserFactory.GetBrowserAsync(BrowserType, LaunchOptions);
        Context ??= await _contextFactory.GetBrowserContextAsync(Browser, ContextOptions);
        Page ??= await _pageFactory.GetPageAsync(Context);
        
        return Page;
    }

    public async Task CloseAsync()
    {
        if (Page != null)
        {
            await _pageFactory.ClosePageAsync(Page);
            Page = null;
        }

        if (Context != null)
        {
            await _contextFactory.CloseBrowserContextAsync(Context);
            Context = null;
        }

        if (Browser != null)
        {
            await _browserFactory.CloseBrowserAsync(Browser);
            Browser = null;
        }
    }

    public void Dispose()
    {
        CloseAsync().GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        await CloseAsync();
    }
}
