using Microsoft.AspNetCore.Mvc;
using Moq;
using MediatR;
using WebApi.Controllers;
using Application.Features.Bio.DTOs;
using Application.Features.Bio.Queries;
using Application.Features.Bio.Commands;

namespace Tests.Controllers;

public class BioControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly BioController _controller;

    public BioControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new BioController(_mockMediator.Object);
    }

    [Fact]
    public async Task GetBio_WhenBioExists_ShouldReturnOkWithBio()
    {
        // Arrange
        var bioDto = new BioDto
        {
            Id = 1,
            FullName = "John Doe",
            Title = "Software Developer",
            Summary = "Experienced developer",
            Email = "john@email.com",
            Phone = "+1234567890",
            Location = "New York, NY",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<GetBioQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(bioDto);

        // Act
        var result = await _controller.GetBio();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBio = Assert.IsType<BioDto>(okResult.Value);
        Assert.Equal(1, returnedBio.Id);
        Assert.Equal("John Doe", returnedBio.FullName);
        Assert.Equal("Software Developer", returnedBio.Title);
        Assert.Equal("john@email.com", returnedBio.Email);

        _mockMediator.Verify(x => x.Send(It.IsAny<GetBioQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBio_WhenBioDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        _mockMediator.Setup(x => x.Send(It.IsAny<GetBioQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((BioDto?)null);

        // Act
        var result = await _controller.GetBio();

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
        _mockMediator.Verify(x => x.Send(It.IsAny<GetBioQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateBio_WithValidData_ShouldReturnCreatedResult()
    {
        // Arrange
        var createBioDto = new CreateBioDto
        {
            FullName = "Jane Smith",
            Title = "Product Manager",
            Summary = "Experienced PM",
            Email = "jane@email.com",
            Phone = "+9876543210",
            Location = "San Francisco, CA"
        };

        var createdBioDto = new BioDto
        {
            Id = 1,
            FullName = createBioDto.FullName,
            Title = createBioDto.Title,
            Summary = createBioDto.Summary,
            Email = createBioDto.Email,
            Phone = createBioDto.Phone,
            Location = createBioDto.Location,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<CreateBioCommand>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(createdBioDto);

        // Act
        var result = await _controller.CreateBio(createBioDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedBio = Assert.IsType<BioDto>(createdResult.Value);
        
        Assert.Equal(1, returnedBio.Id);
        Assert.Equal("Jane Smith", returnedBio.FullName);
        Assert.Equal("Product Manager", returnedBio.Title);
        Assert.Equal("jane@email.com", returnedBio.Email);

        Assert.Equal(nameof(_controller.GetBio), createdResult.ActionName);
        Assert.Equal(1, createdResult.RouteValues?["id"]);

        _mockMediator.Verify(x => x.Send(It.Is<CreateBioCommand>(c => 
            c.BioDto.FullName == "Jane Smith" &&
            c.BioDto.Email == "jane@email.com"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateBio_WithMinimalData_ShouldReturnCreatedResult()
    {
        // Arrange
        var createBioDto = new CreateBioDto
        {
            FullName = "Minimal User",
            Title = "Developer",
            Summary = "Basic summary",
            Email = "minimal@email.com"
        };

        var createdBioDto = new BioDto
        {
            Id = 1,
            FullName = createBioDto.FullName,
            Title = createBioDto.Title,
            Summary = createBioDto.Summary,
            Email = createBioDto.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<CreateBioCommand>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(createdBioDto);

        // Act
        var result = await _controller.CreateBio(createBioDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedBio = Assert.IsType<BioDto>(createdResult.Value);
        
        Assert.Equal("Minimal User", returnedBio.FullName);
        Assert.Equal("minimal@email.com", returnedBio.Email);
        Assert.Null(returnedBio.Phone);
        Assert.Null(returnedBio.Location);
    }

    [Fact]
    public async Task UpdateBio_WithValidData_ShouldReturnOkResult()
    {
        // Arrange
        var bioId = 1;
        var updateBioDto = new UpdateBioDto
        {
            FullName = "Updated Name",
            Title = "Updated Title",
            Summary = "Updated Summary",
            Email = "updated@email.com",
            Phone = "+5555555555",
            Location = "Updated Location"
        };

        var updatedBioDto = new BioDto
        {
            Id = bioId,
            FullName = updateBioDto.FullName,
            Title = updateBioDto.Title,
            Summary = updateBioDto.Summary,
            Email = updateBioDto.Email,
            Phone = updateBioDto.Phone,
            Location = updateBioDto.Location,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<UpdateBioCommand>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(updatedBioDto);

        // Act
        var result = await _controller.UpdateBio(bioId, updateBioDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBio = Assert.IsType<BioDto>(okResult.Value);
        
        Assert.Equal(bioId, returnedBio.Id);
        Assert.Equal("Updated Name", returnedBio.FullName);
        Assert.Equal("Updated Title", returnedBio.Title);
        Assert.Equal("updated@email.com", returnedBio.Email);

        _mockMediator.Verify(x => x.Send(It.Is<UpdateBioCommand>(c => 
            c.BioDto.Id == bioId &&
            c.BioDto.FullName == "Updated Name" &&
            c.BioDto.Email == "updated@email.com"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBio_WhenBioNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var bioId = 999;
        var updateBioDto = new UpdateBioDto
        {
            FullName = "Non-existent Bio",
            Title = "Title",
            Summary = "Summary",
            Email = "test@email.com"
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<UpdateBioCommand>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new ArgumentException("Bio not found"));

        // Act
        var result = await _controller.UpdateBio(bioId, updateBioDto);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);

        _mockMediator.Verify(x => x.Send(It.Is<UpdateBioCommand>(c => 
            c.BioDto.Id == bioId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBio_ShouldSetIdFromRouteParameter()
    {
        // Arrange
        var bioId = 42;
        var updateBioDto = new UpdateBioDto
        {
            Id = 0, // Should be overridden by route parameter
            FullName = "Test User",
            Title = "Test Title",
            Summary = "Test Summary",
            Email = "test@email.com"
        };

        var updatedBioDto = new BioDto
        {
            Id = bioId,
            FullName = updateBioDto.FullName,
            Title = updateBioDto.Title,
            Summary = updateBioDto.Summary,
            Email = updateBioDto.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<UpdateBioCommand>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(updatedBioDto);

        // Act
        await _controller.UpdateBio(bioId, updateBioDto);

        // Assert
        Assert.Equal(bioId, updateBioDto.Id); // Should be set by controller

        _mockMediator.Verify(x => x.Send(It.Is<UpdateBioCommand>(c => 
            c.BioDto.Id == bioId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Valid Name")]
    public async Task CreateBio_ShouldAcceptVariousNameFormats(string fullName)
    {
        // Arrange
        var createBioDto = new CreateBioDto
        {
            FullName = fullName,
            Title = "Developer",
            Summary = "Summary",
            Email = "test@email.com"
        };

        var createdBioDto = new BioDto
        {
            Id = 1,
            FullName = fullName,
            Title = createBioDto.Title,
            Summary = createBioDto.Summary,
            Email = createBioDto.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<CreateBioCommand>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(createdBioDto);

        // Act
        var result = await _controller.CreateBio(createBioDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedBio = Assert.IsType<BioDto>(createdResult.Value);
        Assert.Equal(fullName, returnedBio.FullName);
    }
}