using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET - INDEX
        public async Task<IActionResult> Index()
        {
            var subCategories = await _db.SubCategory.Include(s => s.Category).ToListAsync();
            return View(subCategories);
        }

        //GET - CREATE
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if(ModelState.IsValid)
            {
                var isSubCategoryExists = _db.SubCategory.Include(s => s.Category)
                    .Where(s => s.Name == model.SubCategory.Name && s.CategoryId == model.SubCategory.CategoryId);

                if (isSubCategoryExists.Count() > 0)
                {
                    StatusMessage = "Error: SubCategory exists under " + isSubCategoryExists.First().Category.Name + " category. Please use another name";
                } else
                {
                    _db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            SubCategoryAndCategoryViewModel vmodel = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync(),
                StatusMessage = StatusMessage
            };

            return View(vmodel);

        }

        //GET - Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subcategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);

            if (subcategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel vmodel = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subcategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(vmodel);

        }

        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);

            if (subcategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subcategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync(),
                StatusMessage = StatusMessage
            };

            return View(model);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isSubCategoryExists = _db.SubCategory.Include(s => s.Category)
                    .Where(s => s.Name == model.SubCategory.Name && s.CategoryId == model.SubCategory.CategoryId);

                if (isSubCategoryExists.Count() > 0)
                {
                    StatusMessage = "Error: SubCategory exists under " + isSubCategoryExists.First().Category.Name + " category. Please use another name";
                }
                else
                {
                    var subCategporyFromDb = await _db.SubCategory.FindAsync(id);
                    if (subCategporyFromDb == null)
                    {
                        subCategporyFromDb = new SubCategory();
                    }
                    subCategporyFromDb.Name = model.SubCategory.Name;
                    subCategporyFromDb.CategoryId = model.SubCategory.CategoryId;

                    await _db.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }

            SubCategoryAndCategoryViewModel vmodel = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync(),
                StatusMessage = StatusMessage
            };

            return View(vmodel);

        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await (from subCategory in _db.SubCategory
                             where subCategory.CategoryId == id
                             select subCategory).ToListAsync();

            return Json(new SelectList(subCategories, "Id", "Name"));
        }


        //GET - DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcategory = await _db.SubCategory.FindAsync(id);

            if (subcategory == null)
            {
                return NotFound();
            }

            return View(subcategory);
        }

        //POST - DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subcategory = await _db.SubCategory.FindAsync(id);

            if (subcategory == null)
            {
                return NotFound();
            }

            _db.SubCategory.Remove(subcategory);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}