using Domain.Entities;

namespace Application.Common.Interfaces;

public interface ITokenGenerator
{
    string GenerateToken(User user, out DateTime expiresAt);
}
