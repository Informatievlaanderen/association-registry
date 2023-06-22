namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class IncompleteAdresId : DomainException
{
    public IncompleteAdresId() : base("Een adresId moet een adresbron en waarde bevatten.")
    {
    }

    protected IncompleteAdresId(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
