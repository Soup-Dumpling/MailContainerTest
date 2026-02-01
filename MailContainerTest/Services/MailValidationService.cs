using MailContainerTest.Types;

namespace MailContainerTest.Services
{
    public class MailValidationService : IMailValidationService
    {
        public bool IsMailTransferValid(MailContainer sourceMailContainer, MailContainer destinationMailContainer, MakeMailTransferRequest request)
        {
            if (sourceMailContainer == null || 
                destinationMailContainer == null || 
                sourceMailContainer.MailContainerNumber == destinationMailContainer.MailContainerNumber || 
                sourceMailContainer.Capacity < request.NumberOfMailItems || 
                sourceMailContainer.Status == MailContainerStatus.OutOfService || 
                destinationMailContainer.Status != MailContainerStatus.Operational)
            {
                return false;
            }

            /* Valid Source Mail Container and DestinationMailContainer
             * - Both source mail container and destination mail container exist, i.e. both are not null
             * - Edge case: Ensure that the source mail container and the destination mail container are not the same container
             * - The source mail container has mail capacity equal to or greater than the number of mail items needed to be transferred in the MakeMailTransferRequest
             * - The source mail container status is not OutOfService. It can either be Operational or NoTransfersIn (Specifically blocks the destination mail container from receiving more mail items)
             * - The destination mail container status is Operational
             */

            /* Each container can only hold one type of mail, e.g. StandardLetter, LargeLetter or SmallParcel
             * For each request mail type, check that both the source mail container and the destination mail container is subscribed to the correct AllowedMailType
             */

            return request.MailType switch
            {
                MailType.StandardLetter => IsMailTypeMailTransferValid(sourceMailContainer, destinationMailContainer, AllowedMailType.StandardLetter),
                MailType.LargeLetter => IsMailTypeMailTransferValid(sourceMailContainer, destinationMailContainer, AllowedMailType.LargeLetter),
                MailType.SmallParcel => IsMailTypeMailTransferValid(sourceMailContainer, destinationMailContainer, AllowedMailType.SmallParcel),
                _ => false
            };
        }

        private static bool IsMailTypeMailTransferValid(MailContainer sourceMailContainer, MailContainer destinationMailContainer, AllowedMailType allowedMailType)
        {
            return sourceMailContainer.AllowedMailType.HasFlag(allowedMailType) && destinationMailContainer.AllowedMailType.HasFlag(allowedMailType);
        }
    }
}
