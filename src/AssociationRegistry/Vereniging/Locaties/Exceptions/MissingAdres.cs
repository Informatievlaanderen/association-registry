namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class MissingAdres : DomainException
{
    public MissingAdres() : base("Een locatie moet minstens een adresId of een adres bevatten.")
    {
    }

    protected MissingAdres(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
