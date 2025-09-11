namespace AssociationRegistry.Test.Common.Database;

using System.Diagnostics;
using System.IO;
using System.Reflection;
using Extensions;
using Npgsql;

public static class GrateMigrationRunner
{
    public static void RunMigrations(string connectionString, string databaseName)
    {
        Console.WriteLine($"[GrateMigrationRunner] Starting migrations for database: {databaseName}");

        Console.WriteLine("[GrateMigrationRunner] Assuming grate is available");

        // Create a temporary directory for migrations
        var tempPath = Path.Combine(Path.GetTempPath(), $"grate_migrations_{Guid.NewGuid()}");

        try
        {
            Console.WriteLine($"[GrateMigrationRunner] Creating temp directory: {tempPath}");
            Directory.CreateDirectory(tempPath);
            var scriptsPath = Path.Combine(tempPath, "scripts");
            Directory.CreateDirectory(scriptsPath);

            // Extract embedded migration scripts
            Console.WriteLine("[GrateMigrationRunner] Extracting embedded migrations");
            ExtractEmbeddedMigrations(scriptsPath);

            // Run Grate
            Console.WriteLine("[GrateMigrationRunner] Running Grate");
            RunGrate(connectionString, scriptsPath);

            Console.WriteLine("[GrateMigrationRunner] Migrations completed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GrateMigrationRunner] Error during migrations: {ex.Message}");
            throw;
        }
        finally
        {
            // Clean up temporary directory
            if (Directory.Exists(tempPath))
            {
                try
                {
                    Directory.Delete(tempPath, recursive: true);
                    Console.WriteLine("[GrateMigrationRunner] Cleaned up temp directory");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GrateMigrationRunner] Failed to clean up temp directory: {ex.Message}");
                }
            }
        }
    }

    private static bool IsGrateAvailable()
    {
        var grateExecutable = GetGrateExecutablePath();
        Console.WriteLine($"[GrateMigrationRunner] Checking if grate is available: {grateExecutable}");

        try
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = grateExecutable,
                Arguments = "--help",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            });

            if (process != null)
            {
                var completed = process.WaitForExit(5000); // 5 second timeout
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();

                Console.WriteLine($"[GrateMigrationRunner] Grate version check completed: {completed}, ExitCode: {process.ExitCode}");
                Console.WriteLine($"[GrateMigrationRunner] Grate version output: {output}");
                Console.WriteLine($"[GrateMigrationRunner] Grate version error: {error}");

                return completed && process.ExitCode == 0;
            }

            Console.WriteLine("[GrateMigrationRunner] Failed to start grate process");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GrateMigrationRunner] Exception checking grate availability: {ex.Message}");
            return false;
        }
    }

    private static string GetGrateExecutablePath()
    {
        Console.WriteLine($"[GrateMigrationRunner] Base directory: {AppDomain.CurrentDomain.BaseDirectory}");

        // First try to use the local grate executable that's copied to the output directory
        var localGrate = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "grate");
        Console.WriteLine($"[GrateMigrationRunner] Looking for local grate at: {localGrate}");
        Console.WriteLine($"[GrateMigrationRunner] Local grate exists: {File.Exists(localGrate)}");

        if (File.Exists(localGrate))
        {
            Console.WriteLine($"[GrateMigrationRunner] Using local grate executable: {localGrate}");
            return localGrate;
        }

        // Fall back to global grate command
        Console.WriteLine("[GrateMigrationRunner] Using global grate command");
        return "grate";
    }

    private static void ExtractEmbeddedMigrations(string targetPath)
    {
        var assembly = typeof(GrateMigrationRunner).Assembly;

        // Create subdirectories as Grate expects them
        var runAfterCreateDatabasePath = Path.Combine(targetPath, "runAfterCreateDatabase");
        Directory.CreateDirectory(runAfterCreateDatabasePath);

        // Create the up directory for initial migrations
        var upPath = Path.Combine(targetPath, "up");
        Directory.CreateDirectory(upPath);

        // Also create other directories that Grate might look for (even if empty)
        Directory.CreateDirectory(Path.Combine(targetPath, "functions"));
        Directory.CreateDirectory(Path.Combine(targetPath, "views"));
        Directory.CreateDirectory(Path.Combine(targetPath, "sprocs"));
        Directory.CreateDirectory(Path.Combine(targetPath, "indexes"));
        Directory.CreateDirectory(Path.Combine(targetPath, "runAfterOtherAnyTimeScripts"));

        // Extract schema_only_tst.sql to runAfterCreateDatabase folder
        // This runs after the database is created
        var schemaResource = "AssociationRegistry.Test.Common.Database.schema_only_tst.sql";
        var schemaContent = assembly.GetResourceString(schemaResource);
        var schemaPath = Path.Combine(runAfterCreateDatabasePath, "001_schema_only_tst.sql");
        File.WriteAllText(schemaPath, schemaContent);

        Console.WriteLine($"[GrateMigrationRunner] Extracted schema script to: {schemaPath}");
        Console.WriteLine($"[GrateMigrationRunner] Directory structure created at: {targetPath}");
    }

    private static void RunGrate(string connectionString, string scriptsPath)
    {
        Console.WriteLine($"[GrateMigrationRunner] Scripts path: {scriptsPath}");
        Console.WriteLine($"[GrateMigrationRunner] Connection string: {connectionString}");

        var grateExecutable = GetGrateExecutablePath();
        var processStartInfo = new ProcessStartInfo
        {
            FileName = grateExecutable,
            Arguments = $"--connstring=\"{connectionString}\" --files=\"{scriptsPath}\" --version=\"1.0.0\" --databasetype=PostgreSQL --schema=grate_init --silent",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        Console.WriteLine($"[GrateMigrationRunner] Starting grate with args: {processStartInfo.Arguments}");

        using var process = Process.Start(processStartInfo);
        if (process != null)
        {
            // Read output asynchronously to avoid deadlock
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            var completed = process.WaitForExit(60000); // 60 second timeout

            if (!completed)
            {
                try
                {
                    process.Kill();
                }
                catch { }
                throw new Exception("Grate process timed out after 30 seconds");
            }

            var output = outputTask.Result;
            var error = errorTask.Result;

            Console.WriteLine($"[GrateMigrationRunner] Grate output: {output}");
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine($"[GrateMigrationRunner] Grate error: {error}");
            }

            if (process.ExitCode != 0)
            {
                throw new Exception($"Grate migration failed with exit code {process.ExitCode}. Error: {error}. Output: {output}");
            }
        }
        else
        {
            throw new Exception("Failed to start Grate process");
        }
    }
}
