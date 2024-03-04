using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PrjNullCore.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name ="نام")]
        [MaxLength(150, ErrorMessage = "حداکثر طول {0} به میزان {1} کاراکتر می باشد.")]
        [Required(ErrorMessage = "ورود {0} الزامی می باشد.")]
        public String FirstName { get; set; }
    }
}
