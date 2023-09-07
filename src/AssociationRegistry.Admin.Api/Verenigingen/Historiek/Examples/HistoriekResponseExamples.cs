namespace AssociationRegistry.Admin.Api.Verenigingen.Historiek.Examples;

using Infrastructure.ConfigurationBindings;
using ResponseModels;
using Swashbuckle.AspNetCore.Filters;

public class HistoriekResponseExamples : IExamplesProvider<HistoriekResponse>
{
    private readonly AppSettings _appSettings;

    public HistoriekResponseExamples(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }
    public HistoriekResponse GetExamples()
        => new()
        {
            Context = $"{_appSettings.BaseUrl}/v1/contexten/historiek-vereniging-context.json",
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
                    Tijdstip = "2023-08-23 07:20",
                },
            },
        };
}
