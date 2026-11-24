namespace AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.BevestigRelatieaanvraag.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class BevestigRelatieaanvraagRequestExamples : IExamplesProvider<BevestigRelatieaanvraagRequest>
{
    public BevestigRelatieaanvraagRequest GetExamples() =>
        new() { Identificatie = "LID-001", Beschrijving = "Lid van de koepel" };
}
