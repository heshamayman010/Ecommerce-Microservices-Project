using UserManagement.Core.Entities;

namespace UserManagement.Core.IRepository;

public interface IUsersRepository
{
  Task<ApplicationUser?> AddUser(ApplicationUser user);


  Task<ApplicationUser?> GetUserByEmailAndPassword(string? email, string? password);



  Task<ApplicationUser?> GetUserByUserID(Guid? userID);
}
