namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Resources;

[Serializable]
public class ErkenningKanNietVerlopenWorden : ApplicationException
{
    public ErkenningKanNietVerlopenWorden(
        int erkenningId,
        DateOnly? startdatum,
        DateOnly? einddatum,
        ErkenningStatus status
    )
        : base(
            string.Format(
                ExceptionMessages.ErkenningKanNietVerlopenWorden,
                erkenningId,
                startdatum,
                einddatum,
                status.Value
            )
        ) { }

    protected ErkenningKanNietVerlopenWorden(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
