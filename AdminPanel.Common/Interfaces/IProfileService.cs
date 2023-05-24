using AdminPanel.Common.Models;

namespace AdminPanel.Common.Interfaces;

public interface IProfileService
{
    Task<List<User>> GetUsers();
    Task<User> GetUserInfo(Guid id);
    Task DeleteUser(Guid id);
    Task ChangeUser(User user);
}