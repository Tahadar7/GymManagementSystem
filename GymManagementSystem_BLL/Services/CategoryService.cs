using GymManagementSystem_BLL.Interfaces;
using GymManagementSystem_BLL.ViewModels.CategoryViewModels;
using GymManagementSystem_DAL.Data.DBContexts;
using GymManagementSystem_DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem_BLL.Services
{
    public class CategoryService(
        GymDBContext context,
        ILogger<CategoryService> logger) : ICategoryService
    {
        public async Task<List<CategoryViewModel>> GetAllAsync()
        {
            return await context.Categories.AsNoTracking()
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();
        }

        public async Task<CategoryViewModel?> GetByIdAsync(int id)
        {
            var category = await context.Categories.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category is null) return null;

            return new CategoryViewModel
            {
                Id = category.Id,
                CategoryName = category.CategoryName
            };
        }

        public async Task<EditCategoryViewModel?> GetForEditAsync(int id)
        {
            var category = await context.Categories.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category is null) return null;

            return new EditCategoryViewModel
            {
                CategoryName = category.CategoryName
            };
        }

        public async Task<bool> CreateAsync(CreateCategoryViewModel model)
        {
            try
            {
                // Check if a category with the same name already exists
                var exists = await context.Categories
                    .AnyAsync(c => c.CategoryName == model.CategoryName);

                if (exists) return false;

                var category = new Category
                {
                    CategoryName = model.CategoryName
                };

                context.Categories.Add(category);
                return await context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create category {CategoryName}", model.CategoryName);
                return false;
            }
        }

        public async Task<bool> EditAsync(int id, EditCategoryViewModel model)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
                if (category is null) return false;

                // Check name isn't already taken by another category
                var nameTaken = await context.Categories
                    .AnyAsync(c => c.CategoryName == model.CategoryName && c.Id != id);

                if (nameTaken) return false;

                category.CategoryName = model.CategoryName;
                category.UpdatedAt = DateTime.Now;

                return await context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to edit category with Id {CategoryId}", id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
                if (category is null) return false;

                // Block deletion if any sessions use this category —
                // deleting would leave sessions with a dangling CategoryId
                var hasSessions = await context.Sessions.AnyAsync(s => s.CategoryId == id);
                if (hasSessions) return false;

                context.Categories.Remove(category);
                return await context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete category with Id {CategoryId}", id);
                return false;
            }
        }
    }
}