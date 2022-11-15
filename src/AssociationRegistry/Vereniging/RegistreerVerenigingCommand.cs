namespace AssociationRegistry.Vereniging;

using System;
using MediatR;

public record RegistreerVerenigingCommand(
    string Naam,
    string? KorteNaam,
    string? KorteBeschrijving,
    DateOnly? Startdatum,
    string? KboNummber) : IRequest<Unit>;
