namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class VertegenwoordigerIsNietGekend : DomainException
{
    public VertegenwoordigerIsNietGekend(
        string vertegenwoordigerId) : base(
        $"Vertegenwoordiger met vertegenwoordigerId '{vertegenwoordigerId}' is niet gekend.")
    {
    }

    protected VertegenwoordigerIsNietGekend(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
