namespace AssociationRegistry.Admin.Api.Verenigingen.Historiek;

using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;

public class HistoriekResponseExamples : IExamplesProvider<HistoriekResponse>
{
    public HistoriekResponse GetExamples()
        => new(
            "V0000123",
            new List<HistoriekGebeurtenisResponse>
            {
                new(
                    "VerenigingWerdGeregistreerd",
                    "OVO0000001",
                    "11/30/2022 23:00:00"),
            }
        );
}
