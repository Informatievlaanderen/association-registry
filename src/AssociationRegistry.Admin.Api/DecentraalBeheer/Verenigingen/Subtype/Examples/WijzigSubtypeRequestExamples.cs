namespace AssociationRegistry.Admin.Api.Verenigingen.Subtype.Examples;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Vereniging;
using Swashbuckle.AspNetCore.Filters;
using Adres = Common.Adres;
using AdresId = Common.AdresId;

public class WijzigSubtypeRequestExamples : IExamplesProvider<WijzigSubtypeRequest>
{
    // TODO:
    public WijzigSubtypeRequest GetExamples()
        => new()
        {
        };
}
