namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer;

using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using DecentraalBeheer.Vereniging.Mappers;
using System.Collections.Immutable;
using System.Runtime.Serialization;

[DataContract]
public class PotentialDuplicatesResponse
{

    public PotentialDuplicatesResponse()
    {

    }

    public PotentialDuplicatesResponse(
        string hashedRequest,
        PotentialDuplicatesFound potentialDuplicates,
        AppSettings appSettings,
        IVerenigingstypeMapper verenigingstypeMapper)
    {
        if (potentialDuplicates == null)
            throw new ArgumentNullException(nameof(potentialDuplicates));

        if (appSettings == null)
            throw new ArgumentNullException(nameof(appSettings));

        BevestigingsToken = hashedRequest;
        MogelijkeDuplicateVerenigingen = potentialDuplicates.PotentialDuplicates.Select(c => FromDuplicaatVereniging(c, appSettings, verenigingstypeMapper)).ToArray();
    }

    /// <summary>Dit token wordt gebruikt als bevestiging dat de vereniging uniek is en geregistreerd mag worden,
    /// ondanks de voorgestelde duplicaten.</summary>
    [DataMember(Name = "BevestigingsToken")]
    public string BevestigingsToken { get; init; }

    /// <summary>Een lijst van verenigingen die mogelijks een duplicaat zijn
    /// van de vereniging uit de registreer aanvraag</summary>
    [DataMember(Name = "MogelijkeDuplicateVerenigingen")]
    public DuplicaatVerenigingContract[] MogelijkeDuplicateVerenigingen { get; init; }

    private static DuplicaatVerenigingContract FromDuplicaatVereniging(
        DuplicaatVereniging document,
        AppSettings appSettings,
        IVerenigingstypeMapper verenigingstypeMapper)
        => new(
            document.VCode,
            document.Naam,
            document.KorteNaam,
            verenigingstypeMapper.Map<Verenigingstype,DuplicaatVereniging.Types.Verenigingstype>(document.Verenigingstype),
            verenigingstypeMapper.MapSubtype<Verenigingssubtype, DuplicaatVereniging.Types.Verenigingssubtype>(document.Verenigingssubtype),
            document.HoofdactiviteitenVerenigingsloket.Select(HoofdactiviteitVerenigingsloket.FromDuplicaatVereniging).ToImmutableArray(),
            document.Locaties.Select(Locatie.FromDuplicaatVereniging).ToImmutableArray(),
            new VerenigingLinks(new Uri($"{appSettings.BaseUrl}/v1/verenigingen/{(string?)document.VCode}")));


}
