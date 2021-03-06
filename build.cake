///////////////////////////////////////
// TOOLS
///////////////////////////////////////
#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0

///////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////
// PATHS
///////////////////////////////////////
var assemblyInfoPath = File("./src/DiabloSpeech/Properties/AssemblyInfo.cs");
var solutionPath = File("./src/DiabloSpeech.sln");
var coreBuildDirectory = Directory("./src/DiabloSpeech.Core/bin") + Directory(configuration);
var buildDirectory = Directory("./src/DiabloSpeech/bin") + Directory(configuration);
var testPath = Directory("./tests/bin") + Directory(configuration) + File("tests.dll");
var stagingDirectory = Directory("./staging");
var packageBuildsDirectory = Directory("./builds");
var dependencies = "./dependencies/**/*.sln";
var dataDirectory = Directory("./data");

///////////////////////////////////////
// TASKS
///////////////////////////////////////
Task("Default")
    .IsDependentOn("Package-Files");

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDirectory);
    CleanDirectory(stagingDirectory);
    CleanDirectory(coreBuildDirectory);
    EnsureDirectoryExists(packageBuildsDirectory);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(solutionPath);
    NuGetRestore(GetFiles(dependencies));
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if (IsRunningOnWindows())
    {
        MSBuild(solutionPath, settings =>
            settings.SetConfiguration(configuration));
    }
    else
    {
        XBuild(solutionPath, settings =>
            settings.SetConfiguration(configuration));
    }
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() => NUnit3(testPath, new NUnit3Settings { NoResults = true }));

Task("Copy-Files")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    var path = buildDirectory.ToString();
    var files = GetFiles(path + "/*.exe")
              + GetFiles(path + "/*.dll")
              + GetFiles(path + "/*.ico");
    CopyFiles(files, stagingDirectory);

    // Copy data directory files.
    files = GetFiles(dataDirectory.ToString() + "/**/*");
    CopyFiles(files, stagingDirectory);
});

Task("Package-Files")
    .IsDependentOn("Copy-Files")
    .Does(() =>
{
    var assemblyInfo = ParseAssemblyInfo(assemblyInfoPath);
    var packageName = "DiabloSpeech_" + assemblyInfo.AssemblyInformationalVersion + ".zip";
    Zip(stagingDirectory, packageBuildsDirectory + File(packageName));
});

Task("CIntegration")
    .IsDependentOn("Run-Unit-Tests");

///////////////////////////////////////
// EXECUTION
///////////////////////////////////////
RunTarget(target);
