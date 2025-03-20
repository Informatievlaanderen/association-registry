namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class WijzigSubverenigingMoetMinstensEenVeldTeWijzigenHebben : DomainException
{

    public WijzigSubverenigingMoetMinstensEenVeldTeWijzigenHebben() : base(ExceptionMessages.WijzigSubverenigingMoetMinstensEenVeldTeWijzigenHebben)
    {
    }

    protected WijzigSubverenigingMoetMinstensEenVeldTeWijzigenHebben(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
