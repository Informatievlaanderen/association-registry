namespace AssociationRegistry.Admin.Api.Verenigingen.Historiek.Examples;

using ResponseModels;
using Swashbuckle.AspNetCore.Filters;

public class HistoriekResponseExamples : IExamplesProvider<HistoriekResponse>
{
    public HistoriekResponse GetExamples()
        => new()
        {
            VCode = "V0000123",
            Gebeurtenissen = new HistoriekGebeurtenisResponse[]
            {
                new()
                {
                    Beschrijving = "Vereniging werd aangemaakt met naam 'Eerste vereniging'",
                    Gebeurtenis = "FeitelijkeVerenigingWerdGeregistreerd",
                    Data = new
                    {
                        Naam = "Eerste vereniging",
                    },
                    Initiator = "OVO0001001",
                    Tijdstip = "11/30/2022 23:00:00",
                },
            },
        };
}
