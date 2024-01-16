[CmdletBinding(DefaultParameterSetName = "tag")]
param(
    # The location where Chocolatey CLI sources are located,
    # or will be located if the directory does not already exist.
    # If not specified and the environment variable `CHOCO_SOURCE_LOCATION`
    # is not definied, the location will default to `$env:TEMP\
    [Alias("ChocoLocation")]
    [string] $ChocoSourceLocation = $env:CHOCO_SOURCE_LOCATION,

    # Checkout a specific tag of your own choosing.
    # This value can also be a specific commit or a branch, but
    # will be notified as being a tag.
    # NOTE: Only tags already pulled down will be considered.
    [Parameter(ParameterSetName = "tag")]
    [string] $CheckoutTag = $null,

    # Checkout the latest tag available in the Chocolatey Source Location.
    # NOTE: Only tags already pulled down will be considered.
    [Parameter(ParameterSetName = "latest")]
    [switch] $CheckoutLatestTag,

    # Try check out a tage with the same name as what is used as a reference
    # in the packages.config file. If the reference specified is not a stable
    # version, the latest tag will be checked out instead.
    # NOTE: Only tags already pulled down will be considered.
    [Parameter(ParameterSetName = 'ref-tag')]
    [switch] $CheckoutRefTag,

    # Remove and clone the specified Chocolatey Source Location again.
    # This is a very destructive operation, and should only be used if
    # you are not interested in any local information.
    [switch] $ForceChocoClone,

    # Only copy artifacts from the specified Chocolatey Source location,
    # without building the project again.
    # This can be useful if it is already built and you need to refresh the
    # artifacts.
    [switch] $NoBuild
)

$ErrorActionPreference = 'Stop'

function CheckoutTag {
    param(
        [Parameter(Mandatory)]
        [string] $SourceLocation,

        [string] $TagName
    )

    Push-Location "$SourceLocation"
    if (!$TagName) {
        $TagName = . git tag --sort v:refname | Where-Object { $_ -match "^[\d\.]+$" } | Select-Object -last 1
    }

    if ($TagName) {
        git checkout $TagName -q 2>$null
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Checked out Chocolatey CLI tag '$TagName'"
        }
        else {
            $currentBranch = . git branch --show-current
            if ($currentBranch) {
                Write-Warning "Unable to check out tag $TagName. Leaving source in branch $currentBranch"
            }
            else {
                Write-Warning "Unable to check out tag $TagName. Leaving source in commit $(git rev-parse HEAD)"
            }
        }
    }

    Pop-Location
}

[xml]$packagesConfigFile = Get-Content -Path "$PSScriptRoot/src/Chocolatey.Community.Validation\Chocolatey.Community.Validation.csproj"

$chocolateyLibPackageVersion = $($packagesConfigFile.Project.ItemGroup.PackageReference | Where-Object { $_.Include -eq "chocolatey.lib" }).Version

if (!$chocolateyLibPackageVersion) {
    throw "Chocolatey CLI referenced version was not found!"
}

if ($CheckoutRefTag) {

    if ($chocolateyLibPackageVersion -match '^[\d\.]+$') {
        $CheckoutTag = $chocolateyLibPackageVersion
    }
    else {
        $CheckoutLatestTag = $true
    }
}

if (!$ChocoSourceLocation) {
    # To allow a default path being used for cloning the repository
    $ChocoSourceLocation = "$env:TEMP\chocoSource"
}

Write-Host "We are at $PSScriptRoot"
Write-Host "We are looking for choco in '$ChocoSourceLocation'"

if ($ForceChocoClone -and (Test-Path $ChocoSourceLocation)) {
    Write-Host "Removing existing Chocolatey CLI Source in '$ChocoSourceLocation'"
    # We use error action stop here, as there may be times the `.git` directory is locked.
    # Having information about this is helpful to rectify the issue.
    Remove-Item $ChocoSourceLocation -Recurse -Force -EA Stop
}

if (!(Test-Path $ChocoSourceLocation)) {
    Write-Host "Cloning Chocolatey CLI Repository to '$ChocoSourceLocation'"
    git clone "https://github.com/chocolatey/choco.git" "$ChocoSourceLocation"

    if ($CheckoutLatestTag) {
        CheckoutTag $ChocoSourceLocation
    }
    elseif ($CheckoutTag) {
        CheckoutTag $ChocoSourceLocation -TagName $CheckoutTag
    }
}
elseif ($CheckoutLatestTag) {
    CheckoutTag $ChocoSourceLocation
}
elseif ($CheckoutTag) {
    CheckoutTag $ChocoSourceLocation -TagName $CheckoutTag
}

if (-not (Test-Path -Path $ChocoSourceLocation)) {
    # We leave this here on purpose in case the cloning of the repository has failed.
    throw "Location '$ChocoSourceLocation' not found; please rerun with the -ChocoSourceLocation parameter or set the CHOCO_SOURCE_LOCATION environment variable."
}

Write-Host "Restore packages on project first..."
if ($IsLinux -or $IsMacOS) {
    & ./build.sh --target='Restore'
} else {
    & ./build.ps1 --target='Restore'
}

$chocolateyLibPackageFolder = "$PSScriptRoot/src/packages/chocolatey.lib\$chocolateyLibPackageVersion\lib\net48"

if (!(Test-Path "$chocolateyLibPackageFolder")) {
    throw "A restored package folder for Chocolatey CLI could not be found. Ensure the correct version is referenced in the csproj file, and it uses the nuget.config configuration."
}

if (!$NoBuild) {
    Write-Host "Building choco at $ChocoSourceLocation with Debug..."

    Push-Location $ChocoSourceLocation
    if (Test-Path "recipe.cake") {
        & ./build.debug.bat --target='Run-ILMerge' --shouldRunAnalyze=false --testExecutionType=none
    }
    else {
        & ./build.debug.bat
    }
    Pop-Location
} else {
    Write-Warning "Skipping building of Chocolatey CLI artifacts."
}

Write-Host "Copying chocolatey artifacts to current Chocolatey Package Version folder..."

$codeDropLibs = "$ChocoSourceLocation/code_drop/temp/_PublishedLibs/chocolatey_merged"

if (!(Test-Path $codeDropLibs)) {
    $codeDropLibs = "$ChocoSourceLocation/code_drop/chocolatey/lib"
}

Write-Host "Copying chocolatey lib items from '$codeDropLibs/*' to '$chocolateyLibPackageFolder'."
Copy-Item -Path "$codeDropLibs/*" -Destination "$chocolateyLibPackageFolder/" -Force

$libFolder = "$PSScriptRoot/lib/chocolatey"
if (-not (Test-Path $libFolder)) {
    New-Item -ItemType Directory -Path $libFolder > $null
}

$codeDropApps = "$ChocoSourceLocation/code_drop/temp/_PublishedApps/choco_merged"

if (!(Test-Path $codeDropApps)) {
    $codeDropApps = "$ChocoSourceLocation/code_drop/chocolatey/console"
}

Write-Host "Copying choco.exe items from '$codeDropApps/*' to '$libFolder'."
Copy-Item "$codeDropApps/*" "$libFolder/" -Force
