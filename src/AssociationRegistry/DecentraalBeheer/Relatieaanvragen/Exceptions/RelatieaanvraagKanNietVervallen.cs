namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Formats;
using Resources;

/// <summary>
///     A programming error, not an answer to a partij: the caller checks <see cref="RelatieaanvraagState.KanVervallenOp" />
///     first. Like <c>ErkenningKanNietVerlopenWorden</c>.
/// </summary>
[Serializable]
public class RelatieaanvraagKanNietVervallen : ApplicationException
{
    public RelatieaanvraagKanNietVervallen(
        RelatieaanvraagId aanvraagId,
        RelatieaanvraagStatus status,
        DateOnly vervalDatum
    )
        : base(
            string.Format(
                ExceptionMessages.RelatieaanvraagKanNietVervallen,
                aanvraagId.Value,
                status.Value,
                vervalDatum.FormatAsBelgianDate()
            )
        ) { }

    protected RelatieaanvraagKanNietVervallen(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
