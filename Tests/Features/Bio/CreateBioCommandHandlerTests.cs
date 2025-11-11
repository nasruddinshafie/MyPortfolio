using Moq;
using Application.Features.Bio.Commands;
using Application.Features.Bio.DTOs;
using Application.Common.Interfaces;
using Domain.Entities;

namespace Tests.Features.Bio;

public class CreateBioCommandHandlerTests
{
    private readonly Mock<IBioRepository> _mockRepository;
    private readonly CreateBioCommandHandler _handler;

    public CreateBioCommandHandlerTests()
    {
        _mockRepository = new Mock<IBioRepository>();
        _handler = new CreateBioCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_Bio_Successfully()
    {
        // Arrange
        var createBioDto = new CreateBioDto
        {
            FullName = "John Doe",
            Title = "Software Developer",
            Summary = "Experienced developer",
            DetailedDescription = "Detailed bio description",
            Email = "john@email.com",
            Phone = "+1234567890",
            Location = "New York, NY",
            LinkedInUrl = "https://linkedin.com/in/johndoe",
            GitHubUrl = "https://github.com/johndoe",
            WebsiteUrl = "https://johndoe.com",
            ProfileImageUrl = "https://johndoe.com/avatar.jpg"
        };

        var command = new CreateBioCommand(createBioDto);
        var createdBio = new Domain.Entities.Bio
        {
            Id = 1,
            FullName = createBioDto.FullName,
            Title = createBioDto.Title,
            Summary = createBioDto.Summary,
            DetailedDescription = createBioDto.DetailedDescription,
            Email = createBioDto.Email,
            Phone = createBioDto.Phone,
            Location = createBioDto.Location,
            LinkedInUrl = createBioDto.LinkedInUrl,
            GitHubUrl = createBioDto.GitHubUrl,
            WebsiteUrl = createBioDto.WebsiteUrl,
            ProfileImageUrl = createBioDto.ProfileImageUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.Bio>()))
                      .ReturnsAsync(createdBio);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John Doe", result.FullName);
        Assert.Equal("Software Developer", result.Title);
        Assert.Equal("Experienced developer", result.Summary);
        Assert.Equal("john@email.com", result.Email);
        Assert.Equal("+1234567890", result.Phone);
        Assert.Equal("https://linkedin.com/in/johndoe", result.LinkedInUrl);

        _mockRepository.Verify(x => x.CreateAsync(It.Is<Domain.Entities.Bio>(b => 
            b.FullName == "John Doe" &&
            b.Title == "Software Developer" &&
            b.Email == "john@email.com")), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Set_Timestamps_When_Creating_Bio()
    {
        // Arrange
        var createBioDto = new CreateBioDto
        {
            FullName = "Jane Smith",
            Title = "Product Manager",
            Summary = "Experienced PM",
            Email = "jane@email.com"
        };

        var command = new CreateBioCommand(createBioDto);
        Domain.Entities.Bio? capturedBio = null;

        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.Bio>()))
                      .Callback<Domain.Entities.Bio>(bio => capturedBio = bio)
                      .ReturnsAsync((Domain.Entities.Bio bio) => bio);

        var beforeCreate = DateTime.UtcNow;

        // Act
        await _handler.Handle(command, CancellationToken.None);
        var afterCreate = DateTime.UtcNow;

        // Assert
        Assert.NotNull(capturedBio);
        Assert.True(capturedBio.CreatedAt >= beforeCreate && capturedBio.CreatedAt <= afterCreate);
        Assert.True(capturedBio.UpdatedAt >= beforeCreate && capturedBio.UpdatedAt <= afterCreate);
        Assert.Equal(capturedBio.CreatedAt, capturedBio.UpdatedAt);

        _mockRepository.Verify(x => x.CreateAsync(It.IsAny<Domain.Entities.Bio>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Handle_Minimal_Bio_Data()
    {
        // Arrange
        var createBioDto = new CreateBioDto
        {
            FullName = "Minimal User",
            Title = "Developer",
            Summary = "Basic summary",
            Email = "minimal@email.com"
        };

        var command = new CreateBioCommand(createBioDto);
        var createdBio = new Domain.Entities.Bio
        {
            Id = 1,
            FullName = createBioDto.FullName,
            Title = createBioDto.Title,
            Summary = createBioDto.Summary,
            Email = createBioDto.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.Bio>()))
                      .ReturnsAsync(createdBio);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Minimal User", result.FullName);
        Assert.Equal("Developer", result.Title);
        Assert.Equal("Basic summary", result.Summary);
        Assert.Equal("minimal@email.com", result.Email);
        Assert.Null(result.Phone);
        Assert.Null(result.Location);
        Assert.Null(result.LinkedInUrl);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("John Doe")]
    public async Task Handle_Should_Accept_Various_FullName_Values(string fullName)
    {
        // Arrange
        var createBioDto = new CreateBioDto
        {
            FullName = fullName,
            Title = "Developer",
            Summary = "Summary",
            Email = "test@email.com"
        };

        var command = new CreateBioCommand(createBioDto);
        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.Bio>()))
                      .ReturnsAsync((Domain.Entities.Bio bio) => bio);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRepository.Verify(x => x.CreateAsync(It.Is<Domain.Entities.Bio>(b => 
            b.FullName == fullName)), Times.Once);
    }
}