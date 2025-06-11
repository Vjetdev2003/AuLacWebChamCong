using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuLacChamCong.DataApi
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
        public string? Headline { get; set; }
        public int PsnPrkID { get; set; }
    }
}
