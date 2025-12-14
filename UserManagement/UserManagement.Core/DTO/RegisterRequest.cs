namespace UserManagement.Core.DTO;
public class RegisterRequest {
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? PersonName { get; set; }

    public GenderOptions Gender {get;set;}


}