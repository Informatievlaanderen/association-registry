namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions.Wegwijs;

using Resources;

[Serializable]
public class OrganisatieNietGevondenException : Exception
{
    public OrganisatieNietGevondenException(Exception? innerException)
        : base(string.Format(ExceptionMessages.OrganisatieNietGevondenException, innerException)) { }

    public OrganisatieNietGevondenException(string message)
        : base(string.Format(ExceptionMessages.OrganisatieNietGevondenException, message)) { }
}
