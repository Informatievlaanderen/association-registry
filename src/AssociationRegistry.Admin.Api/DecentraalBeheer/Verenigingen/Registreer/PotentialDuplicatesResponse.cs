namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using AssociationRegistry.DuplicateVerenigingDetection;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using Detail;
using System.Collections.Immutable;
using System.Runtime.Serialization;
using Vereniging;

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
        bool isV2)
    {
        if (potentialDuplicates == null)
            throw new ArgumentNullException(nameof(potentialDuplicates));

        if (appSettings == null)
            throw new ArgumentNullException(nameof(appSettings));

        BevestigingsToken = hashedRequest;
        MogelijkeDuplicateVerenigingen = potentialDuplicates.Candidates.Select(c => FromDuplicaatVereniging(c, appSettings, isV2)).ToArray();
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
        bool isV2)
        => new(
            document.VCode,
            document.Naam,
            document.KorteNaam,
            Map(document.Verenigingstype, isV2),
            document.HoofdactiviteitenVerenigingsloket.Select(HoofdactiviteitVerenigingsloket.FromDuplicaatVereniging).ToImmutableArray(),
            document.Locaties.Select(Locatie.FromDuplicaatVereniging).ToImmutableArray(),
            new VerenigingLinks(new Uri($"{appSettings.BaseUrl}/v1/verenigingen/{(string?)document.VCode}")));

    private static VerenigingsType Map(AssociationRegistry.DuplicateVerenigingDetection.DuplicaatVereniging.VerenigingsType type, bool isV2)
    {
        if (!Verenigingstype.IsGeenKboVereniging(type.Code))
            return new VerenigingsType(type.Code, type.Naam);

        return isV2
            ? new VerenigingsType(Verenigingstype.VZER.Code, Verenigingstype.VZER.Naam)
            : new VerenigingsType(Verenigingstype.FeitelijkeVereniging.Code, Verenigingstype.FeitelijkeVereniging.Naam);
    }
}
