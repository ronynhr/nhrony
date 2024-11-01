
namespace ClearingAndForwarding.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; } // Store password hashes, not plain text
        public string Role { get; set; }
    }

    public class LoginViewModel
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }

    public class RegisterViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }


}
