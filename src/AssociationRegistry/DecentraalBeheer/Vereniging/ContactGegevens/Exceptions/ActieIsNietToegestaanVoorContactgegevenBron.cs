namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
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
