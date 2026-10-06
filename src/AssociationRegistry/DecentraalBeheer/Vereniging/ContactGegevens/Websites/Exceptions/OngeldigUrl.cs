namespace AssociationRegistry.DecentraalBeheer.Vereniging.Websites.Exceptions;

using System.Runtime.Serialization;
using AssociationRegistry.Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class OngeldigUrl : DomainException
{
    public OngeldigUrl()
        : base(ExceptionMessages.OngeldigUrl) { }

    protected OngeldigUrl(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
