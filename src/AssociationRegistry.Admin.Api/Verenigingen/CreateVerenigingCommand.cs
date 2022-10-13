namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Runtime.Serialization;
using MediatR;

[DataContract]
public record CreateVerenigingCommand([property: DataMember] string Naam) : IRequest<Unit>;
