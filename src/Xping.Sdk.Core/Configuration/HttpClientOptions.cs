/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Net.Http.Headers;
using Xping.Sdk.Core.Models;
using Cookie = System.Net.Cookie;

namespace Xping.Sdk.Core.Configuration;

/// <summary>
/// Represents the configuration for HTTP clients used in the Xping SDK library.
/// </summary>
public class HttpClientOptions
{
    private static readonly string[] TextPlainContentType = ["text/plain"];
    private static readonly string[] ApplicationJsonContentType = ["application/json"];
    private static readonly string[] FormUrlEncodedContentType = ["application/x-www-form-urlencoded"];
    private static readonly string[] MultipartContentType = ["multipart/form-data"];
    private static readonly string[] ByteArrayContentType = ["application/octet-stream"];
    private static readonly string[] StreamContentType = ["application/octet-stream"];

    /// <summary>
    /// Default Http request timeout in seconds. 
    /// </summary>
    public const int DefaultHttpRequestTimeoutInSeconds = 30;

    /// <summary>
    /// 
    /// </summary>
    public PropertyBag PropertyBag { get; } = new();

    /// <summary>
    /// Gets or sets a value that specifies the maximum time to wait for a network request or a browser operation to
    /// finish. If the time exceeds this value, current operation is terminated. See  
    /// <see cref="DefaultHttpRequestTimeoutInSeconds"/> for its default value.
    /// </summary>
    public TimeSpan HttpRequestTimeout { get; set; } = TimeSpan.FromSeconds(DefaultHttpRequestTimeoutInSeconds);

