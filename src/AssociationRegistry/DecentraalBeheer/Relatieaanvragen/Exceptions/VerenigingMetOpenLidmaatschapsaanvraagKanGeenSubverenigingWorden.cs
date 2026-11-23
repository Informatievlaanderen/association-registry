namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

/// <summary>The message names every open lidmaatschapsaanvraag: there can be one per koepel.</summary>
[Serializable]
public class VerenigingMetOpenLidmaatschapsaanvraagKanGeenSubverenigingWorden : DomainException
{
    public VerenigingMetOpenLidmaatschapsaanvraagKanGeenSubverenigingWorden(
        IReadOnlyCollection<RelatieaanvraagId> aanvraagIds
    )
        : base(Message(aanvraagIds)) { }

    protected VerenigingMetOpenLidmaatschapsaanvraagKanGeenSubverenigingWorden(
        SerializationInfo info,
        StreamingContext context
    )
        : base(info, context) { }

    private static string Message(IReadOnlyCollection<RelatieaanvraagId> aanvraagIds) =>
        aanvraagIds.Count == 1
            ? string.Format(
                ExceptionMessages.VerenigingMetOpenLidmaatschapsaanvraagKanGeenSubverenigingWorden,
                aanvraagIds.Single().Value
            )
            : string.Format(
                ExceptionMessages.VerenigingMetOpenLidmaatschapsaanvragenKanGeenSubverenigingWorden,
                string.Join(", ", aanvraagIds.Select(aanvraagId => $"'{aanvraagId.Value}'"))
            );
}
