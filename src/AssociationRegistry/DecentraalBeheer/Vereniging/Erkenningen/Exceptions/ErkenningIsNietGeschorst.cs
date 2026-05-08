namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class ErkenningIsNietGeschorst : DomainException
{
    public ErkenningIsNietGeschorst()
        : base(ExceptionMessages.ErkenningIsNietGeschorst) { }

    protected ErkenningIsNietGeschorst(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
