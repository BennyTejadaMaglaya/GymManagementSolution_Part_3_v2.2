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
using OfficeOpenXml;
using Microsoft.AspNetCore.Authorization;

namespace GymManagement.Controllers
{
    [Authorize]
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
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(exerciseCategory);
                    await _context.SaveChangesAsync();
                    return Redirect(ViewData["returnURL"].ToString());
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem " +
                    "persists see your system administrator.");
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
            
            try
            {
                if (exerciseCategory != null)
                {
                    _context.ExerciseCategories.Remove(exerciseCategory);
                }

                await _context.SaveChangesAsync();
                return Redirect(ViewData["returnURL"].ToString());
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem " +
                    "persists see your system administrator.");
            }

            return View(exerciseCategory);
        }

        [HttpPost]
        public async Task<IActionResult> InsertFromExcel(IFormFile theExcel)
        {
            string feedBack = string.Empty;
            if (theExcel != null)
            {
                string mimeType = theExcel.ContentType;
                long fileLength = theExcel.Length;
                if (!(mimeType == "" || fileLength == 0))//Looks like we have a file!!!
                {
                    if (mimeType.Contains("excel") || mimeType.Contains("spreadsheet"))
                    {
                        ExcelPackage excel;
                        using (var memoryStream = new MemoryStream())
                        {
                            await theExcel.CopyToAsync(memoryStream);
                            excel = new ExcelPackage(memoryStream);
                        }
                        var workSheet = excel.Workbook.Worksheets[0];
                        var start = workSheet.Dimension.Start;
                        var end = workSheet.Dimension.End;
                        int successCount = 0;
                        int errorCount = 0;
                        if (workSheet.Cells[1, 1].Text == "Exercise" && workSheet.Cells[1, 2].Text == "FitnessCategory")
                        {
                            for (int row = start.Row + 1; row <= end.Row; row++)
                            {
                                ExerciseCategory exerciseCategory = new ExerciseCategory();
                                Exercise exercise = new Exercise();
                                FitnessCategory fitnessCategory = new FitnessCategory();
                                try
                                {
                                    string exerciseName = workSheet.Cells[row, 1].Text.Trim();

                                    string fitnessCategoryName = workSheet.Cells[row, 2].Text.Trim();

                                    exercise = await _context.Exercises
                                        .FirstOrDefaultAsync(e => e.Name == exerciseName);

                                    if (exercise == null)
                                    {
                                        exercise = new Exercise { Name = exerciseName };
                                        _context.Exercises.Add(exercise);
                                        await _context.SaveChangesAsync();
                                    }

                                    fitnessCategory = await _context.FitnessCategories
                                        .FirstOrDefaultAsync(fc => fc.Category == fitnessCategoryName);

                                    if (fitnessCategory == null)
                                    {
                                        fitnessCategory = new FitnessCategory { Category = fitnessCategoryName };
                                        _context.FitnessCategories.Add(fitnessCategory);
                                        await _context.SaveChangesAsync();
                                    }

                                    var existingCombination = await _context.ExerciseCategories
                                        .FirstOrDefaultAsync(ec => ec.ExerciseID == exercise.ID && ec.FitnessCategoryID == fitnessCategory.ID);

                                    if (existingCombination == null)
                                    {
                                        exerciseCategory.Exercise = exercise;
                                        exerciseCategory.FitnessCategory = fitnessCategory;
                                        _context.ExerciseCategories.Add(exerciseCategory);
                                        await _context.SaveChangesAsync();
                                        successCount++;
                                    }
                                    else
                                    {
                                        feedBack += "Warning: The Exercise \"" + exercise.Name + "\" is already associated with the category \"" + fitnessCategory.Category + "\".<br />";
                                        errorCount++;
                                    }
                                }
                                catch (DbUpdateException dex)
                                {
                                    errorCount++;
                                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                                    {
                                        feedBack += "Error: Record " + exercise.Name +
                                            " was rejected as a duplicate." + "<br />";
                                    }
                                    else
                                    {
                                        feedBack += "Error: Record " + exercise.Name +
                                            " caused a database error." + "<br />";
                                    }
                                    //Here is the trick to using SaveChanges in a loop.  You must remove the 
                                    //offending object from the cue or it will keep raising the same error.
                                    _context.Remove(exerciseCategory);
                                }
                                catch (Exception ex)
                                {
                                    errorCount++;
                                    if (ex.GetBaseException().Message.Contains("correct format"))
                                    {
                                        feedBack += "Error: Record " + exercise.Name
                                            + " was rejected becuase it was not in the correct format." + "<br />";
                                    }
                                    else
                                    {
                                        feedBack += "Error: Record " + exercise.Name
                                            + " caused and error." + "<br />";
                                    }
                                }
                            }
                            feedBack += "Finished Importing " + (successCount + errorCount).ToString() +
                                " Records with " + successCount.ToString() + " inserted and " +
                                errorCount.ToString() + " rejected";
                        }
                        else
                        {
                            feedBack = "Error: You may have selected the wrong file to upload.<br /> " +
                                "Remember, you must have the heading 'Exercise' in the " +
                                "first cell of the first row and 'FitnessCategory' in the first cell of the second column.";
                        }
                    }
                    else
                    {
                        feedBack = "Error: That file is not an Excel spreadsheet.";
                    }
                }
                else
                {
                    feedBack = "Error:  file appears to be empty";
                }
            }
            else
            {
                feedBack = "Error: No file uploaded";
            }

            TempData["Feedback"] = feedBack + "<br /><br />";

            //Note that we are assuming that you are using the Preferred Approach to Lookup Values
            //And the custom LookupsController
            return Redirect(ViewData["returnURL"].ToString());
        }

        private bool ExerciseCategoryExists(int id)
        {
            return _context.ExerciseCategories.Any(e => e.FitnessCategoryID == id);
        }
    }
}
