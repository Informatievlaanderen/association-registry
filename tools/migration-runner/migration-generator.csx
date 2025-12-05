#!/usr/bin/env dotnet-script
#r "nuget: YamlDotNet, 13.7.1"
#r "nuget: Spectre.Console, 0.48.0"

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Spectre.Console;

// Configuration classes
public class MigrationConfig
{
    public Dictionary<string, ProjectConfig> Projects { get; set; } = new();
    public List<CustomScript> CustomScripts { get; set; } = new();
}

public class ProjectConfig
{
    public string Name { get; set; }
    public string CsprojPath { get; set; }
    public string MigrationDestination { get; set; }
    public string ConnectionString { get; set; }
}

public class CustomScript
{
    public string Name { get; set; }
    public string Command { get; set; }
    public string WorkingDirectory { get; set; }
}

// Main application
var rootPath = FindRepositoryRoot();
var configPath = Path.Combine(rootPath, "tools/migration-runner/config.yaml");
if (!File.Exists(configPath))
{
    CreateDefaultConfig(configPath);
    AnsiConsole.MarkupLine("[green]Created default configuration at {0}[/]", configPath);
    AnsiConsole.MarkupLine("[yellow]Please update the configuration and run again.[/]");
    return;
}

var config = LoadConfig(configPath);

// Check if a project was specified as command line argument
var args = Args.ToArray(); // Use Args from dotnet-script
string selectedProject = null;
if (args.Length > 0)
{
    selectedProject = args[0];
}

if (selectedProject != null)
{
    // Direct mode - run specified project and exit
    if (config.Projects.ContainsKey(selectedProject))
    {
        var project = config.Projects[selectedProject];
        await GenerateMigration(project);
    }
    else
    {
        AnsiConsole.MarkupLine($"[red]Unknown project: {selectedProject}[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine("Available projects:");
        foreach (var p in config.Projects.Keys)
        {
            AnsiConsole.WriteLine($"  - {p}");
        }
    }
}
else
{
    // Interactive mode - show menu
    AnsiConsole.Clear();
    AnsiConsole.Write(
        new FigletText("Migration Generator")
            .Centered()
            .Color(Color.Blue));

    var choices = new List<string>();
    choices.AddRange(config.Projects.Keys);
    choices.Add("--- Custom Scripts ---");
    choices.AddRange(config.CustomScripts.Select(s => s.Name));

    var selection = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Select a [green]project[/] to generate migration for:")
            .PageSize(10)
            .AddChoices(choices));

    if (selection == "--- Custom Scripts ---")
    {
        AnsiConsole.MarkupLine("[yellow]Please select a specific project.[/]");
        return;
    }

    if (config.Projects.ContainsKey(selection))
    {
        var project = config.Projects[selection];
        await GenerateMigration(project);
    }
    else
    {
        var script = config.CustomScripts.FirstOrDefault(s => s.Name == selection);
        if (script != null)
        {
            await ExecuteCustomScript(script);
        }
    }
}

