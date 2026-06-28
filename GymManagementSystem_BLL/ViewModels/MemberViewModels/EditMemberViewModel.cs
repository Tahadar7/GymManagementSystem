using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GymManagementSystem_BLL.ViewModels.MemberViewModels
{
    public class EditMemberViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name must contain only letters and spaces")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 100 characters")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^03\d{9}$", ErrorMessage = "Phone must be a valid Pakistani mobile number (03XXXXXXXXX)")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Building number is required")]
        [Range(1, 1000, ErrorMessage = "Building number must be between 1 and 1000")]
        public int BuildingNumber { get; set; }

        [Required(ErrorMessage = "Street is required")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Street must be between 2 and 30 characters")]
        public string Street { get; set; } = null!;

        [Required(ErrorMessage = "City is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City must contain only letters and spaces")]
        public string City { get; set; } = null!;

        // optional only set if new photo otherwise keep the old photo
        public string? Photo { get; set; }
    }
}
