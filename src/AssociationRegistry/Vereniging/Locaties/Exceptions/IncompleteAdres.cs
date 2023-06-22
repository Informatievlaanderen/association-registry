namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class IncompleteAdres : DomainException
{
    public IncompleteAdres() : base("Een adres moet bestaan uit straatnaam, huisnummer, postcode, gemeente en land.")
    {
    }

    protected IncompleteAdres(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
