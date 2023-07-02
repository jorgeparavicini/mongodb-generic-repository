using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CoreUnitTests.Infrastructure;
using CoreUnitTests.Infrastructure.Model;
using FluentAssertions;
using MongoDbGenericRepository.DataAccess.Read;
using Moq;
using Xunit;

namespace CoreUnitTests.KeyedReadOnlyMongoRepositoryTests;

public class CountAsyncTests : TestKeyedReadOnlyMongoRepositoryContext<Guid>
{
    private readonly Expression<Func<TestDocument, bool>> filter = document => document.SomeContent == "SomeContent";

    [Fact]
    public async Task WithFilter_GetsOne()
    {
        // Arrange
        var count = Fixture.Create<long>();

        SetupReader(count);

        // Act
        var result = await Sut.CountAsync(filter);

        // Assert
        result.Should().Be(count);
        Reader.Verify(
            x => x.CountAsync<TestDocument, Guid>(filter, null, CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task WithFilterAndCancellationToken_GetsOne()
    {
        // Arrange
        var count = Fixture.Create<long>();
        var token = new CancellationToken(true);

        SetupReader(count);

        // Act
        var result = await Sut.CountAsync(filter, token);

        // Assert
        result.Should().Be(count);
        Reader.Verify(
            x => x.CountAsync<TestDocument, Guid>(filter, null, token),
            Times.Once);
    }

    [Fact]
    public async Task WithFilterAndPartitionKey_GetsOne()
    {
        // Arrange
        var count = Fixture.Create<long>();
        var partitionKey = Fixture.Create<string>();

        SetupReader(count);

        // Act
        var result = await Sut.CountAsync(filter, partitionKey);

        // Assert
        result.Should().Be(count);
        Reader.Verify(
            x => x.CountAsync<TestDocument, Guid>(filter, partitionKey, CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task WithFilterAndPartitionKeyAndCancellationToken_GetsOne()
    {
        // Arrange
        var count = Fixture.Create<long>();
        var partitionKey = Fixture.Create<string>();
        var token = new CancellationToken(true);

        SetupReader(count);

        // Act
        var result = await Sut.CountAsync(filter, partitionKey, token);

        // Assert
        result.Should().Be(count);
        Reader.Verify(
            x => x.CountAsync<TestDocument, Guid>(filter, partitionKey, token),
            Times.Once);
    }

    private void SetupReader(long count)
    {
        Reader = new Mock<IMongoDbReader>();
        Reader
            .Setup(
                x => x.CountAsync<TestDocument, Guid>(
                    It.IsAny<Expression<Func<TestDocument, bool>>>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(count);
    }
}
