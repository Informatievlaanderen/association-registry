namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;

public interface IProbeerAdresTeMatchenCommandHandler
{
    Task Handle(ProbeerAdresTeMatchenCommand command, CancellationToken cancellationToken = default);
}
