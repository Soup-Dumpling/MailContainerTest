using MailContainerTest.Configuration;
using MailContainerTest.Data;
using MailContainerTest.Types;

namespace MailContainerTest.Services
{
    public class DataStoreFactory(DataStoreOptions options) : IDataStoreFactory
    {
        private readonly DataStoreOptions _options = options;

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
