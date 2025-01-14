/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Xping.Sdk.Core.Common;
using Xping.Sdk.Core.Components;
using Xping.Sdk.Validations.Content.Html.Internals.Selectors;
using Xping.Sdk.Validations.TextUtils;
using Xping.Sdk.Validations.TextUtils.Internals;

namespace Xping.Sdk.Validations.Content.Html.Internals;

internal class HtmlAssertions(IHtmlContent content) : IHtmlAssertions
{
    private readonly TestContext _context = content.Context;
    private readonly HtmlDocument _document = content.Document;

    public IHtmlAssertions ToHaveExternalScript(string externalScript, TextOptions? options = null)
    {
        const string attribute = "src";
        
        _context.SessionBuilder
            .Build(
                new PropertyBagKey(key: "MethodName"),
                new PropertyBagValue<string>(nameof(ToHaveExternalScript)))
            .Build(
                new PropertyBagKey(key: nameof(externalScript)),
                new PropertyBagValue<string>(externalScript))
            .Build(
                new PropertyBagKey(key: nameof(TextOptions)),
                new PropertyBagValue<string>(options?.ToString() ?? "Null"));

        var xpath = XPaths.Script;
        var selector = CreateXPathSelector(xpath);
        var nodes = selector.Select(_document.DocumentNode);
        var textComparer = new TextComparer(options);

        _context.SessionBuilder.Build(
            new PropertyBagKey(key: "Nodes"),
            new PropertyBagValue<string[]>(nodes?.Select(n => n.OriginalName.Trim()).ToArray() ?? ["Null"]));

        if (nodes == null || nodes.Count == 0)
        {
            throw new ValidationException(
                $"No {xpath.Name} node available. Ensure that the html content has {xpath.Name} node before " +
                $"attempting to validate its '{attribute}' attribute. This error occurred during the validation of " +
                $"HTML data.");
        }

        if (!nodes.Any(n => ValidateAttributeText(n, attribute, externalScript, textComparer)))
        {
            throw new ValidationException(
                $"Expected the {xpath.Name} node's `{attribute}` attribute to be \"{externalScript}\", but found " +
                $"none matching this criteria. This error occurred during the validation of HTML data.");
        }

        // Create a successful test step with detailed information about the current state of the HTML locator.
        var testStep = _context.SessionBuilder.Build();
        // Report the progress of this test step.
        _context.Progress?.Report(testStep);

        return this;
    }

    public IHtmlAssertions ToHaveExternalStylesheet(string externalStylesheet, TextOptions? options)
    {
        const string attribute = "href";
        
        _context.SessionBuilder
            .Build(
                new PropertyBagKey(key: "MethodName"),
                new PropertyBagValue<string>(nameof(ToHaveExternalStylesheet)))
            .Build(
                new PropertyBagKey(key: nameof(externalStylesheet)),
                new PropertyBagValue<string>(externalStylesheet))
            .Build(
                new PropertyBagKey(key: nameof(TextOptions)),
                new PropertyBagValue<string>(options?.ToString() ?? "Null"));

        var xpath = XPaths.StyleSheet;
        var selector = CreateXPathSelector(xpath);
        var nodes = selector.Select(_document.DocumentNode);
        var textComparer = new TextComparer(options);

        _context.SessionBuilder.Build(
            new PropertyBagKey(key: "Nodes"),
            new PropertyBagValue<string[]>(nodes?.Select(n => n.OriginalName.Trim()).ToArray() ?? ["Null"]));

        if (nodes == null || nodes.Count == 0)
        {
            throw new ValidationException(
                $"No {xpath.Name} node available. Ensure that the html content has {xpath.Name} node before " +
                $"attempting to validate its '{attribute}' attribute. This error occurred during the validation of " +
                $"HTML data.");
        }

        if (!nodes.Any(n => ValidateAttributeText(n, attribute, externalStylesheet, textComparer)))
        {
            throw new ValidationException(
                $"Expected the {xpath.Name} node's '{attribute}' attribute to be \"{externalStylesheet}\", but found " +
                $"none matching this criteria. This error occurred during the validation of HTML data.");
        }

        // Create a successful test step with detailed information about the current state of the HTML locator.
        var testStep = _context.SessionBuilder.Build();
        // Report the progress of this test step.
        _context.Progress?.Report(testStep);

        return this;
    }

