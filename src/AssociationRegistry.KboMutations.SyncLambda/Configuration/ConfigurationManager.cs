namespace AssociationRegistry.KboMutations.SyncLambda.Configuration;

using Microsoft.Extensions.Configuration;

public class ConfigurationManager
{
    public IConfigurationRoot Build()
    {
        return new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", true, true)
              .AddJsonFile($"appsettings.MSI.json", optional: true,
                           reloadOnChange: false)
              .AddEnvironmentVariables()
              .Build();
    }
}
