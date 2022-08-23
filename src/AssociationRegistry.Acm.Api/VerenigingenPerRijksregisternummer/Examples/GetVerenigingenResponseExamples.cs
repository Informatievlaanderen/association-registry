namespace AssociationRegistry.Acm.Api.VerenigingenPerRijksregisternummer.Examples;

using System.Collections.Immutable;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Filters;

public class GetVerenigingenResponseExamples : IExamplesProvider<GetVerenigingenResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ProblemDetailsHelper _problemDetailsHelper;

    public GetVerenigingenResponseExamples(
        IHttpContextAccessor httpContextAccessor,
        ProblemDetailsHelper problemDetailsHelper)
    {
        _httpContextAccessor = httpContextAccessor;
        _problemDetailsHelper = problemDetailsHelper;
    }

    public GetVerenigingenResponse GetExamples() =>
        new GetVerenigingenResponse(
            "12345678901",
            ImmutableArray.Create(
                new Vereniging("V1234567", "FWA De vrolijke BA’s"),
                new Vereniging("V7654321", "FWA De Bron"))
        );
}
