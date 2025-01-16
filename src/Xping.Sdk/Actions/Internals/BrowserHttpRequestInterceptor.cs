/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using Microsoft.Playwright;
using Xping.Sdk.Core.BrowserManagement;
using Xping.Sdk.Core.Configuration;

namespace Xping.Sdk.Actions.Internals;

internal class BrowserHttpRequestInterceptor(BrowserOptions options) : IHttpRequestInterceptor
{
    public async Task HandleAsync(IRoute route)
    {
        HttpContent? httpContent = options.GetHttpContent();
        byte[]? byteArray = httpContent != null ? await httpContent.ReadAsByteArrayAsync() : null;

        // Modify the HTTP method here
        var routeOptions = new RouteContinueOptions
        {
            Method = options.GetHttpMethod().Method,
            PostData = byteArray
        };

        await route.ContinueAsync(routeOptions).ConfigureAwait(false);
    }
}
