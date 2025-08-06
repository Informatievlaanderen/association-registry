namespace AssociationRegistry.Framework;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using DecentraalBeheer.Vereniging.Exceptions;

public static class OrThrowExtension
{
    public static OrThrowExtensionHelper<TVereniging> OrWhen<TVereniging>(this Task<TVereniging> source, Type type)
        => new(source, type);

    public static OrThrowExtensionHelper<TVereniging> OrWhenUnsupportedOperationForType<TVereniging>(this Task<TVereniging> source)
        => OrWhen(source, typeof(ActieIsNietToegestaanVoorVerenigingstype));

    public class OrThrowExtensionHelper<TVereniging>
    {
        private readonly Task<TVereniging> _vereniging;
        private readonly Type _type;

        public OrThrowExtensionHelper(Task<TVereniging> vereniging, Type type)
        {
            _vereniging = vereniging;
            _type = type;
        }

        public async Task<TVereniging> Throw<TException>() where TException : DomainException, new()
        {
            try
            {
                return await _vereniging;
            }
            catch (Exception ex)
            {
                if (ex.GetType() == _type)
                    throw new TException();

                throw;
            }
        }
    }
}
