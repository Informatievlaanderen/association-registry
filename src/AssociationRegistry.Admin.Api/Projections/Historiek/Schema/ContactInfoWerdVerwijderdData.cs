namespace AssociationRegistry.Admin.Api.Projections.Historiek.Schema;

using Events;

public record KorteBeschrijvingWerdGewijzigdData(string KorteBeschrijving) : IHistoriekData;

public record KorteNaamWerdGewijzigdData(string KorteNaam) : IHistoriekData;

public record NaamWerdGewijzigdData(string Naam) : IHistoriekData;

public record StartdatumWerdGewijzigdData(string? StartDatum) : IHistoriekData;

public record VerenigingWerdgeregistreerdData(VerenigingWerdGeregistreerd Vereniging) : IHistoriekData;
