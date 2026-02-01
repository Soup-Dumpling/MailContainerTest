using FluentAssertions;
using MailContainerTest.Configuration;
using MailContainerTest.Data;
using MailContainerTest.Services;
using MailContainerTest.Types;
using Xunit;

namespace MailContainerTest.Tests.Services
{
    public class DataStoreFactoryUnitTests
    {
        private static DataStoreFactory CreateSUT(DataStoreType dataStoreType) 
        {
            var dataStoreOptions = new DataStoreOptions { DataStoreType = dataStoreType };
            return new DataStoreFactory(dataStoreOptions);
        }

        [Theory]
        [InlineData(DataStoreType.Default, typeof(MailContainerDataStore))]
        [InlineData(DataStoreType.Backup, typeof(BackupMailContainerDataStore))]
        public void CreateDataStore_ShouldReturnExpectedDataStore(DataStoreType dataStoreType, Type expectedType) 
        {
            // Arrange
            var sut = CreateSUT(dataStoreType);

            // Act
            var result = sut.CreateDataStore();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(expectedType);
        }
    }
}
