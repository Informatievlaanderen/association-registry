namespace AssociationRegistry.Admin.Api.Verenigingen.VCodes.Exceptions;

public class InvalidVCodeFormat : InvalidVCode
{
    public InvalidVCodeFormat() : base("Format of VCode must be 'V000000'")
    {
    }
}