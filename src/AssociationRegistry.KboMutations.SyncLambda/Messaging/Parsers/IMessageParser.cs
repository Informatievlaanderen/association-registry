namespace AssociationRegistry.KboMutations.SyncLambda.Messaging.Parsers;

internal interface IMessageParser
{
    MagdaEnvelope ToEnvelope();
}
