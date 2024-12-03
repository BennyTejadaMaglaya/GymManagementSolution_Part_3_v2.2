using GymManagement.CustomControllers;
using GymManagement.Data;
using GymManagement.Models;
using GymManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Controllers
{
    public class FitnessCategoryController : CognizantController
    {
        private readonly GymContext _context;

        public FitnessCategoryController(GymContext context)
        {
            _context = context;
        }

        // GET: FitnessCategory
        public async Task<IActionResult> Index()
        {
            var fitnessCategories = await _context.FitnessCategories
                .Include(c=>c.ExerciseCategories)
                .AsNoTracking()
                .ToListAsync();
            return View(fitnessCategories);
        }

        // GET: FitnessCategory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessCategory = await _context.FitnessCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (fitnessCategory == null)
            {
                return NotFound();
            }

            return View(fitnessCategory);
        }

        // GET: FitnessCategory/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FitnessCategory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Category")] FitnessCategory fitnessCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(fitnessCategory);
                    await _context.SaveChangesAsync();
                    var returnUrl = ViewData["returnURL"]?.ToString();
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    return Redirect(returnUrl);
                }
            }
            catch (DbUpdateException dex)
            {
                ExceptionMessageVM msg = new();
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                {
                    msg.ErrProperty = "Category";
                    msg.ErrMessage = "Unable to save changes. Remember, you cannot have duplicate Categories.";
                }
                ModelState.AddModelError(msg.ErrProperty, msg.ErrMessage);
            }

            return View(fitnessCategory);
        }

        // GET: FitnessCategory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessCategory = await _context.FitnessCategories.FindAsync(id);
            if (fitnessCategory == null)
            {
                return NotFound();
            }
            return View(fitnessCategory);
        }

        // POST: FitnessCategory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            //Go get the Doctor to update
            var fitnessCategoryToUpdate = await _context.FitnessCategories
                .FirstOrDefaultAsync(p => p.ID == id);

            //Check that you got it or exit with a not found error
            if (fitnessCategoryToUpdate == null)
            {
                return NotFound();
            }

            //Try updating it with the values posted
            if (await TryUpdateModelAsync<FitnessCategory>(fitnessCategoryToUpdate, "",
                d => d.Category))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    var returnUrl = ViewData["returnURL"]?.ToString();
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    return Redirect(returnUrl);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FitnessCategoryExists(fitnessCategoryToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException dex)
                {
                    ExceptionMessageVM msg = new();
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                    {
                        msg.ErrProperty = "Category";
                        msg.ErrMessage = "Unable to save changes. Remember, you cannot have duplicate Categories.";
                    }
                    ModelState.AddModelError(msg.ErrProperty, msg.ErrMessage);
                }

            }
            return View(fitnessCategoryToUpdate);
        }

        // GET: FitnessCategory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessCategory = await _context.FitnessCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (fitnessCategory == null)
            {
                return NotFound();
            }

            return View(fitnessCategory);
        }

        // POST: FitnessCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fitnessCategory = await _context.FitnessCategories.FindAsync(id);
            try
            {
                if (fitnessCategory != null)
                {
                    _context.FitnessCategories.Remove(fitnessCategory);
                }

                await _context.SaveChangesAsync();
                var returnUrl = ViewData["returnURL"]?.ToString();
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction(nameof(Index));
                }
                return Redirect(returnUrl);
            }
            catch (DbUpdateException dex)
            {
                ExceptionMessageVM msg = new();
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    msg.ErrProperty = "";
                    msg.ErrMessage = "Unable to Delete " + ViewData["ControllerFriendlyName"] +
                        ". Remember, you cannot delete a " + ViewData["ControllerFriendlyName"] +
                        " that has related records.";
                }
                ModelState.AddModelError(msg.ErrProperty, msg.ErrMessage);
            }
            return View(fitnessCategory);

        }

        private bool FitnessCategoryExists(int id)
        {
            return _context.FitnessCategories.Any(e => e.ID == id);
        }
    }
}
