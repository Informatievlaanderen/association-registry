namespace AssociationRegistry.Admin.Api.Projections.Historiek.Schema;

using Events;
using Events.CommonEventDataTypes;

public record ContactInfoWerdVerwijderdData(string Contactnaam) : IHistoriekData;

public record ContactInfoWerdToegevoegdData(ContactInfo ContactInfo) : IHistoriekData;

public record EmailContactInfoWerdGewijzigdHistoriekData(string Contactnaam, string Email) : IHistoriekData;

public record TelefoonContactInfoWerdGewijzigdHistoriekData(string Contactnaam, string Telefoon) : IHistoriekData;

public record WebsiteContactInfoWerdGewijzigdHistoriekData(string Contactnaam, string Website) : IHistoriekData;

public record SocialMediaContactInfoWerdGewijzigdHistoriekData(string Contactnaam, string SocialMedia) : IHistoriekData;

public record PrimairContactInfoWerdGewijzigdHistoriekData(string Contactnaam, bool Primair) : IHistoriekData;

public record KorteBeschrijvingWerdGewijzigdData(string KorteBeschrijving) : IHistoriekData;

public record KorteNaamWerdGewijzigdData(string KorteNaam) : IHistoriekData;

public record NaamWerdGewijzigdData(string Naam) : IHistoriekData;

public record StartdatumWerdGewijzigdData(string StartDatum) : IHistoriekData;

public record VerenigingWerdgeregistreerdData(VerenigingWerdGeregistreerd Vereniging) : IHistoriekData;
