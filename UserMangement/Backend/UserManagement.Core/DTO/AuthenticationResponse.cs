namespace UserManagement.Core.DTO;
public class AuthenticationResponse
{
    public Guid UserID { get; set; }
    public string? Email { get; set; }
    public string? PersonName { get; set; }
    public string? Gender { get; set; }
    public string? Token { get; set; }
    public bool Sucess { get; set; }

}