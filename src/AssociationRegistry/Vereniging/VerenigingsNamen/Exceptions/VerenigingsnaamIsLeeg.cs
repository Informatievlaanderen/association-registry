namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

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
