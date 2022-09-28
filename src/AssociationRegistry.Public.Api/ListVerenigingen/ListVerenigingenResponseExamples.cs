using Swashbuckle.AspNetCore.Filters;

namespace AssociationRegistry.Public.Api.ListVerenigingen;

public class ListVerenigingenResponseExamples: IExamplesProvider<ListVerenigingenResponse>
{

    private readonly AppSettings _appSettings;

    public ListVerenigingenResponseExamples(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }
    public ListVerenigingenResponse GetExamples() => null!; // TODO implement good example !
}
