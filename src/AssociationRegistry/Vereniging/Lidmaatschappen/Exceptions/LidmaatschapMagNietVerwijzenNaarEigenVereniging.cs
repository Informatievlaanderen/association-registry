namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

public class LidmaatschapMagNietVerwijzenNaarEigenVereniging : DomainException
{
    public LidmaatschapMagNietVerwijzenNaarEigenVereniging() : base(ExceptionMessages.LidmaatschapMagNietVerwijzenNaarEigenVereniging)
    {
    }

    protected LidmaatschapMagNietVerwijzenNaarEigenVereniging(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
