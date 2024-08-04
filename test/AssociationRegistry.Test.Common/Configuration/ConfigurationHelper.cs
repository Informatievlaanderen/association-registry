namespace AssociationRegistry.Test.Common.Configuration;

using Admin.Api;
using Microsoft.Extensions.Configuration;
using System.Reflection;

public class ConfigurationHelper
{
    public static IConfigurationRoot GetConfiguration()
        => new ConfigurationBuilder()
          .SetBasePath(GetRootDirectory())
          .AddJsonFile(path: "appsettings.json", optional: true)
          .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true)
          .AddUserSecrets<Program>()
          .AddEnvironmentVariables()
          .Build();

    private static string GetRootDirectory()
    {
        var maybeRootDirectory = Directory
                                .GetParent(typeof(Program).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;

        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");

        return rootDirectory;
    }
}
