namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class ErkenningKanNietGeactiveerdWorden : DomainException
{
    public ErkenningKanNietGeactiveerdWorden(
        int erkenningId,
        DateOnly? startdatum,
        DateOnly? einddatum,
        ErkenningStatus status
    )
        : base(
            string.Format(
                ExceptionMessages.ErkenningKanNietGeactiveerdWorden,
                erkenningId,
                startdatum,
                einddatum,
                status.Value
            )
        ) { }

    protected ErkenningKanNietGeactiveerdWorden(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
