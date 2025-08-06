namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
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
