namespace AssociationRegistry.Vertegenwoordigers.Exceptions;

using System.Runtime.Serialization;

[Serializable]
public class DuplicateInszProvided : Exception
{
    public DuplicateInszProvided() : base("INSZ moet uniek zijn binnen de vereniging.")
    {
    }

    protected DuplicateInszProvided(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
