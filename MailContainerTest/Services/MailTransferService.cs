using MailContainerTest.Types;

namespace MailContainerTest.Services
{
    public class MailTransferService : IMailTransferService
    {
        private readonly IDataStoreFactory _dataStoreFactory;
        private readonly IMailValidationService _mailValidationService;

        public MailTransferService(IDataStoreFactory dataStoreFactory, IMailValidationService mailValidationService)
        {
            _dataStoreFactory = dataStoreFactory;
            _mailValidationService = mailValidationService;
        }

        public MakeMailTransferResult MakeMailTransfer(MakeMailTransferRequest request)
        {
            var dataStore = _dataStoreFactory.CreateDataStore();
            var sourceMailContainer = dataStore.GetMailContainer(request.SourceMailContainerNumber);
            var destinationMailContainer = dataStore.GetMailContainer(request.DestinationMailContainerNumber);

            var result = new MakeMailTransferResult() { Success = false };

            if (_mailValidationService.IsMailTransferValid(sourceMailContainer, destinationMailContainer, request))
            {
                sourceMailContainer.Capacity -= request.NumberOfMailItems;
                dataStore.UpdateMailContainer(sourceMailContainer);
                destinationMailContainer.Capacity += request.NumberOfMailItems;
                dataStore.UpdateMailContainer(destinationMailContainer);
                result.Success = true;
            }
            return result;
        }
    }
}
