namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerRedenSchorsingErkenning.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class CorrigeerRedenSchorsingErkenningRequestExamples : IExamplesProvider<CorrigeerRedenSchorsingErkenningRequest>
{
    public CorrigeerRedenSchorsingErkenningRequest GetExamples() =>
        new() { RedenSchorsing = "Niet-naleving van de erkenningsvoorwaarden zoals bepaald in het reglement." };
}
