namespace AssociationRegistry.Admin.Api.Verenigingen.VCodes.Exceptions;

public class InvalidVCodeLength : InvalidVCode
{
    public InvalidVCodeLength() : base("VCode must be of length 7")
    {
    }
}