    public IHtmlAssertions ToHaveImageWithAltText(string altText, TextOptions? options = null)
    {
        const string attribute = "alt";
        var normalizedAlt = altText.Trim();

        _context.SessionBuilder
            .Build(
                new PropertyBagKey(key: "MethodName"),
                new PropertyBagValue<string>(nameof(ToHaveImageWithAltText)))
            .Build(
                new PropertyBagKey(key: nameof(altText)),
                new PropertyBagValue<string>(altText))
            .Build(
                new PropertyBagKey(key: nameof(normalizedAlt)),
                new PropertyBagValue<string>(normalizedAlt))
            .Build(
                new PropertyBagKey(key: nameof(options)),
                new PropertyBagValue<string>(options?.ToString() ?? "Null"));

        var xpath = XPaths.Image;
        var selector = CreateXPathSelector(xpath);
        var nodes = selector.Select(_document.DocumentNode);
        var textComparer = new TextComparer(options);

        _context.SessionBuilder.Build(
            new PropertyBagKey(key: "Nodes"),
            new PropertyBagValue<string[]>(nodes?.Select(n => n.OriginalName.Trim()).ToArray() ?? ["Null"]));

        if (nodes == null || nodes.Count == 0)
        {
            throw new ValidationException(
                $"No {xpath.Name} node available. Ensure that the html content has {xpath.Name} node before " +
                $"attempting to validate its '{attribute}' text. This error occurred during the validation of " +
                $"HTML data.");
        }

        if (!nodes.Any(n => ValidateAttributeText(n, attribute, normalizedAlt, textComparer)))
        {
            throw new ValidationException(
                $"Expected the {xpath.Name} node's '{attribute}' attribute to be \"{normalizedAlt}\", but found none " +
                $"matching this criteria. This error occurred during the validation of HTML data.");
        }

        // Create a successful test step with detailed information about the current state of the HTML locator.
        var testStep = _context.SessionBuilder.Build();
        // Report the progress of this test step.
        _context.Progress?.Report(testStep);

        return this;
    }

    public IHtmlAssertions ToHaveImageWithAltText(Regex altText, TextOptions? options = null)
    {
        const string attribute = "alt";

        _context.SessionBuilder
            .Build(
                new PropertyBagKey(key: "MethodName"),
                new PropertyBagValue<string>(nameof(ToHaveImageWithAltText)))
            .Build(
                new PropertyBagKey(key: nameof(altText)),
                new PropertyBagValue<string>(altText.ToString()))
            .Build(
                new PropertyBagKey(key: nameof(options)),
                new PropertyBagValue<string>(options?.ToString() ?? "Null"));

        var xpath = XPaths.Image;
        var selector = CreateXPathSelector(xpath);
        var nodes = selector.Select(_document.DocumentNode);
        var textComparer = new TextComparer(options);

        _context.SessionBuilder.Build(
            new PropertyBagKey(key: "Nodes"),
            new PropertyBagValue<string[]>(nodes?.Select(n => n.OriginalName.Trim()).ToArray() ?? ["Null"]));

        if (nodes == null || nodes.Count == 0)
        {
            throw new ValidationException(
                $"No {xpath.Name} node available. Ensure that the html content has {xpath.Name} node before " +
                $"attempting to validate its '{attribute}' text. This error occurred during the validation of " +
                $"HTML data.");
        }

        if (!nodes.Any(n => ValidateAttributeText(n, attribute, altText, textComparer)))
        {
            throw new ValidationException(
                $"Expected the {xpath.Name} node's '{attribute}' attribute to match \"{altText}\" regex, but found " +
                $"none matching this criteria. This error occurred during the validation of HTML data.");
        }

        // Create a successful test step with detailed information about the current state of the HTML locator.
        var testStep = _context.SessionBuilder.Build();
        // Report the progress of this test step.
        _context.Progress?.Report(testStep);

        return this;
    }

