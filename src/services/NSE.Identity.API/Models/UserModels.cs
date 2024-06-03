using System.ComponentModel.DataAnnotations;

namespace NSE.Identity.API.Models;

public class UserLogin
{
    [Required(ErrorMessage = "The field {0} is required")]
    [EmailAddress(ErrorMessage = "The field {0} is invalid format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "The field {0} is required")]
    [StringLength(100, ErrorMessage = "The field {0} need to have between {2} and {1} characters", MinimumLength = 6)]
    public string Password { get; set; }
}

public class NewUser
{
    [Required(ErrorMessage = "The field {0} is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "The field {0} is required")]
    public string SocialNumber { get; set; }

    [Required(ErrorMessage = "The field {0} is required")]
    [EmailAddress(ErrorMessage = "The field {0} is invalid format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "The field {0} is required")]
    [StringLength(100, ErrorMessage = "The field {0} need to have between {2} and {1} characters", MinimumLength = 6)]
    public string Password { get; set; }

    [Required(ErrorMessage = "The field {0} is required")]
    [Compare("Password", ErrorMessage = "Password and Password Confirm must be the same.")]
    public string PasswordConfirm { get; set; }
}

public class UserLoginResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public double ExpiresIn { get; set; }
    public UserToken UserToken { get; set; }
}

public class UserToken
{
    public string Id { get; set; }
    public string Email { get; set; }
    public IEnumerable<UserClaim> Claims { get; set; }
}

public class UserClaim
{
    public string Value { get; set; }
    public string Type { get; set; }
}