using MailContainerTest.Data;

namespace MailContainerTest.Services
{
    public interface IDataStoreFactory
    {
        IMailContainerDataStore CreateDataStore();
    }
}
