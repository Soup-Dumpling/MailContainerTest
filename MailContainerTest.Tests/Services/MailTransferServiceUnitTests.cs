using FluentAssertions;
using MailContainerTest.Data;
using MailContainerTest.Services;
using MailContainerTest.Types;
using NSubstitute;
using Xunit;

namespace MailContainerTest.Tests.Services
{
    public class MailTransferServiceUnitTests
    {
        private readonly MailTransferService mailTransferService;
        private readonly IDataStoreFactory dataStoreFactoryMock = Substitute.For<IDataStoreFactory>();
        private readonly IMailValidationService mailValidationServiceMock = Substitute.For<IMailValidationService>();

        public MailTransferServiceUnitTests()
        {
            mailTransferService = new MailTransferService(dataStoreFactoryMock, mailValidationServiceMock);
        }

        [Fact]
        public void ValidMakeMailTransfer()
        {
            // Arrange
            var request = new MakeMailTransferRequest()
            {
                SourceMailContainerNumber = "123",
                DestinationMailContainerNumber = "456",
                NumberOfMailItems = 5,
                TransferDate = DateTime.UtcNow,
                MailType = MailType.StandardLetter,
            };
            var sourceMailContainerInitialCapacity = 10;
            var destinationMailContainerInitialCapacity = 30;

            var sourceMailContainer = new MailContainer()
            {
                MailContainerNumber = request.SourceMailContainerNumber,
                Capacity = sourceMailContainerInitialCapacity,
                Status = MailContainerStatus.Operational,
                AllowedMailType = AllowedMailType.StandardLetter,
            };

            var destinationMailContainer = new MailContainer()
            {
                MailContainerNumber = request.DestinationMailContainerNumber,
                Capacity = destinationMailContainerInitialCapacity,
                Status = MailContainerStatus.Operational,
                AllowedMailType = AllowedMailType.StandardLetter,
            };

            var dataStoreMock = Substitute.For<IMailContainerDataStore>();

            dataStoreFactoryMock.CreateDataStore().Returns(dataStoreMock);
            dataStoreMock.GetMailContainer(Arg.Any<string>()).Returns(sourceMailContainer, destinationMailContainer);
            mailValidationServiceMock.IsMailTransferValid(Arg.Any<MailContainer>(), Arg.Any<MailContainer>(), Arg.Any<MakeMailTransferRequest>()).Returns(true);

            // Act
            var result = mailTransferService.MakeMailTransfer(request);

            // Assert
            result.Success.Should().BeTrue();
            dataStoreFactoryMock.Received().CreateDataStore();
            dataStoreMock.Received().GetMailContainer(request.SourceMailContainerNumber);
            dataStoreMock.Received().GetMailContainer(request.DestinationMailContainerNumber);
            mailValidationServiceMock.Received().IsMailTransferValid(Arg.Is<MailContainer>(x => x.MailContainerNumber == sourceMailContainer.MailContainerNumber),
                Arg.Is<MailContainer>(x => x.MailContainerNumber == destinationMailContainer.MailContainerNumber),
                request);
            dataStoreMock.Received().UpdateMailContainer(sourceMailContainer);
            dataStoreMock.Received().UpdateMailContainer(destinationMailContainer);
            sourceMailContainer.Capacity.Should().Be(sourceMailContainerInitialCapacity - request.NumberOfMailItems);
            destinationMailContainer.Capacity.Should().Be(destinationMailContainerInitialCapacity + request.NumberOfMailItems);
        }

        [Fact]
        public void InvalidMakeMailTransfer()
        {
            // Arrange
            var request = new MakeMailTransferRequest()
            {
                SourceMailContainerNumber = "123",
                DestinationMailContainerNumber = "456",
                NumberOfMailItems = 5,
                TransferDate = DateTime.UtcNow,
                MailType = MailType.StandardLetter,
            };
            var sourceMailContainerInitialCapacity = 10;
            var destinationMailContainerInitialCapacity = 30;

            var sourceMailContainer = new MailContainer()
            {
                MailContainerNumber = request.SourceMailContainerNumber,
                Capacity = sourceMailContainerInitialCapacity,
                Status = MailContainerStatus.Operational,
                AllowedMailType = AllowedMailType.LargeLetter,
            };

            var destinationMailContainer = new MailContainer()
            {
                MailContainerNumber = request.DestinationMailContainerNumber,
                Capacity = destinationMailContainerInitialCapacity,
                Status = MailContainerStatus.NoTransfersIn,
                AllowedMailType = AllowedMailType.SmallParcel,
            };

            var dataStoreMock = Substitute.For<IMailContainerDataStore>();

            dataStoreFactoryMock.CreateDataStore().Returns(dataStoreMock);
            dataStoreMock.GetMailContainer(Arg.Any<string>()).Returns(sourceMailContainer, destinationMailContainer);
            mailValidationServiceMock.IsMailTransferValid(Arg.Any<MailContainer>(), Arg.Any<MailContainer>(), Arg.Any<MakeMailTransferRequest>()).Returns(false);

            // Act
            var result = mailTransferService.MakeMailTransfer(request);

            // Assert
            result.Success.Should().BeFalse();
            dataStoreFactoryMock.Received().CreateDataStore();
            dataStoreMock.Received().GetMailContainer(request.SourceMailContainerNumber);
            dataStoreMock.Received().GetMailContainer(request.DestinationMailContainerNumber);
            mailValidationServiceMock.Received().IsMailTransferValid(Arg.Is<MailContainer>(x => x.MailContainerNumber == sourceMailContainer.MailContainerNumber),
                Arg.Is<MailContainer>(x => x.MailContainerNumber == destinationMailContainer.MailContainerNumber),
                request);
            dataStoreMock.DidNotReceive().UpdateMailContainer(sourceMailContainer);
            dataStoreMock.DidNotReceive().UpdateMailContainer(destinationMailContainer);
            sourceMailContainer.Capacity.Should().Be(sourceMailContainerInitialCapacity);
            destinationMailContainer.Capacity.Should().Be(destinationMailContainerInitialCapacity);
        }
    }
}
