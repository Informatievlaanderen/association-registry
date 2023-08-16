namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class EmptyVerenigingsNaam : DomainException
{
    public EmptyVerenigingsNaam() : base(ExceptionMessages.EmptyVerenigingsNaam)
    {
    }

    protected EmptyVerenigingsNaam(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
