namespace Chocolatey.CCR.Tests.Rules
{
    using System.Threading.Tasks;
    using Chocolatey.CCR.Rules;
    using NUnit.Framework;

    [Category("Requirements")]
    public class AnyElementRulesTests : RuleTestBase<AnyElementRules>
    {
        [Test]
        public async Task ShouldFlagAllElementsInADefaultTemplate()
        {
            // This test takes the entire default template created by Chocolatey CLI
            // and runs it through this validation rule. Elements normally commented out
            // has been uncommented. This string should maybe be extracted to a resx file,
            // and possibly use https://github.com/ycanardeau/ResXGenerator (MIT) to generate
            // a static code file. This could possibly be used for rule summaries as well.

            const string testContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Read this before creating packages: https://docs.chocolatey.org/en-us/create/create-packages -->
<!-- It is especially important to read the above link to understand additional requirements when publishing packages to the community feed aka dot org (https://community.chocolatey.org/packages). -->

<!-- Test your packages in a test environment: https://github.com/chocolatey/chocolatey-test-environment -->

<!--
This is a nuspec. It mostly adheres to https://docs.nuget.org/create/Nuspec-Reference. Chocolatey uses a special version of NuGet.Core that allows us to do more than was initially possible. As such there are certain things to be aware of:

* the package xmlns schema url may cause issues with nuget.exe
* Any of the following elements can ONLY be used by choco tools - projectSourceUrl, docsUrl, mailingListUrl, bugTrackerUrl, packageSourceUrl, provides, conflicts, replaces
* nuget.exe can still install packages with those elements but they are ignored. Any authoring tools or commands will error on those elements
-->

<!-- You can embed software files directly into packages, as long as you are not bound by distribution rights. -->
<!-- * If you are an organization making private packages, you probably have no issues here -->
<!-- * If you are releasing to the community feed, you need to consider distribution rights. -->
<!-- Do not remove this test for UTF-8: if “Ω” doesn’t appear as greek uppercase omega letter enclosed in quotation marks, you should use an editor that supports UTF-8, not this one. -->
<package xmlns=""http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd"">
  <metadata>
    <!-- == PACKAGE SPECIFIC SECTION == -->
    <!-- This section is about this package, although id and version have ties back to the software -->
    <!-- id is lowercase and if you want a good separator for words, use '-', not '.'. Dots are only acceptable as suffixes for certain types of packages, e.g. .install, .portable, .extension, .template -->
    <!-- If the software is cross-platform, attempt to use the same id as the debian/rpm package(s) if possible. -->
    <id>empty</id>
    <!-- version should MATCH as closely as possible with the underlying software -->
    <!-- Is the version a prerelease of a version? https://docs.nuget.org/create/versioning#creating-prerelease-packages -->
    <!-- Note that unstable versions like 0.0.1 can be considered a released version, but it's possible that one can release a 0.0.1-beta before you release a 0.0.1 version. If the version number is final, that is considered a released version and not a prerelease. -->
    <version>__REPLACE__</version>
    <packageSourceUrl>Where is this Chocolatey package located (think GitHub)? packageSourceUrl is highly recommended for the community feed</packageSourceUrl>
    <!-- owners is a poor name for maintainers of the package. It sticks around by this name for compatibility reasons. It basically means you. -->
    <owners>__REPLACE_YOUR_NAME__</owners>
    <!-- ============================== -->

    <!-- == SOFTWARE SPECIFIC SECTION == -->
    <!-- This section is about the software itself -->
    <title>empty (Install)</title>
    <authors>__REPLACE_AUTHORS_OF_SOFTWARE_COMMA_SEPARATED__</authors>
    <!-- projectUrl is required for the community feed -->
    <projectUrl>https://_Software_Location_REMOVE_OR_FILL_OUT_</projectUrl>
    <!-- There are a number of CDN Services that can be used for hosting the Icon for a package. More information can be found here: https://docs.chocolatey.org/en-us/create/create-packages#package-icon-guidelines -->
    <!-- Here is an example using Githack -->
    <iconUrl>http://rawcdn.githack.com/__REPLACE_YOUR_REPO__/master/icons/empty.png</iconUrl>
    <copyright>Year Software Vendor</copyright>
    <!-- If there is a license Url available, it is required for the community feed -->
    <licenseUrl>Software License Location __REMOVE_OR_FILL_OUT__</licenseUrl>
    <requireLicenseAcceptance>true</requireLicenseAcceptance>
    <projectSourceUrl>Software Source Location - is the software FOSS somewhere? Link to it with this</projectSourceUrl>
    <docsUrl>At what url are the software docs located?</docsUrl>
    <mailingListUrl></mailingListUrl>
    <bugTrackerUrl></bugTrackerUrl>
    <tags>empty SPACE_SEPARATED</tags>
    <summary>__REPLACE__</summary>
    <description>__REPLACE__MarkDown_Okay </description>
    <releaseNotes>__REPLACE_OR_REMOVE__MarkDown_Okay</releaseNotes>
    <!-- =============================== -->

    <!-- Specifying dependencies and version ranges? https://docs.nuget.org/create/versioning#specifying-version-ranges-in-.nuspec-files -->
    <dependencies>
      <dependency id="""" version=""__MINIMUM_VERSION__"" />
      <dependency id="""" version=""[__EXACT_VERSION__]"" />
      <dependency id="""" version=""[_MIN_VERSION_INCLUSIVE, MAX_VERSION_INCLUSIVE]"" />
      <dependency id="""" version=""[_MIN_VERSION_INCLUSIVE, MAX_VERSION_EXCLUSIVE)"" />
      <dependency id="""" />
      <dependency id=""chocolatey-core.extension"" version=""1.1.0"" />
    </dependencies>
    <!-- chocolatey-core.extension - https://community.chocolatey.org/packages/chocolatey-core.extension -->

    <provides>NOT YET IMPLEMENTED</provides>
    <conflicts>NOT YET IMPLEMENTED</conflicts>
    <replaces>NOT YET IMPLEMENTED</replaces>
  </metadata>
  <files>
    <!-- this section controls what actually gets packaged into the Chocolatey package -->
    <file src=""tools\**"" target=""tools"" />
  </files>
</package>
";

            await VerifyNuspec(testContent);
        }

        [Test]
        public async Task ShouldNotFlagFullNuspecWithoutTemplateValues()
        {
            const string testContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Do not remove this test for UTF-8: if “Ω” doesn’t appear as greek uppercase omega letter enclosed in quotation marks, you should use an editor that supports UTF-8, not this one. -->
<package xmlns=""http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd"">
  <metadata>
    <id>blender</id>
    <version>3.6.4</version>
    <title>Blender</title>
    <owners>chocolatey-community, Redsandro</owners>
    <authors>Blender Foundation</authors>
    <licenseUrl>https://www.blender.org/about/license/</licenseUrl>
    <projectUrl>https://www.blender.org</projectUrl>
    <projectSourceUrl>https://git.blender.org/gitweb/</projectSourceUrl>
    <bugTrackerUrl>https://developer.blender.org/maniphest/</bugTrackerUrl>
    <mailingListUrl>https://wiki.blender.org/wiki/Communication/Contact#Mailing_Lists</mailingListUrl>
    <iconUrl>https://cdn.jsdelivr.net/gh/chocolatey-community/chocolatey-packages@edba4a5849ff756e767cba86641bea97ff5721fe/icons/blender.svg</iconUrl>
    <requireLicenseAcceptance>false</requireLicenseAcceptance>
    <!-- Do not touch the description here in the nuspec file. Description is imported during update from the Readme.md file -->
    <description><![CDATA[
Blender is a free and open-source professional-grade 3D computer graphics and video compositing program.

### Realistic Materials

![Realistic Materials](https://i.imgur.com/AywgUj3.png)

### Fluid Simulations

![Fluid Simulations](https://i.imgur.com/ILwViaO.png)

## Features

* Blender is a fully integrated 3D content creation suite, offering a broad range of essential tools:
* Modeling
* Rendering
* Animation
* Video Editing and Compositing
* Texturing
* Rigging
* Simulations
* Game Creation.
* Cross platform, with an OpenGL GUI that is uniform on all major platforms (and customizable with Python scripts).
* High-quality 3D architecture enabling fast and efficient creation work-flow.
* Excellent community support from forums and IRC.
* Small executable size, optionally portable.

### Notes

- If you have multiple installations of the Blender software, the uninstallation of the Chocolatey package will likely fail. Please manually uninstall the Blender software (through 'Add and Remove Programs' or 'Programs and Features') and then run `choco uninstall blender` to remove the package.
- **If the package is out of date please check [Version History](#versionhistory) for the latest submitted version. If you have a question, please ask it in [Chocolatey Community Package Discussions](https://github.com/chocolatey-community/chocolatey-packages/discussions) or raise an issue on the [Chocolatey Community Packages Repository](https://github.com/chocolatey-community/chocolatey-packages/issues) if you have problems with the package. Disqus comments will generally not be responded to.**
]]></description>
    <summary>Blender is 3D creation for everyone, free to use for any purpose.</summary>
    <releaseNotes>https://wiki.blender.org/wiki/Reference/Release_Notes</releaseNotes>
    <copyright>Blender Foundation</copyright>
    <tags>blender 3d rendering foss cross-platform modeling animation admin</tags>
    <dependencies>
      <dependency id=""vcredist2008"" version=""9.0.30729.6161"" />
      <dependency id=""chocolatey-core.extension"" version=""1.3.3"" />
      <!--We could set chocolatey to version 0.10.4, but that version was broken so we use 0.10.5-->
      <dependency id=""chocolatey"" version=""0.10.5"" />
    </dependencies>
    <packageSourceUrl>https://github.com/chocolatey-community/chocolatey-packages/tree/master/automatic/blender</packageSourceUrl>
  </metadata>
  <files>
    <file src=""tools\**"" target=""tools"" />
  </files>
</package>";

            await VerifyEmptyResults(testContent);
        }
    }
}
