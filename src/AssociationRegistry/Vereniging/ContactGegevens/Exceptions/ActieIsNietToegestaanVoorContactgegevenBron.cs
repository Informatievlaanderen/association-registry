namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class ActieIsNietToegestaanVoorContactgegevenBron : DomainException
{
    public ActieIsNietToegestaanVoorContactgegevenBron() : base(ExceptionMessages.UnsupportedOperationForContactgegevenBron)
    {
    }

    protected ActieIsNietToegestaanVoorContactgegevenBron(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
