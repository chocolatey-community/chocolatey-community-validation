# Chocolatey CCR Extension

This repository deals with how the implementation of validating rules valid for the Chocolatey Community Repository are implemented.
This project is meant to be published as a Chocolatey CLI extension that users can install to validate their packages does not violate
any of the rules required to be passing when pushing packages to Chocolatey Community Repository. Additionally, it is intended that the
Package Validator will eventually reference an internal only NuGet package of this project to prevent the need to duplicate rules in both
places.

## Requirements

To build this project, the following requirements must be met.

1. NET 4.8 installed
2. Visual Studio 2022 (2019 and 2017 may work, but is untested)
3. Ability to Compile Debug version of Chocolatey CLI

### Building and Debugging

Before being able to built the project through Visual Studio and to debug the project, it is required to run the script `Get-ChocoDebugLibraries.ps1` first.
Running this script will take care of the necessary pre-requisite steps to be able to build and debug the project.
The steps to follow in this case is:

There are two options to be able to get the debug version of Chocolatey CLI.

1. The first option is to clone the Chocolatey CLI project to a specific location and reuse these location.
  a. Clone the [`chocolatey/choco`](https://github.com/chocolatey/choco) repository to your preferred location.
  b. Run `.\Get-ChocoDebugLibraries.ps1 -ChocoSourceLocation <PATH_TO_SOURCE_ROOT>` (`ChocoSourceLocation` may be defined as the environment variable `CHOCO_SOURCE_LOCATION`)
2. The second option is to let the debug script clone the necessary repository, and check out the referenced tag. (**NOTE: It is very important you do not have the environment variable `CHOCO_SOURCE_LOCATION` set in this case**).
  a. Run `.\Get-ChocoDebugLibraries.ps1 -CheckoutRefTag` (Alternatively you can check out a specific tag or branch using `.\Get-ChocoDebugLibraries.ps1 -CheckoutTag 2.0.0`).

### Known Issues

- During debugging there may be no information about the variables being used, this is due to referencing `chocolatey.dll` instead of `choco.exe`. It is uncertain of how to fix this problem at this time.
- When implementing rules, it is required to use the full global namespace of `NuspecReader`. This is due to us not directly referencing any of the NuGet.Client libraries ourself.
