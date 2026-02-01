using MailContainerTest.Types;

namespace MailContainerTest.Services
{
    public interface IMailValidationService
    {
        bool IsMailTransferValid(MailContainer sourceMailContainer, MailContainer destinationMailContainer, MakeMailTransferRequest request);
    }
}
