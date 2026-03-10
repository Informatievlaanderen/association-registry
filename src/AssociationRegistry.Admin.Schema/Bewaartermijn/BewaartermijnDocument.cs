namespace AssociationRegistry.Admin.Schema.Bewaartermijn;

using Marten.Schema;
using NodaTime;

public record BewaartermijnDocument(
    [property: Identity] string BewaartermijnId,
    string VCode,
    string BewaartermijnType,
    int RecordId,
    string Reden,
    string Status,
    Instant Vervaldag,
    BewaartermijnGebeurtenis[] Gebeurtenissen
);

public record BewaartermijnGebeurtenis(string Status, Instant Tijdstip);

public record BewaartermijnStatus(string StatusNaam)
{
    public record StatusGepland() : BewaartermijnStatus(Naam)
    {
        public const string Naam = "Gepland";
    }

    public record StatusVerlopen() : BewaartermijnStatus(Naam)
    {
        public const string Naam = "Verlopen";
    }

    public static BewaartermijnStatus Gepland => new StatusGepland();
    public static BewaartermijnStatus Verlopen => new StatusVerlopen();
}
