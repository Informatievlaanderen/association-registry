namespace AssociationRegistry.Public.Api.DetailVerenigingen;

using Swashbuckle.AspNetCore.Filters;

public class DetailVerenigingResponseExamples : IExamplesProvider<DetailVerenigingResponseWithActualData>
{
    private readonly AppSettings _appSettings;

    public DetailVerenigingResponseExamples(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public DetailVerenigingResponseWithActualData GetExamples()
        => null;
}
