namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;

using Swashbuckle.AspNetCore.Filters;

public class VoegLocatieToeRequestExamples : IExamplesProvider<VoegLocatieToeRequest>
{
    public VoegLocatieToeRequest GetExamples()
        => new();
}
