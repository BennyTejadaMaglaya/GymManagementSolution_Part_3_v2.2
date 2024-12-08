using GymManagement.CustomControllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GymManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace GymManagement.Controllers
{
    [Authorize]
    public class LookupController : CognizantController
    {
        private readonly GymContext _context;

        public LookupController(GymContext context)
        {
            _context = context;
        }

        public IActionResult Index(string Tab = "Exercise-Tab")
        {
            //Note: select the tab you want to load by passing in
            //the ID of the tab such as FitnessCategory-Tab,
            //or ExerciseCategory-Tab
            ViewData["Tab"] = Tab;

            return View();
        }

        public PartialViewResult Exercise()
        {
            ViewData["ExerciseID"] = new
                SelectList(_context.Exercises
                .OrderBy(e => e.ID), "ID", "Name");
            return PartialView("_Exercise");
        }

        public PartialViewResult ExerciseCategory()
        {
            ViewData["ExerciseID"] = new
                SelectList(_context.ExerciseCategories
                .Include(ec => ec.Exercise)
                .Include(ec => ec.FitnessCategory)
                .OrderBy(ec => ec.Exercise != null ? ec.Exercise.Name : string.Empty)
                .GroupBy(ec => ec.ExerciseID)
                .Select(group => new
                {
                    ExerciseID = group.Key,
                    ExerciseName = group.FirstOrDefault().Exercise != null
                            ? group.FirstOrDefault().Exercise.Name + " | " + string.Join(", ", group.Select(ec => ec.FitnessCategory != null ? ec.FitnessCategory.Category : "Unassigned"))
                            : "Unassigned"
                }),
                "ExerciseID",
                "ExerciseName"
            );
            return PartialView("_ExerciseCategory");
        }
    }
}
