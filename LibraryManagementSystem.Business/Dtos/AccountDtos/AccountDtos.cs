using System.ComponentModel.DataAnnotations;


namespace LibraryManagementSystem.Business.Dtos.AccountDtos
{
    // Define the account types 
    public enum AccountType { Borrower, Author }

    public class RegisterDto
    {
        [Required]
        [StringLength(50,MinimumLength = 2,ErrorMessage = "First name must be between 2 and 50 character.")]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 character.")]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Phone]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(100 ,MinimumLength = 8,ErrorMessage = "Password must have at least 8 characters")]
        [DataType(DataType.Password)] //give info for the ui Frameworks how to render the data
        public string Password { get; set; } = null!;

        [Required]
        [Compare("Password",ErrorMessage = "The password and confirm password don't match.")]
        [DataType(DataType.Password)] //give info for the ui Frameworks how to render the data
        public string ConfirmPassword { get; set; } = null!;


        [Required]
        public AccountType AccountType { get; set; } //Author , borrower

        public string? Biography { get; set; }
    }



    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string UserName { get; set; } = null!;

        
        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must have at least 8 characters")]
        [DataType(DataType.Password)] //give info for the ui Frameworks how to render the data
        public string Password { get; set; } = null!;
    }


    public class ChangePasswordDto
    {
        [Required]
        [DataType(DataType.Password)] //give info for the ui Frameworks how to render the data
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "New Password must have at least 8 characters.")]
        [DataType(DataType.Password)] //give info for the ui Frameworks how to render the data
        public string NewPassword { get; set; } = null!;

        [Required]
        [Compare("NewPassword", ErrorMessage = "The new password and confirm password don't match.")]
        [DataType(DataType.Password)] //give info for the ui Frameworks how to render the data
        public string ConfirmNewPassword { get; set; } = null!;
    }


    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "A password reset token is required.")]
        public string Token { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "New Password must have at least 8 characters.")]
        [DataType(DataType.Password)] //give info for the ui Frameworks how to render the data
        public string NewPassword { get; set; } = null!;

        [Required]
        [Compare("NewPassword", ErrorMessage = "The new password and confirm password don't match.")]
        [DataType(DataType.Password)] //give info for the ui Frameworks how to render the data
        public string ConfirmNewPassword { get; set; } = null!;
    }


    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }


    public class TokenDto
    {
        [Required]
        public string AccessToken  { get; set; } = null!;

        [Required]
        public string RefreshToken { get; set; } = null!;
    }

    public class LogoutDto
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }

}
