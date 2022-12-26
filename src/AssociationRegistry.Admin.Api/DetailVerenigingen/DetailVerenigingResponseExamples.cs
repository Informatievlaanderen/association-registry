namespace AssociationRegistry.Admin.Api.DetailVerenigingen;

using Swashbuckle.AspNetCore.Filters;

public class DetailVerenigingResponseExamples : IExamplesProvider<DetailVerenigingResponse>
{
    private readonly AppSettings _appSettings;

    public DetailVerenigingResponseExamples(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public DetailVerenigingResponse GetExamples()
        => null!;
}
