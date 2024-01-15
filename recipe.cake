#load nuget:?package=Chocolatey.Cake.Recipe&version=0.25.0

///////////////////////////////////////////////////////////////////////////////
// RECIPE SCRIPT
///////////////////////////////////////////////////////////////////////////////

Environment.SetVariableNames();

var copyright = DateTime.Now.Year > 2024
    ? string.Format("Copyright © 2024 - {0} Chocolatey Software, Inc. - All Rights Reserved", DateTime.Now.Year)
    : "Copyright © 2024 Chocolatey Software, Inc. - All Rights Reserved";

Task("Prepare-Chocolatey-Packages")
    .IsDependeeOf("Create-Chocolatey-Packages")
    .IsDependentOn("Copy-Nuspec-Folders")
    .IsDependentOn("Sign-Assemblies")
    .Does(() =>
{
    var nuspecDirectory = BuildParameters.Paths.Directories.ChocolateyNuspecDirectory;
    var extensionDirectory = nuspecDirectory.Combine("extensions");
    var legalDirectory = nuspecDirectory.Combine("legal");

    CleanDirectory(extensionDirectory);
    CleanDirectory(legalDirectory);

    CopyFiles(BuildParameters.Paths.Directories.PublishedLibraries + "/chocolatey-community-validation/net48/chocolatey-community-validation.*", extensionDirectory);

    var checksum = new StringBuilder();

    using (var algorithm = System.Security.Cryptography.SHA256.Create())
    {
        var hashBytes = algorithm.ComputeHash(System.IO.File.ReadAllBytes(extensionDirectory + "/chocolatey-community-validation.dll"));

        foreach (var b in hashBytes)
        {
            checksum.AppendFormat("{0:x2}", b);
        }
    }

    var licenseUrl = string.Format(
        "https://github.com/{0}/{1}/blob/{2}/LICENSE.txt",
        BuildParameters.RepositoryOwner,
        BuildParameters.RepositoryName,
        BuildParameters.BuildProvider.Repository.Tag.IsTag ? BuildParameters.BuildProvider.Repository.Tag.Name : BuildParameters.BuildProvider.Repository.Branch
    );

    var verificationText = string.Format(@"
VERIFICATION
Verification is intended to assist the Chocolatey moderators and community
in verifying that this package's contents are trustworthy.

The included files in this package is provided by Chocolatey Software Inc and its Community, and can not
be downloaded outside of the package.

The included binary library called chocolatey-community-validation.dll is expected to have the following checksum associated with it:

- Checksum: {0}
- Checksum Type: {1}

The included 'LICENSE.txt' file is obtainable from <{2}>",
        checksum.ToString(),
        "SHA256",
        licenseUrl);

    FileWriteText(legalDirectory + "/VERIFICATION.txt", verificationText);
    CopyFile(BuildParameters.RootDirectoryPath + "/LICENSE.txt", legalDirectory + "/LICENSE.txt");
    
    ReplaceTextInFiles(nuspecDirectory + "/*.nuspec", "REPLACE_WITH_LICENSE_URL", licenseUrl);
    ReplaceTextInFiles(nuspecDirectory + "/*.nuspec", "REPLACE_WITH_COPYRIGHT", copyright);
});

Task("Prepare-NuGet-Packages")
    .IsDependeeOf("Create-NuGet-Packages")
    .IsDependentOn("Sign-Assemblies")
    .Does(() =>
{
    var nuspecDirectory = BuildParameters.Paths.Directories.NuGetNuspecDirectory;
    var destinationDirectory = nuspecDirectory.Combine("lib");
    CleanDirectory(destinationDirectory);
    destinationDirectory = destinationDirectory.Combine("net48");
    EnsureDirectoryExists(destinationDirectory);

    CopyFiles(BuildParameters.Paths.Directories.PublishedLibraries + "/chocolatey-community-validation/net48/chocolatey-community-validation.*", destinationDirectory);
    CopyFile(BuildParameters.RootDirectoryPath + "/LICENSE.txt", destinationDirectory + "/LICENSE.txt");
    ReplaceTextInFiles(nuspecDirectory + "/*.nuspec", "REPLACE_WITH_COPYRIGHT", copyright);
});

BuildParameters.SetParameters(
    context: Context,
    buildSystem: BuildSystem,
    sourceDirectoryPath: "./src",
    solutionFilePath: "./src/Chocolatey.Community.Validation.sln",
    solutionDirectoryPath: "./src/Chocolatey.Community.Validation",
    title: "Chocolatey Community Validation",
    repositoryOwner: "chocolatey-community",
    repositoryName: "chocolatey-community-validation",
    productName: "Chocolatey Community Validation",
    productDescription: "Chocolatey Community Validation is a extension package implementing validation rules to be used together with Chocolatey Community Repository",
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
