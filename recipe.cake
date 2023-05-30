#load nuget:?package=Chocolatey.Cake.Recipe&version=0.24.0

///////////////////////////////////////////////////////////////////////////////
// RECIPE SCRIPT
///////////////////////////////////////////////////////////////////////////////

Environment.SetVariableNames();

var copyright = DateTime.Now.Year > 2023
    ? string.Format("Copyright © 2023 - {0} Chocolatey Software, Inc. - All Rights Reserved", DateTime.Now.Year)
    : "Copyright © 2023 Chocolatey Software, Inc. - All Rights Reserved";

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
