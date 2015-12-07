using Microsoft.WindowsAzure.Storage;

namespace thisistracer.DAL {
    public static class StorageConfigure {
        private static CloudStorageAccount _StorageAccount;

        public static CloudStorageAccount StorageAccount {
            get {
                if (_StorageAccount == null)
                    _StorageAccount = CloudStorageAccount.Parse(Util.Utils.GetAppConfigure("AzureStroageConnection"));
                return _StorageAccount;
            }
        }
    }
}
