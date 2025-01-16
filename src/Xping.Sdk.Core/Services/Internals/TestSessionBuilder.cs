/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using Xping.Sdk.Core.Models;
using Xping.Sdk.Core.Session;
using Xping.Sdk.Shared;

namespace Xping.Sdk.Core.Services.Internals;

internal class TestSessionBuilder : ITestSessionBuilder
{
    private readonly List<TestStep> _steps = [];
    private Uri _url = null!;
    private DateTime _startDate;
    private Error? _error;
    private Guid _uploadToken;

    public ITestSessionBuilder BuildWith(Uri url)
    {
        _url = url.RequireNotNull(nameof(url));
        return this;
    }

    public ITestSessionBuilder BuildWith(DateTime startDate)
    {
        _startDate = startDate;
        return this;
    }

    public ITestSessionBuilder BuildWith(Error error)
    {
        _error = error;
        return this;
    }

    public ITestSessionBuilder BuildWith(TestStep testStep)
    {
        _steps.Add(testStep);
        return this;
    }

    public ITestSessionBuilder BuildWith(Guid uploadToken)
    {
        _uploadToken = uploadToken;
        return this;
    }

    TestSession ITestSessionBuilder.Build()
    {
        return new TestSession
        {
            Url = _url,
            StartDate = _startDate,
            Steps = _steps.AsReadOnly(),
            UploadToken = _uploadToken,
            State = _error == null ? TestSessionState.Completed : TestSessionState.Declined,
            DeclineReason = _error?.Message
        };
    }
}
