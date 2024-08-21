namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

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
