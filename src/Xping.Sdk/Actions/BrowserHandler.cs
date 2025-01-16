/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Xping.Sdk.Actions.Internals;
using Xping.Sdk.Core.BrowserManagement;
using Xping.Sdk.Core.Components;
using Xping.Sdk.Core.Configuration;
using Xping.Sdk.Core.Extensions;
using Xping.Sdk.Core.HttpClients;
using Xping.Sdk.Core.Models;
using Xping.Sdk.Core.Session;
using Xping.Sdk.Shared;

namespace Xping.Sdk.Actions;

/// <summary>
/// The BrowserHandler class is used to send HTTP requests to a web application using a headless browserClient, 
/// such as Chromium, Firefox, or WebKit. It uses the Playwright library to create and control the headless 
/// browserClient instance. 
/// </summary>
/// <remarks>
/// Before using this test component, you need to register the necessary services by calling the 
/// <see cref="DependencyInjectionExtension.AddBrowserClientFactory(IServiceCollection)"/> 
/// method which adds <see cref="IBrowserClientFactory"/> factory service. The Xping SDK provides a default implementation 
/// of this interface, called BrowserClientFactory, which based on the <see cref="BrowserOptions"/> creates a 
/// Chromium, WebKit or Firefox headless browserClient instance. You can also implement your own custom headless 
/// browserClient factory by implementing the <see cref="IBrowserClientFactory"/> interface and adding its implementation into
/// services.
/// </remarks>
public sealed class BrowserHandler : TestComponent
{
    private readonly BrowserManager _browserManager;
    private readonly BrowserOptions _browserOptions;

    /// <summary>
    /// Initializes a new instance of the BrowserHandler class with the specified browser options.
    /// </summary>
    public BrowserHandler(
        BrowserManager browserManager, 
        BrowserOptions options) :
        base(nameof(BrowserHandler), TestStepType.ActionStep)
    {
        _browserManager = browserManager.RequireNotNull(nameof(browserManager));
        _browserOptions = options.RequireNotNull(nameof(options));
    }

    /// <summary>
    /// This method performs the test step operation asynchronously.
    /// </summary>
    /// <param name="context">A <see cref="TestContext"/> object that represents the test session.</param>
    /// <param name="cancellationToken">
    /// An optional CancellationToken object that can be used to cancel this operation.
    /// </param>
    /// <returns><see cref="TestStep"/> object.</returns>
    public override async Task<TestStepResult> HandleAsync(
        TestContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));        
        
        // Initialize URL tracker
        OrderedUrlTracker urlTracker = [context.Url.AbsoluteUri];

        PropertyBag bag = new();
        bag.Add(new("BrowserManager.LaunchOptions.Headless"), $"{_browserManager.LaunchOptions.Headless}");


        TestStepBuilder builder = new TestStepBuilder()
            .WithPropertyBag(bag);

        // Store Browser information 

        var client = new BrowserClient(
            _browserManager, 
            HttpRequestInterceptor,
            HttpResponseHandler(context, urlTracker));

        


        //IBrowserClientFactory? browserFactory =
        //    context.ServiceProvider.GetService<IBrowserClientFactory>() ??
        //throw new InvalidProgramException(Errors.HeadlessBrowserNotFound);

        //BrowserClient browserClient = await browserFactory
        //    .CreateClientAsync(_browserOptions, settings)
        //    .ConfigureAwait(false);

        //TestStep testStep = null!;
        try
        {
            //urlTracker.Add(context.Url.AbsoluteUri);

            IResponse? response = await client.SendAsync(context.Url);

            byte[] buffer = await response.BodyAsync();

            HttpResponseMessage Response() => responseMessage.HttpResponseMessage;
            testStep = context.SessionBuilder
                .Build(PropertyBagKeys.HttpStatus, new PropertyBagValue<string>($"{(int)Response().StatusCode}"))
                .Build(PropertyBagKeys.HttpVersion, new PropertyBagValue<string>($"{Response().Version}"))
                .Build(PropertyBagKeys.HttpReasonPhrase, new PropertyBagValue<string?>(Response().ReasonPhrase))
                .Build(PropertyBagKeys.HttpResponseHeaders, GetHeaders(Response().Headers))
                .Build(PropertyBagKeys.HttpResponseTrailingHeaders, GetHeaders(Response().TrailingHeaders))
                .Build(PropertyBagKeys.HttpContentHeaders, GetHeaders(Response().Content.Headers))
                .Build(PropertyBagKeys.HttpContent, new PropertyBagValue<byte[]>(buffer))
                .Build(PropertyBagKeys.BrowserResponseMessage,
                    new NonSerializable<BrowserResponseMessage>(responseMessage))
                .Build(PropertyBagKeys.HttpResponseMessage, 
                    new NonSerializable<HttpResponseMessage>(responseMessage.HttpResponseMessage))
                .Build();
        }
        catch (Exception exception)
        {
            testStep = context.SessionBuilder.Build(exception);
        }
        finally
        {
            context.Progress?.Report(testStep);
        }
    }

    private HttpRedirectResponseHandler HttpResponseHandler(
        TestContext context,
        OrderedUrlTracker urlTracker) => new(context, urlTracker, _browserOptions.MaxRedirections);

    private BrowserHttpRequestInterceptor HttpRequestInterceptor => new(_browserOptions);

    private static PropertyBagValue<Dictionary<string, string>> GetHeaders(HttpHeaders headers) =>
        new(headers.ToDictionary(h => h.Key.ToUpperInvariant(), h => string.Join(";", h.Value)));
}
