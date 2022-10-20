namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MediatR;

[DataContract]
public record CreateVerenigingCommand(string Naam) : IRequest<Unit>
{
    /// <summary>Naam van de vereniging</summary>
    [DataMember(IsRequired = true)]
    [Required]
    public string Naam { get; init; } = Naam;
}
