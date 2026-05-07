namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class ErkenningIsAlReedsGeschorst : DomainException
{
    public ErkenningIsAlReedsGeschorst()
        : base(ExceptionMessages.ErkenningIsAlReedsGeschorst) { }

    protected ErkenningIsAlReedsGeschorst(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