// Helper methods
void CreateDefaultConfig(string path)
{
    var defaultConfig = new MigrationConfig
    {
        Projects = new Dictionary<string, ProjectConfig>
        {
            ["Admin API"] = new ProjectConfig
            {
                Name = "Admin API",
                CsprojPath = "src/AssociationRegistry.Admin.Api",
                MigrationDestination = "migrations/production/admin.api/scripts/up",
                ConnectionString = "postgresql://127.0.0.1/verenigingsregister"
            },
            ["Wolverine Schema"] = new ProjectConfig
            {
                Name = "Wolverine Schema",
                CsprojPath = "src/AssociationRegistry.Admin.Api",
                MigrationDestination = "migrations/production/admin.api/scripts/up",
                ConnectionString = "wolverine://messages/main"
            },
            ["Admin Projections"] = new ProjectConfig
            {
                Name = "Admin Projections",
                CsprojPath = "src/AssociationRegistry.Admin.ProjectionHost",
                MigrationDestination = "migrations/production/admin.projections/scripts/up",
                ConnectionString = "postgresql://127.0.0.1/verenigingsregister"
            },
            ["Public Projections"] = new ProjectConfig
            {
                Name = "Public Projections",
                CsprojPath = "src/AssociationRegistry.Public.ProjectionHost",
                MigrationDestination = "migrations/production/public.projections/scripts/up",
                ConnectionString = "postgresql://127.0.0.1/verenigingsregister"
            },
            ["ACM API"] = new ProjectConfig
            {
                Name = "ACM API",
                CsprojPath = "src/AssociationRegistry.Acm.Api",
                MigrationDestination = "migrations/production/acm.api/scripts/up",
                ConnectionString = "postgresql://127.0.0.1/verenigingsregister"
            },
            ["Public API"] = new ProjectConfig
            {
                Name = "Public API",
                CsprojPath = "src/AssociationRegistry.Public.Api",
                MigrationDestination = "migrations/production/public.api/scripts/up",
                ConnectionString = "postgresql://127.0.0.1/verenigingsregister"
            }
        },
        CustomScripts = new List<CustomScript>
        {
            new CustomScript
            {
                Name = "Regenerate Marten",
                Command = "./regen-marten.sh",
                WorkingDirectory = "."
            }
        }
    };

    var serializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();
    
    var yaml = serializer.Serialize(defaultConfig);
    File.WriteAllText(path, yaml);
}

MigrationConfig LoadConfig(string path)
{
    var yaml = File.ReadAllText(path);
    var deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();
    
    return deserializer.Deserialize<MigrationConfig>(yaml);
}

async Task GenerateMigration(ProjectConfig project)
{
    var rootPath = FindRepositoryRoot();
    var projectPath = Path.Combine(rootPath, project.CsprojPath);
    var destinationPath = Path.Combine(rootPath, project.MigrationDestination);

    AnsiConsole.MarkupLine($"[yellow]Generating migration for {project.Name}...[/]");
    AnsiConsole.MarkupLine($"[dim]Project: {projectPath}[/]");
    AnsiConsole.MarkupLine($"[dim]Connection: {project.ConnectionString}[/]");
    AnsiConsole.WriteLine();

    var tempFile = Path.GetTempFileName();
    var success = false;

    await AnsiConsole.Status()
        .StartAsync($"Running db-patch...", async ctx =>
        {
            ctx.Spinner(Spinner.Known.Star);
            ctx.SpinnerStyle(Style.Parse("green"));

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"run -- db-patch -d {project.ConnectionString} {tempFile}",
                    WorkingDirectory = projectPath,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            var output = new List<string>();
            var errors = new List<string>();
            var noChangesDetected = false;

            process.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    output.Add(args.Data);
                    // Check for the "No differences" message
                    if (args.Data.Contains("No differences were detected"))
                    {
                        noChangesDetected = true;
                    }
                    // Escape the output to prevent markup interpretation
                    var escaped = args.Data.EscapeMarkup();
                    ctx.Status($"[dim]{escaped}[/]");
                }
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    errors.Add(args.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await Task.Run(() => process.WaitForExit());

            if (process.ExitCode == 0)
            {
                success = true;
                AnsiConsole.MarkupLine("[green]Migration generated successfully![/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Failed to generate migration. Exit code: {process.ExitCode}[/]");
                
                if (errors.Any())
                {
                    AnsiConsole.WriteLine();
                    AnsiConsole.Write(new Rule("[red]Errors[/]"));
                    errors.ForEach(line => AnsiConsole.WriteLine(line));
                }
            }

            // Show output
            if (output.Any())
            {
                AnsiConsole.WriteLine();
                AnsiConsole.Write(new Rule("[yellow]Output[/]"));
                output.ForEach(line => AnsiConsole.WriteLine(line));
            }
        });

    if (success)
    {
        if (File.Exists(tempFile))
        {
            // Check if the file has content
            var sqlContent = File.ReadAllText(tempFile);
            if (string.IsNullOrWhiteSpace(sqlContent))
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[yellow]✓ No schema changes detected. Database is up to date.[/]");
                File.Delete(tempFile);
                return;
            }

            // Show migration content
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule("[blue]Generated SQL[/]"));
            AnsiConsole.WriteLine(sqlContent);
            AnsiConsole.WriteLine();

            // Ask for script name
            var scriptName = AnsiConsole.Ask<string>("Enter a name for this migration script (without number or .sql extension):");
            
            if (!string.IsNullOrWhiteSpace(scriptName))
            {
                SaveMigrationScript(tempFile, destinationPath, scriptName);
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]Migration not saved.[/]");
                File.Delete(tempFile);
            }
        }
        else
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow]No migration file was generated.[/]");
        }
    }
    else
    {
        // Clean up temp file if it exists
        if (File.Exists(tempFile))
        {
            File.Delete(tempFile);
        }
    }
}

