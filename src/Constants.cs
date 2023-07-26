namespace LDAPSyncTool 
{
    public static class Constants
    {
        public const  string CONFIG_FILE_NAME = "LDAPSyncTool.yaml";
        public const int DEFAULT_PAGE_SIZE = 100;
        public const int DEFAULT_BATCH_SIZE = 100;
        public const string IVIVA_SYNC_SERVICE = "System/LDAPSyncUsers";
        public const string IVIVA_STOP_SYNC_SERVICE = "System/LDAPSyncCompleted";
    }
}