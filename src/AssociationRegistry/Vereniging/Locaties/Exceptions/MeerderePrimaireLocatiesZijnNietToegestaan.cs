namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class MeerderePrimaireLocatiesZijnNietToegestaan : DomainException
{
    public MeerderePrimaireLocatiesZijnNietToegestaan() : base(ExceptionMessages.MultiplePrimaireLocaties)
    {
    }

    protected MeerderePrimaireLocatiesZijnNietToegestaan(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
