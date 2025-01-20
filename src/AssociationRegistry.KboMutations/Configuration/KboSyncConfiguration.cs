namespace AssociationRegistry.KboMutations.Configuration;

public record KboSyncConfiguration
{
    public static string Section = "KboSync";

    public string? MutationFileBucketName { get; set; }
    public string MutationFileQueueUrl { get; set; }
    public string MutationFileDeadLetterQueueUrl { get; set; }
    public string SyncQueueUrl { get; set; }
    public string SyncDeadLetterQueueUrl { get; set; }

}