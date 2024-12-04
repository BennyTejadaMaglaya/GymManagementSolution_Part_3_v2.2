using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymManagement.Data;
using GymManagement.Models;
using GymManagement.CustomControllers;

namespace GymManagement.Controllers
{
    public class ExerciseCategoryController : LookupsController
    {
        private readonly GymContext _context;

        public ExerciseCategoryController(GymContext context)
        {
            _context = context;
        }

        // GET: ExerciseCategory
        public async Task<IActionResult> Index()
        {
            var gymContext = _context.ExerciseCategories.Include(e => e.Exercise).Include(e => e.FitnessCategory);
            return Redirect(ViewData["returnURL"].ToString());
        }

        // GET: ExerciseCategory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exerciseCategory = await _context.ExerciseCategories
                .Include(e => e.Exercise)
                .Include(e => e.FitnessCategory)
                .FirstOrDefaultAsync(m => m.FitnessCategoryID == id);
            if (exerciseCategory == null)
            {
                return NotFound();
            }

            return View(exerciseCategory);
        }

        // GET: ExerciseCategory/Create
        public IActionResult Create()
        {
            ViewData["ExerciseID"] = new SelectList(_context.Exercises, "ID", "Name");
            ViewData["FitnessCategoryID"] = new SelectList(_context.FitnessCategories, "ID", "Category");
            return View();
        }

        // POST: ExerciseCategory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FitnessCategoryID,ExerciseID")] ExerciseCategory exerciseCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exerciseCategory);
                await _context.SaveChangesAsync();
                return Redirect(ViewData["returnURL"].ToString());
            }
            ViewData["ExerciseID"] = new SelectList(_context.Exercises, "ID", "Name", exerciseCategory.ExerciseID);
            ViewData["FitnessCategoryID"] = new SelectList(_context.FitnessCategories, "ID", "Category", exerciseCategory.FitnessCategoryID);
            return View(exerciseCategory);
        }

        // GET: ExerciseCategory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exerciseCategory = await _context.ExerciseCategories.FindAsync(id);
            if (exerciseCategory == null)
            {
                return NotFound();
            }
            ViewData["ExerciseID"] = new SelectList(_context.Exercises, "ID", "Name", exerciseCategory.ExerciseID);
            ViewData["FitnessCategoryID"] = new SelectList(_context.FitnessCategories, "ID", "Category", exerciseCategory.FitnessCategoryID);
            return View(exerciseCategory);
        }

        // POST: ExerciseCategory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FitnessCategoryID,ExerciseID")] ExerciseCategory exerciseCategory)
        {
            if (id != exerciseCategory.FitnessCategoryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exerciseCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExerciseCategoryExists(exerciseCategory.FitnessCategoryID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect(ViewData["returnURL"].ToString());
            }
            ViewData["ExerciseID"] = new SelectList(_context.Exercises, "ID", "Name", exerciseCategory.ExerciseID);
            ViewData["FitnessCategoryID"] = new SelectList(_context.FitnessCategories, "ID", "Category", exerciseCategory.FitnessCategoryID);
            return View(exerciseCategory);
        }

        // GET: ExerciseCategory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exerciseCategory = await _context.ExerciseCategories
                .Include(e => e.Exercise)
                .Include(e => e.FitnessCategory)
                .FirstOrDefaultAsync(m => m.FitnessCategoryID == id);
            if (exerciseCategory == null)
            {
                return NotFound();
            }

            return View(exerciseCategory);
        }

        // POST: ExerciseCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exerciseCategory = await _context.ExerciseCategories.FindAsync(id);
            if (exerciseCategory != null)
            {
                _context.ExerciseCategories.Remove(exerciseCategory);
            }

            await _context.SaveChangesAsync();
            return Redirect(ViewData["returnURL"].ToString());
        }

        private bool ExerciseCategoryExists(int id)
        {
            return _context.ExerciseCategories.Any(e => e.FitnessCategoryID == id);
        }
    }
}
