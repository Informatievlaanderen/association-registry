namespace AssociationRegistry.Vereniging;

using System;
using MediatR;

public record RegistreerVerenigingCommand(
    string Naam,
    string? KorteNaam,
    string? KorteBeschrijving,
    DateOnly? Startdatum,
    string? KboNummber,
    IEnumerable<RegistreerVerenigingCommand.ContactInfo>? Contacten) : IRequest<Unit>
{
    public record ContactInfo(
        string? Contactnaam,
        string? Email,
        string? Telefoon,
        string? Website,
        string? SocialMedia);
}
