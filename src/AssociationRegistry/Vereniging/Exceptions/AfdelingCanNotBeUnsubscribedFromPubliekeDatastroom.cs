namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class AfdelingCanNotBeUnsubscribedFromPubliekeDatastroom : DomainException
{
    public AfdelingCanNotBeUnsubscribedFromPubliekeDatastroom() : base(ExceptionMessages.AfdelingCanNotBeUnsubscribedFromPubliekeDatastroom)
    {
    }

    protected AfdelingCanNotBeUnsubscribedFromPubliekeDatastroom(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
