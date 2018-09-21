namespace DNT.IDP.Models
{
    public class OperationalStoreOptions
    {
        public bool EnableTokenCleanup { get; set; } = false;
        public int TokenCleanupInterval { get; set; } = 3600;
        public int TokenCleanupBatchSize { get; set; } = 100;
    }
}