namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class ErkenningIsGeschorst : DomainException
{
    public ErkenningIsGeschorst()
        : base(ExceptionMessages.ErkenningIsGeschorst) { }

    protected ErkenningIsGeschorst(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
