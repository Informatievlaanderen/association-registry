namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class OngeldigUrl : DomainException
{
    public OngeldigUrl()
        : base(ExceptionMessages.OngeldigUrl) { }

    protected OngeldigUrl(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
