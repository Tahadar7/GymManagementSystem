using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GymManagementSystem_BLL.ViewModels.CategoryViewModels
{
    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 20 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Category name must contain only letters and spaces")]
        public string CategoryName { get; set; } = null!;
    }
}
