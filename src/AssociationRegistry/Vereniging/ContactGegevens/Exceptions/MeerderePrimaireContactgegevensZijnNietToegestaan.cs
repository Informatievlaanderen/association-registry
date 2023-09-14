namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class MeerderePrimaireContactgegevensZijnNietToegestaan : DomainException
{
    public MeerderePrimaireContactgegevensZijnNietToegestaan(string type) : base($"Er mag maar één {type} contactgegeven aangeduid zijn als primair.")
    {
    }

    protected MeerderePrimaireContactgegevensZijnNietToegestaan(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
