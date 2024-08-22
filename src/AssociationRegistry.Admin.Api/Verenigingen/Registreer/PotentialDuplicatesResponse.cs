namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using DuplicateVerenigingDetection;
using Hosts.Configuration.ConfigurationBindings;
using System.Collections.Immutable;
using System.Runtime.Serialization;

[DataContract]
public class PotentialDuplicatesResponse
{
    public PotentialDuplicatesResponse(string hashedRequest, PotentialDuplicatesFound potentialDuplicates, AppSettings appSettings)
    {
        BevestigingsToken = hashedRequest;
        MogelijkeDuplicateVerenigingen = potentialDuplicates.Candidates.Select(c => FromDuplicaatVereniging(c, appSettings)).ToArray();
    }

    /// <summary>Dit token wordt gebruikt als bevestiging dat de vereniging uniek is en geregistreerd mag worden,
    /// ondanks de voorgestelde duplicaten.</summary>
    [DataMember(Name = "BevestigingsToken")]
    public string BevestigingsToken { get; init; }

    /// <summary>Een lijst van verenigingen die mogelijks een duplicaat zijn
    /// van de vereniging uit de registreer aanvraag</summary>
    [DataMember(Name = "MogelijkeDuplicateVerenigingen")]
    public DuplicaatVerenigingContract[] MogelijkeDuplicateVerenigingen { get; init; }

    private static DuplicaatVerenigingContract FromDuplicaatVereniging(DuplicaatVereniging document, AppSettings appSettings)
        => new(
            document.VCode,
            document.Naam,
            document.KorteNaam,
            new VerenigingsType(document.Verenigingstype.Code, document.Verenigingstype.Naam),
            document.HoofdactiviteitenVerenigingsloket.Select(HoofdactiviteitVerenigingsloket.FromDuplicaatVereniging).ToImmutableArray(),
            document.Locaties.Select(Locatie.FromDuplicaatVereniging).ToImmutableArray(),
            new VerenigingLinks(new Uri($"{appSettings.BaseUrl}/v1/verenigingen/{(string?)document.VCode}")));
}
