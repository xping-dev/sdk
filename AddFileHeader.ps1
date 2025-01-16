# Define the file header
$fileHeader = @"
/*
 * © 2025 Xping.io. All Rights Reserved.
 * This file is part of the Xping SDK.
 *
 * License: [MIT]
 */

"@

# Get all C# files in the /src directory and its subdirectories
$files = Get-ChildItem -Path .\samples -Recurse -Filter *.cs

foreach ($file in $files) {
    # Read the content of the file
    $content = Get-Content -Path $file.FullName -Raw

    # Check if the file already contains the header
    if ($content[0] -ne "/*" -and $content[1] -ne " * © 2025 Xping.io. All Rights Reserved.") {
        # Add the header to the file
        $newContent = $fileHeader + "`r`n" + $content
        Set-Content -Path $file.FullName -Value $newContent -NoNewline
        Write-Host "Added header to $($file.FullName)"
    } else {
        Write-Host "Header already exists in $($file.FullName)"
    }
}
