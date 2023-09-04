#load nuget:?package=Chocolatey.Cake.Recipe&version=0.24.0

///////////////////////////////////////////////////////////////////////////////
// RECIPE SCRIPT
///////////////////////////////////////////////////////////////////////////////

Environment.SetVariableNames();

var copyright = DateTime.Now.Year > 2023
    ? string.Format("Copyright © 2023 - {0} Chocolatey Software, Inc. - All Rights Reserved", DateTime.Now.Year)
    : "Copyright © 2023 Chocolatey Software, Inc. - All Rights Reserved";

Task("Prrepare-Chocolatey-Packages")
    .IsDependeeOf("Create-Chocolatey-Packages")
    .IsDependentOn("Sign-Assemblies")
    .Does(() =>
{
    var extensionDirectory = BuildParameters.Paths.Directories.ChocolateyNuspecDirectory.Combine("extensions");
    var legalDirectory = BuildParameters.Paths.Directories.ChocolateyNuspecDirectory.Combine("legal");

    CleanDirectory(extensionDirectory);
    CleanDirectory(legalDirectory);

    CopyFiles(BuildParameters.Paths.Directories.PublishedLibraries + "/chocolatey-ccr/net48/chocolatey-ccr.*", extensionDirectory);
    // Placeholder until we know which license should be used for this repository
    //CopyFile(BuildParameters.RootDirectoryPath "/LICENSE.md", legalDirectory + "/LICENSE.md");

    var checksum = new StringBuilder();

    using (var algorithm = System.Security.Cryptography.SHA256.Create())
    {
        var hashBytes = algorithm.ComputeHash(System.IO.File.ReadAllBytes(extensionDirectory + "/chocolatey-ccr.dll"));

        foreach (var b in hashBytes)
        {
            checksum.AppendFormat("{0:x2}", b);
        }
    }

    var verificationText = string.Format(@"
VERIFICATION
Verification is intended to assist the Chocolatey moderators and community
in verifying that this package's contents are trustworthy.

The included files in this package is provided by Chocolatey Software Inc, and can not
be downloaded outside of the package.

The included binary library called chocolatey-ccr.dll is expected to have the following checksum associated with it:

- Checksum: {0}
- Checksum Type: {1}

The included 'LICENSE.md' file is obtainable from <INSERT_REPOSITORY_URL>",
        checksum.ToString(),
        "SHA256");

    FileWriteText(legalDirectory + "/VERIFICATION.txt", verificationText);
});

Task("Prepare-NuGet-Packages")
    .IsDependeeOf("Create-NuGet-Packages")
    .IsDependentOn("Sign-Assemblies")
    .Does(() =>
{
    var destinationDirectory = BuildParameters.Paths.Directories.NuGetNuspecDirectory.Combine("lib");
    CleanDirectory(destinationDirectory);
    destinationDirectory = destinationDirectory.Combine("net48");
    EnsureDirectoryExists(destinationDirectory);

    CopyFiles(BuildParameters.Paths.Directories.PublishedLibraries + "/chocolatey-ccr/net48/chocolatey-ccr.*", destinationDirectory);
    // Placeholder until we know which license should be used for this repository
    //CopyFile(BuildParameters.RootDirectoryPath "/LICENSE.md", destinationDirectory + "/LICENSE.md");
});

BuildParameters.SetParameters(
    context: Context,
    buildSystem: BuildSystem,
    sourceDirectoryPath: "./src",
    solutionFilePath: "./src/chocolatey-ccr-extension.sln",
    solutionDirectoryPath: "./src/Chocolatey.CCR",
    title: "Chocolatey CCR Extension",
    repositoryOwner: "chocolatey",
    repositoryName: "chocolatey-ccr-extension",
    productName: "Chocolatey CCR Extension",
    productDescription: "Chocolatey CCR Extension is a extension package implementing validation rules to be used together with Chocolatey Community Repository",
    productCopyright: copyright,
    shouldStrongNameSignDependentAssemblies: false,
    treatWarningsAsErrors: true,
    //getFilesToSign: getFilesToSign,
    //getFilesToObfuscate: getFilesToObfuscate
    preferDotNetGlobalToolUsage: !IsRunningOnWindows(),
    shouldRunNuGet: IsRunningOnWindows(),
    shouldRunOpenCover: false, // We disable open cover as it does not work properly with VS2017 style projects.
    shouldRunInspectCode: false // Current version of inspect code can not run against VS2017 style projects.
);

ToolSettings.SetToolSettings(context: Context);

BuildParameters.PrintParameters(Context);

Build.Run();