    /// <summary>
    /// Gets or sets a boolean value which determines whether to follow HTTP redirection responses. Default is true.
    /// </summary>
    public bool FollowHttpRedirectionResponses { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of allowed HTTP redirects. Default is 50.
    /// </summary>
    public int MaxRedirections { get; set; } = 50;

    /// <summary>
    /// Stores the specified HTTP request headers for subsequent use in requests made by the http client or headless 
    /// browser. This method ensures that all prepared headers are applied to requests, facilitating consistent 
    /// communication with web servers.
    /// </summary>
    /// <param name="headers">
    /// A dictionary of header names and their corresponding values to be applied to outgoing requests.
    /// </param>
    /// <example>
    /// <code>
    /// var httpRequestHeaders = new Dictionary&lt;string, IEnumerable&lt;string&gt;&gt;
    /// {
    ///     { HeaderNames.UserAgent, ["Chrome/51.0.2704.64 Safari/537.36"] }
    /// };
    /// 
    /// httpClientOptions.SetHttpRequestHeaders(httpRequestHeaders);
    /// </code>
    /// </example>
    public void SetHttpRequestHeaders(IDictionary<string, IEnumerable<string>> headers) =>
        PropertyBag.Add(new("http-headers"), headers);

    /// <summary>
    /// Returns read only HTTP request headers stored in the current configuration instance.
    /// </summary>
    public IReadOnlyDictionary<string, IEnumerable<string>> GetHttpRequestHeaders()
    {
        if (PropertyBag.TryGet<IDictionary<string, IEnumerable<string>>>(new("http-headers"), out var httpHeaders) &&
            httpHeaders is not null)
        {
            return httpHeaders.AsReadOnly();
        }

        return new Dictionary<string, IEnumerable<string>>();
    }

    /// <summary>
    /// Clears the HTTP request headers stored in the current test settings instance.
    /// </summary>
    public void ClearHttpRequestHeaders() => PropertyBag.Properties.Remove(new("http-headers"));

    /// <summary>
    /// Stores the specified HTTP method in the test settings for use in subsequent HTTP requests.
    /// </summary>
    /// <param name="httpMethod">The HTTP method to be used for requests.</param>
    public void SetHttpMethod(HttpMethod httpMethod) => PropertyBag.Add(new("http-method"), httpMethod);

    /// <summary>
    /// Returns HTTP method stored in the current test settings instance.
    /// </summary>
    /// <returns>
    /// HTTP method stored in the current test settings. <see cref="HttpMethod.Get"/> is returned if not specified.
    /// </returns>
    public HttpMethod GetHttpMethod()
    {
        if (PropertyBag.TryGet(new("http-method"), out HttpMethod? httpMethod) && httpMethod is not null)
        {
            return httpMethod;
        }

        return HttpMethod.Get;
    }

    /// <summary>
    /// Stores the specified HttpContent and optionally sets the corresponding content-type header in the current 
    /// configuraiton instance.
    /// </summary>
    /// <param name="httpContent">The HttpContent to store.</param>
    /// <param name="setContentHeaders">
    /// If true, automatically sets the `content-type` header based on the type of HttpContent provided.
    /// </param>
    /// <remarks>
    /// <note>
    /// It is important to note that invoking this method does not alter the HTTP method of the requests; specifically, 
    /// it does not set the HTTP method to POST. If required, the HTTP method should be set independently of the 
    /// HttpContent.
    /// </note>
    /// This content will be used for all subsequent HTTP requests made through the http client and headless browser.
    /// Additionally, if <paramref name="setContentHeaders"/> is set to <c>true</c>, the 'content-type' header will be 
    /// automatically set based on the <paramref name="httpContent"/> provided:
    /// <list type="bullet">
    ///     <item>
    ///         <description><c>StringContent</c>: <c>"text/plain"</c></description>
    ///     </item>
    ///     <item>
    ///         <description><c>JsonContent</c>: <c>"application/json"</c></description>
    ///     </item>
    ///     <item>
    ///         <description><c>FormUrlEncodedContent</c>: <c>"application/x-www-form-urlencoded"</c></description>
    ///     </item>
    ///     <item>
    ///         <description><c>MultipartFormDataContent</c>: <c>"multipart/form-data"</c></description>
    ///     </item>
    ///     <item>
    ///         <description><c>ByteArrayContent</c>: <c>"application/octet-stream"</c></description>
    ///     </item>
    ///     <item>
    ///         <description><c>StreamContent</c>: <c>"application/octet-stream"</c></description>
    ///     </item>
    /// </list>
    /// <exqample>
    /// <code>
    /// HttpContent httpContent = JsonContent.Create("{\"name\":\"John\", \"age\":30, \"car\":null}");
    /// httpClientOptions.SetHttpContent(httpContent);
    /// </code>
    /// </exqample>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Throws when httpContent is null.</exception>
    public void SetHttpContent(HttpContent httpContent, bool setContentHeaders = true)
    {
        ArgumentNullException.ThrowIfNull(httpContent, nameof(httpContent));
        PropertyBag.Add(new("http-content"), httpContent);

        if (!setContentHeaders)
            return;

        var httpHeaders = new Dictionary<string, IEnumerable<string>>(GetHttpRequestHeaders());

        switch (httpContent)
        {
            case StringContent _:
                httpHeaders.TryAdd(HeaderNames.ContentType, TextPlainContentType);
                break;
            case JsonContent _:
                httpHeaders.TryAdd(HeaderNames.ContentType, ApplicationJsonContentType);
                break;
            case FormUrlEncodedContent _:
                httpHeaders.TryAdd(HeaderNames.ContentType, FormUrlEncodedContentType);
                break;
            case MultipartFormDataContent _:
                httpHeaders.TryAdd(HeaderNames.ContentType, MultipartContentType);
                break;
            case ByteArrayContent _:
                httpHeaders.TryAdd(HeaderNames.ContentType, ByteArrayContentType);
                break;
            case StreamContent _:
                httpHeaders[HeaderNames.ContentType] = StreamContentType;
                break;
                // Add other content types as needed
        }

        SetHttpRequestHeaders(httpHeaders);
    }

    /// <summary>
    /// Returns HTTP content stored in the current configuration instance.
    /// </summary>
    public HttpContent? GetHttpContent() =>
        PropertyBag.TryGet(new("http-content"), out HttpContent? httpContent) ? httpContent : null;

    /// <summary>
    /// Clears the HTTP content stored in the current test settings instance.
    /// </summary>
    /// <remarks>
    /// Please note this method does not update the content-type header.
    /// </remarks>
    public void ClearHttpContent()
    {
        PropertyBag.Properties.Remove(new("http-content"));
    }

    /// <summary>
    /// Stores a cookie in the current test settings instance.
    /// </summary>
    /// <param name="cookie">The cookie to be added.</param>
    /// <exception cref="ArgumentNullException">Thrown when cookie is null.</exception>
    public void AddCookie(Cookie cookie)
    {
        ArgumentNullException.ThrowIfNull(cookie, nameof(cookie));

        var httpHeaders = new Dictionary<string, IEnumerable<string>>(GetHttpRequestHeaders());

        // Convert the Cookie object to a string representation
        string cookieString = $"{cookie.Name}={cookie.Value}";

        // Check if the 'Cookie' header already exists
        if (httpHeaders.TryGetValue(HeaderNames.Cookie, out var existingCookies))
        {
            // Append the new cookie to the existing 'Cookie' header
            httpHeaders[HeaderNames.Cookie] = new List<string>(existingCookies) { cookieString };
        }
        else
        {
            // Add a new 'Cookie' header with the new cookie
            httpHeaders[HeaderNames.Cookie] = [cookieString];
        }

        SetHttpRequestHeaders(httpHeaders);
    }

    /// <summary>
    /// Adds a collection of cookies to the current configuration instance. This method iterates through each cookie 
    /// and adds them individually using the AddCookie method.
    /// </summary>
    /// <param name="cookieCollection">The collection of cookies to be added. Must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when cookieCollection is null.</exception>
    public void AddCookies(CookieCollection cookieCollection)
    {
        ArgumentNullException.ThrowIfNull(cookieCollection, nameof(cookieCollection));

        foreach (Cookie cookie in cookieCollection.Cast<Cookie>())
        {
            AddCookie(cookie);
        }
    }

    /// <summary>
    /// Returns a list of cookies stored in the current test settings instance.
    /// </summary>
    /// <returns>A CookieCollection containing all the cookies from the test settings.</returns>
    public CookieCollection GetCookies()
    {
        var httpRequestHeaders = GetHttpRequestHeaders();
        var cookieCollection = new CookieCollection();

        if (httpRequestHeaders.TryGetValue(HeaderNames.Cookie, out var cookies))
        {
            foreach (var cookie in cookies)
            {
                var cookieParts = cookie.Trim().Split('=');

                if (cookieParts.Length == 2)
                {
                    cookieCollection.Add(new Cookie(cookieParts[0].Trim(), cookieParts[1].Trim()));
                }
            }
        }

        return cookieCollection;
    }

    /// <summary>
    /// Clears all the cookies stored in the current configuration instance.
    /// </summary>
    public void ClearCookies()
    {
        var httpRequestHeaders = new Dictionary<string, IEnumerable<string>>(GetHttpRequestHeaders());
        httpRequestHeaders.Remove(HeaderNames.Cookie);
        SetHttpRequestHeaders(httpRequestHeaders);
    }

    /// <summary>
    /// Sets the credentials for HTTP request headers using basic authentication.
    /// <note type="important">
    /// The credentials are encoded in Base64, which does not provide encryption. Base64 is a binary-to-text encoding 
    /// scheme that is easily reversible. Therefore, the credentials are sent in plain text.
    /// </note>
    /// </summary>
    /// <param name="credential">The NetworkCredential object containing the user's credentials.</param>
    public void SetCredentials(NetworkCredential credential)
    {
        ArgumentNullException.ThrowIfNull(credential, nameof(credential));

        var httpRequestHeaders = new Dictionary<string, IEnumerable<string>>(GetHttpRequestHeaders());

        // Set the 'Authorization' header to 'Basic {encoded-credentials}' to authenticate the request.
        string basicAuthenticationValue =
            Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{credential.UserName}:{credential.Password}"));

        httpRequestHeaders[HeaderNames.Authorization] =
            [new AuthenticationHeaderValue("Basic", basicAuthenticationValue).ToString()];

        SetHttpRequestHeaders(httpRequestHeaders);
    }

    /// <summary>
    /// Clears the credentials stored in current test settings instance.
    /// </summary>
    public void ClearCredentials()
    {
        var httpRequestHeaders = new Dictionary<string, IEnumerable<string>>(GetHttpRequestHeaders());
        httpRequestHeaders.Remove(HeaderNames.Authorization);
        SetHttpRequestHeaders(httpRequestHeaders);
    }

    /// <summary>
    /// Gets or sets a boolean value which determines whether to retry HTTP requests when they fail. Default is true.
    /// This property is used only with HttpClient and it is skipped with BrowserClient.
    /// </summary>
    public bool RetryHttpRequestWhenFailed { get; set; } = true;
}
