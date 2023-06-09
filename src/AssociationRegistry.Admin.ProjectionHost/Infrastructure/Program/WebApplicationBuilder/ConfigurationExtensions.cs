namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using ConfigurationBindings;

public static class ConfigurationExtensions
{
    public static PostgreSqlOptionsSection GetValidPostgreSqlOptionsOrThrow(this ConfigurationManager source)
    {
        var postgreSqlOptions = source.GetSection(PostgreSqlOptionsSection.Name)
            .Get<PostgreSqlOptionsSection>();

        ConfigHelpers.ThrowIfInvalidPostgreSqlOptions(postgreSqlOptions);
        return postgreSqlOptions;
    }
}
