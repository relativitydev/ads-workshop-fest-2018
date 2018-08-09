using System;
using System.ComponentModel.DataAnnotations;

namespace OverrideCustomPage.Models
{
    public class BaseRdo
    {
        public int WorkspaceArtifactId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public String Name { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "This is not a valid phone format. [Example: 1234567890]")]
        public String Phone { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$", ErrorMessage = "This is not a valid email format. [Example: h1ello@123.com]")]
        public String Email { get; set; }
    }
}