    public IHtmlAssertions ToHaveImageWithSrc(string src, TextOptions? options = null)
    {
        const string attribute = "src";
        var normalizedSrc = src.Trim();

        _context.SessionBuilder
            .Build(
                new PropertyBagKey(key: "MethodName"),
                new PropertyBagValue<string>(nameof(ToHaveImageWithSrc)))
            .Build(
                new PropertyBagKey(key: nameof(src)),
                new PropertyBagValue<string>(src))
            .Build(
                new PropertyBagKey(key: nameof(normalizedSrc)),
                new PropertyBagValue<string>(normalizedSrc));

        var xpath = XPaths.Image;
        var selector = CreateXPathSelector(xpath);
        var nodes = selector.Select(_document.DocumentNode);
        var textComparer = new TextComparer(options);

        _context.SessionBuilder.Build(
            new PropertyBagKey(key: "Nodes"),
            new PropertyBagValue<string[]>(nodes?.Select(n => n.OriginalName.Trim()).ToArray() ?? ["Null"]));

        if (nodes == null || nodes.Count == 0)
        {
            throw new ValidationException(
                $"No {xpath.Name} node available. Ensure that the html content has {xpath.Name} node before " +
                $"attempting to validate its '{attribute}' value. This error occurred during the validation of " +
                $"HTML data.");
        }

        if (!nodes.Any(n => ValidateAttributeText(n, attribute, normalizedSrc, textComparer)))
        {
            throw new ValidationException(
                $"Expected the {xpath.Name} node's '{attribute}' attribute to be \"{normalizedSrc}\", but found none " +
                $"matching this criteria. This error occurred during the validation of HTML data.");
        }

        // Create a successful test step with detailed information about the current state of the HTML locator.
        var testStep = _context.SessionBuilder.Build();
        // Report the progress of this test step.
        _context.Progress?.Report(testStep);

        return this;
    }
    
    public IHtmlAssertions ToHaveImageWithSrc(Regex src, TextOptions? options = null)
    {
        const string attribute = "src";
        
        _context.SessionBuilder
            .Build(
                new PropertyBagKey(key: "MethodName"),
                new PropertyBagValue<string>(nameof(ToHaveImageWithSrc)))
            .Build(
                new PropertyBagKey(key: nameof(src)),
                new PropertyBagValue<string>(src.ToString()));

        var xpath = XPaths.Image;
        var selector = CreateXPathSelector(xpath);
        var nodes = selector.Select(_document.DocumentNode);
        var textComparer = new TextComparer(options);

        _context.SessionBuilder.Build(
            new PropertyBagKey(key: "Nodes"),
            new PropertyBagValue<string[]>(nodes?.Select(n => n.OriginalName.Trim()).ToArray() ?? ["Null"]));

        if (nodes == null || nodes.Count == 0)
        {
            throw new ValidationException(
                $"No {xpath.Name} node available. Ensure that the html content has {xpath.Name} node before " +
                $"attempting to validate its '{attribute}' value. This error occurred during the validation of " +
                $"HTML data.");
        }

        if (!nodes.Any(n => ValidateAttributeText(n, attribute, src, textComparer)))
        {
            throw new ValidationException(
                $"Expected the {xpath.Name} node's '{attribute}' attribute to match \"{src}\" regex, but found none " +
                $"matching this criteria. This error occurred during the validation of HTML data.");
        }

        // Create a successful test step with detailed information about the current state of the HTML locator.
        var testStep = _context.SessionBuilder.Build();
        // Report the progress of this test step.
        _context.Progress?.Report(testStep);

        return this;
    }

    public IHtmlAssertions ToHaveMaxDocumentSize(int maxSizeInBytes)
    {
        _context.SessionBuilder
            .Build(
                new PropertyBagKey(key: "MethodName"),
                new PropertyBagValue<string>(nameof(ToHaveMaxDocumentSize)))
            .Build(
                new PropertyBagKey(key: nameof(maxSizeInBytes)),
                new PropertyBagValue<string>(maxSizeInBytes.ToString(CultureInfo.InvariantCulture)));

        var byteCount = _document.Encoding.GetByteCount(_document.Text);

        _context.SessionBuilder.Build(
            new PropertyBagKey(key: "SizeInBytes"),
            new PropertyBagValue<string>(byteCount.ToString(CultureInfo.InvariantCulture)));

        if (byteCount > maxSizeInBytes)
        {
            throw new ValidationException(
                $"The expected HTML document size should be equal to or less than {maxSizeInBytes} bytes; however, " +
                $"the actual size was {byteCount} bytes. This error occurred during the validation of HTML data.");
        }

        // Create a successful test step with detailed information about the current state of the HTML locator.
        var testStep = _context.SessionBuilder.Build();
        // Report the progress of this test step.
        _context.Progress?.Report(testStep);

        return this;
    }

