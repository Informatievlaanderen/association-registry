namespace AssociationRegistry.VerenigingsNamen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class EmptyVerenigingsNaam : DomainException
{
    public EmptyVerenigingsNaam() : base("De naam van de vereniging is verplicht.")
    {
    }

    protected EmptyVerenigingsNaam(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
