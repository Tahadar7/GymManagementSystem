using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GymManagementSystem_BLL.ViewModels.HealthRecordViewModels
{
    public class HealthRecordViewModel
    {
        [Required(ErrorMessage = "Height is required")]
        [Range(0.1, 300, ErrorMessage = "Height must be between 0.1 and 300 cm")]
        public decimal Height { get; set; }

        [Required(ErrorMessage = "Weight is required")]
        [Range(0.1, 1000, ErrorMessage = "Weight must be between 0.1 and 1000 kg")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Blood type is required")]
        [StringLength(3, ErrorMessage = "Blood type must be 3 characters or less")]
        public string BloodType { get; set; } = null!;

        public string? Note { get; set; }
    }
}
