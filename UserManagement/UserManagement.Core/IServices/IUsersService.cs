using UserManagement.Core.DTO;

namespace UserManagement.Core.IServices;

public interface IUsersService
{
  Task<AuthenticationResponse?> Login(LoginRequest loginRequest);
  Task<AuthenticationResponse?> Register(RegisterRequest registerRequest);
}
