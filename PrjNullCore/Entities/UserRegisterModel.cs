using System.ComponentModel.DataAnnotations;

namespace PrjNullCore.Entities
{
    public class UserRegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [MaxLength(150)]
        public string FirstName { get; set; }
    }
}