    public IHtmlAssertions ToHaveMetaTag(HtmlAttribute attribute, int? expectedCount = null)
    {
        _context.SessionBuilder
            .Build(
                new PropertyBagKey(key: "MethodName"),
                new PropertyBagValue<string>(nameof(ToHaveMetaTag)))
            .Build(
                new PropertyBagKey(key: nameof(attribute)),
                new PropertyBagValue<string>($"{attribute}"))
            .Build(
                new PropertyBagKey(key: nameof(expectedCount)),
                new PropertyBagValue<string>($"{expectedCount}"));

        var xpath = XPaths.GetMetaTag(attribute);
        var selector = CreateXPathSelector(xpath);
        var nodes = selector.Select(_document.DocumentNode);

        _context.SessionBuilder.Build(
            new PropertyBagKey(key: "Nodes"),
            new PropertyBagValue<string[]>(nodes?.Select(n => n.OriginalName.Trim()).ToArray() ?? ["Null"]));

        if (nodes == null || nodes.Count == 0)
        {
            throw new ValidationException(
                $"No {xpath.Name} node found. Ensure that the HTML content includes a {xpath.Name} node before " +
                $"attempting to validate its presence. This error occurred during the validation of HTML data.");
        }

        if (expectedCount.HasValue && nodes.Count != expectedCount.Value)
        {
            throw new ValidationException(
                $"Expected {expectedCount.Value} {xpath.Name} node(s), but found {nodes.Count}." +
                $"This error occurred during the validation of HTML data.");
        }

        // Create a successful test step with detailed information about the current state of the HTML locator.
        var testStep = _context.SessionBuilder.Build();
        // Report the progress of this test step.
        _context.Progress?.Report(testStep);

        return this;
    }

    public IHtmlAssertions ToHaveMetaTag(HtmlAttribute attribute, string expectedContent, TextOptions? options = null)
    {
        const string contentAttr = "content";
        var normalizedExpectedContent = expectedContent.Trim();

        _context.SessionBuilder
               .Build(
                   new PropertyBagKey(key: "MethodName"),
                   new PropertyBagValue<string>(nameof(ToHaveMetaTag)))
               .Build(
                   new PropertyBagKey(key: nameof(attribute)),
                   new PropertyBagValue<string>($"{attribute}"))
               .Build(
                   new PropertyBagKey(key: nameof(expectedContent)),
                   new PropertyBagValue<string>(expectedContent))
               .Build(
                    new PropertyBagKey(key: nameof(normalizedExpectedContent)),
                    new PropertyBagValue<string>(normalizedExpectedContent));

        var xpath = XPaths.GetMetaTag(attribute);
        var selector = CreateXPathSelector(xpath);
        var nodes = selector.Select(_document.DocumentNode);
        var textComparer = new TextComparer(options);

        _context.SessionBuilder.Build(
            new PropertyBagKey(key: "Nodes"),
            new PropertyBagValue<string[]>(nodes?.Select(n => n.OriginalName.Trim()).ToArray() ?? ["Null"]));

        if (nodes == null || nodes.Count == 0)
        {
            throw new ValidationException(
                $"No {xpath.Name} node available. Ensure that the html content has {xpath.Name} node before " +
                $"attempting to validate its '{contentAttr}' value. This error occurred during the validation of " +
                $"HTML data.");
        }

        if (!nodes.Any(n => ValidateAttributeText(n, contentAttr, normalizedExpectedContent, textComparer)))
        {
            throw new ValidationException(
                $"Expected the {xpath.Name} node's '{contentAttr}' attribute to be \"{normalizedExpectedContent}\"" +
                $", but found none matching this criteria. This error occurred during the validation of HTML data.");
        }
        
        // Create a successful test step with detailed information about the current state of the HTML locator.
        var testStep = _context.SessionBuilder.Build();
        // Report the progress of this test step.
        _context.Progress?.Report(testStep);

        return this;
    }

