using System.ComponentModel.DataAnnotations;

namespace SendEmailService.Models
{
    public class UserLoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
