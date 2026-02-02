using MailContainerTest.Configuration;
using MailContainerTest.Data;
using MailContainerTest.Types;

namespace MailContainerTest.Services
{
    public class DataStoreFactory : IDataStoreFactory
    {
        private readonly DataStoreOptions _options;

        public DataStoreFactory(DataStoreOptions options)
        {
            _options = options;
        }

        public IMailContainerDataStore CreateDataStore()
        {
            if (_options.DataStoreType == DataStoreType.Backup)
            {
                return new BackupMailContainerDataStore();
            }

            return new MailContainerDataStore();
        }
    }
}
