namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class MeerderePrimaireContactgegevensZijnNietToegestaan : DomainException
{
    public MeerderePrimaireContactgegevensZijnNietToegestaan(string type) : base(
        $"Er mag maar één {type} contactgegeven aangeduid zijn als primair.")
    {
    }

    protected MeerderePrimaireContactgegevensZijnNietToegestaan(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
