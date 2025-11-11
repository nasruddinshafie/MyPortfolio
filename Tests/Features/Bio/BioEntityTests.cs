using Domain.Entities;

namespace Tests.Features.Bio;

public class BioEntityTests
{
    [Fact]
    public void Bio_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var bio = new Domain.Entities.Bio();

        // Assert
        Assert.Equal(0, bio.Id);
        Assert.Equal(string.Empty, bio.FullName);
        Assert.Equal(string.Empty, bio.Title);
        Assert.Equal(string.Empty, bio.Summary);
        Assert.Null(bio.DetailedDescription);
        Assert.Equal(string.Empty, bio.Email);
        Assert.Null(bio.Phone);
        Assert.Null(bio.Location);
        Assert.Null(bio.LinkedInUrl);
        Assert.Null(bio.GitHubUrl);
        Assert.Null(bio.WebsiteUrl);
        Assert.Null(bio.ProfileImageUrl);
        Assert.Equal(default(DateTime), bio.CreatedAt);
        Assert.Equal(default(DateTime), bio.UpdatedAt);
    }

    [Fact]
    public void Bio_Should_Set_Properties_Correctly()
    {
        // Arrange
        var bio = new Domain.Entities.Bio();
        var now = DateTime.UtcNow;

        // Act
        bio.Id = 1;
        bio.FullName = "John Doe";
        bio.Title = "Software Developer";
        bio.Summary = "Experienced developer";
        bio.DetailedDescription = "Detailed bio description";
        bio.Email = "john.doe@email.com";
        bio.Phone = "+1234567890";
        bio.Location = "New York, NY";
        bio.LinkedInUrl = "https://linkedin.com/in/johndoe";
        bio.GitHubUrl = "https://github.com/johndoe";
        bio.WebsiteUrl = "https://johndoe.com";
        bio.ProfileImageUrl = "https://johndoe.com/avatar.jpg";
        bio.CreatedAt = now;
        bio.UpdatedAt = now;

        // Assert
        Assert.Equal(1, bio.Id);
        Assert.Equal("John Doe", bio.FullName);
        Assert.Equal("Software Developer", bio.Title);
        Assert.Equal("Experienced developer", bio.Summary);
        Assert.Equal("Detailed bio description", bio.DetailedDescription);
        Assert.Equal("john.doe@email.com", bio.Email);
        Assert.Equal("+1234567890", bio.Phone);
        Assert.Equal("New York, NY", bio.Location);
        Assert.Equal("https://linkedin.com/in/johndoe", bio.LinkedInUrl);
        Assert.Equal("https://github.com/johndoe", bio.GitHubUrl);
        Assert.Equal("https://johndoe.com", bio.WebsiteUrl);
        Assert.Equal("https://johndoe.com/avatar.jpg", bio.ProfileImageUrl);
        Assert.Equal(now, bio.CreatedAt);
        Assert.Equal(now, bio.UpdatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("John Doe")]
    [InlineData("A very long name that exceeds normal expectations")]
    public void Bio_FullName_Should_Accept_Various_Values(string fullName)
    {
        // Arrange
        var bio = new Domain.Entities.Bio();

        // Act
        bio.FullName = fullName;

        // Assert
        Assert.Equal(fullName, bio.FullName);
    }

    [Theory]
    [InlineData("john@email.com")]
    [InlineData("john.doe+test@example.org")]
    [InlineData("user123@domain.co.uk")]
    public void Bio_Email_Should_Accept_Valid_Email_Formats(string email)
    {
        // Arrange
        var bio = new Domain.Entities.Bio();

        // Act
        bio.Email = email;

        // Assert
        Assert.Equal(email, bio.Email);
    }
}