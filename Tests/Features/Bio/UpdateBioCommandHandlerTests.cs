using Moq;
using Application.Features.Bio.Commands;
using Application.Features.Bio.DTOs;
using Application.Common.Interfaces;
using Domain.Entities;

namespace Tests.Features.Bio;

public class UpdateBioCommandHandlerTests
{
    private readonly Mock<IBioRepository> _mockRepository;
    private readonly UpdateBioCommandHandler _handler;

    public UpdateBioCommandHandlerTests()
    {
        _mockRepository = new Mock<IBioRepository>();
        _handler = new UpdateBioCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Update_Bio_Successfully()
    {
        // Arrange
        var existingBio = new Domain.Entities.Bio
        {
            Id = 1,
            FullName = "Original Name",
            Title = "Original Title",
            Summary = "Original Summary",
            Email = "original@email.com",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var updateBioDto = new UpdateBioDto
        {
            Id = 1,
            FullName = "Updated Name",
            Title = "Updated Title",
            Summary = "Updated Summary",
            Email = "updated@email.com",
            Phone = "+1234567890",
            Location = "San Francisco, CA"
        };

        var command = new UpdateBioCommand(updateBioDto);

        _mockRepository.Setup(x => x.GetAsync())
                      .ReturnsAsync(existingBio);

        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Bio>()))
                      .ReturnsAsync((Domain.Entities.Bio bio) => bio);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Updated Name", result.FullName);
        Assert.Equal("Updated Title", result.Title);
        Assert.Equal("Updated Summary", result.Summary);
        Assert.Equal("updated@email.com", result.Email);
        Assert.Equal("+1234567890", result.Phone);
        Assert.Equal("San Francisco, CA", result.Location);

        _mockRepository.Verify(x => x.GetAsync(), Times.Once);
        _mockRepository.Verify(x => x.UpdateAsync(It.Is<Domain.Entities.Bio>(b => 
            b.FullName == "Updated Name" &&
            b.Title == "Updated Title" &&
            b.Email == "updated@email.com")), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Bio_Not_Found()
    {
        // Arrange
        var updateBioDto = new UpdateBioDto
        {
            Id = 1,
            FullName = "Updated Name",
            Title = "Updated Title",
            Summary = "Updated Summary",
            Email = "updated@email.com"
        };

        var command = new UpdateBioCommand(updateBioDto);

        _mockRepository.Setup(x => x.GetAsync())
                      .ReturnsAsync((Domain.Entities.Bio?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _handler.Handle(command, CancellationToken.None));

        _mockRepository.Verify(x => x.GetAsync(), Times.Once);
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.Bio>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Update_UpdatedAt_Timestamp()
    {
        // Arrange
        var existingBio = new Domain.Entities.Bio
        {
            Id = 1,
            FullName = "Original Name",
            Title = "Original Title",
            Summary = "Original Summary",
            Email = "original@email.com",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var updateBioDto = new UpdateBioDto
        {
            Id = 1,
            FullName = "Updated Name",
            Title = "Updated Title",
            Summary = "Updated Summary",
            Email = "updated@email.com"
        };

        var command = new UpdateBioCommand(updateBioDto);
        var beforeUpdate = DateTime.UtcNow;

        _mockRepository.Setup(x => x.GetAsync())
                      .ReturnsAsync(existingBio);

        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Bio>()))
                      .ReturnsAsync((Domain.Entities.Bio bio) => bio);

        // Act
        await _handler.Handle(command, CancellationToken.None);
        var afterUpdate = DateTime.UtcNow;

        // Assert
        _mockRepository.Verify(x => x.UpdateAsync(It.Is<Domain.Entities.Bio>(b => 
            b.UpdatedAt >= beforeUpdate && b.UpdatedAt <= afterUpdate &&
            b.CreatedAt == existingBio.CreatedAt)), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Update_All_Fields_Including_Optional_Ones()
    {
        // Arrange
        var existingBio = new Domain.Entities.Bio
        {
            Id = 1,
            FullName = "Original Name",
            Title = "Original Title",
            Summary = "Original Summary",
            Email = "original@email.com",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var updateBioDto = new UpdateBioDto
        {
            Id = 1,
            FullName = "Complete Update",
            Title = "Senior Developer",
            Summary = "Comprehensive summary",
            DetailedDescription = "Detailed description here",
            Email = "complete@email.com",
            Phone = "+1234567890",
            Location = "Seattle, WA",
            LinkedInUrl = "https://linkedin.com/in/complete",
            GitHubUrl = "https://github.com/complete",
            WebsiteUrl = "https://complete.dev",
            ProfileImageUrl = "https://complete.dev/avatar.jpg"
        };

        var command = new UpdateBioCommand(updateBioDto);

        _mockRepository.Setup(x => x.GetAsync())
                      .ReturnsAsync(existingBio);

        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Bio>()))
                      .ReturnsAsync((Domain.Entities.Bio bio) => bio);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Complete Update", result.FullName);
        Assert.Equal("Senior Developer", result.Title);
        Assert.Equal("Detailed description here", result.DetailedDescription);
        Assert.Equal("complete@email.com", result.Email);
        Assert.Equal("+1234567890", result.Phone);
        Assert.Equal("Seattle, WA", result.Location);
        Assert.Equal("https://linkedin.com/in/complete", result.LinkedInUrl);
        Assert.Equal("https://github.com/complete", result.GitHubUrl);
        Assert.Equal("https://complete.dev", result.WebsiteUrl);
        Assert.Equal("https://complete.dev/avatar.jpg", result.ProfileImageUrl);
    }

    [Fact]
    public async Task Handle_Should_Allow_Null_Optional_Fields()
    {
        // Arrange
        var existingBio = new Domain.Entities.Bio
        {
            Id = 1,
            FullName = "Original Name",
            Title = "Original Title",
            Summary = "Original Summary",
            Email = "original@email.com",
            Phone = "+9876543210",
            Location = "Old Location",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var updateBioDto = new UpdateBioDto
        {
            Id = 1,
            FullName = "Updated Name",
            Title = "Updated Title",
            Summary = "Updated Summary",
            Email = "updated@email.com",
            Phone = null,
            Location = null
        };

        var command = new UpdateBioCommand(updateBioDto);

        _mockRepository.Setup(x => x.GetAsync())
                      .ReturnsAsync(existingBio);

        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Bio>()))
                      .ReturnsAsync((Domain.Entities.Bio bio) => bio);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Phone);
        Assert.Null(result.Location);
        Assert.Equal("Updated Name", result.FullName);
        Assert.Equal("updated@email.com", result.Email);
    }
}