    public IHtmlAssertions ToHaveTitle(string title, TextOptions? options = null)
    {
        var normalizedTitle = title.Trim();

        _context.SessionBuilder
            .Build(
                new PropertyBagKey(key: "MethodName"),
                new PropertyBagValue<string>(nameof(ToHaveTitle)))
            .Build(
                new PropertyBagKey(key: nameof(title)),
                new PropertyBagValue<string>(title))
            .Build(
                new PropertyBagKey(key: nameof(normalizedTitle)),
                new PropertyBagValue<string>(normalizedTitle))
            .Build(
                new PropertyBagKey(key: nameof(TextOptions)),
                new PropertyBagValue<string>(options?.ToString() ?? "Null"));

        var xpath = XPaths.Title;
        var selector = CreateXPathSelector(xpath);
        var nodes = selector.Select(_document.DocumentNode);
        var textComparer = new TextComparer(options);

        _context.SessionBuilder.Build(
            new PropertyBagKey(key: "Nodes"),
            new PropertyBagValue<string[]>(nodes?.Select(n => n.OriginalName.Trim()).ToArray() ?? ["Null"]));

        switch (nodes?.Count)
        {
            case null:
            case 0:
                throw new ValidationException(
                    $"No {xpath.Name} node available. Ensure that the html content has {xpath.Name} node before " +
                    $"attempting to validate its value. This error occurred during the validation of HTML data.");
            case > 1:
                throw new ValidationException(
                    $"Multiple {xpath.Name} nodes were found. The method expects a single {xpath.Name} node under " +
                    $"the <head> node. Please ensure that the HTML content contains only one {xpath.Name} node for " +
                    $"proper validation. This error occurred during the validation of HTML data.");
        }

        // The method expects a single <title> node under the <head> node.
        var actualText = nodes.First().InnerText.Trim();

        if (!textComparer.Compare(actualText, normalizedTitle))
        {
            throw new ValidationException(
                $"Expected the {xpath.Name} node's inner text to be \"{normalizedTitle}\", but the actual " +
                $"{xpath.Name} node's inner text was \"{actualText}\". This error occurred during the validation of " +
                $"HTML data.");
        }

        // Create a successful test step with detailed information about the current state of the HTML locator.
        var testStep = _context.SessionBuilder.Build();
        // Report the progress of this test step.
        _context.Progress?.Report(testStep);

        return this;
    }
    
    public IHtmlAssertions ToHaveTitle(Regex title, TextOptions? options = null)
    {
        _context.SessionBuilder
            .Build(
                new PropertyBagKey(key: "MethodName"),
                new PropertyBagValue<string>(nameof(ToHaveTitle)))
            .Build(
                new PropertyBagKey(key: nameof(title)),
                new PropertyBagValue<string>(title.ToString()))
            .Build(
                new PropertyBagKey(key: nameof(TextOptions)),
                new PropertyBagValue<string>(options?.ToString() ?? "Null"));

        var xpath = XPaths.Title;
        var selector = CreateXPathSelector(xpath);
        var nodes = selector.Select(_document.DocumentNode);
        var textComparer = new TextComparer(options);

        _context.SessionBuilder.Build(
            new PropertyBagKey(key: "Nodes"),
            new PropertyBagValue<string[]>(nodes?.Select(n => n.OriginalName.Trim()).ToArray() ?? ["Null"]));

        switch (nodes?.Count)
        {
            case null:
            case 0:
                throw new ValidationException(
                    $"No {xpath.Name} node available. Ensure that the html content has {xpath.Name} node before " +
                    $"attempting to validate its value. This error occurred during the validation of HTML data.");
            case > 1:
                throw new ValidationException(
                    $"Multiple {xpath.Name} nodes were found. The method expects a single {xpath.Name} node under " +
                    $"the <head> node. Please ensure that the HTML content contains only one {xpath.Name} node for " +
                    $"proper validation. This error occurred during the validation of HTML data.");
        }

        // The method expects a single <title> node under the <head> node.
        var actualText = nodes.First().InnerText.Trim();

        if (!textComparer.CompareWithRegex(actualText, title.ToString()))
        {
            throw new ValidationException(
                $"Expected the {xpath.Name} node's inner text to be \"{title}\", but the actual {xpath.Name} node's " +
                $"inner text was \"{actualText}\". This error occurred during the validation of HTML data.");
        }

        // Create a successful test step with detailed information about the current state of the HTML locator.
        var testStep = _context.SessionBuilder.Build();
        // Report the progress of this test step.
        _context.Progress?.Report(testStep);

        return this;
    }

    protected virtual ISelector CreateXPathSelector(XPath xpath)
    {
        return new XPathSelector(xpath.Expression);
    }
    
    private static bool ValidateAttributeText(
        HtmlNode node,
        string attribute,
        string expectedValue,
        TextComparer textComparer)
    {
        var actualValue = node.GetAttributeValue<string>(name: attribute, def: string.Empty);
        return string.IsNullOrEmpty(expectedValue) && string.IsNullOrEmpty(actualValue) || 
               !string.IsNullOrEmpty(actualValue) && textComparer.Compare(actualValue, expectedValue);
    }
    
    private static bool ValidateAttributeText(
        HtmlNode node,
        string attribute,
        Regex expectedValue,
        TextComparer textComparer)
    {
        var actualValue = node.GetAttributeValue<string>(name: attribute, def: string.Empty);
        return !string.IsNullOrEmpty(actualValue) &&
               textComparer.CompareWithRegex(actualValue, regexPattern: expectedValue.ToString());
    }
}
