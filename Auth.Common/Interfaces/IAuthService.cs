using Auth.Common.Dto;

namespace Auth.Common.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> Login(AuthRequest request);

    Task<AuthResponse> Register(RegisterRequest request);

    Task Revoke(string userEmail);

    Task<Token> RefreshToken(Token tokenModel);
}