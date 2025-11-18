namespace AssociationRegistry.Test.E2E.Framework;

using Admin.Schema.Persoonsgegevens;

public sealed record GeregistreerdeVereniging<TEvent>(
    TEvent GeregistreerdEvent,
    VertegenwoordigerPersoonsgegevensDocument[] PersoonsgegevensDocumenten);
