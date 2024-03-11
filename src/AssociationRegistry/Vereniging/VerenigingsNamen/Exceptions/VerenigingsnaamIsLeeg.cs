namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingsnaamIsLeeg : DomainException
{
    public VerenigingsnaamIsLeeg() : base(ExceptionMessages.EmptyVerenigingsNaam)
    {
    }

    protected VerenigingsnaamIsLeeg(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
