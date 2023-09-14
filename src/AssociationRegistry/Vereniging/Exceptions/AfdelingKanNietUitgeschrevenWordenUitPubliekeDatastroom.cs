namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class AfdelingKanNietUitgeschrevenWordenUitPubliekeDatastroom : DomainException
{
    public AfdelingKanNietUitgeschrevenWordenUitPubliekeDatastroom() : base(ExceptionMessages.AfdelingCanNotBeUnsubscribedFromPubliekeDatastroom)
    {
    }

    protected AfdelingKanNietUitgeschrevenWordenUitPubliekeDatastroom(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
