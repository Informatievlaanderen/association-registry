namespace AssociationRegistry.Test.Acm.Api.Framework;

using AssociationRegistry.Acm.Api.Caches;

public class VerenigingenRepositoryStub : IVerenigingenRepository
{
    public VerenigingenPerRijksregisternummer Verenigingen { get; set; } =
        VerenigingenPerRijksregisternummer
            .FromVerenigingenAsDictionary(
                new VerenigingenAsDictionary()
                {
                    {
                        "7103",
                        new()
                        {
                            { "V0000001", "De eenzame in de lijst" },
                        }
                    },
                    {
                        "9803",
                        new()
                        {
                            { "V1234567", "FWA De vrolijke BA’s" },
                            { "V7654321", "FWA De Bron" },
                        }
                    },
                }
            );

    public Task UpdateVerenigingen(VerenigingenAsDictionary verenigingenAsDictionary, Stream verenigingenStream,
        CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
