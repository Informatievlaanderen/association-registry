namespace AssociationRegistry.Integrations.Magda.Shared.Exceptions;

using Persoon.GeefPersoon;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class MagdaException : Exception
{
    public MagdaException(string magdaDienst, string message): base(string.Format(ExceptionMessages.MagdaException, magdaDienst, message))
    {
    }

    public MagdaException(Exception? innerException) : base(ExceptionMessages.MagdaException, innerException)
    {
    }

    public MagdaException(string? message) : base(message)
    {
    }

    public MagdaException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected MagdaException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    private MagdaException(UitzonderingType[] uitzonderingen): base(MagdaExceptionStringBuilder.Build(uitzonderingen))
    {
    }

    private MagdaException(Repertorium.RegistreerInschrijving0200.UitzonderingType[] uitzonderingen): base(MagdaExceptionStringBuilder.Build(uitzonderingen))
    {
    }

    public static MagdaException WithMagdaFout(string magdaDienst, string message)
        => new(magdaDienst, message);

    public static MagdaException WithUitzonderingen(Repertorium.RegistreerInschrijving0200.UitzonderingType[] uitzonderingen)
    {
        return new MagdaException(uitzonderingen);
    }

    public static MagdaException WithUitzonderingen(UitzonderingType[] uitzonderingen)
    {
        return new MagdaException(uitzonderingen);
    }

    public static Exception WithNonSuccessStatus(HttpResponseMessage response, string content)
    {
        return new MagdaException($"Magda returned non success status code \n{response}\n{content}");
    }
}
