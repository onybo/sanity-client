var target          = Argument("target", "Default");
var configuration   = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var packPath            = Directory("./src/client");
var buildArtifacts      = Directory("./artifacts/packages");
// A directory path to an Artifacts directory.
var artifactsDirectory = Directory("./artifacts");

var isAppVeyor          = AppVeyor.IsRunningOnAppVeyor;
var version             = AppVeyor.IsRunningOnAppVeyor ?
                             "0.0." + AppVeyor.Environment.Build.Number :
                             "0.0.1";

///////////////////////////////////////////////////////////////////////////////
// Clean
///////////////////////////////////////////////////////////////////////////////
Task("Clean")
    .Does(() =>
{
    CleanDirectories(new DirectoryPath[] { buildArtifacts, artifactsDirectory });
});

///////////////////////////////////////////////////////////////////////////////
// Restore
///////////////////////////////////////////////////////////////////////////////
Task("Restore")
    .Does(() =>
{
    var settings = new DotNetCoreRestoreSettings
    {
        Sources = new [] { "https://api.nuget.org/v3/index.json" }
    };

    var projects = GetFiles("./**/*.csproj");

	foreach(var project in projects)
	{
	    DotNetCoreRestore(project.GetDirectory().FullPath, settings);
    }
});

///////////////////////////////////////////////////////////////////////////////
// Build
///////////////////////////////////////////////////////////////////////////////
Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings 
    {
        Configuration = configuration,
        ArgumentCustomization = args => args.Append("/p:SemVer=" + version)
    };

    // libraries
	var projects = GetFiles("./src/**/*.csproj");
	foreach(var project in projects)
	{
	    DotNetCoreBuild(project.GetDirectory().FullPath, settings); 
    }

    // tests
	projects = GetFiles("./test/**/*.csproj");
	foreach(var project in projects)
	{
	    DotNetCoreBuild(project.GetDirectory().FullPath, settings); 
    }
});

///////////////////////////////////////////////////////////////////////////////
// Test
// Look under a 'Tests' folder and run dotnet test against all of those projects.
// Then drop the XML test results file in the Artifacts folder at the root.
///////////////////////////////////////////////////////////////////////////////
Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
        var projects = GetFiles("./test/**/*tests.csproj");
        foreach(var project in projects)
        {
            DotNetCoreTest(
                project.GetDirectory().FullPath,
                new DotNetCoreTestSettings()
                {
                    ArgumentCustomization = args => args
                        .Append("--logger \"trx;LogFileName=" 
                            + artifactsDirectory.Path.CombineWithFilePath(project.GetFilenameWithoutExtension()).FullPath + ".xml\""),
                    Configuration = configuration,
                    NoBuild = true
                });
        }
});


///////////////////////////////////////////////////////////////////////////////
// Pack
///////////////////////////////////////////////////////////////////////////////
Task("Pack")
    .IsDependentOn("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var settings = new DotNetCorePackSettings
    {
        Configuration = configuration,
        OutputDirectory = buildArtifacts,
        ArgumentCustomization = args => args.Append("--include-symbols").Append(" /p:SemVer=" + version)
    };

    DotNetCorePack(packPath, settings);
});

Task("Default")
  .IsDependentOn("Build")
  .IsDependentOn("Test")
  .IsDependentOn("Pack");

RunTarget(target);