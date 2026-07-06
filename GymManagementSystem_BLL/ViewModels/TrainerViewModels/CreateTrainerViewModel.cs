using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using GymManagementSystem_DAL.Entities.Enums;

namespace GymManagementSystem_BLL.ViewModels.TrainerViewModels
{
    public class CreateTrainerViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 100 characters")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^03\d{9}$", ErrorMessage = "Phone must be a valid Pakistani mobile number (03XXXXXXXXX)")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Building number is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Building number must be greater than 0")]
        public int BuildingNumber { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "City must be between 2 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City can only contain letters and spaces")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "Street is required")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Street must be between 1 and 30 characters")]
        public string Street { get; set; } = null!;

        [Required(ErrorMessage = "Specialty is required")]
        [EnumDataType(typeof(Specialties))]
        public Specialties Specialties { get; set; }
    }
}