void SaveMigrationScript(string sourceFile, string destinationPath, string scriptName)
{
    try
    {
        // Ensure destination directory exists
        Directory.CreateDirectory(destinationPath);

        // Get existing scripts to determine next number
        var existingScripts = Directory.GetFiles(destinationPath, "*.sql")
            .Select(Path.GetFileName)
            .Where(f => Regex.IsMatch(f, @"^\d+_.*\.sql$"))
            .OrderBy(f => f)
            .ToList();

        int nextNumber = 1;
        if (existingScripts.Any())
        {
            var lastScript = existingScripts.Last();
            var match = Regex.Match(lastScript, @"^(\d+)_");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        // Format the new filename
        var newFileName = $"{nextNumber:D5}_{scriptName}.sql";
        var destinationFile = Path.Combine(destinationPath, newFileName);

        // Move the file
        File.Move(sourceFile, destinationFile);
        AnsiConsole.MarkupLine($"[green]✓ Migration saved as: {newFileName}[/]");
        AnsiConsole.MarkupLine($"[dim]Full path: {destinationFile}[/]");
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine($"[red]Error saving migration: {ex.Message}[/]");
    }
}

async Task ExecuteCustomScript(CustomScript script)
{
    var rootPath = FindRepositoryRoot();
    var workingDir = Path.Combine(rootPath, script.WorkingDirectory);

    await AnsiConsole.Status()
        .StartAsync($"Executing {script.Name}...", async ctx =>
        {
            ctx.Spinner(Spinner.Known.Star);
            ctx.SpinnerStyle(Style.Parse("yellow"));

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{script.Command}\"",
                    WorkingDirectory = workingDir,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            var output = new List<string>();

            process.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    output.Add(args.Data);
                    // Escape the output to prevent markup interpretation
                    var escaped = args.Data.EscapeMarkup();
                    ctx.Status($"[dim]{escaped}[/]");
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await Task.Run(() => process.WaitForExit());

            AnsiConsole.MarkupLine($"[yellow]Script finished with exit code: {process.ExitCode}[/]");
            
            if (output.Any())
            {
                AnsiConsole.WriteLine();
                AnsiConsole.Write(new Rule("[yellow]Output[/]"));
                output.Take(20).ToList().ForEach(line => AnsiConsole.WriteLine(line));
                if (output.Count > 20)
                {
                    AnsiConsole.MarkupLine($"[dim]... and {output.Count - 20} more lines[/]");
                }
            }
        });
}

string FindRepositoryRoot()
{
    var currentDir = Directory.GetCurrentDirectory();
    while (!string.IsNullOrEmpty(currentDir))
    {
        if (File.Exists(Path.Combine(currentDir, "AssociationRegistry.sln")))
        {
            return currentDir;
        }
        currentDir = Path.GetDirectoryName(currentDir);
    }
    throw new InvalidOperationException("Could not find repository root");
}