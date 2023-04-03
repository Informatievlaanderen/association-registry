namespace AssociationRegistry.Admin.Api.Constants;

using System.Runtime.Serialization;

[DataContract]
public enum RequestContactgegevenTypes
{
    Email = 1,
    Telefoon = 2,
    Website = 3,
    SocialMedia = 4,
}
