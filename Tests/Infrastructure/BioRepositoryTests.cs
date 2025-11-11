using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Infrastructure.Features.Bio.Repositories;
using Domain.Entities;

namespace Tests.Infrastructure;

public class BioRepositoryTests : IDisposable
{
    private readonly PortfolioDbContext _context;
    private readonly BioRepository _repository;

    public BioRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<PortfolioDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PortfolioDbContext(options);
        _repository = new BioRepository(_context);
    }

    [Fact]
    public async Task GetAsync_WhenNoBioExists_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAsync_WhenBioExists_ShouldReturnBio()
    {
        // Arrange
        var bio = new Domain.Entities.Bio
        {
            FullName = "John Doe",
            Title = "Software Developer",
            Summary = "Experienced developer",
            Email = "john@email.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Bios.Add(bio);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John Doe", result.FullName);
        Assert.Equal("Software Developer", result.Title);
        Assert.Equal("john@email.com", result.Email);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateBioSuccessfully()
    {
        // Arrange
        var bio = new Domain.Entities.Bio
        {
            FullName = "Jane Doe",
            Title = "Product Manager",
            Summary = "Experienced PM",
            Email = "jane@email.com",
            Phone = "+1234567890",
            Location = "San Francisco, CA",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.CreateAsync(bio);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Jane Doe", result.FullName);
        Assert.Equal("Product Manager", result.Title);
        Assert.Equal("+1234567890", result.Phone);

        var savedBio = await _context.Bios.FindAsync(result.Id);
        Assert.NotNull(savedBio);
        Assert.Equal("Jane Doe", savedBio.FullName);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateBioSuccessfully()
    {
        // Arrange
        var bio = new Domain.Entities.Bio
        {
            FullName = "Original Name",
            Title = "Original Title",
            Summary = "Original Summary",
            Email = "original@email.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Bios.Add(bio);
        await _context.SaveChangesAsync();

        // Modify the bio
        bio.FullName = "Updated Name";
        bio.Title = "Updated Title";
        bio.Email = "updated@email.com";
        bio.UpdatedAt = DateTime.UtcNow;

        // Act
        var result = await _repository.UpdateAsync(bio);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.FullName);
        Assert.Equal("Updated Title", result.Title);
        Assert.Equal("updated@email.com", result.Email);

        var updatedBio = await _context.Bios.FindAsync(result.Id);
        Assert.NotNull(updatedBio);
        Assert.Equal("Updated Name", updatedBio.FullName);
    }

    [Fact]
    public async Task DeleteAsync_WhenBioExists_ShouldDeleteBio()
    {
        // Arrange
        var bio = new Domain.Entities.Bio
        {
            FullName = "To Delete",
            Title = "Delete Title",
            Summary = "Delete Summary",
            Email = "delete@email.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Bios.Add(bio);
        await _context.SaveChangesAsync();
        var bioId = bio.Id;

        // Act
        await _repository.DeleteAsync(bioId);

        // Assert
        var deletedBio = await _context.Bios.FindAsync(bioId);
        Assert.Null(deletedBio);
    }

    [Fact]
    public async Task DeleteAsync_WhenBioDoesNotExist_ShouldNotThrow()
    {
        // Act & Assert
        await _repository.DeleteAsync(999); // Non-existent ID
        
        // Should not throw exception
        Assert.True(true);
    }

    [Fact]
    public async Task ExistsAsync_WhenNoBioExists_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.ExistsAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WhenBioExists_ShouldReturnTrue()
    {
        // Arrange
        var bio = new Domain.Entities.Bio
        {
            FullName = "Existing Bio",
            Title = "Existing Title",
            Summary = "Existing Summary",
            Email = "existing@email.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Bios.Add(bio);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GetAsync_WhenMultipleBiosExist_ShouldReturnFirst()
    {
        // Arrange
        var bio1 = new Domain.Entities.Bio
        {
            FullName = "First Bio",
            Title = "First Title",
            Summary = "First Summary",
            Email = "first@email.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var bio2 = new Domain.Entities.Bio
        {
            FullName = "Second Bio",
            Title = "Second Title",
            Summary = "Second Summary",
            Email = "second@email.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Bios.AddRange(bio1, bio2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("First Bio", result.FullName);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}