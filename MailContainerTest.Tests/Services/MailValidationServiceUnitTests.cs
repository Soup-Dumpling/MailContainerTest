using FluentAssertions;
using MailContainerTest.Services;
using MailContainerTest.Types;
using Xunit;

namespace MailContainerTest.Tests.Services
{
    public class MailValidationServiceUnitTests
    {
        private readonly MailValidationService mailValidationService;

        public MailValidationServiceUnitTests()
        {
            mailValidationService = new MailValidationService();
        }

        public class InvalidMailValidationScenarios : TheoryData<MailContainer, MailContainer, MakeMailTransferRequest>
        {
            public InvalidMailValidationScenarios()
            {
                // Arrange
                // Scenario: Source mail container is null
                Add(null, new MailContainer(), new MakeMailTransferRequest());
                // Scenario: Destination mail container is null
                Add(new MailContainer(), null, new MakeMailTransferRequest());
                // Scenario: No mail containers found
                Add(null, null, new MakeMailTransferRequest());
                // Scenario: Self transfer
                Add(new MailContainer() { MailContainerNumber = "123" }, new MailContainer() { MailContainerNumber = "123" }, new MakeMailTransferRequest());
                // Scenario: Insufficient stock/capacity in source mail container
                Add(
                    new MailContainer() { MailContainerNumber = "123", Capacity = 3, Status = MailContainerStatus.Operational, AllowedMailType = AllowedMailType.StandardLetter },
                    new MailContainer() { MailContainerNumber = "456", Capacity = 10, Status = MailContainerStatus.Operational, AllowedMailType = AllowedMailType.StandardLetter },
                    new MakeMailTransferRequest() { SourceMailContainerNumber = "123", DestinationMailContainerNumber = "456", NumberOfMailItems = 5, TransferDate = DateTime.UtcNow, MailType = MailType.StandardLetter }
                    );
                // Scenario: Source mail container is OutOfService
                Add(
                    new MailContainer() { Status = MailContainerStatus.OutOfService },
                    new MailContainer() { Status = MailContainerStatus.Operational },
                    new MakeMailTransferRequest()
                    );
                // Scenario: Destination mail container is NoTransfersIn
                Add(
                    new MailContainer() { Status = MailContainerStatus.Operational },
                    new MailContainer() { Status = MailContainerStatus.NoTransfersIn },
                    new MakeMailTransferRequest()
                    );
                // Scenario: Destination mail container is OutOfService
                Add(
                    new MailContainer() { Status = MailContainerStatus.Operational },
                    new MailContainer() { Status = MailContainerStatus.OutOfService },
                    new MakeMailTransferRequest()
                    );
                // Scenario: Source mail container and destination mail container are subscribed to different mail types. Both containers are not subscribed to the mail type given in the MakeMailTransferRequest
                Add(
                    new MailContainer() { MailContainerNumber = "123", Capacity = 3, Status = MailContainerStatus.Operational, AllowedMailType = AllowedMailType.StandardLetter },
                    new MailContainer() { MailContainerNumber = "456", Capacity = 10, Status = MailContainerStatus.Operational, AllowedMailType = AllowedMailType.LargeLetter },
                    new MakeMailTransferRequest() { SourceMailContainerNumber = "123", DestinationMailContainerNumber = "456", NumberOfMailItems = 2, TransferDate = DateTime.UtcNow, MailType = MailType.SmallParcel }
                    );
            }
        }

        [Theory]
        [ClassData(typeof(InvalidMailValidationScenarios))]
        public void IsMailTransferValid_WhenScenariosAreInvalid_ShouldReturnFalse(MailContainer sourceMailContainer, MailContainer destinationMailContainer, MakeMailTransferRequest request)
        {
            // Act
            var result = mailValidationService.IsMailTransferValid(sourceMailContainer, destinationMailContainer, request);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsMailTransferValid_WhenScenarioIsValid_ShouldReturnTrue()
        {
            // Arrange
            var sourceMailContainer = new MailContainer()
            {
                MailContainerNumber = "123",
                Capacity = 7,
                Status = MailContainerStatus.Operational,
                AllowedMailType = AllowedMailType.LargeLetter,
            };
            var destinationMailContainer = new MailContainer() 
            {
                MailContainerNumber = "456",
                Capacity = 15,
                Status = MailContainerStatus.Operational,
                AllowedMailType= AllowedMailType.LargeLetter,
            };
            var request = new MakeMailTransferRequest()
            {
                SourceMailContainerNumber = sourceMailContainer.MailContainerNumber,
                DestinationMailContainerNumber = destinationMailContainer.MailContainerNumber,
                NumberOfMailItems = 5,
                TransferDate = DateTime.UtcNow,
                MailType = MailType.LargeLetter,
            };

            // Act
            var result = mailValidationService.IsMailTransferValid(sourceMailContainer, destinationMailContainer, request);

            // Assert
            result.Should().BeTrue();
        }
    }
}
