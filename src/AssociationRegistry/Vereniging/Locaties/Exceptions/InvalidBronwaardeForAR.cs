namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidBronwaardeForAR : DomainException
{
    public InvalidBronwaardeForAR() : base("De bronwaarde voor een adres uit het addressenregister moet een Data Vlaanderen PURI zijn.")
    {
    }

    protected InvalidBronwaardeForAR(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
