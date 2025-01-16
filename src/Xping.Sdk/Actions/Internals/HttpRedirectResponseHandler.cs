/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using System.Net;
using Microsoft.Net.Http.Headers;
using Microsoft.Playwright;
using Xping.Sdk.Core.BrowserManagement;
using Xping.Sdk.Core.HttpClients;
using Xping.Sdk.Core.Models;
using Xping.Sdk.Core.Session;

namespace Xping.Sdk.Actions.Internals;

/// <summary>
/// This handler is designed to be used with the Playwright library and various browsers to handle only HTTP redirect 
/// responses. It can detect circular dependencies in redirects.
/// </summary>
internal class HttpRedirectResponseHandler(OrderedUrlTracker urlTracker, int maxRedirections) : IHttpResponseHandler
{
    public event EventHandler<IResponse>? RedirectResponseReceived;
    public event EventHandler<Uri>? CircularDependencyDetected;
    public event EventHandler? MaxRedirectionsReached;

    public bool IsMaxRedirectionsReached => urlTracker.Count > maxRedirections;

    public void HandleResponse(object? sender, IResponse response)
    {
        if (response != null && response.Status >= 300 && response.Status <= 399)
        {
            RedirectResponseReceived?.Invoke(sender: this, response);

            //var httpHeadersBag = CreateHttpResponseHeadersPropertyBag(response);
            //var testStep = CreateTestStep(response, httpHeadersBag);

            //// Notify context's progress mechanism about the new test step created.
            //context.Progress?.Report(testStep);

            // Try get HTTP location header to indicate the URL where a client should be redirected.
            string? redirectUrl = TryGetHttpLocationHeader(httpHeadersBag);

            if (IsCircularDependencyDetected(redirectUrl))
            {
                CircularDependencyDetected?.Invoke(sender: this, new Uri(redirectUrl));
                //// Circular dependency detected
                //throw new InvalidOperationException(
                //    $"A circular dependency was detected for the URL {redirectUrl}. " +
                //    $"The redirection chain is: {urlTracker.First()} -> {string.Join(" -> ", urlTracker)}.");
            }
            else
            {
                urlTracker.Add(redirectUrl);
            }

            if (IsMaxRedirectionsReached)
            {
                MaxRedirectionsReached?.Invoke(sender: this, EventArgs.Empty);
                //// Throw an exception if the max number of redirects has been reached.
                //throw new TooManyRedirectsException(
                //    $"The maximum number of redirects ({maxRedirections}) has been exceeded for the URL " +
                //    $"{urlTracker.First()}. The last redirect URL was " +
                //    $"{urlTracker.FindLastMatchingItem(str => !string.IsNullOrEmpty(str))}.");
            }

            // Restart the instrumentation log after this test step to ensure accurate timing for subsequent steps.
            //_context.Instrumentation.Restart();
        }
    }

    //private static PropertyBagValue<Dictionary<string, string>> CreateHttpResponseHeadersPropertyBag(IResponse response)
    //{
    //    var headers = new PropertyBagValue<Dictionary<string, string>>(
    //        response.Headers.ToDictionary(
    //            pair => pair.Key.ToUpperInvariant(),
    //            pair => pair.Value));

    //    return headers;
    //}

    //private TestStep CreateTestStep(IResponse response, PropertyBagValue<Dictionary<string, string>> httpHeadersBag)
    //{
    //    var httpStatucCode = Enum.Parse<HttpStatusCode>($"{response.Status}", ignoreCase: true);

    //    var testStep = context.SessionBuilder
    //        .Build(PropertyBagKeys.HttpStatus, new PropertyBagValue<string>($"{(int)httpStatucCode}"))
    //        .Build(PropertyBagKeys.HttpReasonPhrase, new PropertyBagValue<string?>(response.StatusText))
    //        .Build(PropertyBagKeys.HttpResponseHeaders, httpHeadersBag)
    //        .Build();

    //    return testStep;
    //}

    private static string? TryGetHttpLocationHeader(PropertyBagValue<Dictionary<string, string>> httpHeadersBag)
    {
        httpHeadersBag.Value.TryGetValue(HeaderNames.Location.ToUpperInvariant(), out string? redirectUrl);

        return redirectUrl;
    }

    private bool IsCircularDependencyDetected(string? redirectUrl) => urlTracker.Contains(redirectUrl);
}
