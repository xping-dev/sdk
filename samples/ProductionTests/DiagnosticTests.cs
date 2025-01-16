/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Xping.Sdk;
using Xping.Sdk.Core;
using Xping.Sdk.Core.BrowserManagement;
using Xping.Sdk.Core.Utilities;
using Xping.Sdk.Extensions;
using Cookie = Microsoft.Playwright.Cookie;

namespace ProductionTests;

[TestFixtureSource(typeof(XpingTestFixture))]
public class DiagnosticTests(TestAgent testAgent, IServiceProvider services) : BaseTest(testAgent)
{
    private const int MaxHtmlSizeAllowed = 150 * 1024; // 150 KB in bytes
    private const int MaxResponseTimeMs = 3000; // 3 seconds in milliseconds

    private readonly BrowserManager _browserManager = services.GetRequiredService<BrowserManager>();
  
    [SetUp]
    public void SetUp()
    {
        _browserManager.BrowserType = BrowserType.Chromium;
        _browserManager.LaunchOptions.Headless = true;
        _browserManager.ContextOptions.UserAgent = UserAgent.ChromeDesktop;

        TestAgent.UseBrowser(_browserManager, options =>
        {
            options.AddCookie(new Cookie("name", "value"));
        });
    }

    [TearDown]
    public void TearDown()
    {
        TestAgent.Cleanup();
    }

    [Ignore("Investigate: An error occurred while sending the ping request on CI runner.")]
    [Test]
    public async Task VerifyDnsAndIpAddressAvailability()
    {
        TestAgent
            .UseDnsLookup()
            .UseIPAddressAccessibilityCheck();

        await RunAsync();
    }

    [Test]
    [TestCase("/")] // home page
    [TestCase("/prod.html?idp_=1")] // product page
    public async Task VerifyHttpStatusCode200(string relativeAddress)
    {
        TestAgent.UseHttpValidation(response =>
        {
            Expect(response).ToHaveStatusCode(HttpStatusCode.OK);
        });

        await RunAsync(relativeAddress);
    }
    
    [Test]
    [TestCase("/")] // home page
    [TestCase("/prod.html?idp_=1")] // product page
    public async Task VerifyResponseTimeIsAcceptable(string relativeAddress)
    {
        TestAgent.UseHttpValidation(response =>
        {
            Expect(response)
                .ToHaveResponseTimeLessThan(TimeSpan.FromMilliseconds(MaxResponseTimeMs));
        });

        await RunAsync(relativeAddress);
    }
    
    [Test]
    [TestCase("/")] // home page
    [TestCase("/prod.html?idp_=1")] // product page
    public async Task VerifyMaxHtmlSizeAllowed(string relativeAddress)
    {
        TestAgent.UseHtmlValidation(html =>
        {
            Expect(html).ToHaveMaxDocumentSize(MaxHtmlSizeAllowed);
        });

        await RunAsync(relativeAddress);
    }
}