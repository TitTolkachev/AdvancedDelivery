using Auth.Common.Dto;

namespace Auth.Common.Interfaces;

public interface IProfileService
{
    Task ChangeUserProfile(ProfileRequest request, string userEmail);

    Task<ProfileResponse> GetUserProfile(string userEmail);

    Task DeleteUserProfile(string userEmail);
}