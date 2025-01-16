/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

using Xping.Sdk.Core.Utilities.Comparison.Internals;

namespace Xping.Sdk.Core.Utilities.Comparison;

/// <summary>
/// Factory class for creating instances of IDiffPresenter based on the specified format.
/// </summary>
public static class DiffPresenterFactory
{
    /// <summary>
    /// Creates an IDiffPresenter instance according to the provided DiffPresenterFormat.
    /// </summary>
    /// <param name="format">The format of the diff presenter to create.</param>
    /// <returns>An instance of IDiffPresenter.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an invalid format is specified.</exception>
    public static IDiffPresenter Create(DiffPresenterFormat format)
    {
        return format switch
        {
            DiffPresenterFormat.Markdown => new MarkdownDiffPresenter(),
            _ => throw new InvalidOperationException(
                $"Unsupported format specified. Currently, only '{nameof(DiffPresenterFormat.Markdown)}' format is " +
                $"supported. Please use the supported format and try again.")
        };
    }
}
