/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using System.Net;
using System.Net.Http.Headers;
using Xping.Sdk.Core.Session;
using Xping.Sdk.Core.Utilities.Serialization;

namespace Xping.Sdk.Core.Services.Internals;

internal class TestSessionUploader(IHttpClientFactory httpClientFactory) : ITestSessionUploader
{
    public const string HttpClientName = nameof(TestSessionUploader);

    public async Task<HttpStatusCode> UploadAsync(TestSession testSession, CancellationToken cancellationToken = default)
    {
        using var httpClient = httpClientFactory.CreateClient(name: HttpClientName);
        using var form = new MultipartFormDataContent();

        using MemoryStream memoryStream = new();
        TestSessionSerializer serializer = new();
        serializer.Serialize(testSession, memoryStream, SerializationFormat.XML);
        using var fileContent = new StreamContent(memoryStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
        form.Add(fileContent, name: "file", fileName: "test-session.xml");
        var response = await httpClient.PostAsync(
            new Uri("https://localhost:7100/api/upload"), form, cancellationToken).ConfigureAwait(false);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return response.StatusCode;
    }
}
