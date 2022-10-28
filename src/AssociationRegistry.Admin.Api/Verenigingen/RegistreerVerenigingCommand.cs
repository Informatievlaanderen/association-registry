namespace AssociationRegistry.Admin.Api.Verenigingen;

using MediatR;

public record RegistreerVerenigingCommand(string Naam) : IRequest<Unit>;
